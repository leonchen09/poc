using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;
using Pdw.Managers.Hcl;
using Microsoft.Office.Interop.Excel;
using PdePlugin.control;
using ProntoDoc.Framework.CoreObject;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon2();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace PdePlugin
{
    [ComVisible(true)]
    public class Ribbon2 : Office.IRibbonExtensibility
    {
        public bool IsEnableButton = false;

        private Office.IRibbonUI ribbon;
        private ProntoExcelMarkup proMarkupCtrl;
        private CustomTaskPane RightPanel;

        private ProcessMap process;
        public ExportData export;
        public ConditionGoalSeek condGS;
        private CustomService cs;
        private bool CustomBtn1Visiable = false;
        public Ribbon2()
        {
            process = new ProcessMap();
            export = new ExportData();
            condGS = new ConditionGoalSeek();
            cs = new CustomService();
            LoadCustomDll();
        }

        private void LoadCustomDll()
        {
            System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFile(@"D:\work\source\POC\PdePlugin\CustomButtonDemo\bin\Debug\CustomButtonDemo.dll");
            Type type = ass.GetType("CustomButtonDemo.Class1");
            System.Reflection.MethodInfo method = type.GetMethod("CustomButtonInit");
            Object obj = ass.CreateInstance("CustomButtonDemo.Class1");
            method.Invoke(obj, new object[] {cs });
            CustomBtn1Visiable = true;
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("PdePlugin.Ribbon2.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, select the Ribbon XML item in Solution Explorer and then press F1

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        public void btnSaveAsPde_Click(Office.IRibbonControl control)
        {
            SaveTemplate(Globals.ThisAddIn.Application.ActiveWorkbook);
        }

        public void btnStartPronto_Click(Office.IRibbonControl control)
        {
            if (IsEnableButton) 
                showPane(false);
            else
                showPane(true);
        }

        public void btnMapCell_Click(Office.IRibbonControl control)
        {
            process.MapCell(Globals.ThisAddIn.Application.ActiveWorkbook, proMarkupCtrl.selectedNode);
        }

        public void btnMapTable_Click(Office.IRibbonControl control)
        {
            process.MapTable(Globals.ThisAddIn.Application.ActiveWorkbook, proMarkupCtrl.selectedNode);
        }

        public void btnExport_Click(Office.IRibbonControl control)
        {
            proMarkupCtrl.tabControl1.SelectedIndex = 1;
            export.exportItem(Globals.ThisAddIn.Application.ActiveWorkbook, proMarkupCtrl);
        }

        public void btnCondGS_Click(Office.IRibbonControl control)
        {
            condGS.createCGS(Globals.ThisAddIn.Application.ActiveWorkbook);
        }

        public void btnCondGSMan_Click(Office.IRibbonControl control)
        {
            condGS.listCGS(Globals.ThisAddIn.Application.ActiveWorkbook);
        }

        public void btnExportTab_Click(Office.IRibbonControl control)
        {
            proMarkupCtrl.tabControl1.SelectedIndex = 1;
            export.exportTable(Globals.ThisAddIn.Application.ActiveWorkbook, proMarkupCtrl);
        }

        public void btnTest_Click(Office.IRibbonControl control)
        {
            //SaveTemplate(Globals.ThisAddIn.Application.ActiveWorkbook);
            //ConditionGoalSeek condGS = new ConditionGoalSeek();
            //condGS.seek(Globals.ThisAddIn.Application.ActiveWorkbook);
            //process.TestMacro(Globals.ThisAddIn.Application.ActiveWorkbook, "test1");
            //process.TestXml(1);
           // process.TestXml(2);

            cs.Btn1Event("ddddd", (new Class2()).CallBackMethod);
            //cs.TriggerBnt1Event("ok");
        }

        public bool getTestVisiable(Office.IRibbonControl control)
        {
            return CustomBtn1Visiable;
        }

        public void OpenPdeTemplate(Workbook workbook)
        {
            //read the map information between business domain and workbook for template update.
            process.ReadMapInfo(workbook);

            //read the export items information for template update.
            export.ReadExportInfo(workbook);

            //read the condition goal seek information for template update.
            condGS.ReadCGSInfo(workbook);
        }

        public void SaveTemplate(Workbook workbook)
        {
            WaitingForm frmWaiting = null;
            XlMousePointer originalCursor = XlMousePointer.xlDefault;
            try
            {
                originalCursor = workbook.Application.Cursor;

                //if (templateInfo.IsProntoDoc)
                //{
                string filter = "Pronto Document Excel|*.pde";
                //switch (type)
                //{
                //    case TemplateType.Pdw:
                //        filter = PdwFilter;
                //        break;
                //    case TemplateType.Pdh:
                //        filter = PdhFilter;
                //        break;
                //    case TemplateType.Pdz:
                //        filter = PdzFilter;
                //        break;
                //    default:
                //        break;
                //}

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = filter;
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // set wating cursor
                    workbook.Application.Cursor = XlMousePointer.xlWait;
                    frmWaiting = new WaitingForm();
                    frmWaiting.Show();

                    //save the map information between excel table/cell and business domain tree into customer xml part of file.
                    process.SaveMapInfo(workbook);

                    //save exported items into customer xml part
                    if (export.exportItems != null && export.exportItems.Count > 0)//there is some data to export.
                    {
                        export.SaveExportInfo(workbook);
                        export.ExportXsd(workbook);
                    }

                    //save condition goal seek information into customer xml part.
                    if (condGS.cgsInfo.CGSInfos.Count > 0)
                    {
                        condGS.SaveCGSInfo(workbook);
                    }

                    workbook.SaveAs(saveDialog.FileName);

                    if (File.Exists(workbook.FullName))
                        MessageBox.Show(string.Format("ProntoDoc template {0} saved.",workbook.FullName));
                }
                //}
                //else
                //    MessageBox.Show(MessageUtils.Expand(Properties.Resources.ipm_NotIsProntoDoc,
                //        Properties.Resources.ipe_NotIsProntoDoc));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //catch (BaseException baseExp)
            //{
            //    ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SavePdwError);
            //    mgrExp.Errors.Add(baseExp);

            //    LogUtils.LogManagerError(mgrExp);
            //}
            finally
            {
                try
                {
                    if (frmWaiting != null)
                    {
                        frmWaiting.Close();
                        frmWaiting.Dispose();
                    }

                    workbook.Application.Cursor = originalCursor;
                }
                catch { }
            }
        }

        private void initPanel()
        {
            if (RightPanel != null)
                return;
            CustomTaskPaneCollection CustomTaskPanes = Globals.Factory.CreateCustomTaskPaneCollection(null, null, "CustomTaskPanes", "CustomTaskPanes", this);
            proMarkupCtrl = new ProntoExcelMarkup();
            proMarkupCtrl.tabControl1.SelectedIndex = 0;

            RightPanel = CustomTaskPanes.Add(proMarkupCtrl, "Pronto Excel");
            RightPanel.Width = 252;

            //update exported tree
            foreach (ExportItemMap item in export.exportItems)
            {
                TreeNode node = proMarkupCtrl.treeView2.Nodes[0].Nodes.Add(item.treeNodeName);
                if (item.mapType == ProntoDoc.Framework.CoreObject.MapType.Table)
                {
                    foreach (TableColumnMap col in item.tabCols)
                    {
                        node.Nodes.Add(col.treeNodeName);
                    }
                }
            }
        }

        private void showPane(bool show)
        {
            initPanel();
            RightPanel.Visible = show;
            IsEnableButton = show;
            ribbon.Invalidate();
        }

        public bool IsPronto(Office.IRibbonControl control)
        {
            return IsEnableButton;
        }

        public string GetLabel(Office.IRibbonControl control)
        {
            string tag = control.Tag;
            switch (control.Id)
            {
                case "btnStartPronto":
                    if (!IsEnableButton)
                        return "Show";
                    else
                        return "Hidden";
                default:
                    return control.Id;
            }
        }

        public stdole.IPictureDisp GetImage(Office.IRibbonControl control)
        {
            if (IsEnableButton)
                return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Start);
            else
                return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Stop);
        }

        public stdole.IPictureDisp GetMapCellImage(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.import);
        }

        public stdole.IPictureDisp GetMapTableImage(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.table);
        }

        public stdole.IPictureDisp GetCGSImage(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.CondGoalSeek);
        }

        public stdole.IPictureDisp GetExportImage(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.Icon_fix);
        }

        public stdole.IPictureDisp GetPdwIcon(Office.IRibbonControl control)
        {
            return PictureConverter.ConvertBitmapToPicDisp(Properties.Resources.pde);
        }

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

        #endregion
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
    
    }

}
