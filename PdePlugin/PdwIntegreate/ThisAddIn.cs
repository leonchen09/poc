using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;

namespace PdwIntegreate
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Application.DocumentBeforeClose += new Word.ApplicationEvents4_DocumentBeforeCloseEventHandler(Application_DocumentBeforeClose);
        }
        private void Application_DocumentBeforeClose(Word.Document Doc, ref bool Cancel)
        {
            // remove internal bookmark, makrup object and datasegment
           // Doc.Shapes.AddLabel(Office.MsoTextOrientation.msoTextOrientationMixed, 30, 2, 33, 44);
            Doc.Save();
            Zip zip = new Zip();
            zip.ServerDir = @"E:\ProntoDir\pde\";
            zip.UnZipFile("doc3.docx");
        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

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
    }
}
