using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlugInSQLUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.Visible = false;
            Form2 form2 = new Form2();
            Rectangle ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(this);
            form2.Left = ScreenArea.Width - 272;
            form2.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView1.ExpandAll();

            comboBox1.SelectedIndex = 1;

            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows[0].Cells[0].Value = "@CurEmpID";
            dataGridView1.Rows[0].Cells[1].Value = "int";
            dataGridView1.Rows[0].Cells[2].Value = "10";
            dataGridView1.Rows[0].Cells[4].Value = "1";

            dataGridView1.Rows[1].Cells[0].Value = "@VirtualParameter";
            dataGridView1.Rows[1].Cells[1].Value = "int";
            dataGridView1.Rows[1].Cells[2].Value = "10";
            dataGridView1.Rows[1].Cells[4].Value = "9";

            dataGridView2.Rows.Add();
            dataGridView2.Rows[0].Cells[0].Value = true;
            //DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dataGridView2.Rows[0].Cells[0];
            //chk.Selected = true;
            dataGridView2.Rows[0].Cells[1].Value = "Employee";
            dataGridView2.Rows[0].Cells[2].Value = "Employee111";


            dataGridView2.Rows.Add();
            //DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dataGridView2.Rows[0].Cells[0];
            //chk.Selected = true;
            dataGridView2.Rows[1].Cells[1].Value = "JobHistory";
            dataGridView2.Rows[1].Cells[2].ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("After user input or change sql, click this to parse new sql.");
            tabControl1.SelectedIndex = 2;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("After user input parameter value and sharding table, send sql to database to executed.");
            tabControl1.SelectedIndex = 3;
        }
    }
}
