using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Pdw.PreviewOsql.Entity
{
    /// <summary>
    /// Information of Databases: DBID, ConnectToAppDB, ConnectionPoolSize,...
    /// </summary>
    [XmlInclude(typeof(AppDB))]
    [XmlRoot("MDB")]
    public class MDB
    {
        [XmlElement("AppDB")]
        public List<AppDB> ListAppDB { get; set; }

        //public MDB()
        //{
        //    ListAppDB = new List<AppDB>();
        //}
    }

    [XmlRoot("AppDB")]
    public class AppDB
    {
        /// <summary>
        /// DBID/DBTypeDescription
        /// </summary>
        [XmlAttribute("DBID")]
        public string DBID { get; set; }

        /// <summary>
        /// Original ConnectToAppDB, from API
        /// </summary>
        [XmlElement("ConnectToAppDB")]
        public string ConnectToAppDB { get; set; }

        [XmlElement("ConnectionPoolSize")]
        public int? ConnectionPoolSize { get; set; }
    }
}
