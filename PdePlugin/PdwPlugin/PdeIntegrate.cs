using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xlsx = Microsoft.Office.Interop.Excel;
using docx = Microsoft.Office.Interop.Word;
using System.IO;
using Microsoft.Office.Core;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Pdw.Services;

namespace PdwPlugin
{
    class PdeIntegrate
    {
        public static xlsx.Application OpenExcel(xlsx.Application excel, docx.Document doc)
        {
            if (excel == null)
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory="c:\\";
            openFileDialog.Filter = "Excel File|*.xlsx";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                excel.Workbooks.Open(openFileDialog.FileName);
                excel.Visible = true;
            }
            //PdeService.AddImportedFileName(doc, openFileDialog.FileName);
            return excel;
        }

        //to check wether user has been open excel.
        //User can open excel by two way, 1. click import excel button on rabbion; 2. right click the chart and choose edit data.
        private static bool CheckExcel(ref xlsx.Application excel, docx.Document doc)
        {
            if (excel != null && excel.Visible == false)//has been close the excel application
            {
                excel.Quit();
                excel = null;
            }
            if (excel == null)//meaning user do not click import button.
            {
                foreach (docx.InlineShape s in doc.InlineShapes)
                {
                    if (s.HasChart == MsoTriState.msoTrue)
                    {
                        docx.Chart c = s.Chart;
                        try
                        {
                            excel = c.ChartData.Workbook.Application;
                            excel.Visible = true;
                            break;
                        }
                        catch{}//just abort the exception, because the user didn't click the edit data button.
                    }
                }
            }
            if (excel == null || excel.ActiveWorkbook == null)
            {
                MessageBox.Show("Please import Excel file first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        public static void InsertTable(xlsx.Application excel, docx.Document doc)
        {
            if (!CheckExcel(ref excel, doc)) return;
            try
            {
                xlsx.Range selectedRange = excel.Selection;
                string name = GetNameForRange(excel);
                selectedRange.Copy();
                doc.Application.Selection.PasteExcelTable(true, false, false);
                Clipboard.Clear();
            }
            catch (InvalidCastException )
            {
                MessageBox.Show("Please select range in Excel workbook first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static string GetNameForRange(xlsx.Application excel)
        {
            String rangName;
            try
            {
                rangName = excel.Selection.Name.Name;
                return rangName;
            }catch(System.Runtime.InteropServices.COMException comEx)
            {
              
            }
            DateTime dt = DateTime.Now;
            rangName = "pde_"+dt.ToString("yyyyMMddHHmmss")+excel.Names.Count;
            excel.Names.Add(rangName, excel.Selection);
            return rangName;
            //int maxNames = 0;
            //DocumentProperty dp = null;
            //DocumentProperties tempDp = (Microsoft.Office.Core.DocumentProperties)book.CustomDocumentProperties;
            //foreach (DocumentProperty tempProerty in tempDp)
            //{
            //    if (tempProerty.Name.Equals("Max Value of Names"))
            //    {
            //        dp = tempProerty;
            //    }
            //}
            //if (dp == null)
            //{
                //foreach (xlsx.Name n in book.Names)
                //{
                //    if (n.Name.StartsWith("Pde_"))
                //    {
                //        int temp = Convert.ToInt16(n.Name.Substring(4));
                //        maxNames = temp > maxNames ? temp : maxNames;
                //    }
                //}
                //maxNames = maxNames + 1;
            //    book.CustomDocumentProperties.Add("Max Value of Names", false, MsoDocProperties.msoPropertyTypeNumber, maxNames);
            //}
            //else//already has this property, so use it and store value back.
            //{
            //    maxNames = dp.Value + 1;
            //    dp.Value = maxNames;
            //}
            //return "Pde_" + maxNames;
        }

        public static void InsertChart(xlsx.Application excel, docx.Document doc)
        {
            if (!CheckExcel(ref excel, doc)) return;
            xlsx.Workbook book = excel.ActiveWorkbook;
            xlsx.Chart selectedChart = excel.ActiveWorkbook.ActiveChart;
            
            if (selectedChart == null)
            {
                MessageBox.Show("Please select chart in Excel workbook first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            selectedChart.ChartArea.Copy();
            doc.Application.Selection.Paste();

            //reset enviorment.If we copy a chart, then copy table, the table wouldn't link to excel.
            book.ActiveSheet.Range("A1").Copy();
            selectedChart.ChartArea.Select();
            //Clipboard.Clear();
        }
        public static void SaveExcelToDoc(docx.Document doc, string ExcelFile)
        {
            CustomXMLPart xmlPart = doc.CustomXMLParts.Add();
            //XmlObject excelContent = new XmlObject(ExcelToBase64(ExcelFile));
            string xmlContent = ExcelToBase64(ExcelFile);
            xmlContent = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><content>" + xmlContent + "</content>";
            xmlPart.LoadXML(xmlContent);

        }
        public static string ExcelToBase64(string ExcelFile)
        {
            FileStream fs = File.OpenRead(ExcelFile);
            Byte[] buffer = new Byte[fs.Length];
            //从流中读取字节块并将该数据写入给定缓冲区buffer中
            fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
            string result = Convert.ToBase64String(buffer);
            return result;
        }

        public static void ExcelFromBase64(string encodeStr, string file)
        {
            Byte[] data = Convert.FromBase64String(encodeStr);
            MemoryStream m = new MemoryStream(data);
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            m.WriteTo(fs);
            m.Close();
            fs.Close();
            m = null;
            fs = null;
        }

        public static void RefreshData(docx.Document doc)
        {
            doc.Application.ScreenUpdating = false;
            docx.Chart c = null;
            //foreach (docx.Shape s in doc.Shapes)
            //{
            //    if (s.HasChart == MsoTriState.msoTrue)
            //    {
            //        c = s.Chart;
            //        c.ChartData.Activate();
            //        c.ChartData.Workbook.Close();
            //    }
            //}
            foreach(docx.InlineShape s in doc.InlineShapes)
            {
                if (s.HasChart == MsoTriState.msoTrue)
                {
                    c = s.Chart;
                    c.ChartData.Activate();
                    c.ChartData.Workbook.Application.Visible = false;
                    c.ChartData.Workbook.Close();
                }
            }
            doc.Application.ScreenUpdating = true;
        }

        public static void GetName(xlsx.Application excel, docx.Document doc)
        {
            //xlsx.Workbook book = excel.ActiveWorkbook;
            //string name = book.Application.Selection.Name.Name;
            //doc.Application.Selection.TypeText(name);
            foreach (docx.InlineShape s in doc.InlineShapes)
            {
                if (s.HasChart == MsoTriState.msoTrue)
                {
                    Chart c = s.Chart;
                    Series ss = c.SeriesCollection(1);
                    doc.Application.Selection.TypeText(ss.Formula);
                }
            }
            doc.Application.Selection.TypeText(doc.FullName);
            doc.Application.Selection.TypeText(doc.Path);
        }

        public static void ImportExcel2(docx.Document wDoc, string excelfile)
        {
            //Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
            //wApp.Visible = true;
            //Document wDoc = wApp.Documents.Open(FileName: wordfile);
            
            Microsoft.Office.Interop.Word.Chart chart1 = wDoc.Shapes.AddChart().Chart;
            xlsx.Workbook excelbook = chart1.ChartData.Workbook;
            excelbook.Application.DisplayAlerts = false;

            excelbook.ActiveSheet.Name = "___";
            //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            //excel.Visible = true;
            xlsx.Workbook importBook = excelbook.Application.Workbooks.Open(excelfile);
            foreach (xlsx.Worksheet s in importBook.Worksheets)
            {
                //s.Select();
                s.Copy(After: excelbook.Sheets.get_Item(excelbook.Sheets.Count));
                //excelbook.Sheets.Add();
                //excelbook.ActiveSheet.paste();
            }
            //excelbook.Sheets.get_Item(1).delete();
            importBook.Close();
            //excelbook.Application.DisplayAlerts = true;
            //excelbook.Sheets.Add();

            //excelbook.Application.Visible = true;
            //wDoc.Save();
            //wApp.Quit();
        }
    }
}
