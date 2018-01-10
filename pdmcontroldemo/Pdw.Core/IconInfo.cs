
namespace Pdw.Core
{
    public class IconInfo
    {
        /// <summary>
        /// index of image in ImageList
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// DBID of field
        /// </summary>
        public string DBID { get; set; }

        /// <summary>
        /// ID (guid) of icon. It is CustomerIconID in DSTreeView
        /// </summary>
        public string CustomerIconID { get; set; }

        /// <summary>
        /// get the key of IconInfo
        /// </summary>
        public string Key
        {
            get
            {
                return GenKey(DBID, CustomerIconID);
            }
        }

        /// <summary>
        /// gen key of Icon
        /// </summary>
        /// <param name="dbid"></param>
        /// <param name="customerIconID"></param>
        /// <returns></returns>
        public static string GenKey(string dbid, string customerIconID)
        {
            string key = string.Format("img{0}{1}{2}", dbid, (char)27, customerIconID);
            return BaseMarkupUtilities.XmlEncode(key);
        }
    }
}
