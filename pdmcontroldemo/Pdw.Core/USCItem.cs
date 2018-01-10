
using System.Xml.Serialization;
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.CoreObject.DataSegment;

namespace Pdw.Core
{
    [XmlRoot("USCItem")]
    public class USCItem
    {
        #region Properties
        [XmlElement("BusinessName")]
        public string BusinessName { get; set; }

        [XmlElement("DataType", typeof(SQLDBType))]
        public SQLDBType DataType { get; set; }

        [XmlElement("Type", typeof(DSIconType))]
        public DSIconType Type { get; set; }

        [XmlElement("UniqueName")]
        public string UniqueName { get; set; }

        [XmlElement("BaseOnField")]
        public string BaseOnField { get; set; }

        [XmlElement("TableIndex")]
        public int TableIndex { get; set; }

        [XmlElement("DomainName")]
        public string DomainName { get; set; }

        /// <summary>
        /// using when check domain
        /// </summary>
        public string OriginalUniqueName { get { return System.Xml.XmlConvert.DecodeName(UniqueName); } }

        #region Used for User Specificed Condition
        /// <summary>
        /// Value is used for User Specificed Condition
        /// </summary>
        [XmlElement("SQLExpression")]
        public string SQLExpression { get; set; }

        /// <summary>
        /// Fields is used for User Specificed Condition.
        /// </summary>
        public List<USCItem> Fields { get; set; }

        [XmlIgnore]
        public string ErrorMessage { get; set; }

        [XmlIgnore]
        public bool IsValid { get; set; }
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// This is used for Markup
        /// </summary>
        /// <param name="businessName"></param>
        /// <param name="dataType"></param>
        /// <param name="type"></param>
        public USCItem(string businessName, string sqlExpression, SQLDBType dataType, DSIconType type)
        {
            BusinessName = businessName;
            SQLExpression = sqlExpression;
            DataType = dataType;
            Type = type;
            IsValid = true;
        }

        /// <summary>
        /// This method is used for Add new User Specificed Condition
        /// </summary>
        /// <param name="businessName"></param>
        /// <param name="value"></param>
        public USCItem(string businessName, string value)
        {
            BusinessName = businessName;
            SQLExpression = value;
            Type = DSIconType.USC;
            IsValid = true;
        }

        public USCItem()
        {
        }
        #endregion

        public USCItem Clone()
        {
            USCItem clone = MemberwiseClone() as USCItem;

            if (Fields != null)
            {
                clone.Fields = new List<USCItem>();
                foreach (USCItem field in Fields)
                    clone.Fields.Add(field.Clone());
            }

            return clone;
        }
    }
}