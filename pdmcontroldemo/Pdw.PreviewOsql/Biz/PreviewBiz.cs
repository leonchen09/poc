using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pdw.PreviewOsql.Dao.Sql08;
using Pdw.PreviewOsql.Entity;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;
using ProntoDoc.Framework.CoreObject.Render;
using ProntoDoc.Framework.CoreObject.Render.Value;

namespace Pdw.PreviewOsql.Biz
{
    public class PreviewBiz
    {
        public Dictionary<string, string> GetOsqlDataFromAppDB(List<ChecksumInfoItem> checksumItems, RenderArgumentValue renderArgumentValue, MDB mdb, OsqlXml osqlXml, ref string msg)
        {
            Dictionary<string, string> dicOsqlResult = new Dictionary<string, string>();
            Validator validator = new Validator();

            bool isValid = true;
            //Get list DBID , Connection string and validate DBID
            Dictionary<string, string> lstAppDb = GetDBID(mdb.ListAppDB, ref msg);

            //Validate DBID
            List<string> lstDBID = lstAppDb.Keys.ToList();
            isValid &= Validator.ValidateDBID(checksumItems, lstDBID, ref msg);

            foreach (var appdb in lstAppDb)
            {
                List<ChecksumInfoItem> lstCheckSumItems = checksumItems.Where(x => string.Compare(x.DBID, appdb.Key, true) == 0).ToList();

                if (lstCheckSumItems.Count() > 0)
                {
                    //2. Execute all Osqls, get data for domains
                    foreach (ChecksumInfoItem item in lstCheckSumItems)
                    {
                        RenderArgDomainValue domainRenderArgValue = new RenderArgDomainValue();
                        try
                        {
                            domainRenderArgValue = renderArgumentValue.Domains.First(c => c.Name == item.DomainName);
                        }
                        catch (Exception)
                        { msg += item.DomainName + " not match with input info !!! \n"; }

                        //Validate RenderArgumentDomainValue
                        isValid &= validator.ValidateRenderArgumentDomainValue(item, domainRenderArgValue, ref msg);

                        string result = ProcessOsqlGetAppData(item, domainRenderArgValue, appdb.Value, osqlXml);

                        string encodedDomainName = XmlHelper.Encode(item.DomainName);
                        dicOsqlResult.Add(encodedDomainName, result);
                    }
                }

            }

            if (!isValid)
                throw new Exception(msg);

            //3. Return result of all Osql executed on a Database
            return dicOsqlResult;
        }

        private string ProcessOsqlGetAppData(ChecksumInfoItem item, RenderArgDomainValue renderArgumentValue, string conn, OsqlXml osqlXml)
        {
            string osqlData = string.Empty;
            foreach (var osqlItem in osqlXml.OsqlXmlItems)
            {
                if (string.Compare(osqlItem.DomainName, renderArgumentValue.Name, true) == 0)
                    osqlData = GetDataAgrumentValueWithDBType(item, osqlItem.Osql, renderArgumentValue, conn);
            }
            return osqlData;
        }

        private string GetDataAgrumentValueWithDBType(ChecksumInfoItem item, string osqlStr, RenderArgDomainValue renderArgumentValue, string conn)
        {
            DBType dbtype = (DBType)Utility.GetDBType(item.DBType);
            switch (dbtype)
            {
                case DBType.Sql2008:
                    {
                        Sql08GetDataAgrumentDao dao = new Sql08GetDataAgrumentDao();
                        string osqlData = dao.GetDataAgrumentValueFromAppDB(item, osqlStr, renderArgumentValue, conn);
                        return osqlData;
                    }
                case DBType.Orc10g:
                default:
                    throw new NotSupportedException();
            }
        }

        private Dictionary<string, string> GetDBID(List<AppDB> lstAppDb, ref string msg)
        {
            List<string> lstSplitErr = new List<string>();
            List<string> lstDupDBIDErr = new List<string>();
            Dictionary<string, string> dicAppID = new Dictionary<string, string>();

            foreach (AppDB appDb in lstAppDb)
            {
                //1. Split DBID
                string[] arrDBID = Utility.SplitDBID(appDb.DBID);

                if (arrDBID.Length != Constants.DBID_PART_COUNT)
                {
                    lstSplitErr.Add(appDb.DBID);
                    continue;
                }

                //2. Normalize DBID . DBID is first processed to remove all insignificant space/s and converted to
                string actualDBID = Utility.NormaliseDBID(arrDBID[0]).ToUpper();

                //3. Check dupplicate DBID)
                if (!dicAppID.ContainsKey(actualDBID))
                    dicAppID.Add(actualDBID, appDb.ConnectToAppDB);
                else
                {
                    lstDupDBIDErr.Add(appDb.DBID);
                    continue;
                }
            }

            if (lstSplitErr.Count > 0)
                msg += string.Format("DBID format incorrect: {0}", Utility.GetStringFromList(lstSplitErr));

            if (lstDupDBIDErr.Count > 0)
                msg += string.Format("DBID duplicated: {0}", Utility.GetStringFromList(lstDupDBIDErr));

            return dicAppID;
        }

    }
}
