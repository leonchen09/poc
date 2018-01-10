using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Core;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PdePlugin
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.subTempMaps = new Dictionary<string, string>();
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }
        //the word application which lanch this excel
        public Word.Application wordApp{get; set;}
        //save the filename and subtemplate name map information
        //the key is full file name of the workbook and the value is subtemplatename which defined by user while open this workbook.
        public Dictionary<string, string> subTempMaps { get; set; }

        public Ribbon2 ribbon2 { get; set; }

        #region code for call addin from outof word application.
        private ExcelAddinUtils addinUtilities;

        protected override object RequestComAddInAutomationService()
        {
            if (addinUtilities == null)
            {
                addinUtilities = new ExcelAddinUtils();
            }
            return addinUtilities;
        }
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
            Application.WorkbookOpen += new Excel.AppEvents_WorkbookOpenEventHandler(ThisWorkbook_Open);
        }
        
        #endregion
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            ribbon2 = new Ribbon2();
            return ribbon2;
        }

        private void ThisWorkbook_Open(Excel.Workbook workbook)
        {
            if (workbook.FullName.EndsWith("pde"))
            {
                ribbon2.OpenPdeTemplate(workbook);
            }
            else if (workbook.FullName.EndsWith("pder"))
            {
                //merge the xml data into workbook.
                string fileName = "e:\\pdeexportdata_temp.xml";
                new RenderService().Render(workbook, fileName);
            }
            if (workbook.HasVBProject)
                MessageBox.Show("xlsm");
        }
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IExcelAddinUtils
    {
        void LinkWord(Word.Application word, string subTemplateName);
        void ReleaseWord();
    }



    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ExcelAddinUtils : StandardOleMarshalObject, IExcelAddinUtils
    {
        public void LinkWord(Word.Application word, string subTemplateName)
        {
            Globals.ThisAddIn.wordApp = word;
            string fullFileName = Globals.ThisAddIn.Application.ActiveWorkbook.FullName;
            Globals.ThisAddIn.subTempMaps.Add(fullFileName, subTemplateName);
        }

        public void ReleaseWord()
        {
            Globals.ThisAddIn.wordApp = null;
        }
    }
}
