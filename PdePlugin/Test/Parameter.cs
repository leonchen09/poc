using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Test
{
    [XmlRoot("Parameter")]
    public class RenderParameterBase
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        [XmlAttribute("Name")]
        public string Name;

        [XmlElement("Value")]
        //[XmlArray("Value")]        
        //public Boolean[] Value;
        public Object[] Value;

        [XmlElement("sappid")]
        public int? sappid;

        public bool ShouldSerializesappid()
        {
            return sappid.HasValue;
        }

        [XmlIgnore]
        public List<string> ss;

        public RenderParameterBase()
        {
            ss = new List<string>();
        }

    }

    public class RenderParameter : RenderParameterBase
    {
        [XmlElement("SS")]
        public List<string> ss;
    }
}
