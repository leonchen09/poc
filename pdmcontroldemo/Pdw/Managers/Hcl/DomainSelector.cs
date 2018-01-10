
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using Pdw.Core;

namespace Pdw.Managers.Hcl
{
    public partial class DomainSelector : Form
    {
        private bool _isOK = false;

        public DomainSelector()
        {
            InitializeComponent();

            BindData();
        }

        private void BindData()
        {
            _isOK = false;
            TemplateInfo templateInfo = WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            InternalBookmark ibm = templateInfo.InternalBookmark;

            BindComboBoxColor(cboDSCColor, ibm.DocumentSpecificColor);
            
            foreach (InternalBookmarkDomain ibmDomain in ibm.InternalBookmarkDomains)
                AddDomain( ibmDomain.DomainName, ibmDomain.Color);
        }

        public string DocumentSpecificConditionColor
        {
            get
            {
                return cboDSCColor.Text;
            }
        }

        public Dictionary<string, string> DataTagColor
        {
            get
            {
                Dictionary<string, string> domainNames = new Dictionary<string, string>();
                if (_isOK)
                {
                    string dscColor = cboDSCColor.Text;
                    foreach (DataGridViewRow row in dgvData.Rows)
                    {
                        string domainName = row.Cells[ColDomain.Name].Value as string;
                        if (string.IsNullOrWhiteSpace(domainName))
                            continue;

                        string dataTagColor = row.Cells[ColColor.Name].Value as string;
                        if (string.IsNullOrWhiteSpace(dataTagColor))
                            continue;

                        if (!domainNames.ContainsKey(domainName))
                            domainNames.Add(domainName, dataTagColor);
                    }
                }

                return domainNames;
            }
        }
        
        public bool IsHighlight { get; private set; }

        private void btnUnhighlight_Click(object sender, EventArgs e)
        {
            _isOK = true;
            IsHighlight = false;
            Close();
        }

        private void btnHightlight_Click(object sender, System.EventArgs e)
        {
            _isOK = true;
            IsHighlight = true;
            Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            _isOK = false;
            Close();
        }

        private void AddDomain(string domainName, string color)
        {
            dgvData.Rows.Add(domainName, color);
        }

        private void BindComboBoxColor(ComboBox cboColor, string selectedColor)
        {
            int itemIndex = -1;
            int selectedItem = -1;
            foreach (string colorName in ProntoMarkup.Colors.Keys)
            {
                itemIndex++;
                cboColor.Items.Add(colorName);
                if (colorName.Equals(selectedColor, StringComparison.OrdinalIgnoreCase))
                    selectedItem = itemIndex;
            }
            cboColor.SelectedIndex = selectedItem;
            cboColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            cboColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cboColor.DrawItem += new DrawItemEventHandler(cboColor_DrawItem);
        }

        private void cboColor_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                ComboBox cboColor = sender as ComboBox;
                e.DrawBackground();

                int indexItem = e.Index;
                if (indexItem < 0 || indexItem >= cboColor.Items.Count)
                    return;

                string text = cboColor.Items[indexItem].ToString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    Brush brush = new SolidBrush(Color.FromName(text));
                    e.Graphics.FillRectangle(brush, e.Bounds.X, e.Bounds.Y + 1, 12, 12);
                }
                e.Graphics.DrawString(text, cboColor.Font, System.Drawing.Brushes.Black,
                    new RectangleF(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));

                e.DrawFocusRectangle();
            }
            catch { }
        }
    }
}
