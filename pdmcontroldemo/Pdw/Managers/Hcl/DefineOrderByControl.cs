using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Office.Interop.Word;

namespace Pdw.Managers.Hcl
{
    public partial class DefineOrderByControl : Form
    {
        #region private variants
        private Bookmark _foreach;
        private List<Bookmark> _dataTags;
        #endregion

        #region public properties
        public string OrderByValue { get; private set; }
        #endregion

        #region constructors
        public DefineOrderByControl(Bookmark foreachBm, List<Bookmark> dataTags)
        {
            InitializeComponent();

            _foreach = foreachBm;
            _dataTags = dataTags;

            dgvTags.AutoGenerateColumns = false;
            dgvTags.Rows.Clear();
            BindData();
            colName.ReadOnly = true;
            colSort.ReadOnly = false;
        }
        #endregion

        #region events: OK, Up, Down
        private void btnOK_Click(object sender, System.EventArgs e)
        {
            string sort = string.Empty;
            foreach (DataGridViewRow row in dgvTags.Rows)
            {
                if (row.Cells[1].Value.ToString() == Core.OrderByType.Asc.ToString())
                    sort += row.Cells[0].Value.ToString() + Core.Constants.OrderBy.Concat + Core.Constants.OrderBy.AscMark + Core.Constants.OrderBy.Delimiter;
                else if (row.Cells[1].Value.ToString() == Core.OrderByType.Desc.ToString())
                    sort += row.Cells[0].Value.ToString() + Core.Constants.OrderBy.Concat + Core.Constants.OrderBy.DescMark + Core.Constants.OrderBy.Delimiter;
            }
            if (sort != string.Empty)
                sort = Core.Constants.OrderBy.ForeachSortMark + sort.Remove(sort.Length - 2) + Core.Constants.OrderBy.CloseTag;

            OrderByValue = sort;
            this.Close();
        }

        private void btnUp_Click(object sender, System.EventArgs e)
        {
            if (dgvTags.SelectedRows != null && dgvTags.SelectedRows.Count > 0)
            {
                int currentIndex = dgvTags.SelectedRows[0].Index;
                if (currentIndex > 0)
                {
                    DataGridViewRow currentRow = dgvTags.SelectedRows[0];

                    DataGridViewRow newRow = CreateRow(currentRow.Cells[0].Value, currentRow.Cells[1].Value);
                    dgvTags.Rows.Insert(currentIndex - 1, newRow);
                    dgvTags.Rows.Remove(currentRow);

                    newRow.Selected = true;
                }
            }
        }

        private void btnDown_Click(object sender, System.EventArgs e)
        {
            if (dgvTags.SelectedRows != null && dgvTags.SelectedRows.Count > 0)
            {
                int currentIndex = dgvTags.SelectedRows[0].Index;
                if (currentIndex < dgvTags.RowCount - 1)
                {
                    DataGridViewRow currentRow = dgvTags.SelectedRows[0];

                    DataGridViewRow newRow = CreateRow(currentRow.Cells[0].Value, currentRow.Cells[1].Value);
                    if (currentIndex >= dgvTags.RowCount - 2)
                        dgvTags.Rows.Add(newRow);
                    else
                        dgvTags.Rows.Insert(currentIndex + 2, newRow);
                    dgvTags.Rows.Remove(currentRow);

                    newRow.Selected = true;
                }
            }
        }
        #endregion

        #region helper methods
        private void BindData()
        {
            Dictionary<string, bool> dataTags = new Dictionary<string, bool>();
            Dictionary<string, Core.OrderByType> sorted = 
                Core.MarkupUtilities.GetOldOrderBy(Core.MarkupUtilities.GetRangeText(_foreach.Range), true);

            foreach (Bookmark bm in _dataTags)
            {
                string bmText = Core.MarkupUtilities.GetRangeText(bm.Range);
                if (!dataTags.ContainsKey(bmText))
                {
                    string orderValue = Core.OrderByType.None.ToString();
                    if (sorted.ContainsKey(bmText))
                        orderValue = sorted[bmText].ToString();

                    DataGridViewRow row = CreateRow(bmText, orderValue);
                    dgvTags.Rows.Add(row);
                    dataTags.Add(bmText, false);
                }
            }
        }

        private DataGridViewRow CreateRow(object name, object orderValue)
        {
            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell tagCell = new DataGridViewTextBoxCell();
            // tagCell.ReadOnly = true;
            tagCell.Value = name;
            row.Cells.Add(tagCell);

            DataGridViewComboBoxCell orderCell = new DataGridViewComboBoxCell();
            // orderCell.ReadOnly = false;
            orderCell.Items.Add(Core.OrderByType.None.ToString()); 
            orderCell.Items.Add(Core.OrderByType.Asc.ToString()); 
            orderCell.Items.Add(Core.OrderByType.Desc.ToString());
            orderCell.Value = orderValue;
            row.Cells.Add(orderCell);

            return row;
        }
        #endregion
    }    
}
