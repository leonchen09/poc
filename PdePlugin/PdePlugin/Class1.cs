using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdePlugin
{
    [System.Xml.Serialization.XmlRoot("MapInfo")]
    public class Class1
    {
        [System.Xml.Serialization.XmlElement("Content")]
        public string Content { get; set; }

        [System.Xml.Serialization.XmlElement("Map")]
        public List<string> maps { get; set; }
        [System.Xml.Serialization.XmlElement("c2")]
        public Cl2 c2 { get; set; }
        [System.Xml.Serialization.XmlElement("Map2")]
        public List<Cl2> cm { get; set; }
    }
    [System.Xml.Serialization.XmlRoot("cl2")]
    public class Cl2
    {
        [System.Xml.Serialization.XmlElement("i")]
        public int i { get; set; }

    }


}
