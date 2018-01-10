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

namespace Test
{
    public partial class Form2 : Form
    {
        static readonly Encoding UTF8Encoding = new UTF8Encoding(false);
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string source = textBox1.Text;
            string unmatchtag;
            int endposition;
            FindAllUnmatchedTag(source, out unmatchtag, out endposition);
            textBox2.Text = unmatchtag+"\r\n" + endposition;
           
        }

        private void FindAllUnmatchedTag(string source, out string unmatchedTag, out int endPosition)
        {
            unmatchedTag = source;
            endPosition = source.Length;
            int charIndex = 0;
            char[] sourceCharArr = source.ToCharArray();
            Boolean inEndTag = false;//whether in endtag ,such as </span>,
            Boolean foundAll = false;//all the unmatched tag found.
            while (charIndex < source.Length && !foundAll)
            {
                switch (sourceCharArr[charIndex])
                {
                    case '<'://just ingore it. do nothing;

                        break;
                    case '/'://meaing in the start position of endtag
                        inEndTag = true;
                        break;
                    case '>':
                        if (inEndTag)//one tag finish, reset flag.
                        {
                            string temp = source.Substring(0, charIndex + 1);
                            if (temp.EndsWith("</p>"))
                            {
                                foundAll = true;
                                endPosition = charIndex - 3;//do not include </p>
                                unmatchedTag = source.Substring(0, endPosition);
                            }
                            inEndTag = false;
                        }
                        break;
                    default:
                        if (inEndTag)
                            ;//do nothing, just go to next char.
                        else
                        {
                            endPosition = charIndex;
                            unmatchedTag = source.Substring(0, endPosition);
                            if(unmatchedTag.EndsWith("<"))//meaning current position in <span,<bold and etc.
                            {
                                endPosition  = endPosition - 1;
                                unmatchedTag = unmatchedTag.Substring(0, endPosition);
                            }
                            foundAll = true;
                        }
                        break;

                }
                charIndex++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string xml = string.Empty;

            RenderParameterBase p = new RenderParameter();//new RenderParameterBase();
            p.Name = "name1";
            p.Value = new Object[]{true, false};
            p.sappid = 1;

            if (p != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = UTF8Encoding }))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(RenderParameter));
                        XmlAttributeAttribute att1 = new XmlAttributeAttribute();
                        att1.AttributeName = "ss";


                        serializer.Serialize(writer, p);
                    }

                    xml = UTF8Encoding.GetString(stream.ToArray());
                }
            }
            textBox1.Text = xml;
        }
    }
}
