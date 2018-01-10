using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestExpression
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            richTextBox2.Focus();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Save the experssion.This message can be show only in UI prototype to descibe the function.");
            richTextBox3.Clear();
            richTextBox3.Text = @"SELECT " + richTextBox1.Text + @", column2, column3, column4 FROM table1 LEFT JOIN  table2 ON table1.column2 = table2.column4 WHERE ";
            //if (richTextBox2.Text.Length > 0)
            //    richTextBox3.AppendText(richTextBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox2.Text.Trim().Length < 1)
            {
                MessageBox.Show("Please input where condition first.");
                richTextBox2.Focus();
                return;
            }
                
            richTextBox3.AppendText(richTextBox2.Text);
            MessageBox.Show("Connection to Applicaiton database to test the expression using sql. This message can be show only in UI prototype to descibe the function.");
            textBox1.Text = "True";
            DataTable mytable = dataSet1.Tables["Table1"];
            DataRow myrow = mytable.Rows.Add();
            myrow["Column1"] = "True";
            myrow["Column2"] = "aaa";
            myrow["Column3"] = "bb";
            myrow["Column4"] = "2.2";
            //mytable.Rows.Add();
            dataGridView1.Refresh();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox2.Focus();
        }
    }
}
