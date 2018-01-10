using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using InfoPath = Microsoft.Office.Interop.InfoPath;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;

namespace PdmPlugin
{
    public partial class ThisAddIn
    {

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            Ribbon1 ribbon = new Ribbon1();
            return ribbon;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
            InfoPath.ApplicationEvents events = (InfoPath.ApplicationEvents)((InfoPath._Application3)Application).Events;
            events.XDocumentOpen += new InfoPath._ApplicationEvents_XDocumentOpenEventHandler(ThisXDocument_Open);
            events.XDocumentBeforeSave += new InfoPath._ApplicationEvents_XDocumentBeforeSaveEventHandler(ThisXDocument_BeforeSave);
            
        }

        private void ThisXDocument_Open(InfoPath._XDocument xDocument)
        {
            MessageBox.Show("open");
            //xDocument.ImportFile(@"E:\ProntoDir\pdm\Pdmpocplugin.xsd");
        }

        private void ThisXDocument_BeforeSave(InfoPath._XDocument xDocument, ref bool cancel)
        {
            MessageBox.Show("save");
            //xDocument.ImportFile(@"E:\ProntoDir\pdm\Pdmpocplugin.xsd");
        }

        #endregion
    }
}
