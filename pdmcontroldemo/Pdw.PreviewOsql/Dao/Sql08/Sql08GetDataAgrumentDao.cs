using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;
using Pdw.PreviewOsql.Entity;
using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.CoreObject.Render;
using ProntoDoc.Framework.Render;
using ProntoDoc.Framework.CoreObject.Render.Schema;
using ProntoDoc.Framework.CoreObject.Render.Value;

namespace Pdw.PreviewOsql.Dao.Sql08
{
    class Sql08GetDataAgrumentDao : BaseDao
    {
        #region Get Osql Data
        public string GetDataAgrumentValueFromAppDB(ChecksumInfoItem item, string osql, RenderArgDomainValue renderArgumentValue, string conn)
        {
            string osqlData = string.Empty;
            //Retrieve data from Application database
            XmlReader xmlReader = null;
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = osql;//osqlQuery always not null or empty.

                if (item.RenderArgument != null && renderArgumentValue != null)
                {
                    for (int i = 0; i < item.RenderArgument.Parameters.Count; i++)
                    {
                        RenderParameterSchema sxParam = item.RenderArgument.Parameters[i];
                        SqlParameter sqlParameter = CoreRenderHelper.CreateSqlParamForAnArgument(sxParam, renderArgumentValue);
                        command.Parameters.Add(sqlParameter);
                    }
                }

                //Add System Supplier param
                SystemParameter systemParameter = item.SystemParameter;
                AddSystemSupplier(ref command, systemParameter);

                //Add Watermark string
                command.Parameters.Add(CoreRenderHelper.CreateAppSqlParam("@" + FrameworkConstants.PdwWatermark, "WaterMark", SqlDbType.NVarChar));

                //Add param RenderArgumentX , RenderArgumentY
                AddParamRenderArgumentXY(ref command, renderArgumentValue, item);

                string connectionStr = base.GetConnectionString(conn.Trim());
                xmlReader = ExecuteXmlReader(connectionStr, command);
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            #region Transfer data to StringBuilder object

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmlReader);
            xmlReader.Close();

            osqlData = xDoc.InnerXml;

            #endregion

            return osqlData;
        }

        private void AddSystemSupplier(ref SqlCommand command, SystemParameter systemParameter)
        {
            if (systemParameter.HasTemplateId)
                command.Parameters.Add(CoreRenderHelper.CreateAppSqlParam("@" + FrameworkConstants.PluginSystemInfo.TemplateID, "TemplateID", SqlDbType.NVarChar));

            if (systemParameter.HasTemplateName)
                command.Parameters.Add(CoreRenderHelper.CreateAppSqlParam("@" + FrameworkConstants.PluginSystemInfo.TemplateName, "TemplateName", SqlDbType.NVarChar));

            if (systemParameter.HasTemplateVersion)
                command.Parameters.Add(CoreRenderHelper.CreateAppSqlParam("@" + FrameworkConstants.PluginSystemInfo.TemplateVersion, "TemplateVersion", SqlDbType.NVarChar));

            if (systemParameter.HasComputerName)
                command.Parameters.Add(CoreRenderHelper.CreateAppSqlParam("@" + FrameworkConstants.PluginSystemInfo.ComputerName, "CreatedOn", SqlDbType.NVarChar));

            if (systemParameter.HasUserId)
                command.Parameters.Add(CoreRenderHelper.CreateAppSqlParam("@" + FrameworkConstants.PluginSystemInfo.UserId, "CreatedBy", SqlDbType.NVarChar));
            if (systemParameter.HasRenderRequestID)
                command.Parameters.Add(CoreRenderHelper.CreateAppSqlParam("@" + FrameworkConstants.PluginSystemInfo.RenderRequestID, "RenderID", SqlDbType.NVarChar));
        }

        private void AddParamRenderArgumentXY(ref SqlCommand command, RenderArgDomainValue renderArgumentValue, ChecksumInfoItem item)
        {
            if (item.RenderArgument.RenderArgumentX)
                command.Parameters.Add(CoreRenderHelper.CreateAppSqlParam("@" + FrameworkConstants.RenderArgumentX,
                                                        string.Format("{0}", renderArgumentValue.RenderArgumentX),
                                                        SqlDbType.NVarChar));

            if (item.RenderArgument.RenderArgumentY)
                command.Parameters.Add(CoreRenderHelper.CreateAppSqlParam("@" + FrameworkConstants.RenderArgumentY,
                                                       string.Format("{0}", renderArgumentValue.RenderArgumentY),
                                                       SqlDbType.NVarChar));
        }

        #endregion

        #region Build Connection and Execute
        private XmlReader ExecuteXmlReader(string connection, SqlCommand command)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                cn.ConnectionString = connection;
                cn.Open();

                command.Connection = cn;

                XmlReader myDataReader = command.ExecuteXmlReader();

                cn.Close();
                return myDataReader;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Build ConnectToAppDB for Sql server database
        /// </summary>
        /// <returns></returns>
        public override string BuildConnectToAppDB(string dbUrl, string user, string password)
        {
            //1. Split dbUrl string to parts
            List<string> lstDBPart = dbUrl.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < lstDBPart.Count; i++)
            {
                lstDBPart[i] = lstDBPart[i].Trim();
            }
            if (lstDBPart.Count == 0 || lstDBPart.Count > 2)
                throw new Exception("Can't parse DBurl for Sql server and database");

            string server = lstDBPart[0];
            string database = lstDBPart[1];
            string sqlConnectToAppDB = string.Format(Constants.SQL_STRING_CONNECTION, server, database, user, password);

            return sqlConnectToAppDB;
        }

        #endregion
    }
}
