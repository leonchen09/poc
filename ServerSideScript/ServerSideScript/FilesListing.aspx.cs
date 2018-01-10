using System;

namespace ServerSideScript
{
    public partial class FilesListing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //required to keep the page from being cached on the client's browser
            Response.Expires = -1;

            string folderPath = System.Configuration.ConfigurationManager.AppSettings["FolderPath"];
            string html = "<table>";
            bool isOrd = false;
            foreach (string filePath in System.IO.Directory.GetFiles(folderPath))
            {
                html += "<tr class='tr" + isOrd.ToString() +  "'><td>" + filePath + "</td></tr>";
                isOrd = !isOrd;
            }
            html += "</table>";
            Response.ContentType = "text/plain";
            Response.Write(html);
            Response.End();
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion
    }
}