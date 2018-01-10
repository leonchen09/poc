using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdePlugin
{
    [System.Xml.Serialization.XmlRoot("CondGoalSeekInfo")]
    public class CondGoalSeekInfo
    {
        public CondGoalSeekInfo()
        {
            this.CGSInfos = new List<CondGoalSeekItem>();
        }

        [System.Xml.Serialization.XmlElement("CGSInfos")]
        public List<CondGoalSeekItem> CGSInfos { get; set; }
    }
    [System.Xml.Serialization.XmlRoot("CondGoalSeekItem")]
    public class CondGoalSeekItem
    {
        [System.Xml.Serialization.XmlElement("seekName")]
        public string seekName { get; set; }
        [System.Xml.Serialization.XmlElement("conditionList")]
        public List<CondidtionItem> conditionList { get; set; }
        [System.Xml.Serialization.XmlElement("targetCell")]
        public string targetCell { get; set; }
        [System.Xml.Serialization.XmlElement("targetValue")]
        public string targetValue { get; set; }
        [System.Xml.Serialization.XmlElement("variableCell")]
        public string variableCell { get; set; }

        public override string ToString()
        {
            return "Target Cell:" + targetCell + ", Target Value:" + targetValue + ", Variable Cell:" + variableCell + ", conditions: " + string.Join(";",conditionList);
        }
    }
    [System.Xml.Serialization.XmlRoot("CondidtionItem")]
    public class CondidtionItem
    {
        [System.Xml.Serialization.XmlElement("condCell")]
        public string condCell { get; set; }
        [System.Xml.Serialization.XmlElement("operatorStr")]
        public string operatorStr { get; set; }
        [System.Xml.Serialization.XmlElement("condValue")]
        public string condValue { get; set; }

        public override string ToString()
        {
            return "cell: " + condCell + ", operator: " + operatorStr + ", value: " + condValue;
        }
    }
}
