package com.pronto.doc.sx.word;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import com.jacob.activeX.ActiveXComponent;
import com.jacob.com.ComFailException;
import com.jacob.com.Dispatch;
import com.jacob.com.Variant;


public class WordHelper {
    /**
     * ��ȡCom�ӿ��쳣��������Դ���.
     */
    private static final int MAX_RETRY = 10;
    /**
     * word�ĵ�.
     */
    private Dispatch doc;
    /**
     * word���г������.
     */
    private ActiveXComponent wordApp = null;
    /**
     * ѡ���ķ�Χ������.
     */
    private Dispatch selection;
    /**
     * �˳�ʱ�Ƿ񱣴��ĵ�.
     */
    private boolean saveOnExit = false;
 
    /**
     * ���캯��.
     * @param show �Ƿ�ɼ�.
     */
    public WordHelper(final boolean show) {
        if (wordApp == null) {
            wordApp = new ActiveXComponent("Word.Application");
            wordApp.setProperty("Visible", new Variant(show));
        }
    }
 
    /**
     * �����˳�ʱ����.
     * @param b boolean true-�˳�ʱ�����ļ���false-�˳�ʱ�������ļ�
     */
    public void setSaveOnExit(
            final boolean b) {
        this.saveOnExit = b;
    }
 
    /**
     * ��ѡ�������ݻ����������ƶ�.
     * @param pos �ƶ��ľ���
     */
    public void moveUp(
            final int pos) {
 
        if (selection == null) {
            selection = Dispatch.get(wordApp, "Selection").toDispatch();
        }
 
        for (int i = 0; i < pos; i++) {
            Dispatch.call(selection, "MoveUp");
        }
    }
 
    /**
     * ��ѡ�������ݻ��߲���������ƶ�.
     * @param pos �ƶ��ľ���
     */
    public void moveDown(
            final int pos) {
 
        if (selection == null) {
            selection = Dispatch.get(wordApp, "Selection").toDispatch();
        }
 
        for (int i = 0; i < pos; i++) {
            Dispatch.call(selection, "MoveDown");
        }
    }
 
    /**
     * ��ѡ�������ݻ��߲���������ƶ�.
     * @param pos �ƶ��ľ���
     */
    public void moveLeft(
            final int pos) {
 
        if (selection == null) {
            selection = Dispatch.get(wordApp, "Selection").toDispatch();
        }
 
        for (int i = 0; i < pos; i++) {
            Dispatch.call(selection, "MoveLeft");
        }
    }
 
    /**
     * ��ѡ�������ݻ��߲���������ƶ�.
     * @param pos �ƶ��ľ���
     */
    public void moveRight(
            final int pos) {
 
        if (selection == null) {
            selection = Dispatch.get(wordApp, "Selection").toDispatch();
        }
 
        for (int i = 0; i < pos; i++) {
            Dispatch.call(selection, "MoveRight");
        }
    }
 
    /**
     * �Ѳ�����ƶ����ļ���λ��.
     */
    public void moveStart() {
        if (selection == null) {
            selection = Dispatch.get(wordApp, "Selection").toDispatch();
        }
 
        Dispatch.call(selection, "HomeKey", new Variant(6));
    }
 
    /**
     * �Ѳ�����ƶ����ļ�βλ��.
     */
    public void moveEnd() {
        if (selection == null) {
            selection = Dispatch.get(wordApp, "Selection").toDispatch();
        }
 
        Dispatch.call(selection, "EndKey", new Variant(6));
    }
 
    /**
     * ��������.
     * @param pos ������
     */
    public void listIndent(
            final int pos) {
 
        Dispatch range = Dispatch.get(this.selection, "Range").toDispatch();
        Dispatch listFormat = Dispatch.get(range, "ListFormat").toDispatch();
        for (int i = 0; i < pos; i++) {
            Dispatch.call(listFormat, "ListIndent");
        }
    }
 
    /**
     * ��������.
     * @param pos ������
     */
    public void listOutdent(
            final int pos) {
 
        Dispatch range = Dispatch.get(this.selection, "Range").toDispatch();
        Dispatch listFormat = Dispatch.get(range, "ListFormat").toDispatch();
        for (int i = 0; i < pos; i++) {
            Dispatch.call(listFormat, "ListOutdent");
        }
    }
 
