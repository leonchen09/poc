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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            treeView1.ExpandAll();
            comboBox1.SelectedIndex = 0;
        }
    }
}
