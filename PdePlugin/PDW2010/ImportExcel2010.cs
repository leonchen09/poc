using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xlsx = Microsoft.Office.Interop.Excel;
using docx = Microsoft.Office.Interop.Word;

namespace PDW2010
{
    class ImportExcel2010
    {
        public static xlsx.Workbook ImportExcel(docx.Document wDoc, string excelfile)
        {
            xlsx.Application excel = new xlsx.Application();
            xlsx.Workbook book = excel.Workbooks.Open(excelfile);
            return book;
        }
        
        public static void CopyExcel(xlsx.Workbook book, docx.Document doc)
        {
            xlsx.Range r = book.ActiveSheet.ActiveCell.CurrentRange;
            //doc.Sections.PasteExcelTable();
            docx.Table dt = doc.Tables.Add(doc.Range(0, 0), 2, 3);
            dt.Cell(1, 1).FitText = (r.Cells.get_Item(1, 1));
        }

        public static void ImportExcel2(docx.Document wDoc, string excelfile)
        {
            //Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
            //wApp.Visible = true;
            //Document wDoc = wApp.Documents.Open(FileName: wordfile);
            //wDoc.
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

        public static void InsertChart(docx.Document wDoc, xlsx.Workbook importBook)
        {
            docx.Chart chart1 = wDoc.InlineShapes.AddChart().Chart;
            xlsx.Workbook excelbook = chart1.ChartData.Workbook;
            //查找选中的chart
            xlsx.Chart importchart = importBook.ActiveChart;
            //foreach (xlsx.Series myser in importchart.SeriesCollection)
            //{
            //    string temp = myser.Formula;
            //    temp = temp.Substring(9, temp.Length - 9);
                
            //}

        }
    }
}
