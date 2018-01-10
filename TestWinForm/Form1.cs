using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using System.Data.SqlClient;
using System.Xml.Xsl;

namespace TestWinForm
{
    public partial class MainForm : Form
    {
        private XmlDocument _xDoc;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            //string filePath = @"D:\NgocBV\Sample\TestWinForm\TestWinForm\bin\Debug\test.xml";
            //_xDoc = new XmlDocument();
            //_xDoc.Load(filePath);

            //XmlNode doc = _xDoc.DocumentElement;
            //XmlNode start = null;
            //XmlNode end = null;
            //MakeTreeNode(doc.ChildNodes, out start);

            //doc.AppendChild(start);

            //_xDoc.Save(filePath);
            //Application.Exit();
            //return;
            //_xDoc = new XmlDocument();
            //_xDoc.Load(txtFileName.Text);

            //XmlNode pdwNode = _xDoc.DocumentElement;
            //XmlNode p1Node = pdwNode.FirstChild;

            //string imgAtt = "AffiliateID";
            //string image = p1Node.Attributes[imgAtt].Value;
            //p1Node.Attributes[imgAtt].Value = ToBase64String(image);

            //_xDoc.Save(@"D:\POC\Image binary + xslt\replace.xml");
            string xmlFilePath = @"C:\Users\Bui Van Ngoc\AppData\Roaming\Omni Apps\ProntoDoc for Word\705924d0-4145-457c-8326-e6a2764af609.xml";
            string xslFilePath = @"C:\Users\Bui Van Ngoc\AppData\Roaming\Omni Apps\ProntoDoc for Word\705924d0-4145-457c-8326-e6a2764af609.xsl";
            Transform(xslFilePath, xmlFilePath);
            Application.Exit();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            string cnn = "Data Source=localhost;Initial Catalog=testImage;Integrated Security=True";
            // string sql = "select top 1 categoryname as AllowStoringCreditCardNumber,picture as AffiliateID from categories P1 for xml auto, root('PdwData'), binary base64";
            string sql = "delete tblImage;insert into tblImage values(@picture)";

            using (SqlConnection sqlCnn = new SqlConnection(cnn))
            {
                sqlCnn.Open();

                using (SqlCommand sqlCmm = new SqlCommand(sql, sqlCnn))
                {
                    Image imgFile = Image.FromFile(txtFileName.Text);
                    using (MemoryStream imgMem = new MemoryStream())
                    {
                        imgFile.Save(imgMem, System.Drawing.Imaging.ImageFormat.Jpeg);
                        sqlCmm.Parameters.Add("@picture", SqlDbType.Image, 0).Value = imgMem.ToArray();
                    }

                    sqlCmm.ExecuteNonQuery();
                }

                sqlCnn.Close();
            }
            Cursor = Cursors.Default;
            Application.Exit();
        }

        private void btnToBase64_Click(object sender, EventArgs e)
        {
            string content = string.Empty;
            int lineLength = 76;// 78 is the size of the OLE header for Northwind images, 76 is length of base64

            //using (StreamReader reader = new StreamReader(@"C:\Users\Bui Van Ngoc\AppData\Roaming\Omni Apps\ProntoDoc for Word\image without image\7f971416-c213-45e5-bb14-85847773ed6a.xml"))
            //{
            //    content = reader.ReadToEnd();
            //    reader.Close();
            //}

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"C:\Users\Bui Van Ngoc\AppData\Roaming\Omni Apps\ProntoDoc for Word\image without image\WithoutForeachWithImage_one line.xml");
            content = xDoc.DocumentElement.FirstChild.Attributes["image"].Value;

            lineLength = 30000;
            using (StreamWriter writer = new StreamWriter(@"C:\Users\Bui Van Ngoc\AppData\Roaming\Omni Apps\ProntoDoc for Word\image without image\WithoutForeachWithImage_30000 chars.txt"))
            {
                while (content.Length > lineLength)
                {
                    string temp = content.Substring(0, lineLength);
                    writer.WriteLine(temp);
                    content = content.Substring(lineLength);
                }

                writer.Write(content);

                writer.Close();
            }

            Application.Exit();
        }

        private string ToBase64String(string content)
        {
            string result = string.Empty;
            int lineLength = 76;

            while (content.Length > lineLength)
            {
                string temp = content.Substring(0, lineLength);
                result = result + temp + "\r\n";
                content = content.Substring(lineLength);
            }

            result = result + content;

            return result;
        }

        private void MakeTreeNode(XmlNodeList nodes, out XmlNode startNode)
        {
            string variantName = string.Empty;
            startNode = nodes[0].CloneNode(true);
            AddVariantNode(startNode, 0, out variantName);
            XmlNode endNode = null;
            for (int index = 1; index < nodes.Count; index++)
            {
                endNode = (endNode == null ? startNode : endNode).AppendChild(nodes[index].CloneNode(true));
                AddVariantNode(endNode, index, out variantName);
            }

            if (endNode != null)
            {
                string prefix = DateTime.Now.ToString("yyyyMMddhhmmssff");
                XmlNode relation = endNode.OwnerDocument.CreateElement("Relationship");

                XmlAttribute relationId = endNode.OwnerDocument.CreateAttribute("Id");
                relationId.Value = "r" + prefix + "{$" + variantName + "}";
                relation.Attributes.Append(relationId);

                XmlAttribute relationTarget = endNode.OwnerDocument.CreateAttribute("Target");
                relationTarget.Value = "media/image" + prefix + "{$" + variantName + "}.jpeg";
                relation.Attributes.Append(relationTarget);

                endNode.AppendChild(relation);
            }
        }

        private void AddVariantNode(XmlNode foreachNode, int index, out string variantName)
        {
            XmlNode variant = foreachNode.OwnerDocument.CreateElement("variable");
            
            XmlAttribute varName = foreachNode.OwnerDocument.CreateAttribute("name");
            variantName = "pdwImg" + (index + 1).ToString();
            varName.Value = variantName;
            variant.Attributes.Append(varName);

            XmlAttribute varSelect = foreachNode.OwnerDocument.CreateAttribute("select");
            varSelect.Value = index == 0 ? "10*position()" : "$pdwImg" + index.ToString() + " + position()";
            variant.Attributes.Append(varSelect);

            foreachNode.AppendChild(variant);
        }

        private void Transform(string xslFile, string xmlFile)
        {
            try
            {
                XslCompiledTransform transformer = new XslCompiledTransform();
                transformer.Load(xslFile);
                transformer.Transform(xmlFile, xmlFile + ".out.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(@"E:\ProntoDir\pocdata.xml");
            XmlNode mainnode = xmldoc.SelectSingleNode("pdepoc");
            string s = mainnode.InnerText;

            s += mainnode.ChildNodes.Count;
            label3.Text = s;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

    }
}
