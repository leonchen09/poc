using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Excel;

namespace PdwPlugin
{
    public partial class Ribbon1
    {
        private Microsoft.Office.Interop.Excel.Application excel = null;

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            PdeIntegrate.InsertTable(excel, Globals.ThisAddIn.Application.ActiveDocument);
        }

        private void button2_Click(object sender, RibbonControlEventArgs e)
        {
            excel = PdeIntegrate.OpenExcel(excel, Globals.ThisAddIn.Application.ActiveDocument);
        }

        private void button3_Click(object sender, RibbonControlEventArgs e)
        {
            PdeIntegrate.InsertChart(excel, Globals.ThisAddIn.Application.ActiveDocument);
        }

        private void button4_Click(object sender, RibbonControlEventArgs e)
        {
            PdeIntegrate.GetName(excel, Globals.ThisAddIn.Application.ActiveDocument);
            //PdeIntegrate.RefreshData(Globals.ThisAddIn.Application.ActiveDocument);
        }
    }
}
