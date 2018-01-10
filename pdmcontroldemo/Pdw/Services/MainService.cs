
using System.Collections.Generic;

using Pdw.Core;
using Pdw.WKL.Profiler.Manager;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;
using Pdw.Services.Template;
using System;
using System.Windows.Forms;

namespace Pdw.Services
{
    public class MainService
    {
        #region Declare main event
        public static event MainEventHandler MainEvent;
        #endregion

        #region declare services
        private static Content.ContentService bmService = null;
        private static Integration.IntegrationService proService = null;
        private static Integration.PdeService pdeService = null;
        private static Integrity.IntegrityService validateProService = null;
        private static Integrity.Validator validateChecksumService = null;
        private static Template.PdwGeneratorService pdwGeneratorService = null;
        public static Template.TemplateService templateService = null;

        //HACK: FORM CONTROLS - pdmService
        private static Integration.PdmService pdmService = null;
        #endregion

        #region Get service
        public Content.ContentService BookmarkService
        {
            get
            {
                InitializeBookmarkService();

                return bmService;
            }
        }

        public Integration.IntegrationService PropertyService
        {
            get
            {
                InitializePropertyService();

                return proService;
            }
        }

        public Integration.PdeService PdeService
        {
            get
            {
                InitializePdeService();

                return pdeService;
            }
        }

        public Integrity.IntegrityService ValidatePropertyService
        {
            get
            {
                InitializeValidatePropertyService();

                return validateProService;
            }
        }

        public Integrity.Validator ChecksumService
        {
            get
            {
                InitializeValidateChecksumService();

                return validateChecksumService;
            }
        }

        public Template.PdwGeneratorService PdwGeneratorService
        {
            get
            {
                InitializePdwGeneratorService();
                return pdwGeneratorService;
            }
        }

        //HACK: FORM CONTROLS - PdmService, pdmService_PdmControlEvent
        public Integration.PdmService PdmService 
        {
            get
            {
                if (pdmService == null)
                {
                    pdmService = new Integration.PdmService();

                    pdmService.AddInternalBookmarkItem += new AddInternalBookmarkItemEventHandler(PdmService_AddInternalBookmarkItem);
                    pdmService.MarkProntoDoc += new MarkProntoDocEventHandler(PdmService_MarkProntoDoc);
                    pdmService.ControlEventBinding += new ControlEventBindingEventHandler(PdmService_ControlEventBinding);
                }

                return pdmService;
            }
        }

        private void PdmService_MarkProntoDoc(string key)
        {
            bmService_MarkProntoDoc(key);
        }

        private void PdmService_AddInternalBookmarkItem(string key)
        {
            ServicesProfile profile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

            profile.IntegrationService.AddInternalBM_IBookmark = profile.PdmProfile.BindingBookmark;

            PropertyService.AddInternalBookmark(key);
        }

        private void PdmService_ControlEventBinding(string key)
        {
            PdmServiceProfile serviceProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).PdmProfile;
            ManagerProfile managerProfile = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            managerProfile.PdmControl = serviceProfile.Control;
            managerProfile.IsAddingPdmControl = serviceProfile.IsAdding;
            managerProfile.ServiceType = ServiceType.ContentService;
            managerProfile.EventType = EventType.ControlEventBinding;
            
            MainEvent(key);

            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        #region User Specificed Condition service
        public Content.ValidateConditionHelper GetUscService(string expression, Dictionary<string, USCItem> listFields)
        {
            return new Content.ValidateConditionHelper(expression, listFields);
        }
        #endregion

        #endregion

        #region Bookmarks
        /// <summary>
        /// Initialize bmService
        /// </summary>
        private void InitializeBookmarkService()
        {
            if (bmService != null)
                return;

            bmService = new Content.ContentService();

            // bookmarks            
            bmService.DeleteBookmark += new DeleteBookmarkEventHandler(bmService_DeleteBookmark);
            bmService.AddBookmarkImageEvent += new AddBookmarkImageEventHandler(bmService_AddBookmarkImageEvent);
            bmService.HighlightBookmark += new HighlightBookmarkEventHandler(bmService_HighlightBookmark);
            bmService.GetInternalBookmark += new GetInternalBookmarkEventHandler(proService_GetInternalBookmark);
            bmService.GetForeachTagsBoundCurrentPosEvent += new GetForeachTagsBoundCurrentPosEventHandler(bmService_GetForeachTagsBoundCurrentPosEvent);

            // helpers
            bmService.AddBookmarkInCurrentSelection += new AddBookmarkEventHandler(bmService_AddBookmarkInCurrentSelection);
            bmService.AddTextToCurrentSelection += new AddTextToCurrentSelectionEventHandler(bmService_AddTextToCurrentSelection);
            bmService.GetCurrentSelection += new DIGetCurrentSelectionEventHandler(bmService_GetCurrentSelection);
            
            bmService.MarkProntoDoc += new MarkProntoDocEventHandler(bmService_MarkProntoDoc);
            bmService.MoveChars += new MoveCharactersEventHandler(bmService_MoveChars);
            bmService.ProtectBookmark += new DIProtectBookmarkEventHandler(bmService_ProtectBookmark);
            bmService.UnProtectBookmark += new DIUnProtectBookmarkEventHandler(bmService_UnProtectBookmark);
            bmService.ProtectWord += new DIProtectWordEventHandler(bmService_ProtectWord);
            bmService.UnProtectWord += new DIUnProtectWordEventHandler(bmService_UnProtectWord);

            bmService.EnableComboboxDomain += new EnableComboboxDomainEventHandler(bmService_EnableComboboxDomain);
            bmService.SetFontForCurrentSelection += new SetFontForCurrentSelectionEventHandler(bmService_SetFontForCurrentSelection);
        }

