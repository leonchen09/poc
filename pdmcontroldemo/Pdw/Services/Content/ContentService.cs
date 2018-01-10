
using System;
using System.Collections.Generic;

using Microsoft.Office.Interop.Word;

using Pdw.Core;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Services.Content
{
    /// <summary>
    /// Using for bookmark biz
    /// </summary>
    public class ContentService
    {
        #region Event
        // bookmarks
        public event DeleteBookmarkEventHandler DeleteBookmark;
        public event AddBookmarkImageEventHandler AddBookmarkImageEvent;
        public event HighlightBookmarkEventHandler HighlightBookmark;
        public event GetInternalBookmarkEventHandler GetInternalBookmark;
        public event GetForeachTagsBoundCurrentPosEventHandler GetForeachTagsBoundCurrentPosEvent;

        // helpers
        public event MarkProntoDocEventHandler MarkProntoDoc;
        public event AddTextToCurrentSelectionEventHandler AddTextToCurrentSelection;
        public event SetFontForCurrentSelectionEventHandler SetFontForCurrentSelection;
        public event AddBookmarkEventHandler AddBookmarkInCurrentSelection;
        public event MoveCharactersEventHandler MoveChars;
        public event DIGetCurrentSelectionEventHandler GetCurrentSelection;
        public event DIProtectBookmarkEventHandler ProtectBookmark;
        public event DIUnProtectBookmarkEventHandler UnProtectBookmark;
        public event DIProtectWordEventHandler ProtectWord;
        public event DIUnProtectWordEventHandler UnProtectWord;

        //ProntoDocMarkup
        public event EnableComboboxDomainEventHandler EnableComboboxDomain;
        #endregion

        #region add bookmark
        /// <summary>
        /// Add a bookmark into current document
        /// </summary>
        /// <param name="name">Name of bookmark</param>
        /// <param name="value">Value of bookmark</param>
        /// <param name="xsltType">XsltType (Select, Foreach or If)</param>
        /// <returns></returns>
        public bool AddBookmark(string key)
        {
            try
            {
                ServicesProfile serviceProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
                InternalBookmarkItem bm = serviceProfile.ContentService.AddBookmark_IBookmark;
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.IsAdding = true;

                serviceProfile.ContentService.MarkProntDoc = true;
                switch (bm.Type)
                {
                    case XsltType.Foreach:
                        AddDoubleBookmark(bm);
                        MarkProntoDoc(key);
                        break;
                    case XsltType.If:
                        AddDoubleBookmark(bm);
                        MarkProntoDoc(key);
                        break;
                    case XsltType.Select:
                        if (bm.IsImage())
                        {
                            serviceProfile.WbmKey = bm.Key + ProntoMarkup.KeyImage;
                            serviceProfile.WbmValue = MarkupUtilities.GenTextXslTag(bm.BizName, bm.Type, true);
                            serviceProfile.AlternativeText = MarkupUtilities.CreateAlternativeText(serviceProfile.WbmKey,
                                serviceProfile.WbmValue);
                            AddBookmarkImageEvent(key);
                        }
                        else
                            AddSingleBookmark(bm);
                        MarkProntoDoc(key);
                        break;
                    case XsltType.Comment:
                        AddCommentBookmark(bm);
                        break;
                    default:
                        break;
                }

                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.IsAdding = false;
                return true;

            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_AddBookmarkError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_AddBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_AddBookmarkError, ex.Message), ex.StackTrace);

                throw srvExp;
            }
        }

        /// <summary>
        /// Add select bookmark
        /// </summary>
        /// <param name="name">Name of bookmark</param>
        /// <param name="value">Value of bookmark</param>
        private void AddSingleBookmark(InternalBookmarkItem bm)
        {
            string key;
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);

            //Add a space before text.
            servicePro.ContentService.AddToSelection_Text = " ";
            servicePro.ContentService.AddToSelection_IsSelected = false;
            AddTextToCurrentSelection(key);

            // add select text
            servicePro.ContentService.AddToSelection_Text = MarkupUtilities.GenTextXslTag(bm.BizName, bm.Type, true);
            servicePro.ContentService.AddToSelection_IsSelected = true;
            AddTextToCurrentSelection(key);

            // add select bookmark
            servicePro.ContentService.AddBookmark_Name = MarkupUtilities.GenKeyForXslTag(bm.Key, bm.Type, true);
            AddBookmarkInCurrentSelection(key);

            // set cursor after bookmark
            servicePro.ContentService.MoveChars_Count = 1;
            servicePro.ContentService.MoveChars_IsLeft = false;
            MoveChars(key);

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        /// <summary>
        /// add a comment
        /// </summary>
        /// <pparam name="bm"></pparam>
        private void AddCommentBookmark(InternalBookmarkItem bm)
        {
            string key;
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);

            // add a space before comment
            servicePro.ContentService.AddToSelection_Text = " ";
            servicePro.ContentService.AddToSelection_IsSelected = false;
            AddTextToCurrentSelection(key);

            // change font color to Red
            servicePro.ContentService.WdColorIndex = WdColorIndex.wdRed;
            SetFontForCurrentSelection(key);

            // add select text
            string commentText = MarkupUtilities.GenTextXslTag(bm.BizName, bm.Type, true);
            servicePro.ContentService.AddToSelection_Text = commentText;
            servicePro.ContentService.AddToSelection_IsSelected = false;
            AddTextToCurrentSelection(key);

            // change font color to normal
            servicePro.ContentService.WdColorIndex = WdColorIndex.wdBlack;
            SetFontForCurrentSelection(key);

            // add a space after comment
            servicePro.ContentService.AddToSelection_Text = " ";
            servicePro.ContentService.AddToSelection_IsSelected = false;
            AddTextToCurrentSelection(key);

            // move left a character to back to comment
            servicePro.ContentService.MoveChars_Count = 1;
            servicePro.ContentService.MoveChars_IsLeft = true;
            servicePro.ContentService.MoveChars_IsExtend = false;
            MoveChars(key);

            // select comment
            servicePro.ContentService.MoveChars_Count = commentText.Length;
            servicePro.ContentService.MoveChars_IsLeft = true;
            servicePro.ContentService.MoveChars_IsExtend = true;
            MoveChars(key);

            // add select bookmark
            servicePro.ContentService.AddBookmark_Name = MarkupUtilities.GenKeyForXslTag(bm.Key, bm.Type, true);
            AddBookmarkInCurrentSelection(key);

            // move to end of comment
            servicePro.ContentService.MoveChars_Count = 1;
            servicePro.ContentService.MoveChars_IsLeft = false;
            servicePro.ContentService.MoveChars_IsExtend = false;
            MoveChars(key);

            // move to comment text position
            servicePro.ContentService.MoveChars_Count = ProntoMarkup.ValueCommentTextPosition;
            servicePro.ContentService.MoveChars_IsLeft = true;
            servicePro.ContentService.MoveChars_IsExtend = false;
            MoveChars(key);

            // change font color to normal
            servicePro.ContentService.WdColorIndex = WdColorIndex.wdBlack;
            SetFontForCurrentSelection(key);

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        /// <summary>
        /// Add foreach bookmark
        /// </summary>
        /// <param name="name">Name of bookmark</param>
        /// <param name="value">Value of bookmark</param>
        private void AddDoubleBookmark(InternalBookmarkItem bm)
        {
            //1. Get FontColor of BM
            //2. Generate tag
            //3. Add start tag (text) to document
            //4. Add start tag to BM
            //5. Move cursor to after
            //6. Add end tag (text) to document
            //7. Add end tag to BM
            //8. Move cursor to between two tags
            //9. Set color

            string key;
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);

            WdColor fontColor = GetFontColor(bm.Type);
            WdColor textColor = WdColor.wdColorBlack;
            int textLength = 0;

            if (Globals.ThisAddIn.Application.Selection.Range.Start != Globals.ThisAddIn.Application.Selection.Range.End)
            {
                textLength = Globals.ThisAddIn.Application.Selection.Range.Text.Length;
                textColor = Globals.ThisAddIn.Application.Selection.Range.Font.Color;
                servicePro.ContentService.MoveChars_Count = 1;
                servicePro.ContentService.MoveChars_IsLeft = true;
                MoveChars(key);
            }

            //2.Generate tag
            string startTag = MarkupUtilities.GenTextXslTag(bm.BizName, bm.Type, true);

            //Add a space before text
            servicePro.ContentService.AddToSelection_Text = " ";
            servicePro.ContentService.AddToSelection_IsSelected = false;
            AddTextToCurrentSelection(key);

            //3. add start foreach text
            servicePro.ContentService.AddToSelection_Text = startTag.Trim();
            servicePro.ContentService.AddToSelection_IsSelected = true;
            AddTextToCurrentSelection(key);

            //4. add start foreach bookmark
            servicePro.ContentService.AddBookmark_Name = MarkupUtilities.GenKeyForXslTag(bm.Key, bm.Type, true);
            AddBookmarkInCurrentSelection(key);
            servicePro.ContentService.AddBookmark_BookmarkReturn.Range.Font.Color = fontColor;
            //if (bm.Type == XsltType.If) // todo: ngocbv add highlight color for condition tag
            //    servicePro.ContentService.AddBookmark_BookmarkReturn.Range.HighlightColorIndex = ProntoMarkup.IfColorBackground;

            //5. add space
            servicePro.ContentService.MoveChars_Count = (textLength > 0 ? textLength + 1 : 1);
            servicePro.ContentService.MoveChars_IsLeft = false;
            MoveChars(key);

            servicePro.ContentService.AddToSelection_Text = " ";
            servicePro.ContentService.AddToSelection_IsSelected = false;
            AddTextToCurrentSelection(key);

            Globals.ThisAddIn.Application.Selection.Font.Color = textColor;

            //6. add end foreach text
            servicePro.ContentService.AddToSelection_Text = MarkupUtilities.GenTextXslTag(bm.BizName, bm.Type, false);
            servicePro.ContentService.AddToSelection_IsSelected = true;
            AddTextToCurrentSelection(key);

            //7. add end foreach bookmarl
            servicePro.ContentService.AddBookmark_Name = MarkupUtilities.GenKeyForXslTag(bm.Key, bm.Type, false);
            AddBookmarkInCurrentSelection(key);
            servicePro.ContentService.AddBookmark_BookmarkReturn.Range.Font.Color = fontColor;
            //if (bm.Type == XsltType.If) // todo: ngocbv add highlight color for condition tag
            //    servicePro.ContentService.AddBookmark_BookmarkReturn.Range.HighlightColorIndex = ProntoMarkup.IfColorBackground;

            //8.set cursor before end tag
            servicePro.ContentService.MoveChars_Count = 2;
            servicePro.ContentService.MoveChars_IsLeft = true;

            MoveChars(key);

            //9. reset color
            Globals.ThisAddIn.Application.Selection.Font.Color = textColor;

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        /// <summary>
        /// Get font color for xsl tag
        /// </summary>
        /// <param name="xslType"></param>
        /// <returns></returns>
        private WdColor GetFontColor(XsltType xslType)
        {
            switch (xslType)
            {
                case XsltType.Foreach:
                    return ProntoMarkup.ForeachColor;
                case XsltType.If:
                    return ProntoMarkup.IfColor;
                default:
                    return ProntoMarkup.SelectColor;
            }
        }
        #endregion

        #region validate bookmark collection
        /// <summary>
        /// Remove all word bookmark that not exist on internal bookmark collection
        /// </summary>
        /// <param name="iBms"></param>
        public void ValidateBookmarkCollection(string key)
        {
            // 1. Get all word bookmark
            // 2. If bookmark in word not exist in internal bookmark
            // 2.1. markup this to return
            // 2.2. if isUpdate = true, we delete its
            try
            {
                ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).ContentService;
                InternalBookmark iBms = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(
                    contentProfile.ValidateBookmark_ITemplateName).InternalBookmark;

                GetBookmarkCollection(key);

                List<string> deletedList = new List<string>();
                Dictionary<string, string> wBms = contentProfile.GetBookmarks_OListBM;

                foreach (string bmKey in wBms.Keys)
                {
                    InternalBookmarkItem item = iBms.GetInternalBookmarkItem(bmKey);

                    if (item == null || item.BizName != wBms[bmKey]) // compare key and value
                    {
                        if (MarkupUtilities.IsProntoDocCommentBookmark(bmKey) ||
                            MarkupUtilities.IsPdeTagBookmark(bmKey))
                            continue;

                        deletedList.Add(wBms[bmKey]);
                        contentProfile.HighlightBookmarkName = bmKey;
                        HighlightBookmark(key);

                        if (contentProfile.ValidateBookmark_IIsUpdate)
                        {
                            contentProfile.DeletedBookmarkName = bmKey;
                            contentProfile.DeleteWholeBookmark = false;
                            DeleteBookmark(key);
                        }
                    }
                }

                contentProfile.ValidateBookmark_ORemovedList = deletedList;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_ValidateWordBookmarkError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_ValidateWordBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_ValidateWordBookmarkError, ex.Message), ex.StackTrace);

                throw srvExp;
            }
        }

        /// <summary>
        /// Highlight all word bookmark that not exist on data domain
        /// </summary>
        /// <param name="iBms"></param>
        public void ValidateBookmarkCollectionWithDomain(string key)
        {
            try
            {
                bool isHighlight = false;
                List<string> unMatched = new List<string>();

                //Get Bookmark Collection then put to WKL
                GetBookmarkCollection(key);

                ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).ContentService;
                Dictionary<string, string> wBm = contentProfile.GetBookmarks_OListBM;

                //Get Internal Bookmark
                TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(
                    contentProfile.ValidateBookmark_ITemplateName);
                InternalBookmark interBm = templateInfo.InternalBookmark;

                //Get Domain Data
                foreach (string domainName in templateInfo.DomainNames)
                {
                    DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName);
                    if (domainInfo != null)
                    {
                        InternalBookmarkDomain ibmDomain = interBm.GetInternalBookmarkDomain(domainName);
                        foreach (InternalBookmarkItem item in ibmDomain.InternalBookmarkItems)
                        {
                            if (!MarkupUtilities.IsExistOnDomain(item, domainInfo.Fields, true))
                            {
                                if (wBm.ContainsKey(item.Key) && wBm[item.Key] == item.BizName)
                                {
                                    contentProfile.HighlightBookmarkName = item.Key;
                                    unMatched.Add(item.BizName);
                                    HighlightBookmark(key);
                                    isHighlight = true;
                                }
                            }
                        }
                    }
                    else
                        isHighlight = true;
                }

                contentProfile.UnMatchedFields = unMatched;
                contentProfile.Result = !isHighlight;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_ValidateWordBookmarkWithDomainError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_ValidateWordBookmarkWithDomainError,
                    MessageUtils.Expand(Properties.Resources.ipe_ValidateWordBookmarkWithDomainError, ex.Message), ex.StackTrace);

                throw srvExp;
            }
        }
        #endregion

        #region get and highlight bookmark
        /// <summary>
        /// Get bookmark collection in word
        /// </summary>
        /// <returns>Dictionary with key is bookmark name and value is bookmark text</returns>
        public void GetBookmarkCollection(string key)
        {
            try
            {
                ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).ContentService;

                Dictionary<string, string> bookmarks = new Dictionary<string, string>();

                Bookmarks bms = Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks;

                foreach (Bookmark bookmark in bms)
                {
                    if (bookmark.Name.Contains(ProntoMarkup.KeyImage))
                        bookmarks.Add(bookmark.Name, MarkupUtilities.GetBizNameOfBookmarkImage(bookmark.Name,
                            Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.InlineShapes));
                    else
                        bookmarks.Add(bookmark.Name, MarkupUtilities.GetRangeText(bookmark.Range));
                }

                contentProfile.GetBookmarks_OListBM = bookmarks;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_GetWordBookmarksError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_GetWordBookmarksError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetWordBookmarksError, ex.Message), ex.StackTrace);

                throw srvExp;
            }
        }

        public void HighlightWordBookmark(string key)
        {
            try
            {
                ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).ContentService;
                string keyService;
                ServicesProfile serviceProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out keyService);
                if (!string.IsNullOrEmpty(contentProfile.HighlightBookmark_IBMName))
                {
                    HighlightBookmark(keyService);
                }
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_HighlightBookmarkError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_HighlightBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_HighlightBookmarkError, ex.Message, ex.StackTrace), ex.StackTrace);

                throw srvExp;
            }
        }
        #endregion

        #region Word event
        public void ProtectDocument()
        {
            ProtectWord();
        }
        public void UnProtectDocument()
        {
            UnProtectWord();
        }
        #endregion

        #region GoTo
        /// <summary>
        /// Go to the first bookmark in the document.
        /// </summary>
        /// <param name="item"></param>
        public void GoToFirstBookmark(BookmarkItem item)
        {
            GetFirstBookmark(item).Range.Select();
        }

        /// <summary>
        /// Go to next bookmark from the cursor.
        /// </summary>
        /// <param name="item"></param>
        public void GoNextBookmark(BookmarkItem item)
        {
            GetNextBookmark(item).Range.Select();
        }

        /// <summary>
        /// Go to previous bookmark from the cursor.
        /// </summary>
        /// <param name="item"></param>
        public void GoPreviousBookmark(BookmarkItem item)
        {
            Bookmark bm = GetPreviousBookmark(item);
            if (bm != null)
                bm.Range.Select();
        }

        /// <summary>
        /// Go to last bookmark.
        /// </summary>
        /// <param name="item"></param>
        public void GoLastBookmark(BookmarkItem item)
        {
            GetLastBookmark(item).Range.Select();
        }
        #endregion

        #region HighLight

        public void HighLightOneBookmark(BookmarkItem item)
        {
            HighlightWithPair(GetFirstBookmark(item));
        }

        public void HighLightFirstBookmark(BookmarkItem item)
        {
            HighlightWithPair(GetFirstBookmark(item));
        }

        public void HighLightLastBookmark(BookmarkItem item)
        {
            HighlightWithPair(GetLastBookmark(item));
        }

        public void HighLightNextBookmark(BookmarkItem item)
        {
            HighlightWithPair(GetNextBookmark(item));
        }

        public void HighLightPreviousBookmark(BookmarkItem item)
        {
            HighlightWithPair(GetPreviousBookmark(item));
        }

        public void HighLightAllBookmark(BookmarkItem item)
        {
            foreach (Bookmark bm in item.Items)
            {
                HighlightWithPair(bm);
            }
        }

        #endregion

        /// <summary>
        /// Get bookmark collection in word
        /// </summary>
        /// <returns>Dictionary with key is bookmark name and value is bookmark text</returns>
        public void GetDistinctBookmarks(string key)
        {
            //1.Get All Bookmarks
            //2.Do not add EndIf, EndForEach bookmark
            //3.If a bookmark is existed in List, increase number entry
            //4.Else insert to list.
            try
            {
                ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).ContentService;
                List<BookmarkItem> listBookmarks = new List<BookmarkItem>();
                Bookmarks bms = Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks;

                for (int j = 1; j <= bms.Count; j++)
                {
                    Bookmark bookmark = bms[j];

                    //2.Do not add EndIf, EndForEach bookmark
                    if (!MarkupUtilities.GetRangeText(bookmark.Range).Contains(Constants.BookMarkControl.EndIfTag))
                    {
                        BookmarkItem item = new BookmarkItem(bookmark.Name, string.Empty, string.Empty, bookmark);
                        if (bookmark.Name.Contains("Image"))
                        {
                            GetInternalBookmark(key);
                            InternalBookmark internalBM = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).Ibm;
                            foreach (InternalBookmarkDomain ibmDomain in internalBM.InternalBookmarkDomains)
                            {
                                foreach (InternalBookmarkItem internalItem in ibmDomain.InternalBookmarkItems)
                                {
                                    if (internalItem.Key.CompareTo(bookmark.Name) == 0)
                                    {
                                        item.Value = internalItem.BizName;
                                        item.DisplayName = SplitValue(internalItem.BizName);
                                    }
                                }
                            }
                        }
                        else
                        {
                            item.Value = MarkupUtilities.GetRangeText(bookmark.Range);
                            item.DisplayName = SplitValue(item.Value);
                        }

                        if (listBookmarks.Count == 0)
                            listBookmarks.Add(item);
                        else
                        {
                            int n = -1;
                            bool existed = false;
                            for (int i = 0; i < listBookmarks.Count; i++)
                            {
                                n += 1;
                                if (listBookmarks[i].Value.Equals(item.Value))
                                {
                                    existed = true;
                                    break;
                                }
                            }

                            //3.
                            if (existed)
                            {
                                listBookmarks[n].NumberEntry += 1;
                                listBookmarks[n].Items.Add(bookmark);
                            }
                            else//4.
                                listBookmarks.Add(item);
                        }
                    }
                }
                contentProfile.GetDistinctBM_OListBM = listBookmarks;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_GetDistinctBookmarkError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_GetDistinctBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetDistinctBookmarkError, ex.Message), ex.StackTrace);

                throw srvExp;
            }
        }

        #region Delete

        /// <summary>
        /// Delete Current Bookmark.
        /// </summary>
        /// <param name="bmItem"></param>
        /// <returns></returns>
        public bool DeleteOneBookmark(BookmarkItem bmItem)
        {
            //1.If selected text is bookmark, delete current bookmark
            //2.else: delete first bookmark
            //3. If bookmark is data tag, delete one bookmark only
            //4. If bookmark is a pair of bookmark, delete both.

            try
            {
                string srvKey = string.Empty;
                ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
                GetCurrentSelection(srvKey);
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);
                Selection selectedText = srvPro.ContentService.WSelection;
                Bookmark deletedBM;

                if (selectedText.Text.Trim().Equals(bmItem.Value) && (selectedText.Bookmarks.Count > 0))
                    deletedBM = selectedText.Bookmarks[1];
                else
                    deletedBM = GetFirstBookmark(bmItem);

                bmItem.Items.Remove(deletedBM);
                DeleteBookmarkWithPair(deletedBM);

                return true;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_DeleteBookmarkError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_DeleteBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_DeleteBookmarkError, ex.Message), ex.StackTrace);

                return false;
                throw srvExp;
            }
        }

        /// <summary>
        /// Delete all bookmarks with same value
        /// </summary>
        /// <param name="bmItem"></param>
        /// <returns></returns>
        public bool DeleteAllBookmarks(BookmarkItem bmItem)
        {
            try
            {
                for (int i = 0; i < bmItem.Items.Count; i++)
                {
                    DeleteBookmarkWithPair(bmItem.Items[i]);
                }
                return true;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_DeleteBookmarkError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_DeleteBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_DeleteBookmarkError, ex.Message), ex.StackTrace);
                return false;
                throw srvExp;
            }
        }
        #endregion

        #region User Specificed Condition
        public bool ValidateFieldsInCondition(List<USCItem> listCondition, Dictionary<string, USCItem> listItems)
        {
            if (listCondition == null)
                return true;
            else
                if (listItems == null)
                    return false;

            foreach (USCItem item in listCondition)
                if (item.Fields != null && item.Fields.Count > 0)
                {
                    foreach (USCItem subField in item.Fields)
                    {
                        if (listItems.ContainsKey(subField.BusinessName.ToLower()))
                        {
                            USCItem field = listItems[subField.BusinessName.ToLower()];
                            subField.BusinessName = field.BusinessName;
                            subField.DataType = field.DataType;
                            subField.Type = field.Type;
                            item.IsValid = true;
                        }
                        else
                        {
                            item.IsValid = false;
                            item.ErrorMessage += MessageUtils.Expand(Properties.Resources.ipm_M005, Properties.Resources.ipe_M005, subField.BusinessName);
                        }
                    }
                }

            return true;
        }
        #endregion

        public void EnableDomain()
        {
            string key;
            ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key).ContentService;

            GetDistinctBookmarks(key);

            if (contentProfile.GetDistinctBM_OListBM.Count <= 0)
                EnableComboboxDomain();

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        public void ValidateDataTagPosition(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            GetForeachTagsBoundCurrentPosEvent(key);

            srvPro.ContentService.IsValid = true;
            InternalBookmark ibm = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.InternalBookmark;
            //DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(srvPro.ContentService.DomainName);
            //Dictionary<int, int> tableIndexes = new Dictionary<int, int>();
            foreach (string wbmName in srvPro.ContentService.DicBookmarks.Keys)
            {
                string startName = string.Empty;
                string endName = string.Empty;

                if (wbmName.EndsWith(ProntoMarkup.KeyStartForeach))
                {
                    startName = wbmName;
                    endName = wbmName.Replace(ProntoMarkup.KeyStartForeach, ProntoMarkup.KeyEndForeach);
                }
                else if (wbmName.EndsWith(ProntoMarkup.KeyEndForeach))
                {
                    endName = wbmName;
                    startName = wbmName.Replace(ProntoMarkup.KeyEndForeach, ProntoMarkup.KeyStartForeach);
                }
                //else
                //{
                //    InternalBookmarkItem ibmItem = ibm.GetInternalBookmarkItem(wbmName);
                //    if (ibmItem != null)
                //    {
                //        int tableIndex = Math.Max(0, ibmItem.TableIndex);
                //        if (!tableIndexes.ContainsKey(tableIndex))
                //            tableIndexes.Add(tableIndex, tableIndex);
                //    }
                //}

                #region check: all data tags can  inside only Domain
                if (srvPro.ContentService.DicBookmarks.ContainsKey(startName) && srvPro.ContentService.DicBookmarks.ContainsKey(endName))
                {
                    InternalBookmarkDomain ibmDomain = ibm.GetInternalBookmarkDomainByItemKey(startName);
                    if (ibmDomain.DomainName != srvPro.ContentService.DomainName)
                    {
                        srvPro.ContentService.IsValid = false;
                        srvPro.Message = "Only allow data tag of [" + ibmDomain.DomainName + "] in this foreach tag.";
                        return;
                    }

                    #region get order by
                    //Dictionary<string, OrderByType> sorteds = MarkupUtilities.GetOldOrderBy(srvPro.ContentService.DicBookmarks[startName].Range.Text, false);
                    //if (sorteds.Count > 0)
                    //{
                    //    foreach (string bizName in sorteds.Keys)
                    //    {
                    //        string originalBizName = domainInfo.GetUdfSortedBizName(bizName);
                    //        ProntoDoc.Framework.CoreObject.DataSegment.DSTreeView field = domainInfo.GetField(originalBizName);
                    //        if (field != null)
                    //        {
                    //            int tableIndex = field.TableIndex;
                    //            tableIndex = Math.Max(0, tableIndex);
                    //            if (!tableIndexes.ContainsKey(tableIndex))
                    //                tableIndexes.Add(tableIndex, tableIndex);
                    //        }
                    //    }
                    //}
                    #endregion
                }
                #endregion
            }

            #region check: all data tags can  inside only Domain
            //int pathCount = 0;
            //if (tableIndexes.Count > 0)
            //{
            //    foreach (var dsRelationRow in domainInfo.DSDomainData.TableRelations.Rows.Items)
            //    {
            //        if ("WhereClause".Equals(dsRelationRow.Name, StringComparison.OrdinalIgnoreCase))
            //            continue;

            //        string relations = ProntoDoc.Framework.Utils.BitHelper.ToBinaryFormat(dsRelationRow.Data, true);
            //        relations = BaseMarkupUtilities.ReverseString(relations);
            //    }
            //}
            #endregion
        }

        #region Private Functions

        /// <summary>
        /// Get first bookmark in bookmark collection at the same value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Bookmark GetFirstBookmark(BookmarkItem bmItem)
        {
            int min = int.MaxValue, index = 0;
            for (int i = 0; i < bmItem.Items.Count; i++)
                if (min > bmItem.Items[i].Start)
                {
                    min = bmItem.Items[i].Start;
                    index = i;
                }

            return bmItem.Items[index];
        }

        /// <summary>
        /// Get last bookmark in bookmark collection at the same value
        /// </summary>
        /// <param name="bmItem"></param>
        /// <returns></returns>
        private Bookmark GetLastBookmark(BookmarkItem bmItem)
        {
            int max = -1, index = 0;
            for (int i = 1; i < bmItem.Items.Count; i++)
                if (max < bmItem.Items[i].Start)
                {
                    max = bmItem.Items[i].Start;
                    index = i;
                }

            return bmItem.Items[index];
        }

        /// <summary>
        /// Get Next Bookmark from the cursor.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Bookmark GetNextBookmark(BookmarkItem item)
        {
            string srvKey = string.Empty;
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
            GetCurrentSelection(srvKey);
            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);
            int cursorPosition = srvPro.ContentService.WSelection.Start;
            int min = int.MaxValue;
            int index = 0;

            if ((cursorPosition >= GetLastBookmark(item).Start))
                return GetFirstBookmark(item);

            for (int i = 0; i < item.Items.Count; i++)
            {
                if (item.Items[i].Start > cursorPosition)
                {
                    if (item.Items[i].Start < min)
                    {
                        min = item.Items[i].Start;
                        index = i;
                    }
                }
            }

            return item.Items[index];
        }

        /// <summary>
        /// Get Previous from the cursor.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Bookmark GetPreviousBookmark(BookmarkItem item)
        {
            string srvKey = string.Empty;
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
            GetCurrentSelection(srvKey);
            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);
            int cursorPosition = srvPro.ContentService.WSelection.Start;
            int max = -1;
            int index = -1;
            for (int i = 0; i < item.Items.Count; i++)
            {
                if (item.Items[i].Start < cursorPosition && item.Items[i].Start > max)
                {
                    max = item.Items[i].Start;
                    index = i;
                }
            }

            if (index > -1)
                return item.Items[index];
            else
                return null;
        }

        private string SplitValue(string value)
        {
            if (value.Length < 2)
                return string.Empty;
            if (value.Substring(value.Length - 2, 2).Contains(Constants.BookMarkControl.EndDataTag))
                return value.Substring(1, value.Length - 1).Substring(0, value.Length - 3);
            else
                return value.Substring(1, value.Length - 1).Substring(0, value.Length - 2);
        }

        private void HighlightWithPair(Bookmark bm)
        {
            Highlight(bm);

            string name = bm.Name;
            if (name.Contains(ProntoMarkup.KeyStartForeach) || name.Contains(ProntoMarkup.KeyStartIf))
            {
                name = name.Replace(ProntoMarkup.KeyStartIf, ProntoMarkup.KeyEndIf);
                name = name.Replace(ProntoMarkup.KeyStartForeach, ProntoMarkup.KeyEndForeach);

                Bookmarks bms = Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks;

                for (int j = 1; j <= bms.Count; j++)
                {
                    Bookmark bookmark = bms[j];
                    if (bookmark.Name.Equals(name))
                        Highlight(bookmark);
                }
            }
        }

        /// <summary>
        /// Highlight a bookmark
        /// </summary>
        /// <param name="bm"></param>
        private void Highlight(Bookmark bm)
        {
            bm.Range.Select();

            UnProtectBookmark();

            bm.Range.HighlightColorIndex = ProntoMarkup.BackgroundHighLight;
            bm.Range.Font.ColorIndex = ProntoMarkup.ForeColorHighLight;

            string key;
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);
            servicePro.ContentService.MoveChars_IsLeft = false;
            servicePro.ContentService.MoveChars_Count = 1;

            MoveChars(key);
            ProtectBookmark();

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        /// <summary>
        /// Delete Pair of bookmark (Foreach, If)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool DeleteBookmarkWithPair(Bookmark bookmark)
        {
            string name = bookmark.Name;
            string key;
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);
            servicePro.ContentService.DeletedBookmarkName = bookmark.Name;
            servicePro.ContentService.DeleteWholeBookmark = true;
            DeleteBookmark(key);

            //Delete internal bookmark
            Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.InternalBookmark.RemoveInternalBookmarkItem(name);

            if (name.Contains(ProntoMarkup.KeyStartForeach) || name.Contains(ProntoMarkup.KeyStartIf))
            {
                name = name.Replace(ProntoMarkup.KeyStartIf, ProntoMarkup.KeyEndIf);
                name = name.Replace(ProntoMarkup.KeyStartForeach, ProntoMarkup.KeyEndForeach);
                servicePro.ContentService.DeleteWholeBookmark = true;
                servicePro.ContentService.DeletedBookmarkName = name;
                DeleteBookmark(key);
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.InternalBookmark.RemoveInternalBookmarkItem(name);
            }

            return false;
        }
        #endregion
    }
}
