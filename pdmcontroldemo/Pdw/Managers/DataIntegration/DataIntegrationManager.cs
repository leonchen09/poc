
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Pdw.Core;
using Pdw.WKL.Profiler.Manager;
using WinForm = System.Drawing;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Managers.DataIntegration
{
    public class DataIntegrationManager : BaseManager
    {
        #region Bookmark

        #region Add Bookmark

        /// <summary>
        /// Add bookmark with Name = name and Value = current selected range
        /// </summary>
        /// <param name="name"></param>
        public void AddBookmark(string key)
        {
            try
            {
                ManagerProfile mgrProfile = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
                mgrProfile.DeletedBookmarkName = mgrProfile.AddBookmark_Name;
                DeleteBookmark(key);

                Range range = CommonProfile.CurrentSelection.Range;
                Bookmark newBM = CommonProfile.Bookmarks.Add(mgrProfile.AddBookmark_Name, range);
                CommonProfile.Bookmarks.DefaultSorting = WdBookmarkSortBy.wdSortByLocation;
                CommonProfile.Bookmarks.ShowHidden = false;
                newBM.Range.HighlightColorIndex = WdColorIndex.wdNoHighlight;
                mgrProfile.AddBookmark_BookmarkReturn = newBM;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_AddWordBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_AddWordBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// add image into bookmark
        /// </summary>
        /// <param name="name">Name (key) of bookmark</param>
        /// <param name="text">text (value = biz name) of bookmark</param>
        /// <returns></returns>
        public void AddImageBookmark(string key)
        {
            try
            {
                ManagerProfile mgrProfile = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
                mgrProfile.DeletedBookmarkName = mgrProfile.WbmKey;
                DeleteBookmark(key);

                // 1. get user data folder
                string filePath = AssetManager.FileAdapter.TemporaryFolderPath;

                // 2. make image and save into user data folder
                using (WinForm.Image img = Properties.Resources.ImageHolder)
                {
                    WriteTextIntoImage(img, mgrProfile.WbmValue);
                    filePath = string.Format("{0}\\{1}{2}.jpg", filePath, mgrProfile.WbmKey, ProntoMarkup.ImageHolderKey);
                    img.Save(filePath);
                    img.Dispose();
                }

                // 3. make image into word
                CommonProfile.CurrentSelection.TypeText(" ");
                InlineShape inlineShape = CommonProfile.CurrentSelection.InlineShapes.AddPicture(filePath, false, true);
                inlineShape.AlternativeText = mgrProfile.AlternativeText;

                // 4. add bookmark
                Selection selection = CommonProfile.CurrentSelection;
                selection.MoveLeft(WdUnits.wdCharacter, 1);
                selection.MoveRight(WdUnits.wdCharacter, 1, WdMovementType.wdExtend);
                mgrProfile.AddBookmark_BookmarkReturn = CommonProfile.Bookmarks.Add(mgrProfile.WbmKey, selection.Range);
                selection.MoveRight(WdUnits.wdCharacter);

                // 5. delete file out of user data
                System.IO.File.Delete(filePath);
            }
            catch (System.Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_GenerateImageError,
                    MessageUtils.Expand(Properties.Resources.ipe_GenerateImageError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        #region helper method
        /// <summary>
        /// Write a text into image
        /// </summary>
        /// <param name="img"></param>
        /// <param name="text"></param>
        private void WriteTextIntoImage(WinForm.Image img, string text)
        {
            #region 0. declare variants
            WinForm.Font font = new WinForm.Font("Arial", 8);
            WinForm.Brush brush = WinForm.Brushes.White;
            float xText = 2;
            float yText = 0;
            float hText = 0;
            float wText = 0;
            WinForm.Graphics g;
            WinForm.SizeF sizeF;
            string line = string.Empty;
            System.Collections.Generic.List<string> lines = new System.Collections.Generic.List<string>();
            #endregion

            #region 1. initialize graphic object
            g = WinForm.Graphics.FromImage(img);
            #endregion

            #region 2. make sure this image is unique
            g.DrawString(System.Guid.NewGuid().ToString(), new WinForm.Font("Arial", 1),
                WinForm.Brushes.CornflowerBlue, 0, 0);
            #endregion

            #region 3. measure size of text
            sizeF = g.MeasureString(text, font);
            hText = sizeF.Height;
            wText = sizeF.Width;
            #endregion

            #region 4. calculate necessary line to display text
            for (int index = 0; index < text.Length; index++)
            {
                line += text[index].ToString();
                sizeF = g.MeasureString(line, font);
                if (sizeF.Width > img.Width - 2 * xText)
                {
                    lines.Add(line);
                    line = string.Empty;
                }
            }
            if (!string.IsNullOrEmpty(line))
                lines.Add(line);
            #endregion

            #region 5. make sure the text is displayes inbound of image holder
            int count = lines.Count;
            while (lines.Count * hText > img.Height)
                lines.RemoveAt(lines.Count - 1);
            if (count != lines.Count)
            {
                line = lines[lines.Count - 1];
                for (int index = 1; index <= 4; index++)
                {
                    if (line.Length > 1)
                        line = line.Remove(line.Length - 1, 1);
                }
                lines[lines.Count - 1] = line + "...>";
            }
            #endregion

            #region 6. draw text into image
            yText = (img.Height - lines.Count * hText) / 2;
            foreach (string str in lines)
            {
                g.DrawString(str, font, brush, xText, yText);
                yText += hText;
            }
            #endregion

            #region 7. dispose graphic object
            g.Dispose();
            #endregion
        }
        #endregion
        #endregion

        #region Delete Bookmark
        /// <summary>
        /// Delete a bookmark with name in current document
        /// </summary>
        /// <param name="name">Name of bookmark</param>
        /// <returns></returns>
        public bool DeleteBookmark(string key)
        {
            ManagerProfile mgrProfile = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            try
            {
                foreach (Bookmark bookmark in Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks)
                {
                    if (bookmark.Name.Equals(mgrProfile.DeletedBookmarkName))
                    {
                        DeleteBookmark(bookmark, mgrProfile.DeleteWholeBookmark);
                        break;
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_DeleteBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_DeleteBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        //public bool DeleteAllBookmark(string value)
        //{
        //    try
        //    {
        //        foreach (Bookmark bookmark in Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks)
        //            if (MarkupUtilities.GetRangeText(bookmark.Range).Equals(value))
        //                DeleteBookmark(bookmark);
        //        return true;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        ManagerException mgrExp = new ManagerException(ErrorCode.ipe_DeleteBookmarkError,
        //            MessageUtils.Expand(Properties.Resources.ipe_DeleteBookmarkError, ex.Message), ex.StackTrace);

        //        throw mgrExp;
        //    }
        //}

        private void DeleteBookmark(Bookmark bookmark, bool isDeleteWhole)
        {
            if (bookmark != null)
            {
                // 1. remove order by if bookmark is data tag
                string name = bookmark.Name;
                string text = MarkupUtilities.GetRangeText(bookmark.Range);
                if (name.EndsWith(ProntoMarkup.KeySelect))
                {
                    System.Collections.Generic.List<Bookmark> bms = GetBookmarksOrderByPosition();
                    System.Collections.Generic.Stack<Bookmark> foreachBms = new System.Collections.Generic.Stack<Bookmark>();
                    bool isExist = false;
                    foreach (Bookmark bm in bms)
                    {
                        if (bm.Name.EndsWith(ProntoMarkup.KeyStartForeach))
                            foreachBms.Push(bm);
                        if (bm.Name.EndsWith(ProntoMarkup.KeyEndForeach))
                            foreachBms.Pop();
                        if (bm.Name == name)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (isExist && foreachBms.Count > 0)
                    {
                        Bookmark foreachBm = foreachBms.Pop();
                        string oldText = MarkupUtilities.GetRangeText(foreachBm.Range);
                        string newText = oldText.Replace(text +
                            Constants.OrderBy.Concat +
                            Constants.OrderBy.AscMark +
                            Constants.OrderBy.Delimiter, "");
                        newText = newText.Replace(text +
                            Constants.OrderBy.Concat +
                            Constants.OrderBy.DescMark +
                            Constants.OrderBy.Delimiter, "");
                        newText = newText.Replace(Constants.OrderBy.Delimiter + text +
                            Constants.OrderBy.Concat +
                            Constants.OrderBy.AscMark, "");
                        newText = newText.Replace(Constants.OrderBy.Delimiter + text +
                            Constants.OrderBy.Concat +
                            Constants.OrderBy.DescMark, "");
                        if (oldText != newText)
                            UpdateBookmarkText(foreachBm, newText);
                    }
                }

                // 2. remove bookmark
                if (isDeleteWhole)
                    bookmark.Range.Text = "";
                else
                    bookmark.Delete();
            }
        }

        #endregion

        #region edit bookmark (text, background)
        /// <summary>
        /// highlight bookmark
        /// </summary>
        /// <param name="name"></param>
        public void HighLightBookmark(string key)
        {
            try
            {
                ManagerProfile managerPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
                Bookmark bm = GetBookmark(managerPro.HighlightBookmarkName);
                WdColorIndex color = managerPro.WdColorIndex == null ? ProntoMarkup.BackgroundHighLight : managerPro.WdColorIndex;

                if (bm != null && (bm.Range.HighlightColorIndex != color))
                {
                    bm.Range.HighlightColorIndex = color;
                    bm.Range.Font.ColorIndex = ProntoMarkup.ForeColorHighLight;
                }
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_HighlightBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_HighlightBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// highlight bookmark
        /// </summary>
        /// <param name="name"></param>
        public void UnHighLightBookmark(string key)
        {
            try
            {
                ManagerProfile managerPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
                Bookmark bm = GetBookmark(managerPro.HighlightBookmarkName);
                WdColorIndex color = managerPro.WdColorIndex == null ? ProntoMarkup.BackgroundHighLight : managerPro.WdColorIndex;

                if (bm != null && (bm.Range.HighlightColorIndex == color))
                {
                    bm.Range.HighlightColorIndex = ProntoMarkup.BackgroundUnHighLight;
                }
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_UnHighlightWordBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_UnHighlightWordBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// get a bookmark object in word follow by bookmark's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bookmark GetBookmark(string name)
        {
            try
            {
                if (Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks.Exists(name))
                    return Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks[name];

                return null;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_GetWordBookmarksError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetWordBookmarksError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// Update value of bookmark
        /// </summary>
        /// <param name="bm"></param>
        /// <param name="newText"></param>
        public void UpdateBookmarkText(Bookmark bm, string newText)
        {
            try
            {
                // 1. get old information
                string name = bm.Name;
                Range range = bm.Range;

                // 2. update word bookmark
                range.Text = newText;
                Bookmark newBm = Wkl.MainCtrl.CommonCtrl.CommonProfile.Bookmarks.Add(name, range);
                Bookmark endBm = GetBookmark(name.Replace(ProntoMarkup.KeyStartForeach, ProntoMarkup.KeyEndForeach));
                if (newText.Contains(Constants.OrderBy.ForeachSortMark))
                {
                    newBm.Range.Font.Color = ProntoMarkup.ForeachConditionColor;
                    if (endBm != null)
                        endBm.Range.Font.Color = ProntoMarkup.ForeachConditionColor;
                }
                else
                {
                    newBm.Range.Font.Color = ProntoMarkup.ForeachColor;
                    if (endBm != null)
                        endBm.Range.Font.Color = ProntoMarkup.ForeachColor;
                }

                // 3. update internal bookmark
                CurrentTemplateInfo.InternalBookmark.GetInternalBookmarkItem(name).BizName = newText;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_UpdateWordBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_UpdateWordBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }
        #endregion

        #region using for Order menu button
        /// <summary>
        /// Get bookmark list order by position (top , left -> bottom, right) in document
        /// </summary>
        /// <returns></returns>
        public List<Bookmark> GetBookmarksOrderByPosition()
        {
            try
            {
                List<Bookmark> bms = new List<Bookmark>();

                foreach (Bookmark bm in CommonProfile.Bookmarks)
                    bms.Add(bm);

                bms.Sort(delegate(Bookmark bm1, Bookmark bm2)
                {
                    return bm1.Start.CompareTo(bm2.Start);
                });

                return bms;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_GetWordBookmarkByPositionError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetWordBookmarkByPositionError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// Check this section has bookmark or no
        /// </summary>
        /// <param name="Sel">Section</param>
        /// <returns></returns>
        public string HasBookmark(Selection Sel)
        {
            string bmName = string.Empty;
            if (Sel != null)
            {
                if (Sel.Bookmarks != null)
                {
                    int count = Sel.Bookmarks.Count;
                    if (count > 0)
                    {
                        string selText = Sel.Range.Text;
                        foreach (Bookmark bm in Sel.Bookmarks)
                        {
                            string bmValue = bm.Range.Text;

                            if (string.IsNullOrEmpty(selText))
                            {
                                if (!MarkupUtilities.IsProntoDocCommentBookmark(bm.Name))
                                {
                                    bm.Range.Select();
                                    bmName = count == 1 ? bm.Name : string.Empty;
                                }
                                continue;
                            }

                            if (!selText.Contains(bmValue) && !MarkupUtilities.IsProntoDocCommentBookmark(bm.Name))
                            {
                                if (bm.End > Sel.End) // bookmark is end of selection
                                    Sel.End = bm.End;

                                if (Sel.Start > bm.Start) // bookmark is start of selection
                                    Sel.Start = bm.Start;
                                bmName = count == 1 ? bm.Name : string.Empty;
                            }
                        }
                    }
                }
            }

            return bmName;
        }

        /// <summary>
        /// The selection has foreach tag or no (using for show order by menu)
        /// </summary>
        /// <param name="Sel"></param>
        /// <returns></returns>
        public Bookmark GetForeachTag(Selection Sel)
        {
            if (Sel != null)
            {
                if (Sel.Bookmarks != null)
                {
                    foreach (Bookmark bm in Sel.Bookmarks)
                    {
                        if (bm.Name.EndsWith(ProntoMarkup.KeyStartForeach))
                            return bm;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get all data tags bookmark inside bookmarkName tag (ex: inside foreach tag)
        /// </summary>
        /// <param name="bookmarkName"></param>
        /// <returns>Only get data tags (select tags)</returns>
        public List<Bookmark> GetInsideBookmarks(string startName, string endName)
        {
            List<Bookmark> bms = new List<Bookmark>();
            List<Bookmark> sortedBms = GetBookmarksOrderByPosition();

            int countForeach = 0;
            foreach (Bookmark bm in sortedBms)
            {
                // 1. found open foreach tag
                if (bm.Name == startName)
                {
                    countForeach = 1;
                    continue;
                }

                // 2. Continue if still not found open foreach tag
                if (countForeach == 0)
                    continue;

                // 3. found close tag then exist
                if (bm.Name == endName)
                    break;

                // 4. mark going into child foreach tag
                if (bm.Name.EndsWith(ProntoMarkup.KeyStartForeach))
                {
                    countForeach++;
                    continue;
                }
                if (bm.Name.EndsWith(ProntoMarkup.KeyEndForeach))
                {
                    countForeach--;
                    continue;
                }

                // 5. add data tag into result
                if (countForeach == 1 && bm.Name.EndsWith(ProntoMarkup.KeySelect))
                    bms.Add(bm);
            }

            return bms;
        }
        #endregion

        /// <summary>
        /// get all foreach tags that contains current position (current position belong to that)
        /// </summary>
        /// <param name="key"></param>
        public void GetForeachTagsBoundCurrentPos(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            mgrPro.DicBookmarks = new Dictionary<string, Bookmark>();

            int sPos = CommonProfile.CurrentSelection.Start;
            int ePos = CommonProfile.CurrentSelection.End;
            //int startCount = 0;

            foreach (Bookmark wbm in CommonProfile.Bookmarks)
            {
                // check start position
                int pos = wbm.Start;
                if (pos <= sPos)
                {
                    if (wbm.Name.EndsWith(ProntoMarkup.KeyStartForeach))
                    {
                        mgrPro.DicBookmarks.Add(wbm.Name, wbm);
                        //startCount++;
                    }
                    //else if(startCount > 0)
                    //    mgrPro.DicBookmarks.Add(wbm.Name, wbm);
                }

                // check end position
                pos = wbm.End;
                if (pos >= ePos)
                {
                    if (wbm.Name.EndsWith(ProntoMarkup.KeyEndForeach))
                    {
                        mgrPro.DicBookmarks.Add(wbm.Name, wbm);
                        //startCount--;
                    }
                    //else if(startCount > 0)
                    //    mgrPro.DicBookmarks.Add(wbm.Name, wbm);
                }

                //if (wbm.Name.EndsWith(ProntoMarkup.KeyStartForeach))
                //{
                //    int pos = wbm.Start;
                //    if (pos <= sPos)
                //        mgrPro.DicBookmarks.Add(wbm.Name, wbm);
                //}
                //if (wbm.Name.EndsWith(ProntoMarkup.KeyEndForeach))
                //{
                //    int pos = wbm.End;
                //    if (pos >= ePos)
                //        mgrPro.DicBookmarks.Add(wbm.Name, wbm);
                //}
            }
        }
        #endregion

        #region Document Motion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isProntoDoc"></param>
        public void MarkProntoDoc(string key)
        {
            ManagerProfile mgrProfile = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.IsProntoDoc = mgrProfile.MarkProntDoc;
            if (mgrProfile.MarkProntDoc)
            {
                if (!System.IO.File.Exists(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.FullName))
                    Wkl.MainCtrl.CommonCtrl.CommonProfile.App.Options.SaveInterval = 0;
            }
        }

        /// <summary>
        /// Add text into current selection in current document
        /// </summary>
        /// <param name="text">text string</param>
        /// <param name="isSelect">Select or not after add</param>
        public void AddText(string key)
        {
            ManagerProfile mgrProfile = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            CommonProfile.CurrentSelection.TypeText(mgrProfile.AddToSelection_Text);

            if (mgrProfile.AddToSelection_IsSelected)
            {
                MoveCharacters(mgrProfile.AddToSelection_Text.Length, true, true);
            }
        }

        /// <summary>
        /// Move cursor up to count line
        /// </summary>
        /// <param name="count"></param>
        private void MoveLines(int count, bool isUp)
        {
            if (isUp)
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentSelection.MoveUp(WdUnits.wdLine, count);
            else
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentSelection.MoveDown(WdUnits.wdLine, count);
        }

        /// <summary>
        /// Move cursor to [left] or [right] with [count] characters
        /// </summary>
        /// <param name="count">Number of characters</param>
        /// <param name="isLeft">true if go the left</param>
        public void MoveCharacters(string key)
        {
            ManagerProfile mgrProfile = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            if (mgrProfile.MoveChars_IsExtend == null || !mgrProfile.MoveChars_IsExtend.Value)
            {
                if (mgrProfile.MoveChars_IsLeft)
                {
                    CommonProfile.CurrentSelection.MoveLeft(WdUnits.wdCharacter, mgrProfile.MoveChars_Count);
                }
                else
                    CommonProfile.CurrentSelection.MoveRight(WdUnits.wdCharacter, mgrProfile.MoveChars_Count);
            }
            else
            {
                if (mgrProfile.MoveChars_IsLeft)
                {
                    CommonProfile.CurrentSelection.MoveLeft(WdUnits.wdCharacter, mgrProfile.MoveChars_Count, WdMovementType.wdExtend);
                }
                else
                    CommonProfile.CurrentSelection.MoveRight(WdUnits.wdCharacter, mgrProfile.MoveChars_Count, WdMovementType.wdExtend);
            }
        }

        /// <summary>
        /// Move cursor to [left] or [right] with [count] characters
        /// </summary>
        /// <param name="count">Number of characters</param>
        /// <param name="isLeft">true if go the left</param>
        /// <param name="isExtend">true if text is selected</param>
        private void MoveCharacters(int count, bool isLeft, bool isExtend)
        {
            string key;
            ManagerProfile mgrProfile = Wkl.MainCtrl.ManagerCtrl.CreateProfile(out key);
            mgrProfile.MoveChars_Count = count;
            mgrProfile.MoveChars_IsLeft = isLeft;
            if (isExtend)
            {
                if (isLeft)
                    Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentSelection.MoveLeft(WdUnits.wdCharacter, count, WdMovementType.wdExtend);
                else
                    Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentSelection.MoveRight(WdUnits.wdCharacter, count, WdMovementType.wdExtend);
            }
            else
                MoveCharacters(key);

            Wkl.MainCtrl.ManagerCtrl.RemoveDataObject(key);
        }

        public Selection GetCurrentSelection()
        {
            return CommonProfile.CurrentSelection;
        }

        public void SetFont(string key)
        {
            ManagerProfile mgrProfile = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            if(mgrProfile != null && mgrProfile.WdColorIndex != null)
            {
                CommonProfile.CurrentSelection.Font.ColorIndex = mgrProfile.WdColorIndex;
            }
        }
        #endregion

        public void ProcessFormControlAction(string key)
        {
            ManagerProfile profile = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);

            Pdw.FormControls.ControlBase control = profile.PdmControl;

            control.DataDindPropertyChanged += (o, e) => new MainManager().RibbonManager.UpdateUI(EventType.RefreshControlPropertyPanel);

            control.Component.Click += new EventHandler(DisplayControlProperties);

            if (profile.IsAddingPdmControl)
            {
                DisplayControlProperties(control.Component, EventArgs.Empty);
            }
        }

        private void DisplayControlProperties(object sender, EventArgs e)
        {
            new MainManager().RibbonManager.UpdateUI(EventType.ShowControlPropertyPanel);

            CurrentTemplateInfo.ControlPropertyGrid.SelectedObject = (sender as Control).Tag;
        }
    }
}