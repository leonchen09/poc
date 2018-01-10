
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject.DataSegment;

namespace Pdw.Core
{
    [XmlRoot("InternalBookmark")]
    public class InternalBookmark
    {
        private bool _isNull = false;

        [XmlElement("InternalBookmarkDomains")]
        public List<InternalBookmarkDomain> InternalBookmarkDomains { get; set; }

        [XmlElement("PdeDataTags")]
        public List<PdeDataTagInfo> PdeDataTagInfos { get; set; }

        [XmlAttribute()]
        public string DocumentSpecificColor { get; set; }

        public InternalBookmark()
            : this(false)
        {
        }

        public InternalBookmark(bool isNull)
        {            
            InternalBookmarkDomains = new List<InternalBookmarkDomain>();
            PdeDataTagInfos = new List<PdeDataTagInfo>();
            _isNull = isNull;
        }

        public bool IsNull()
        {
            return _isNull;
        }

        public InternalBookmark Clone()
        {
            InternalBookmark cloneObject = new InternalBookmark();

            // domain
            foreach (InternalBookmarkDomain item in InternalBookmarkDomains)
                cloneObject.InternalBookmarkDomains.Add(item.Clone());

            // pdetag
            foreach (PdeDataTagInfo item in PdeDataTagInfos)
                cloneObject.PdeDataTagInfos.Add(item.Clone());

            return cloneObject;
        }

        public bool HasSelectIBM()
        {
            if (InternalBookmarkDomains != null)
            {
                foreach (InternalBookmarkDomain domain in InternalBookmarkDomains)
                {
                    if (domain != null && domain.InternalBookmarkItems != null)
                    {
                        int count = domain.InternalBookmarkItems.Count(c => c.Type == XsltType.Select);
                        if (count > 0)
                            return true;
                    }
                }
            }

            return false;
        }

        public InternalBookmarkDomain CreateInternalBookmarkDomain(string domainName)
        {
            if (InternalBookmarkDomains == null)
                InternalBookmarkDomains = new List<InternalBookmarkDomain>();
            if (domainName != null)
            {
                if (!InternalBookmarkDomains.Exists(
                    ibmDomain => domainName.Equals(ibmDomain.DomainName, System.StringComparison.OrdinalIgnoreCase)))
                {
                    InternalBookmarkDomain ibmDomain = new InternalBookmarkDomain();
                    ibmDomain.DomainName = domainName;
                    InternalBookmarkDomains.Add(ibmDomain);

                    return ibmDomain;
                }
            }

            return null;
        }

        public InternalBookmarkDomain GetInternalBookmarkDomain(string domainName)
        {
            if (InternalBookmarkDomains != null)
                return InternalBookmarkDomains.FirstOrDefault(d => d.DomainName == domainName);

            return null;
        }

        public InternalBookmarkDomain GetInternalBookmarkDomainByItemKey(string key)
        {
            if (InternalBookmarkDomains != null)
            {
                foreach (InternalBookmarkDomain domain in InternalBookmarkDomains)
                {
                    if (domain != null && domain.InternalBookmarkItems != null)
                    {
                        foreach (InternalBookmarkItem item in domain.InternalBookmarkItems)
                        {
                            if (item != null && item.Key == key)
                                return domain;
                        }
                    }
                }
            }

            return null;
        }

        public InternalBookmarkItem GetInternalBookmarkItem(string key)
        {
            if (InternalBookmarkDomains != null)
            {
                foreach (InternalBookmarkDomain domain in InternalBookmarkDomains)
                {
                    if (domain != null && domain.InternalBookmarkItems != null)
                    {
                        foreach (InternalBookmarkItem item in domain.InternalBookmarkItems)
                        {
                            if (item != null && item.Key == key)
                                return item;
                        }
                    }
                }
            }

            return null;
        }

        public void RemoveInternalBookmarkItem(string key)
        {
            if (InternalBookmarkDomains != null)
            {
                foreach (InternalBookmarkDomain domain in InternalBookmarkDomains)
                {
                    if (domain != null)
                    {
                        if (domain.RemoveInternalBookmarkItem(key))
                            return;
                    }
                }
            }
        }

        public bool HasDocumentSpecific
        {
            get
            {
                if (InternalBookmarkDomains != null)
                    return InternalBookmarkDomains.Exists(c => c.HasDocumentSpecific);

                return false;
            }
        }

        public PdeDataTagInfo GetPdeDataTagInfo(string bmName)
        {
            return PdeDataTagInfos.FirstOrDefault(c => string.Equals(bmName, c.BookmarkName));
        }
    }
}