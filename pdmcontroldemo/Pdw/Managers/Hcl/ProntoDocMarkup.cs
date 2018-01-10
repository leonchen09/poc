
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.CoreObject.DataSegment;
using Pdw.Core;
using Pdw.Core.Kernal;
using Pdw.Managers.Context;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Managers.Hcl
{

    public partial class ProntoDocMarkup : UserControl
    {
        private MainManager _mainManager;
        private ToolTip _domainToolTip;
        private const int DefaultWidth = 260;
        private bool _hasException = false;
        private readonly int _heightOfMainPanel;
        private readonly int _heightClassiferPanel;
        private readonly int _heightPdeFilePanel;
        private readonly int _heightComboDomainPanel;
        private readonly string _showAll = "All";
        public string LastCurrentDomain { get; set; }

        public ProntoDocMarkup()
        {
            InitializeComponent();
            this.trvDomain.ImageIndex = 0;

            this.trvDomain.ImageList = this.imgIcon;
            this.trvDomain.SelectedImageIndex = 0;
            lblUSC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.24f, FontStyle.Regular);

            cboDomain.Text = Properties.Resources.DomainComboBoxText;
            btnRemove.Enabled = false;

            _mainManager = new MainManager();
            _mainManager.RegisterEvents();

            _domainToolTip = new ToolTip();

            //Load Domain data
            LoadDomain();

            // cheat for display combobox
            cplDomain.Collapse = true;
            cplDomain.Collapse = false;

            _heightPdeFilePanel = pnlPdeFile.Height;
            _heightClassiferPanel = pnlClassifer.Height;
            _heightOfMainPanel = pnlDomainCombo.Height;
            _heightComboDomainPanel = pnlDomainComboDomain.Height;

            CalculateBizDomainPanel();
        }

        #region Events

        #region button change color
        private void btnChangeColor_Click(object sender, EventArgs e)
        {
            if (btnChangeColor.BackColor == Color.Gold)
            {
                SetPropertiesToBizDomainControl();
                CalculateBizDomainPanel();
                if (cboClassifier.Items.Count > 0)
                    VisibleComboboxClassifier(true);
            }
            else
            {
                SetPropertiesToPdeControl();
                VisibleComboboxClassifier(false);
                CalculatePdeImportPanel();
            }
        }
        #endregion

        #region Resizable events (Collapse/Expand)
        private void cplDomain_CollapseExpand(EventArgs e)
        {
            MakePosition();
        }

        private void cplException_CollapseExpand(EventArgs e)
        {
            MakePosition();
        }

        private void cplCondition_CollapseExpand(EventArgs e)
        {
            MakePosition();
        }
        #endregion

        #region Change domain events (change value in combobox, double click on node in tree)
        private void cboClassifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDomain.Enabled)
            {
                if (btnChangeColor.BackColor == Color.Green)
                    SetPropertiesToBizDomainControl();
            }
        }

        /// <summary>
        /// Chose another domain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboDomain_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboDomain.Text))
                {
                    if (btnChangeColor.BackColor == Color.Green)
                    {
                        LoadDomainData();
                        Wkl.MainCtrl.CommonCtrl.CommonProfile.UserData.LastDomain = cboDomain.Text;
                        trvDomain.SetSelectedNode(0);
                    }
                    else
                    {
                        //Bind sheet to combobox
                        BindPdeSheetToCombobox();

                        //Bind data to tree
                        BindPdeTree();
                    }
                }
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ChangeDomainError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_ChangeDomainError,
                    MessageUtils.Expand(Properties.Resources.ipe_ChangeDomainError, ex.Message), ex.StackTrace);

                LogUtils.LogManagerError(mgrExp);
            }
        }

        private void cboDomain_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Markup a property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvDomain_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (btnChangeColor.BackColor == Color.Green)
                DoubleClickItemForBizDomain(e);
            else
                DoubleClickItemForPde(e);
        }
        #endregion

        #region Drop into condition textbox
        private void txtUSCValue_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.Text))
                {
                    txtUSCValue.Text += string.Format(Constants.ProntoDocMarkup.FieldFormat,
                        e.Data.GetData(DataFormats.Text).ToString().Replace("]", "]]"));
                }
            }
            catch { }
        }

        private void txtUSCValue_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
        #endregion

        #region Drag node in domain treeview
        private void trvDomain_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode dragNode = e.Item as TreeNode;
            if (dragNode != null)
            {
                DoDragDrop(dragNode.Text, DragDropEffects.Copy);
            }
        }
        #endregion

        #region User Specificed Condition
        /// <summary>
        /// enable or disable add conditon button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUSCValue_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUSCValue.Text.Trim()))
                btnAdd.Enabled = false;
            else
                btnAdd.Enabled = true;
        }

        /// <summary>
        /// Remove User Specificed Condition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            //1.If a condition is selected
            //    1.1 delete from list.
            //    1.2 delete from properties.  
            //    1.3 add to status
            //2.Else: do not delete.
            //3.If no items, enable combobox Domain.
            try
            {
                if (lstUSC.Items.Count > 0)
                {
                    if (lstUSC.SelectedItem != null)
                    {
                        lstUSC.Items.Remove(lstUSC.SelectedItem);

                        btnRemove.Enabled = false;
                        txtUSCName.Text = string.Empty;
                        txtUSCValue.Text = string.Empty;

                        List<USCItem> listUSC = BindingToList();
                        _mainManager.MainService.PropertyService.UpdateUscItemCollection(listUSC);
                    }
                }
                else
                    _mainManager.MainService.BookmarkService.EnableDomain();

                DecorateTreeView();
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_RemoveUSCItemError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_RemoveUSCItemError,
                    MessageUtils.Expand(Properties.Resources.ipe_RemoveUSCItemError, ex.Message), ex.StackTrace);

                LogUtils.LogManagerError(mgrExp);
            }
        }

        /// <summary>
        /// Add condition into bookmark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //----------------------------------------
            //1.If textbox name is blank -> message
            //2.Else: If Name is existed -> message
            //3.Else if: Expression is not valid -> message
            //4.Else all: 
            //   4.1: Add to ListBox
            //   4.2: Add to status
            //-----------------------------------------
            try
            {
                ContextManager contextManager = new ContextManager();
                // string domainName = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.DomainName;
                string domainName = cboDomain.Text;
                Dictionary<string, USCItem> listItemsInTree =
                    Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName).FieldLists;

                //1.
                if (string.IsNullOrEmpty(txtUSCName.Text))
                    contextManager.UpdateStatus(MessageUtils.Expand(Properties.Resources.ipm_M001,
                        Properties.Resources.ipe_M001));
                else
                {
                    //2.
                    if (CheckExistedCondition(txtUSCName.Text, lstUSC, listItemsInTree))
                        contextManager.UpdateStatus(MessageUtils.Expand(Properties.Resources.ipm_M001,
                            Properties.Resources.ipe_M002));
                    else
                    {
                        //3.
                        string errorMessage = string.Empty;
                        USCItem item = _mainManager.MainService.PropertyService.CreateUscItem(txtUSCName.Text,
                            txtUSCValue.Text, listItemsInTree, ref errorMessage);
                        item.DomainName = domainName;

                        if (!string.IsNullOrWhiteSpace(errorMessage))
                            contextManager.UpdateStatus(errorMessage);
                        else//4.
                        {
                            //Update UniqueName for UscItem
                            string key;
                            IntegrationServiceProfile inteProfile =
                                Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key).IntegrationService;
                            inteProfile.UscItem = item;
                            _mainManager.MainService.PropertyService.UpdateUniqueNameForUscItem(key);

                            lstUSC.DisplayMember = Constants.ProntoDocMarkup.DisplayInListBox;
                            lstUSC.Items.Add(item);

                            contextManager.UpdateStatus(MessageUtils.Expand(Properties.Resources.ipm_M004,
                                item.BusinessName));

                            List<USCItem> listUSC = BindingToList();

                            _mainManager.MainService.PropertyService.UpdateUscItemCollection(listUSC);
                            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
                        }
                    }
                }

                EnabledComboboxDomain(false);
                EnabledComboboxClassifier(false);

                DecorateTreeView();
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_AddUSCItemError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_AddUSCItemError,
                    MessageUtils.Expand(Properties.Resources.ipe_AddUSCItemError, ex.Message), ex.StackTrace);

                LogUtils.LogManagerError(mgrExp);
            }
        }
        #endregion

        #region add document condition
        private void lstUSC_DoubleClick_1(object sender, EventArgs e)
        {
            //1.Add to bookmark: Add text to document and Add to internal bookmark.
            //2.Add to object.

            try
            {
                if (lstUSC.SelectedItem != null)
                {
                    USCItem item = (USCItem)lstUSC.SelectedItem;

                    string bookmarkKey = DateTime.Now.ToString(ProntoMarkup.BookmarkKeyFormat);
                    string bookmarkName = item.BusinessName;
                    bookmarkKey = bookmarkKey + "Sys"; // using for highlight in raw-mode
                    InternalBookmarkItem bm = new InternalBookmarkItem(bookmarkKey, bookmarkName,
                        item.SQLExpression, DSIconType.USC.ToString(), XsltType.If);
                    bm.TableIndex = -1;//Set -1 to build osql for special item
                    AddBookmark(bm);
                    btnRemove.Enabled = false;
                }
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_TagUSCItemError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_TagUSCItemError,
                    MessageUtils.Expand(Properties.Resources.ipe_TagUSCItemError, ex.Message), ex.StackTrace);

                LogUtils.LogManagerError(mgrExp);
            }
        }

        private void lstUSC_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (lstUSC.SelectedItem != null)
                {
                    USCItem item = lstUSC.SelectedItem as USCItem;
                    txtUSCName.Text = item.BusinessName;
                    txtUSCValue.Text = item.SQLExpression;
                    btnRemove.Enabled = !CheckUSCTagged((lstUSC.SelectedItem as USCItem).BusinessName);
                }
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SelectUSCItemError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SelectUSCItemError,
                    MessageUtils.Expand(Properties.Resources.ipe_SelectUSCItemError, ex.Message), ex.StackTrace);

                LogUtils.LogManagerError(mgrExp);
            }
        }
        #endregion

        #region splitter
        int pnlConditionHeight = 0;
        private void splitter2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            try
            {
                lstUSC.Height = pnlListUSC.Height + (lstUSC.ItemHeight - pnlListUSC.Height % lstUSC.ItemHeight);
                pnlConditionHeight = pnlCondition.Height;
            }
            catch { }
        }

        private void splitterInsideConditionTop_SplitterMoved(object sender, SplitterEventArgs e)
        {
            try
            {
                int heightChanged = pnlName.Height - 20;

                pnlName.Height = 20;

                if (heightChanged > 0) // decrease height of list condition
                {
                    int temp = pnlListUSC.Height - heightChanged;

                    if (temp > lstUSC.ItemHeight * 2)
                        pnlConditionExpression.Height += heightChanged;
                }
                else
                {
                    int temp = pnlConditionExpression.Height + heightChanged;
                    if (temp >= lstUSC.ItemHeight * 2)
                        pnlConditionExpression.Height = temp;
                }

                lstUSC.Height = pnlListUSC.Height + (lstUSC.ItemHeight - pnlListUSC.Height % lstUSC.ItemHeight);
            }
            catch { }
        }
        #endregion

        #region using for display image in combobox
        /// <summary>
        /// overwrite event DrawItem for add image.
        /// must be set: 
        /// - DrawMode = OwnerDrawVariable (for event raise event DrawItem)
        /// - DropDownStype = DropDownList (for display image in editable textbox)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboDomain_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();

                int indexItem = e.Index;
                if (indexItem < 0 || indexItem >= cboDomain.Items.Count)
                    return;

                string text = cboDomain.Items[indexItem].ToString();
                bool isAddSection = false;
                try
                {
                    DataSegmentHelper.GetListDomain();
                    isAddSection = Wkl.MainCtrl.CommonCtrl.CommonProfile.ListDomains[text];
                }
                catch { }

                int iconHeight = 12;
                if (isAddSection)
                {
                    Image icon = Properties.Resources.DomainAddSection;
                    e.Graphics.DrawImage(icon, e.Bounds.X, e.Bounds.Y + 1, iconHeight, iconHeight);
                }
                e.Graphics.DrawString(text, cboDomain.Font, System.Drawing.Brushes.Black,
                    new RectangleF(e.Bounds.X + iconHeight, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));

                e.DrawFocusRectangle();
            }
            catch { }
        }
        #endregion

        #region events control with pde
        private void chkUseInPdw_CheckedChanged(object sender, EventArgs e)
        {
            BindPdeTree();
        }

        private void cboSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPdeTree();
        }
        #endregion

        #endregion

        #region Helper methods

        /// <summary>
        /// To Prevent Flicker Control
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        /// <summary>
        /// Set default controls what are displayed
        /// </summary>
        private void SetDefaultControl()
        {
            if (!_hasException)
            {
                pnlException.Visible = false;
                pnlDomain.Dock = DockStyle.Fill;
                splitter1.Visible = false;
            }
            else
            {
                pnlException.Visible = true;
                pnlDomain.Dock = DockStyle.Top;
                pnlException.Dock = DockStyle.Fill;
                splitter1.Visible = true;
            }

            cplDomain.Collapse = false;
            cplCondition.Collapse = true;
            cplException.Collapse = true;

            MakePosition();
        }

        /// <summary>
        /// Load all domains and bind to combobox
        /// </summary>
        private void LoadDomain()
        {
            // load data segment
            DataSegmentHelper.GetListDomain();

            // update image list icons
            UpdateImageListCustomerIcons();

            // load template info
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CreateTemplateInfo(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.FullName);
            templateInfo.FullDocName = Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.FullName;

            //Set default domain
            UserData userData = _mainManager.MainService.PropertyService.GetLastSelectedDomain();
            Wkl.MainCtrl.CommonCtrl.CommonProfile.UserData = (userData != null) ? userData : (new UserData());

            // Load UserData
            if (Wkl.MainCtrl.CommonCtrl.CommonProfile.UserData == null)
                Wkl.MainCtrl.CommonCtrl.CommonProfile.UserData = _mainManager.MainService.PropertyService.GetLastSelectedDomain();

            LoadClassifierIntoCombobox();
            LoadDomainIntoComboBox(null);

            string lastUserDomain = Wkl.MainCtrl.CommonCtrl.CommonProfile.UserData.LastDomain;
            if (!string.IsNullOrWhiteSpace(lastUserDomain) && cboDomain.Items.Contains(lastUserDomain))
                cboDomain.Text = lastUserDomain;
            else//2.2
            {
                cboDomain.SelectedItem = (cboDomain.Items.Count > 0) ? cboDomain.Items[0] : null;

                //set default font
                Wkl.MainCtrl.CommonCtrl.CommonProfile.UserData.LastDomain = cboDomain.Text;
                Wkl.MainCtrl.CommonCtrl.CommonProfile.UserData.LastFontSize = 8;
            }
        }

        private void UpdateImageListCustomerIcons()
        {
            if (Wkl.MainCtrl.CommonCtrl.CommonProfile.DataSegmentInfo != null &&
                Wkl.MainCtrl.CommonCtrl.CommonProfile.DataSegmentInfo.Icons != null)
            {
                List<ProntoDoc.Framework.CoreObject.DataSegment.Icon> icons =
                    Wkl.MainCtrl.CommonCtrl.CommonProfile.DataSegmentInfo.Icons;
                foreach (var icon in icons)
                {
                    if (icon != null && icon.Data != null)
                    {
                        IconInfo iconInfo = new IconInfo();
                        iconInfo.DBID = icon.DBID;
                        iconInfo.CustomerIconID = icon.CustomIconID;

                        string key = iconInfo.Key;
                        if (!imgIcon.Images.ContainsKey(key))
                        {
                            iconInfo.Index = imgIcon.Images.Count;
                            using (System.IO.MemoryStream imgStream = new System.IO.MemoryStream(icon.Data))
                            {
                                Image img = Image.FromStream(imgStream);
                                imgIcon.Images.Add(key, img);
                            }
                            Wkl.MainCtrl.CommonCtrl.CommonProfile.AddIconInfo(iconInfo);
                        }
                    }
                }
            }
        }

        private void LoadClassifierIntoCombobox()
        {
            //bool isToggled = bool.TrueString.Equals(picToggleDomain.Tag.ToString(), StringComparison.OrdinalIgnoreCase);
            
            bool isAll = string.Compare(cboClassifier.Text, _showAll, StringComparison.OrdinalIgnoreCase) == 0;
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;

            // make sure a new template can use all classifier, domain
            if (!isAll)
            {
                if (templateInfo.InternalBookmark.InternalBookmarkDomains == null ||
                    templateInfo.InternalBookmark.InternalBookmarkDomains.Count == 0)
                    isAll = true;
            }

            string oldClassifier = cboClassifier.Visible ? cboClassifier.Text : string.Empty;
            List<string> classifiers = new List<string>();
            if (isAll)
            {
                classifiers = Wkl.MainCtrl.CommonCtrl.CommonProfile.Classifiers.Keys.ToList();
                classifiers.Add(_showAll);                
            }
            else
            {
                foreach (InternalBookmarkDomain ibmDomain in templateInfo.InternalBookmark.InternalBookmarkDomains)
                {
                    string domainName = ibmDomain.DomainName;
                    DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName);
                    string classifier = domainInfo.DSDomainData.Classifier;
                    classifier = !string.IsNullOrWhiteSpace(classifier) ? domainInfo.DSDomainData.Classifier : string.Empty;
                    if (!classifiers.Exists(c => c == classifier))
                        classifiers.Add(domainInfo.DSDomainData.Classifier);
                }
            }

            cboClassifier.Items.Clear();
            if (classifiers.Count == 0 || (classifiers.Count == 2 && classifiers[0] == string.Empty))
                VisibleComboboxClassifier(false);
            else
            {
                VisibleComboboxClassifier(true);
                cboClassifier.Items.AddRange(classifiers.ToArray());
                if (classifiers.Exists(c => c == oldClassifier))
                    cboClassifier.Text = oldClassifier;
            }
        }

        private void LoadDomainIntoComboBox(string classifier)
        {
            // 1. Toggled (all)
            // 1.1. Load all classifier
            // 1.2. Load all domains belong to classifier
            // 2. Un-toggle
            // 2.1. Load all classifier of using domains
            // 2.2. Load all using domains belong to classifier
            //bool isToggled = bool.TrueString.Equals(picToggleDomain.Tag.ToString(), StringComparison.OrdinalIgnoreCase);
            bool isAll = true;
            if(cboClassifier.Items.Count > 0)
                isAll = string.Compare(cboClassifier.Text, _showAll, StringComparison.OrdinalIgnoreCase) == 0;

            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;

            // make sure a new template can use all classifier, domain
            if (!isAll)
            {
                if (templateInfo.InternalBookmark.InternalBookmarkDomains == null ||
                    templateInfo.InternalBookmark.InternalBookmarkDomains.Count == 0)
                    isAll = true;
            }

            List<string> domainNames = new List<string>();
            string oldDomain = cboDomain.Text;
            if (isAll)
                domainNames = Wkl.MainCtrl.CommonCtrl.CommonProfile.ListDomains.Keys.ToList();
            else
                domainNames = templateInfo.InternalBookmark.InternalBookmarkDomains.Select(d => d.DomainName).ToList();

            if (!string.IsNullOrEmpty(classifier) && string.Compare(classifier, _showAll) != 0)
            {
                int count = domainNames.Count;
                for (int index = count - 1; index > -1; index--)
                {
                    string domainName = domainNames[index];
                    DomainInfo domain = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName);
                    string tempClassifier = Wkl.MainCtrl.CommonCtrl.CommonProfile.FindClassifier(domainName);
                    if (tempClassifier != classifier)
                        domainNames.RemoveAt(index);
                }
            }

            cboDomain.Items.Clear();
            cboDomain.Items.AddRange(domainNames.ToArray());
            if (domainNames.Exists(d => d == oldDomain))
                cboDomain.Text = oldDomain;
            else if (domainNames.Count > 0)
                cboDomain.Text = domainNames[0];

            if (string.IsNullOrEmpty(classifier))
            {
                string domainClassifier = Wkl.MainCtrl.CommonCtrl.CommonProfile.FindClassifier(cboDomain.Text);
                domainClassifier = domainClassifier == null ? string.Empty : domainClassifier;
                if (cboClassifier.Items.Contains(domainClassifier))
                    cboClassifier.Text = domainClassifier;
            }
        }

        private void VisibleComboboxClassifier(bool isVisible)
        {
            cboClassifier.Visible = isVisible;
            if (isVisible)
            {
                pnlDomainComboDomain.Location = new Point(2, 23);
                pnlDomainComboDomain.Parent.Height = pnlDomainComboDomain.Height + cboClassifier.Height + 2;
            }
            else
            {
                pnlDomainComboDomain.Location = new Point(2, 0);
                pnlDomainComboDomain.Parent.Height = pnlDomainComboDomain.Height;
            }
        }

        /// <summary>
        /// Load domain data and bind to tree
        /// </summary>
        private void LoadDomainData()
        {
            trvDomain.Clear();

            //Load data from datasegment to Wkl
            DataSegmentHelper.LoadDomainData(cboDomain.Text);

            //Binding to TreeNodes
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            templateInfo.AddDomainName(cboDomain.Text);
            templateInfo.TreeViewData = trvDomain;
            templateInfo.SelectedDomainName = cboDomain.Text;
            _mainManager.MainService.TemplateService.BindTreeViewAndUpdateInternalBM(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc.FullName);

            SetFontSize(Wkl.MainCtrl.CommonCtrl.CommonProfile.UserData.LastFontSize);

            trvDomain.ExpandAll();
            trvDomain.ShowNodeToolTips(true);

            // set tooltip for domain combobox
            DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(cboDomain.Text);
            _domainToolTip.SetToolTip(cboDomain, domainInfo.DSDomainData.ToolTip);

            // load condition
            LoadAllConditions();

            //// set classifier
            //if (!string.IsNullOrWhiteSpace(domainInfo.DSDomainData.Classifier))
            //{
            //    cboClassifier.SelectedIndexChanged -= new EventHandler(cboClassifier_SelectedIndexChanged);
            //    if (!cboClassifier.Items.Contains(domainInfo.DSDomainData.Classifier))
            //        cboClassifier.Items.Add(domainInfo.DSDomainData.Classifier);
            //    cboClassifier.Text = domainInfo.DSDomainData.Classifier;
            //    cboClassifier.SelectedIndexChanged += new EventHandler(cboClassifier_SelectedIndexChanged);
            //}

            DecorateTreeView();
        }

        #region decorate treeview
        public void SetFontSize(int fontSize)
        {
            DecorateTreeView(fontSize);
        }

        public void DecorateTreeView()
        {
            Dictionary<string, bool> dataTagFields = null;
            Dictionary<string, bool> docSpecConditionFields = null;
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            if (templateInfo != null)
            {
                InternalBookmark ibm = templateInfo.InternalBookmark;
                if (ibm != null)
                {
                    InternalBookmarkDomain ibmDomain = ibm.GetInternalBookmarkDomain(cboDomain.Text);
                    dataTagFields = GetDataTagFields(ibmDomain);
                }
            }
            docSpecConditionFields = GetDocSpecConditionFields();

            DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(cboDomain.Text);
            if (domainInfo != null && domainInfo.MapBizTreeNodeNames != null)
                DecorateTreeView(dataTagFields, docSpecConditionFields);
        }

        private Dictionary<string, bool> GetDataTagFields(InternalBookmarkDomain ibmDomain)
        {
            Dictionary<string, bool> dataTagFields = new Dictionary<string, bool>();
            if (ibmDomain == null || ibmDomain.InternalBookmarkItems == null || ibmDomain.InternalBookmarkItems.Count == 0)
                return dataTagFields;

            // get word bookmark
            string key = string.Empty;
            ServicesProfile servicePro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);
            _mainManager.MainService.BookmarkService.GetBookmarkCollection(key);
            Dictionary<string, string> wBms = servicePro.ContentService.GetBookmarks_OListBM;

            // get biz name
            foreach (string bmName in wBms.Keys)
            {
                InternalBookmarkItem ibmItem = ibmDomain.InternalBookmarkItems.FirstOrDefault(c => c.Key == bmName);
                if (ibmItem == null)
                    continue;
                if (!bmName.Contains(ProntoMarkup.KeyForeach))
                {
                    string bizName = MarkupUtilities.GetOriginalBizName(ibmItem.Key, ibmItem.BizName);
                    if (!dataTagFields.ContainsKey(bizName))
                        dataTagFields.Add(bizName, false);
                }
            }

            return dataTagFields;
        }

        private Dictionary<string, bool> GetDocSpecConditionFields()
        {
            Dictionary<string, bool> docSpecConditionFields = new Dictionary<string, bool>();

            List<USCItem> uscItems = BindingToList();
            if (uscItems != null)
            {
                foreach (USCItem uscItem in uscItems)
                {
                    if (uscItem.Fields != null)
                    {
                        foreach (USCItem field in uscItem.Fields)
                        {
                            string bizName = field.BusinessName;
                            if (!docSpecConditionFields.ContainsKey(bizName))
                                docSpecConditionFields.Add(bizName, false);
                        }
                    }
                }
            }

            return docSpecConditionFields;
        }

        /// <summary>
        /// decorate the tree with rulwe:
        /// 1. in dataTags => bold
        /// 2. in dscFields => underline
        /// </summary>
        /// <param name="dataTags"></param>
        /// <param name="dscFields"></param>
        private void DecorateTreeView(Dictionary<string, bool> dataTags, Dictionary<string, bool> dscFields)
        {
            #region validate
            if ((dataTags == null || dataTags.Count == 0) && (dscFields == null || dscFields.Count == 0))
                return;

            if (trvDomain.AllNodes == null || trvDomain.AllNodes.Count == 0)
                return;
            #endregion

            foreach (TreeNode trnNode in trvDomain.AllNodes)
            {
                string bizName = trnNode.Text;
                Font font = trnNode.NodeFont == null ? trvDomain.TreeTop.Font : trnNode.NodeFont;
                FontStyle style = FontStyle.Regular;

                // data tags
                if (dataTags != null && dataTags.ContainsKey(bizName))
                    style = style | FontStyle.Bold;

                // dsc fields
                if (dscFields != null && dscFields.ContainsKey(bizName))
                    style = style | FontStyle.Underline;

                // apply new font
                trnNode.NodeFont = new Font(font, style);
            }

            if (trvDomain.TreeTop.Nodes.Count > 0)
            {
                // cheat for make sure enough width
                trvDomain.TreeTop.Nodes[0].Text += string.Empty;
                if (trvDomain.TreeBottom.Nodes.Count > 0)
                    trvDomain.TreeBottom.Nodes[0].Text += string.Empty;
            }
        }

        private void DecorateTreeView(int fontSize)
        {
            if (fontSize <= 0)
                return;

            foreach (TreeNode trnNode in trvDomain.AllNodes)
            {
                string bizName = trnNode.Text;
                FontStyle style = trnNode.NodeFont == null ? trvDomain.TreeTop.Font.Style : trnNode.NodeFont.Style;
                string name = trnNode.NodeFont == null ? trvDomain.TreeTop.Font.Name : trnNode.NodeFont.Name;

                // apply new font
                trnNode.NodeFont = new Font(name, fontSize, style);
            }

            if (trvDomain.TreeTop.Nodes.Count > 0)
            {
                // set item height for make sure enough height
                trvDomain.TreeTop.ItemHeight = fontSize + 10;
                trvDomain.TreeBottom.ItemHeight = fontSize + 10;
                trvDomain.TreeTop.Nodes[0].Expand();

                // cheat for make sure enough width
                trvDomain.TreeTop.Nodes[0].Text += string.Empty;
                if (trvDomain.TreeBottom.Nodes.Count > 0)
                    trvDomain.TreeBottom.Nodes[0].Text += string.Empty;
            }
        }
        #endregion

        #region User Specificed Condition
        /// <summary>
        /// Load all User Specificed Conditions.
        /// </summary>
        private void LoadAllConditions()
        {
            //1.Get all Conditions from Custom Properties
            //2.Validate Fields
            //3.Validate Express of each condition
            //4.Add to list.

            InternalBookmark iBm = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.InternalBookmark;
            DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(cboDomain.Text);
            Dictionary<string, USCItem> listItems = domainInfo.FieldLists;

            lstUSC.Items.Clear();
            txtUSCName.Text = string.Empty;
            txtUSCValue.Text = string.Empty;
            if (iBm != null && iBm.InternalBookmarkDomains != null)
            {
                foreach (InternalBookmarkDomain ibmDomain in iBm.InternalBookmarkDomains)
                {
                    bool isvalid = _mainManager.MainService.BookmarkService.ValidateFieldsInCondition(ibmDomain.USCItems, listItems);
                    foreach (USCItem item in ibmDomain.USCItems)
                        lstUSC.Items.Add(item);
                }
            }

            lstUSC.DisplayMember = Constants.ProntoDocMarkup.DisplayInListBox;
        }
        #endregion

        #region Update exceptions panel when compare wbms vs ibms
        /// <summary>
        /// list bookmark collection that exist on word bookmark collection but 
        /// not exist on internal bookmark collection on exception list
        /// </summary>
        /// <param name="bms"></param>
        public void UpdateExceptionList(List<string> bms, bool isDomainMatchWithDataSegment, string domainName)
        {
            if (bms.Count > 0)
            {
                _hasException = true;
                foreach (string bm in bms)
                    SetTextWithColorInException(rtbException, bm);
            }
            else
                _hasException = false;

            SetDefaultControl();

            // update combobox
            if (!string.IsNullOrEmpty(domainName))
            {
                cboDomain.Text = domainName;
                EnabledComboboxDomain(false);
                EnabledComboboxClassifier(false);

                // load data for USC
                if (lstUSC.Items.Count <= 0)
                    LoadAllConditions();
            }
        }
        #endregion

        /// <summary>
        /// Set position for Domain, Exception, Condition section.
        /// </summary>
        private void MakePosition()
        {
            int headerHeight = pnlConditionHeader.Height;
            int splitterHeight = 2;
            // int minimizeConditionHeight = 130;
            int minimizeConditionHeight = Math.Max(130, pnlConditionHeight); // change to keep previous height of conditon panel

            if (!_hasException)
            {
                if (cplCondition.Collapse)
                    pnlCondition.Height = headerHeight;
                else
                    pnlCondition.Height = (cplDomain.Collapse) ? pnlFullControl.Height - headerHeight - splitterHeight : minimizeConditionHeight;

                cplDomain.Height = pnlDomain.Height;
            }
            else
            {
                if (cplDomain.Collapse)
                {
                    if (cplCondition.Collapse)
                    {
                        pnlDomain.Height = (cplException.Collapse) ? pnlFullControl.Height - headerHeight * 2 - 2 * splitterHeight : headerHeight;
                        pnlCondition.Height = headerHeight;
                    }
                    else
                    {
                        pnlDomain.Height = headerHeight;
                        pnlCondition.Height = (cplException.Collapse) ? (pnlFullControl.Height - headerHeight * 2) : minimizeConditionHeight;
                    }
                }
                else
                {
                    if (cplCondition.Collapse)
                    {
                        pnlDomain.Height = (cplException.Collapse) ? pnlFullControl.Height - headerHeight * 2 - 2 * splitterHeight :
                            (pnlFullControl.Height - headerHeight - 2 * splitterHeight) / 2;
                        pnlCondition.Height = headerHeight;
                    }
                    else
                    {
                        pnlCondition.Height = minimizeConditionHeight;
                        pnlDomain.Height = (cplException.Collapse) ? (pnlFullControl.Height - headerHeight) / 2 : (pnlFullControl.Height - pnlCondition.Height) / 2;
                    }
                }
                cplException.Height = pnlException.Height;
            }

            cplCondition.Height = pnlCondition.Height;

            // make sure lstUSC.Height = int * lstUSC.ItemHeight
            lstUSC.Height = pnlListUSC.Height + (lstUSC.ItemHeight - pnlListUSC.Height % lstUSC.ItemHeight);
        }

        /// <summary>
        /// Set Height for this control when first load.
        /// </summary>
        /// <param name="height"></param>
        public void SetHeightControl(int height)
        {
            this.Height = height - 23;
            SetDefaultControl();
        }

        /// <summary>
        /// Add bookmark into word document (word object and internal bookmark)
        /// </summary>
        /// <param name="item"></param>
        private void AddBookmark(InternalBookmarkItem item)
        {
            string key;
            ServicesProfile serviceProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);

            // validate position of data tag with foreach tags
            string message = ValidateDataTagPosition(key, cboDomain.Text);
            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
                return;
            }

            // add bookmark into internal bookmark
            IntegrationServiceProfile integrationPro = serviceProfile.IntegrationService;
            integrationPro.AddInternalBM_IBookmark = item;
            _mainManager.MainService.PropertyService.AddInternalBookmark(key);

            // add bookmark into work bookmark
            serviceProfile.ContentService.AddBookmark_IBookmark = item;
            _mainManager.MainService.BookmarkService.AddBookmark(key);

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }

        private string ValidateDataTagPosition(string key, string domainName)
        {
            ServicesProfile serviceProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            serviceProfile.ContentService.DomainName = domainName;
            _mainManager.MainService.BookmarkService.ValidateDataTagPosition(key);
            if (!serviceProfile.ContentService.IsValid)
                return serviceProfile.Message.ToString();

            return string.Empty;
        }

        /// <summary>
        /// Check existed Condition in list.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="listCondition"></param>
        /// <returns></returns>
        private bool CheckExistedCondition(string condition, ListBox listCondition, Dictionary<string, USCItem> listFields)
        {
            //1. Check exist in list USC
            if (listCondition != null && listCondition.Items.Count > 0)
                foreach (USCItem item in listCondition.Items)
                    if (item.BusinessName.Equals(condition))
                        return true;
            //2. Check exist in Domain Tree
            if (listFields != null && listFields.Count > 0 && listFields.ContainsKey(condition.ToLower()))
                return true;

            return false;
        }

        private List<USCItem> BindingToList()
        {
            List<USCItem> listCondition = new List<USCItem>();

            if (lstUSC.Items.Count > 0)
                foreach (USCItem item in lstUSC.Items)
                    listCondition.Add(item);

            return listCondition;
        }

        public void EnableDomain()
        {
            if (lstUSC.Items.Count <= 0)
            {
                EnabledComboboxDomain(true);
                if (cboClassifier.Items.Count > 2)
                    EnabledComboboxClassifier(true);
            }
        }

        /// <summary>
        /// Set color for rich text Exception.
        /// </summary>
        /// <param name="richText"></param>
        /// <param name="text"></param>
        private void SetTextWithColorInException(RichTextBox richText, string text)
        {
            string predix = Properties.Resources.ipm_ExceptionErrorPredix;
            if (richText.Text.Length == 0)
            {
                richText.SelectedText = predix + text;
                richText.SelectionStart = predix.Length;
                richText.SelectionLength = text.Length;
                richText.SelectionColor = Color.Maroon;
            }
            else
            {

                int start = richText.Text.Length;
                string appendText = Environment.NewLine + predix;

                richText.AppendText(appendText);

                richText.SelectionStart = start;
                richText.SelectionLength = predix.Length;
                richText.SelectionColor = Color.Black;

                appendText = text;
                richText.AppendText(appendText);
                richText.SelectionStart = start + predix.Length;
                richText.SelectionLength = text.Length;
                richText.SelectionColor = Color.Maroon;
            }
        }

        private bool CheckUSCTagged(string uscName)
        {
            string key;
            ContentServiceProfile contentProfile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key).ContentService;
            _mainManager.MainService.BookmarkService.GetDistinctBookmarks(key);

            if (contentProfile.GetDistinctBM_OListBM.Count > 0)
                foreach (BookmarkItem item in contentProfile.GetDistinctBM_OListBM)
                {
                    if (item.DisplayName.Equals(uscName))
                    {
                        Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
                        return true;
                    }

                }
            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
            return false;
        }

        #region Double click item
        private void DoubleClickItemForBizDomain(TreeNodeMouseClickEventArgs e)
        {
            try
            {
                DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(cboDomain.Text);
                if (!domainInfo.Fields.ContainsKey((string)e.Node.Tag))
                    return;

                DSTreeView item = domainInfo.Fields[(string)e.Node.Tag];

                string bookmarkKey = DateTime.Now.ToString(ProntoMarkup.BookmarkKeyFormat);
                InternalBookmarkItem bm = new InternalBookmarkItem(bookmarkKey, item.Text, item.TechName,
                    item.Type.ToString(), item.TableIndex, item.Relation);
                bm.UniqueName = item.UniqueName;
                bm.DataType = item.DataType;
                bm.DomainName = cboDomain.Text;
                bm.JavaName = item.JavaClause;

                switch (item.Type)
                {
                    case DSIconType.RenderXY:
                        bm.TechName = bm.TechName.Replace("@", "");
                        goto case DSIconType.SystemInfo;
                    case DSIconType.SystemInfo:
                    case DSIconType.UDF:
                    case DSIconType.Field:
                        //HACK:FORM CONTROLS - DoubleClickItemForBizDomain
                        bm.Type = XsltType.Select;

                        if (IsPdmControlBindable())
                        {
                            PdmControlDataBind(bm);
                        }
                        else
                        {
                            AddBookmark(bm);
                        }
                        break;
                    case DSIconType.ForEach: // foreach
                        bm.Type = XsltType.Foreach;
                        AddBookmark(bm);
                        break;
                    case DSIconType.Condition: // condition                   
                        bm.Type = XsltType.If;
                        AddBookmark(bm);
                        break;
                    default:
                        break;
                }

                _mainManager.RibbonManager.UpdateUI(EventType.VisiablePreviewOsql);
                DecorateTreeView();
            }
            catch (BaseException baseExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_AddDataTagError);
                mgrExp.Errors.Add(baseExp);

                LogUtils.LogManagerError(mgrExp);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_AddDataTagError,
                    MessageUtils.Expand(Properties.Resources.ipe_AddDataTagError, ex.Message), ex.StackTrace);

                LogUtils.LogManagerError(mgrExp);
            }
        }

        private void DoubleClickItemForPde(TreeNodeMouseClickEventArgs e)
        {
            string stn = cboDomain.Text;
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            if (templateInfo == null)
                return;
            PdeContent pdeContent = templateInfo.PdeContent;
            if (pdeContent == null || pdeContent.Items == null || pdeContent.Items.Count == 0)
                return;
            PdeContentItem pdeContentItem = pdeContent.Items.FirstOrDefault(
                c => string.Equals(stn, c.STN, StringComparison.OrdinalIgnoreCase));
            if (pdeContentItem == null)
                return;

            PdeDataTagInfo pdeTag = null;
            if (e.Node.Tag is ExportItem)
            {
                ExportItem expItem = e.Node.Tag as ExportItem;
                switch (expItem.MapType)
                {
                    case MapType.SingleCell:
                        pdeTag = TagPdeField(pdeContentItem, expItem);
                        break;
                    case MapType.Chart:
                        pdeTag = TagPdeChart(pdeContentItem, expItem);
                        break;
                    default:
                        break;
                }
            }
            else if (e.Node.Tag is ColumnExportItem)
            {
                ColumnExportItem expColumn = e.Node.Tag as ColumnExportItem;
                pdeTag = TagPdeColumn(pdeContentItem, expColumn);
            }

            if (pdeTag != null)
            {
                if (templateInfo.InternalBookmark == null)
                    templateInfo.InternalBookmark = new InternalBookmark();
                templateInfo.InternalBookmark.PdeDataTagInfos.Add(pdeTag);
            }
        }

        private PdeDataTagInfo TagPdeField(PdeContentItem pdeContentItem, ExportItem expItem)
        {
            string bmName = _mainManager.MainService.PdeService.ImportPdeTag(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc,
                pdeContentItem, expItem.ParentName, expItem.ExcelName, string.Empty, expItem.TreeNodeName);
            return new PdeDataTagInfo(MapType.SingleCell, pdeContentItem.EncodeSTN, 
                string.Empty, expItem.ExcelName, expItem.TreeNodeName, bmName);
        }

        private PdeDataTagInfo TagPdeChart(PdeContentItem pdeContentItem, ExportItem expItem)
        {
            if (expItem.MapType != MapType.Chart || expItem.Chart == null)
                return null;

            string bmName = _mainManager.MainService.PdeService.ImportPdeChart(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc,
                pdeContentItem, expItem.ParentName, expItem.Chart.Name, expItem.Chart.Content);

            return new PdeDataTagInfo(MapType.Chart, 
                pdeContentItem.EncodeSTN, expItem.ParentName, expItem.Chart.Name, expItem.Chart.Name, bmName);
        }

        private PdeDataTagInfo TagPdeColumn(PdeContentItem pdeContentItem, ColumnExportItem expColumn)
        {
            string bmName = _mainManager.MainService.PdeService.ImportPdeTag(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc,
                    pdeContentItem, expColumn.DomainName, expColumn.ParentName, expColumn.ExcelName, expColumn.TreeNodeName);

            return new PdeDataTagInfo(MapType.Table, 
                pdeContentItem.EncodeSTN, expColumn.ParentName, expColumn.ExcelName, expColumn.ExcelName, bmName);
        }
        #endregion

        #region toggle classifier/domain combobox
        private void EnabledComboboxDomain(bool isEnable)
        {
            // cboDomain.Enabled = isEnable;
        }
        private void EnabledComboboxClassifier(bool isEnable)
        {
            cboClassifier.Enabled = isEnable;
        }
        #endregion

        #region intergrate with pde
        /// <summary>
        /// Load pde data when has imported file.
        /// </summary>
        public void LoadImportPDE()
        {
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            if (templateInfo.PdeContent != null && templateInfo.PdeContent.Items.Count > 0)
            {
                List<string> templateNames = new List<string>();
                foreach (PdeContentItem item in templateInfo.PdeContent.Items)
                    templateNames.Add(item.STN);

                cboDomain.Items.Clear();
                cboDomain.Items.AddRange(templateNames.ToArray());
                if (templateNames.Count > 0)
                    cboDomain.Text = templateNames[templateNames.Count - 1];
            }
        }

        private void BindPdeTree()
        {
            trvDomain.Clear();
            PdeContent pdeContent = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.PdeContent;
            if (pdeContent == null || pdeContent.Items == null || pdeContent.Items.Count == 0)
                return;

            PdeContentItem pdeContentItem = pdeContent.Items.FirstOrDefault(
                c => string.Equals(cboDomain.Text, c.STN, StringComparison.OrdinalIgnoreCase));
            BindPdeTree(pdeContentItem, chkUseInPdw.Checked);
        }

        private void BindPdeTree(PdeContentItem itemContent, bool isAll)
        {
            if (itemContent == null)
                return;

            TreeNode trvField = null;
            TreeNode trvTable = null;
            TreeNode trvChart = null;
            PdeExports export = itemContent.ExportData;
            if (export == null || export.Items == null || export.Items.Count < 1)
                return;

            // bind data
            foreach (DomainExportItem expDomain in export.Items)
            {
                //TreeNode domainNode = trvDomain.Nodes.Add(expDomain.DomainName);
                //domainNode.ExpandAll();

                foreach (ExportItem expItem in expDomain.Items)
                {
                    if (!expItem.Selected)
                        continue;
                    if (!isAll && !expItem.IsUsed) // user check on Use Pdw => only load tagged item
                        continue;

                    if (string.Compare(expItem.ExcelSheetName, cboSheet.Text) != 0)
                        continue;

                    TreeNode trnItem = null;
                    switch (expItem.MapType)
                    {
                        case MapType.SingleCell:
                            if (trvField == null)
                            {
                                trvField = new TreeNode();
                                trvField = trvField.Nodes.Add("Fields");
                            }
                            trnItem = trvField.Nodes.Add(expItem.TreeNodeName);
                            trnItem.Tag = expItem;
                            break;
                        case MapType.Table:
                            if (trvTable == null)
                            {
                                trvTable = new TreeNode();
                                trvTable = trvTable.Nodes.Add("Tables");
                            }
                            trnItem = trvTable.Nodes.Add(expItem.TreeNodeName);
                            trnItem.Tag = expItem;
                            if (expItem.Columns != null && expItem.Columns.Count > 0)
                            {
                                foreach (ColumnExportItem column in expItem.Columns)
                                {
                                    TreeNode trnColumn = trnItem.Nodes.Add(column.TreeNodeName);
                                    trnColumn.Tag = column;
                                }
                            }
                            break;
                        case MapType.Chart:
                            if (trvChart == null)
                            {
                                trvChart = new TreeNode();
                                trvChart = trvChart.Nodes.Add("Chart");
                            }
                            trnItem = trvChart.Nodes.Add(expItem.TreeNodeName);
                            trnItem.Tag = expItem;
                            break;
                        default:
                            break;
                    }
                }

                if(trvField != null)
                    trvDomain.Nodes.Add(trvField);

                if (trvTable != null)
                    trvDomain.Nodes.Add(trvTable);

                if (trvChart != null)
                    trvDomain.Nodes.Add(trvChart);

                if(trvField == null && trvChart == null && trvTable == null)
                    trvDomain.Nodes.Clear();
            }
        }

        private void BindPdeSheetToCombobox()
        {
            cboSheet.Items.Clear();
            PdeContent pdeContent = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.PdeContent;
            if (pdeContent == null || pdeContent.Items == null || pdeContent.Items.Count == 0)
                return;

            PdeContentItem pdeContentItem = pdeContent.Items.FirstOrDefault(
                c => string.Equals(cboDomain.Text, c.STN, StringComparison.OrdinalIgnoreCase));

            if (pdeContentItem == null)
                return;

            PdeExports export = pdeContentItem.ExportData;
            List<string> lstSheetName = new List<string>();
            if(export != null)
            {
                foreach (DomainExportItem domainExport in export.Items)
                {
                    foreach (ExportItem expItem in domainExport.Items)
                    {
                        if (!string.IsNullOrEmpty(expItem.ExcelSheetName))
                            lstSheetName.Add(expItem.ExcelSheetName);
                    }
                }
            }

            lstSheetName = lstSheetName.GroupBy(x => x).Select(g => g.Key).ToList();
            lstSheetName.Sort();
            cboSheet.Items.AddRange(lstSheetName.ToArray());
            if (lstSheetName.Count > 0)
                cboSheet.Text = lstSheetName[0];
        }

        #endregion

        private void CalculateBizDomainPanel()
        {
            if (cboClassifier.Items.Count > 0)
            {
                pnlDomainCombo.Height = _heightOfMainPanel - _heightPdeFilePanel - 7;
                pnlClassifer.Height = _heightClassiferPanel;
            }
            else
            {
                pnlDomainCombo.Height = _heightOfMainPanel - _heightPdeFilePanel - _heightClassiferPanel - 7;
                pnlClassifer.Height = 0;
            }

            pnlPdeFile.Height = 0;
        }

        private void CalculatePdeImportPanel()
        {
            pnlPdeFile.Height = _heightPdeFilePanel;
            pnlDomainCombo.Height = _heightOfMainPanel + _heightPdeFilePanel - _heightClassiferPanel - _heightComboDomainPanel - 11;
            pnlClassifer.Height = 0;
        }

        private void SetPropertiesToBizDomainControl()
        {
            cboDomain.Items.Clear();
            trvDomain.Clear();
            btnChangeColor.BackColor = Color.Green;
            cplDomain.HeaderText = "Business Domain";
            pnlPdeFile.Visible = false;
            cplDomain.Collapse = false;
            cplCondition.Visible = true;

            string classifier = cboClassifier.Text;
            LoadDomainIntoComboBox(classifier);
        }

        private void SetPropertiesToPdeControl()
        {
            cboDomain.Items.Clear();
            trvDomain.Clear();
            cplDomain.HeaderText = "Integrated Workbook";
            btnChangeColor.BackColor = Color.Gold;
            cplDomain.Collapse = false;
            pnlPdeFile.Visible = true;

            LoadImportPDE();
        }

        #endregion

        #region Public methods

        #region show full map
        public void UpdateFullMap(InternalBookmarkItem ibm)
        {
            if (ibm != null)
            {
                if (ibm.ItemType == DSIconType.USC.ToString()) // document-specific condition
                {
                    txtToolBar.Text = ibm.TechName;
                }
                else
                {
                    string bizName = (ibm.Type == XsltType.If || ibm.Type == XsltType.Select) ?
                        MarkupUtilities.GetOriginalBizName(ibm.BizName) : string.Empty;
                    txtToolBar.Text = string.IsNullOrEmpty(bizName) ? string.Empty : FindPath(bizName, trvDomain.Nodes[0]);
                }
            }
            else
                txtToolBar.Text = string.Empty;
        }

        private string FindPath(string nodeText, TreeNode trnNode)
        {
            if (trnNode == null)
                return string.Empty;
            if (nodeText.Equals(trnNode.Text, StringComparison.OrdinalIgnoreCase))
                return FindPath(trnNode);

            if (trnNode.Nodes != null)
            {
                foreach (TreeNode trnChild in trnNode.Nodes)
                {
                    string path = FindPath(nodeText, trnChild);
                    if (!string.IsNullOrEmpty(path))
                        return path;
                }
            }

            return string.Empty;
        }

        private string FindPath(TreeNode trnNode)
        {
            string path = string.Empty;

            TreeNode trnParent = trnNode;
            while (trnParent != null)
            {
                path = "\\" + trnParent.Text + path;
                trnParent = trnParent.Parent;
            }

            return string.IsNullOrEmpty(path) ? path : path.Substring(1);
        }
        #endregion

        #endregion

        //HACK:FORM CONTROLS - DataBind

        private bool IsPdmControlBindable()
        {
            var taskPane = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.LeftPanel;
            var propertyPanel = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.ControlPropertyGrid;

            bool bindable = taskPane != null && taskPane.Visible && propertyPanel != null && propertyPanel.SelectedObject is Pdw.FormControls.ControlBase;

            return bindable;
        }

        private void PdmControlDataBind(InternalBookmarkItem item)
        {
            string key;

            ServicesProfile profile = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out key);
            profile.PdmProfile.BindingBookmark = item;

            string path = item.ItemType == DSIconType.USC.ToString() ? item.TechName : FindPath(item.BizName, trvDomain.Nodes[0]);
            profile.PdmProfile.DatabindingPath = path;

            PropertyGrid propertyGrid = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.ControlPropertyGrid;
            profile.PdmProfile.CurrentPropertyName = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;

            Pdw.FormControls.ControlBase control = propertyGrid.SelectedObject as Pdw.FormControls.ControlBase;
            profile.PdmProfile.Control = control;
            
            try
            {
                _mainManager.MainService.PdmService.DataBind(key);
            }
            catch (Exception)
            {
            }

            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(key);
        }
    }
}
