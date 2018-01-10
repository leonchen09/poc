
using System.Collections.Generic;

namespace Pdw.Core
{
    public class DomainMatch
    {
        /// <summary>
        /// using to store old Domain nearest with another domain
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// using to store candidate domain when user quick view
        /// </summary>
        public string CandidateDomainName { get; set; }
        /// <summary>
        /// using to store new domain (replaced domain is chose by user)
        /// </summary>
        public string NewDomainName { get; set; }

        public Dictionary<string, DomainMatchItem> DomainMatchItems { get; set; }

        public DomainMatch()
        {
            DomainMatchItems = new Dictionary<string, DomainMatchItem>();
        }

        public DomainMatch(List<string> domainNames):this()
        {
            if (domainNames != null)
            {
                foreach (string domainName in domainNames)
                {
                    if (!string.IsNullOrEmpty(domainName) && !DomainMatchItems.ContainsKey(domainName))
                    {
                        DomainMatchItem domainMatchItem = new DomainMatchItem(domainName);
                        DomainMatchItems.Add(domainName, domainMatchItem);
                    }
                }
            }
        }

        public bool IsMatch
        {
            get
            {
                if (DomainMatchItems != null)
                {
                    foreach (DomainMatchItem item in DomainMatchItems.Values)
                    {
                        if (!item.IsMatch)
                            return false;
                    }
                }
                return true;
            }
        }
    }

    public class DomainMatchItem
    {
        private string DescriptionFormat = "{0} ({1}/{2})";
        private string NoChooseDomain = Properties.Resources.ipm_NoChooseDomain;
        private Dictionary<string, bool> _bizs = new Dictionary<string, bool>();

        public string DomainName { get; set; }
        public List<string> NotMatchFields { get; set; }
        public List<string> MatchedFields { get; set; }
        public List<string> NotMatchBizs { get; private set; }
        public bool IsMatchRelationOn { get; set; }
        public bool IsMatchWhereClause { get; set; }

        public int ItemsCount { get { return NotMatchFields.Count + MatchedFields.Count; } }
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(DomainName))
                    return NoChooseDomain;

                if (NotMatchFields.Count != MatchedFields.Count)
                    return string.Format(DescriptionFormat, DomainName, MatchedFields.Count, ItemsCount);

                return DomainName;
            }
        }
        public bool IsMatch
        {
            get
            {
                return (NotMatchFields.Count == 0 && IsMatchWhereClause);
            }
        }
        public void SetSingleBiz(string bizName)
        {
            if (!_bizs.ContainsKey(bizName))
            {
                NotMatchBizs.Add(string.Format(ProntoMarkup.ValueSelect, bizName));
                _bizs.Add(bizName, false);
            }
        }
        public void SetDoubleBiz(string bizName)
        {
            if (!_bizs.ContainsKey(bizName))
            {
                NotMatchBizs.Add(string.Format(ProntoMarkup.ValueStartIf, bizName));
                NotMatchBizs.Add(string.Format(ProntoMarkup.ValueEndIf, bizName));
                _bizs.Add(bizName, false);
            }
        }

        public DomainMatchItem()
        {
            NotMatchFields = new List<string>();
            MatchedFields = new List<string>();
            NotMatchBizs = new List<string>();
            IsMatchRelationOn = true;
        }
        public DomainMatchItem(string domainName)
        {
            NotMatchFields = new List<string>();
            MatchedFields = new List<string>();
            NotMatchBizs = new List<string>();
            DomainName = domainName;
            IsMatchRelationOn = true;
        }
    }
}
