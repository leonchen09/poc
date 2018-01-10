using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Pdw.Core;
using Pdw.FormControls;
using Pdw.Managers.Context;
using Pdw.Managers.Hcl;
using Pdw.WKL.Profiler.Services;
using Pdwx.DataObjects;
using ProntoDoc.Framework.CoreObject.DataSegment;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.CoreObject.Render.Schema;
using ProntoDoc.Framework.Utils;
using Office = Microsoft.Office.Core;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Managers.Service
{
    [ComVisible(true)]
    public class ProntoRibbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;
        public bool IsEnableBookmarkButton = true;
        public bool IsReconstruct = false;
        private Managers.Service.UIManager uiService;
        private bool _isSmallToBig = true;
        private bool _isForceDisablePreviewOsql = false;

        public ProntoRibbon()
        {
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("Pdw.Managers.Service.ProntoRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;

            MainManager mainMgr = new MainManager();
            uiService = mainMgr.RibbonManager;
            RegisterHandlerUI();
        }

        #region service events (show, hide, disable)
        /// <summary>
        /// register handler ui for service
        /// </summary>
        private void RegisterHandlerUI()
        {
            uiService.ShowPanel += new Core.ShowPanelEnventHandler(uiService_ShowPanel);
            uiService.HidePanel += new Core.HidePanelEventHandler(uiService_HidePanel);
            uiService.DisableRibbon += new Core.DisableRibbonEventHandler(uiService_DisableRibbon);
            uiService.VisableReconstruct += new Core.VisiableReconstructEventHandler(uiService_VisableReconstruct);
            uiService.VisablePreviewOsql += new Core.VisiablePreviewOsqlEventHandler(uiService_VisableReconstruct);
            uiService.ForceDisablePreviewOsql += new Core.ForceDisablePreviewOsqlEventHandler(uiService_ForceDisablePreviewOsql);
        }

        /// <summary>
        /// when user close panel (is fired on from ThisAddIn)
        /// </summary>
        private void uiService_DisableRibbon()
        {
            IsEnableBookmarkButton = true;
            ribbon.Invalidate();
        }

        /// <summary>
        /// when user click on "Hide" button
        /// </summary>
        private void uiService_HidePanel()
        {
            IsEnableBookmarkButton = true;
            ribbon.Invalidate();
        }

        /// <summary>
        /// When user click on "Show" button
        /// </summary>
        public void uiService_ShowPanel()
        {
            IsEnableBookmarkButton = false;
            ribbon.Invalidate();
        }

        void uiService_VisableReconstruct()
        {
            ribbon.Invalidate();
        }

        void uiService_ForceDisablePreviewOsql()
        {
            _isForceDisablePreviewOsql = false;
            ribbon.Invalidate();
        }
        #endregion

        #region events
        #region office button: btnSaveAsPdw_Click, btnSaveAsPdm_Click, btnSaveAsPdz_Click, FileSave
        public void btnSaveAsPdw_Click(Office.IRibbonControl control)
        {
            SaveTemplate(TemplateType.Pdw);
        }

        public void btnSaveAsPdm_Click(Office.IRibbonControl control)
        {
            SaveTemplate(TemplateType.Pdh);
        }

        public void btnSaveAsPdz_Click(Office.IRibbonControl control)
        {
            SaveTemplate(TemplateType.Pdz);
        }

        //HACK:FORM CONTROLS - btnSaveAsPdm_Hack_Click
        public void btnSaveAsPdm_Hack_Click(Office.IRibbonControl control)
        {
            SaveTemplate(TemplateType.Pdm);
        }

        public void FileSave(Office.IRibbonControl control, bool Cancel)
        {
            try
            {
                TemplateInfo templateInfo =
                    WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;

                if (templateInfo.IsProntoDoc)
                    templateInfo.IsAutoSave = false;

                WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.Save();
            }
            catch (COMException) // prevent user press Ctrl-S and Cancel
            {
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SaveDocumentError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
        }
        #endregion

        #region proto plugin: show/hide
        public void btnStartPronto_Click(Office.IRibbonControl control)
        {
            if (IsEnableBookmarkButton) // currently, bookmark button is enable so we need to disable and show panel
                uiService.UpdateUI(Core.EventType.ShowPanel);
            else
                uiService.UpdateUI(Core.EventType.HidePanel);
        }
        #endregion

        #region controls: preview osql, reconstruct
        public void btnPreviewOsql_Click(Office.IRibbonControl control)
        {
            _isForceDisablePreviewOsql = false;
            Document doc = Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc;

            try
            {
                //Validate document
                string mgrKey = string.Empty;
                Pdw.WKL.Profiler.Manager.ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out mgrKey);
                mgrPro.WdColorIndex = DefineColors.GetColor(selectedColor).Color;
                mgrPro.IsSaveDocument = false;

                ContextValidator validator = new ContextValidator(doc);
                validator.ValidateBeforeSave(mgrKey);

                //If document has invalid bookmarks
                if (!mgrPro.IsCorrect)
                {
                    //Ask user to remove invalid bookmarks
                    mgrPro.WDoc = doc;
                    Hcl.SaveMessageDialog saveNotify = new Hcl.SaveMessageDialog(mgrKey);
                    DialogResult userConfirm = saveNotify.ShowDialog();

                    if (userConfirm != DialogResult.OK || !HasSelectIBM())
                        return;
                }

                //Build and show osql
                PdwInfo pdwInfo = GetPdwInfo(doc);

                ChecksumInfo checkSum = ProntoDoc.Framework.Utils.ObjectSerializeHelper.Deserialize<ChecksumInfo>(pdwInfo.ChecksumString);
                OsqlXml osqlXml = ProntoDoc.Framework.Utils.ObjectSerializeHelper.Deserialize<OsqlXml>(pdwInfo.OsqlString);
                string renderArgument, jRenderArgument;
                GetRenderArguments(checkSum, out renderArgument, out jRenderArgument);

                Pdw.PreviewOsql.PreviewOsqlForm previewDialog = new Pdw.PreviewOsql.PreviewOsqlForm();
                previewDialog.Osql = osqlXml.GetOsql("/*------------------------------------*/");
                previewDialog.JOsql = osqlXml.GetJsql("/*------------------------------------*/");
                previewDialog.Xsl = pdwInfo.XsltString;
                previewDialog.RenderArgument = renderArgument;
                previewDialog.JRenderArgument = jRenderArgument;
                previewDialog.CheckSumInfo = checkSum;
                previewDialog.OsqlXml = osqlXml;
                previewDialog.ShowDialog();
            }
            catch { }
        }

        public void btnReconstruct_Click(Office.IRibbonControl control)
        {
            try
            {
                ContextManager context = new ContextManager();
                context.Reconstruct();
            }
            catch (BaseException srvExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ReconstructError);
                mgrExp.Errors.Add(srvExp);

                LogUtils.LogManagerError(mgrExp);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ReconstructError,
                    MessageUtils.Expand(Properties.Resources.ipe_ReconstructError, ex.Message), ex.StackTrace);

                LogUtils.LogManagerError(mgrExp);
            }
        }
        #endregion

        #region bookmarks: bookmark control, highlight bookmarks
        public void btnBookmarks_Click(Office.IRibbonControl control)
        {
            BookmarkControl bmDialog = new BookmarkControl();
            bmDialog.ShowDialog();
        }

        public void btnHighlightBookmark_Click(Office.IRibbonControl control)
        {
            try
            {
                TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
                if (templateInfo != null && templateInfo.InternalBookmark != null &&
                    templateInfo.DomainNames != null &&
                    templateInfo.DomainNames.Count > 0)
                {
                    DomainSelector domainSelector = new DomainSelector();
                    domainSelector.ShowDialog();
                    Dictionary<string, string> selectedDomains = domainSelector.DataTagColor;
                    bool isHighlight = domainSelector.IsHighlight;
                    string colorNameDS = domainSelector.DocumentSpecificConditionColor;
                    DefineColors.DefineColor colorDS = DefineColors.GetColor(colorNameDS, false);
                    foreach (KeyValuePair<string, string> selectedDomain in selectedDomains)
                    {
                        InternalBookmarkDomain ibmDomain = templateInfo.InternalBookmark.GetInternalBookmarkDomain(selectedDomain.Key);
                        if (ibmDomain != null && ibmDomain.InternalBookmarkItems != null)
                        {
                            string colorNameDT = selectedDomain.Value;
                            DefineColors.DefineColor colorDT = DefineColors.GetColor(colorNameDT, false);
                            
                            foreach (InternalBookmarkItem ibmItem in ibmDomain.InternalBookmarkItems)
                            {
                                string key = string.Empty;
                                Pdw.WKL.Profiler.Manager.ManagerProfile profile = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out key);
                                profile.HighlightBookmarkName = ibmItem.Key;
                                Managers.DataIntegration.DataIntegrationManager bmMgr = new Managers.DataIntegration.DataIntegrationManager();
                                bool isDocumentSpecific = MarkupUtilities.IsProntoDocDocumentSpecificBookmark(ibmItem.Key);
                                if (isDocumentSpecific)
                                {
                                    if (colorDS != null)
                                        profile.WdColorIndex = colorDS.Color;
                                }
                                else
                                {
                                    if (colorDT != null)
                                        profile.WdColorIndex = colorDT.Color;
                                }

                                if (isHighlight)
                                    bmMgr.HighLightBookmark(key);
                                else
                                    bmMgr.UnHighLightBookmark(key);
                            }
                            ibmDomain.Color = isHighlight ? colorNameDT : string.Empty;
                            templateInfo.InternalBookmark.DocumentSpecificColor = isHighlight ? colorNameDS : string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.Log("chkHighlightBookmark_Click", ex);
            }
        }
        #endregion

        #region domain tree: font asc, font desc
        public void btnChangeFontAsc_Click(Office.IRibbonControl control)
        {
            try
            {
                ContextManager context = new ContextManager();

                _isSmallToBig = context.ChangeFontSize(true);

                ribbon.Invalidate();
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ChangeFontError,
                    MessageUtils.Expand(Properties.Resources.ipe_ChangeFontError, ex.Message), ex.StackTrace);

                LogUtils.LogManagerError(mgrExp);
            }
        }

        public void btnChangeFontDesc_Click(Office.IRibbonControl control)
        {
            try
            {
                ContextManager context = new ContextManager();

                _isSmallToBig = context.ChangeFontSize(false);

                ribbon.Invalidate();
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ChangeFontError,
                    MessageUtils.Expand(Properties.Resources.ipe_ChangeFontError, ex.Message), ex.StackTrace);

                LogUtils.LogManagerError(mgrExp);
            }
        }
        #endregion

        #region data selection: comment, foreach, validate
        public void btnComment_Click(Office.IRibbonControl control)
        {
            try
            {
                string bookmarkKey = DateTime.Now.ToString(ProntoMarkup.BookmarkKeyFormat);
                InternalBookmarkItem bm = new InternalBookmarkItem(bookmarkKey, string.Empty,
                    string.Empty, DSIconType.Unknown.ToString());
                bm.Type = XsltType.Comment;
                AddCommentBookmark(bm);
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_AddForechTagError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_AddForechTagError,
                    MessageUtils.Expand(Properties.Resources.ipe_AddForechTagError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// Add bookmark into word document (word object and internal bookmark)
        /// </summary>
        /// <param name="item"></param>
        private void AddCommentBookmark(InternalBookmarkItem item)
        {
            MainManager _mainManager = new MainManager();
            _mainManager.RegisterEvents();

            string key;
            ServicesProfile serviceProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);

            // validate position of data tag with foreach tags
            serviceProfile.ContentService.DomainName = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.SelectedDomainName;

            // add into work bookmark
            serviceProfile.ContentService.AddBookmark_IBookmark = item;
            _mainManager.MainService.BookmarkService.AddBookmark(key);

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        public void btnForEach_Click(Office.IRibbonControl control)
        {
            try
            {
                string bookmarkKey = DateTime.Now.ToString(ProntoMarkup.BookmarkKeyFormat);
                InternalBookmarkItem bm = new InternalBookmarkItem(bookmarkKey, ProntoMarkup.TagContentSForeach,
                    ProntoMarkup.TagContentEForeach, DSIconType.ForEach.ToString());
                bm.Type = XsltType.Foreach;
                AddForeachBookmark(bm);
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_AddForechTagError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_AddForechTagError,
                    MessageUtils.Expand(Properties.Resources.ipe_AddForechTagError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// Add bookmark into word document (word object and internal bookmark)
        /// </summary>
        /// <param name="item"></param>
        private void AddForeachBookmark(InternalBookmarkItem item)
        {
            MainManager _mainManager = new MainManager();
            _mainManager.RegisterEvents();

            string key;
            ServicesProfile serviceProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);

            // validate position of data tag with foreach tags
            serviceProfile.ContentService.DomainName = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.SelectedDomainName;
            _mainManager.MainService.BookmarkService.ValidateDataTagPosition(key);
            if (!serviceProfile.ContentService.IsValid)
            {
                MessageBox.Show(serviceProfile.Message.ToString());
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
                return;
            }

            // add into internal bookmark
            serviceProfile.IntegrationService.AddInternalBM_IBookmark = item;
            _mainManager.MainService.PropertyService.AddInternalBookmark(key);

            // add into work bookmark
            serviceProfile.ContentService.AddBookmark_IBookmark = item;
            _mainManager.MainService.BookmarkService.AddBookmark(key);

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        public void chkCollapseForEach_Click(Office.IRibbonControl control, bool pressed)
        {
            try
            {
                foreach (Microsoft.Office.Interop.Word.Bookmark bm in Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks)
                {
                    string bmName = bm.Name;
                    if (bmName.EndsWith(MarkupConstant.MarkupStartForeach)) // is for-each tag
                    {
                        InternalBookmarkItem ibm =
                            Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.InternalBookmark.GetInternalBookmarkItem(bmName);
                        if (ibm != null && ibm.BizName.StartsWith(Constants.OrderBy.ForeachSortMark)) // has collation
                        {
                            Microsoft.Office.Interop.Word.Range range = bm.Range;
                            if (!pressed) // collapse
                            {
                                range.Font.Hidden = 0;
                            }
                            else // expand old: ibm.BizName != range.Text
                            {
                                int start = bm.Start;
                                int end = bm.End;
                                int startHide = start + ProntoMarkup.TagStartSForeach.Length + ProntoMarkup.TagContentSForeach.Length;
                                int endHide = end - ProntoMarkup.TagEndSForeah.Length;
                                range.SetRange(startHide, endHide);
                                range.Font.Hidden = 1;
                                range.SetRange(start, end);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.Log("btnCollapseForEach_Click", ex);
            }
        }

        public void btnValidate_Click(Office.IRibbonControl control)
        {
            try
            {
                ValidateDocument(DefineColors.GetColor(selectedColor).Color);
            }
            catch { }
        }

        public void glrValidate_Click(Office.IRibbonControl control, string selectedId, int selectedIndex)
        {
            selectedColor = selectedId.Replace("glrColor", "");
            this.ribbon.InvalidateControl("btnValidate");
        }

        private bool ValidateDocument(WdColorIndex color)
        {
            if (Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks.Count > 0)
            {
                ContextManager contextMgr = new ContextManager();
                string mgrKey = string.Empty;
                Pdw.WKL.Profiler.Manager.ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out mgrKey);
                mgrPro.WdColorIndex = color;
                mgrPro.IsSaveDocument = false;
                contextMgr.ValidateDocument(mgrKey);

                return mgrPro.IsCorrect;
            }
            return false;
        }

        private PdwInfo GetPdwInfo(Document Doc)
        {
            string srvKey = string.Empty;

            try
            {
                Document doc = Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc;

                string tempFileName;
                if (File.Exists(doc.FullName))
                    tempFileName = doc.FullName;
                else
                {
                    tempFileName = Path.Combine(Pdw.AssetManager.FileAdapter.TemporaryFolderPath,
                                                Guid.NewGuid().ToString() + FileExtension.ProntoDocumenentWord);
                }
                Context.ContextManager contextMgr = new Context.ContextManager();
                PdwInfo pdwInfo = contextMgr.SaveAsTemplate(doc, tempFileName, false);
                return pdwInfo;
            }
            finally
            {
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);
            }
        }
        #endregion

        #region intergrate pde: import, table, chart, link
        public void btnPdeIntergrateImport_Click(Office.IRibbonControl control)
        {
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            ImportPdeControl importer = new ImportPdeControl(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc,
                templateInfo.PdeContent, true);
            importer.ShowDialog();

            templateInfo.PdeContent = importer.PdeContent;
            templateInfo.IsProntoDoc = true;
        }
        #endregion


        //HACK:FORM CONTROLS - AddControl
        public void btnTextInput_Click(Office.IRibbonControl control)
        {
            AddControl(FormControlType.TextInput);
        }

        public void btnTextArea_Click(Office.IRibbonControl control)
        {
            AddControl(FormControlType.TextArea);
        }

        public void btnButton_Click(Office.IRibbonControl control)
        {
            AddControl(FormControlType.Button);
        }

        public void btnCheckBox_Click(Office.IRibbonControl control)
        {
            AddControl(FormControlType.CheckBox);
        }

        public void btnRadio_Click(Office.IRibbonControl control)
        {
            AddControl(FormControlType.Radio);
        }

        public void btnSelect_Click(Office.IRibbonControl control)
        {
            AddControl(FormControlType.Select);
        }

        public void btnCheckBoxList_Click(Office.IRibbonControl control)
        {
            AddControl(FormControlType.CheckBoxList);
        }

        public void btnRadioList_Click(Office.IRibbonControl control)
        {
            AddControl(FormControlType.RadioList);
        }

        private ControlBase AddControl(FormControlType type)
        {
            string key;
            ControlBase control = null;

            ServicesProfile profile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);
            profile.PdmProfile.ControlType = type;
            profile.PdmProfile.IsAdding = true;

            MainManager manager = new MainManager();

            try
            {
                manager.MainService.PdmService.AddControl(key);
                control = profile.PdmProfile.Control;
            }
            catch (Exception e)
            {
                throw e;
            }

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);

            return control;
        }

        #endregion

        #region visible/disable
        public bool IsEnable(Office.IRibbonControl control)
        {
            // ngocbv: bookmark button is disable when flag is false or document is pronto document          
            return !(!IsEnableBookmarkButton ||
                WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.IsProntoDoc);
        }

        public bool IsVisibleRibbon(Office.IRibbonControl control)
        {
            Pdw.Core.Kernal.DataSegmentHelper.GetListDomain();
            System.Collections.Generic.Dictionary<string, bool> domains = Wkl.MainCtrl.CommonCtrl.CommonProfile.ListDomains;
            bool isVisible = (domains != null && domains.Count > 0);

            return isVisible;
        }

        public bool IsPronto(Office.IRibbonControl control)
        {
            return !IsEnableBookmarkButton;
        }

        public bool EnableReconstruct(Office.IRibbonControl control)
        {
            return WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.IsReconstruct;
        }

        public bool IsEnablePreviewOsql(Office.IRibbonControl control)
        {
            //Value of this variable is set when button preview is clicked and validate failed.
            if (_isForceDisablePreviewOsql)
            {
                _isForceDisablePreviewOsql = false;
                return false;
            }

            bool isEnable = !IsEnableBookmarkButton && HasSelectIBM();
            return isEnable;
        }

        public bool IsVisiblePreviewOsql(Office.IRibbonControl control)
        {
            //visible if the preview dll is found.
            bool isVisible = Pdw.WKL.Profiler.CommonProfile.IsPreviewOsqlDllExist;
            return isVisible;
        }

        public bool EnableForEach(Office.IRibbonControl control)
        {
            return !IsEnableBookmarkButton;
        }

        public bool IsEnableHighlight(Office.IRibbonControl control)
        {
            try
            {
                TemplateInfo template = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
                if (template != null && template.DomainNames != null)
                    return template.DomainNames.Count > 0;
            }
            catch { }

            return false;
        }
        #endregion

        #region icons
        public stdole.IPictureDisp GetPdwIcon(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.pdw.ToBitmap());
        }

        public stdole.IPictureDisp GetPdmIcon(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.pdh.ToBitmap());
        }

        public stdole.IPictureDisp GetPdzIcon(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.pdz.ToBitmap());
        }

        //HACK:FORM CONTROLS - GetPdmIcon_Hack
        public stdole.IPictureDisp GetPdmIcon_Hack(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.pdm.ToBitmap());
        }

        public stdole.IPictureDisp GetImage(Office.IRibbonControl control)
        {
            if (IsEnableBookmarkButton)
                return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Start);
            else
                return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Stop);
        }

        public stdole.IPictureDisp GetBookmarkImage(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Bookmarks);
        }

        public stdole.IPictureDisp GetPreview(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Preview);
        }

        public stdole.IPictureDisp GetFontSizeImageAsc(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.SmallToBigFont);
        }

        public stdole.IPictureDisp GetFontSizeImageDesc(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.BigToSmallFont);
        }

        public stdole.IPictureDisp GetReconstructImage(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Icon_fix);
        }

        public stdole.IPictureDisp GetForEachImage(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Foreach);
        }

        public stdole.IPictureDisp GetCommentImage(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Comment);
        }

        public Bitmap GetGalleryValidateImage(Office.IRibbonControl control)
        {
            return DefineColors.GetColor(selectedColor).Bitmap;
        }

        public stdole.IPictureDisp GetPdeIntergrateImportImage(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Icon_fix);
        }

        private string selectedColor = DefineColors.DefaultColor;
        public Bitmap LoadImage(string imageName)
        {
            string colorName = imageName == "GalleryValidate" ? selectedColor : imageName;
            return DefineColors.GetColor(colorName).Bitmap;
        }
        #endregion

        #region legend
        public string GetLabel(Office.IRibbonControl control)
        {
            string controlID = control.Id;
            return GetLegendContent(controlID, true);
        }

        public string GetDescription(Office.IRibbonControl control)
        {
            string controlID = control.Id;
            return GetLegendContent(controlID, false);
        }

        private string GetLegendContent(string ribbonID, bool isLable)
        {
            if (ribbonID == "btnStartPronto")
                ribbonID = IsEnableBookmarkButton ? ribbonID : "btnHidePronto";
            if (Wkl.MainCtrl.CommonCtrl.CommonProfile.DataSegmentInfo == null)
                Pdw.Core.Kernal.DataSegmentHelper.GetListDomain();
            LegendInfo legend = Wkl.MainCtrl.CommonCtrl.CommonProfile.DataSegmentInfo.GetLegendByName(ribbonID);

            switch (ribbonID)
            {
                case "TabProntoDoc":
                    return isLable ? "ProntoDoc" : "";
                case "ProntoPlugin":
                    return isLable ? "Plugin" : "Command of ProntoDoc plugin";
                case "btnStartPronto":
                case "btnHidePronto":
                    return IsEnableBookmarkButton ? (isLable ? Properties.Resources.ipm_RibbonBtnShow : "Show the right panel") :
                        (isLable ? Properties.Resources.ipm_RibbonBtnHide : "Hide the right panel");
                //
                case "Controls":
                    return isLable ? "Controls" : "Command of ProntoDoc Plugin";
                case "btnReconstruct":
                    return isLable ? "Reconstruct" : "Auto fix and reconstruct Template";
                case "btnPreviewOsql":
                    return isLable ? "Preview" : "Preview Osql Statement";
                //
                case "Bookmarks":
                    return isLable ? "Bookmarks" : "Bookmarks Template";
                case "btnBookmarks":
                    return isLable ? "Bookmarks" : "Show all bookmarks of Template";
                case "btnHighlightBookmark":
                    return isLable ? "Highlight Bookmark" : "Highlight or un-highlight Bookmarks";
                //
                case "DomainTree":
                    return isLable ? "Domain Tree Font Size" : "Change Domain Tree Size";
                case "btnFontAsc":
                    return isLable ? "Ascending" : "Increase Domain Tree Font Size";
                case "btnFontDesc":
                    return isLable ? "Descending" : "Decrease Domain Tree Font Size";
                //
                case "DataSection":
                    return isLable ? "Data Section" : "Data Section";
                case "btnForEach":
                    return isLable ? "For-each" : "Define repeat data section";
                case "chkCollapseForeach":
                    return isLable ? "Collapse For-each" : "Collapse or expand collation of foreach tag";
                case "btnComment":
                    return isLable ? "New Comment" : "Add comment into template";
                case "btnValidate":
                    return isLable ? "Validate" : "Validate and highlight incorrect bookmarks by selected color";
                //
                case "ProntoPdeIntegrate":
                    return isLable ? "Pde Integreate" : "Intergrate with pde template";
                case "btnPdeIntergrateImport":
                    return isLable ? "Import" : "Open a pde template to import";

                //HACK:FORM CONTROLS - GetLegendContent
                case "PdmTemplate":
                    return isLable ? "Pdm Template" : "Controls to design HTML tags";
                case "btnTextInput":
                    return isLable ? "TextBox" : "<input type=\"text\" />";
                case "btnTextArea":
                    return isLable ? "TextArea" : "<textarea />";
                case "btnButton":
                    return isLable ? "Button" : "<input type=\"button|submit|reset\" />";
                case "btnCheckBox":
                    return isLable ? "CheckBox" : "<input type=\"checkbox\" />";
                case "btnRadio":
                    return isLable ? "Radio" : "<input type=\"radio\" />";
                case "btnSelect":
                    return isLable ? "Select" : "<select />";
                case "btnCheckBoxList":
                    return isLable ? "CheckBox List" : "Group of <input type=\"checkbox\" />";
                case "btnRadioList":
                    return isLable ? "Radio List" : "Group of <input type=\"radio\" />";
                default:
                    return string.Empty;
            }
        }
        #endregion

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #region picture helpers
        private class PictureConverter : AxHost
        {
            private PictureConverter()
                : base(string.Empty)
            {
            }

            public static stdole.IPictureDisp ConvertBitmapToPicDisp(Bitmap img)
            {
                return GetIPictureDispFromPicture(img) as stdole.IPictureDisp;
            }
        }
        #endregion

        #region color helpers
        private class DefineColors
        {
            public class DefineColor
            {
                public string Name { get; set; }
                public WdColorIndex Color { get; set; }
                public Bitmap Bitmap { get; set; }

                public DefineColor() { }
                public DefineColor(string name) : this() { Name = name; }
                public DefineColor(string name, WdColorIndex color) : this(name) { Color = color; }
                public DefineColor(string name, WdColorIndex color, Bitmap bitmap) : this(name, color) { Bitmap = bitmap; }
            }

            public static string DefaultColor = "Yellow";
            public static DefineColor GetColor(string colorName, bool useDefaultValue = true)
            {
                if (colorName != null && Colors.ContainsKey(colorName))
                    return Colors[colorName];

                return useDefaultValue ? Colors[DefaultColor] : null;
            }

            private static readonly System.Collections.Generic.Dictionary<string, DefineColor> Colors =
                new System.Collections.Generic.Dictionary<string, DefineColor>(){
                    {"Yellow", new DefineColor("Yellow", WdColorIndex.wdYellow, Properties.Resources.coloryellow)},
                    {"BrightGreen", new DefineColor("BrightGreen", WdColorIndex.wdBrightGreen, Properties.Resources.colorbrightgreen)},
                    {"Turquoise", new DefineColor("Turquoise", WdColorIndex.wdTurquoise, Properties.Resources.colorturquoise)},
                    {"Pink", new DefineColor("Pink", WdColorIndex.wdPink, Properties.Resources.colorpink)},
                    {"Blue", new DefineColor("Blue", WdColorIndex.wdBlue, Properties.Resources.colorblue)},
                    {"Red", new DefineColor("Red", WdColorIndex.wdRed, Properties.Resources.colorred)},
                    {"DarkBlue", new DefineColor("DarkBlue", WdColorIndex.wdDarkBlue, Properties.Resources.colordarkblue)},
                    {"Teal", new DefineColor("Teal", WdColorIndex.wdTeal, Properties.Resources.colorteal)},
                    {"Green", new DefineColor("Green", WdColorIndex.wdGreen, Properties.Resources.colorgreen)},
                    {"Violet", new DefineColor("Violet", WdColorIndex.wdViolet, Properties.Resources.colorviolet)},
                    {"DarkRed", new DefineColor("DarkRed", WdColorIndex.wdDarkRed, Properties.Resources.colordarkred)},
                    {"DarkYellow", new DefineColor("DarkYellow", WdColorIndex.wdDarkYellow, Properties.Resources.colordarkyellow)},
                    {"Gray50", new DefineColor("Gray50", WdColorIndex.wdGray50, Properties.Resources.colorgray50)},
                    {"Gray25", new DefineColor("Gray25", WdColorIndex.wdGray25, Properties.Resources.colorgray25)},
                    {"Black", new DefineColor("Black", WdColorIndex.wdBlack, Properties.Resources.colorblack)}
            };
        }
        #endregion

        private const string PdwFilter = "Pronto Document Word|*.pdw";
        private const string PdhFilter = "Pronto Document Mht|*.pdh";
        private const string PdzFilter = "Pronto Document pdz|*.pdz";

        //HACK:FORM CONTROLS - PdmFilter
        private const string PdmFilter = "Pronto Document pdm|*.pdm";

        private void SaveTemplate(TemplateType type)
        {
            WaitingForm frmWaiting = null;
            WdCursorType originalCursor = WdCursorType.wdCursorNormal;

            try
            {
                originalCursor = Wkl.MainCtrl.CommonCtrl.CommonProfile.App.System.Cursor;
                TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;

                if (templateInfo.IsProntoDoc)
                {
                    string filter = string.Empty;
                    switch (type)
                    {
                        case TemplateType.Pdw:
                            filter = PdwFilter;
                            break;
                        case TemplateType.Pdh:
                            filter = PdhFilter;
                            break;
                        case TemplateType.Pdz:
                            filter = PdzFilter;
                            break;

                        //HACK:FORM CONTROLS - TEMPORARY INSTEAD OF TemplateType.Pdm
                        case TemplateType.Pdm:
                            filter = PdmFilter;
                            break;
                        default:
                            break;
                    }

                    SaveFileDialog saveDialog = new SaveFileDialog { Filter = filter };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // set wating cursor
                        Wkl.MainCtrl.CommonCtrl.CommonProfile.App.System.Cursor = WdCursorType.wdCursorWait;
                        frmWaiting = new WaitingForm();
                        frmWaiting.Show();

                        Context.ContextManager contextMgr = new Context.ContextManager();
                        contextMgr.SaveAsTemplate(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc, saveDialog.FileName);

                        if (File.Exists(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.FullName))
                            MessageBox.Show(MessageUtils.Expand(Properties.Resources.ipm_M006,
                                Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.FullName));
                    }
                }
                else
                    MessageBox.Show(MessageUtils.Expand(Properties.Resources.ipm_NotIsProntoDoc,
                        Properties.Resources.ipe_NotIsProntoDoc));
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SavePdwError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
            finally
            {
                try
                {
                    if (frmWaiting != null)
                    {
                        frmWaiting.Close();
                        frmWaiting.Dispose();
                    }

                    Wkl.MainCtrl.CommonCtrl.CommonProfile.App.System.Cursor = originalCursor;
                }
                catch { }
            }
        }

        private void GetRenderArguments(ChecksumInfo checksum, out string renderArgument, out string jRenderArgument)
        {
            System.Text.StringBuilder renderArgBuilder = new System.Text.StringBuilder();
            System.Text.StringBuilder jrenderArgBuilder = new System.Text.StringBuilder();

            foreach (ChecksumInfoItem item in checksum.ChecksumInfoItems)
            {
                string sysArg = ObjectSerializeHelper.SerializeToString<ProntoDoc.Framework.CoreObject.SystemParameter>(item.SystemParameter);
                string temp = ObjectSerializeHelper.SerializeToString<RenderArgDomainSchema>(item.RenderArgument);
                temp = sysArg + Environment.NewLine + Environment.NewLine + Environment.NewLine + temp;
                renderArgBuilder.AppendLine(temp);

                temp = ObjectSerializeHelper.SerializeToString<JRenderArgDomainSchema>(item.JRenderArgument);
                jrenderArgBuilder.AppendLine(temp);
            }

            renderArgument = renderArgBuilder.ToString();
            jRenderArgument = jrenderArgBuilder.ToString();
        }

        private bool HasSelectIBM()
        {
            return Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.InternalBookmark.HasSelectIBM();
        }
        #endregion
    }
}
