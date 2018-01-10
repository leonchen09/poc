using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Tools;
using Pdw.Managers.Hcl;

namespace PdePlugin
{
    public partial class Ribbon1
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            CustomTaskPaneCollection CustomTaskPanes = Globals.Factory.CreateCustomTaskPaneCollection(null, null, "CustomTaskPanes", "CustomTaskPanes", this);
            ProntoExcelMarkup proMarkupCtrl = new ProntoExcelMarkup();
            CustomTaskPane RightPanel = CustomTaskPanes.Add(proMarkupCtrl, "ProntoExcel");
            RightPanel.Width = 252;
            RightPanel.Visible = true;
        }
    }
}