    /**
     * �س�����.
     */
    public void enter() {
        int index = 1;
        while (true) {
            try {
                Dispatch.call(this.selection, "TypeParagraph");
                break;
            } catch (ComFailException e) {
                if (index++ >= MAX_RETRY) {
                    throw e;
                } else {
                    continue;
                }
            }
        }
    }
 
    /**
     * ����һ����ҳ��.
     */
    public void insertPageBreak() {
        Dispatch.call(this.selection, "InsertBreak", new Variant(2));
    }
 
    /**
     * ����word�ĵ��Ƿ�ɼ�.
     * @param isVisible �Ƿ�ɼ�
     */
    public void setIsVisible(
            final boolean isVisible) {
 
        wordApp.setProperty("Visible", new Variant(isVisible));
    }
 
    /**
     * �ж��ĵ��Ƿ����.
     * @param docName �ĵ�����.
     * @return boolean �Ƿ����.
     */
    private boolean isExist(
            final String docName) {
 
        boolean result = false;
        File file = new File(docName);
        result = file.exists();
        file = null;
        return result;
    }
 
    /**
     * ��ȡ�ļ�����.
     * @param docName �ĵ�·��.
     * @return �ļ�����
     */
    public String getFileName(
            final String docName) {
 
        int pos = docName.lastIndexOf("\\");
        return docName.substring(pos + 1);
    }
 
    /**
     * ���ĵ�.
     * @param docName �ĵ�·��.
     * @throws Exception �쳣
     */
    public void openDocument(
            final String docName)
    throws Exception {
 
        Dispatch docs = wordApp.getProperty("Documents").toDispatch();
 
        if (isExist(docName)) {
            this.closeDocument();
            doc = Dispatch.call(docs, "Open", docName).toDispatch();
        } else {
            wordApp.invoke("Quit", new Variant[] {});
            new Exception("[Open doc failed]: file["
                    + docName + "] isn't existed!");
        }
 
        selection = Dispatch.get(wordApp, "Selection").toDispatch();
    }
 
    /**
     * ���һ�����ĵ�.
     * @param docName �ĵ�·��.
     * @throws Exception �쳣
     */
    public void newDocument(
            final String docName)
    throws Exception {
 
        try {
            Dispatch docs = wordApp.getProperty("Documents").toDispatch();
            doc = Dispatch.call(docs, "Add").toDispatch();
            selection = Dispatch.get(wordApp, "Selection").toDispatch();
        } catch (com.jacob.com.ComFailException cfe) {
            throw new Exception(cfe.getMessage());
        } catch (com.jacob.com.ComException ce) {
            throw new Exception(ce.getMessage());
        }
    }
 
    /**
     * ����һ������.
     * @param textToInsert ����
     * @param style ��ʽ
     */
    public void insertText(
            final String textToInsert,
            final String style) {
 
        Dispatch.put(selection, "Text", textToInsert);
        Dispatch.put(selection, "Style", getOutlineStyle(style));
        Dispatch.call(selection, "MoveRight");
    }
 
    /**
     * ����һ��ͼƬ.
     * @param imagePath ͼƬ·��.
     * @param style ͼƬ��ʽ
     */
    public void insertImage(
            final String imagePath,
            final String style) {
 
        Dispatch.call(Dispatch.get(selection, "InLineShapes")
                .toDispatch(), "AddPicture", imagePath);
 
        Dispatch.call(selection, "MoveRight");
        Dispatch.put(selection, "Style", getOutlineStyle(style));
        this.enter();
    }
 
    /**
     * ��ȡ��Ӧ���Ƶ�Style����.
     * @param style Style����.
     * @return Style����
     */
    public Variant getOutlineStyle(
            final String style) {
 
        int index = 1;
        while (true) {
            try {
                return Dispatch.call(
                        Dispatch.get(this.doc, "Styles").toDispatch(),
                        "Item", new Variant(style));
            } catch (ComFailException e) {
                if (index++ >= MAX_RETRY) {
                    throw e;
                } else {
                    continue;
                }
            }
        }
    }
 
    /**
     * �������.
     * @param text ��������.
     * @param style ���ñ��������
     */
    public void insertOutline(
            final String text,
            final String style) {
 
        this.insertText(text, style);
        this.enter();
    }
 
