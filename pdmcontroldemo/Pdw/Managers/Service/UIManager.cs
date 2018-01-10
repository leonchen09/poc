
using System;

using Pdw.Core;
using System.Windows.Forms;
using Pdw.WKL.Profiler.Manager;

namespace Pdw.Managers.Service
{
    public class UIManager : BaseManager
    {
        /// <summary>
        /// Show panel in the word document
        /// </summary>
        public event ShowPanelEnventHandler ShowPanel;

        /// <summary>
        /// Invisible panel in the word document
        /// </summary>
        public event HidePanelEventHandler HidePanel;

        /// <summary>
        /// Disable pronto ribbon
        /// </summary>
        public event DisableRibbonEventHandler DisableRibbon;

        /// <summary>
        /// To Visable Reconstruct button
        /// </summary>
        public event VisiableReconstructEventHandler VisableReconstruct;

        /// <summary>
        /// To Visable Preview Osql button
        /// </summary>
        public event VisiablePreviewOsqlEventHandler VisablePreviewOsql;

        /// <summary>
        /// Force To Visable Preview Osql button
        /// </summary>
        public event ForceDisablePreviewOsqlEventHandler ForceDisablePreviewOsql;

        /// <summary>
        /// Update word UI (status of ribbon and panel)
        /// </summary>
        /// <param name="eventType"></param>
        public void UpdateUI(EventType eventType)
        {
            try
            {
                switch (eventType)
                {
                    case EventType.ShowPanel:
                        ShowPanel();
                        if (CurrentTemplateInfo.RightPanel == null)
                        {
                            CommonProfile.AddIn.AddProntoTaskPane();
                            FocusOnRibbon();
                        }
                        else
                            CurrentTemplateInfo.RightPanel.Visible = true;
                        CurrentTemplateInfo.RightPanelStatus = true;

                        // disable others panel
                        foreach (Microsoft.Office.Tools.CustomTaskPane panel in CommonProfile.AddIn.CustomTaskPanes)
                        {
                            if (panel != CurrentTemplateInfo.RightPanel)
                                panel.Visible = false;
                        }
                        break;
                    case EventType.HidePanel:
                        HidePanel();

                        if (CommonProfile.AddIn.CustomTaskPanes != null)
                        {
                            foreach (Microsoft.Office.Tools.CustomTaskPane panel in CommonProfile.AddIn.CustomTaskPanes)
                            {
                                if (panel != null)
                                    panel.Visible = false;
                            }
                        }
                        CurrentTemplateInfo.RightPanelStatus = false;
                        break;
                    case EventType.DisableRibbon:
                        DisableRibbon();
                        break;
                    case EventType.VisiableReconstruct:
                        VisableReconstruct();
                        break;
                    case EventType.VisiablePreviewOsql:
                        VisablePreviewOsql();
                        break;
                    case EventType.ForceDisablePreviewOsql:
                        ForceDisablePreviewOsql();
                        break;

                    //HACK:FORM CONTROLS - ShowControlPropertyPanel
                    case EventType.ShowControlPropertyPanel:
                        ShowControlPanel();
                        break;
                    case EventType.RefreshControlPropertyPanel:
                        if (CurrentTemplateInfo.ControlPropertyGrid != null)
                        {
                            CurrentTemplateInfo.ControlPropertyGrid.Refresh();
                        }
                        break;
                }
            }
            catch { }
        }

        private void ShowControlPanel()
        {
            if (CurrentTemplateInfo.LeftPanel == null)
            {
                UserControl wrapper = new UserControl();
                wrapper.Controls.Add(new PropertyGrid { Dock = DockStyle.Fill });

                //HACK:FORM CONTROLS - TODO:RESOURCE
                var taskPane = CommonProfile.AddIn.CustomTaskPanes.Add(wrapper, "Pdm Control Property Details");
                taskPane.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionLeft;

                CurrentTemplateInfo.LeftPanel = taskPane;
            }

            CurrentTemplateInfo.LeftPanel.Visible = true;
        }

        private void FocusOnRibbon()
        {
            //NativeMethods.SendKey(NativeConstants.Alt, NativeConstants.KeyDown); // press Alt
            //NativeMethods.SendKey(NativeConstants.Alt, NativeConstants.KeyUp); // release Alt

            //NativeMethods.SendKey(NativeConstants.X, NativeConstants.KeyDown); // press X
            //NativeMethods.SendKey(NativeConstants.X, NativeConstants.KeyUp); // release X

            //NativeMethods.SendKey(NativeConstants.Alt, NativeConstants.KeyDown); // press Alt
            //NativeMethods.SendKey(NativeConstants.Alt, NativeConstants.KeyUp); // release Alt
        }
    }
}
