using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using Pdw.Core;

namespace Pdw.Managers.Hcl
{
    public partial class frmImportItem : Form
    {
        public PdeContentItem pdeContent{get;set;}

        public frmImportItem()
        {
            InitializeComponent();
        }

        public void ShowImportedItem()
        {
            if (pdeContent == null || pdeContent.ExportData.Items == null)
            {
                dgvImportItems.DataSource = null;
                return;
            }

            dgvImportItems.AutoGenerateColumns = false;
            List<ImportedItem> listItem = new List<ImportedItem>();

            foreach (DomainExportItem item in pdeContent.ExportData.Items)
            {
                //TreeNode domainNode = trvImportData.Nodes[0].Nodes.Add(item.DomainName);

                foreach (ExportItem node in item.Items)
                {
                    ImportedItem itemImport = new ImportedItem();
                    itemImport.DomainName = item.DomainName;
                    itemImport.ItemName = node.TreeNodeName;
                    itemImport.Selected = node.Selected;
                    if (node.TreeNodeName.EndsWith(BaseProntoMarkup.KeySelect) && !node.TreeNodeName.Contains(BaseProntoMarkup.KeyTable))
                    {
                        itemImport.ItemType = "Field";
                        listItem.Add(itemImport);
                    }

                    if (node.TreeNodeName.EndsWith(BaseProntoMarkup.KeyTable))
                    {
                        itemImport.ItemType = "Table";
                        listItem.Add(itemImport);
                    }

                    if (node.MapType == MapType.Chart)
                    {
                        itemImport.ItemType = "Chart";
                        listItem.Add(itemImport);
                    }
                }
            }

            dgvImportItems.AutoGenerateColumns = false;
            ColSelect.DataPropertyName = "Selected";
            ColDomainName.DataPropertyName = "DomainName";
            ColItemType.DataPropertyName = "ItemType";
            ColItemName.DataPropertyName = "ItemName";
            
            BindingSource dataSource = new BindingSource();
            dataSource.DataSource = listItem;
            dgvImportItems.DataSource = dataSource;
            
        }

        public class ImportedItem
        {
            public string DomainName { get; set; }
            public string ItemType { get; set; }
            public string ItemName { get; set; }
            public bool Selected { get; set; }
        }

        private void frmImportItem_Load(object sender, EventArgs e)
        {
            ShowImportedItem();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvImportItems.Rows)
            {
                //DataGridViewRow row = dgvData.SelectedRows[0];
                string itemName = row.Cells[ColItemName.Name].Value.ToString();
                string domainName = row.Cells[ColDomainName.Name].Value.ToString();
                bool isSelected = bool.Parse(row.Cells[ColSelect.Name].Value.ToString());

                DomainExportItem domainExport = pdeContent.ExportData.Items.FirstOrDefault(
                   c => string.Equals(domainName, c.DomainName, StringComparison.OrdinalIgnoreCase));

                if (domainExport == null)
                    continue;

                ExportItem item = domainExport.Items.FirstOrDefault(
                   c => string.Equals(itemName, c.TreeNodeName, StringComparison.OrdinalIgnoreCase));

                item.Selected = isSelected;
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
