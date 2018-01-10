using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Oracle.DataAccess.Client;
using ProntoDoc.Framework.Utils;
using System.Collections;
using Microsoft.Office.Interop.Excel;
using System.Xml;
using System.IO;
using Microsoft.Vbe.Interop;
using System.Diagnostics;
using Microsoft.Win32;
using System.Xml.Xsl;

namespace WaterMark
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;
           
           // Document doc = wordApp.Documents.Open(@"e:\12.pdw");
            int i = 0;
            i += 1;

            //doc.SaveAs(@"e:\1.pdw");
            //doc.Save();
            Microsoft.Office.Core.COMAddIns wAddIns = wordApp.COMAddIns;
            foreach (Microsoft.Office.Core.COMAddIn wAddIn in wAddIns)
            {
                if (!wAddIn.Connect)
                {
                    RegistryKey key = Registry.CurrentUser;
                    key = key.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Office").OpenSubKey("Word").OpenSubKey("Addins").OpenSubKey(wAddIn.ProgId);
                    if (key != null && Convert.ToInt32(key.GetValue("LoadBehavior")) == 3)
                    {
                        MessageBox.Show(wAddIn.Description + ", " + wAddIn.ProgId);
                    }
                    else
                    {
                        MessageBox.Show(wAddIn.Description + ":dddd");
                    }
                }
                //if (wAddIn.Description.EndsWith("ProntoDoc for Word", StringComparison.OrdinalIgnoreCase))
                //{
                //    Object wPlugin = wAddIn.Object;
                //    object[] args = new object[] { @"e:\12.pdw",@"e:\1.pdw" };
                //    wPlugin.GetType().InvokeMember("ConvertToProntoDocTemplate",
                //        System.Reflection.BindingFlags.InvokeMethod, null, wPlugin,
                //        args);
                //    break;
                //}
            }
            //doc.Close();
            wordApp.Quit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application eApp = new Microsoft.Office.Interop.Excel.Application();
            eApp.Visible = true;
            eApp.Workbooks.Add();
        }

        private void callBack1Imp(string param)
        {
            MessageBox.Show("value2:" + param);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CallBack cb = new CallBack();
            CallBackWork cbw = new CallBackWork();
            cbw.working("a", cb.callBack1Imp, callBack1Imp, cb.callBack2Imp);
            cbw.working2("b");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddWaterMark.AddWordWatermark(@"e:\1.docx");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            String constr = "Data Source=myorcl;User Id=smallschema;Password=smallschema;";
            string s = string.Format("1:{0},2:{3}", "a", "b", "c","e");
            OracleConnection con = new OracleConnection(constr);
            con.Open();
            con.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;
            Document wDoc = wordApp.Documents.Open(@"e:\1.docx");
            //wDoc.TrackRevisions = true;
            wordApp.ActiveWindow.View.RevisionsView = WdRevisionsView.wdRevisionsViewFinal;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;
            Document wDoc = wordApp.Documents.Open(@"e:\1.docx");
            Bookmark bm = wDoc.Bookmarks[1];
            string name = bm.Name;
            Microsoft.Office.Interop.Word.Range bmRange = bm.Range;

            bm.Range.InsertParagraphBefore();
            bmRange.Start = bmRange.Start + 1;
            bm.Delete();
            wDoc.Bookmarks.Add(name, bmRange);
            Microsoft.Office.Interop.Word.Range bmpRange = bm.Range.Paragraphs[1].Range;
            int count = bm.Range.Paragraphs.Count;
            //MessageBox.Show(bmRange.Text);
            //MessageBox.Show(bmpRange.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;
            Document wDoc = wordApp.Documents.Open(@"e:\1.docx");
            string documentName = wDoc.FullName;
            documentName = wordApp.ActiveDocument.FullName;
            //Table table = wDoc.Tables[1];
            ////table.Cell(1, 1).Select();
            //Microsoft.Office.Interop.Word.Range tabRange = table.Cell(1, 1).Range;
            //foreach(Row row in table.Rows)
            //{
            //    MessageBox.Show("r:" + row.HeadingFormat);
            //}
            //Bookmark bm = wDoc.Bookmarks["dddd"];
            //Microsoft.Office.Interop.Word.Range bmRange = bm.Range;
            //MessageBox.Show(bmRange.Text);

            //Microsoft.Office.Interop.Word.Range bmpRange = bmRange.Paragraphs[1].Range;
            //MessageBox.Show(bmpRange.Text);

            //string docText = wDoc.Content.Text;
            //MessageBox.Show(docText);
            wDoc.ExportAsFixedFormat(@"e:\1.xps", WdExportFormat.wdExportFormatXPS, OpenAfterExport: true);
            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.UseShellExecute = true;
            pInfo.FileName = @"e:\1.xps";
            Process.Start(pInfo);
            wDoc.Close();
            wordApp.Quit();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            List<string> strs = new List<string>();
            strs.Add("SS_SQLServer");
            strs.Add("輝百新(紹興)有限公司 (TEST)_GMS");
            //int index = strs.FindIndex(s => s == "zh中文测试_gms(test)");
            //index = strs.FindIndex(s => s == "a");

            string result = ObjectSerializeHelper.SerializeToString<List<string>>(strs);
            Console.WriteLine(result);

            List<string> strs1 = new List<string>();
            strs1.Add("SS_SQLServer");
            strs1.Add("company1");
            string result1 = ObjectSerializeHelper.SerializeToString<List<string>>(strs1);
           
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string filePath = @"e:\333.xlsx";
            Microsoft.Office.Interop.Excel.Application eApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workBook = eApp.Workbooks.Open(filePath, AddToMru: false);
            ChartObjects exCharts = (ChartObjects)workBook.Sheets[1].ChartObjects(Type.Missing);
            ChartObject xlChartObj = (ChartObject)exCharts.Item(1);
            string name = xlChartObj.Name;
            MessageBox.Show(name);
            MessageBox.Show(xlChartObj.Chart.Name);
            workBook.Close();
            eApp.Quit();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string filePath = @"e:\工作簿1.xlsx";
            Microsoft.Office.Interop.Excel.Application eApp = new Microsoft.Office.Interop.Excel.Application();
            eApp.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook workBook = eApp.Workbooks.Open(filePath, AddToMru: false);
            ChartObjects exCharts = workBook.Sheets[1].ChartObjects();
            ChartObject xlChartObj = exCharts.Item(1);
            string name = xlChartObj.Chart.Name;
            string actSheetName = workBook.ActiveSheet.Name;
            MessageBox.Show(xlChartObj.Name);
            MessageBox.Show(name.Substring(actSheetName.Length));


            //ChartObjects exCharts2 = workBook.Sheets[2].ChartObjects();
            //ChartObject xlChartObj2 = exCharts2.Item(1);
            //string name2 = xlChartObj2.Chart.Name;
            //MessageBox.Show(xlChartObj2.Name);
            //MessageBox.Show(name2);
            //MessageBox.Show(xlChartObj2.Chart.CodeName);
            workBook.Close();
            eApp.Quit();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            FileStream fs = File.Open("e:\\1112.xml",FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            doc.Load(sr);
            XmlNamespaceManager _xmlNamespace = new XmlNamespaceManager(doc.NameTable);
            _xmlNamespace.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            _xmlNamespace.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            XmlNodeList node = doc.SelectNodes("//w:bookmarkStart", _xmlNamespace);
            int nodenum = node.Count;
            fs.Close();
            MessageBox.Show("count:" + nodenum);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application eApp = new Microsoft.Office.Interop.Excel.Application();
            eApp.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook workBook = eApp.Workbooks.Add();
            Microsoft.Vbe.Interop.VBProject project = null;
            
            try
            {
                project = workBook.VBProject;
            }
            catch (Exception ex)
            {
                StringBuilder sbMes = new StringBuilder();
                sbMes.AppendLine("  Please enable \"Trust access to VBA project model\" in Microsoft Excel .");
                sbMes.AppendLine();
                sbMes.AppendLine("      1. File > Option > Trust Center ");
                sbMes.AppendLine("      2. Click to 'Trust Center Setting...' ");
                sbMes.AppendLine("      3. Click to 'Macro Setting' tab");
                sbMes.AppendLine("      4. Check the box 'Trust access to VBA project model' ");
                sbMes.AppendLine("      5. Click 'OK'");

                MessageBox.Show(sbMes.ToString());
            }
            var projectName = project.Name;
            var procedureType = Microsoft.Vbe.Interop.vbext_ProcKind.vbext_pk_Proc;
            List<string> lstMacro = new List<string>();

            foreach (var component in project.VBComponents)
            {
                VBComponent vbComponent = component as VBComponent;
                if (vbComponent != null)
                {
                    string componentName = vbComponent.Name;
                    var componentCode = vbComponent.CodeModule;
                    bool saved = vbComponent.Saved;
                    bool openDesigner = vbComponent.HasOpenDesigner;

                    int componentCodeLines = componentCode.CountOfLines;
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application eApp = new Microsoft.Office.Interop.Excel.Application();
            eApp.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook workBook = eApp.Workbooks.Open(@"e:/1.xlsx");
            Worksheet sheet = workBook.ActiveSheet;

            Microsoft.Office.Interop.Excel.Range cell = sheet.Cells[2, 2] as Microsoft.Office.Interop.Excel.Range;
            cell.Select();
            cell.XPath.Clear();
            //eApp.DisplayAlerts = false;

            cell.NumberFormat = "General";
            //cell.NumberFormatLocal = "G/通用格式";

            bool hasFormula = cell.HasFormula;

            Microsoft.Office.Interop.Excel.Range colRange = sheet.get_Range("H14");

            hasFormula = colRange.HasFormula;
            //hasFormula = colRange.ListObject.Range.HasFormula;

            Microsoft.Office.Interop.Excel.Range colRange2 = sheet.get_Range("A14");
            Microsoft.Office.Interop.Excel.Range colRange3 = sheet.get_Range("J16");
            hasFormula = colRange2.ListObject == colRange3.ListObject;

            hasFormula = colRange2.ListObject.ListColumns[5].Range.HasFormula;

            hasFormula = colRange2.ListObject.ListColumns[5] == sheet.ListObjects[2].ListColumns[5];

            hasFormula = sheet.ListObjects[2].ListColumns[2].Range.HasFormula;

            hasFormula = sheet.ListObjects[2].ListColumns[3].Range.HasFormula;

            hasFormula = sheet.ListObjects[2].ListColumns[4].Range.HasFormula;

            hasFormula = sheet.ListObjects[2].ListColumns[5].Range.HasFormula;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string fileContent = "abc";
            byte[] data = Encoding.Default.GetBytes(fileContent);
            using (MemoryStream memory = new MemoryStream(data))
            {
                //using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate))
                using (FileStream file = new FileStream(@"e:\1.txt", FileMode.OpenOrCreate))//overwrite existed file or create one.
                {
                    memory.WriteTo(file);
                    memory.Close();
                    file.Close();
                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application eApp = new Microsoft.Office.Interop.Excel.Application();
            eApp.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook workBook = eApp.Workbooks.Open(@"e:/2.xlsx");
            Worksheet sheet = workBook.ActiveSheet;
            PivotTable pt = sheet.PivotTables()[1];
            Microsoft.Office.Interop.Excel.Range range = pt.TableRange2;
            range.CopyPicture(XlPictureAppearance.xlScreen, XlCopyPictureFormat.xlPicture);
            Worksheet newSheet = workBook.Sheets.Add();
            //newSheet.Paste();
            //Microsoft.Office.Interop.Excel.Shape s = newSheet.Shapes.Item(1);
            //s.Chart.Export(@"e:\2.png", "PNG");

            ChartObjects objs = newSheet.ChartObjects();
            ChartObject chartobj = objs.Add(0, 0, range.Width, range.Height);
            //if (chartobj.Chart != null)
            chartobj.ProtectChartObject = false;
            chartobj.Chart.Paste();
            chartobj.Chart.Export(@"e:\1.png", "PNG");
            chartobj.Delete();
            eApp.Quit();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if (flag)
            {
#if Debug 
                string name = new SQLDao.SQLDao().name;
#endif
            }
            else
            {
                string name = new OracleDao.OracleProvider().name;
            }
            MessageBox.Show("ok");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            List<string> strs = new List<string>();
            strs.Add("1");
            strs.Add("2");
            var indcs = strs.Select(f => f).Concat(new string[]{});
            MessageBox.Show("result:" + indcs.ToString());
        }

        private void button19_Click(object sender, EventArgs e)
        {
            FileBufferedStream xml = new FileBufferedStream();
            FileStream fs = File.OpenRead(@"D:\work\ProntoDir\xsl\merge.xml");
            xml.Append(fs);
            
            StreamReader sr = new StreamReader(@"D:\work\ProntoDir\xsl\merge.xsl");
            string xsl = sr.ReadToEnd();
            sr.Close();

            FileBufferedStream result = new FileBufferedStream();

            result.Append("1111111111111111111111111111");

            TransformDocumentToStream(xml, xsl, result);


            result.Append("2222222222222222");
            string strresult = result.Reader().ReadToEnd();
            MessageBox.Show(strresult);

        }

        public static void TransformDocumentToStream(FileBufferedStream xmlDataStream, string xslContent, FileBufferedStream fbs)
        {
            //1. Load xml and xsl content to XmlReader
            XmlReader xsl = XmlReader.Create(new StringReader(xslContent));
            XmlReader xml = XmlReader.Create(xmlDataStream.GetStream());

            //2. Create XmlWriter
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;

            using (XmlWriter writer = XmlWriter.Create(fbs, settings))
            {
                //3. Transform document
                XslCompiledTransform transformer = new XslCompiledTransform();
                transformer.Load(xsl, new System.Xml.Xsl.XsltSettings(false, true), null);
                transformer.Transform(xml, writer);
            }
        }
    }
}
