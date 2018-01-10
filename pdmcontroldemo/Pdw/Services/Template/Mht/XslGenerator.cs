
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Pdw.Core;

using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Services.Template.Mht
{
    class XslGenerator:Base.BaseXslGenerator
    {
        private List<Part> _parts;
        private Part _htmlPart;
        private List<Part> _imageParts;
        private List<PartBookmark> _bookmarks;

        public XslGenerator(string mhtFilePath)
        {
            _isAddSection = Wkl.MainCtrl.CommonCtrl.CommonProfile.IsAddSection;

            Mime mhtMime = new Mime();
            _parts = mhtMime.Parse(mhtFilePath);

            _htmlPart = null;
            _imageParts = new List<Part>();
            foreach (Part part in _parts)
            {
                if (part.Type == PartType.MhtHtml)
                {
                    _htmlPart = part;
                    continue;
                }

                if (part.Type == PartType.MhtImage)
                    _imageParts.Add(part);
            }
        }

        public string GenXsl(string key)
        {
            try
            {
                ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
                if (_htmlPart == null)
                    return string.Empty;

                // check struct of xml file
                _bookmarks = _htmlPart.GetBookmarks();
                if (_bookmarks.Count == 0)
                    return string.Empty;

                ValidateBookmarks();

                // prepare data
                _ibm = srvPro.Ibm;

                // replace bookmarks by xslt tags                
                ReplaceBookmarkTag();

                // add mht add-on and multi-section
                AddMhtAddOnAndSection();

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

            foreach (PartBookmark bm in _bookmarks)
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
            for (int index = 0; index < _bookmarks.Count; index++)
            {
                PartBookmark bookmark = _bookmarks[index];
                List<string> selectedTable = GetSelectedTables(bookmark);
                Relations relations = GetRelations(bookmark);
                if (!bookmark.IsDelete)
                {
                    string xslPath = string.Empty;
                    switch (bookmark.Type)
                    {
                        case Core.BookmarkType.EndForeach:
                            bookmark.XslString = XslContent.XslFormat.XslEndForeach;
                            if (_foreach.Count > 0)
                                _foreach.RemoveAt(_foreach.Count - 1);
                            UpdateBookmarkPart(bookmark, index);
                            break;
                        case Core.BookmarkType.EndIf:
                            bookmark.XslString = XslContent.XslFormat.XslEndIf;
                            UpdateBookmarkPart(bookmark, index);
                            break;
                        case Core.BookmarkType.Image:
                            xslPath = GetXslPath(bookmark, bookmark.BizName, false, selectedTable, relations);
                            bookmark.XslString = XslContent.XslValueOfTag(xslPath);
                            UpdateImageBookmark(bookmark);
                            break;
                        case Core.BookmarkType.Select:
                            xslPath = GetXslPath(bookmark, bookmark.BizName, false, selectedTable, relations);
                            bookmark.XslString = XslContent.XslValueOfTag(xslPath,
                                bookmark.BizName == ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark ? false : true);
                            UpdateBookmarkPart(bookmark, index);
                            break;
                        case Core.BookmarkType.StartIf:
                            xslPath = GetXslPath(bookmark, bookmark.BizName, true, selectedTable, relations);
                            bookmark.XslString = XslContent.XslStartIfTag(xslPath);
                            UpdateBookmarkPart(bookmark, index);
                            break;
                        case Core.BookmarkType.StartForeach:
                            int indexForeach = _foreach.Count + 1;
                            string variant = Core.MarkupConstant.XslVariableImage + index.ToString();
                            InternalBookmarkDomain ibmDomain = _ibm.GetInternalBookmarkDomainByItemKey(bookmark.Key);
                            ForeachItem fItem = new ForeachItem(index, _bookmarks.Cast<Base.BaseBookmark>(), 
                                relations, ibmDomain.SelectedTables, indexForeach, variant, string.Empty);
                            _foreach.Add(fItem);

                            xslPath = GetXslPath(bookmark, string.Empty, false, selectedTable, relations);
                            InternalBookmarkItem ibmItem = _ibm.GetInternalBookmarkItem(bookmark.Key);
                            string sort = GetSortXsl(bookmark.BizName, ibmItem.DomainName, selectedTable, relations, true);
                            bookmark.XslString = XslContent.XslStartForeachTag(xslPath,
                                variant, Core.MarkupConstant.XslMultiPosition, sort);
                            fItem.XslString = bookmark.XslString;

                            UpdateBookmarkPart(bookmark, index);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private List<string> GetSelectedTables(int pbmItemIndex)
        {
            return GetSelectedTables(_bookmarks[pbmItemIndex]);
        }

        private Relations GetRelations(int pbmItemIndex)
        {
            return GetRelations(_bookmarks[pbmItemIndex]);
        }

        #region update part
        private void UpdateBookmarkPart(PartBookmark bookmark, int count)
        {
            int startLineIndex = bookmark.StartLineIndex;
            int endLineIndex = bookmark.EndLineIndex;
            int startCharIndex = bookmark.StartCharIndexOfStartLine;
            int endCharIndex = bookmark.EndCharIndexOfEndLine;
            if (bookmark.Type == Core.BookmarkType.StartForeach || bookmark.Type == Core.BookmarkType.StartIf ||
                bookmark.Type == Core.BookmarkType.EndForeach || bookmark.Type == Core.BookmarkType.EndIf)
            {
                _htmlPart.FindStartParagrap(bookmark.StartLineIndex, bookmark.StartCharIndexOfStartLine,
                    out startLineIndex, out startCharIndex);
                _htmlPart.FindEndParagrap(bookmark.EndLineIndex, bookmark.EndCharIndexOfEndLine,
                    out endLineIndex, out endCharIndex);
            }

            string line = _htmlPart.Lines[startLineIndex];
            int change = line.Length;
            if (startLineIndex == endLineIndex)
            {
                int numberChar = Math.Min(endCharIndex - startCharIndex, line.Length - startCharIndex);
                line = _htmlPart.RemoveString(line, startCharIndex, numberChar);
            }
            else if (startLineIndex < endLineIndex)
            {
                for (int i = startLineIndex + 1; i < endLineIndex; i++)
                    _htmlPart.RemoveLine(i);

                line = _htmlPart.RemoveString(line, startCharIndex);

                string endLineString = _htmlPart.Lines[endLineIndex];
                endLineString = _htmlPart.RemoveString(endLineString, 0, endCharIndex);
                _htmlPart.Lines[endLineIndex] = endLineString;
            }

            line = line.Insert(startCharIndex, bookmark.XslString);
            _htmlPart.Lines[startLineIndex] = line;

            // update index
            change = line.Length - change;
            if (change != 0)
            {
                for (int bmIndex = count + 1; bmIndex < _bookmarks.Count; bmIndex++)
                {
                    PartBookmark bm = _bookmarks[bmIndex];
                    if (bm.StartLineIndex == bookmark.StartLineIndex)
                        bm.StartCharIndexOfStartLine += change;
                }
            }
        }

        private void UpdateImageBookmark(PartBookmark bookmark)
        {
            //1. Find src=3D" attribute 
            int index = bookmark.StartLineIndex;
            string line = _htmlPart.Lines[index];
            while (index < _htmlPart.Lines.Count && !line.Contains(Mime.SourceImage))
            {
                index++;
                line = _htmlPart.Lines[index];
            }
            if (index >= _htmlPart.Lines.Count)
                return;

            //2. Find end of src3D attribute (Char: '"'), last position of Dot ('.')
            int dotLineIndex = index;
            int dotCharIndex = line.LastIndexOf(Mime.Dot);
            string imagePath = string.Empty;
            line = line.Substring(line.IndexOf(Mime.SourceImage) + Mime.SourceImage.Length);
            while (index < _htmlPart.Lines.Count && !string.IsNullOrEmpty(line) && !line.Contains(Mime.EndAttribute))
            {
                imagePath += line.TrimEnd(new char[] { Mime.Equal });
                index++;
                line = _htmlPart.Lines[index];

                if (line.Contains(Mime.Dot))
                {
                    dotLineIndex = index;
                    dotCharIndex = line.LastIndexOf(Mime.Dot);
                }
            }
            if (index >= _htmlPart.Lines.Count)
                return;

            //3. Insert xsl string and get result
            string variant = string.Empty;
            if (_foreach.Count > 0)
                variant = _foreach[_foreach.Count - 1].VariantName;
            if (!string.IsNullOrEmpty(variant))
                variant = XslContent.XslValueOfTag(variant, false, true);
            if (_isAddSection)
                variant = XslContent.XslValueOfTag(Core.MarkupConstant.XslVariableSection, false) + variant;
            _htmlPart.Lines[dotLineIndex] = _htmlPart.Lines[dotLineIndex].Insert(dotCharIndex, variant);
            imagePath += line.Substring(0, line.IndexOf(Mime.EndAttribute));
            UpdateImagePart(imagePath, bookmark, variant);
        }

        private void UpdateImagePart(string imageFilePath, PartBookmark bookmark, string variant)
        {
            // 1. find image part
            foreach (Part imgPart in _imageParts)
            {
                if (imgPart.Content.Location.Trim().EndsWith(imageFilePath))
                {
                    // replace
                    for (int lineIndex = 0; lineIndex < imgPart.Lines.Count; lineIndex++)
                    {
                        string line = imgPart.Lines[lineIndex];
                        if (string.IsNullOrEmpty(line) || line.StartsWith(Mime.StartPart) ||
                            line.StartsWith(PartContent.ContentTransferEncoding) || line.StartsWith(PartContent.ContentType))
                            continue;

                        if (line.StartsWith(PartContent.ContentLocation))
                        {
                            int lastDotIndex = line.LastIndexOf(Mime.Dot);
                            line = line.Insert(lastDotIndex, variant);
                            imgPart.Lines[lineIndex] = line;

                            continue;
                        }

                        imgPart.RemoveLine(lineIndex);
                    }

                    // insert for-each tag
                    string startLine = string.Empty;
                    string endLine = string.Empty;
                    if (_isAddSection)
                    {

                        startLine = XslContent.XslStartForeachTag(GetSectionPath(),
                            Core.MarkupConstant.XslVariableSection, Core.MarkupConstant.XslPosition);
                        endLine = XslContent.XslFormat.XslEndForeach;
                    }

                    foreach (ForeachItem fItem in _foreach)
                    {
                        startLine = startLine + fItem.XslString;
                        endLine = endLine + XslContent.XslFormat.XslEndForeach;
                    }

                    if (!string.IsNullOrEmpty(startLine))
                        imgPart.Lines.Insert(0, XslContent.InsertBreakLine(startLine, true));
                    imgPart.Lines.Add(bookmark.XslString);
                    if (!string.IsNullOrEmpty(endLine))
                        imgPart.Lines.Add(XslContent.InsertBreakLine(endLine, false));

                    return;
                }
            }
        }
        #endregion
        #endregion

        #region add mht add-on and multi-section
        private void AddMhtAddOnAndSection()
        {
            // find body tag
            int startBodyIndex = -1;
            int endBodyIndex = -1;
            int endHead = -1;

            for (int lineIndex = 0; lineIndex < _htmlPart.Lines.Count; lineIndex++)
            {
                string line = _htmlPart.Lines[lineIndex];
                if (string.IsNullOrEmpty(line))
                    continue;

                if (_isAddSection && line.StartsWith(Mime.StartBody) && startBodyIndex == -1) // open body
                    startBodyIndex = lineIndex;
                if (_isAddSection && line.StartsWith(Mime.EndBody) && endBodyIndex == -1) // end body
                    endBodyIndex = lineIndex;
                if (line.StartsWith(Mime.EndHead) && endHead == -1) // end head
                    endHead = lineIndex;

                if (startBodyIndex != -1 && endBodyIndex != -1 && endHead != -1)
                    break;
            }

            if (endHead > -1 && startBodyIndex != -1 && endBodyIndex != -1)
            {
                AddApplicationHeader(endHead); // add header

                AddApplicationBeforeDocument(startBodyIndex); // add before document

                AddApplicationAfterDocument(endBodyIndex); // add after document
            }
            else
                _isAddSection = false;
        }

        private void AddApplicationHeader(int lineIndex)
        {
            string line = _htmlPart.Lines[lineIndex];
            line = XslContent.XslValueOfTag(ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwApplication + "/"
                + ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwHeader) + line;

            _htmlPart.Lines[lineIndex] = line;
        }

        private void AddApplicationBeforeDocument(int lineIndex)
        {
            // find end of tag
            int index = lineIndex;
            string line = _htmlPart.Lines[lineIndex];
            while (!string.IsNullOrEmpty(line) && !line.EndsWith(Mime.EndTag))
            {
                index++;
                line = _htmlPart.Lines[index];
            }

            // add before document
            line = line + XslContent.XslValueOfTag(ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwApplication + "/"
                + ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwBeforeDocumet);

            // add multi-section
            if (_isAddSection)
            {
                line = line + XslContent.XslStartForeachTag(GetSectionPath(),
                    Core.MarkupConstant.XslVariableSection, Core.MarkupConstant.XslPosition);
            }

            // update line
            _htmlPart.Lines[lineIndex] = line;
        }

        private void AddApplicationAfterDocument(int lineIndex)
        {
            string line = _htmlPart.Lines[lineIndex];

            // add after document
            line = XslContent.XslValueOfTag(ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwApplication + "/"
                + ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwAfterDocument) + line;

            // add multi-section
            if (_isAddSection)
                line = XslContent.XslFormat.XslEndForeach + line;

            // update line
            _htmlPart.Lines[lineIndex] = line;
        }
        #endregion

        #region get xsl string (put header, footer and get part string)
        private string GetXslString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(XslContent.XslHeader());

            foreach (Part part in _parts)
                builder.AppendLine(part.ToString());

            builder.Append(XslContent.XslFooter());
            return builder.ToString();
        }
        #endregion
    }
}