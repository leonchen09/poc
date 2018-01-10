using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WcfClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ServiceReference1.Service1Client TClient = new ServiceReference1.Service1Client(); 
        private void button1_Click(object sender, EventArgs e)
        {
            int i = int.Parse(textBox1.Text);
            textBox1.Text = TClient.GetData(i);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int i = int.Parse(textBox2.Text);
            int j = int.Parse(textBox3.Text);
            textBox3.Text = TClient.sum(i, j).ToString();
        }
    }
}
