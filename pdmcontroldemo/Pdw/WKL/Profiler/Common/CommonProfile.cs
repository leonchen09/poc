
using System.Collections.Generic;

using Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;

using ProntoDoc.Framework.CoreObject.DataSegment;

using Pdw.Core;
using Pdw.Core.Kernal;

namespace Pdw.WKL.Profiler
{
    public class CommonProfile
    {
        #region 1. Word properties
        public ThisAddIn AddIn
        {
            get
            {
                return Globals.ThisAddIn;
            }
        }

        public Application App
        {
            get
            {
                return AddIn.Application;
            }
        }

        public Document ActiveDoc
        {
            get
            {
                try
                {
                    return App.ActiveDocument;
                }
                catch { }

                return null;
            }
        }

        public Bookmarks Bookmarks
        {
            get
            {
                return ActiveDoc == null ? null : ActiveDoc.Bookmarks;
            }
        }

        public Selection CurrentSelection
        {
            get
            {
                return App.Selection;
            }
        }

        public Office.DocumentProperties CustomProperties
        {
            get
            {
                return ActiveDoc == null ? null : ActiveDoc.CustomDocumentProperties;
            }
        }

        public string CurrentFullDocName
        {
            get
            {
                return ActiveDoc != null ? ActiveDoc.FullName : string.Empty;
            }
        }
        #endregion
        
        /// <summary>
        /// Contains all domains in plugin
        /// </summary>
        public Dictionary<string, bool> ListDomains { get; set; }

        public Dictionary<string, Classifer> Classifiers { get; set; }

        #region 2. Template information
        public Dictionary<string, TemplateInfo> TemplateInfos;

        /// <summary>
        /// get current template information (activing document)
        /// </summary>
        /// <returns></returns>
        public TemplateInfo CurrentTemplateInfo
        {
            get
            {
                string fullDocName = CurrentFullDocName;
                if (string.IsNullOrEmpty(fullDocName) )
                    TemplateInfos[fullDocName] = new Core.TemplateInfo();

                else if (!TemplateInfos.ContainsKey(fullDocName))
                    TemplateInfos.Add(fullDocName, new Core.TemplateInfo());
               
                return TemplateInfos[fullDocName];               
            }
        }

        #endregion

        #region 3. Datasegment information
        public Dictionary<string, DomainInfo> DomainInfos;
        
        private DSPlugin _pluginInfo;
        public DSPlugin PluginInfo
        {
            get
            {
                if (_pluginInfo == null)
                    _pluginInfo = DataSegmentHelper.GetPlugInInfo();
                return _pluginInfo;
            }
        }

        public Pdw.SharedMemoryWrapper.DataSegmentInfo DataSegmentInfo { get; set; }

        public Dictionary<string, IconInfo> _iconInfos;
        public void AddIconInfo(IconInfo iconInfo)
        {
            if (_iconInfos == null)
                _iconInfos = new Dictionary<string, IconInfo>();

            if (iconInfo != null)
            {
                string key = iconInfo.Key;
                if(!_iconInfos.ContainsKey(key))
                    _iconInfos.Add(key, iconInfo);
            }
        }

        public IconInfo GetIconInfo(string dbid, string customerIconID)
        {
            string key = IconInfo.GenKey(dbid, customerIconID);
            if (_iconInfos != null && _iconInfos.ContainsKey(key))
                return _iconInfos[key];

            return null;
        }
        #endregion

        public UserData UserData { get; set; }

        public CommonProfile()
        {
            TemplateInfos = new Dictionary<string, TemplateInfo>();
            DomainInfos = new Dictionary<string, DomainInfo>();
        }

        public bool IsAddSection
        {
            get
            {
                try
                {
                    // rule: only accept multi section when template using one BD and BD allows multi section
                    if (CurrentTemplateInfo.DomainNames != null && CurrentTemplateInfo.DomainNames.Count == 1)
                    {
                        string domainName = CurrentTemplateInfo.DomainNames[0];
                        DomainInfo domainInfo = WKL.DataController.MainController.MainCtrl.CommonCtrl.GetDomainInfo(domainName);
                        return domainInfo.DSDomainData.MultiSection;
                    }
                }
                catch { }

                return false;
            }
        }

        private static bool? _isPreviewOsqlDllExist = null;
        public static bool IsPreviewOsqlDllExist {
            get
            {
                if (_isPreviewOsqlDllExist == null)
                    _isPreviewOsqlDllExist = System.IO.File.Exists(Constants.PreviewOsqlControl.DllName);

                return _isPreviewOsqlDllExist.Value;
            }
        }

        public string FindClassifier(string domainName)
        {
            if (!string.IsNullOrWhiteSpace(domainName) && Classifiers != null)
            {
                foreach (KeyValuePair<string, Classifer> classifer in Classifiers)
                {
                    if (classifer.Value.DomainNames != null &&
                        classifer.Value.DomainNames.Exists(d => domainName.Equals(d, System.StringComparison.OrdinalIgnoreCase)))
                        return classifer.Key;
                }
            }

            return null;
        }
    }
}