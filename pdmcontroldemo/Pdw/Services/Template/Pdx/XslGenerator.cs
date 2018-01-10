
using System;
using System.Linq;
using System.Collections.Generic;

using Pdw.Core;

using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Services.Template.Pdx
{
    class XslGenerator : Base.BaseXslGenerator
    {
        #region private properties
        private TextParser _textParser;        
        #endregion

        public XslGenerator(string content)
        {
            _isAddSection = Wkl.MainCtrl.CommonCtrl.CommonProfile.IsAddSection;
            _textParser = new TextParser(content);
        }

        public string GenXsl(string key)
        {
            try
            {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

            ValidateBookmarks();

            // prepare data
            _ibm = srvPro.Ibm;

            // replace bookmarks by xslt tags                
            ReplaceBookmarkTag();

            // add multi-section
            AddMultiSection();

            // build xsl string (put header, body, footer)
            return GetXslString();
            }
            catch (Pdw.Core.BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(Pdw.Core.ErrorCode.ipe_GetXslContentError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(Pdw.Core.ErrorCode.ipe_GetXslContentError,
                    Pdw.Core.MessageUtils.Expand(Properties.Resources.ipe_GetXslContentError, ex.Message), ex.StackTrace);

                throw srvExp;
            }
        }

        #region validate bookmark
        private void ValidateBookmarks()
        {
            List<PartBookmark> ifCollection = new List<PartBookmark>();
            List<PartBookmark> foreachCollection = new List<PartBookmark>();

            foreach (PartBookmark bm in _textParser.Bookmarks)
            {
                switch (bm.Type)
                {
                    case Core.BookmarkType.StartForeach:
                        foreachCollection.Add(bm);
                        break;
                    case Core.BookmarkType.EndForeach:
                        if (foreachCollection.Count == 0)
                            bm.MarkDelete();
                        else
                            foreachCollection.RemoveAt(foreachCollection.Count - 1);
                        break;
                    case Core.BookmarkType.StartIf:
                        ifCollection.Add(bm);
                        break;
                    case Core.BookmarkType.EndIf:
                        if (ifCollection.Count == 0)
                            bm.MarkDelete();
                        else
                            ifCollection.RemoveAt(ifCollection.Count - 1);
                        break;
                    default:
                        break;
                }
            }

            foreach (PartBookmark bm in ifCollection)
                bm.MarkDelete();

            foreach (PartBookmark bm in foreachCollection)
                bm.MarkDelete();
        }
        #endregion

        #region process bookmark tag
        private void ReplaceBookmarkTag()
        {
            _foreach = new List<ForeachItem>();
            for (int index = 0; index < _textParser.Bookmarks.Count; index++)
            {
                PartBookmark bookmark = _textParser.Bookmarks[index];
                List<string> selectedTables = GetSelectedTables(bookmark);
                Relations relations = GetRelations(bookmark);
                if (!bookmark.IsDelete)
                {
                    string xslPath = string.Empty;
                    switch (bookmark.Type)
                    {
                        case Core.BookmarkType.EndForeach:
                            bookmark.XslString = Mht.XslContent.XslFormat.XslEndForeach;
                            if (_foreach.Count > 0)
                                _foreach.RemoveAt(_foreach.Count - 1);
                            UpdateBookmarkPart(bookmark);
                            break;
                        case Core.BookmarkType.EndIf:
                            bookmark.XslString = Mht.XslContent.XslFormat.XslEndIf;
                            UpdateBookmarkPart(bookmark);
                            break;
                        case Core.BookmarkType.Image:
                            break;
                        case Core.BookmarkType.Select:
                            xslPath = GetXslPath(bookmark, bookmark.BizName, false, selectedTables, relations);
                            bookmark.XslString = Mht.XslContent.XslValueOfTag(xslPath, false);
                            UpdateBookmarkPart(bookmark);
                            break;
                        case Core.BookmarkType.StartIf:
                            xslPath = GetXslPath(bookmark, bookmark.BizName, true, selectedTables, relations);
                            bookmark.XslString = Mht.XslContent.XslStartIfTag(xslPath, false);
                            UpdateBookmarkPart(bookmark);
                            break;
                        case Core.BookmarkType.StartForeach:
                            int indexForeach = _foreach.Count + 1;
                            string variant = Core.MarkupConstant.XslVariableImage + index.ToString();
                            InternalBookmarkDomain ibmDomain = _ibm.GetInternalBookmarkDomainByItemKey(bookmark.Key);
                            ForeachItem fItem = new ForeachItem(index, _textParser.Bookmarks.Cast<Base.BaseBookmark>(), 
                                relations, ibmDomain.SelectedTables,
                                indexForeach, variant, string.Empty);
                            _foreach.Add(fItem);

                            xslPath = GetXslPath(bookmark, string.Empty, false, selectedTables, relations);
                            InternalBookmarkItem ibmItem = _ibm.GetInternalBookmarkItem(bookmark.Key);
                            string sort = GetSortXsl(bookmark.BizName, ibmItem.DomainName, selectedTables, relations, false);
                            bookmark.XslString = Mht.XslContent.XslStartForeachTag(xslPath,
                                variant, Core.MarkupConstant.XslMultiPosition, sort, false);
                            fItem.XslString = bookmark.XslString;

                            UpdateBookmarkPart(bookmark);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private List<string> GetSelectedTables(int pbmItemIndex)
        {
            return GetSelectedTables(_textParser.Bookmarks[pbmItemIndex]);
        }

        private Relations GetRelations(int pbmItemIndex)
        {
            return GetRelations(_textParser.Bookmarks[pbmItemIndex]);
        }

        #region update part
        private void UpdateBookmarkPart(PartBookmark bookmark)
        {
            _textParser.ReplaceXsl(bookmark);
        }
        #endregion
        #endregion

        #region add multi-section
        private void AddMultiSection()
        {
            if (_isAddSection)
            {
                // add start multi-section
                _textParser.AppendHeader(Mht.XslContent.XslStartForeachTag(GetSectionPath(),
                    Core.MarkupConstant.XslVariableSection, Core.MarkupConstant.XslPosition, false));

                // add end multi-section
                _textParser.AppendFooter(Mht.XslContent.XslFormat.XslEndForeach);
            }
        }
        #endregion

        #region get xsl string (put header, footer and get part string)
        private string GetXslString()
        {
            // add header
            _textParser.AppendHeader(Mht.XslContent.XslHeader(isBreakLine:false));

            // add footer
            _textParser.AppendFooter(Mht.XslContent.XslFooter(false));

            return _textParser.ToString();
        }
        #endregion
    }
}
