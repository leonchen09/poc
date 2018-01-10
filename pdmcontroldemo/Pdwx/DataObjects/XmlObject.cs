
//namespace Pdwx.DataObjects
//{
//    [System.Xml.Serialization.XmlRoot("XmlObject")]
//    public class XmlObject
//    {
//        [System.Xml.Serialization.XmlElement("Content")]
//        public string Content { get; set; }

//        [System.Xml.Serialization.XmlElement("ContentType", typeof(ContentType))]
//        public ContentType ContentType { get; set; }

//        public XmlObject() { }

//        public XmlObject(string content)
//        {
//            Content = content;
//            ContentType = ContentType.None;
//        }

//        public XmlObject(string content, ContentType contentType)
//        {
//            Content = content;
//            ContentType = contentType;
//        }
//    }

//    public enum ContentType
//    {
//        Osql,
//        Xslt,
//        Checksum,
//        PdwrXml,
//        PdwrXsl,
//        PdwrSettings,
//        None
//    }
//}
