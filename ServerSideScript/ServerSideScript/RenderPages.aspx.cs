using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Pronto.SX;
using Pronto.SX.SharedComponent;
using Pdwx;

namespace ServerSideScript
{
    public partial class RenderPages : System.Web.UI.Page
    {
        private static SXService sxService;
        private const string DBXConnectionString = "DBXConnectionString";
        private Guid PluginGuid = new Guid("a640c969-f421-4cb8-8c0f-7d32c8794365");
        private const int CategoryId = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                sxService = new SXService();
                cboRenderMode.DataSource = new List<RenderMode>() { RenderMode.RawMode, RenderMode.TestMode };
                cboRenderMode.DataBind();
                btnGetTemplate.Enabled = false;
            }
        }

        protected void btnStartSX_Click(object sender, EventArgs e)
        {
            try
            {
                sxService.DDX_Start(System.Configuration.ConfigurationManager.AppSettings[DBXConnectionString], CategoryId, false);
                btnStartSX.Enabled = false;
                btnGetTemplate.Enabled = true;

                lblMessage.Text = "SX Service is started successful.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void btnRender_Click(object sender, EventArgs e)
        {
            try
            {
                TemplateInfo _templateInfo = new TemplateInfo();
                _templateInfo.TemplateID = new Guid(grdTemplate.SelectedRow.Cells[1].Text);

                RenderRequestInfo renderRequestInfo = new RenderRequestInfo();
                renderRequestInfo.ComputerName = txtComputerName.Text;
                renderRequestInfo.CreatedBy = txtCreatedBy.Text;
                renderRequestInfo.PriorityID = PriorityId.Medium;
                renderRequestInfo.TemplateID = new Guid(_templateInfo.TemplateID.ToString());
                renderRequestInfo.RequestTypeID = (int)(RenderMode)Enum.Parse(typeof(RenderMode), cboRenderMode.SelectedItem.Value);

                List<object> ParamValues = new List<object>() { int.Parse(txtParam1.Text) };
                PackageData result = sxService.APIRenderRequest.AddToRenderRequestQ(renderRequestInfo, ParamValues);

                if (result.Status == SXStatus.Success)
                    lblMessage.Text = "The output is store in reposity folder at file: " + ((string)result.Data);
                else
                {
                    foreach (var code in result.ErrorCodes)
                        lblMessage.Text += " | " + code.ToString();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void btnGetTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                PackageData data = sxService.APITemplate.GetTemplateCollectionByCategoryId(PluginGuid, CategoryId, true);
                if (data.Status == SXStatus.Success)
                {
                    List<TemplateInfo> templates = data.Data as List<TemplateInfo>;
                    grdTemplate.DataSource = templates;
                    grdTemplate.DataBind();

                    lblMessage.Text = "Templates are load successful.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
    }
}