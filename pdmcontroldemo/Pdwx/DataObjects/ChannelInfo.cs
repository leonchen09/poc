
//using System.Collections.Generic;

//namespace Pdwx.DataObjects
//{
//    public class ChannelInfo
//    {
//    }

//    public class EmailInfo : ChannelInfo
//    {
//        public List<string> To { get; set; }
//        public List<string> Cc { get; set; }
//        public List<string> Bcc { get; set; }
//        public string Subject { get; set; }
//        public string Body1 { get; set; }
//        public string Body2 { get; set; }
//        public SendMode PressSend { get; set; }

//        public EmailInfo()
//        {
//            this.To = new List<string>();
//            this.Cc = new List<string>();
//            this.Bcc = new List<string>();
//            this.PressSend = DataObjects.SendMode.Manual;
//        }
//    }

//    public enum SendMode
//    {
//        Auto,
//        Manual
//    }

//    public class FaxInfo : ChannelInfo
//    {
//        public string DestinationStation { get; set; }
//    }
//}