    /**
     * ����Ŀ¼.
     * tablesOfContents�Ĳ����ĺ��� Add(Range As Range, [UseHeadingStyles],
     * [UpperHeadingLevel], [LowerHeadingLevel], [UseFields], [TableID],
     * --������Ҫ��Ҫ������ [RightAlignPageNumbers],[IncludePageNumbers], [AddedStyles],
     * --�������������ֵ,����������,���������,��com.jacob.com.ComFailException
     * [UseHyperlinks],[HidePageNumbersInWeb], [UseOutlineLevels])
     */
    public void insertTablesOfContents() {
        Dispatch tablesOfContents = Dispatch.get(this.doc, "TablesOfContents")
                .toDispatch();
 
        Dispatch range = Dispatch.get(this.selection, "Range").toDispatch();
        // Dispatch.call�еĲ��������9��,�������9��,����Dispatch.callN����Dispathc.invoke
        /*
         * Dispatch.invoke(tablesOfContents, "Add", Dispatch.Method,new
         * Object[]{range,new Variant(true),new Variant(1), new Variant(3),new
         * Variant(true), new Variant(true),new Variant(true) ,new
         * Variant("1"),new Variant(true),new Variant(true)},new int[10]);
         */
        Dispatch.callN(tablesOfContents, "Add", new Object[] {
                range,
                new Variant(true),
                new Variant(1),
                new Variant(3),
                new Variant(false),
                new Variant(true),
                new Variant(true),
                new Variant("1"),
                new Variant(true),
                new Variant(true)});
    }
 
    /**
     * ��ѡ�����ݻ����㿪ʼ�����ı�.
     * @param toFindText Ҫ���ҵ��ı�
     * @return boolean true-���ҵ���ѡ�и��ı���false-δ���ҵ��ı�
     */
    public boolean find(
            final String toFindText) {
 
        if (toFindText == null
                || toFindText.equals("")) {
            return false;
        }
 
        // ��selection����λ�ÿ�ʼ��ѯ
        Dispatch find = Dispatch.call(selection, "Find").toDispatch();
        // ����Ҫ���ҵ�����
        Dispatch.put(find, "Text", toFindText);
        // ��ǰ����
        Dispatch.put(find, "Forward", "True");
        // ���ø�ʽ
        Dispatch.put(find, "Format", "True");
        // ��Сдƥ��
        Dispatch.put(find, "MatchCase", "True");
        // ȫ��ƥ��
        Dispatch.put(find, "MatchWholeWord", "True");
        // ���Ҳ�ѡ��
        return Dispatch.call(find, "Execute").getBoolean();
    }
 
    /**
    * ��ѡ��ѡ�������趨Ϊ�滻�ı�.
    * @param toFindText �����ַ���
    * @param newText Ҫ�滻������
    * @return boolean true-���ҵ���ѡ�и��ı���false-δ���ҵ��ı�
    */
    public boolean replaceText(
            final String toFindText,
            final String newText) {
 
        if (!find(toFindText)) {
            return false;
        }
 
        Dispatch.put(selection, "Text", newText);
        return true;
    }
 
    /**
     * �������.
     * @param numCols ����
     * @param numRows ����
     * @param autoFormat Ĭ�ϸ�ʽ
     * @return ������
     */
    public Dispatch createTable(
            final int numRows,
            final int numCols,
            final int autoFormat) {
 
        int index = 1;
        while (true) {
            try {
                Dispatch tables = Dispatch.get(doc, "Tables").toDispatch();
                Dispatch range = Dispatch.get(selection, "Range").toDispatch();
                Dispatch newTable = Dispatch.call(tables, "Add", range,
                        new Variant(numRows),
                        new Variant(numCols)).toDispatch();
 
                Dispatch.call(selection, "MoveRight");
                Dispatch.call(newTable, "AutoFormat", new Variant(autoFormat));
                return newTable;
            } catch (ComFailException e) {
                if (index++ >= MAX_RETRY) {
                    throw e;
                } else {
                    continue;
                }
            }
        }
    }
 
    /**
     * ��ָ���ı�ͷ����д����.
     * @param table ���
     * @param cellColIdx �к�
     * @param txt ����
     * @param style ��ʽ
     */
    public void putTableHeader(
            final Dispatch table,
            final int cellColIdx,
            final String txt,
            final String style) {
 
        Dispatch cell = Dispatch.call(table, "Cell", new Variant(1),
                new Variant(cellColIdx)).toDispatch();
 
        Dispatch.call(cell, "Select");
        Dispatch.put(selection, "Text", txt);
        Dispatch.put(this.selection, "Style", getOutlineStyle(style));
    }
 
