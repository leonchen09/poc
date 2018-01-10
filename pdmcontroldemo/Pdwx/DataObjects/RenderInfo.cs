
//using System.Xml;
//using System.Text;

//namespace Pdwx.DataObjects
//{
//    public class RenderInfo
//    {
//        #region public properties
//        public string PdwFilePath { get; private set; }
//        public XmlDocument XmlDoc { get; private set; }
//        public byte[] XslContent { get; private set; }
//        public RenderMode RenderMode { get; private set; }
//        public string OutputFilePath { get; private set; }
//        public RenderSettings RenderSettings { get; private set; }
//        #endregion

//        #region internal properties
//        internal string XmlFilePath { get; private set; }
//        internal string XslFilePath { get; private set; }
//        #endregion

//        #region constructors
//        /// <summary>
//        /// Initialize render information
//        /// </summary>
//        /// <param name="outputFilePath"></param>
//        /// <param name="renderMode"></param>
//        /// <param name="xslContent">Alway set exclude renderMode = RenderMode.ApprovalRaw</param>
//        /// <param name="xmlContent">Alway set exclude renderMode = RenderMode.ApprovalRaw</param>
//        /// <param name="pdwFilePath">Must be set when renderMode = RenderMode.ApprovalRaw</param>
//        /// <param name="renderSetting">Must be set when renderMode = RenderMode.Normal</param>
//        //public RenderInfo(string outputFilePath, RenderMode renderMode,
//        //    byte[] xslContent = null, StringBuilder xmlData = null,
//        //    string pdwFilePath = "", RenderSettings renderSetting = null)
//        //{
//        //    this.OutputFilePath = outputFilePath;
//        //    this.RenderMode = renderMode;
//        //    this.XslContent = xslContent;
//        //    this.XmlData = xmlData;
//        //    this.PdwFilePath = pdwFilePath;
//        //    this.RenderSettings = renderSetting;
//        //}
//        public RenderInfo(string outputFilePath, RenderMode renderMode,
//            byte[] xslContent = null, XmlDocument xmlData = null,
//            string pdwFilePath = "", RenderSettings renderSetting = null)
//        {
//            this.OutputFilePath = outputFilePath;
//            this.RenderMode = renderMode;
//            this.XslContent = xslContent;
//            this.XmlDoc = xmlData;
//            this.PdwFilePath = pdwFilePath;
//            this.RenderSettings = renderSetting;
//        }
//        #endregion

//        #region internal methods
//        internal void CreatTempFiles()
//        {
//            // 1. get folder path
//            string folderPath = System.IO.Path.GetDirectoryName(OutputFilePath) + "\\";

//            // 2. create xml file
//            XmlFilePath = folderPath + System.Guid.NewGuid().ToString() + FileExtension.Xml;
//            XmlDoc.Save(XmlFilePath);

//            // 3. create xsl file
//            XslFilePath = folderPath + System.Guid.NewGuid().ToString() + FileExtension.Xsl;
//            Utilities.WriteFile(XslFilePath, XslContent == null ? string.Empty : ProntoDoc.Framework.Utils.FileHelper.ConvertByteArrayToString(XslContent), false, false);
//        }

//        internal void DeleteTempFiles()
//        {
//            Utilities.DeleteFile(XmlFilePath);
//            Utilities.DeleteFile(XslFilePath);
//        }
//        #endregion
//    }
//}