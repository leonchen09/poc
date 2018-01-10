
using System.Xml;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

namespace Pdw.Core
{
    public class TemplateInfo
    {
        #region Properties

        /// <summary>
        /// keep right panel of the document
        /// </summary>
        public Microsoft.Office.Tools.CustomTaskPane RightPanel { get; set; }

        public Pdw.Managers.Hcl.ProntoDocMarkup ProntoDocMarkup
        {
            get
            {
                return RightPanel == null ? null : RightPanel.Control as Pdw.Managers.Hcl.ProntoDocMarkup;
            }
        }

        /// <summary>
        /// internal bookmark of the document
        /// </summary>
        private InternalBookmark _internalBookMark;
        public InternalBookmark InternalBookmark
        {
            get
            {
                if (null == _internalBookMark)
                {
                    _internalBookMark = new InternalBookmark();
                }
                return _internalBookMark;
            }
            set { _internalBookMark = value; }
        }

        /// <summary>
        /// markup the documet is reconstruct or no
        /// </summary>
        public bool IsReconstruct { get; set; }

        /// <summary>
        /// font size of the document
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// the document is markup autosave or no
        /// </summary>
        public bool IsAutoSave { get; set; }

        /// <summary>
        /// protect level of the document
        /// </summary>
        public ProtectLevel ProtectLevel { get; set; }

        /// <summary>
        /// markup the document is pronto document or no
        /// </summary>
        public bool IsProntoDoc { get; set; }

        /// <summary>
        /// markup the document is adding or no
        /// </summary>
        public bool IsAdding { get; set; }

        /// <summary>
        /// markup the document is saving or no
        /// </summary>
        public bool IsSaving { get; set; }

        /// <summary>
        /// markup right panel of the document is visible or disable
        /// </summary>
        public bool RightPanelStatus { get; set; }

        public List<string> DomainNames { get; private set; }
        public string SelectedDomainName { get; set; }

        public string FullDocName { get; set; }

        public TreeViewSpliter TreeViewData { get; set; }

        public string XmlContent { get; set; }

        public XmlNode WordXmlNode { get; set; }

        public List<XmlNode> DeletedTags { get; set; }

        public bool IsReConstructing { get; set; }

        private PdeContent _pdeContent = null;
        public PdeContent PdeContent 
        {
            get
            {
                if (_pdeContent == null)
                    _pdeContent = new PdeContent();

                return _pdeContent;
            }
            set
            {
                _pdeContent = value;
            }
        }

        //HACK:FORM CONTROLS
        public Microsoft.Office.Tools.CustomTaskPane LeftPanel { get; set; }

        public PropertyGrid ControlPropertyGrid
        {
            get
            {
                return LeftPanel == null ? null : LeftPanel.Control.Controls[0] as PropertyGrid;
            }
        }

        #endregion

        #region constructors
        public TemplateInfo()
        {
            IsProntoDoc = false;
            RightPanelStatus = false;
            IsAutoSave = true;

            DomainNames = new List<string>();
        }

        public TemplateInfo(string fullDocName)
            : this()
        {
            FullDocName = fullDocName;
        }
        #endregion

        #region domains name
        public string FirstDomain
        {
            get
            {
                if (DomainNames != null && DomainNames.Count > 0)
                    return DomainNames[0];

                return string.Empty;
            }
        }

        public void AddDomainName(string domainName)
        {
            if (DomainNames == null)
                DomainNames = new List<string>();

            if (!string.IsNullOrWhiteSpace(domainName))
            {
                if (!DomainNames.Exists(d => domainName.Equals(d, System.StringComparison.OrdinalIgnoreCase)))
                {
                    DomainNames.Add(domainName);
                    DomainNames = DomainNames.OrderBy(name => name).ToList();
                }
            }
        }

        public void RemoveDomainAt(int index)
        {
            if(DomainNames != null && DomainNames.Count > index)
                DomainNames.RemoveAt(index);
        }

        public void UpdateDomainNames()
        {
            DomainNames = new List<string>();
            if (InternalBookmark != null && InternalBookmark.InternalBookmarkDomains != null)
            {
                foreach (InternalBookmarkDomain ibmDomain in InternalBookmark.InternalBookmarkDomains)
                {
                    AddDomainName(ibmDomain.DomainName);
                }
            }
        }
        #endregion
    }
}
