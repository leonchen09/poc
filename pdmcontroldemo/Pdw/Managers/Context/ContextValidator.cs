
using System.Xml;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using System.Linq;
using Pdw.Core;
using Pdw.WKL.Profiler.Manager;
using Wkl = Pdw.WKL.DataController.MainController;
using WinForm = System.Windows.Forms;
using Pdw.FormControls;
using System;
using System.Reflection;

namespace Pdw.Managers.Context
{
    public class ContextValidator
    {
        private Document _doc;

        private Pdw.Core.InternalBookmark iBm
        {
            get
            {
                return Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(_doc.FullName).InternalBookmark;
            }
        }

        private PdeContent PdeContent
        {
            get
            {
                return Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(_doc.FullName).PdeContent;
            }
        }

        private IEnumerable<ControlBase> BindingControls
        {
            get
            {
                var vstoDoc = Globals.Factory.GetVstoObject(_doc);

                return vstoDoc.Controls.GetDataBoundItems();
            }
        }

        private IEnumerable<string> PdmBindingKeys
        {
            get
            {
                foreach (var control in BindingControls)
                {
                    string key = control.DataBindingKey;

                    if (!string.IsNullOrEmpty(key))
                    {
                        yield return key;
                    }

                    if (control is ISelectable)
                    {
                        key = ((ISelectable)control).DataBindingSelectKey;

                        if (!string.IsNullOrEmpty(key))
                        {
                            yield return key;
                        }
                    }
                }
            }
        }

        public ContextValidator(Document Doc)
        {
            _doc = Doc;
        }

        #region save document
        public void Save()
        {
            try
            {
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.IsAutoSave = false;
                _doc.Save();
            }
            catch { }
        }
        #endregion

