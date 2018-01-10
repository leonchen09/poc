using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using ProntoDoc.Framework.CoreObject;

namespace PdePlugin.control
{
    public partial class ExpTableForm : Form
    {

        public List<TableColumnMap> cols { get; set; }

        public ExpTableForm()
        {
            InitializeComponent();
        }

        public void initDataSource(ListColumns cols)
        {
            dataGridView1.Rows.Add(cols.Count);
            for (int i = 0; i < cols.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = true;
                dataGridView1.Rows[i].Cells[1].Value = cols.get_Item(i + 1).Name;
                dataGridView1.Rows[i].Cells[2].Value = getDataType(cols.get_Item(i + 1).DataBodyRange.Cells[1][1]);
                dataGridView1.Rows[i].Cells[3].Value = cols.get_Item(i + 1).Name;
            }
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Columns[1].ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<TableColumnMap> cols = new List<TableColumnMap>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                Boolean selected = Convert.ToBoolean( dataGridView1.Rows[i].Cells[0].Value );
                if (selected)
                {
                    cols.Add(new TableColumnMap(dataGridView1.Rows[i].Cells[1].Value as string, dataGridView1.Rows[i].Cells[2].Value as string, dataGridView1.Rows[i].Cells[3].Value as string));
                }
            }
            this.cols = cols;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string getDataType(Range cell)
        {
            string result = "";
            WorksheetFunction func = cell.Application.WorksheetFunction;
            if (func.IsLogical(cell))
                result = "boolean";
            else if (func.IsNumber(cell))
            {
                try
                {
                    Convert.ToDateTime(cell.Text);
                    result = "date";
                }
                catch
                {
                    result = "decimal";
                }
            }
            else 
                result = "string"; //default type is string
            return result;
        }
    }
}
