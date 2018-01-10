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
     * 读取Com接口异常的最多重试次数.
     */
    private static final int MAX_RETRY = 10;
    /**
     * word文档.
     */
    private Dispatch doc;
    /**
     * word运行程序对象.
     */
    private ActiveXComponent wordApp = null;
    /**
     * 选定的范围或插入点.
     */
    private Dispatch selection;
    /**
     * 退出时是否保存文档.
     */
    private boolean saveOnExit = false;
 
    /**
     * 构造函数.
     * @param show 是否可见.
     */
    public WordHelper(final boolean show) {
        if (wordApp == null) {
            wordApp = new ActiveXComponent("Word.Application");
            wordApp.setProperty("Visible", new Variant(show));
        }
    }
 
    /**
     * 设置退出时参数.
     * @param b boolean true-退出时保存文件，false-退出时不保存文件
     */
    public void setSaveOnExit(
            final boolean b) {
        this.saveOnExit = b;
    }
 
    /**
     * 把选定的内容或插入点向上移动.
     * @param pos 移动的距离
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
     * 把选定的内容或者插入点向下移动.
     * @param pos 移动的距离
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
     * 把选定的内容或者插入点向左移动.
     * @param pos 移动的距离
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
     * 把选定的内容或者插入点向右移动.
     * @param pos 移动的距离
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
     * 把插入点移动到文件首位置.
     */
    public void moveStart() {
        if (selection == null) {
            selection = Dispatch.get(wordApp, "Selection").toDispatch();
        }
 
        Dispatch.call(selection, "HomeKey", new Variant(6));
    }
 
    /**
     * 把插入点移动到文件尾位置.
     */
    public void moveEnd() {
        if (selection == null) {
            selection = Dispatch.get(wordApp, "Selection").toDispatch();
        }
 
        Dispatch.call(selection, "EndKey", new Variant(6));
    }
 
    /**
     * 增加缩进.
     * @param pos 缩进量
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
     * 减少缩进.
     * @param pos 缩进量
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
     * 回车换行.
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
     * 插入一个换页符.
     */
    public void insertPageBreak() {
        Dispatch.call(this.selection, "InsertBreak", new Variant(2));
    }
 
    /**
     * 设置word文档是否可见.
     * @param isVisible 是否可见
     */
    public void setIsVisible(
            final boolean isVisible) {
 
        wordApp.setProperty("Visible", new Variant(isVisible));
    }
 
    /**
     * 判断文档是否存在.
     * @param docName 文档名称.
     * @return boolean 是否存在.
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
     * 获取文件名称.
     * @param docName 文档路径.
     * @return 文件名称
     */
    public String getFileName(
            final String docName) {
 
        int pos = docName.lastIndexOf("\\");
        return docName.substring(pos + 1);
    }
 
    /**
     * 打开文档.
     * @param docName 文档路径.
     * @throws Exception 异常
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
     * 添加一个新文档.
     * @param docName 文档路径.
     * @throws Exception 异常
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
     * 插入一段文字.
     * @param textToInsert 文字
     * @param style 样式
     */
    public void insertText(
            final String textToInsert,
            final String style) {
 
        Dispatch.put(selection, "Text", textToInsert);
        Dispatch.put(selection, "Style", getOutlineStyle(style));
        Dispatch.call(selection, "MoveRight");
    }
 
    /**
     * 插入一个图片.
     * @param imagePath 图片路径.
     * @param style 图片样式
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
     * 获取对应名称的Style对象.
     * @param style Style名称.
     * @return Style对象
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
     * 插入标题.
     * @param text 标题文字.
     * @param style 设置标题的类型
     */
    public void insertOutline(
            final String text,
            final String style) {
 
        this.insertText(text, style);
        this.enter();
    }
 
    /**
     * 插入目录.
     * tablesOfContents的参数的含义 Add(Range As Range, [UseHeadingStyles],
     * [UpperHeadingLevel], [LowerHeadingLevel], [UseFields], [TableID],
     * --这两个要不要都可以 [RightAlignPageNumbers],[IncludePageNumbers], [AddedStyles],
     * --这个参数必须有值,必须是数字,如果是其它,则报com.jacob.com.ComFailException
     * [UseHyperlinks],[HidePageNumbersInWeb], [UseOutlineLevels])
     */
    public void insertTablesOfContents() {
        Dispatch tablesOfContents = Dispatch.get(this.doc, "TablesOfContents")
                .toDispatch();
 
        Dispatch range = Dispatch.get(this.selection, "Range").toDispatch();
        // Dispatch.call中的参数最多是9个,如果超过9个,请用Dispatch.callN或者Dispathc.invoke
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
     * 从选定内容或插入点开始查找文本.
     * @param toFindText 要查找的文本
     * @return boolean true-查找到并选中该文本，false-未查找到文本
     */
    public boolean find(
            final String toFindText) {
 
        if (toFindText == null
                || toFindText.equals("")) {
            return false;
        }
 
        // 从selection所在位置开始查询
        Dispatch find = Dispatch.call(selection, "Find").toDispatch();
        // 设置要查找的内容
        Dispatch.put(find, "Text", toFindText);
        // 向前查找
        Dispatch.put(find, "Forward", "True");
        // 设置格式
        Dispatch.put(find, "Format", "True");
        // 大小写匹配
        Dispatch.put(find, "MatchCase", "True");
        // 全字匹配
        Dispatch.put(find, "MatchWholeWord", "True");
        // 查找并选中
        return Dispatch.call(find, "Execute").getBoolean();
    }
 
    /**
    * 把选定选定内容设定为替换文本.
    * @param toFindText 查找字符串
    * @param newText 要替换的内容
    * @return boolean true-查找到并选中该文本，false-未查找到文本
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
     * 创建表格.
     * @param numCols 列数
     * @param numRows 行数
     * @param autoFormat 默认格式
     * @return 表格对象
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
     * 在指定的表头里填写数据.
     * @param table 表格
     * @param cellColIdx 列号
     * @param txt 文字
     * @param style 样式
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
     * 在指定的单元格里填写数据.
     * @param table 表格
     * @param cellRowIdx 行号
     * @param cellColIdx 列号
     * @param txt 文字
     * @param style 样式
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
     * 关闭当前word文档.
     */
    public void closeDocument() {
        if (doc != null) {
//            Dispatch.call(doc, "Save");
            Dispatch.call(doc, "Close", new Variant(saveOnExit));
            doc = null;
        }
    }
 
    /**
     * 文件保存或另存为.
     * @param savePath 保存或另存为路径
     */
    public void saveFileAs(
            final String savePath) {
 
        Dispatch.call(doc, "SaveAs", savePath);
    }
    
    /**
     * 输出到pdf.
     * @param pdfFileName
     */
    public void saveAsPdf(final String pdfFileName){
    	Dispatch.invoke(doc, "ExportAsFixedFormat", Dispatch.Method, new Object[]{pdfFileName, new Variant(17)}, new int[1]);
    }
    
    public void saveAsMht(final String mhtFileName){
    	Dispatch.invoke(doc, "SaveAs", Dispatch.Method, new Object[]{mhtFileName, new Variant(9)}, new int[1]);
    }
    
    /**
     * 文档设置水印
     *  
     * @param waterMarkStr 水印字符串
     */
     public void setWaterMark(String waterMarkStr)
     {
     // 取得活动窗格对象
     Dispatch activePan = Dispatch.get(wordApp.getProperty("ActiveWindow").toDispatch(), "ActivePane")
     .toDispatch();
     // 取得视窗对象
     Dispatch view = Dispatch.get(activePan, "View").toDispatch();
     //输入页眉内容
     Dispatch.put(view, "SeekView", new Variant(9));
     Dispatch headfooter = Dispatch.get(selection, "HeaderFooter")
     .toDispatch();
     //取得图形对象
     Dispatch shapes = Dispatch.get(headfooter, "Shapes").toDispatch();
     //给文档全部加上水印
     Dispatch wm_selection = Dispatch.call(shapes, "AddTextEffect",
     new Variant(0), waterMarkStr, "宋体", new Variant(1),
     new Variant(false), new Variant(false), new Variant(0),
     new Variant(0)).toDispatch();
     Dispatch.call(wm_selection, "Select");
     //设置水印参数
     Dispatch shapeRange = Dispatch.get(selection, "ShapeRange")
     .toDispatch();
     Dispatch.put(shapeRange, "Name", "PowerPlusWaterMarkObject1");
     Dispatch textEffect = Dispatch.get(shapeRange, "TextEffect").toDispatch();
     Dispatch.put(textEffect, "NormalizedHeight", new Boolean(false));
     Dispatch line = Dispatch.get(shapeRange, "Line").toDispatch();
     Dispatch.put(line, "Visible", new Boolean(false));
     Dispatch fill = Dispatch.get(shapeRange, "Fill").toDispatch();
     Dispatch.put(fill, "Visible", new Boolean(true));
     //设置水印透明度
     Dispatch.put(fill, "Transparency", new Variant(0.5));
     Dispatch foreColor = Dispatch.get(fill, "ForeColor").toDispatch();
     //设置水印颜色
     Dispatch.put(foreColor, "RGB", new Variant(12632256));
     Dispatch.call(fill, "Solid");
     //设置水印旋转
     Dispatch.put(shapeRange, "Rotation", new Variant(315));
     Dispatch.put(shapeRange, "LockAspectRatio", new Boolean(true));
     Dispatch.put(shapeRange, "Height", new Variant(117.0709));
     Dispatch.put(shapeRange, "Width", new Variant(468.2835));
     Dispatch.put(shapeRange, "Left", new Variant(-999995));
     Dispatch.put(shapeRange, "Top", new Variant(-999995));
     Dispatch wrapFormat = Dispatch.get(shapeRange, "WrapFormat").toDispatch();
     //是否允许交叠
     Dispatch.put(wrapFormat, "AllowOverlap", new Variant(true));
     Dispatch.put(wrapFormat, "Side", new Variant(3));
     Dispatch.put(wrapFormat, "Type", new Variant(3));
     Dispatch.put(shapeRange, "RelativeHorizontalPosition", new Variant(0));
     Dispatch.put(shapeRange, "RelativeVerticalPosition", new Variant(0));
     Dispatch.put(view, "SeekView", new Variant(0));
     }
     /**
     * 删除书签
     * 
     * @param mark  书签名
     * @param info  可替换
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
     * 根据书签插入数据
     * 
     * @param bookMarkKey 书签名
     * @param info  插入的数据
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
     * 获得全部的标签
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
     * 高亮全部的书签
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
     * 关闭文档.
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