        private void bmService_EnableComboboxDomain()
        {
            string key = string.Empty;
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.EnableComboboxDomain;
            
            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void bmService_UnProtectWord()
        {
            string key = string.Empty;
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.UnProtectWord;
            
            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void bmService_ProtectWord()
        {
            string key = string.Empty;
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.ProtectWord;
            
            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void bmService_UnProtectBookmark()
        {
            string key = string.Empty;
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.UnProtectBookmark;
            
            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void bmService_ProtectBookmark()
        {
            string key = string.Empty;
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.ProtectBookmark;
            
            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void bmService_MoveChars(string key)
        {
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.MoveChars;

            mgrPro.MoveChars_Count = servicePro.ContentService.MoveChars_Count;
            mgrPro.MoveChars_IsLeft = servicePro.ContentService.MoveChars_IsLeft;
            mgrPro.MoveChars_IsExtend = servicePro.ContentService.MoveChars_IsExtend;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void bmService_MarkProntoDoc(string key)
        {           
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.MarkProntoDoc;

            mgrPro.MarkProntDoc = servicePro.ContentService.MarkProntDoc;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }
        
        private void bmService_HighlightBookmark(string key)
        {   
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.HighlightBookmark;

            mgrPro.HighlightBookmarkName = servicePro.ContentService.HighlightBookmarkName;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
            
        }

        private void bmService_GetCurrentSelection(string key)
        {
         
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.GetCurrentSelection;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);

            servicePro.ContentService.WSelection = mgrPro.WSelection;
        }
        
        private void bmService_DeleteBookmark(string key)
        {         
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.DeleteBookmark;

            mgrPro.DeletedBookmarkName = servicePro.ContentService.DeletedBookmarkName;
            mgrPro.DeleteWholeBookmark = servicePro.ContentService.DeleteWholeBookmark;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void bmService_AddTextToCurrentSelection(string key)
        {        
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);
           
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.AddTextToCurrentSelection;

            mgrPro.AddToSelection_Text = servicePro.ContentService.AddToSelection_Text;
            mgrPro.AddToSelection_IsSelected = servicePro.ContentService.AddToSelection_IsSelected;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void bmService_AddBookmarkInCurrentSelection(string key)
        {           
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);
            
            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.AddBookmarkInCurrentSelection;

            mgrPro.AddBookmark_Name = servicePro.ContentService.AddBookmark_Name;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);

            servicePro.ContentService.AddBookmark_BookmarkReturn = mgrPro.AddBookmark_BookmarkReturn;
        }

        private void bmService_AddBookmarkImageEvent(string key)
        {
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.AddBookmarkImage;

            mgrPro.WbmKey = servicePro.WbmKey;
            mgrPro.WbmValue = servicePro.WbmValue;
            mgrPro.AlternativeText = servicePro.AlternativeText;

            MainEvent(key);

            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);

            servicePro.ContentService.AddBookmark_BookmarkReturn = mgrPro.AddBookmark_BookmarkReturn;
        }

        private void bmService_GetForeachTagsBoundCurrentPosEvent(string key)
        {
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.GetForeachTagsBoundCurrentPos;

            MainEvent(key);

            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);

            servicePro.ContentService.DicBookmarks = mgrPro.DicBookmarks;
        }

        private void bmService_SetFontForCurrentSelection(string key)
        {
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            mgrPro.ServiceType = ServiceType.ContentService;
            mgrPro.EventType = EventType.SetFontForCurrentSelection;

            mgrPro.WdColorIndex = servicePro.ContentService.WdColorIndex;

            MainEvent(key);

            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }
        #endregion

        #region Properties
        /// <summary>
        /// initialize proService
        /// </summary>
        private void InitializePropertyService()
        {
            if (proService != null)
                return;

            proService = new Integration.IntegrationService();

            // internal bookmark object
            proService.GetInternalBookmark += new GetInternalBookmarkEventHandler(proService_GetInternalBookmark);
            proService.SaveInternalBookmark += new SaveInternalBookmarkEventHandler(proService_SaveInternalBookmark);
            // proService.UpdateInternalBM += new UpdateInternalBookmarkEventHandler(proService_UpdateInternalBM);

            // markup properties
            // proService.AddProperty += new AddCustomPropertyEventHandler(proService_AddProperty);// ngocbv_rem re-check
            // proService.GetProperty += new GetPropertyEventHandler(proService_GetProperty); // ngocbv_rem re-check

            // internal bookmark item
            proService.AddInternalBookmarkItem += new AddInternalBookmarkItemEventHandler(proService_AddInternalBookmarkItem);
            proService.RemoveInternalBookmarkItem += new RemoveInternalBookmarkItemEventHandler(proService_RemoveInternalBookmarkItem);

            // usc
            proService.UpdateUscItems += new UpdateUscItemsEventHandler(proService_UpdateUscItems);

            // pdw info
            proService.SavePdwInfo += new SavePdwInfoEventHandler(proService_SavePdwInfo);
            proService.GetPdwInformation += new GetPdwInformationEventHandler(proService_GetPdwInformation);

            // pdwr info
            proService.GetPdwrInformation += new GetPdwrInformationEventHandler(proService_GetPdwrInformation);
        }

        #region markup properties
        //private string proService_GetProperty(string key)
        //{
        //    Package package = new Package(propertyName, ServiceType.IntegrationService, EventType.GetProperty);

        //    MainEvent(package);

        //    return package.Result as string;
        //} // ngocbv_rem re-check

        //private void proService_AddProperty(string name, string value)
        //{
        //    Package package = new Package(ServiceType.IntegrationService, EventType.AddProperty);
        //    package.AddData(name);
        //    package.AddData(value);

        //    MainEvent(package);
        //} // ngocbv_rem re-check
        #endregion

        #region internal bookmark object
        private void proService_SaveInternalBookmark()
        {
            string key = string.Empty;
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out key);
            
            mgrPro.ServiceType = ServiceType.IntegrationService;
            mgrPro.EventType = EventType.SaveInternalBookmark;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void proService_GetInternalBookmark(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            mgrPro.ServiceType = ServiceType.IntegrationService;
            mgrPro.EventType = EventType.GetInternalBookmark;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);

            srvPro.Ibm = mgrPro.Ibm;
            srvPro.PdeContent = mgrPro.PdeContent;
        }
        #endregion

        #region internal bookmark item
        private void proService_AddInternalBookmarkItem(string key)
        {
     
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            mgrPro.ServiceType = ServiceType.IntegrationService;
            mgrPro.EventType = EventType.AddInternalBookmarkItem;
            mgrPro.IbmItem = srvPro.IbmItem;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);

            
        }

