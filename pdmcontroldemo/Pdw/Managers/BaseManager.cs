
using System;

using Office = Microsoft.Office.Core;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdw.Core;

namespace Pdw.Managers
{
    public class BaseManager
    {
        #region properties
        protected WKL.Profiler.CommonProfile CommonProfile
        {
            get
            {
                return WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile;
            }
        }

        protected TemplateInfo CurrentTemplateInfo
        {
            get
            {
                return CommonProfile.CurrentTemplateInfo;
            }
        }
        #endregion

        #region methods
        protected void ChangeTemplateInfoKey(string oldKey, string newKey)
        {
            WKL.DataController.MainController.MainCtrl.CommonCtrl.ChangeTemplateInfoKey(oldKey, newKey);
        }

        /// <summary>
        /// Prevent edit content of bookmark
        /// </summary>
        /// <param name="missing"></param>
        public void ProtectBookmark()
        {
            return;
            //try
            //{
            //    if (!CurrentTemplateInfo.IsAdding)
            //    {
            //        if (CurrentTemplateInfo.ProtectLevel == ProtectLevel.Bookmark)
            //            return;
            //        Pdwx.Services.WordHeper.ProtectDocument(CommonProfile.ActiveDoc, ProtectLevel.Bookmark);
            //        CurrentTemplateInfo.ProtectLevel = ProtectLevel.Bookmark;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ProtectBookmarkError,
            //        MessageUtils.Expand(Properties.Resources.ipe_ProtectBookmarkError, ex.Message), ex.StackTrace);

            //    throw mgrExp;
            //}
        }

        /// <summary>
        /// Allow edit content out of bookmark
        /// </summary>
        public void UnprotectBookmark()
        {
            return;
            //try
            //{
            //    if (CurrentTemplateInfo.ProtectLevel != ProtectLevel.Bookmark)
            //        return;
            //    Pdwx.Services.WordHeper.RemoveProtectPassword(CommonProfile.ActiveDoc, ProtectLevel.Bookmark);
            //    if (CurrentTemplateInfo.ProtectLevel == ProtectLevel.Bookmark)
            //        CurrentTemplateInfo.ProtectLevel = ProtectLevel.None;
            //}
            //catch (Exception ex)
            //{
            //    ManagerException mgrExp = new ManagerException(ErrorCode.ipe_UnProtectBookmarkError,
            //        MessageUtils.Expand(Properties.Resources.ipe_UnProtectBookmarkError, ex.Message), ex.StackTrace);

            //    throw mgrExp;
            //}
        }

        /// <summary>
        /// prevent edit document
        /// </summary>
        public void ProtectDocument()
        {
            try
            {
                if (CurrentTemplateInfo.ProtectLevel == ProtectLevel.Document)
                    return;
                Pdwx.Services.WordHeper.ProtectDocument(CommonProfile.ActiveDoc, ProtectLevel.Document);
                CurrentTemplateInfo.ProtectLevel = ProtectLevel.Document;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ProtectDocumentError,
                    MessageUtils.Expand(Properties.Resources.ipe_ProtectDocumentError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// allow edit document
        /// </summary>
        public void UnprotectDocument()
        {
            try
            {
                if (CurrentTemplateInfo.ProtectLevel != ProtectLevel.Document)
                    return;
                Pdwx.Services.WordHeper.RemoveProtectPassword(CommonProfile.ActiveDoc, ProtectLevel.Document);
                if (CurrentTemplateInfo.ProtectLevel == ProtectLevel.Document)
                    CurrentTemplateInfo.ProtectLevel = ProtectLevel.None;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_UnProtectDocumentError,
                    MessageUtils.Expand(Properties.Resources.ipe_UnProtectDocumentError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        #region define order by
        public static Office.CommandBarButton DefineOrderByButton = null;

        public static void ChangeOrderByStatus(bool isVisible)
        {
            try
            {
                DefineOrderByButton.Visible = isVisible;
            }
            catch { }
        }
        #endregion

        #endregion
    }
}
