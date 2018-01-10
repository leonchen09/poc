using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.Utils;

namespace Pdw.PreviewOsql.Entity
{
    public class Utility
    {
         public static int GetDBType(int type)
        {
            return 1;
        }

        public static string MergeOsqlResultData(Dictionary<string, string> dicAppData)
        {
            StringBuilder xmlData = new StringBuilder();

            xmlData.AppendLine(string.Format("<{0}>", FrameworkConstants.PdwDataRootName));
            foreach (var osqlResult in dicAppData)
            {
                if (string.IsNullOrWhiteSpace(osqlResult.Value))
                    xmlData.AppendLine(string.Format("<{0}></{0}>", osqlResult.Key));
                else
                    xmlData.AppendLine(osqlResult.Value);
            }
            xmlData.AppendLine(string.Format("</{0}>", FrameworkConstants.PdwDataRootName));

            return xmlData.ToString();
        }

        public static string[] SplitDBID(string dbid)
        {
            return dbid.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string NormaliseDBID(string dbid)
        {
            return dbid.Replace(" ", "").ToUpper();
        }

        public static string GetStringFromList(List<string> lstStr)
        {
            if (lstStr == null || lstStr.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append(lstStr[0]);
            if (lstStr.Count > 1)
            {
                for (int i = 1; i < lstStr.Count; i++)
                {
                    sb.AppendFormat(", {0}", lstStr[i]);
                }
            }
            return sb.ToString();
        }
    }
}
