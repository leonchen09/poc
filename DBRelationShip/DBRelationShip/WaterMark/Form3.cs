using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;
using System.Data.OracleClient;

namespace WaterMark
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connection = "Data Source=.;User ID=pdx;Password=pdx;Database=app";
            SqlConnection sqlConnection = new SqlConnection(connection);
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();
            string sql = "select * from testencode where name='name姓名' for xml auto";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sql,sqlConnection);
            DataTable dtResult = new DataTable();
            dataAdapter.Fill(dtResult);
            dataGridView1.DataSource = dtResult;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string connection = "Data Source=.;User ID=pdx;Password=pdx;Database=app2";
            SqlConnection sqlConnection = new SqlConnection(connection);
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();
            string sql = "select * from testencode for xml auto";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, sqlConnection);
            DataTable dtResult = new DataTable();
            dataAdapter.Fill(dtResult);
            dataGridView1.DataSource = dtResult;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //string connection = "Data Source=localhost;User ID=root;Password=mysql;Database=app";
            //MySqlConnection sqlConnection = new MySqlConnection(connection);
            //if (sqlConnection.State != System.Data.ConnectionState.Open)
            //    sqlConnection.Open();
            //string sql = "select * from testencode ";
            //MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sql, sqlConnection);
            //DataTable dtResult = new DataTable();
            //dataAdapter.Fill(dtResult);
            //dataGridView1.DataSource = dtResult;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string connection = "Data Source=orcl;User ID=app;Password=app";
            OracleConnection sqlConnection = new OracleConnection(connection);
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();
            string sql = "select * from testencode";
            OracleDataAdapter dataAdapter = new OracleDataAdapter(sql, sqlConnection);
            DataTable dtResult = new DataTable();
            dataAdapter.Fill(dtResult);
            dataGridView1.DataSource = dtResult;
        }
    }
}
