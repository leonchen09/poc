using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace PdePlugin.control
{
    public partial class TableMapForm : Form
    {
        public ProcessMap processMap { get; set; }

        public string parentNodePath { get; set; }

        public TableMapForm()
        {
            InitializeComponent();
        }

        public void InitDataSource(ListColumns cols, TreeNodeCollection nodes)
        {
            dataGridView1.Rows.Add(cols.Count);
            DataGridViewComboBoxColumn col = dataGridView1.Columns[1] as DataGridViewComboBoxColumn;
            for(int i = 0; i < nodes.Count; i ++)
            {
                col.Items.Add(nodes[i].Name);
            }
            for (int i = 0; i < cols.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = cols.get_Item(i + 1).Name;
            }
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Columns[0].ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //check the tree node user selected, to make sure no duplicate map.

            //construct column and treenode mapinfo.
            List<TabCol> mapCols = new List<TabCol>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string treeNodeVal = dataGridView1.Rows[i].Cells[1].Value as string;
                if (treeNodeVal != null && treeNodeVal.Length > 0)
                {
                    mapCols.Add(new TabCol(dataGridView1.Rows[i].Cells[0].Value as string, parentNodePath + "\\" + treeNodeVal));
                }
            }
            if (mapCols.Count > 0)
                processMap.mapCols = mapCols;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
