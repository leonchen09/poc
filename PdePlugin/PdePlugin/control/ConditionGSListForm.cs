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
    public partial class ConditionGSListForm : Form
    {
        public ConditionGoalSeek condGS { get; set; }

        public ConditionGSListForm()
        {
            InitializeComponent();
        }

        public void initData()
        {
            List<CondGoalSeekItem> cgsinfos = condGS.cgsInfo.CGSInfos;
            if (cgsinfos.Count > 0)
                dataGridView1.Rows.Add(cgsinfos.Count);
            for (int i = 0; i < cgsinfos.Count; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = cgsinfos[i].seekName;
                dataGridView1.Rows[i].Cells[2].Value = cgsinfos[i].ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value))
                {
                    string seekname = dataGridView1.Rows[i].Cells[1].Value as string;
                    dataGridView1.Rows.RemoveAt(i);
                    i--;
                    List<CondGoalSeekItem> cgsinfos = condGS.cgsInfo.CGSInfos;
                    foreach (CondGoalSeekItem item in cgsinfos)
                    {
                        if (item.seekName.Equals(seekname))
                        {
                            cgsinfos.Remove(item);
                            break;
                        }
                    }
                }
            }
            //this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
