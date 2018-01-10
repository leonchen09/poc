//using System.Xml.Serialization;

//using ProntoDoc.Framework.CoreObject;

//namespace Pdwx.DataObjects
//{
//    [XmlRoot("Checksum")]
//    [XmlInclude(typeof(DataAgrument))]
//    public class ChecksumInfo
//    {
//        public string DomainName { get; set; }
//        public string PluginId { get; set; }
//        public string PluginName { get; set; }
//        public string UserName { get; set; }
//        public string InternalBookmark { get; set; }
//        public string Osql { get; set; }
//        public string ChecksumString { get; set; }
//        public DataAgrument Args { get; set; }

//        /// <summary>
//        /// domain language (is select by user when save template in plugin)
//        /// </summary>
//        public string DomainLL { get; set; }

//        public ChecksumInfo() { }

//        public ChecksumInfo(string domainName, string pluginId, string pluginName, string userName,
//            string checkSumString, DataAgrument args)//, string internalBookmark)
//        {
//            DomainName = domainName;
//            PluginId = pluginId;
//            PluginName = pluginName;
//            UserName = userName;
//            ChecksumString = checkSumString;
//            Args = args;
//            // InternalBookmark = internalBookmark;
//        }
//    }
//}