        private void proService_RemoveInternalBookmarkItem(string key)
        {            
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            mgrPro.ServiceType = ServiceType.IntegrationService;
            mgrPro.EventType = EventType.RemoveInternalBookmarkItem;
            mgrPro.WbmKey = srvPro.WbmKey;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }
        #endregion

        #region Usc
        private void proService_UpdateUscItems(string key)
        {
            string mgrKey = string.Empty;
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out mgrKey);

            mgrPro.UscItems = srvPro.UscItems;
            mgrPro.ServiceType = ServiceType.IntegrationService;
            mgrPro.EventType = EventType.UpdateUscItems;

            MainEvent(mgrKey);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(mgrKey);
        }
        #endregion

        #region pdw information
        private void proService_SavePdwInfo(string key)
        {
            
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            mgrPro.PdwInfo = srvPro.PdwInfo;
            mgrPro.ServiceType = ServiceType.IntegrationService;
            mgrPro.EventType = EventType.SavePdwInfo;

            MainEvent(key);
            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        private void proService_GetPdwInformation(string key)
        {
           
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            mgrPro.ServiceType = ServiceType.IntegrationService;
            mgrPro.EventType = EventType.GetPdwInformation;
            mgrPro.TemplateType = srvPro.TemplateType;

            MainEvent(key);

            srvPro.XmlObjects = mgrPro.XmlObjects;

            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }
        #endregion

        #region pdwr info
        private void proService_GetPdwrInformation(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(key);

            mgrPro.ServiceType = ServiceType.IntegrationService;
            mgrPro.EventType = EventType.GetPdwrInformation;
            
            MainEvent(key);

            srvPro.IntegrationService.PdwrInfo = mgrPro.PdwrInfo;

            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
 
        }
        #endregion

        #endregion

        #region Validate property (check pronto doc  via properties
        /// <summary>
        /// initialize validateProService
        /// </summary>
        private void InitializeValidatePropertyService()
        {
            if (validateProService != null)
                return;

            validateProService = new Integrity.IntegrityService();
        }
        #endregion

        #region Validate checksum
        /// <summary>
        /// initialize validateChecksumService
        /// </summary>
        private void InitializeValidateChecksumService()
        {
            if (validateChecksumService != null)
                return;

            validateChecksumService = new Integrity.Validator();
        }
        #endregion

        #region PdwGenerateService
        private void InitializePdwGeneratorService()
        {
            if (pdwGeneratorService != null)
                return;

            pdwGeneratorService = new Template.PdwGeneratorService();
        }
        #endregion

        #region Template service
        public Template.TemplateService TemplateService
        {
            get 
            {
                if (templateService == null)
                    templateService = new Template.TemplateService();
                return templateService; 
            }
        }       
        #endregion

        #region Pde service
        /// <summary>
        /// initialize proService
        /// </summary>
        private void InitializePdeService()
        {
            if (pdeService != null)
                return;

            pdeService = new Integration.PdeService();
            
        }
        #endregion
    }
}
