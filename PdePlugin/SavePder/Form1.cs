using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using word = Microsoft.Office.Interop.Word;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using System.Text.RegularExpressions;
using Microsoft.Office.Core;
using Pdw;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;
using System.IO;
using System.Threading;
using Microsoft.Office.Interop.Word;
using Com.StepFunction.ProntoDoc.SX.Kernel.Utils;

namespace PdwPlugin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook workbook = app.Workbooks.Open(textBox1.Text);
            CustomXMLPart xmlPart = workbook.CustomXMLParts.Add();
            
            //读取xml文件
            XmlDocument data = new XmlDocument();
            data.Load(textBox8.Text);
            XmlObject excelContent = new XmlObject(data.OuterXml, ContentType.PderXml);
            xmlPart.LoadXML(ObjectSerializeHelper.SerializeToString<XmlObject>(excelContent));

            //string id = xmlPart.Id;
            //textBox1.Text = id;
            workbook.SaveAs(textBox1.Text + "r");
            workbook.Close();
            app.Quit();
            MessageBox.Show("pder ok.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XmlDocument xsltDoc = new XmlDocument();
            xsltDoc.Load(textBox2.Text);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xsltDoc.NameTable);
            nsmgr.AddNamespace("pkg", "http://schemas.microsoft.com/office/2006/xmlPackage");
            nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
            XmlNode documentNode = xsltDoc.SelectNodes("//pkg:part[@pkg:name='/word/document.xml']",nsmgr)[0];
            documentNode.InnerXml = documentNode.InnerXml.Replace(textBox1.Text, "<xsl:value-of xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" select=\"P1/@xmlpath\"/>");
            XmlNodeList chartNodes = xsltDoc.SelectNodes("//pkg:part[contains(@pkg:name,'/word/charts/_rels/chart')]", nsmgr);
            foreach (XmlNode chartNode in chartNodes)
            {
                //XmlNode relationShip = chartNode.SelectSingleNode("//Relationship");
                XmlNode relationShip = chartNode.FirstChild.FirstChild.FirstChild;
                XmlElement selectValue = xsltDoc.CreateElement("xsl:attribute", "http://www.w3.org/1999/XSL/Transform");
                selectValue.SetAttribute("name", "Target");
                selectValue.InnerText = "file:///";
                //selectValue.InnerXml = "file:///<xsl:value-of select=\"P1/@xmlpath2\"/>";
                XmlElement valueOf = xsltDoc.CreateElement("xsl:value-of", "http://www.w3.org/1999/XSL/Transform");
                valueOf.SetAttribute("select", "P1/@xmlpath2");
                selectValue.AppendChild(valueOf);
                relationShip.AppendChild(selectValue);
            }
            xsltDoc.Save(textBox3.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //new Zip().ZipFile(textBox4.Text, textBox5.Text);
            FastZip fastZip = new FastZip();
            fastZip.CreateZip(textBox5.Text, textBox4.Text, true, "");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //new Zip().unZipFile(textBox5.Text, textBox4.Text);
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(textBox5.Text, textBox4.Text, "");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            word.Application wordApp = new word.Application();
            COMAddIns addins = wordApp.COMAddIns;
            foreach (COMAddIn addin in addins)
            {
                if(addin.Description.Equals("Pdw"))
                {
                    AddinUtilities util = (AddinUtilities)addin.Object;
                    string result = util.DoSomething("from1");
                    MessageBox.Show(result);

                    Object pdwApp = addin.Object;
                    object[] invokeArgs = { "name in form1" };
                    object retVal = pdwApp.GetType().InvokeMember("DoSomething",
                        System.Reflection.BindingFlags.InvokeMethod, null, pdwApp, invokeArgs);
                    MessageBox.Show(retVal.ToString());
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string source = textBox6.Text;
            string result = source.Replace("%", "%25").Replace(" ", "%20").Replace("#", "%23");
            result = result.Replace("^", "%5e").Replace("{", "%7b").Replace("}", "%7d");
            result = result.Replace("[", "%5b").Replace("]", "%5d").Replace("`", "%60");
            textBox7.Text = result;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string source = textBox7.Text;
            string result = source.Replace("%20", " ").Replace("%23", "#");
            result = result.Replace("%5e", "^").Replace("%7b", "{").Replace("%7d", "}");
            result = result.Replace("%5b", "[").Replace("%5d", "]").Replace("%60", "`");
            result = result.Replace("%25", "%");
            textBox6.Text = result;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            word.Application app = new word.Application();
            app.Visible = true;

            Thread.Sleep(2000);

            //disable the pdw plugin
            COMAddIn pdwAddin = null;
            COMAddIns addins = app.COMAddIns;
            foreach (COMAddIn addin in addins)
            {
                if (addin.Description.Equals("Pdw"))
                {
                    addin.Connect = false;
                    pdwAddin = addin;
                    break;
                }
            }
            Thread.Sleep(1000);
            string tempFile = textBox_pdw.Text;
            string wordFile = tempFile.Split('.')[0] + ".docx";
            File.Copy(tempFile, wordFile, true);
            word.Document doc = app.Documents.Open(wordFile);
            //change properties of document
            //foreach (DocumentProperty docPro in doc.CustomDocumentProperties)
            //{
            //    if (docPro.Name.StartsWith("Pronto_ImportedExcelFileName"))
            //    {
            //        docPro.Value = textBox_pdwpder.Text;
            //    }
            //}
            //remove pde template and add pder file.
            PdeContent pdeImported = null;
            string xsl = null;
            foreach (CustomXMLPart xmlPart in doc.CustomXMLParts)
            {
                if (xmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(xmlPart.XML);
                if (customObject.ContentType == ContentType.ImportedPde)
                {
                    pdeImported = ObjectSerializeHelper.Deserialize<PdeContent>(customObject.Content);
                }
                else if (customObject.ContentType == ContentType.Xslt)
                {
                    xsl = customObject.Content;
                }
                xmlPart.Delete();
            }
            //add pder file
            CustomXMLPart xmlPart_pder = doc.CustomXMLParts.Add();
            pdeImported.FileContent = FileHelper.ExcelToBase64(textBox_pdwpder.Text);
            //pdeImported.FullFileName = textBox_pdwpder.Text;
            XmlObject pderObj = new XmlObject();
            pderObj.ContentType = ContentType.ImportedPde;
            pderObj.Content = ObjectSerializeHelper.SerializeToString<PdeContent>(pdeImported);
            xmlPart_pder.LoadXML(ObjectSerializeHelper.SerializeToString<XmlObject>(pderObj));
            //add xml data file
            XmlDocument dataXml = new XmlDocument();
            dataXml.Load(textBox_pdwd.Text);
            XmlObject pdwrXml = new XmlObject(dataXml.InnerXml, ContentType.PdwrXml);
            CustomXMLPart xmlPart_xml = doc.CustomXMLParts.Add();
            xmlPart_xml.LoadXML(ObjectSerializeHelper.SerializeToString<XmlObject>(pdwrXml));
            //add xsl file
            XmlObject xslObj = new XmlObject();
            xslObj.ContentType = ContentType.PdwrXsl;
            xslObj.Content = xsl;
            CustomXMLPart xmlpart_xsl = doc.CustomXMLParts.Add();
            xmlpart_xsl.LoadXML(ObjectSerializeHelper.SerializeToString<XmlObject>(xslObj));
            //add pdwr setting
            XmlObject settObj = new XmlObject();
            settObj.ContentType = ContentType.PdwrSettings;
            settObj.Content = "<?xml version=\"1.0\" encoding=\"utf-16\"?><RenderSettings xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Media>Docx</Media><Channel>Display</Channel></RenderSettings>";
            CustomXMLPart xmlpart_sett = doc.CustomXMLParts.Add();
            xmlpart_sett.LoadXML(ObjectSerializeHelper.SerializeToString<XmlObject>(settObj));

            doc.Save();
            doc.Close();
            pdwAddin.Connect = true;
            app.Quit();
            File.Copy(wordFile, wordFile.Split('.')[0]+".pdwr",true);
            MessageBox.Show("Create pdwr file success.");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            BulkCopyTest test = new BulkCopyTest();
            test.CopyData2();
        }

        private void ChangeLink_Click(object sender, EventArgs e)
        {
            string oldLink = tbOldLink.Text;
            string newLink = tbnewLink.Text;

            openExcelForUpdate(newLink);

            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            //Microsoft.Office.Interop.Word.Document Doc = word.Documents.Open(tbWord.Text);
            Microsoft.Office.Interop.Word.Document Doc = word.Documents.Open(
                FileName: tbWord.Text+"1.xml",
                    Format: WdOpenFormat.wdOpenFormatXML,
                    XMLTransform: tbWord.Text+"1.xslt");

            Doc.SaveAs(tbWord.Text + "1.xml", WdSaveFormat.wdFormatFlatXML);

            showChart(Doc);

            Doc.ActiveWindow.View.Type = WdViewType.wdPrintView;

            for (int i = 1; i <= Doc.Fields.Count; i++)
            {
                Field field = Doc.Fields[i];
                if (field.Type == WdFieldType.wdFieldLink)
                {
                    string linkSource = field.LinkFormat.SourceFullName;
                    field.LinkFormat.SourceFullName = newLink;
                }
            }
            //Doc.Close();
            //word.Quit();
            MessageBox.Show("ok");
        }

        private void openExcelForUpdate(string fileName)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            //excel.Visible = true;
            excel.Workbooks.Open(fileName);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document Doc = word.Documents.Open(textBox5.Text);
            showChart(Doc);
            Doc.SaveAs(textBox5.Text.Substring(0, textBox5.Text.Length - 4) + "xml", WdSaveFormat.wdFormatFlatXML);
            showChart(Doc);
            MessageBox.Show("finish");
            word.Quit();
        }


        private void showChart(Document Doc)
        {
            for (int i = 1; i <= Doc.InlineShapes.Count; i++)
            {
                InlineShape s = Doc.InlineShapes[i];
                if (s.HasChart == MsoTriState.msoTrue && s.LinkFormat != null)
                {
                    string linkSource = s.LinkFormat.SourceFullName;
                    MessageBox.Show(linkSource);
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            CaptCha cc = new CaptCha(300, 75, 6);
            MessageBox.Show(cc.Text);
            cc.Image.Save("e:\\1.jpg");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            word.Application app = createWord();
            String wordFile = textBox10.Text;
            string xsltFile = textBox9.Text;
            word.Document doc = app.Documents.Open(wordFile);
            //change properties of document
            //foreach (DocumentProperty docPro in doc.CustomDocumentProperties)
            //{
            //    if (docPro.Name.StartsWith("Pronto_ImportedExcelFileName"))
            //    {
            //        docPro.Value = textBox_pdwpder.Text;
            //    }
            //}
            //remove pde template and add pder file.
            string xsl = null;
            foreach (CustomXMLPart xmlPart in doc.CustomXMLParts)
            {
                if (xmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(xmlPart.XML);
                if (customObject.ContentType == ContentType.PdwrXsl)
                {
                    xsl = customObject.Content;
                    //xmlPart.Delete();
                    break;
                }
            }
            FileStream fs = new FileStream(xsltFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(xsl);
            sw.Close();
            fs.Close();
            doc.Close();
            enablePdw(app);
            app.Quit();
        }

        private word.Application createWord()
        {
            word.Application app = new word.Application();
            app.Visible = true;

            Thread.Sleep(2000);

            //disable the pdw plugin
            COMAddIn pdwAddin = null;
            COMAddIns addins = app.COMAddIns;
            foreach (COMAddIn addin in addins)
            {
                if (addin.Description.Equals("Pdw"))
                {
                    addin.Connect = false;
                    pdwAddin = addin;
                    break;
                }
            }
            Thread.Sleep(1000);
            return app;
        }

        private void enablePdw(word.Application app)
        {
            COMAddIns addins = app.COMAddIns;
            foreach (COMAddIn addin in addins)
            {
                if (addin.Description.Equals("Pdw"))
                {
                    addin.Connect = true;
                    break;
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            word.Application app = createWord();
            String wordFile = textBox10.Text;
            string xsltFile = textBox9.Text;
            word.Document doc = app.Documents.Open(wordFile);

            StreamReader sr = new StreamReader(xsltFile);
            string xsl = sr.ReadToEnd();
            //delete old xsl file
            foreach (CustomXMLPart xmlPart in doc.CustomXMLParts)
            {
                if (xmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(xmlPart.XML);
                if (customObject.ContentType == ContentType.PdwrXsl)
                {
                    xmlPart.Delete();
                    break;
                }
            }

            //add xsl file
            XmlObject xslObj = new XmlObject();
            xslObj.ContentType = ContentType.PdwrXsl;
            xslObj.Content = xsl;
            CustomXMLPart xmlpart_xsl = doc.CustomXMLParts.Add();
            xmlpart_xsl.LoadXML(ObjectSerializeHelper.SerializeToString<XmlObject>(xslObj));

            sr.Close();
            doc.Save();
            enablePdw(app);
            app.Quit();

        }

        private void button14_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = true;
            Workbook wb = excel.Workbooks.Add();
            Worksheet sheet = wb.ActiveSheet;
            ListObject tab1 = sheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange,sheet.get_Range("A1:D4"));
            PublishObject po = wb.PublishObjects.Add(XlSourceType.xlSourceRange, "e:\\1.htm", "Sheet1", "A1:D4", XlHtmlType.xlHtmlStatic, "wkbk_0009", "");
            Microsoft.Office.Interop.Excel.Shape shape = sheet.Shapes.AddChart(Microsoft.Office.Interop.Excel.XlChartType.xlPie);
            Microsoft.Office.Interop.Excel.Chart chart = shape.Chart;// (Microsoft.Office.Interop.Excel.Chart)
            chart.Export("e:\\11.png");
            //chart.ExportAsFixedFormat(XlFixedFormatType.xlTypeXPS, "e:\\111.xps");
            po.Publish(true);
            po.AutoRepublish = false;
            wb.Close(false);
            excel.Quit();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            ObjTest obj = FileUtils.DeserializeXmlFile<ObjTest>("e:\\210.xml");
            MessageBox.Show(obj.birthday.ToString());
        }

        private void button16_Click(object sender, EventArgs e)
        {
            ObjTest t = new ObjTest();
            t.age = 10;
            t.name = "张三";
            t.birthday = DateTime.Now;
            t.sex = false;
            List<Child> children = new List<Child>();
            children.Add(new Child());
            children.Add(new Child());
            t.children = children;
            t.strs = new List<string>();
            t.strs.Add("aaa");
            t.strs.Add("bbb");
            FileUtils.SerializeXmlFile<ObjTest>(t, "e:\\220.xml");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            tt t = new tt();
            lockme l = new lockme();
            t.lck = l;
            Thread th = new Thread(new ThreadStart(t.t2));
            th.Start();
            lock (l)
            {
                MessageBox.Show("lock in");
                Monitor.Wait(l);
            }
            MessageBox.Show("lock release");
        }
    }
    public class lockme
    {
    }

    public class tt
    {
        public lockme lck;
        public void t2()
        {
            Thread.Sleep(10000);
            lock (lck)
            {
                Monitor.Pulse(lck);
            }
        }
    }
}
