
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdw.Core;
using Pdwx.DataObjects;

namespace Pdw.WKL.Profiler.Services
{
    public class ServicesProfile:BaseProfile
    {
        public TemplateType TemplateType { get; set; }
        public List<DomainInfo> DomainInfos { get; set; }
        public string FullDocName { get; set; }
        public string XsltString { get; set; }
        public bool IsFullDoc { get; set; }
        public PdwInfo PdwInfo { get; set; }        
        public bool IsXsl2007Format { get; set; }

        public string WbmKey { get; set; }
        public string WbmValue { get; set; }
        public string AlternativeText { get; set; }
        
        public InternalBookmark Ibm { get; set; }

        public InternalBookmarkItem IbmItem { get; set; }

        public List<USCItem> UscItems { get; set; }

        public List<XmlObject> XmlObjects { get; set; }

        public PdwrInfo PdwrInfo { get; set; }
        public PdeContent PdeContent { get; set; }

        public string OldKey { get; set; }
        public string NewKey { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }

        #region Content Service Object Transport.

        private ContentServiceProfile _contentService;

        public ContentServiceProfile ContentService
        {
            get
            {
                if (null == _contentService)
                {
                    _contentService = new ContentServiceProfile();
                }
                return _contentService;
            }
            set { _contentService = value; }
        }
        #endregion

        #region Integration Service Object transport

        private IntegrationServiceProfile _integrationService;

        public IntegrationServiceProfile IntegrationService
        {
            get
            {
                if (null == _integrationService)
                {
                    _integrationService = new IntegrationServiceProfile();
                }
                return _integrationService;
            }
            set { _integrationService = value; }
        }
        #endregion

        #region Template Service

        private TemplateServiceProfile _templateService;

        public TemplateServiceProfile TemplateService
        {
            get
            {
                if (null == _templateService)
                {
                    _templateService = new TemplateServiceProfile();
                }
                return _templateService;
            }
            set { _templateService = value; }
        }

        #endregion

        //HACK:FORM CONTROLS

        private PdmServiceProfile _pdmServiceProfile;

        public PdmServiceProfile PdmProfile
        {
            get 
            {
                if (_pdmServiceProfile == null)
                {
                    _pdmServiceProfile = new PdmServiceProfile();
                    _pdmServiceProfile.Result = false;
                }

                return _pdmServiceProfile;
            }
        }
    }
}
