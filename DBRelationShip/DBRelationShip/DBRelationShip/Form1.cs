using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OfficeOperate;
using Pub.Class;

namespace DBRelationShip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox3.Items.Add("");
            for (int i = 0; i < 5; i++)
                comboBox3.Items.Add(((ComboBox)sender).Text + "column" + i);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();
            comboBox4.Items.Add("");
            for (int i = 0; i < 5; i++)
                comboBox4.Items.Add(((ComboBox)sender).Text + "column" + i);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }


        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox2.AppendText(" " + richTextBox1.Text);
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            richTextBox2.AppendText(richTextBox1.Text);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (comboBox3.Text.Length < 1)
                return;
            if (comboBox4.Text.Length < 1)
                return;
            if ((richTextBox1.Text == null) || ((int)richTextBox1.Text.Length < 1))
            {
                richTextBox1.AppendText(comboBox3.Text + "=" + comboBox4.Text);
                //richTextBox2.AppendText(comboBox3.Text + "=" + comboBox4.Text);
            }
            else
            {
                richTextBox1.AppendText(" AND " + comboBox3.Text + "=" + comboBox4.Text);
                //richTextBox2.AppendText(" AND " + comboBox3.Text + "=" + comboBox4.Text);
            }
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
        }
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 1)
            {
                richTextBox2.Clear();
                if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 3)
                    richTextBox2.AppendText("TableA LEFT JOIN TableB ON ");
                else
                    richTextBox2.AppendText("TableA INNER JOIN TableB ON ");
                richTextBox2.AppendText(richTextBox1.Text);
            }
        }

        private void button4_Click_2(object sender, EventArgs e)
        {
            WordOperate.copyHeaderFooter(richTextBox1.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            WordOperate.TestMht();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String fileName = richTextBox1.Text;
            System.Drawing.Image image = System.Drawing.Image.FromFile(fileName);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(image, 0, 0, image.Width, image.Height);
            Font f = new Font("Verdana", 32);
            Brush b = new SolidBrush(Color.Red);
            g.DrawString("Leonchen", f, b, 10, 10);
            g.Dispose();
            string spath = fileName.Substring(0, fileName.LastIndexOf(".")) + DateTime.Now.ToString("MMddHHmmss") + fileName.Substring(fileName.LastIndexOf("."));
            image.Save(spath);
            image.Dispose();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            WordOperate.insertPic("e:\\ProntoDir\\wordpicture.docx", "e:\\ProntoDir\\Repository.PNG");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            WordOperate.getPictures("e:\\ProntoDir\\wordpicture.docx");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string base64str = @"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
eWw3xpR4AAAAAElFTkSuQmCC";
            WordXmlPic.convertFromBase64(base64str, "e:\\ProntoDir\\image2.png");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            String base64str = WordXmlPic.convertToBase64("e:\\ProntoDir\\Repository.PNG");
            richTextBox1.Text = base64str;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            WordXmlPic.updateXMLPic("e:\\ProntoDir\\2007.xml", "image1.png", "e:\\ProntoDir\\Repository.PNG");
        }
    }
}
