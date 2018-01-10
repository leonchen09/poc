using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProntoDoc.Framework.Utils;

namespace Pdw.PreviewOsql.Dao
{
    public abstract class BaseDao
    {
        /// <summary>
        /// Get actual connection string from normalised (ex: DBurl=...)
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public string GetConnectionString(string normalisedConnStr)
        {
            if (string.IsNullOrWhiteSpace(normalisedConnStr))
                throw new Exception("ConnectionString is null or Empty");

            //1. Split Conn string to parts
            List<string> lstCnnPart = normalisedConnStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < lstCnnPart.Count; i++)
            {
                lstCnnPart[i] = lstCnnPart[i].Trim();
            }

            string actualConnStr;
            try
            {
                //2. Get DBUrl, User and Password values
                string dbUrl = ConnectionHelper.GetPartValue(lstCnnPart, "dburl");
                string user = ConnectionHelper.GetPartValue(lstCnnPart, "user");
                string password = ConnectionHelper.GetPartValue(lstCnnPart, "password");

                //3. Build connection string for database

                actualConnStr = BuildConnectToAppDB(dbUrl, user, password);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Connection string incorrect: {0}", normalisedConnStr));
            }

            return actualConnStr;
        }

        public abstract string BuildConnectToAppDB(string dbUrl, string user, string password);
    }
}
