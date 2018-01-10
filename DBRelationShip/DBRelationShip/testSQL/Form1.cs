using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Oracle.DataAccess.Client;

namespace testSQL
{
    public partial class Form1 : Form
    {
        KeyboardHook keyboardHook = new KeyboardHook();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string StringConnection = "database=SmallSchema;server=localhost;uid=pdx; pwd=pdx;";
            SqlConnection sqlConnection = new SqlConnection(StringConnection);
            //SqlCommand oSqlCommand = new SqlCommand(@"SELECT e.[EmployeeID],e.[LastName],e.[FirstName],e.[BirthDate],e.[NationalityCode],e.[Email],e.[Language] FROM [SmallSchema].[dbo].[Employee] e Full outer join [Employee] e1 on 1=1 full join Employee e2 on 1=1 full join Employee on 1=1 ");//for XML RAW, ELEMENTS XSINIL,BINARY BASE64");
            SqlCommand oSqlCommand = new SqlCommand(@"select * from Table1");
            //oSqlCommand.Parameters.AddWithValue("@DiscardRenderRequest", 0);
            sqlConnection.Open();
            DataTable dtResult = new DataTable();
            oSqlCommand.Connection = sqlConnection;

            SqlDataReader reader = oSqlCommand.ExecuteReader();
            int columnCount = reader.FieldCount;
            int i = 0;
            while (i < columnCount)
            {
                MessageBox.Show("column " + i + ": " + reader.GetDataTypeName(i));
                i++;
            }
            reader.Close();
            return;

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(oSqlCommand))
            {
                dataAdapter.Fill(dtResult);
            }
            StringBuilder result = new StringBuilder();
            foreach (DataRow row in dtResult.Rows)
            {
                result.Append(row[0].ToString());
            }
            richTextBox1.Text = result.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string StringConnection = @"Data Source = 
              (DESCRIPTION =
                (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                (CONNECT_DATA =
                  (SERVER = DEDICATED)
                  (SERVICE_NAME = orcl)
                )
              );User id=smallschema; password=smallschema;";
            OracleConnection sqlConnection = new OracleConnection(StringConnection);
            sqlConnection.Open();
//            OracleCommand oCommand = new OracleCommand(@" IF(  0 = :DiscardRenderRequest ) 
//             BEGIN 
//              select * from customer where 1=1  
//             END 
//             ELSE 
//             BEGIN 
//              select * from customer where 1=2 
//             END ");
            OracleCommand oCommand = new OracleCommand(@"select * from table1");
            oCommand.Connection = sqlConnection;

            OracleDataReader reader = oCommand.ExecuteReader();
            int columnCount = reader.FieldCount;
            int i = 0;
            while (i < columnCount)
            {
                MessageBox.Show("column " + i + ": " + reader.GetDataTypeName(i));
                i++;
            }
            reader.Close();
            return;

            oCommand.CommandType = CommandType.Text;
            OracleParameter p1 = new OracleParameter(":DiscardRenderRequest", OracleDbType.Int16, 0);
            p1.Direction = ParameterDirection.Input;
            oCommand.Parameters.Add(p1);
            
            DataTable dtResult = new DataTable();
            
            using (OracleDataAdapter dataAdapter = new OracleDataAdapter(oCommand))
            {
                dataAdapter.Fill(dtResult);
            }
            richTextBox1.Text = dtResult.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string str = "<row>0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij</row>";
            FileBufferedStream fstr = new FileBufferedStream();
            fstr.Append("<rows>");
            int i = 0;
            while (i < 100)
            {
                i++;
                fstr.Append(str);
            }
            fstr.Append("</rows>");

            Class1 c1 = new Class1();
            c1.age = 20;
            c1.name = "anme";
            c1.content = fstr;

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer =
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            FileStream stream = new FileStream(@"e:\1.tmp", FileMode.CreateNew, FileAccess.ReadWrite);
            serializer.Serialize(stream, c1);
            stream.Close();

            FileStream stream2 = new FileStream(@"e:\1.tmp", FileMode.Open, FileAccess.ReadWrite);
            Class1 c12 = serializer.Deserialize(stream2) as Class1;

            richTextBox1.Text = c12.age + "," + c12.name + "," + c12.content.Length;

            //XmlElement node = ObjectSerializeHelper.Deserialize<XmlElement>("<row>ddddd</row>");
            //MessageBox.Show("name:" + node.LastChild.Name);

            byte[] bytes = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            //MemoryStream ms = new MemoryStream(bytes);
            //ms.Write(bytes, 0, bytes.Length);
            fstr.Seek(0, SeekOrigin.Begin);
            //StreamReader sr = new StreamReader(ms, encoding: Encoding.UTF8);
            //string str2 = Encoding.UTF8.GetString(ms.ToArray());
            //TextReader tr = new StringReader(str2);

            XmlElement node = Deserialize<XmlElement>(fstr);
            fstr.Dispose();
            //XmlElement node = Deserialize<XmlElement>(fstr.GetStream());
            //XmlElement node = ObjectSerializeHelper.Deserialize<XmlElement>(str2);
            MessageBox.Show("name:" + node.LastChild.Name);
            Stream ss = SerializeToString<XmlElement>(node);
            ss.Seek(0, SeekOrigin.Begin);
            XmlElement nd = Deserialize<XmlElement>(ss);
            MessageBox.Show("name2:" + nd.LastChild.Name);
            ss.Dispose();
        }

        public static T Deserialize<T>(Stream stream)
        {
            T obj = default(T);
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                obj = (T)serializer.Deserialize(reader);
            }
            return obj;
        }

        public static T Deserialize<T>(TextReader reader1)
        {
            T obj = default(T);

            using (XmlReader reader = XmlReader.Create(reader1))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                obj = (T)serializer.Deserialize(reader);
            }

            return obj;
        }

        public static T Deserialize<T>(StreamReader reader1)
        {
            T obj = default(T);

            //using (XmlReader reader = XmlReader.Create(reader1))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));

                    obj = (T)serializer.Deserialize(reader1);
                }

            return obj;
        }

        public static Stream SerializeToString<T>(T obj)
        {
            string xml = string.Empty;
            FileBufferedStream stream = new FileBufferedStream();
            if (obj != null)
            {
                //using ()
                {
                    using (XmlWriter writer = XmlWriter.Create(stream))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        serializer.Serialize(writer, obj);
                    }

                    //return "";
                }
            }

            return stream;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //string str = "<row>0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij0123456789zbcdefghij</row>";
            //MemoryStream ms = new MemoryStream();
            //byte[] bytes = Encoding.UTF8.GetBytes(str);
            ////fstr.Append("<row>");
            //int i = 0;
            //while (i < 3095152)
            //{
            //    i++;
            //    ms.Write(bytes, 0, bytes.Length);
            //}
            //fstr.Append("</row>");

            //XmlElement node = ObjectSerializeHelper.Deserialize<XmlElement>("<row>ddddd</row>");
            //MessageBox.Show("name:" + node.LastChild.Name);
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlNode root = xmlDoc.CreateElement("first");
            XmlNode zh = xmlDoc.CreateElement("中文");
            XmlText zh1 = xmlDoc.CreateTextNode("z中文2");
            xmlDoc.AppendChild(root);
            root.AppendChild(zh);
            zh.AppendChild(zh1);
            xmlDoc.Save(@"e:\1.xml");
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            keyboardHook.InitHook();
        }
    }
}
