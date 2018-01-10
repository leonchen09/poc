
using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Microsoft.Office.Interop.Word;

using Pdw.Core;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Managers.Hcl
{
    public partial class BookmarkControl : Form
    {

        #region Variables

        private MainManager mainManager;

        #endregion

        public BookmarkControl()
        {
            InitializeComponent();

            mainManager = new MainManager();

            mainManager.RegisterEvents();
        }

        #region Form Event
        private void BookmarkControl_Load(object sender, EventArgs e)
        {
            LoadBookmarks(lstBookmarks);
            ShowContextMenuDelete();
            ShowContextMenuGoto();
            ShowContextMenuHightLight();
        }

        private void cmdGoto_Click(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.GoToFirstBookmark(bm);
            }
        }

        private void cmdHightlight_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Menu Event

        /// <summary>
        /// Delete Current BM.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuDeleteOne_Click(object sender, EventArgs e)
        {
            DeleteOneClick();
        }

        private void mnuDeleteAll_Click(object sender, EventArgs e)
        {

            BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
            mainManager.MainService.BookmarkService.DeleteAllBookmarks(bm);

            RemoveListItem(lstBookmarks, lstBookmarks.SelectedIndex);

            EnableComboboxDomain();
            DecorateTreeView();
        }

        private void mnuGoFirst_Click(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.GoToFirstBookmark(bm);
            }
        }

        private void mnuGoLast_Click(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.GoLastBookmark(bm);
            }
        }

        private void mnuGoNext_Click(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.GoNextBookmark(bm);
            }
        }

        private void mnuGoPrevious_Click(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.GoPreviousBookmark(bm);
            }
        }

        private void mnuHighlightNext_Click(object sender, EventArgs e)
        {
            HighLightClick();
        }

        private void mnuHighlightPrevious_Click(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.HighLightPreviousBookmark(bm);
            }

        }

        private void mnuHighlightFirst_Click(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.HighLightFirstBookmark(bm);
            }

        }

        private void mnuHighlightLast_Click(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.HighLightLastBookmark(bm);
            }

        }

        private void mnuHighlightAll_Click(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.HighLightAllBookmark(bm);
            }

        }
        #endregion

        #region Private Function

        /// <summary>
        /// Load all bookmark to listbox.
        /// </summary>
        /// <param name="listBookmarks"></param>
        private void LoadBookmarks(ListBox listBookmarks)
        {
            // 1. Load bookmark collection
            // 2. Binding to the list
            // 3. Set Selected item.

            //1.
            string key;
            ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key).ContentService;

            mainManager.MainService.BookmarkService.GetDistinctBookmarks(key);

            List<BookmarkItem> bookmarks = contentProfile.GetDistinctBM_OListBM;

            //2.
            foreach (BookmarkItem itemBM in bookmarks)
            {
                if(MarkupUtilities.IsProntoDocBookmark(itemBM.Name, 
                    Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.InternalBookmark))
                    listBookmarks.Items.Add(itemBM);
            }

            listBookmarks.DisplayMember = Constants.BookMarkControl.DisplayField;

            string selectedText = WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentSelection.Text.Trim();
            //3.
            if (listBookmarks.Items.Count > 0)
                listBookmarks.SelectedIndex = 0;

            for (int i = 0; i < listBookmarks.Items.Count; i++)
                if (((BookmarkItem)listBookmarks.Items[i]).Value == selectedText)
                {
                    listBookmarks.SetSelected(i, true);
                }

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        /// <summary>
        /// Show all context menus of Deleting.
        /// </summary>
        private void ShowContextMenuDelete()
        {
            ContextMenu mnuContext = new ContextMenu();
            mnuContext.Popup += new EventHandler(mnuContextDelete_Popup);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuDeleteCurrent).Click += new EventHandler(mnuDeleteOne_Click);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuDeleteAll).Click += new EventHandler(mnuDeleteAll_Click);

            cmdDelete.ContextMenu = mnuContext;
        }

        /// <summary>
        /// Show all context menus of Going to.
        /// </summary>
        private void ShowContextMenuGoto()
        {
            ContextMenu mnuContext = new ContextMenu();
            mnuContext.Popup += new EventHandler(mnuContextGoto_Popup);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuGotoNext).Click += new EventHandler(mnuGoNext_Click);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuGoPrevious).Click += new EventHandler(mnuGoPrevious_Click);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuGoFirst).Click += new EventHandler(mnuGoFirst_Click);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuGoLast).Click += new EventHandler(mnuGoLast_Click);


            cmdGoto.ContextMenu = mnuContext;
        }

        private void ShowContextMenuHightLight()
        {
            ContextMenu mnuContext = new ContextMenu();
            mnuContext.Popup += new EventHandler(mnuContextHighlight_Popup);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuHighlightAll).Click += new EventHandler(mnuHighlightAll_Click);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuHighlightNext).Click += new EventHandler(mnuHighlightNext_Click);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuHighlightPrevious).Click += new EventHandler(mnuHighlightPrevious_Click);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuHighlightFirst).Click += new EventHandler(mnuHighlightFirst_Click);

            CreateMenuItem(mnuContext, Properties.Resources.ipm_mnuHighlightLast).Click += new EventHandler(mnuHighlightLast_Click);

            cmdHightlight.ContextMenu = mnuContext;
        }

        private void mnuContextDelete_Popup(object sender, EventArgs e)
        {


            if (lstBookmarks.SelectedItem != null)
            {
                cmdDelete.ContextMenu.MenuItems[1].Enabled = (lstBookmarks.Items.Count > 0 &&
                                                ((BookmarkItem)lstBookmarks.SelectedItem).NumberEntry > 1) ? true : false;

                cmdDelete.ContextMenu.MenuItems[0].Enabled = (lstBookmarks.Items.Count > 0) ? true : false;

            }
            else
            {
                cmdDelete.ContextMenu.MenuItems[1].Enabled = false;
                cmdDelete.ContextMenu.MenuItems[0].Enabled = false;
            }

        }

        private void mnuContextHighlight_Popup(object sender, EventArgs e)
        {
            bool enable = false;
            if (lstBookmarks.SelectedItem != null)
            {
                enable = (lstBookmarks.Items.Count > 0 &&
                                               ((BookmarkItem)lstBookmarks.SelectedItem).NumberEntry > 1) ? true : false;
            }

            cmdHightlight.ContextMenu.MenuItems[0].Enabled = enable;
            cmdHightlight.ContextMenu.MenuItems[1].Enabled = enable;
            cmdHightlight.ContextMenu.MenuItems[2].Enabled = enable;
            cmdHightlight.ContextMenu.MenuItems[3].Enabled = enable;
            cmdHightlight.ContextMenu.MenuItems[4].Enabled = enable;

        }

        private void mnuContextGoto_Popup(object sender, EventArgs e)
        {
            if (lstBookmarks.SelectedItem != null)
            {
                cmdGoto.ContextMenu.MenuItems[1].Enabled = (lstBookmarks.Items.Count > 0 &&
                                                ((BookmarkItem)lstBookmarks.SelectedItem).NumberEntry > 1) ? true : false;

                cmdGoto.ContextMenu.MenuItems[0].Enabled = (lstBookmarks.Items.Count > 0) ? true : false;
            }
            else
            {
                cmdGoto.ContextMenu.MenuItems[1].Enabled = false;
                cmdGoto.ContextMenu.MenuItems[0].Enabled = false;
            }
        }

        /// <summary>
        /// Create a Menu Item for button.
        /// </summary>
        /// <param name="mnuContext"></param>
        /// <param name="menuName"></param>
        /// <returns></returns>
        private MenuItem CreateMenuItem(ContextMenu mnuContext, string menuName)
        {
            MenuItem mnuItem = new MenuItem();
            mnuItem.Text = menuName;
            mnuContext.MenuItems.Add(mnuItem);
            mnuItem.Enabled = false;
            return mnuItem;
        }

        /// <summary>
        /// Remove Item in ListBox and set Selected Item.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private void RemoveListItem(ListBox list, int index)
        {
            if (list.Items.Count > 1)
            {
                if (index == 0)
                    list.SelectedIndex = index + 1;
                if (index == list.Items.Count - 1)
                    list.SelectedIndex = index - 1;

            }

            list.Items.RemoveAt(index);

        }

        private void DecorateTreeView()
        {
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            if (templateInfo != null && templateInfo.RightPanel != null)
            {
                ProntoDocMarkup prontoDocMarkup = templateInfo.RightPanel.Control as ProntoDocMarkup;
                if (prontoDocMarkup != null)
                    prontoDocMarkup.DecorateTreeView();
            }
        }

        private void EnableComboboxDomain()
        {
            if (lstBookmarks.Items.Count <= 0)
                mainManager.MainService.BookmarkService.EnableDomain();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBookmarkName_KeyUp(object sender, KeyEventArgs e)
        {
            object selectItem = null;

            if (lstBookmarks.Items.Count > 0)
            {
                foreach (BookmarkItem item in lstBookmarks.Items)
                {
                    // find equal
                    if (item.DisplayName.ToLower().Equals(txtBookmarkName.Text.ToLower()))
                    {
                        lstBookmarks.SelectedItem = item;
                        return;
                    }

                    bool isMatch = (rdbStartWith.Checked && item.DisplayName.ToLower().StartsWith(txtBookmarkName.Text.ToLower())) ||
                        (rdbContain.Checked && item.DisplayName.ToLower().Contains(txtBookmarkName.Text.ToLower()));
                    if (isMatch)
                        selectItem = (selectItem == null) ? item : selectItem;
                }
                lstBookmarks.SelectedItem = selectItem;
            }
        }

        private void cmdDelete_ButtonClick(object sender, EventArgs e)
        {
            DeleteOneClick();
        }

        private void DeleteOneClick()
        {
            if (lstBookmarks.Items.Count > 0 && lstBookmarks.SelectedItem != null)
            {
                BookmarkItem selectedBookmark = lstBookmarks.SelectedItem as BookmarkItem;
                mainManager.MainService.BookmarkService.DeleteOneBookmark(selectedBookmark);

                if (selectedBookmark.NumberEntry == 1)
                {
                    RemoveListItem(lstBookmarks, lstBookmarks.SelectedIndex);
                }
                else
                    selectedBookmark.NumberEntry -= 1;

                EnableComboboxDomain();
                DecorateTreeView();
                mainManager.RibbonManager.UpdateUI(EventType.VisiablePreviewOsql);
            }
        }

        private void cmdGoto_ButtonClick(object sender, EventArgs e)
        {
            GoToNextClick();
        }

        private void GoToNextClick()
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.GoNextBookmark(bm);
            }
        }

        private void HighLightClick()
        {
            if (lstBookmarks.SelectedIndex > -1)
            {
                BookmarkItem bm = lstBookmarks.SelectedItem as BookmarkItem;
                if (bm != null)
                    mainManager.MainService.BookmarkService.HighLightNextBookmark(bm);
            }
        }

        private void cmdHightlight_ButtonClick(object sender, EventArgs e)
        {
            HighLightClick();


        }

        private void cmdHightlight_MouseClick(object sender, MouseEventArgs e)
        {
            HighLightClick();
        }
    }
}
