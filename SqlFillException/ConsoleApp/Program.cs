using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;

namespace ConsoleApp
{

    class Program
    {
        //[ThreadStatic]
        static SqlConnection con = null;

        static SqlCommand cmd = null;

        private static string cs = @"server=John;uid=sa; pwd=12345; database=Northwind";


        static void Main(string[] args)
        {
            Dictionary<Guid, int> dict = new Dictionary<Guid,int>();
            for (int i = 0; i < 1000000; i++) {
                Console.WriteLine(i);
                dict.Add(Guid.NewGuid(), i);                
            }
            //RecontructException();
        }

        private static void RecontructException()
        {            
            try
            {
                string sql = "SELECT CustomerID, ContactName FROM Customers";
                using (SqlConnection con = new SqlConnection(Program.cs))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    //SqlDataReader rdr = cmd.ExecuteReader();
                    using(SqlDataReader rdr = cmd.ExecuteReader()){
                        while (rdr.Read())
                        {
                            Console.WriteLine("cid: {0}, ctext: {1}", rdr[0].ToString(), rdr[1].ToString());
                        } 
                    }                                       
                    Console.ReadLine();                                      
                    // Now, trying to use the connection again will throw exception.
                    string update = "UPDATE Customers SET ContactName = 'New Value' WHERE CustomerID = 'ALFKI'";
                    cmd.CommandText = update;
                    cmd.ExecuteNonQuery();
                    con.Close();

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
    }
}
