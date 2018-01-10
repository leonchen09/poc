using System;
using System.Text;
using System.Xml.Serialization;

namespace Pdw.Core
{
    [Serializable]
    public class UserData
    {
        [XmlElement("LastDomain")]
        public string LastDomain { get; set; }
        [XmlElement("LastFontSize")]
        public int LastFontSize { get; set; }
    }
}