    /**
     * ��ָ���ĵ�Ԫ������д����.
     * @param table ���
     * @param cellRowIdx �к�
     * @param cellColIdx �к�
     * @param txt ����
     * @param style ��ʽ
     */
    public void putTableCell(
            final Dispatch table,
            final int cellRowIdx,
            final int cellColIdx,
            final String txt,
            final String style) {
 
        Dispatch cell = Dispatch.call(table, "Cell", new Variant(cellRowIdx),
                new Variant(cellColIdx)).toDispatch();
 
        Dispatch.call(cell, "Select");
        Dispatch.put(selection, "Text", txt);
        Dispatch.put(this.selection, "Style", getOutlineStyle(style));
    }
 
    /**
     * �رյ�ǰword�ĵ�.
     */
    public void closeDocument() {
        if (doc != null) {
//            Dispatch.call(doc, "Save");
            Dispatch.call(doc, "Close", new Variant(saveOnExit));
            doc = null;
        }
    }
 
    /**
     * �ļ���������Ϊ.
     * @param savePath ��������Ϊ·��
     */
    public void saveFileAs(
            final String savePath) {
 
        Dispatch.call(doc, "SaveAs", savePath);
    }
    
    /**
     * �����pdf.
     * @param pdfFileName
     */
    public void saveAsPdf(final String pdfFileName){
    	Dispatch.invoke(doc, "ExportAsFixedFormat", Dispatch.Method, new Object[]{pdfFileName, new Variant(17)}, new int[1]);
    }
    
    public void saveAsMht(final String mhtFileName){
    	Dispatch.invoke(doc, "SaveAs", Dispatch.Method, new Object[]{mhtFileName, new Variant(9)}, new int[1]);
    }
    
    /**
     * �ĵ�����ˮӡ
     *  
     * @param waterMarkStr ˮӡ�ַ���
     */
     public void setWaterMark(String waterMarkStr)
     {
     // ȡ�û�������
     Dispatch activePan = Dispatch.get(wordApp.getProperty("ActiveWindow").toDispatch(), "ActivePane")
     .toDispatch();
     // ȡ���Ӵ�����
     Dispatch view = Dispatch.get(activePan, "View").toDispatch();
     //����ҳü����
     Dispatch.put(view, "SeekView", new Variant(9));
     Dispatch headfooter = Dispatch.get(selection, "HeaderFooter")
     .toDispatch();
     //ȡ��ͼ�ζ���
     Dispatch shapes = Dispatch.get(headfooter, "Shapes").toDispatch();
     //���ĵ�ȫ������ˮӡ
     Dispatch wm_selection = Dispatch.call(shapes, "AddTextEffect",
     new Variant(0), waterMarkStr, "����", new Variant(1),
     new Variant(false), new Variant(false), new Variant(0),
     new Variant(0)).toDispatch();
     Dispatch.call(wm_selection, "Select");
     //����ˮӡ����
     Dispatch shapeRange = Dispatch.get(selection, "ShapeRange")
     .toDispatch();
     Dispatch.put(shapeRange, "Name", "PowerPlusWaterMarkObject1");
     Dispatch textEffect = Dispatch.get(shapeRange, "TextEffect").toDispatch();
     Dispatch.put(textEffect, "NormalizedHeight", new Boolean(false));
     Dispatch line = Dispatch.get(shapeRange, "Line").toDispatch();
     Dispatch.put(line, "Visible", new Boolean(false));
     Dispatch fill = Dispatch.get(shapeRange, "Fill").toDispatch();
     Dispatch.put(fill, "Visible", new Boolean(true));
     //����ˮӡ͸����
     Dispatch.put(fill, "Transparency", new Variant(0.5));
     Dispatch foreColor = Dispatch.get(fill, "ForeColor").toDispatch();
     //����ˮӡ��ɫ
     Dispatch.put(foreColor, "RGB", new Variant(12632256));
     Dispatch.call(fill, "Solid");
     //����ˮӡ��ת
     Dispatch.put(shapeRange, "Rotation", new Variant(315));
     Dispatch.put(shapeRange, "LockAspectRatio", new Boolean(true));
     Dispatch.put(shapeRange, "Height", new Variant(117.0709));
     Dispatch.put(shapeRange, "Width", new Variant(468.2835));
     Dispatch.put(shapeRange, "Left", new Variant(-999995));
     Dispatch.put(shapeRange, "Top", new Variant(-999995));
     Dispatch wrapFormat = Dispatch.get(shapeRange, "WrapFormat").toDispatch();
     //�Ƿ�������
     Dispatch.put(wrapFormat, "AllowOverlap", new Variant(true));
     Dispatch.put(wrapFormat, "Side", new Variant(3));
     Dispatch.put(wrapFormat, "Type", new Variant(3));
     Dispatch.put(shapeRange, "RelativeHorizontalPosition", new Variant(0));
     Dispatch.put(shapeRange, "RelativeVerticalPosition", new Variant(0));
     Dispatch.put(view, "SeekView", new Variant(0));
     }
     /**
     * ɾ����ǩ
     * 
     * @param mark  ��ǩ��
     * @param info  ���滻
     * @return
     */
    public boolean deleteBookMark(String markKey, String info) throws Exception{
//        Dispatch bookMarks = wordApp.call(doc, "Bookmarks").toDispatch();
//        boolean isExists = wordApp.call(bookMarks, "Exists", markKey).changeType(Variant.VariantBoolean).getBoolean();
    	Dispatch bookMarks = Dispatch.call(doc, "Bookmarks").toDispatch();
    	boolean isExists = Dispatch.call(bookMarks, "Exists", markKey).changeType(Variant.VariantBoolean).getBoolean();
    	if (isExists) {
            Dispatch n = Dispatch.call(bookMarks, "Item", markKey).toDispatch();
            Dispatch.call(n, "Delete");
            return true;
        } 
        return false;
    }
     /**
     * ������ǩ��������
     * 
     * @param bookMarkKey ��ǩ��
     * @param info  ���������
     * @return
     */
  
