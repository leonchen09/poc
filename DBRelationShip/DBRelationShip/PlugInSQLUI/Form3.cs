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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void splitContainer4_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
            tabControl2.Cursor = Cursors.Default;
            tabControl3.Cursor = Cursors.Default;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows[0].Cells[0].Value = "@CurEmpID";
            //dataGridView1.Rows[0].Cells[1].Value = "int";
            //dataGridView1.Rows[0].Cells[2].Value = "10";
            //dataGridView1.Rows[0].Cells[4].Value = "1";

            dataGridView1.Rows[1].Cells[0].Value = "@VirtualParameter";
            //dataGridView1.Rows[1].Cells[1].Value = "int";
            //dataGridView1.Rows[1].Cells[2].Value = "10";
            //dataGridView1.Rows[1].Cells[4].Value = "9";

            dataGridView2.Rows.Clear();
            dataGridView2.Rows.Add();
           // dataGridView2.Rows[0].Cells[0].Value = true;
            dataGridView2.Rows[0].Cells[1].Value = "Employee";
            //dataGridView2.Rows[0].Cells[2].Value = "Employee111";


            dataGridView2.Rows.Add();
            //DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dataGridView2.Rows[0].Cells[0];
            //chk.Selected = true;
            dataGridView2.Rows[1].Cells[1].Value = "JobHistory";
            dataGridView2.Rows[1].Cells[2].ReadOnly = true;


        }

        private void button5_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();

            dataGridView3.Rows.Clear();
            dataGridView3.Rows.Add();
            dataGridView3.Rows[0].Cells[0].Value = 1;
            dataGridView3.Rows[0].Cells[1].Value = "Joe";
            dataGridView3.Rows[0].Cells[2].Value = "1999-12-31";
            dataGridView3.Rows[0].Cells[3].Value = "Jack";
            dataGridView3.Rows[0].Cells[4].Value = "1999-1-1";

            dataGridView3.Rows.Add();
            dataGridView3.Rows[1].Cells[0].Value = 1;
            dataGridView3.Rows[1].Cells[1].Value = "Joe";
            dataGridView3.Rows[1].Cells[2].Value = "1999-12-31";
            dataGridView3.Rows[1].Cells[3].Value = "Jack";
            dataGridView3.Rows[1].Cells[4].Value = "1990-2-2";

            dataGridView3.Rows.Add();
            dataGridView3.Rows[2].Cells[0].Value = 1;
            dataGridView3.Rows[2].Cells[1].Value = "Joe";
            dataGridView3.Rows[2].Cells[2].Value = "1997-6-8";
            dataGridView3.Rows[2].Cells[3].Value = "Sam";
            dataGridView3.Rows[2].Cells[4].Value = "1990-2-2";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            Rectangle ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(this);
            form2.Left = ScreenArea.Width - 272;
            form2.Show();
            form2.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
