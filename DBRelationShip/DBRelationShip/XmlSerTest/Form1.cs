using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XmlSerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Obj1 o = new Obj1();
            o.id = 1;
            o.name = "name";
            o.birthday = DateTime.Now;
            o.d1 = 10.1;
            o.f1 = 1000000000.99f;
            o.dec1 = 1.000000099999111111m;
            o.status = objstatus.loading;
            o.strArray = new List<string>(){"a","b", "c"};
            o.intArray = new List<int>() { 1, 3, 5, 8 };
            List<Obj1> os = new List<Obj1>();
            os.Add(o);
            os.Add(o);
            os.Add(o);
            //SerializeXmlFile<Obj1>(o, @"e:\1.xml");
            SerializeXmlFile<List<Obj1>>(os, @"e:\1.xml");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Obj1 o1 = DeserializeXmlFile<Obj1>(@"e:\1.xml");
            //richTextBox1.Text = o1.ToString();
            List<Obj1> os = DeserializeXmlFile<List<Obj1>>(@"e:\1.xml");
            richTextBox1.Text = os[2].ToString();
        }

        public static void SerializeXmlFile<T>(T obj, string filePath)
        {
            FileStream fs = null;
            XmlTextWriter xmlTextWriter = null;
            try
            {
                XmlSerializer ser = new XmlSerializer(obj.GetType());
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                xmlTextWriter = new XmlTextWriter(fs, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                ser.Serialize(xmlTextWriter, obj);
                xmlTextWriter.Flush();
                if (xmlTextWriter != null)
                    xmlTextWriter.Close();
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public static T DeserializeXmlFile<T>(string filePath)
        {
            FileStream fs = null;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);

                T xmlContent = (T)ser.Deserialize(fs);

                return xmlContent;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //FileInfo fi = new FileInfo(@"D:\work\source\POC\DBRelationShip\DBRelationShip\XmlSerTest\bin\Debug\XmlSerTest.exe");
            //System.Diagnostics.FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(@"D:\work\source\pronto\java\ProntoMobile\bin\ProntoMobileActivity.apk");
            //richTextBox1.Text = fv.FileDescription;
            string maxstr = "";
            try
            {
                maxstr = new string('a', 24750427);
            }
            catch (Exception ex)
            {

            }
            string maxstr2 = maxstr;
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }


    }
}
