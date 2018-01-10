using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;

namespace WaterMark
{
    class AddWaterMark
    {
        private string waterMarkFormat = "<w:sdtContent xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"><w:p w:rsidR=\"00910F8C\" w:rsidRDefault=\"007957B3\"><w:r><w:rPr><w:noProof /></w:rPr><w:pict><v:shapetype id=\"_x0000_t136\" coordsize=\"21600,21600\" o:spt=\"136\" adj=\"10800\" path=\"m@7,l@8,m@5,21600l@6,21600e\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:v=\"urn:schemas-microsoft-com:vml\"><v:formulas><v:f eqn=\"sum #0 0 10800\" /><v:f eqn=\"prod #0 2 1\" /><v:f eqn=\"sum 21600 0 @1\" /><v:f eqn=\"sum 0 0 @2\" /><v:f eqn=\"sum 21600 0 @3\" /><v:f eqn=\"if @0 @3 0\" /><v:f eqn=\"if @0 21600 @1\" /><v:f eqn=\"if @0 0 @2\" /><v:f eqn=\"if @0 @4 21600\" /><v:f eqn=\"mid @5 @6\" /><v:f eqn=\"mid @8 @5\" /><v:f eqn=\"mid @7 @8\" /><v:f eqn=\"mid @6 @7\" /><v:f eqn=\"sum @6 0 @5\" /></v:formulas><v:path textpathok=\"t\" o:connecttype=\"custom\" o:connectlocs=\"@9,0;@10,10800;@11,21600;@12,10800\" o:connectangles=\"270,180,90,0\" /><v:textpath on=\"t\" fitshape=\"t\" /><v:handles><v:h position=\"#0,bottomRight\" xrange=\"6629,14971\" /></v:handles><o:lock v:ext=\"edit\" text=\"t\" shapetype=\"t\" /></v:shapetype><v:shape id=\"PowerPlusWaterMarkObject357476642\" o:spid=\"_x0000_s3076\" type=\"#_x0000_t136\" style=\"position:absolute;left:0;text-align:left;margin-left:0;margin-top:0;width:527.85pt;height:131.95pt;rotation:315;z-index:-251656192;mso-position-horizontal:center;mso-position-horizontal-relative:margin;mso-position-vertical:center;mso-position-vertical-relative:margin\" o:allowincell=\"f\" fillcolor=\"silver\" stroked=\"f\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:v=\"urn:schemas-microsoft-com:vml\"><v:fill opacity=\".5\" /><v:textpath style=\"font-family:&quot;Simsun&quot;;font-size:1pt\" string=\"{0}\" /><w10:wrap anchorx=\"margin\" anchory=\"margin\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" /></v:shape></w:pict></w:r></w:p></w:sdtContent>";
        public void addWaterMark(string fileName, String waterMark)
        {
            XmlDocument wordxml = new XmlDocument();
            wordxml.Load(fileName);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(wordxml.NameTable);
            nsmgr.AddNamespace("pkg", "http://schemas.microsoft.com/office/2006/xmlPackage");
            nsmgr.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            XmlNode headerNode = findDefaultHeader(wordxml, nsmgr);
            updateNode(headerNode, string.Format(waterMarkFormat, waterMark), nsmgr);
            wordxml.Save(fileName);
        }
        private XmlNode findDefaultHeader(XmlDocument doc, XmlNamespaceManager nsmgr)
        {
            XmlNode package = doc.SelectSingleNode("/pkg:package", nsmgr);
            //XmlNode package = doc.GetElementsByTagName("pkg:package")[0];
            XmlNode docNode = null ;
            XmlNode resNode = null;
            XmlNode headerNode = null;
            foreach(XmlNode node in package.ChildNodes)
            {
                    //XmlNode node = package.ChildNodes[i];
                if (node.Attributes["pkg:name"].Value.Equals("/word/document.xml"))
                {
                    docNode = node;
                }
                else if (node.Attributes["pkg:name"].Value.Equals("/word/_rels/document.xml.rels"))
                {
                    resNode = node;
                }
            }
            XmlNode defHeaderRid = docNode.SelectSingleNode("pkg:xmlData/w:document/w:body/w:sectPr/w:headerReference[@w:type=\"default\"]", nsmgr);
            string rid = defHeaderRid.Attributes["r:id"].Value;

            XmlNode defHeaderShip = null;// resNode.SelectSingleNode("pkg:xmlData/Relationships/Relationship[@Id=\"" + rid + "\"]", nsmgr);
            XmlNodeList relationships = resNode.SelectSingleNode("pkg:xmlData", nsmgr).FirstChild.ChildNodes;
            foreach (XmlNode n in relationships)
            {
                if (n.Attributes["Id"].Value.Equals(rid))
                {
                    defHeaderShip = n;
                    break;
                }
            }
            
            string headerName = defHeaderShip.Attributes["Target"].Value;
            foreach (XmlNode node in package.ChildNodes)
            {
                if (node.Attributes["pkg:name"].Value.Equals("/word/" + headerName))
                {
                    headerNode = node;
                    break;
                }
            }
            return headerNode;
        }

