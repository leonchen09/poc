using System;
using System.Text;

namespace Pdw.Services.Integrity
{
    public class Validator
    {
        /// <summary>
        /// {DomainName};{PluginId};{PluginName};{UserName};{InternalBookmark};{Osql}
        /// </summary>
        private const string ChecksumFormat = "{0};{1};{2};{3};{4}";

        /// <summary>
        /// Generate checksum string
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="pluginName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GenCheckSum(string pluginId, string pluginName, string userName, string internalBookmark, string osql)
        {
            internalBookmark = PrepareInternalBookmark(internalBookmark);
            osql = PrepareOsql(osql);
            string strCheckSum = string.Format(ChecksumFormat, pluginId, pluginName, userName, internalBookmark, osql);
            strCheckSum = Encrypt(strCheckSum);
            return strCheckSum;
        }

        private string PrepareInternalBookmark(string internalBookmarkString)
        {
            // 1. remove enter line character
            // 2. remove go to begin of line character
            // 3. remove space character
            // 4. remove empty fields property empty in Usc
            return internalBookmarkString.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("<Fields/>", "");
        }

        private string PrepareOsql(string osql)
        {
            // 1. remove enter line character
            // 2. remove go to begin of line character
            // 3. remove space character
            if (string.IsNullOrEmpty(osql))
                osql = "";
            return osql.Replace("\n", "").Replace("\r", "").Replace(" ", "");
        }

        /// <summary>
        /// validate checksum string
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="pluginName"></param>
        /// <param name="userName"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        public bool IsValid(string pluginId, string pluginName, string userName, string internalBookmark,
            string osql, string checksum)
        {
            string realChecksum = GenCheckSum(pluginId, pluginName, userName, internalBookmark, osql);
            return realChecksum.Equals(checksum);
        }

        /// <summary>
        /// Get current user name
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserName()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            
            if(!string.IsNullOrEmpty(userName) && userName.Contains("\\"))
                return userName.Substring(userName.LastIndexOf('\\') + 1);
            
            return userName;
        }

        /// <summary>
        /// Get current user name
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserName(bool isImpersonate)
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent(isImpersonate).Name;
        }

        public string GetComputerName()
        {
            try
            {
                return System.Environment.MachineName;
            }
            catch { }

            return string.Empty;
        }

        /// <summary>
        /// Encrypt checksum string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        private string Encrypt(string plainText)
        {
            // instant md5 object
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            // convert string to byte array
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(plainText));

            // build encrypted text
            StringBuilder strBuilder = new StringBuilder();

            // re-format each byte in data as hexadecimal string
            for (int index = 0; index < data.Length; index++)
                strBuilder.Append(data[index].ToString("x2"));

            // clear md5 instant
            md5.Clear();

            return strBuilder.ToString();
        }
    }
}
