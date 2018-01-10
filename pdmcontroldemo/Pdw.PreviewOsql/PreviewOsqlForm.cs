
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Pdw.PreviewOsql.Biz;
using Pdw.PreviewOsql.Entity;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;
using ProntoDoc.Framework.CoreObject.Render;
using ProntoDoc.Framework.CoreObject.Render.Value;

namespace Pdw.PreviewOsql
{
    public partial class PreviewOsqlForm : Form
    {
        public string Osql { get; set; }
        public string JOsql { get; set; }
        public string Xsl { get; set; }
        public string RenderArgument { get; set; }
        public string JRenderArgument { get; set; }
        public ChecksumInfo CheckSumInfo { get; set; }
        public RenderArgDomainValue SXRenderArgDomainValue { get; set; }
        public OsqlXml OsqlXml { get; set; }


        public PreviewOsqlForm()
        {
            InitializeComponent();
        }

        private void PreviewOsql_Load(object sender, EventArgs e)
        {
            BindDataToForm();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BindDataToForm()
        {
            txtOsql.Text = Osql;
            txtJOsql.Text = JOsql;
            txtXslt.Text = Xsl;
            txtRenderArgument.Text = RenderArgument;
            txtJRenderArgument.Text = JRenderArgument;
            txtMDB.Text = GetMDB();
            txtRenderArgDomain.Text = GetRenderArgumentValue();
        }

        private void BuildRender()
        {
            string msg = string.Empty;
            try
            {
                string mdbStr = txtMDB.Text;
                MDB mdb = new MDB();
                RenderArgumentValue renderArgValue = new RenderArgumentValue();

                try
                {
                    mdb = ObjectSerializeHelper.Deserialize<MDB>(mdbStr);
                }
                catch (Exception)
                { msg += "Format invalid: MDB !!!"; }


                string renderArgumentValue = txtRenderArgDomain.Text;
                try
                {
                    renderArgValue = ObjectSerializeHelper.Deserialize<RenderArgumentValue>(renderArgumentValue);
                }
                catch (Exception)
                { msg += "Format invalid: SXRenderArgumentValue !!!"; }

                //Get Osql data from db
                PreviewBiz previewBiz = new PreviewBiz();

                List<ChecksumInfoItem> lstCheckSumItems = this.CheckSumInfo.ChecksumInfoItems;
                Dictionary<string, string> dicOsqlXml = previewBiz.GetOsqlDataFromAppDB(lstCheckSumItems, renderArgValue, mdb, this.OsqlXml, ref msg);

                if (!string.IsNullOrEmpty(msg))
                    throw new Exception(msg);
                //Merge Osql data to string
                string osqlStr = Utility.MergeOsqlResultData(dicOsqlXml);
                XmlDocument xmlDoc = ObjectSerializeHelper.Deserialize<XmlDocument>(osqlStr);

                //Tranform to word
                TranformHelper.RenderFile(xmlDoc, Xsl);
            }
            catch (Exception)
            {
                MessageBox.Show(msg == string.Empty ? "Can't preview , check input data again !!!" : msg, "Error");
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            BuildRender();
        }

        private string GetRenderArgumentValue()
        {
            StringBuilder sbRenderArg = new StringBuilder();
            sbRenderArg.AppendLine("<?xml version=\"1.0\" encoding=\"utf-16\"?>");
            sbRenderArg.AppendLine("<RenderArgument xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            sbRenderArg.AppendLine("    <Domains>");
            sbRenderArg.AppendLine("        <Domain Name=\"EmployeeHistory\">");
            sbRenderArg.AppendLine("            <Parameters>");
            sbRenderArg.AppendLine("                <Parameter Name=\"@1\">");
            sbRenderArg.AppendLine("                <Value xsi:type=\"xsd:string\">1</Value>");
            sbRenderArg.AppendLine("                </Parameter>");
            sbRenderArg.AppendLine("            </Parameters>");
            sbRenderArg.AppendLine("            <DomainAlias />");
            sbRenderArg.AppendLine("        </Domain>");
            sbRenderArg.AppendLine("        <Domain Name=\"EmployeeSalary\">");
            sbRenderArg.AppendLine("            <Parameters>");
            sbRenderArg.AppendLine("                <Parameter Name=\"@1\">");
            sbRenderArg.AppendLine("                <Value xsi:type=\"xsd:string\">1</Value>");
            sbRenderArg.AppendLine("                </Parameter>");
            sbRenderArg.AppendLine("            </Parameters>");
            sbRenderArg.AppendLine("            <DomainAlias />");
            sbRenderArg.AppendLine("        </Domain>");
            sbRenderArg.AppendLine("    </Domains>");
            sbRenderArg.AppendLine("</RenderArgument>");
            return sbRenderArg.ToString();
        }

        private string GetMDB()
        {
            StringBuilder sbMdb = new StringBuilder();
            sbMdb.AppendLine("<MDB xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            sbMdb.AppendLine("  <AppDB DBID=\"SmallSchema/MS SQLServer\">");
            sbMdb.AppendLine("      <ConnectToAppDB>DBurl=192.168.2.99:SmallSchema; User=sa; Password=123456</ConnectToAppDB>");
            sbMdb.AppendLine("      <ConnectionPoolSize>3</ConnectionPoolSize>");
            sbMdb.AppendLine("  </AppDB>");
            sbMdb.AppendLine("</MDB>");
            return sbMdb.ToString();
        }
    }
}