        private void updateNode(XmlNode headerNode, String content, XmlNamespaceManager nsmgr)
        {
            XmlNode node = headerNode.SelectSingleNode("pkg:xmlData/w:hdr", nsmgr).FirstChild;

            node.InnerXml = node.InnerXml + content;
        }

        public static void WordSave(bool saved)
        {
            Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
            wApp.Visible = true;
            Document wDoc = wApp.Documents.Add();
            wDoc.InlineShapes.AddChart(Microsoft.Office.Core.XlChartType.xl3DArea);
            wDoc.Saved = saved;
        }

        public static void AddWordWatermark(String fileName)
        {
            Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
            wApp.Visible = true;
            Document wDoc = wApp.Documents.Open(FileName: fileName);

            Selection Selection = wApp.Selection;
            Microsoft.Office.Interop.Word.Shape wmShape;
            //Selection.Select();

            // set focus on header to put watermark
            wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;

            //Create the watermar shape
            wmShape = Selection.HeaderFooter.Shapes.AddTextEffect(
                        Microsoft.Office.Core.MsoPresetTextEffect.msoTextEffect1,
                        "water mark", "Times New Roman", 1,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        100, 500);

            //Set all of the attributes of the watermark
            wmShape.Select();
            wmShape.Name = "PowerPlusWaterMarkObjectPPPP";
            wmShape.TextEffect.NormalizedHeight = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Fill.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Fill.Solid();
            wmShape.Fill .ForeColor.RGB = (int)WdColor.wdColorGray15;
            //wmShape.Fill.ForeColor.RGB = (int)WdColor.wdColorGray05;
            wmShape.Fill.Transparency = 0.5f;//100% transparency
            wmShape.Rotation = 315.0f;
            wmShape.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
            wmShape.Height = 150;//minimum hight 
            wmShape.Width = 500;//minimum width
            wmShape.Left = (float)WdShapePosition.wdShapeCenter;
            Selection.ShapeRange.WrapFormat.AllowOverlap = -1;
            Selection.ShapeRange.WrapFormat.Side = WdWrapSideType.wdWrapBoth;
            Selection.ShapeRange.WrapFormat.Type = WdWrapType.wdWrapNone;  //3
            Selection.ShapeRange.RelativeHorizontalPosition =
                        WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
            Selection.ShapeRange.RelativeVerticalPosition =
                        WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
            Selection.ShapeRange.Left = (float)WdShapePosition.wdShapeCenter;
            Selection.ShapeRange.Top = (float)WdShapePosition.wdShapeCenter;
            //wmShape.ZOrder(Microsoft.Office.Core.MsoZOrderCmd.msoSendBehindText）;
            wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
            //wDoc.Save();
            //wApp.Quit();
        }

        public static void AddWordWatermark2(String fileName)
        {
            Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
            wApp.Visible = true;
            Document wDoc = wApp.Documents.Open(FileName: fileName);

            Selection Selection = wApp.Selection;
            Microsoft.Office.Interop.Word.Shape wmShape;
            //Selection.Select();
            wDoc.Sections[1].Range.Select();

            // set focus on header to put watermark
            wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;

            //Create the watermar shape
            wmShape = Selection.HeaderFooter.Shapes.AddTextEffect(
                        Microsoft.Office.Core.MsoPresetTextEffect.msoTextEffect1,
                        "water mark", "Times New Roman", 1,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        0, 0);

            //Set all of the attributes of the watermark
            //wmShape.Select();
            wmShape.Name = "PowerPlusWaterMarkObjectPPPP";
            wmShape.TextEffect.NormalizedHeight = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Fill.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Fill.Solid();
            wmShape.Fill.ForeColor.RGB = (int)WdColor.wdColorGray15;
            //wmShape.Fill.ForeColor.RGB = (int)WdColor.wdColorGray05;
            wmShape.Fill.Transparency = 0.5f;//100% transparency
            wmShape.Rotation = 315.0f;
            wmShape.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
            wmShape.Height = 150;//minimum hight 
            wmShape.Width = 500;//minimum width
            wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
            wDoc.Sections[1].Range.Select();
            Selection.TypeText("zzzzz");
            //wmShape.WrapFormat.AllowOverlap = -1;
            //wmShape.WrapFormat.Side = WdWrapSideType.wdWrapBoth;
            //wmShape.WrapFormat.Type = WdWrapType.wdWrapNone;  //3
            wmShape.RelativeHorizontalPosition =
                        WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
            wmShape.RelativeVerticalPosition =
                        WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
            wmShape.Left = (float)WdShapePosition.wdShapeCenter;
            wmShape.Top = (float)WdShapePosition.wdShapeCenter;
            //wmShape.ZOrder(Microsoft.Office.Core.MsoZOrderCmd.msoSendBehindText）;
            wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
            //wDoc.Save();
            //wApp.Quit();
        }