        #region validate
        /// <summary>
        /// validate bookmark (internal and word)
        /// </summary>
        /// <param name="key"></param>
        public void ValidateBeforeSave(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            mgrPro.WbmKeys = new List<string>();

            if (_doc == null)
            {
                mgrPro.IsCorrect = true;
                return;
            }
            if (iBm == null)
            {
                mgrPro.IsCorrect = false;
                return;
            }

            bool hasDataTag = HasDataTag();
            if (!hasDataTag && mgrPro.TemplateType == ProntoDoc.Framework.CoreObject.PdwxObjects.TemplateType.Pdz)
            {
                if (System.Windows.Forms.MessageBox.Show("There are no data tags in this document. Do you want to continue process?",
                    "Confirm", System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    hasDataTag = true;
            }

            //HACK:FORM CONTROLS - ValidateBeforeSave
            bool isCorrectPdmBookmark = true;
            if (mgrPro.TemplateType == TemplateType.Pdm)
            {
                hasDataTag = true;
                isCorrectPdmBookmark = ValidatePdmBookmark();
            }

            bool isCorrectXml = ValidateData(mgrPro.WbmKeys, mgrPro.WdColorIndex);
            bool isCorrectBM = ValidateBookmark(mgrPro.WbmKeys, mgrPro.WdColorIndex);

            mgrPro.IsCorrect = (hasDataTag && isCorrectXml && isCorrectBM && isCorrectPdmBookmark);
        }

        #region make sure the document has data tag
        /// <summary>
        /// only acept save when the document has word bookmark (has data tag) and internal bookmark
        /// </summary>
        /// <returns></returns>
        private bool HasDataTag()
        {
            bool hasWbm = false;
            if (_doc != null && _doc.Bookmarks != null && _doc.Bookmarks.Count > 0)
                hasWbm = true;
            if (!hasWbm)
                return false;

            // check pdw tags
            if (iBm != null && iBm.InternalBookmarkDomains != null)
            {
                foreach (InternalBookmarkDomain ibmDomain in iBm.InternalBookmarkDomains)
                {
                    if (ibmDomain.InternalBookmarkItems.Count > 0)
                        return true;
                }
            }

            // check pde tags
            PdeContent pdeContent = PdeContent;
            if (pdeContent != null && pdeContent.Items != null)
            {
                foreach (PdeContentItem pdeContentItem in pdeContent.Items)
                {
                    if (pdeContentItem.ExportData != null && pdeContentItem.ExportData.Items != null)
                    {
                        foreach (DomainExportItem expDomain in pdeContentItem.ExportData.Items)
                        {
                            if (expDomain.Items != null && expDomain.Items.Exists(c => c.IsUsed))
                                return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region validate data
        /// <summary>
        /// validate bookmark position (open tag and close tag must be have same node level)
        /// </summary>
        /// <param name="Doc"></param>
        /// <returns></returns>
        private bool ValidateData(List<string> highlightedBmKeys, WdColorIndex color)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                string abc = _doc.WordOpenXML;

                xDoc.LoadXml(_doc.WordOpenXML);

                foreach (Bookmark bm in _doc.Bookmarks)
                {
                    string startBmName = bm.Name;
                    string endBmName = string.Empty;

                    if (startBmName.EndsWith(ProntoMarkup.KeyStartForeach))
                        endBmName = bm.Name.Replace(ProntoMarkup.KeyStartForeach, ProntoMarkup.KeyEndForeach);
                    else if (startBmName.EndsWith(ProntoMarkup.KeyStartIf))
                        endBmName = bm.Name.Replace(ProntoMarkup.KeyStartIf, ProntoMarkup.KeyEndIf);

                    if (!string.IsNullOrEmpty(endBmName))
                    {
                        if (!ValidateBookmark(xDoc, startBmName, endBmName))
                        {
                            highlightedBmKeys.Add(startBmName);
                            highlightedBmKeys.Add(endBmName);
                            HighLightBookmark(startBmName, color);
                            HighLightBookmark(endBmName, color);

                            return false;
                        }
                    }
                }

                return true;
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ValidateDataError);
                mgrExp.Errors.Add(baseExp);

                throw mgrExp;
            }
            catch (System.Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ValidateDataError,
                    MessageUtils.Expand(Properties.Resources.ipe_ValidateWordBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        #region helper methods
        private bool ValidateBookmark(XmlDocument xDoc, string startBmName, string endBmName)
        {
            XmlNodeList bmNodes = xDoc.GetElementsByTagName(XmlDocNodeName.StartBookmark);

            XmlNode startNode = null;
            XmlNode endNode = null;
            foreach (XmlNode node in bmNodes)
            {
                if (node.Attributes[NodeAttribute.Name].Value == startBmName)
                    startNode = node;
                if (node.Attributes[NodeAttribute.Name].Value == endBmName)
                    endNode = node;

                if (startNode != null && endNode != null)
                    break;
            }

            int startLevel = FindXmlNodeLevel(startNode);
            int endLevel = FindXmlNodeLevel(endNode);
            bool isSameLevel = (startLevel == endLevel);

            return isSameLevel;
        }

        private int FindXmlNodeLevel(XmlNode node)
        {
            int level = -1;
            XmlNode temp = node;
            while (temp != null)
            {
                level++;
                temp = temp.ParentNode;
            }

            return level;
        }
        #endregion

        /// <summary>
        /// highlight bookmark
        /// </summary>
        /// <param name="name"></param>
        private void HighLightBookmark(string name, WdColorIndex color)
        {
            Bookmark bm = GetBookmark(name);

            if (bm != null)
            {
                bm.Range.HighlightColorIndex = color;
                bm.Range.Font.ColorIndex = ProntoMarkup.ForeColorHighLight;
            }
        }

        /// <summary>
        /// get a bookmark object in word follow by bookmark's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Bookmark GetBookmark(string name)
        {
            if (_doc.Bookmarks.Exists(name))
                return _doc.Bookmarks[name];

            return null;
        }

        #endregion

        #region make sure internal bookmark and word bookmark are match
        /// <summary>
        /// make sure internal bookmark and word bookmark are match
        /// </summary>
        /// <returns></returns>
        private bool ValidateBookmark(List<string> highlightedBmKeys, WdColorIndex color)
        {
            try
            {
                // update word bm
                List<string> removedWord = ValidateWordBookmark(color);
                highlightedBmKeys.AddRange(removedWord);

                // update internal bookmark (remove bm exist in internal but not exist in word)
                ValidateInternalBookmark();

                return (removedWord.Count == 0);
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ValidateBookmarkError);
                mgrExp.Errors.Add(baseExp);

                throw mgrExp;
            }
            catch (System.Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ValidateBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_ValidateWordBookmarkWithDomainError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// validate word bookmark (if exist in word but not exit in internal => highlight and markup to return)
        /// </summary>
        /// <param name="iBms"></param>
        /// <returns></returns>
        private List<string> ValidateWordBookmark(WdColorIndex color)
        {
            List<string> deletedList = new List<string>();

            foreach (Bookmark wBm in _doc.Bookmarks)
            {
                InternalBookmarkItem item = iBm.GetInternalBookmarkItem(wBm.Name);

                // 1. check null (can not find ibm)
                if (item == null)
                {
                    // todo: ngocbv_rem => confirm to remove export item
                    //if (!MarkupUtilities.IsProntoDocCommentBookmark(wBm.Name))
                    //{
                    //    deletedList.Add(wBm.Name);
                    //    HighLightBookmark(wBm.Name, color);
                    //}
                    continue;
                }

                // 2. get real biz name
                string bizName = item.IsImage() ? MarkupUtilities.GetBizNameOfBookmarkImage(wBm.Name, _doc.InlineShapes) : wBm.Range.Text;

                if (item.BizName != bizName) // compare key and value
                {
                    deletedList.Add(wBm.Name);
                    HighLightBookmark(wBm.Name, color);
                }
                else if (item != null && item.Type == XsltType.Select) // check match with domain (only check for field)
                {
                    DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(item.DomainName);

                    if (domainInfo != null && !domainInfo.Fields.ContainsKey(MarkupUtilities.GetOriginalBizName(wBm.Name, item.BizName)))
                    {
                        deletedList.Add(wBm.Name);
                        HighLightBookmark(wBm.Name, color);
                    }
                }
            }

            return deletedList;
        }

        /// <summary>
        /// validate internal bookmark (if exist in internal but not exist in word => remove)
        /// </summary>
        private void ValidateInternalBookmark()
        {
            List<string> removed = new List<string>();

            foreach (InternalBookmarkDomain ibmDomain in iBm.InternalBookmarkDomains)
            {
                foreach (InternalBookmarkItem item in ibmDomain.InternalBookmarkItems)
                {
                    if (!_doc.Bookmarks.Exists(item.Key))
                    {
                        //HACK:FORM CONTROLS - ValidateInternalBookmark
                        //current internal bookmark is used for pdm control's data binding process.
                        if (PdmBindingKeys.Any(key => key == item.Key))
                        {
                            continue;
                        }

                        removed.Add(item.Key);
                    }
                }
            }

            foreach (string bmKey in removed)
                iBm.RemoveInternalBookmarkItem(bmKey);
        }
        #endregion

        //HACK:FORM CONTROLS - ValidatePdmBookmark
        private bool ValidatePdmBookmark()
        {
            var result = true;
            var internalBookmarks = iBm.InternalBookmarkDomains.SelectMany(d => d.InternalBookmarkItems);

            if (BindingControls.Count() != 0)
            {
                result = BindingControls.All(bc => DoesControlMatchWithBookmark(bc, internalBookmarks));
            }

            return result;
        }

        private bool DoesControlMatchWithBookmark(Pdw.FormControls.ControlBase control, IEnumerable<InternalBookmarkItem> bookmarks)
        {
            bool match = true;

            string key = control.DataBindingKey;

            if (string.IsNullOrEmpty(key) || !bookmarks.Any(b => b.Key == key))
            {
                match = false;
            }


            if (match && control is ISelectable)
            {
                key = ((ISelectable)control).DataBindingSelectKey;

                if (!string.IsNullOrEmpty(key) && !bookmarks.Any(b => b.Key == key))
                {
                    match = false;
                }
            }

            return match;
        }

        #endregion

        public void RemoveBookmark(string key)
        {
            try
            {
                ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);

                foreach (string bmKey in mgrPro.WbmKeys)
                {
                    if (_doc.Bookmarks.Exists(bmKey))
                    {
                        _doc.Bookmarks[bmKey].Range.HighlightColorIndex = ProntoMarkup.BackgroundUnHighLight;
                        _doc.Bookmarks[bmKey].Delete();
                    }

                    iBm.RemoveInternalBookmarkItem(bmKey);
                }
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_RemoveBookmarkError);
                mgrExp.Errors.Add(baseExp);

                throw mgrExp;
            }
            catch (System.Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_RemoveBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_DeleteBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }
    }
}
