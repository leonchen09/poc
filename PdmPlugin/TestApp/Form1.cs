using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using word = Microsoft.Office.Interop.Word;
using System.Xml;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Test t = new Test();
            t.importXsd(t.createApp());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //word.Application app = new word.Application();
            //app.Visible = true;
            //word.Document doc = app.Documents.Open("");
            XmlDocument htm = new XmlDocument();
            htm.Load(@"E:\ProntoDir\pdm\Doc1.htm");
            XmlNodeList activeXs = htm.SelectNodes("//object[starts-with(@classid,'CLSID')]");
            foreach(XmlNode actX in activeXs)
            {
                XmlNode parentNode = actX.ParentNode;
                XmlElement newInput = htm.CreateElement("input");
                newInput.SetAttribute("name", "input1");
                newInput.SetAttribute("type", "text");
                parentNode.ReplaceChild(newInput, actX);
            }

        }
    }
}