        //public static void AddWordWatermarkWord(String fileName)
        //{
        //    Application wApp = new Application();
        //    wApp.Visible = false;
        //    Document wDoc = wApp.Documents.Open(FileName: fileName);

        //    Selection Selection = wApp.Selection;
        //    Shape wmShape;
        //    Selection.Select();

        //    // set focus on header to put watermark
        //    wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;

        //    //Create the watermar shape
        //    wmShape = Selection.HeaderFooter.Shapes.AddTextEffect(
        //                Microsoft.Office.Core.MsoPresetTextEffect.msoTextEffect1,
        //                " ", "Times New Roman", 1,
        //                Microsoft.Office.Core.MsoTriState.msoFalse,
        //                Microsoft.Office.Core.MsoTriState.msoFalse,
        //                0, 0);

        //    //Set all of the attributes of the watermark
        //    wmShape.Select();
        //    wmShape.Name = "PowerPlusWaterMarkObjectqqq1";
        //    wmShape.TextEffect.NormalizedHeight = Microsoft.Office.Core.MsoTriState.msoFalse;
        //    wmShape.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
        //    wmShape.Fill.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
        //    wmShape.Fill.Solid();
        //    wmShape.Fill.ForeColor.RGB = (int)WdColor.wdColorGray05;
        //    wmShape.Fill.Transparency = 0.0f;//100% transparency
        //    wmShape.Rotation = 315.0f;
        //    wmShape.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
        //    wmShape.Height = 1;//minimum hight 
        //    wmShape.Width = 1;//minimum width
        //    wmShape.WrapFormat.AllowOverlap = -1; //true
        //    wmShape.WrapFormat.Side = WdWrapSideType.wdWrapBoth;
        //    wmShape.WrapFormat.Type = WdWrapType.wdWrapNone;  //3
        //    wmShape.RelativeHorizontalPosition =
        //                WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
        //    wmShape.RelativeVerticalPosition =
        //                WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
        //    wmShape.Left = (float)WdShapePosition.wdShapeCenter;
        //    wmShape.Top = (float)WdShapePosition.wdShapeCenter;
        //    //wmShape.ZOrder(Microsoft.Office.Core.MsoZOrderCmd.msoSendBehindText）;
        //    wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
        //    wDoc.SaveAs("E:\\ProntoDir\\output\\eeeee_new.xml", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatFlatXML);
        //    wApp.Quit();
        //}

        //public static void AddWordTable(String fileName)
        //{
        //    Application wApp = new Application();
        //    wApp.Visible = false;
        //    Document wDoc = wApp.Documents.Open(FileName: fileName);
        //    Range r = wDoc.Range(0,0);
        //    wDoc.Tables.Add(r,2,3);
        //    wDoc.Save();
        //    wApp.Quit();
        //}

        public static void ImportExcel(String wordfile, string excelfile)
        {
            Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
            wApp.Visible = true;
            Document wDoc = wApp.Documents.Open(FileName: wordfile);
            //InlineShape chart = wDoc.InlineShapes.AddOLEObject("Excel.Sheet.12", excelfile, false, true);
            //InlineShape chart = wDoc.InlineShapes.AddOLEObject("MSGraph.Chart.12");
            Chart chart = (Chart)wDoc.Shapes.AddChart();
            //chart.OLEFormat.Activate();
            //chart.OLEFormat.Open();//打开被嵌入的excel  
            //chart.Application.Visible = true;
            //wDoc.Save();
            //wApp.Quit();
        }
        public static string OpenExcel(string fileName)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = true;
            Workbook book = excel.Workbooks.Open(fileName);
            //xlsx.Range r = book.ActiveSheet.ActiveCell.CurrentRange;
            //Sheets s = book.Sheets.get_Item(1);
           return (book.ActiveSheet.Cells(2,3).value);
        }
        //public static void ImportExcel2(String wordfile, string excelfile)
        //{
        //    Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
        //    wApp.Visible = true;
        //    //wApp.DisplayAlerts = false;
        //    Document wDoc = wApp.Documents.Open(FileName: wordfile);
        //    Microsoft.Office.Interop.Word.Chart chart1 = wDoc.Shapes.AddChart().Chart;
        //    Workbook excelbook = chart1.ChartData.Workbook;
        //    excelbook.Application.DisplayAlerts = false;
        //    excelbook.ActiveSheet.Name = "___";
        //    //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        //    //excel.Visible = true;
        //    Workbook importBook = excelbook.Application.Workbooks.Open(excelfile);
        //    foreach (Worksheet s in importBook.Worksheets)
        //    {
        //        //s.Select();
        //        s.Copy(After:excelbook.Sheets.get_Item(excelbook.Sheets.Count));
        //        //excelbook.Sheets.Add();
        //        //excelbook.ActiveSheet.paste();
        //    }
        //    excelbook.Sheets.get_Item(1).delete();
        //    importBook.Close();
        //    //excelbook.Sheets.Add();

        //    //excelbook.Application.Visible = true;
        //    //wDoc.Save();
        //    //wApp.Quit();
        //}
        
    }
}
