﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32;
using Microsoft.Office.Interop.Word;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;

namespace OfficeOperate
{
    public class WordOperate
    {

        #region 新建Word文档
        /// <summary>
        /// 动态生成Word文档并填充内容 
        /// </summary>
        /// <param name="dir">文档目录</param>
        /// <param name="fileName">文档名</param>
        /// <returns>返回自定义信息</returns>
        public static bool CreateWordFile(string dir, string fileName)
        {
            try
            {
                Object oMissing = System.Reflection.Missing.Value;

                if (!Directory.Exists(dir))
                {
                    //创建文件所在目录
                    Directory.CreateDirectory(dir);
                }
                //创建Word文档(Microsoft.Office.Interop.Word)
                Microsoft.Office.Interop.Word._Application WordApp = new Application();
                WordApp.Visible = true;
                Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Add(
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                                
                //保存
                object filename = dir + fileName;
                WordDoc.SaveAs(ref filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, 
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        #endregion 新建Word文档

        public static void TestMht()
        {
            Microsoft.Office.Interop.Word._Application WordApp = new Application();
            WordApp.Visible = true;
            object filename = "E:\\ProntoDir\\CompareXml2003and2007format\\2007 Format.xml";
            Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Open(ref filename);
            WordDoc.SaveAs("E:\\ProntoDir\\CompareXml2003and2007format\\2007 Format.mht", WdSaveFormat.wdFormatWebArchive);
        }

        public static void getPictures(String filePath)
        {
            Microsoft.Office.Interop.Word._Application WordApp = new Application();
            WordApp.Visible = true;
            object filename = filePath;
            Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Open(ref filename);
            Hashtable collections = new Hashtable();
            for (int i = 1; i <= WordDoc.InlineShapes.Count; i++)
            {
                String altText = WordDoc.InlineShapes[i].AlternativeText;
                if(altText.StartsWith("ProntoDocPictureHolder"))
                {
                    collections.Add(altText.Substring(altText.LastIndexOf("%")), "image"+i+".jpg");
                }
            }
        }

        public static void insertPic(String filePath, String picFileName)
        {
            Microsoft.Office.Interop.Word._Application WordApp = new Application();
            WordApp.Visible = true;
            object filename = filePath;
            Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Open(ref filename);
            InlineShape newPic = WordDoc.InlineShapes.AddPicture(picFileName, false, true);
            newPic.AlternativeText = "ProntoPictureHolder%biztag1";
        }

        public static void copyHeaderFooter(String filePath)
        {
            Microsoft.Office.Interop.Word._Application WordApp = new Application();
            WordApp.Visible = true;
            object filename = filePath;
            Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Open(ref filename);
            Microsoft.Office.Interop.Word.WdStatistic stat = Microsoft.Office.Interop.Word.WdStatistic.wdStatisticPages;
            int num = WordDoc.ComputeStatistics(stat);

            WordApp.ActiveWindow.ActivePane.View.Type = WdViewType.wdPrintView;
            WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;
            //WordApp.Selection.HeaderFooter.Range.ExportFragment("e:\\ProntoDir\\header.mht", WdSaveFormat.wdFormatWebArchive);// ExportAsFixedFormat("e:\\ProntoDir\\headerfooter.pdf", WdExportFormat.wdExportFormatPDF);
            WordApp.Selection.HeaderFooter.Range.CopyAsPicture();
            WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument;
            WordApp.Selection.HomeKey(WdUnits.wdStory);
            Microsoft.Office.Interop.Word.Paragraph para = WordDoc.Content.Paragraphs.Add();
            //WordApp.Selection.MoveStart();
            WordApp.Selection.PasteAndFormat(WdRecoveryType.wdPasteDefault);

            WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekCurrentPageFooter;
            WordApp.Selection.HeaderFooter.Range.CopyAsPicture();
            WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument;
            WordApp.Selection.EndKey(WdUnits.wdStory);
            Microsoft.Office.Interop.Word.Paragraph para1 = WordDoc.Content.Paragraphs.Add();
            WordApp.Selection.PasteAndFormat(WdRecoveryType.wdPasteDefault);

            int WaterMarkType = 0;
            System.Collections.IEnumerator shapes = WordApp.ActiveDocument.Shapes.GetEnumerator();
            while (shapes.MoveNext())
            {
                Shape s1 = (Shape)shapes.Current;
                if (s1.Name.StartsWith("PowerPlusWaterMark"))
                {
                    s1.Select();
                    s1.IncrementTop(WordApp.ActiveDocument.PageSetup.TopMargin - WordApp.ActiveDocument.PageSetup.HeaderDistance);
                    WordApp.Selection.Copy();
                    WaterMarkType = 1;//text
                }
                if (s1.Name.StartsWith("WordPictureWatermark"))
                {
                    s1.Select();
                    s1.IncrementTop(WordApp.ActiveDocument.PageSetup.HeaderDistance);
                    WordApp.Selection.Copy();
                    WaterMarkType = 2;//picture
                }
            }
            if (WaterMarkType > 0)
            {
                for (int i = 1; i < num; i++)
                {
                    WordApp.Selection.Paste();
                    System.Console.WriteLine("select:" + WordApp.Selection.GetHashCode());
                    if (WaterMarkType == 1)
                    {
                        WordApp.Selection.ShapeRange.IncrementTop(WordApp.ActiveDocument.PageSetup.PageHeight * i);
                    }
                    else
                    {
                        //WordApp.Selection.Select();
                        WordApp.Selection.MoveDown(WdUnits.wdLine, i*WordApp.ActiveDocument.PageSetup.LinesPage);
                    }
                }
            }
            WordDoc.SaveAs(filePath.Substring(0, filePath.LastIndexOf("."))+".mht", WdSaveFormat.wdFormatWebArchive);
            //WordApp.Selection.HeaderFooter.Range.ExportFragment("e:\\ProntoDir\\foot.mht", WdSaveFormat.wdFormatWebArchive);


            //Microsoft.Office.Interop.Word._Document NewDoc = WordApp.Documents.Add();
            //Microsoft.Office.Interop.Word.Paragraph para;
            ////WordApp.Selection.TypeParagraph();
            //para = WordDoc.Content.Paragraphs.Add();
            //WordApp.Selection.PasteAndFormat(WdRecoveryType.wdPasteDefault);
            //NewDoc.SaveAs("e:\\ProntoDir\\headerfooter.docx");
            //WordDoc.Close();
            //NewDoc.Close();
        }

        #region 给word文档添加页眉页脚
        /// <summary>
        /// 给word文档添加页眉
        /// </summary>
        /// <param name="filePath">文件名</param>
        /// <returns></returns>
        public static bool AddPageHeaderFooter(string filePath)
        {
            try
            {
                Object oMissing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word._Application WordApp = new Application();
                WordApp.Visible = true;
                object filename = filePath;
                Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Open(ref filename, ref oMissing, 
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, 
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                ////添加页眉方法一：
                //WordApp.ActiveWindow.View.Type = WdViewType.wdOutlineView;
                //WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekPrimaryHeader;
                //WordApp.ActiveWindow.ActivePane.Selection.InsertAfter( "**公司" );//页眉内容

                ////添加页眉方法二：
                if (WordApp.ActiveWindow.ActivePane.View.Type == WdViewType.wdNormalView ||
                    WordApp.ActiveWindow.ActivePane.View.Type == WdViewType.wdOutlineView)
                {
                    WordApp.ActiveWindow.ActivePane.View.Type = WdViewType.wdPrintView;
                }
                WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;
                WordApp.Selection.HeaderFooter.LinkToPrevious = false;
                WordApp.Selection.HeaderFooter.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                WordApp.Selection.HeaderFooter.Range.Text = "页眉内容";
                
                WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekCurrentPageFooter;
                WordApp.Selection.HeaderFooter.LinkToPrevious = false;
                WordApp.Selection.HeaderFooter.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                WordApp.ActiveWindow.ActivePane.Selection.InsertAfter("页脚内容");

                //跳出页眉页脚设置
                WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument;

                //保存
                WordDoc.Save();
                WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        #endregion 给word文档添加页眉页脚

        #region 设置文档格式并添加文本内容
        /// <summary>
        /// 设置文档格式并添加文本内容
        /// </summary>
        /// <param name="filePath">文件名</param>
        /// <returns></returns>
        public static bool AddContent(string filePath)
        {
            try
            {
                Object oMissing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word._Application WordApp = new Application();
                WordApp.Visible = true;
                object filename = filePath;
                Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Open(ref filename, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //设置居左
                WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;

                //设置文档的行间距
                WordApp.Selection.ParagraphFormat.LineSpacing = 15f;
                //插入段落
                //WordApp.Selection.TypeParagraph();
                Microsoft.Office.Interop.Word.Paragraph para;
                para = WordDoc.Content.Paragraphs.Add(ref oMissing);
                //正常格式
                para.Range.Text = "This is paragraph 1";
                //para.Range.Font.Bold = 2;
                //para.Range.Font.Color = WdColor.wdColorRed;
                //para.Range.Font.Italic = 2;
                para.Range.InsertParagraphAfter();

                para.Range.Text = "This is paragraph 2";
                para.Range.InsertParagraphAfter();

                //插入Hyperlink
                Microsoft.Office.Interop.Word.Selection mySelection = WordApp.ActiveWindow.Selection;
                mySelection.Start = 9999;
                mySelection.End = 9999;
                Microsoft.Office.Interop.Word.Range myRange = mySelection.Range;
                                
                Microsoft.Office.Interop.Word.Hyperlinks myLinks = WordDoc.Hyperlinks;
                object linkAddr = @"http://www.cnblogs.com/lantionzy";
                Microsoft.Office.Interop.Word.Hyperlink myLink = myLinks.Add(myRange, ref linkAddr, 
                    ref oMissing);
                WordApp.ActiveWindow.Selection.InsertAfter("\n");
              
                //落款
                WordDoc.Paragraphs.Last.Range.Text = "文档创建时间：" + DateTime.Now.ToString();
                WordDoc.Paragraphs.Last.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;

                //保存
                WordDoc.Save();
                WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        #endregion 设置文档格式并添加文本内容

        #region 文档中添加图片
        /// <summary>
        /// 文档中添加图片
        /// </summary>
        /// <param name="filePath">word文件名</param>
        /// <param name="picPath">picture文件名</param>
        /// <returns></returns>
        public static bool AddPicture(string filePath, string picPath)
        {
            try
            {
                Object oMissing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word._Application WordApp = new Application();
                WordApp.Visible = true;
                object filename = filePath;
                Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Open(ref filename, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //移动光标文档末尾
                object count = WordDoc.Paragraphs.Count;
                object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdParagraph;
                WordApp.Selection.MoveDown(ref WdLine, ref count, ref oMissing);//移动焦点
                WordApp.Selection.TypeParagraph();//插入段落

                object LinkToFile = false;
                object SaveWithDocument = true;
                object Anchor = WordDoc.Application.Selection.Range;
                WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(picPath, ref LinkToFile, ref SaveWithDocument, ref Anchor);

                //保存
                WordDoc.Save();
                WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        #endregion 文档中添加图片

        #region 表格处理（插入表格、设置格式、填充内容）
        /// <summary>
        /// 表格处理
        /// </summary>
        /// <param name="filePath">word文件名</param>
        /// <returns></returns>
        public static bool AddTable(string filePath)
        {
            try
            {
                Object oMissing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word._Application WordApp = new Application();
                WordApp.Visible = true;
                object filename = filePath;
                Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Open(ref filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                //插入表格
                Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables.Add(WordApp.Selection.Range, 12, 3, ref oMissing, ref oMissing);
                //设置表格
                newTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleThickThinLargeGap;
                newTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                newTable.Columns[1].Width = 100f;
                newTable.Columns[2].Width = 220f;
                newTable.Columns[3].Width = 105f;
                
                //填充表格内容
                newTable.Cell(1, 1).Range.Text = "我的简历";
                //设置单元格中字体为粗体
                newTable.Cell(1, 1).Range.Bold = 2;

                //合并单元格
                newTable.Cell(1, 1).Merge(newTable.Cell(1, 3));
                
                //垂直居中
                WordApp.Selection.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                //水平居中
                WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                //填充表格内容
                newTable.Cell(2, 1).Range.Text = "座右铭：...";
                //设置单元格内字体颜色
                newTable.Cell(2, 1).Range.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorDarkBlue;
                //合并单元格
                newTable.Cell(2, 1).Merge(newTable.Cell(2, 3));
                WordApp.Selection.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                //填充表格内容
                newTable.Cell(3, 1).Range.Text = "姓名：";
                newTable.Cell(3, 2).Range.Text = "雷鑫";
                //纵向合并单元格
                newTable.Cell(3, 3).Select();
                //选中一行
                object moveUnit = Microsoft.Office.Interop.Word.WdUnits.wdLine;
                object moveCount = 3;
                object moveExtend = Microsoft.Office.Interop.Word.WdMovementType.wdExtend;
                WordApp.Selection.MoveDown(ref moveUnit, ref moveCount, ref moveExtend);
                WordApp.Selection.Cells.Merge();

                //表格中插入图片
                string pictureFileName = System.IO.Directory.GetCurrentDirectory() + @"\picture.jpg";
                object LinkToFile = false;
                object SaveWithDocument = true;
                object Anchor = WordDoc.Application.Selection.Range;
                WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(pictureFileName, ref LinkToFile, ref SaveWithDocument, ref Anchor);
                //图片宽度
                WordDoc.Application.ActiveDocument.InlineShapes[1].Width = 100f;
                //图片高度
                WordDoc.Application.ActiveDocument.InlineShapes[1].Height = 100f;
                //将图片设置为四周环绕型
                Microsoft.Office.Interop.Word.Shape s = WordDoc.Application.ActiveDocument.InlineShapes[1].ConvertToShape();
                s.WrapFormat.Type = Microsoft.Office.Interop.Word.WdWrapType.wdWrapSquare;

                newTable.Cell(12, 1).Range.Text = "备注：";
                newTable.Cell(12, 1).Merge(newTable.Cell(12, 3));
                //在表格中增加行
                WordDoc.Content.Tables[1].Rows.Add(ref oMissing);

                //保存
                WordDoc.Save();
                WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        #endregion #region 表格处理

        #region 把Word文档转化为Html文件
        /// <summary>
        /// 把Word文档转化为Html文件
        /// </summary>
        /// <param name="wordFileName">word文件名</param>
        /// <param name="htmlFileName">要保存的html文件名</param>
        /// <returns></returns>
        public static bool WordToHtml(string wordFileName, string htmlFileName)
        {
            try
            {
                Object oMissing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word._Application WordApp = new Application();
                WordApp.Visible = true;
                object filename = wordFileName;
                Microsoft.Office.Interop.Word._Document WordDoc = WordApp.Documents.Open(ref filename, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

               // Type wordType = WordApp.GetType();
                // 打开文件
                Type docsType = WordApp.Documents.GetType();
                // 转换格式，另存为
                Type docType = WordDoc.GetType();
                object saveFileName = htmlFileName;
                docType.InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod, null, WordDoc, 
                    new object[] { saveFileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML });

                #region 其它格式：
                ///wdFormatHTML
                ///wdFormatDocument
                ///wdFormatDOSText
                ///wdFormatDOSTextLineBreaks
                ///wdFormatEncodedText
                ///wdFormatRTF
                ///wdFormatTemplate
                ///wdFormatText
                ///wdFormatTextLineBreaks
                ///wdFormatUnicodeText
                // 退出 Word
                //wordType.InvokeMember( "Quit", System.Reflection.BindingFlags.InvokeMethod,
                //    null, WordApp, null );
                #endregion

                //保存
                WordDoc.Save();
                WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        #endregion 把Word文档转化为Html文件
        
        #region word中添加新表
        /// <summary>
        /// word中添加新表
        /// </summary>
        public static void AddTable()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word._Application WordApp;
            Microsoft.Office.Interop.Word._Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.Application();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            object start = 0;
            object end = 0;
            Microsoft.Office.Interop.Word.Range tableLocation = WordDoc.Range(ref start, ref end);
            WordDoc.Tables.Add(tableLocation, 3, 4, ref oMissing, ref oMissing);//3行4列的表
        }
        #endregion word中添加新表

        #region 在表中插入新行

        /// <summary>
        /// 在表中插入新的1行
        /// </summary>
        public static void AddRow()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word._Application WordApp;
            Microsoft.Office.Interop.Word._Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.Application();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            object start = 0;
            object end = 0;
            Microsoft.Office.Interop.Word.Range tableLocation = WordDoc.Range(ref start, ref end);
            WordDoc.Tables.Add(tableLocation, 3, 4, ref oMissing, ref oMissing);

            Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables[1];
            object beforeRow = newTable.Rows[1];
            newTable.Rows.Add(ref beforeRow);
        }
        #endregion

        #region 合并单元格
        /// <summary>
        /// 合并单元格
        /// </summary>
        public static void CombinationCell()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word._Application WordApp;
            Microsoft.Office.Interop.Word._Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.Application();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            object start = 0;
            object end = 0;
            Microsoft.Office.Interop.Word.Range tableLocation = WordDoc.Range(ref start, ref end);
            WordDoc.Tables.Add(tableLocation, 3, 4, ref oMissing, ref oMissing);

            Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables[1];
            object beforeRow = newTable.Rows[1];
            newTable.Rows.Add(ref beforeRow);

            Microsoft.Office.Interop.Word.Cell cell = newTable.Cell(2, 1);//2行1列合并2行2列为一起
            cell.Merge(newTable.Cell(2, 2));
            //cell.Merge( newTable.Cell( 1, 3 ) );
        }
        #endregion 合并单元格

        #region 分离单元格
        /// <summary>
        /// 分离单元格
        /// </summary>
        public static void SeparateCell()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word._Application WordApp;
            Microsoft.Office.Interop.Word._Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.Application();
            WordApp.Visible = true;
            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            object start = 0;
            object end = 0;
            Microsoft.Office.Interop.Word.Range tableLocation = WordDoc.Range(ref start, ref end);
            WordDoc.Tables.Add(tableLocation, 3, 4, ref oMissing, ref oMissing);

            Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables[1];
            object beforeRow = newTable.Rows[1];
            newTable.Rows.Add(ref beforeRow);

            Microsoft.Office.Interop.Word.Cell cell = newTable.Cell(1, 1);
            cell.Merge(newTable.Cell(1, 2));

            object Rownum = 2;
            object Columnnum = 2;
            cell.Split(ref Rownum, ref  Columnnum);
        }
        #endregion 分离单元格

        #region 通过段落控制插入
        /// <summary>
        /// 通过段落控制插入Insert a paragraph at the beginning of the document.
        /// </summary>
        public static void InsertParagraph()
        {
            object oMissing = System.Reflection.Missing.Value;
            //object oEndOfDoc = "\\endofdoc"; 
            //endofdoc is a predefined bookmark

            //Start Word and create a new document.
            Microsoft.Office.Interop.Word._Application WordApp;
            Microsoft.Office.Interop.Word._Document WordDoc;
            WordApp = new Microsoft.Office.Interop.Word.Application();
            WordApp.Visible = true;

            WordDoc = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            //Insert a paragraph at the beginning of the document.
            Microsoft.Office.Interop.Word.Paragraph oPara1;
            oPara1 = WordDoc.Content.Paragraphs.Add(ref oMissing);
            oPara1.Range.Text = "Heading 1";
            oPara1.Range.Font.Bold = 1;
            oPara1.Format.SpaceAfter = 24;    //24 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();
        }
        #endregion 通过段落控制插入

        #region word文档设置及获取光标位置

        /// <summary>
        /// word文档设置及获取光标位置
        /// </summary>
        public static void WordSet()
        {
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word._Application WordApp;
            WordApp = new Microsoft.Office.Interop.Word.Application();

            #region 文档格式设置
            WordApp.ActiveDocument.PageSetup.LineNumbering.Active = 0;//行编号
            WordApp.ActiveDocument.PageSetup.Orientation = Microsoft.Office.Interop.Word.WdOrientation.wdOrientPortrait;//页面方向
            WordApp.ActiveDocument.PageSetup.TopMargin = WordApp.CentimetersToPoints(float.Parse("2.54"));//上页边距
            WordApp.ActiveDocument.PageSetup.BottomMargin = WordApp.CentimetersToPoints(float.Parse("2.54"));//下页边距
            WordApp.ActiveDocument.PageSetup.LeftMargin = WordApp.CentimetersToPoints(float.Parse("3.17"));//左页边距
            WordApp.ActiveDocument.PageSetup.RightMargin = WordApp.CentimetersToPoints(float.Parse("3.17"));//右页边距
            WordApp.ActiveDocument.PageSetup.Gutter = WordApp.CentimetersToPoints(float.Parse("0"));//装订线位置
            WordApp.ActiveDocument.PageSetup.HeaderDistance = WordApp.CentimetersToPoints(float.Parse("1.5"));//页眉
            WordApp.ActiveDocument.PageSetup.FooterDistance = WordApp.CentimetersToPoints(float.Parse("1.75"));//页脚
            WordApp.ActiveDocument.PageSetup.PageWidth = WordApp.CentimetersToPoints(float.Parse("21"));//纸张宽度
            WordApp.ActiveDocument.PageSetup.PageHeight = WordApp.CentimetersToPoints(float.Parse("29.7"));//纸张高度
            WordApp.ActiveDocument.PageSetup.FirstPageTray = Microsoft.Office.Interop.Word.WdPaperTray.wdPrinterDefaultBin;//纸张来源
            WordApp.ActiveDocument.PageSetup.OtherPagesTray = Microsoft.Office.Interop.Word.WdPaperTray.wdPrinterDefaultBin;//纸张来源
            WordApp.ActiveDocument.PageSetup.SectionStart = Microsoft.Office.Interop.Word.WdSectionStart.wdSectionNewPage;//节的起始位置：新建页
            WordApp.ActiveDocument.PageSetup.OddAndEvenPagesHeaderFooter = 0;//页眉页脚-奇偶页不同
            WordApp.ActiveDocument.PageSetup.DifferentFirstPageHeaderFooter = 0;//页眉页脚-首页不同
            WordApp.ActiveDocument.PageSetup.VerticalAlignment = Microsoft.Office.Interop.Word.WdVerticalAlignment.wdAlignVerticalTop;//页面垂直对齐方式
            WordApp.ActiveDocument.PageSetup.SuppressEndnotes = 0;//不隐藏尾注
            WordApp.ActiveDocument.PageSetup.MirrorMargins = 0;//不设置首页的内外边距
            WordApp.ActiveDocument.PageSetup.TwoPagesOnOne = false;//不双面打印
            WordApp.ActiveDocument.PageSetup.BookFoldPrinting = false;//不设置手动双面正面打印
            WordApp.ActiveDocument.PageSetup.BookFoldRevPrinting = false;//不设置手动双面背面打印
            WordApp.ActiveDocument.PageSetup.BookFoldPrintingSheets = 1;//打印默认份数
            WordApp.ActiveDocument.PageSetup.GutterPos = Microsoft.Office.Interop.Word.WdGutterStyle.wdGutterPosLeft;//装订线位于左侧
            WordApp.ActiveDocument.PageSetup.LinesPage = 40;//默认页行数量
            WordApp.ActiveDocument.PageSetup.LayoutMode = Microsoft.Office.Interop.Word.WdLayoutMode.wdLayoutModeLineGrid;//版式模式为“只指定行网格”
            #endregion 文档格式设置

            #region 段落格式设定
            WordApp.Selection.ParagraphFormat.LeftIndent = WordApp.CentimetersToPoints(float.Parse("0"));//左缩进
            WordApp.Selection.ParagraphFormat.RightIndent = WordApp.CentimetersToPoints(float.Parse("0"));//右缩进
            WordApp.Selection.ParagraphFormat.SpaceBefore = float.Parse("0");//段前间距
            WordApp.Selection.ParagraphFormat.SpaceBeforeAuto = 0;//
            WordApp.Selection.ParagraphFormat.SpaceAfter = float.Parse("0");//段后间距
            WordApp.Selection.ParagraphFormat.SpaceAfterAuto = 0;//
            WordApp.Selection.ParagraphFormat.LineSpacingRule = Microsoft.Office.Interop.Word.WdLineSpacing.wdLineSpaceSingle;//单倍行距
            WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphJustify;//段落2端对齐
            WordApp.Selection.ParagraphFormat.WidowControl = 0;//孤行控制
            WordApp.Selection.ParagraphFormat.KeepWithNext = 0;//与下段同页
            WordApp.Selection.ParagraphFormat.KeepTogether = 0;//段中不分页
            WordApp.Selection.ParagraphFormat.PageBreakBefore = 0;//段前分页
            WordApp.Selection.ParagraphFormat.NoLineNumber = 0;//取消行号
            WordApp.Selection.ParagraphFormat.Hyphenation = 1;//取消段字
            WordApp.Selection.ParagraphFormat.FirstLineIndent = WordApp.CentimetersToPoints(float.Parse("0"));//首行缩进
            WordApp.Selection.ParagraphFormat.OutlineLevel = Microsoft.Office.Interop.Word.WdOutlineLevel.wdOutlineLevelBodyText;
            WordApp.Selection.ParagraphFormat.CharacterUnitLeftIndent = float.Parse("0");
            WordApp.Selection.ParagraphFormat.CharacterUnitRightIndent = float.Parse("0");
            WordApp.Selection.ParagraphFormat.CharacterUnitFirstLineIndent = float.Parse("0");
            WordApp.Selection.ParagraphFormat.LineUnitBefore = float.Parse("0");
            WordApp.Selection.ParagraphFormat.LineUnitAfter = float.Parse("0");
            WordApp.Selection.ParagraphFormat.AutoAdjustRightIndent = 1;
            WordApp.Selection.ParagraphFormat.DisableLineHeightGrid = 0;
            WordApp.Selection.ParagraphFormat.FarEastLineBreakControl = 1;
            WordApp.Selection.ParagraphFormat.WordWrap = 1;
            WordApp.Selection.ParagraphFormat.HangingPunctuation = 1;
            WordApp.Selection.ParagraphFormat.HalfWidthPunctuationOnTopOfLine = 0;
            WordApp.Selection.ParagraphFormat.AddSpaceBetweenFarEastAndAlpha = 1;
            WordApp.Selection.ParagraphFormat.AddSpaceBetweenFarEastAndDigit = 1;
            WordApp.Selection.ParagraphFormat.BaseLineAlignment = Microsoft.Office.Interop.Word.WdBaselineAlignment.wdBaselineAlignAuto;
            #endregion 段落格式设定

            #region 字体格式设定
            WordApp.Selection.Font.NameFarEast = "华文中宋";
            WordApp.Selection.Font.NameAscii = "Times New Roman";
            WordApp.Selection.Font.NameOther = "Times New Roman";
            WordApp.Selection.Font.Name = "宋体";
            WordApp.Selection.Font.Size = float.Parse("14");
            WordApp.Selection.Font.Bold = 0;
            WordApp.Selection.Font.Italic = 0;
            WordApp.Selection.Font.Underline = Microsoft.Office.Interop.Word.WdUnderline.wdUnderlineNone;
            WordApp.Selection.Font.UnderlineColor = Microsoft.Office.Interop.Word.WdColor.wdColorAutomatic;
            WordApp.Selection.Font.StrikeThrough = 0;//删除线
            WordApp.Selection.Font.DoubleStrikeThrough = 0;//双删除线
            WordApp.Selection.Font.Outline = 0;//空心
            WordApp.Selection.Font.Emboss = 0;//阳文
            WordApp.Selection.Font.Shadow = 0;//阴影
            WordApp.Selection.Font.Hidden = 0;//隐藏文字
            WordApp.Selection.Font.SmallCaps = 0;//小型大写字母
            WordApp.Selection.Font.AllCaps = 0;//全部大写字母
            WordApp.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorAutomatic;
            WordApp.Selection.Font.Engrave = 0;//阴文
            WordApp.Selection.Font.Superscript = 0;//上标
            WordApp.Selection.Font.Subscript = 0;//下标
            WordApp.Selection.Font.Spacing = float.Parse("0");//字符间距
            WordApp.Selection.Font.Scaling = 100;//字符缩放
            WordApp.Selection.Font.Position = 0;//位置
            WordApp.Selection.Font.Kerning = float.Parse("1");//字体间距调整
            WordApp.Selection.Font.Animation = Microsoft.Office.Interop.Word.WdAnimation.wdAnimationNone;//文字效果
            WordApp.Selection.Font.DisableCharacterSpaceGrid = false;
            WordApp.Selection.Font.EmphasisMark = Microsoft.Office.Interop.Word.WdEmphasisMark.wdEmphasisMarkNone;
            #endregion 字体格式设定

            #region 获取光标位置
            ////get_Information
            WordApp.Selection.get_Information(WdInformation.wdActiveEndPageNumber);
            //关于行号-页号-列号-位置
            //information 属性 
            //返回有关指定的所选内容或区域的信息。variant 类型，只读。 
            //expression.information(type) 
            //expression 必需。该表达式返回一个 range 或 selection 对象。 
            //type long 类型，必需。需要返回的信息。可取下列 wdinformation 常量之一： 
            //wdactiveendadjustedpagenumber 返回页码，在该页中包含指定的所选内容或区域的活动结尾。如果设置了一个起始页码，并对页码进行了手工调整，则返回调整过的页码。 
            //wdactiveendpagenumber 返回页码，在该页中包含指定的所选内容或区域的活动结尾，页码从文档的开头开始计算而不考虑对页码的任何手工调整。 
            //wdactiveendsectionnumber 返回节号，在该节中包含了指定的所选内容或区域的活动结尾。 
            //wdatendofrowmarker 如果指定的所选内容或区域位于表格的行结尾标记处，则本参数返回 true。 
            //wdcapslock 如果大写字母锁定模式有效，则本参数返回 true。 
            //wdendofrangecolumnnumber 返回表格列号，在该表格列中包含了指定的所选内容或区域的活动结尾。 
            //wdendofrangerownumber 返回表格行号，在该表格行包含了指定的所选内容或区域的活动结尾。 
            //wdfirstcharactercolumnnumber 返回指定的所选内容或区域中第一个字符的位置。如果所选内容或区域是折叠的，则返回所选内容或区域右侧紧接着的字符编号。 
            //wdfirstcharacterlinenumber 返回所选内容中第一个字符的行号。如果 pagination 属性为 false，或 draft 属性为 true，则返回 - 1。 
            //wdframeisselected 如果所选内容或区域是一个完整的图文框文本框，则本参数返回 true。 
            //wdheaderfootertype 返回一个值，该值表明包含了指定的所选内容或区域的页眉或页脚的类型，如下表所示。 值 页眉或页脚的类型 
            //- 1 无 
            //0 偶数页页眉 
            //1 奇数页页眉 
            //2 偶数页页脚 
            //3 奇数页页脚 
            //4 第一个页眉 
            //5 第一个页脚 
            //wdhorizontalpositionrelativetopage 返回指定的所选内容或区域的水平位置。该位置是所选内容或区域的左边与页面的左边之间的距离，以磅为单位。如果所选内容或区域不可见，则返回 - 1。 
            //wdhorizontalpositionrelativetotextboundary 返回指定的所选内容或区域相对于周围最近的正文边界的左边的水平位置，以磅为单位。如果所选内容或区域没有显示在当前屏幕，则本参数返回 - 1。 
            //wdinclipboard 有关此常量的详细内容，请参阅 microsoft office 98 macintosh 版的语言参考帮助。 
            //wdincommentpane 如果指定的所选内容或区域位于批注窗格，则返回 true。 
            //wdinendnote 如果指定的所选内容或区域位于页面视图的尾注区内，或者位于普通视图的尾注窗格中，则本参数返回 true。 
            //wdinfootnote 如果指定的所选内容或区域位于页面视图的脚注区内，或者位于普通视图的脚注窗格中，则本参数返回 true。 
            //wdinfootnoteendnotepane 如果指定的所选内容或区域位于页面视图的脚注或尾注区内，或者位于普通视图的脚注或尾注窗格中，则本参数返回 true。详细内容，请参阅前面的 wdinfootnote 和 wdinendnote 的说明。 
            //wdinheaderfooter 如果指定的所选内容或区域位于页眉或页脚窗格中，或者位于页面视图的页眉或页脚中，则本参数返回 true。 
            //wdinmasterdocument 如果指定的所选内容或区域位于主控文档中，则本参数返回 true。 
            //wdinwordmail 返回一个值，该值表明了所选内容或区域的的位置，如下表所示。值 位置 
            //0 所选内容或区域不在一条电子邮件消息中。 
            //1 所选内容或区域位于正在发送的电子邮件中。 
            //2 所选内容或区域位于正在阅读的电子邮件中。 
            //wdmaximumnumberofcolumns 返回所选内容或区域中任何行的最大表格列数。 
            //wdmaximumnumberofrows 返回指定的所选内容或区域中表格的最大行数。 
            //wdnumberofpagesindocument 返回与所选内容或区域相关联的文档的页数。 
            //wdnumlock 如果 num lock 有效，则本参数返回 true。 
            //wdovertype 如果改写模式有效，则本参数返回 true。可用 overtype 属性改变改写模式的状态。 
            //wdreferenceoftype 返回一个值，该值表明所选内容相对于脚注、尾注或批注引用的位置，如下表所示。 值 描述 
            //— 1 所选内容或区域包含、但不只限定于脚注、尾注或批注引用中。 
            //0 所选内容或区域不在脚注、尾注或批注引用之前。 
            //1 所选内容或区域位于脚注引用之前。 
            //2 所选内容或区域位于尾注引用之前。 
            //3 所选内容或区域位于批注引用之前。 
            //wdrevisionmarking 如果修订功能处于活动状态，则本参数返回 true。 
            //wdselectionmode 返回一个值，该值表明当前的选定模式，如下表所示。 值 选定模式 
            //0 常规选定 
            //1 扩展选定 
            //2 列选定 
            //wdstartofrangecolumnnumber 返回所选内容或区域的起点所在的表格的列号。 
            //wdstartofrangerownumber 返回所选内容或区域的起点所在的表格的行号。 
            //wdverticalpositionrelativetopage 返回所选内容或区域的垂直位置，即所选内容的上边与页面的上边之间的距离，以磅为单位。如果所选内容或区域没有显示在屏幕上，则本参数返回 - 1。 
            //wdverticalpositionrelativetotextboundary 返回所选内容或区域相对于周围最近的正文边界的上边的垂直位置，以磅为单位。如果所选内容或区域没有显示在屏幕上，则本参数返回 - 1。 
            //wdwithintable 如果所选内容位于一个表格中，则本参数返回 true。 
            //wdzoompercentage 返回由 percentage 属性设置的当前的放大百分比。
            #endregion 获取光标位置

            #region 光标移动
            //移动光标
            //光标下移3行 上移3行
            object unit = Microsoft.Office.Interop.Word.WdUnits.wdLine;
            object count = 3;
            WordApp.Selection.MoveEnd(ref unit, ref count);
            WordApp.Selection.MoveUp(ref unit, ref count, ref oMissing);

            //Microsoft.Office.Interop.Word.WdUnits说明
            //wdCell                  A cell. 
            //wdCharacter             A character. 
            //wdCharacterFormatting   Character formatting. 
            //wdColumn                A column. 
            //wdItem                  The selected item. 
            //wdLine                  A line. //行
            //wdParagraph             A paragraph. 
            //wdParagraphFormatting   Paragraph formatting. 
            //wdRow                   A row. 
            //wdScreen                The screen dimensions. 
            //wdSection               A section. 
            //wdSentence              A sentence. 
            //wdStory                 A story. 
            //wdTable                 A table. 
            //wdWindow                A window. 
            //wdWord                  A word. 

            //录制的vb宏
            //     ,移动光标至当前行首
            //    Selection.HomeKey unit:=wdLine
            //    '移动光标至当前行尾
            //    Selection.EndKey unit:=wdLine
            //    '选择从光标至当前行首的内容
            //    Selection.HomeKey unit:=wdLine, Extend:=wdExtend
            //    '选择从光标至当前行尾的内容
            //    Selection.EndKey unit:=wdLine, Extend:=wdExtend
            //    '选择当前行
            //    Selection.HomeKey unit:=wdLine
            //    Selection.EndKey unit:=wdLine, Extend:=wdExtend
            //    '移动光标至文档开始
            //    Selection.HomeKey unit:=wdStory
            //    '移动光标至文档结尾
            //    Selection.EndKey unit:=wdStory
            //    '选择从光标至文档开始的内容
            //    Selection.HomeKey unit:=wdStory, Extend:=wdExtend
            //    '选择从光标至文档结尾的内容
            //    Selection.EndKey unit:=wdStory, Extend:=wdExtend
            //    '选择文档全部内容（从WholeStory可猜出Story应是当前文档的意思）
            //    Selection.WholeStory
            //    '移动光标至当前段落的开始
            //    Selection.MoveUp unit:=wdParagraph
            //    '移动光标至当前段落的结尾
            //    Selection.MoveDown unit:=wdParagraph
            //    '选择从光标至当前段落开始的内容
            //    Selection.MoveUp unit:=wdParagraph, Extend:=wdExtend
            //    '选择从光标至当前段落结尾的内容
            //    Selection.MoveDown unit:=wdParagraph, Extend:=wdExtend
            //    '选择光标所在段落的内容
            //    Selection.MoveUp unit:=wdParagraph
            //    Selection.MoveDown unit:=wdParagraph, Extend:=wdExtend
            //    '显示选择区的开始与结束的位置，注意：文档第1个字符的位置是0
            //    MsgBox ("第" & Selection.Start & "个字符至第" & Selection.End & "个字符")
            //    '删除当前行
            //    Selection.HomeKey unit:=wdLine
            //    Selection.EndKey unit:=wdLine, Extend:=wdExtend
            //    Selection.Delete
            //    '删除当前段落
            //    Selection.MoveUp unit:=wdParagraph
            //    Selection.MoveDown unit:=wdParagraph, Extend:=wdExtend
            //    Selection.Delete


            //表格的光标移动
            //光标到当前光标所在表格的地单元格
            WordApp.Selection.Tables[1].Cell(1, 1).Select();
            //unit对象定义
            object unith = Microsoft.Office.Interop.Word.WdUnits.wdRow;//表格行方式
            object extend = Microsoft.Office.Interop.Word.WdMovementType.wdExtend;//extend对光标移动区域进行扩展选择
            object unitu = Microsoft.Office.Interop.Word.WdUnits.wdLine;//文档行方式,可以看成表格一行.不过和wdRow有区别
            object unitp = Microsoft.Office.Interop.Word.WdUnits.wdParagraph;//段落方式,对于表格可以选择到表格行后的换车符,对于跨行合并的行选择,我能找到的最简单方式
            //object count = 1;//光标移动量

            #endregion 光标移动
        }
        #endregion word文档设置及获取光标位置

        #region 读取Word表格中某个单元格的数据。其中的参数分别为文件名（包括路径），行号，列号。

        /// <summary>
        /// 读取Word表格中某个单元格的数据。其中的参数分别为文件名（包括路径），行号，列号。
        /// </summary>
        /// <param name="fileName">word文档</param>
        /// <param name="rowIndex">行</param>
        /// <param name="colIndex">列</param>
        /// <returns>返回数据</returns>
        public static string ReadWord_tableContentByCell(string fileName, int rowIndex, int colIndex)
        {
            Microsoft.Office.Interop.Word._Application cls = null;
            Microsoft.Office.Interop.Word._Document doc = null;
            Microsoft.Office.Interop.Word.Table table = null;
            object missing = System.Reflection.Missing.Value;
            object path = fileName;
            cls = new Application();
            try
            {
                doc = cls.Documents.Open
                  (ref path, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing);
                table = doc.Tables[1];
                string text = table.Cell(rowIndex, colIndex).Range.Text.ToString();
                text = text.Substring(0, text.Length - 2);　　//去除尾部的mark
                return text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (doc != null)
                    doc.Close(ref missing, ref missing, ref missing);
                cls.Quit(ref missing, ref missing, ref missing);
            }
        }
        #endregion 读取Word表格中某个单元格的数据。
        
        #region 修改word表格中指定单元格的数据
        /// <summary>
        /// 修改word表格中指定单元格的数据
        /// </summary>
        /// <param name="fileName">word文档包括路径</param>
        /// <param name="rowIndex">行</param>
        /// <param name="colIndex">列</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool UpdateWordTableByCell(string fileName, int rowIndex, int colIndex, string content)
        {
            Microsoft.Office.Interop.Word._Application cls = null;
            Microsoft.Office.Interop.Word._Document doc = null;
            Microsoft.Office.Interop.Word.Table table = null;
            object missing = System.Reflection.Missing.Value;
            object path = fileName;
            cls = new Application();
            try
            {
                doc = cls.Documents.Open
                    (ref path, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing);

                table = doc.Tables[1];
                //doc.Range( ref 0, ref 0 ).InsertParagraphAfter();//插入回车
                table.Cell(rowIndex, colIndex).Range.InsertParagraphAfter();//.Text = content;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(ref missing, ref missing, ref missing);
                    cls.Quit(ref missing, ref missing, ref missing);
                }
            }
        }
        #endregion

        #region 关闭word进程
        /// <summary>
        /// 关闭word进程
        /// </summary>
        public static void KillWordProcess()
        {
            System.Diagnostics.Process[] myProcess;
            myProcess = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process process in myProcess)
            {
                if (process.Id != 0)
                {
                    string myS = "WINWORD.EXE" + process.ProcessName + "  ID:" + process.Id.ToString();
                    try
                    {
                        if (process.Modules != null)
                            if (process.Modules.Count > 0)
                            {
                                System.Diagnostics.ProcessModule pm = process.Modules[0];
                                myS += "\n Modules[0].FileName:" + pm.FileName;
                                myS += "\n Modules[0].ModuleName:" + pm.ModuleName;
                                myS += "\n Modules[0].FileVersionInfo:\n" + pm.FileVersionInfo.ToString();
                                if (pm.ModuleName.ToLower() == "winword.exe")
                                    process.Kill();
                            }
                    }
                    catch
                    { }
                    finally
                    {
                    }
                }
            }
        }
        #endregion 关闭word进程

        #region 判断系统是否装word

        /// <summary>
        /// 判断系统是否装word
        /// </summary>
        /// <returns></returns>
        public static bool IsWordInstalled()
        {
            RegistryKey machineKey = Registry.LocalMachine;
            if (IsWordInstalledByVersion("12.0", machineKey))
            {
                return true;
            }
            if (IsWordInstalledByVersion("11.0", machineKey))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 判断系统是否装某版本的word
        /// </summary>
        /// <param name="strVersion">版本号</param>
        /// <param name="machineKey"></param>
        /// <returns></returns>
        private static bool IsWordInstalledByVersion(string strVersion, RegistryKey machineKey)
        {
            try
            {
                RegistryKey installKey = 
                    machineKey.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey(
                    "Office").OpenSubKey(strVersion).OpenSubKey("Word").OpenSubKey("InstallRoot");
                if (installKey == null)
                {
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        #endregion 判断系统是否装word
    }
}