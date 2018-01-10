
using Pdw.Core;
using Pdw.Managers.Integration;
using Pdw.Managers.DataIntegration;

using Pdw.WKL.Profiler.Manager;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Managers
{
    public class MainManager
    {
        #region Declare managers
        private IntegrationManager proMgr = new Integration.IntegrationManager();
        private DataIntegrationManager bmMgr = new DataIntegration.DataIntegrationManager();
        #endregion

        #region Declare Service
        private static Services.MainService _mainService;
        public Services.MainService MainService
        {
            get { return _mainService; }
        }
        #endregion

        #region register event for manager
        public void RegisterEvents()
        {
            if (_mainService != null)
                return;

            _mainService = new Services.MainService();
            Services.MainService.MainEvent += new MainEventHandler(mainService_MainEvent);
        }

        private void mainService_MainEvent(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            switch (mgrPro.ServiceType)
            {
                case ServiceType.ContentService:
                    BookmarkEvent(key);
                    break;
                case ServiceType.IntegrationService:
                    PropertyEvent(key);
                    break;
            }
        }

        private void BookmarkEvent(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            switch (mgrPro.EventType)
            {
                case EventType.AddBookmarkInCurrentSelection:
                   bmMgr.AddBookmark(key);
                    break;
                case EventType.AddTextToCurrentSelection:
                    bmMgr.AddText(key);
                    break;
                case EventType.DeleteBookmark:
                    bmMgr.DeleteBookmark(key);
                    break;                
                case EventType.GetCurrentSelection:
                    mgrPro.WSelection = bmMgr.GetCurrentSelection();
                    break;
                case EventType.HighlightBookmark:
                    bmMgr.HighLightBookmark(key);
                    break;
                case EventType.MarkProntoDoc:
                    bmMgr.MarkProntoDoc(key);
                    break;
                case EventType.MoveChars:
                    bmMgr.MoveCharacters(key);
                    break;
                case EventType.ProtectBookmark:
                    bmMgr.ProtectBookmark();
                    break;
                case EventType.UnProtectBookmark:
                    bmMgr.UnprotectBookmark();
                    break;
                case EventType.ProtectWord:
                    bmMgr.ProtectDocument();
                    break;
                case EventType.UnProtectWord:
                    bmMgr.UnprotectDocument();
                    break;
                case EventType.EnableComboboxDomain:
                    Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.ProntoDocMarkup.EnableDomain();
                    break;
                case EventType.AddBookmarkImage:
                    bmMgr.AddImageBookmark(key);
                    break;
                case EventType.GetForeachTagsBoundCurrentPos:
                    bmMgr.GetForeachTagsBoundCurrentPos(key);
                    break;
                case EventType.SetFontForCurrentSelection:
                    bmMgr.SetFont(key);
                    break;
                case EventType.ControlEventBinding:
                    bmMgr.ProcessFormControlAction(key);
                    break;
                default:
                    break;
            }
        }

        private void PropertyEvent(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            switch (mgrPro.EventType)
            {
                case EventType.GetInternalBookmark:
                    proMgr.GetInternalBookmark(key);
                    break;
                case EventType.SaveInternalBookmark:
                    proMgr.SaveInternalBookmark();
                    break;
                case EventType.UpdateInternalBookmark:
                    proMgr.UpdateInternalBookmarkCollection(key);
                    break;
                case EventType.AddInternalBookmarkItem:
                    proMgr.AddInternalBookmarkItem(key);
                    break;
                case EventType.UpdateUscItems:
                    proMgr.UpdateUscItems(key);
                    break;
                case EventType.SavePdwInfo:
                    proMgr.SavePdwInfo(key);
                    break;
                case EventType.GetPdwInformation:
                    proMgr.GetPdwInformation(key);
                    break;
                case EventType.GetPdwrInformation:
                    proMgr.GetPdwrInformation(key);
                    break;
                case EventType.RemoveInternalBookmarkItem:
                    proMgr.RemoveInternalBookmarkItem(key);
                    break;
                default:
                    break;
            }
        }

        //private void ValidatePropertyEvent(string key)
        //{
        //    ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
        //    switch (mgrPro.EventType)
        //    {
        //        case EventType.GetProperty:
        //            proMgr.GetCustomProperty(key);
        //            break;
        //        default:
        //            break;
        //    }
        //}
        #endregion

        #region Using for Ribbon
        private static Service.UIManager _uiMgr;
        public Service.UIManager RibbonManager
        {
            get
            {
                if (_uiMgr == null)
                    _uiMgr = new Service.UIManager();

                return _uiMgr;
            }
        }
        #endregion
    }
}
