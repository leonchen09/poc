using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PdePlugin.control
{
    public partial class ConditionGSForm : Form
    {
        public ConditionGoalSeek condGS { get; set; } 

        public ConditionGSForm()
        {
            InitializeComponent();
        }

        //validate.
        private void button3_Click(object sender, EventArgs e)
        {
            string targetValue = textBox2.Text;
            string targetCell = textBox1.Text;
            string variableCell = textBox3.Text;
            condGS.seek(targetCell, variableCell, targetValue);
        }

        //ok button
        private void button1_Click(object sender, EventArgs e)
        {
            CondGoalSeekItem CGSItem = new CondGoalSeekItem();
            List<CondidtionItem> conds = new List<CondidtionItem>();
            CGSItem.conditionList = conds;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                CondidtionItem item = new CondidtionItem();
                string condCell = dataGridView1.Rows[i].Cells[0].Value as string;
                if (condCell == null || condCell.Length < 1)
                    continue;
                condGS.workbook.Application.get_Range(condCell).Select();
                item.condCell = condGS.GetNameForRange(condGS.workbook.Application);
                item.operatorStr = dataGridView1.Rows[i].Cells[1].Value as string;
                item.condValue = dataGridView1.Rows[i].Cells[2].Value as string;
                conds.Add(item);
            }
            condGS.workbook.Application.get_Range(textBox1.Text).Select();
            CGSItem.targetCell = condGS.GetNameForRange(condGS.workbook.Application);
            CGSItem.targetValue = textBox2.Text;
            condGS.workbook.Application.get_Range(textBox3.Text).Select();
            CGSItem.variableCell = condGS.GetNameForRange(condGS.workbook.Application);
            CGSItem.seekName = textBox4.Text;

            condGS.cgsInfo.CGSInfos.Add(CGSItem);

            this.Close();
        }

        //cancel;
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
