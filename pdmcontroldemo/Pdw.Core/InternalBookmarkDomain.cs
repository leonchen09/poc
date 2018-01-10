
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Pdw.Core
{
    public class InternalBookmarkDomain
    {
        [XmlAttribute("DomainName")]
        public string DomainName { get; set; }

        [XmlElement("Color")]
        public string Color { get; set; }

        [XmlElement("InternalBookmarkItems")]
        public List<InternalBookmarkItem> InternalBookmarkItems { get; set; }

        [XmlElement("USCs")]
        public List<USCItem> USCItems { get; set; }

        [XmlElement("WhereClause")]
        public string WhereClause { get; set; }

        [XmlElement("RelationOn")]
        public List<RelationOn> RelationOns { get; set; }

        [XmlIgnore]
        public List<List<string>> SelectedTables { get; set; }

        [XmlIgnore]
        public Relations Relations { get; set; }

        public InternalBookmarkDomain()
        {
            InternalBookmarkItems = new List<InternalBookmarkItem>();
            USCItems = new List<USCItem>();
            RelationOns = new List<RelationOn>();
        }

        public InternalBookmarkDomain Clone()
        {
            InternalBookmarkDomain clone = MemberwiseClone() as InternalBookmarkDomain;

            // internal bookmark item
            clone.InternalBookmarkItems = new List<InternalBookmarkItem>();
            foreach (InternalBookmarkItem item in InternalBookmarkItems)
                clone.InternalBookmarkItems.Add(item.Clone());

            // usc items
            clone.USCItems = new List<USCItem>();
            foreach (USCItem item in USCItems)
                clone.USCItems.Add(item.Clone());

            // relationons
            clone.RelationOns = new List<RelationOn>();
            foreach (RelationOn relationOn in RelationOns)
                clone.RelationOns.Add(relationOn.Clone());

            return clone;
        }

        public bool IsExistInternalBookmarkItem(string key)
        {
            if (InternalBookmarkItems != null)
            {
                return InternalBookmarkItems.Exists(item => item.Key == key);
            }

            return false;
        }

        public bool RemoveInternalBookmarkItem(string key)
        {
            if (InternalBookmarkItems != null)
            {
                int count = InternalBookmarkItems.Count;
                for (int index = 0; index < count; index++)
                {
                    InternalBookmarkItem item = InternalBookmarkItems[index];
                    if (item != null && item.Key == key)
                    {
                        InternalBookmarkItems.RemoveAt(index);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// get path of root table (is encoded)
        /// </summary>
        public string RootTablePath
        {
            get
            {
                string rootTablePath = BaseMarkupUtilities.XmlEncode(DomainName);
                List<string> firstPath = SelectedTables != null && SelectedTables.Count > 0 ? SelectedTables[0] : null;

                if (firstPath != null && firstPath.Count > 1)
                    rootTablePath = rootTablePath + "/" + BaseMarkupUtilities.XmlEncode(firstPath[1]);

                return rootTablePath;
            }
        }

        public bool HasDataTag
        {
            get
            {
                if (InternalBookmarkItems != null)
                {
                    foreach (InternalBookmarkItem ibmItem in InternalBookmarkItems)
                    {
                        if (ibmItem != null)
                        {
                            if (ibmItem.Type == XsltType.Select)
                                return true;

                            if (ibmItem.Type == XsltType.If && !ibmItem.Key.Contains("Sys"))
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        public bool HasDocumentSpecific
        {
            get
            {
                if (InternalBookmarkItems != null)
                {
                    foreach (InternalBookmarkItem ibmItem in InternalBookmarkItems)
                    {
                        if (ibmItem != null)
                        {
                            if (ibmItem.Type == XsltType.If && ibmItem.Key.Contains("Sys"))
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        public USCItem GetUscItem(string bizName)
        {
            if (USCItems != null && !string.IsNullOrWhiteSpace(bizName))
            {
                return USCItems.FirstOrDefault(usc => usc.BusinessName == bizName);
            }

            return null;
        }

        public List<string> GetAllSelectedTables()
        {
            List<string> selectedTables = new List<string>();

            if (SelectedTables != null)
            {
                foreach (List<string> path in SelectedTables)
                {
                    if (path != null)
                    {
                        foreach (string table in path)
                        {
                            if (!BaseMarkupUtilities.IsExistOnList(selectedTables, table))
                                selectedTables.Add(table);
                        }
                    }
                }
            }

            return selectedTables;
        }

        public List<string> GetSelectedTabledOfPath(string tableName)
        {
            if (SelectedTables != null)
            {
                foreach (List<string> path in SelectedTables)
                {
                    if (path != null && BaseMarkupUtilities.IsExistOnList(path, tableName))
                        return path;
                }
            }

            return GetAllSelectedTables();
        }
    }
}