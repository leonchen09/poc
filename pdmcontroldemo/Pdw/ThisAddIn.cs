
using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Word = Microsoft.Office.Interop.Word;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdw.Core;
using Pdw.Managers;
using Pdw.Managers.Hcl;
using Pdw.Managers.Context;
using Pdw.Managers.Service;
using Pdw.Managers.DataIntegration;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw
{
    public partial class ThisAddIn
    {
        private delegate void ClosePdwrFileDelegate(Word.Document Doc, List<string> tempFiles);

        #region private properties
        private Word.Document ActiveDocument
        {
            get
            {
                return WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc;
            }
        }

        private TemplateInfo TemplateInfo
        {
            get
            {
                return WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            }
        }
        #endregion

        #region Add or Remove Panel
        /// <summary>
        /// add right panel into document
        /// </summary>
        public void AddProntoTaskPane()
        {
            try
            {
                TemplateInfo templateInfo = this.TemplateInfo;
                if (templateInfo.RightPanel != null)
                    CustomTaskPanes.Add(templateInfo.RightPanel.Control, Properties.Resources.ipm_RibbonTitle);
                else
                {
                    ProntoDocMarkup proMarkupCtrl = new ProntoDocMarkup();
                    
                    // backup width before add
                    int parentWidth = proMarkupCtrl.cboDomain.Parent.Width;
                    int width = proMarkupCtrl.cboDomain.Width;

                    // add to right panel of document
                    templateInfo.RightPanel = CustomTaskPanes.Add(proMarkupCtrl, Properties.Resources.ipm_RibbonTitle);

                    // after add, the width of domain control is reset to 0 then we need restore its.
                    proMarkupCtrl.cboDomain.Parent.Width = parentWidth;
                    proMarkupCtrl.cboDomain.Width = width;

                    templateInfo.RightPanel.Width = 252;
                    templateInfo.RightPanel.Visible = true;
                    templateInfo.RightPanel.VisibleChanged += new EventHandler(ProntoTaskPane_VisibleChanged);

                    if (ActiveDocument != null)
                        AddOrderContextMenu();
                }

                SetHeightForProntoDocMarkup();
            }
            catch (BaseException srvExp)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_LoadResourceError);
                mgrExp.Errors.Add(srvExp);

                LogUtils.LogManagerError(mgrExp);
            }
        }

        #region TaskPanel event (SizeChanged, VisibleChanged)
        /// <summary>
        /// fetch height of right panel into control inside
        /// </summary>
        private void SetHeightForProntoDocMarkup()
        {
            TemplateInfo templateInfo = this.TemplateInfo;
            if (templateInfo.RightPanel != null)
            {
                ProntoDocMarkup proMarkupCtrl = templateInfo.RightPanel.Control as ProntoDocMarkup;
                proMarkupCtrl.SetHeightControl(templateInfo.RightPanel.Height);
            }
        }

        /// <summary>
        /// visible/disable right panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProntoTaskPane_VisibleChanged(object sender, EventArgs e)
        {
            MainManager mainMgr = new MainManager();
            TemplateInfo template = this.TemplateInfo;
            if (template.RightPanel != null)
            {
                if (!template.RightPanel.Visible)
                    mainMgr.RibbonManager.UpdateUI(EventType.DisableRibbon);
                else
                    mainMgr.RibbonManager.UpdateUI(EventType.ShowPanel);
            }
        }
        #endregion
        #endregion

        #region load ribbon UI
        /// <summary>
        /// Call ribbon interface
        /// </summary>
        /// <returns></returns>
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new ProntoRibbon();
        }
        #endregion

        #region application events (Startup, DocumentOpen, DocumentBeforeSave, Shutdown)
        /// <summary>
        /// startup application (EntryPoint)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // VariableName = "Variable in add-in";
            Application.DocumentOpen +=
                new Word.ApplicationEvents4_DocumentOpenEventHandler(ThisDocument_DocumentOpen);
            Application.DocumentBeforeSave +=
                new Word.ApplicationEvents4_DocumentBeforeSaveEventHandler(ThisDocument_DocumentBeforeSave);
            Application.WindowSelectionChange += new Word.ApplicationEvents4_WindowSelectionChangeEventHandler(Application_WindowSelectionChange);
            Application.DocumentBeforeClose += new Word.ApplicationEvents4_DocumentBeforeCloseEventHandler(Application_DocumentBeforeClose);
            Application.DocumentChange += new Word.ApplicationEvents4_DocumentChangeEventHandler(Application_DocumentChange);
        }

        /// <summary>
        /// Occurs when a document is opened. (Inherited from ApplicationEvents4_Event.)
        /// 1. Check Pronto document. If correct go to 2
        /// 2. Check bookmark. If incorrect go to 3
        /// 3. Re-contructor documet and re-open
        /// </summary>
        /// <param name="Doc"></param>
        private void ThisDocument_DocumentOpen(Word.Document Doc)
        {
            string pdwrExtension = FileExtension.Pdwr;

            // 1. verify this document is comming from web server or no
            if (Doc.FullName.Contains("/") && Doc.FullName.ToLower().EndsWith(pdwrExtension))
                Application.Visible = true;

            if (Application.Visible)
            {
                ContextManager contextManager = new ContextManager();
                if (Doc.FullName.ToLower().EndsWith(pdwrExtension))
                {
                    Application.ScreenUpdating = false;
                    List<string> tempFiles = new List<string>();
                    try
                    {
                        contextManager.OpenPdwr(Doc, ref tempFiles);
                    }
                    catch (BaseException baseExp)
                    {
                        ManagerException mgrExp = new ManagerException(ErrorCode.ipe_OpenPdwrError);
                        mgrExp.Errors.Add(baseExp);

                        LogUtils.LogManagerError(mgrExp);
                    }
                    catch (Exception ex)
                    {
                        ManagerException mgrExp = new ManagerException(ErrorCode.ipe_OpenPdwrError,
                            MessageUtils.Expand(Properties.Resources.ipe_OpenPdwrError, ex.Message), ex.StackTrace);

                        LogUtils.LogManagerError(mgrExp);
                    }

                    ClosePdwrFileDelegate closePdwrFile = new ClosePdwrFileDelegate(ClosePdwrFile);
                    closePdwrFile.BeginInvoke(Doc, tempFiles, null, null);

                    if (Application.Documents.Count == 0 ||
                        (Application.Documents.Count == 1 && Application.ActiveDocument.FullName == Doc.FullName))
                        ((Word._Application)Application).Quit();
                }
                else
                {
                    try
                    {
                        contextManager.OpenDocument(Doc);
                    }
                    catch (BaseException baseExp)
                    {
                        ManagerException mgrExp = new ManagerException(ErrorCode.ipe_OpenDocumentError);
                        mgrExp.Errors.Add(baseExp);
                        LogUtils.LogManagerError(mgrExp);
                    }
                    catch (Exception ex)
                    {
                        ManagerException mgrExp = new ManagerException(ErrorCode.ipe_OpenDocumentError,
                            MessageUtils.Expand(Properties.Resources.ipe_OpenDocumentError, ex.Message), ex.StackTrace);
                        LogUtils.LogManagerError(mgrExp);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs before any open document is saved. (Inherited from ApplicationEvents4_Event.)
        /// </summary>
        /// <param name="Doc"></param>
        /// <param name="SaveAsUI"></param>
        /// <param name="Cancel"></param>
        private void ThisDocument_DocumentBeforeSave(Word.Document Doc, ref bool SaveAsUI, ref bool Cancel)
        {
            try
            {
                if (Doc != Application.ActiveDocument)
                    return;
            }
            catch { return; }

            TemplateInfo template = this.TemplateInfo;
            if (template.IsProntoDoc)
            {
                try
                {
                    ContextManager contexMgr = new ContextManager();
                    contexMgr.SaveDocument(Doc, ref SaveAsUI, ref Cancel);
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
        }

        /// <summary>
        /// Occurs when the selection changes in the active document window. (Inherited from ApplicationEvents4_Event.)
        /// When user change selection in word document
        /// </summary>
        /// <param name="Sel"></param>
        private void Application_WindowSelectionChange(Word.Selection Sel)
        {
            TemplateInfo template = this.TemplateInfo;
            if (template.IsProntoDoc)
            {
                DataIntegrationManager bmMgr = new DataIntegrationManager();
                string bmName = bmMgr.HasBookmark(Sel);
                if (bmMgr.GetForeachTag(Sel) != null)
                    BaseManager.ChangeOrderByStatus(true);
                else
                    BaseManager.ChangeOrderByStatus(false);

                #region update full map
                TemplateInfo templateInfo = this.TemplateInfo;
                if (templateInfo.RightPanel != null)
                {
                    ProntoDocMarkup proMarkupCtrl = templateInfo.RightPanel.Control as ProntoDocMarkup;
                    if (string.IsNullOrEmpty(bmName))
                        proMarkupCtrl.UpdateFullMap(null);
                    else
                    {
                        InternalBookmarkItem iBm = TemplateInfo.InternalBookmark.GetInternalBookmarkItem(bmName);
                        proMarkupCtrl.UpdateFullMap(iBm);
                    }
                }
                #endregion
            }
            else
                BaseManager.ChangeOrderByStatus(false);
        }

        /// <summary>
        /// Occurs immediately before any open document closes. (Inherited from ApplicationEvents4_Event.)
        /// </summary>
        /// <param name="Doc"></param>
        /// <param name="Cancel"></param>
        private void Application_DocumentBeforeClose(Word.Document Doc, ref bool Cancel)
        {
            // get template information
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(Doc.FullName);
            if (templateInfo == null)
                return;

            // remove internal bookmark, makrup object and datasegment
            if (!templateInfo.IsReConstructing)
                Wkl.MainCtrl.CommonCtrl.RemoveTemplateInfo(Doc.FullName);

            // check pde content
            ContextManager contextMgr = new ContextManager();
            contextMgr.CheckPdeContent(Doc, templateInfo.PdeContent);
            if (templateInfo.PdeContent != null && templateInfo.PdeContent.Items != null &&
                templateInfo.PdeContent.Items.Exists(c => c.Status == Constants.ContextManager.NotUsingStatus))
            {
                DialogResult result = MessageBox.Show("Has some pde template are not used. Do you want to check and process?",
                    "Confirm", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    ImportPdeControl importer = new ImportPdeControl(Doc, templateInfo.PdeContent, false);
                    importer.ShowDialog();
                    templateInfo.PdeContent = importer.PdeContent;
                }
            }

            contextMgr.CloseExcel();
        }

        /// <summary>
        /// Occurs when a new document is created, when an existing document is opened, 
        /// or when another document is made the active document. (Inherited from ApplicationEvents4_Event.)
        /// </summary>
        private void Application_DocumentChange()
        {
            CheckTaskPanel(ActiveDocument);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region helpers method
        private void CheckTaskPanel(Word.Document Doc)
        {
            TemplateInfo template = this.TemplateInfo;
            MainManager mainMgr = new MainManager();

            if (template.RightPanelStatus)
            {
                if (Doc.ActiveWindow.WindowState != Word.WdWindowState.wdWindowStateMinimize)
                {
                    // Managers.BaseManager.UIManager().UpdateUI(EventType.ShowPanel);
                    mainMgr.RibbonManager.UpdateUI(EventType.ShowPanel);
                    //HACK:FORM CONTROLS - CheckTaskPanel
                    mainMgr.RibbonManager.UpdateUI(EventType.ShowControlPropertyPanel);
                }
                else
                    // Managers.BaseManager.UIManager().UpdateUI(EventType.HidePanel);
                    mainMgr.RibbonManager.UpdateUI(EventType.HidePanel);
            }
            else
                // Managers.BaseManager.UIManager().UpdateUI(EventType.HidePanel);
                mainMgr.RibbonManager.UpdateUI(EventType.HidePanel);
        }

        private void AddOrderContextMenu()
        {
            string rightMenuName = Core.Constants.OrderBy.ContextMenuName;
            string caption = Properties.Resources.ipm_OrderByMenuCaption;
            string tag = Core.Constants.OrderBy.OrderByMenuTag;

            RemoveOrderBy();

            // 1. find existed order by menu and reset event
            if (BaseManager.DefineOrderByButton == null)
            {
                foreach (Microsoft.Office.Core.CommandBarControl bar in Application.CommandBars[rightMenuName].Controls)
                {
                    if (bar.Caption == caption && bar.Tag == tag)
                    {
                        BaseManager.DefineOrderByButton = bar as Microsoft.Office.Core.CommandBarButton;
                        break;
                    }
                }
            }

            // 2. Add new order by menu
            if (BaseManager.DefineOrderByButton == null)
            {
                BaseManager.DefineOrderByButton = Application.CommandBars[rightMenuName].Controls.Add(
                    Microsoft.Office.Core.MsoControlType.msoControlButton) as Microsoft.Office.Core.CommandBarButton;
                BaseManager.DefineOrderByButton.Caption = caption;
                BaseManager.DefineOrderByButton.Tag = tag;
            }

            try
            {
                BaseManager.DefineOrderByButton.Click -= DefineOrderBy;
            }
            catch { }
            BaseManager.DefineOrderByButton.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(DefineOrderBy);
        }

        public void DefineOrderBy(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            try
            {
                DataIntegrationManager bmMgr = new DataIntegrationManager();
                Word.Bookmark foreachBm = bmMgr.GetForeachTag(Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentSelection);
                List<Word.Bookmark> dataTags = bmMgr.GetInsideBookmarks(foreachBm.Name,
                    foreachBm.Name.Replace(ProntoMarkup.KeyStartForeach, ProntoMarkup.KeyEndForeach));

                if (dataTags.Count > 0)
                {
                    DefineOrderByControl defineOrderByCtrl = new DefineOrderByControl(foreachBm, dataTags);
                    defineOrderByCtrl.ShowDialog();
                    if (!string.IsNullOrEmpty(defineOrderByCtrl.OrderByValue))
                        bmMgr.UpdateBookmarkText(foreachBm, defineOrderByCtrl.OrderByValue);
                }
            }
            catch { }
        }

        /// <summary>
        /// remove define order by context menu (must be remove when golive)
        /// </summary>
        private void RemoveOrderBy()
        {
            BaseManager.DefineOrderByButton = null;
            string rightMenuName = Core.Constants.OrderBy.ContextMenuName;
            string caption = Properties.Resources.ipm_OrderByMenuCaption;
            string tag = Core.Constants.OrderBy.OrderByMenuTag;

            foreach (Microsoft.Office.Core.CommandBarControl bar in Application.CommandBars[rightMenuName].Controls)
                if (bar.Caption == caption && bar.Tag == tag)
                    bar.Delete();
        }
        #endregion

        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

        private void ClosePdwrFile(Word.Document Doc, List<string> tempFiles)
        {
            try
            {
                ((Word._Document)Doc).Close();
                foreach (string tempFile in tempFiles)
                    if (System.IO.File.Exists(tempFile))
                        System.IO.File.Delete(tempFile);
            }
            catch { }
        }

        #region interactive with excel
        private AddinUtilities _addinUtilities;

        protected override object RequestComAddInAutomationService()
        {
            if (_addinUtilities == null)
                _addinUtilities = new AddinUtilities();

            return _addinUtilities;
        }

        [System.Runtime.InteropServices.ComVisible(true)]
        [System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
        public interface IAddinUtilities
        {
            void ImportTable(Microsoft.Office.Interop.Excel.Range eRange);

            void ImportChart(Microsoft.Office.Interop.Excel.Chart eChart);
        }

        [System.Runtime.InteropServices.ComVisible(true)]
        [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.None)]
        public class AddinUtilities : System.Runtime.InteropServices.StandardOleMarshalObject, IAddinUtilities
        {
            public void ImportTable(Microsoft.Office.Interop.Excel.Range eRange)
            {
                ContextManager contextMgr = new ContextManager();
                contextMgr.ImportPdeTable(eRange, Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc);
            }

            public void ImportChart(Microsoft.Office.Interop.Excel.Chart eChart)
            {
                ContextManager contextMgr = new ContextManager();
                contextMgr.ImportPdeChart(eChart, Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc);
            }
        }
        #endregion
    }
}
