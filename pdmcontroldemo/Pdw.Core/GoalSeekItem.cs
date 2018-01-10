using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pdw.Core
{
    public class GoalSeekItem
    {
        [System.Xml.Serialization.XmlElement("ExcelSheetName")]
        public string ExcelSheetName { get; set; }
        [System.Xml.Serialization.XmlElement("SeekName")]
        public string SeekName { get; set; }
        [System.Xml.Serialization.XmlElement("Condition")]
        public string Condition { get; set; }
        [System.Xml.Serialization.XmlElement("TargetCell")]
        public string TargetCell { get; set; }
        [System.Xml.Serialization.XmlElement("TargetValue")]
        public string TargetValue { get; set; }
        [System.Xml.Serialization.XmlElement("VariableCell")]
        public string VariableCell { get; set; }
    }
}
