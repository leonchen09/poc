//using System;
//using System.Xml;

//using Word = Microsoft.Office.Interop.Word;

//namespace Pdwx.DataObjects
//{
//    public class PdwrWriter
//    {
//        #region public properties
//        public byte[] XslContent { get; private set; }
//        public string PdwrFilePath { get; private set; }
//        // public StringBuilder XmlData { get; private set; }
//        public XmlDocument XmlDoc { get; private set; }
//        public RenderSettings RenderSettings { get; private set; }
//        #endregion

//        #region constructor
//        //public PdwrWriter(byte[] xslContent, StringBuilder xmlData, RenderSettings renderSetting)
//        //{
//        //    this.XslContent = xslContent;
//        //    this.XmlData = xmlData;
//        //    this.RenderSettings = renderSetting;
//        //}
//        public PdwrWriter(byte[] xslContent, XmlDocument xmlData, RenderSettings renderSetting)
//        {
//            this.XslContent = xslContent;
//            this.XmlDoc = xmlData;
//            this.RenderSettings = renderSetting;
//        }
//        #endregion

//        #region public methods: Save(string)
//        public void Save(string pdwrFilePath)
//        {
//            this.PdwrFilePath = pdwrFilePath;

//            Save();
//        }
//        #endregion

//        #region prive methods: Save()
//        private void Save()
//        {
//            Exception ex = null;
            
//            // 1. initialize word application
//            Word.Application wApp = new Word.Application();
//            wApp.Visible = false;

//            // 2. create new word document
//            Word.Document wDoc = wApp.Documents.Add();

//            // 3. put content into word document
//            try
//            {
//                wApp.Selection.TypeText("Data is processing...");
                
//                AddCustomXml(wDoc, GetXmlString());
                
//                AddCustomXml(wDoc, GetXslString());
                
//                AddCustomXml(wDoc, GetSettingString());
                
//                wDoc.SaveAs(PdwrFilePath);
//                wDoc.Saved = true;
//            }
//            catch (Exception e) { ex = e; }

//            // 4. close word application and word document
//            Services.WordHeper.Close(wApp, wDoc);
            
//            if (ex != null)
//                throw ex;
//        }

//        private void AddCustomXml(Word.Document wDoc, string xmlContent)
//        {
//            Microsoft.Office.Core.CustomXMLPart xmlPart = wDoc.CustomXMLParts.Add();
//            xmlPart.LoadXML(xmlContent);
//        }

//        private string GetXmlString()
//        {
//            XmlObject xmlObject = new XmlObject();
//            xmlObject.ContentType = ContentType.PdwrXml;

//            // xmlObject.Content = this.XmlData.ToString();
//            xmlObject.Content = this.XmlDoc.OuterXml;
            
//            return Utilities.Serialize(xmlObject);
//        }

//        private string GetXslString()
//        {
//            XmlObject xmlObject = new XmlObject(
//                ProntoDoc.Framework.Utils.FileHelper.ConvertByteArrayToString(this.XslContent), ContentType.PdwrXsl);
            
//            return Utilities.Serialize(xmlObject);
//        }

//        private string GetSettingString()
//        {
//            XmlObject xmlObject = new XmlObject();
//            xmlObject.ContentType = ContentType.PdwrSettings;
//            xmlObject.Content = (this.RenderSettings == null) ? string.Empty : Utilities.Serialize(this.RenderSettings);

//            return Utilities.Serialize(xmlObject);
//        }
//        #endregion
//    }
//}
