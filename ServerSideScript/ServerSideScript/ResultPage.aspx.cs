using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ServerSideScript
{
    public partial class ResultPage : System.Web.UI.Page
    {
        protected string Path;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Path = "D:\\a640c969-f421-4cb8-8c0f-7d32c8794365.pdwr";
            }
        }
    }
}