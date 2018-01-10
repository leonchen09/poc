using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Oracle.DataAccess.Client;

namespace PdwPlugin
{
    class BulkCopyTest
    {

        public void CopyData()
        {
            DataTable datas = new DataTable();
            OracleConnection inConn = getOrclConn();
            inConn.Open();
            OracleDataAdapter inAdp = new OracleDataAdapter("select table_name, column_name, data_type from user_tab_columns", inConn);
            inAdp.Fill(datas);

            SqlConnection outConn = getSqlConn();
            outConn.Open();
            using ( SqlTransaction tran = outConn.BeginTransaction( ) )
            {
                SqlBulkCopy bulkCopyOrders = new SqlBulkCopy( outConn , SqlBulkCopyOptions.Default , tran );
                bulkCopyOrders.DestinationTableName = "orcl_cols";

                bulkCopyOrders.ColumnMappings.Add("table_name", "table_name");
                bulkCopyOrders.ColumnMappings.Add("column_name", "column_name");
                bulkCopyOrders.ColumnMappings.Add("data_type", "data_type");

                bulkCopyOrders.BulkCopyTimeout = 1000;
                try
                {
                    bulkCopyOrders.WriteToServer( datas );
                    tran.Commit( );
                }
                finally
                {
                    datas = null;
                }
            }
            inConn.Close();
            outConn.Close();
        }

        public void CopyData2()
        {
            DataTable datas = new DataTable();
            SqlConnection inConn = getSqlConn();
            inConn.Open();
            SqlDataAdapter inAdp = new SqlDataAdapter("select id,name,xtype from syscolumns", inConn);
            inAdp.Fill(datas);

            OracleConnection outConn = getOrclConn();
            outConn.Open();
            using (OracleTransaction tran = outConn.BeginTransaction())
            {
                OracleBulkCopy bulkCopyOrders = new OracleBulkCopy(outConn, OracleBulkCopyOptions.Default);
                bulkCopyOrders.DestinationTableName = "sqlcolumns";

                bulkCopyOrders.ColumnMappings.Add("id", "id");
                bulkCopyOrders.ColumnMappings.Add("name", "name");
                bulkCopyOrders.ColumnMappings.Add("xtype", "xtype");

                bulkCopyOrders.BulkCopyTimeout = 1000;
                try
                {
                    bulkCopyOrders.WriteToServer(datas);
                    tran.Commit();
                }
                finally
                {
                    datas = null;
                }
            }
            inConn.Close();
            outConn.Close();
        }

        private OracleConnection getOrclConn()
        {
            string conStr = "Data Source=ORCL;user id=app;password=app";
            OracleConnection conn = new OracleConnection(conStr);
            return conn;
        }

        private SqlConnection getSqlConn()
        {
            string conStr = "Data Source=localhost;Database=app;user id=pdx;password=pdx";
            SqlConnection conn = new SqlConnection(conStr);
            return conn;
        }
    }
}
