using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using docx = Microsoft.Office.Interop.Word;
using System.IO;

namespace PdxDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //docx.Application word = new docx.Application();
            //word.Visible = false;
            //docx.Document doc = word.Documents.Open("");
            //doc.CustomXMLParts.get_Item(1).xml
            Byte[] data = Convert.FromBase64String(richTextBox1.Text);
            MemoryStream m = new MemoryStream(data);
            FileStream fs = new FileStream("f:\\1.xlsx", FileMode.OpenOrCreate);
            m.WriteTo(fs);
            m.Close();
            fs.Close();
            m = null;
            fs = null; 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            docx.Application word = new docx.Application();
            word.Visible = true;
            docx.Document doc = word.Documents.Open(richTextBox1.Text);
            //docx.Table dt = doc.Tables[1].
        }
    }
}
