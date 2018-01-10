
using System.Xml.Serialization;

using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.CoreObject.DataSegment;
using System;

namespace Pdw.Core
{
    [XmlRoot("InternalBookmarkItem")]
    [XmlInclude(typeof(DSRelation))]
    [XmlInclude(typeof(XsltType))]
    [XmlInclude(typeof(SQLDBType))]
    public class InternalBookmarkItem
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }

        [XmlAttribute("DomainName")]
        public string DomainName { get; set; }

        [XmlAttribute("BusinessName")]
        public string BizName { get; set; } // using for check domain

        [XmlAttribute("TechnicalName")]
        public string TechName { get; set; }

        [XmlAttribute("JavaName")]
        public string JavaName { get; set; }

        [XmlAttribute("Path")]
        public string ItemType { get; set; }

        [XmlAttribute("XsltType")]
        public XsltType Type { get; set; }

        [XmlElement("DataType", typeof(SQLDBType))]
        public SQLDBType DataType { get; set; } // using for check domain

        [XmlAttribute("UniqueName")]
        public string UniqueName { get; set; } // using for check domain and keep in internal bookmark (must be encode)
        public string OriginalUniqueName // using for check domain
        {
            get { return System.Xml.XmlConvert.DecodeName(UniqueName); }
        }

        [XmlAttribute("TableIndex")]
        public int TableIndex { get; set; }

        [XmlAttribute("RenderXY")]
        public int RenderXY { get; set; }

        /// <summary>
        /// use for pde
        /// </summary>
        [XmlAttribute("XmlPath")]
        public string XmlPath { get; set; }
        [XmlAttribute("RangeName")]
        public string RangeName { get; set; }

        [XmlElement("Relation", typeof(DSRelationRow))]
        public DSRelationRow Relation { get; set; }

        [XmlIgnore]
        public int ItemTypeNumber 
        {
            get{
                return (int)((DSIconType)Enum.Parse(typeof(DSIconType), ItemType));
            }
        }

        [XmlIgnore]
        public int OrderNo { get; set; }

        #region constructors
        public InternalBookmarkItem() { }

        /// <summary>
        /// using for check domain
        /// </summary>
        /// <param name="bizName"></param>
        /// <param name="uniqueName"></param>
        /// <param name="dataType"></param>
        public InternalBookmarkItem(string bizName, string uniqueName, ProntoDoc.Framework.CoreObject.SQLDBType dataType, 
            XsltType type):this()
        {
            BizName = bizName; UniqueName = uniqueName; DataType = dataType; Type = type;
        }

        public InternalBookmarkItem(string key)
            : this()
        {
            Key = key;
        }

        public InternalBookmarkItem(string key, string bizName, string techName, string itemType, XsltType xslType)
            : this(key, bizName, techName, itemType)
        {
            Type = xslType;
        }

        public InternalBookmarkItem(string key, string bizName, string techName, string itemType, int tableIndex, DSRelationRow relation)
            : this(key, bizName, techName, itemType)
        {
            TableIndex = tableIndex;
            Relation = relation;          
        }

        public InternalBookmarkItem(string key, string bizName, string techName, string itemType, int tableIndex, DSRelationRow relation, int renderXY)
            : this(key, bizName, techName, itemType)
        {
            TableIndex = tableIndex;
            Relation = relation;
            RenderXY = renderXY;
        }

        public InternalBookmarkItem(string key, string bizName, string techName, string itemType)
            : this(key)
        {
            BizName = bizName;
            TechName = techName;
            ItemType = itemType;

        }
        #endregion

        public InternalBookmarkItem Clone()
        {
            InternalBookmarkItem clone = MemberwiseClone() as InternalBookmarkItem;
            clone.Type = this.Type;
            clone.DataType = this.DataType;
            clone.Relation = this.Relation;

            return clone;
        }

        /// <summary>
        /// get DSIconType
        /// </summary>
        public DSIconType DSIconType
        {
            get
            {
                return (DSIconType)Enum.Parse(typeof(DSIconType), ItemType);
            }
        }

        /// <summary>
        /// get original business name (biz name of field)
        /// </summary>
        public string OrginalBizName
        {
            get
            {
                return BaseMarkupUtilities.GetOriginalBizName(Key, BizName);
            }
        }

        public bool IsImage()
        {
            if (this.DataType != null &&
                (this.DataType.Name == SQLTypeName.IMAGE || this.DataType.Name == SQLTypeName.ASIMAGE))
                return true;

            return false;
        }

        public bool IsTruncImage()
        {
            if (this.DataType != null && this.DataType.Name == SQLTypeName.IMAGE)
                return true;

            return false;
        }
    }
}