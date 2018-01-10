
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;

using ProntoDoc.Framework.Utils;
using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdwx.DataObjects;

using Pdw.Core;
using Pdw.WKL.Profiler.Manager;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Managers.Context
{
    public class ContextManager
    {
        private MainManager mainManager;

        public ContextManager()
        {
            mainManager = new MainManager();
            mainManager.RegisterEvents();
        }

        #region 1. Open document
        /// <summary>
        /// open document
        /// </summary>
        /// <param name="Doc"></param>
        public void OpenDocument(Document Doc)
        {
            //1.Check File structure
            //If true
            //   1.1 Update Status
            //   1.2 Create TemplateInfo object in WKL
            //   1.3 Validate with Domain
            string key;
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);

            servicePro.IntegrationService.CheckCorrectContent_IFilePath = Doc.FullName;
            mainManager.MainService.PropertyService.IsCorrectFileStructure(key);

            if ((bool)servicePro.IntegrationService.Result)
            {
                UpdateStatus(MessageUtils.Expand(Properties.Resources.ipm_CheckingMessage, Doc.FullName));

                // Create TemplateInfo to WKL. Key is full name of doc
                TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CreateTemplateInfo(Doc.FullName);
                templateInfo.IsProntoDoc = true;

                // check struct
                List<string> message = new List<string>();
                bool isMatchWithDomain = true;
                bool isMatchWithDataSegment = true;
                bool isCorrectChecksum = true;

                // Validate Internal BM with any Domain
                servicePro.IntegrationService.TemplateFileName = Doc.FullName;
                mainManager.MainService.PropertyService.IsInternalBMMatchWithDomain(key);
                isMatchWithDomain = (bool)servicePro.IntegrationService.Result;
                string popupMessage = Properties.Resources.ipm_WrongMessages;

                // Validate Word structure 
                IsCorrectWordStruct(key, Doc, ref message);

                // Compare Internal BM and Word BM.
                IsMatchBetweenInternalAndWord(ref message, false, Doc.FullName);

                // show reconstruct button
                if (message.Count > 0)
                    templateInfo.IsReconstruct = true;
                else
                    templateInfo.IsReconstruct = false;

                if (!isMatchWithDomain)
                {
                    if (servicePro.IntegrationService.CheckMatchWithDomain_OListMatch != null &&
                        servicePro.IntegrationService.CheckMatchWithDomain_OListMatch.Count > 0)
                    {
                        //Show dialog to user select a domain.
                        Hcl.ChooseDomain chooseDomain = new Hcl.ChooseDomain(servicePro.IntegrationService.CheckMatchWithDomain_OListMatch);
                        chooseDomain.ShowDialog();

                        List<DomainMatch> domainMatches = chooseDomain.DomainMatches;
                        chooseDomain.Dispose();
                        mainManager.RibbonManager.UpdateUI(EventType.ShowPanel);
                        if (!domainMatches.Exists(d => !string.IsNullOrWhiteSpace(d.NewDomainName)))
                        {
                            if (templateInfo.RightPanel != null)
                                (templateInfo.RightPanel.Control as Hcl.ProntoDocMarkup).UpdateExceptionList(message, false, string.Empty);
                        }
                        else
                        {
                            foreach (DomainMatch domainMatch in domainMatches)
                            {
                                if (string.IsNullOrWhiteSpace(domainMatch.NewDomainName))
                                    continue;

                                if (domainMatch.NewDomainName.Equals(domainMatch.DomainName))
                                {
                                    HighlightNotMatchFields(domainMatch); //Highlight unmatch Fields
                                    continue;
                                }

                                templateInfo.DomainNames.Add(domainMatch.NewDomainName);
                                HighlightNotMatchFields(domainMatch); //Highlight unmatch Fields
                                isMatchWithDataSegment = IsMatchWithDataSegment(Doc.FullName, ref message);
                                if (templateInfo.RightPanel != null)
                                    (templateInfo.RightPanel.Control as Hcl.ProntoDocMarkup).UpdateExceptionList(message,
                                        isMatchWithDataSegment, domainMatch.NewDomainName);
                            }

                            if (templateInfo.IsReconstruct)
                            {
                                if (message.Count > 0) // has error when check construct
                                    popupMessage += Properties.Resources.ipm_WrongBookmarkMessages;
                                if (!isMatchWithDomain || !isMatchWithDataSegment)
                                    popupMessage += Properties.Resources.ipm_WrongDomainMessages;
                                mainManager.MainService.BookmarkService.ProtectDocument();
                            }
                            else
                            {
                                mainManager.MainService.BookmarkService.UnProtectDocument();
                                Wkl.MainCtrl.CommonCtrl.CommonProfile.App.Options.SaveInterval = ProntoMarkup.SaveInterval;
                            }
                        }
                    }
                    else
                    {
                        HighlightAllBookmark(templateInfo.InternalBookmark);
                        if (templateInfo.RightPanel != null)
                            (templateInfo.RightPanel.Control as Hcl.ProntoDocMarkup).UpdateExceptionList(message, false, string.Empty);
                    }
                }
                else
                {
                    //HACK:FORM CONTROLS - Restore
                    TemplateType templateType = MarkupUtilities.GetTemplateType(Doc.FullName);

                    if (templateType == TemplateType.Pdm)
                    {
                        mainManager.MainService.PdmService.Restore(key);
                    }

                    mainManager.RibbonManager.UpdateUI(EventType.ShowPanel);
                }

                // decorate treeview
                if (templateInfo.RightPanel != null)
                    (templateInfo.RightPanel.Control as Hcl.ProntoDocMarkup).DecorateTreeView();

                // Validate CheckSum
                isCorrectChecksum = IsCorrectChecksum();

                if (!isCorrectChecksum)
                    popupMessage += Properties.Resources.ipm_WrongChecksumMessage;

                if (popupMessage != Properties.Resources.ipm_WrongMessages)
                    ShowPopupMessage(string.Format(popupMessage, "\n"));

                UpdateStatus(MessageUtils.Expand(Properties.Resources.ipm_FinishLoading, Doc.FullName));
            }

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        #region Helper

        #region CheckSum

        private bool IsCorrectChecksum()
        {
            return mainManager.MainService.ValidatePropertyService.IsProntoDoc();
        }

        #endregion

        #region Struct

        private bool IsCorrectWordStruct(string key, Document Doc, ref List<string> message)
        {
            try
            {
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.XmlContent = GetWordBodyOuterXml(Doc);
                mainManager.MainService.TemplateService.CheckWordBodyStructure();

                foreach (XmlNode node in Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.DeletedTags)
                    message.Add(node.OuterXml);

                bool isError = (Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.DeletedTags.Count == 0);

                return isError;
            }
            catch (BaseException baseEx)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_CheckWordStructError);
                mgrExp.Errors.Add(baseEx);

                throw mgrExp;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_CheckWordStructError,
                    MessageUtils.Expand(Properties.Resources.ipe_CheckWordStructError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// Get outer xml of body node (w:document) in word open xml document
        /// </summary>
        /// <returns></returns>
        private string GetWordBodyOuterXml(Document doc)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(doc.WordOpenXML);
                XmlNodeList wDoc = xmlDoc.GetElementsByTagName(XmlDocNodeName.Document);

                if (wDoc != null && wDoc.Count > 0)
                    return wDoc[0].OuterXml;
            }
            catch
            {
                throw new Exception(Properties.Resources.ipe_GetWordBodyOuterXml);
            }

            throw new Exception(Properties.Resources.ipe_GetWordBodyOuterXml);
        }
        #endregion

        #region Compare Internal Bookmark and Word BM
        private bool IsMatchBetweenInternalAndWord(ref List<string> message, bool isUpdate, string templateName)
        {
            try
            {
                // update word bm (remove bm exist in word but not exist in internal)
                InternalBookmark interBm = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(templateName).InternalBookmark;
                string serviceKey;

                //Add to WKL
                ServicesProfile serviceProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out serviceKey);
                ContentServiceProfile contentProfile = serviceProfile.ContentService;
                contentProfile.ValidateBookmark_IIsUpdate = isUpdate;
                contentProfile.ValidateBookmark_ITemplateName = templateName;
                mainManager.MainService.BookmarkService.ValidateBookmarkCollection(serviceKey);
                message.AddRange(contentProfile.ValidateBookmark_ORemovedList);

                // update internal bookmark (remove bm exist in internal but not exist in word)
                mainManager.MainService.BookmarkService.GetBookmarkCollection(serviceKey);

                IntegrationServiceProfile integrationPro = serviceProfile.IntegrationService;
                integrationPro.ValidateInternalBM_IListBM = contentProfile.GetBookmarks_OListBM;
                integrationPro.ValidateInternalBM_IIsUpdate = isUpdate;
                mainManager.MainService.PropertyService.ValidateInternalBookmarkCollection(serviceKey);
                message.AddRange(integrationPro.ValidateInternalBM_OListError);

                //Remove objects in Wkl
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(serviceKey);

                return (contentProfile.ValidateBookmark_ORemovedList.Count == 0 &&
                    integrationPro.ValidateInternalBM_OListError.Count == 0);
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_CheckIbmAndWbmError);
                mgrExp.Errors.Add(baseExp);

                throw mgrExp;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_CheckIbmAndWbmError,
                    MessageUtils.Expand(Properties.Resources.ipe_CheckIbmAndWbmError, ex.Message), ex.StackTrace);
                throw mgrExp;
            }
        }
        #endregion

        #region Match With DataSegment
        private bool IsMatchWithDataSegment(string docFullName, ref List<string> message)
        {
            try
            {
                // compare with data domain. if not exist in datadomain then highlight
                string key;
                ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key).ContentService;
                contentProfile.ValidateBookmark_ITemplateName = docFullName;

                mainManager.MainService.BookmarkService.ValidateBookmarkCollectionWithDomain(key);

                bool result = (bool)contentProfile.Result;

                message.AddRange(contentProfile.UnMatchedFields);

                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);

                return result;
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_CheckWbmWithDatasegment);
                mgrExp.Errors.Add(baseExp);

                throw mgrExp;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_CheckWbmWithDatasegment,
                    MessageUtils.Expand(Properties.Resources.ipe_CheckWbmWithDatasegment, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }
        #endregion

        #region Highlight

        private void HighlightNotMatchFields(DomainMatch domainMatch)
        {
            try
            {
                foreach (DomainMatchItem matchItem in domainMatch.DomainMatchItems.Values)
                {
                    foreach (string wBmValue in matchItem.NotMatchBizs)
                    {
                        foreach (Bookmark wBm in Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.Bookmarks)
                        {
                            if (MarkupUtilities.GetRangeText(wBm.Range) == wBmValue)
                                wBm.Range.HighlightColorIndex = ProntoMarkup.BackgroundHighLight;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_HighlightBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_HighlightBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        private void HighlightAllBookmark(InternalBookmark iBm)
        {
            try
            {
                if (iBm != null && iBm.InternalBookmarkDomains != null)
                {
                    string key;

                    ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key).ContentService;
                    foreach (InternalBookmarkDomain ibmDomain in iBm.InternalBookmarkDomains)
                    {
                        foreach (InternalBookmarkItem item in ibmDomain.InternalBookmarkItems)
                        {
                            contentProfile.HighlightBookmark_IBMName = item.Key;
                            mainManager.MainService.BookmarkService.HighlightWordBookmark(key);
                        }
                    }
                    Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
                }
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_HighlightBookmarkError);
                mgrExp.Errors.Add(baseExp);
                throw mgrExp;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_HighlightBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_HighlightBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        #endregion

        /// <summary>
        /// show message in the status bar of word document
        /// </summary>
        /// <param name="message"></param>
        public void UpdateStatus(string message)
        {
            try
            {
                Wkl.MainCtrl.CommonCtrl.CommonProfile.App.StatusBar = message;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_UpdateStatusBarError,
                    MessageUtils.Expand(Properties.Resources.ipe_UpdateStatusBarError, ex.Message), ex.StackTrace);
                throw mgrExp;
            }
        }

        private void ShowPopupMessage(string message)
        {
            System.Windows.Forms.MessageBox.Show(message, Properties.Resources.ipm_ConfirmCaption,
                System.Windows.Forms.MessageBoxButtons.OK);
        }
        #endregion
        #endregion

        #region 2. Open pdwr
        /// <summary>
        /// Oen pdwr file
        /// </summary>
        /// <param name="Doc"></param>
        public void OpenPdwr(Document Doc, ref List<string> tempFiles)
        {
            // get pdwr info
            string mgrKey = string.Empty;
            string srvKey = string.Empty;

            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
            mainManager.MainService.PropertyService.GetPdwrInfos(srvKey);
            PdwrInfo pdwrInfo = srvPro.IntegrationService.PdwrInfo;

            // translate pdwr
            // if (!string.IsNullOrEmpty(pdwrInfo.XmlString) && !string.IsNullOrEmpty(pdwrInfo.XsltString))
            if (!string.IsNullOrEmpty(pdwrInfo.XmlString))
            {
                string folderPath = AssetManager.FileAdapter.TemporaryFolderPath;
                string fileName = System.Guid.NewGuid().ToString();

                string xmlFilePath = string.Format("{0}\\{1}{2}", folderPath, fileName, FileExtension.Xml); // mwxml
                string xslFilePath = string.Format("{0}\\{1}{2}", folderPath, fileName, FileExtension.Xsl);

                FileHelper.CreateFile(xmlFilePath, pdwrInfo.XmlString);
                tempFiles.Add(xmlFilePath);
                if (!string.IsNullOrWhiteSpace(pdwrInfo.XsltString)) // support old file
                {
                    FileHelper.CreateFile(xslFilePath, pdwrInfo.XsltString);
                    tempFiles.Add(xslFilePath);
                }

                Document newDoc = null;
                if (!string.IsNullOrWhiteSpace(pdwrInfo.XsltString)) // support old file
                {
                    newDoc = Wkl.MainCtrl.CommonCtrl.CommonProfile.App.Documents.Open(
                        FileName: xmlFilePath,
                        Format: WdOpenFormat.wdOpenFormatXML,
                        XMLTransform: xslFilePath);
                }
                else
                {
                    newDoc = Wkl.MainCtrl.CommonCtrl.CommonProfile.App.Documents.Open(
                        FileName: xmlFilePath, Format: WdOpenFormat.wdOpenFormatXML);
                }
                Wkl.MainCtrl.CommonCtrl.CommonProfile.App.ScreenUpdating = true;
                newDoc.ActiveWindow.View.Type = WdViewType.wdPrintView;

                // update pde
                if (!string.IsNullOrWhiteSpace(pdwrInfo.PdeContent))
                {
                    PdeContent pdeContent = ObjectSerializeHelper.Deserialize<PdeContent>(pdwrInfo.PdeContent);
                    List<string> excelFiles = mainManager.MainService.PropertyService.RenderPdeInPdw(newDoc, pdeContent);
                    tempFiles.AddRange(excelFiles);
                }
                newDoc.Save();

                #region process RenderSetting
                if (!string.IsNullOrEmpty(pdwrInfo.SettingString))
                {
                    RenderSettings renderSetting = ObjectSerializeHelper.Deserialize<RenderSettings>(pdwrInfo.SettingString);
                    if (renderSetting != null)
                    {
                        switch (renderSetting.Channel)
                        {
                            #region Media = Display
                            case RenderSettings.ChannelType.Display:
                                switch (renderSetting.Media)
                                {
                                    case RenderSettings.MediaType.Pdf:
                                        newDoc.ExportAsFixedFormat(Path.ChangeExtension(xmlFilePath, FileExtension.PdfNoDot),
                                            WdExportFormat.wdExportFormatPDF, true);
                                        ((_Document)newDoc).Close();
                                        break;
                                    case RenderSettings.MediaType.Xps:
                                        newDoc.ExportAsFixedFormat(Path.ChangeExtension(xmlFilePath, FileExtension.XpsNoDot),
                                            WdExportFormat.wdExportFormatXPS, true);
                                        ((_Document)newDoc).Close();
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            #endregion
                            #region Media = Email, Media = Attachment
                            case RenderSettings.ChannelType.Email:
                            case RenderSettings.ChannelType.Attachment:
                                MailHelper.SendEmail(newDoc, renderSetting, ref tempFiles);
                                break;
                            #endregion
                            #region Media = Fax
                            case RenderSettings.ChannelType.Fax:
                                Wkl.MainCtrl.CommonCtrl.CommonProfile.App.CommandBars.ExecuteMso(
                                    Constants.SendInternetFax);
                                break;
                            #endregion
                            default:
                                break;
                        }
                    }
                }

                #endregion
            }
        }
        #endregion

        #region 3. re-construct
        /// <summary>
        /// reconstruct the documemnt
        /// </summary>
        public void Reconstruct()
        {
            Document Doc = Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc;
            string filePath = Doc.FullName;

            if (System.Windows.Forms.MessageBox.Show(MessageUtils.Expand(Properties.Resources.ipm_ConfirmMessage, filePath),
                Properties.Resources.ipm_ConfirmCaption
                        , System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                // unprotect word
                mainManager.MainService.BookmarkService.UnProtectDocument();

                // update word bookmark and internal bookmark
                List<string> message = new List<string>();
                IsMatchBetweenInternalAndWord(ref message, true, filePath);

                string key;
                TemplateServiceProfile templateServicePro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key).TemplateService;
                templateServicePro.TemplateName = Doc.FullName;
                templateServicePro.FilePath = filePath;

                // save document
                Save(Doc, Doc.FullName, isReconstruct: true);

                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.XmlContent = GetWordBodyOuterXml(Doc);
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.IsReConstructing = true;

                // close document for repair xml file
                ((_Document)Doc).Close();

                // repair xml file      
                if (mainManager.MainService.TemplateService != null)
                    mainManager.MainService.TemplateService.Repair(key);
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.IsReConstructing = false;

                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);

                // re-open file
                System.Diagnostics.Process.Start(filePath);
            }
        }
        #endregion

        #region 4. Save document

        public void ValidateDocument(string key, Document Doc = null)
        {
            if (Doc == null)
                Doc = Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc;
            ContextValidator validator = new ContextValidator(Doc);
            validator.ValidateBeforeSave(key);

            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            if (!mgrPro.IsCorrect)
            {
                mgrPro.WDoc = Doc;
                Hcl.SaveMessageDialog saveNotify = new Hcl.SaveMessageDialog(key);
                saveNotify.Show();
            }
        }

        #region 4.1. Save docx
        /// <summary>
        /// Save a document
        /// </summary>
        /// <param name="Doc"></param>
        /// <param name="SaveAsUI"></param>
        /// <param name="Cancel"></param>
        public void SaveDocument(Document Doc, ref bool SaveAsUI, ref bool Cancel)
        {
            TemplateInfo tempInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(Doc.FullName);
            if (tempInfo == null)
                return;

            if (!tempInfo.IsSaving)
            {
                string mgrKey = string.Empty;
                string filePath = Doc.FullName;
                bool isAutoSaving = tempInfo.IsAutoSave;
                ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out mgrKey);
                mgrPro.TemplateType = MarkupUtilities.GetTemplateType(filePath);

                ContextValidator validator = new ContextValidator(Doc);
                validator.ValidateBeforeSave(mgrKey);

                tempInfo.IsAutoSave = true;
                Cancel = true;
                SaveAsUI = false;

                if (!mgrPro.IsCorrect) // not AutoSave then we need to validate, check file path and prevent save event of word object
                {
                    if (!isAutoSaving)
                    {
                        tempInfo.IsSaving = false;
                        if (mgrPro.WbmKeys.Count == 0)
                            ShowPopupMessage(MessageUtils.Expand(Properties.Resources.ipe_NotIsProntoDoc, Properties.Resources.ipe_ValidateMessage));
                        else
                        {
                            mgrPro.WDoc = Doc;
                            Hcl.SaveMessageDialog saveNotify = new Hcl.SaveMessageDialog(mgrKey);
                            saveNotify.Show();
                        }
                    }

                    UpdateStatus(MessageUtils.Expand(Properties.Resources.ipm_NotIsProntoDoc, Properties.Resources.ipe_NotSaveMessage));
                }
                else
                {
                    if (!isAutoSaving && !File.Exists(filePath))
                        filePath = GetNewFileName();

                    Save(Doc, filePath);

                    SaveUserData();
                }

                Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(mgrKey);
            }
        }
        #endregion

        #region 4.2. Save pdw
        /// <summary>
        /// Save document as pronto template
        /// </summary>
        /// <param name="Doc"></param>
        /// <param name="filePath"></param>
        /// <param name="isToFinal">true: Actually Save to file. false: save to temp file to get xsl,...</param>
        public PdwInfo SaveAsTemplate(Document Doc, string filePath, bool isToFinal = true)
        {
            TemplateInfo tempInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(Doc.FullName);
            if (tempInfo == null)
                return null;

            string mgrKey = string.Empty;
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out mgrKey);
            mgrPro.TemplateType = MarkupUtilities.GetTemplateType(filePath);
            ContextValidator validator = new ContextValidator(Doc);
            validator.ValidateBeforeSave(mgrKey);

            PdwInfo pdwInfo = null;
            if (!mgrPro.IsCorrect)
            {
                tempInfo.IsSaving = false;
                if (mgrPro.WbmKeys.Count == 0)
                    ShowPopupMessage(MessageUtils.Expand(Properties.Resources.ipe_NotIsProntoDoc,
                        Properties.Resources.ipe_ValidateMessage));
                else
                {
                    mgrPro.WDoc = Doc;
                    Hcl.SaveMessageDialog saveNotify = new Hcl.SaveMessageDialog(mgrKey);
                    saveNotify.Show();
                }
            }
            else
            {
                pdwInfo = Save(Doc, filePath, isToFinal: isToFinal);

                if (isToFinal)
                    SaveUserData();
            }

            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(mgrKey);

            return pdwInfo;
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Save document information
        /// </summary>
        /// <param name="Doc"></param>
        /// <param name="filePath"></param>
        /// <param name="isReconstruct"></param>
        private PdwInfo Save(Document Doc, string filePath, bool isReconstruct = false, bool isToFinal = true)
        {
            try
            {
                TemplateInfo tempInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(Doc.FullName);
                if (tempInfo == null)
                    return null;

                PdwInfo pdwInfo = null;
                tempInfo.IsSaving = true; // markup the document is saving

                if (!string.IsNullOrEmpty(filePath))
                {
                    // put where clause into internal bookmark
                    foreach (string domainName in tempInfo.DomainNames)
                    {
                        DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName);
                        InternalBookmarkDomain ibmDomain = tempInfo.InternalBookmark.GetInternalBookmarkDomain(domainName);
                        if (ibmDomain != null)
                            ibmDomain.WhereClause = domainInfo.DSDomainData.WhereClause.Clause;
                    }

                    // save pdwinfo (checksum, oslq, xslt)
                    pdwInfo = SavePdwInfo(filePath, Doc, isReconstruct);

                    // save internal bookmark
                    mainManager.MainService.PropertyService.SaveInterBookmark();

                    // save the document
                    if (isToFinal)
                    {
                        TemplateType type = MarkupUtilities.GetTemplateType(filePath);

                        if (type == TemplateType.Pdm)
                        {
                            mainManager.MainService.PdmService.Save();
                        }
                        else
                        {
                            // Doc.SaveAs(filePath);
                            Doc.Save();
                        }
                    }

                    tempInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(Doc.FullName);
                    if (tempInfo != null)
                    {
                        tempInfo.IsProntoDoc = true; // markup the document is pronto doc

                        // set autosave option
                        if (!isReconstruct && Wkl.MainCtrl.CommonCtrl.CommonProfile.App.Options.SaveInterval != ProntoMarkup.SaveInterval)
                            Wkl.MainCtrl.CommonCtrl.CommonProfile.App.Options.SaveInterval = ProntoMarkup.SaveInterval;

                        if (isToFinal)
                            UpdateStatus(MessageUtils.Expand(Properties.Resources.ipm_SaveSuccessful, filePath)); // update status
                    }
                }

                if (tempInfo != null)
                    tempInfo.IsSaving = false; // markup the document is saved

                return pdwInfo;
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SaveDocumentError);
                mgrExp.Errors.Add(baseExp);

                throw mgrExp;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SaveDocumentError,
                    MessageUtils.Expand(Properties.Resources.ipe_SaveDocumentError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// save pdw information into document
        /// save document -> input pdw information -> re-save document
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="Doc"></param>
        /// <param name="isReconstruct"></param>
        private PdwInfo SavePdwInfo(string filePath, Document Doc, bool isReconstruct)
        {
            try
            {
                string srvKey = string.Empty;
                ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);

                // get pdw information                
                string fileExt = Path.GetExtension(filePath);
                TemplateType templateType = GetTemplateType(fileExt);
                if (!isReconstruct && templateType != TemplateType.None)
                {
                    string oldKey = Doc.FullName;
                    Doc.SaveAs(filePath);
                    Wkl.MainCtrl.CommonCtrl.ChangeTemplateInfoKey(oldKey, Doc.FullName);
                    srvPro.TemplateType = templateType;
                    srvPro.IsFullDoc = true;
                }
                else
                {
                    srvPro.IsFullDoc = false;
                    srvPro.TemplateType = TemplateType.None;
                }
                srvPro.FullDocName = Doc.FullName;
                mainManager.MainService.PdwGeneratorService.GetPdwInfo(srvKey);

                // save pdw information
                mainManager.MainService.PropertyService.SavePdwInformation(srvKey);

                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);

                return srvPro.PdwInfo;
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SavePdwInfoError);
                mgrExp.Errors.Add(baseExp);

                throw mgrExp;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SavePdwInfoError,
                    MessageUtils.Expand(Properties.Resources.ipe_SaveDocumentError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        private TemplateType GetTemplateType(string fileExt)
        {
            if (FileExtension.ProntoDocumenentWord.Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                return TemplateType.Pdw;
            else if (FileExtension.ProntoDocumentMht.Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                return TemplateType.Pdh;
            else if (FileExtension.ProntoDocumentPdz.Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                return TemplateType.Pdz;

            //HACK:FORM CONTROLS - GetTemplateType
            else if (FileExtension.ProntoDocumentMobile.Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                return TemplateType.Pdm;

            return TemplateType.None;
        }

        /// <summary>
        /// Show dialog and get file path where user want to store pdw file
        /// </summary>
        /// <returns></returns>
        private string GetNewFileName()
        {
            System.Windows.Forms.SaveFileDialog customSave = new System.Windows.Forms.SaveFileDialog();
            customSave.Title = Properties.Resources.ipm_DialogSaveTitle;
            customSave.Filter = GetFilterString(true);

            if (customSave.ShowDialog() == System.Windows.Forms.DialogResult.OK) // click on Save button
                return customSave.FileName;

            return string.Empty;
        }

        /// <summary>
        /// Get filter list of word SaveFileDiaglog and update pdw extension
        /// </summary>
        /// <param name="isIncludeProntoDoc"></param>
        /// <returns></returns>
        private string GetFilterString(bool isIncludeProntoDoc)
        {
            string filterString = "";

            // get filter from office
            Office.FileDialog fileSave =
                Wkl.MainCtrl.CommonCtrl.CommonProfile.App.get_FileDialog(Office.MsoFileDialogType.msoFileDialogSaveAs);
            foreach (Office.FileDialogFilter filter in fileSave.Filters)
                filterString += string.Format(FormatString.FullFilterDialog, filter.Description, filter.Extensions);

            // add pronto document word  filter
            if (isIncludeProntoDoc)
                filterString += string.Format(FormatString.DetailFilterDialog,
                    Properties.Resources.ipm_DescriptionFile, FileExtension.ProntoExtension);
            else if (filterString.EndsWith("|"))
                filterString = filterString.Remove(filterString.Length - 1);

            return filterString;
        }

        /// <summary>
        /// Save current font size, domain name into user folder
        /// </summary>
        private void SaveUserData()
        {
            mainManager.MainService.PropertyService.SaveSelectedDomainToFile(
               Wkl.MainCtrl.CommonCtrl.CommonProfile.UserData);
        }
        #endregion

        #endregion

        #region 5. Change Font Size
        /// <summary>
        /// Change font-size of current document
        /// </summary>
        /// <param name="isSmallToBig"></param>
        /// <returns>true if using large font image in ribbon</returns>
        public bool ChangeFontSize(bool isSmallToBig)
        {
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            bool isSmallImage = isSmallToBig;
            int size = templateInfo.FontSize;
            if (size == 0)
                size = Pdw.Core.Constants.ContextManager.DefaultFontSize;

            if (isSmallToBig)//Small To Big
            {
                if (size >= Pdw.Core.Constants.ContextManager.MaxFontSize)
                    size = Pdw.Core.Constants.ContextManager.MaxFontSize;
                else
                    size += 1;
            }
            else//Big to small
            {
                if (size <= Pdw.Core.Constants.ContextManager.MinFontSize)
                    size = Pdw.Core.Constants.ContextManager.MinFontSize;
                else
                    size -= 1;
            }

            templateInfo.FontSize = size;
            templateInfo.ProntoDocMarkup.SetFontSize(size);

            return isSmallImage;
        }
        #endregion

        #region 6. intergrate with excel
        public void ImportPde(PdeContentItem pdeContentItem, Document wDoc)
        {
            mainManager.MainService.PropertyService.ImportPde(pdeContentItem, wDoc);
        }

        public void ImportPdeTable(Microsoft.Office.Interop.Excel.Range eRange, Document wDoc)
        {
            mainManager.MainService.PropertyService.ImportPdeTable(eRange, wDoc);
        }

        public void ImportPdeChart(Microsoft.Office.Interop.Excel.Chart eChart, Document wDoc)
        {
            mainManager.MainService.PropertyService.ImportPdeChart(eChart, wDoc);
        }

        public void CheckPdeContent(Document wDoc, PdeContent pdeContent)
        {
            mainManager.MainService.PropertyService.CheckPdeContent(wDoc, pdeContent);
        }

        public void CloseExcel()
        {
            mainManager.MainService.PropertyService.CloseExcel();
        }
        #endregion
    }
}