    public boolean intoValueBookMark(String bookMarkKey, String info) throws Exception{
//        Dispatch bookMarks = wordApp.call(doc, "Bookmarks").toDispatch();
//        boolean bookMarkExist = wordApp.call(bookMarks, "Exists", bookMarkKey).toBoolean();
    	Dispatch bookMarks = Dispatch.call(doc, "Bookmarks").toDispatch();
    	boolean bookMarkExist = Dispatch.call(bookMarks, "Exists", bookMarkKey).changeType(Variant.VariantBoolean).getBoolean();
        if (bookMarkExist) {
            Dispatch rangeItem = Dispatch.call(bookMarks, "Item", bookMarkKey)
                    .toDispatch();
            Dispatch range = Dispatch.call(rangeItem, "Range").toDispatch();
            Dispatch.put(range, "Text", new Variant(info));
            return true;
        } 
        return false;
    }
    /**
     * ���ȫ���ı�ǩ
     * @return
     * @throws Exception
     */
    public List<Dispatch> getBookmarks() throws Exception{
    	List<Dispatch> result = new ArrayList<Dispatch>();
    	Dispatch bookMarks = Dispatch.call(doc, "Bookmarks").toDispatch();
    	int count = Dispatch.call(bookMarks, "Count").changeType(Variant.VariantInt).getInt();
    	for(int i = 0; i < count; i ++){
    		Dispatch item = Dispatch.call(bookMarks, "Item", i).toDispatch();
    		result.add(item);
    	}
    	return result;
    }
    /**
     * ����ȫ������ǩ
     * @param fontColor
     * @param bgColor
     * @throws Exception
     */
    public void highLightBookmarks(int fontColor, int bgColor) throws Exception{
    	Dispatch bookMarks = Dispatch.call(doc, "Bookmarks").toDispatch();
    	int count = Dispatch.call(bookMarks, "Count").changeType(Variant.VariantInt).getInt();
    	for(int i = 1; i <= count; i ++){
    		Dispatch item = Dispatch.call(bookMarks, "Item", i).toDispatch();
    		Dispatch range = Dispatch.call(item, "Range").toDispatch();
    		Dispatch.put(range, "HighlightColorIndex", new Variant(bgColor));
    		Dispatch font = Dispatch.call(range, "Font").toDispatch();
    		Dispatch.put(font, "Color", new Variant(fontColor));
    	}
    }
    /**
     * �ر��ĵ�.
     */
    public void close() {
        closeDocument();
        if (wordApp != null) {
            Dispatch.call(wordApp, "Quit");
            wordApp = null;
        }
        selection = null;
    }

}
