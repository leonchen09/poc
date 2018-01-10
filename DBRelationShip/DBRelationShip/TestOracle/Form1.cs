using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;

namespace TestOracle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string connString = "User ID=app;Password=app;Data Source=(DESCRIPTION = (ADDRESS_LIST= (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.199.254)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = orcl)))";
            string connString = richTextBox1.Text;
            OracleConnection conn = new OracleConnection(connString);
            string sql = "select * from catype t";
            try
            {
                conn.Open();
                DataTable ds = new DataTable();
                OracleDataAdapter adapter = new OracleDataAdapter(sql, conn);
                adapter.Fill(ds);
                bindingSource1.DataSource = ds;
                dataGridView1.DataSource = bindingSource1;
            }
            finally
            {
                conn.Close();
            }
        }


    }
}
