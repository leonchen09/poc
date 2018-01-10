
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdw.Core;

namespace Pdw.Services.Template
{
    public partial class GenOsqlHelper
    {
        /// <summary>
        /// get OsqlString
        /// </summary>
        public string OsqlString { get; private set; }
        public InternalBookmark Ibm { get; private set; }
        public List<ChecksumInfoItem> ChecksumInfoItems { get; private set; }
        public OsqlXml OsqlXml { get; private set; }

        public void GenOsql(List<DomainInfo> domains, InternalBookmark ibm)
        {
            OsqlString = string.Empty;
            Ibm = ibm.Clone();
            ChecksumInfoItems = new List<ChecksumInfoItem>();
            OsqlXml = new OsqlXml();

            int domainIndex = 1;
            foreach (DomainInfo domain in domains)
            {
                BaseGenOsqlHelper genHelper = null;
                ProntoDoc.Framework.CoreObject.AppDbTypes dbType = GetDatabaseType(domain);
                switch (dbType)
                {
                    case ProntoDoc.Framework.CoreObject.AppDbTypes.Oracle:
                        genHelper = new GenOsqlOracleHelper();
                        break;
                    case ProntoDoc.Framework.CoreObject.AppDbTypes.Sql:
                        genHelper = new GenOsqlSql08Helper();
                        break;
                    default:
                        break;
                }

                if (genHelper != null)
                {
                    genHelper.GenOsql(domain, Ibm, domainIndex);
                    // genHelper.ChecksumInfoItem.DocumentSpecificColor = ibm.DocumentSpecificColor; // replace for BaseGenOsqlHelper
                    ChecksumInfoItems.Add(genHelper.ChecksumInfoItem);
                    OsqlXml.OsqlXmlItems.Add(genHelper.OsqlXmlItem);
                }

                domainIndex++;
            }

            OsqlString = ProntoDoc.Framework.Utils.ObjectSerializeHelper.SerializeToString<OsqlXml>(OsqlXml);
        }

        private ProntoDoc.Framework.CoreObject.AppDbTypes GetDatabaseType(DomainInfo domainInfo)
        {
            if (domainInfo.DSDomainData != null)
            {
                switch (domainInfo.DSDomainData.DBType)
                {
                    case 1:
                        return ProntoDoc.Framework.CoreObject.AppDbTypes.Oracle;
                    case 2:
                        return ProntoDoc.Framework.CoreObject.AppDbTypes.Sql;
                }
            }

            return ProntoDoc.Framework.CoreObject.AppDbTypes.Sql;
        }
    }
}