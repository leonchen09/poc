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
    public partial class ScopeTree : Form
    {
        public ScopeTree()
        {
            InitializeComponent();
            treeView1.ExpandAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (new Form2()).Show();
            TreeNode t2 = treeView1.Nodes[0].Nodes[0].Nodes[0];
            t2.ImageIndex = 1;
            t2.Text = "Table2(Left Join)";
            t2.Nodes[0].ImageIndex = 1;
            t2.Nodes[0].Text = "Table3(Inner Join)";
            t2.Nodes[0].Nodes[0].ImageIndex = 1;
            t2.Nodes[0].Nodes[0].Text = "Table4(Inner Join)";
            TreeNode t21 = treeView1.Nodes[0].Nodes[1].Nodes[0].Nodes[0];
            t21.ImageIndex = 1;
            t21.Text = "Table2(Left Join)";
            t21.Nodes[0].ImageIndex = 1;
            t21.Nodes[0].Text = "Table3(Inner Join)";
            t21.Nodes[0].Nodes[0].ImageIndex = 1;
            t21.Nodes[0].Nodes[0].Text = "Table4(Inner Join)";
        }
    }
}
