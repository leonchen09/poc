
using System;
using System.Linq;
using System.Windows.Forms;

using Word = Microsoft.Office.Interop.Word;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdw.Managers.Context;
using System.Collections.Generic;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Managers.Hcl
{
    public partial class ImportPdeControl : Form
    {
        private Word.Document _wDoc;
        public PdeContent PdeContent { get; private set; }

        public ImportPdeControl(Word.Document wDoc, PdeContent pdeContent, bool isAdd)
        {
            InitializeComponent();

            _wDoc = wDoc;
            PdeContent = pdeContent;
            BindPdeContent();

            btnAdd.Enabled = isAdd;
            btnUpdate.Enabled = !isAdd;
            btnDelete.Enabled = !isAdd;
        }

        #region events
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pronto Excel Template|*.pde";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtPdeFile.Text = openFileDialog.FileName;
                txtSTN.Text = GenSTN(txtPdeFile.Text);
                numSAppID.Value = 0;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows != null && dgvData.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvData.SelectedRows[0];
                string oldSTN = row.Cells[ColSTN.Name].Value.ToString();

                PdeContentItem item = PdeContent.Items.FirstOrDefault(
                    c => string.Equals(oldSTN, c.STN, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    PdeContent.Items.Remove(item);
                    dgvData.Rows.RemoveAt(row.Index);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows != null && dgvData.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvData.SelectedRows[0];
                string oldSTN = row.Cells[ColSTN.Name].Value.ToString();

                PdeContentItem item = PdeContent.Items.FirstOrDefault(
                    c => string.Equals(oldSTN, c.STN, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    #region validate
                    if (string.IsNullOrWhiteSpace(txtSTN.Text))
                    {
                        MessageBox.Show("Supplemetary template name is not allowed empty.");
                        return;
                    }

                    if (PdeContent.Items.Exists(c => string.Equals(txtSTN.Text, c.STN, StringComparison.OrdinalIgnoreCase) &&
                        (!string.Equals(oldSTN, c.STN, StringComparison.OrdinalIgnoreCase))))
                    {
                        MessageBox.Show("This supplemetary template name is already defined. Please input another.");
                        return;
                    }
                    #endregion

                    // update pde content
                    item.SAppID = Convert.ToInt32(numSAppID.Value);
                    item.STN = txtSTN.Text;

                    // update ui
                    row.Cells[ColSAppID.Name].Value = numSAppID.Value;
                    row.Cells[ColSTN.Name].Value = txtSTN.Text;

                    // todo: ngocbv_check and update in here
                    //ContextManager contextMgr = new ContextManager();
                    //contextMgr.UpdateLinkSource();
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPdeFile.Text))
            {
                MessageBox.Show("Please select a pde file.");
                return;
            }

            if (!System.IO.File.Exists(txtPdeFile.Text))
            {
                MessageBox.Show("Pde file is not exist.");
                return;
            }

            string templateName = System.IO.Path.GetFileNameWithoutExtension(txtPdeFile.Text);
            PdeContentItem pdeContentItem = 
                AddPdeConentItem(txtPdeFile.Text, templateName, txtSTN.Text, Convert.ToInt32(numSAppID.Value));

            if(pdeContentItem == null)
                return;

            BindPdeContent();

            ContextManager contextMgr = new ContextManager();
            contextMgr.ImportPde(pdeContentItem, _wDoc);

            frmImportItem importItem = new frmImportItem();
            importItem.pdeContent = pdeContentItem;
            importItem.ShowDialog();
            importItem.ShowImportedItem();

            //Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.ProntoDocMarkup.LoadImportPDE();
            //Close();
        }

        private void dgvData_SelectionChanged(object sender, EventArgs e)
        {
            // default value
            txtPdeFile.Text = string.Empty;
            txtSTN.Text = string.Empty;
            numSAppID.Value = 0;

            // set value
            if (dgvData.SelectedRows != null && dgvData.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvData.SelectedRows[0];
                txtPdeFile.Text = row.Cells[ColFilePath.Name].Value.ToString();
                txtSTN.Text = row.Cells[ColSTN.Name].Value.ToString();
                int sAppId = 0;
                int.TryParse(row.Cells[ColSAppID.Name].Value.ToString(), out sAppId);
                numSAppID.Value = sAppId;
            }
        }
        #endregion

        #region public methods
        #endregion

        #region helper methods
        private void BindPdeContent()
        {
            if (PdeContent == null || PdeContent.Items == null)
            {
                dgvData.DataSource = null;
                return;
            }

            dgvData.AutoGenerateColumns = false;
            ColTemplateName.DataPropertyName = "TemplateName";
            ColSTN.DataPropertyName = "STN";
            ColSAppID.DataPropertyName = "SAppID";
            ColStatus.DataPropertyName = "Status";
            ColFilePath.DataPropertyName = "FilePath";

            BindingSource dataSource = new BindingSource();
            dataSource.DataSource = PdeContent.Items;
            dgvData.DataSource = dataSource;
        }

        private PdeContentItem AddPdeConentItem(string pdeFilePath, string templateName, string stn, int sAppID)
        {
            #region validate
            if (string.IsNullOrWhiteSpace(stn))
            {
                MessageBox.Show("Supplemetary template name is not allowed empty.");
                return null;
            }

            if (PdeContent.Items.Exists(c => string.Equals(stn, c.STN, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("This supplemetary template name is already defined. Please input another.");
                return null;
            }
            #endregion

            #region add
            PdeContentItem pdeContentItem = new PdeContentItem();
            pdeContentItem.FileContent = ProntoDoc.Framework.Utils.FileHelper.ExcelToBase64(pdeFilePath);
            pdeContentItem.SAppID = sAppID;
            pdeContentItem.Status = Core.Constants.ContextManager.NotUsingStatus;
            pdeContentItem.STN = stn;
            pdeContentItem.TemplateName = templateName;
            pdeContentItem.FilePath = pdeFilePath;

            PdeContent.Items.Add(pdeContentItem);
            #endregion

            return pdeContentItem;
        }

        private string GenSTN(string pdeFilePath)
        {
            string templateName = System.IO.Path.GetFileNameWithoutExtension(pdeFilePath);
            if (!PdeContent.Items.Exists(c => string.Equals(templateName, c.STN, StringComparison.OrdinalIgnoreCase)))
                return templateName;

            return string.Format("{0}_{1}", Guid.NewGuid(), templateName);
        }
        #endregion

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvData.Rows[e.RowIndex];
            string oldSTN = row.Cells[ColSTN.Name].Value.ToString();

            PdeContentItem item = PdeContent.Items.FirstOrDefault(
                c => string.Equals(oldSTN, c.STN, StringComparison.OrdinalIgnoreCase));

            frmImportItem importItem = new frmImportItem();
            importItem.pdeContent = item;
            importItem.ShowDialog();
            importItem.ShowImportedItem();
        }
    }
}
