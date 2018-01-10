
//namespace Pdwx.DataObjects
//{
//    /// <summary>
//    /// using for render
//    /// RenderOption = server: transform and export to pdf, xps or msxml
//    /// RenderOption = client: put setting information into pdwr for client execute
//    /// </summary>
//    [System.Xml.Serialization.XmlRoot("RenderSettings")]
//    [System.Xml.Serialization.XmlInclude(typeof(ChannelInfo))]
//    [System.Xml.Serialization.XmlInclude(typeof(EmailInfo))]
//    [System.Xml.Serialization.XmlInclude(typeof(FaxInfo))]
//    public class RenderSettings
//    {
//        public MediaType Media { get; set; }
//        public ChannelType Channel { get; set; }
//        public ChannelInfo ChannelSpecificInfo { get; set; }

//        public enum MediaType:int
//        {
//            MsXml = 0,
//            Xps = 1,
//            Pdf = 2,
//        }

//        public enum ChannelType:int
//        {
//            Display = 0,
//            Email = 1,
//            Attachment = 2,
//            Fax,
//        }
//    }
//}
