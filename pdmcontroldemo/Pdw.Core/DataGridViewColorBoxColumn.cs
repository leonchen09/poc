
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pdw.Core
{
    public class DataGridViewColorBoxColumn : DataGridViewColumn
    {
        public DataGridViewColorBoxColumn()
            : base(new DataGridViewColorBoxCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a DataGridViewColorBoxCell. 
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewColorBoxCell)))
                    throw new InvalidCastException("Must be a DataGridViewColorBoxCell");

                base.CellTemplate = value;
            }
        }
    }

    public class DataGridViewColorBoxCell : DataGridViewTextBoxCell
    {
        public DataGridViewColorBoxCell()
            : base()
        {
        }

        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            ComboboxColor ctl = DataGridView.EditingControl as ComboboxColor;
            if (this.Value == null)
                ctl.Text = (string)this.DefaultNewRowValue;
            else
                ctl.Text = (string)this.Value;
        }

        public override Type EditType
        {
            get
            {
                return typeof(ComboboxColor);
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(string);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// draw color in display mode
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="clipBounds"></param>
        /// <param name="cellBounds"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellState"></param>
        /// <param name="value"></param>
        /// <param name="formattedValue"></param>
        /// <param name="errorText"></param>
        /// <param name="cellStyle"></param>
        /// <param name="advancedBorderStyle"></param>
        /// <param name="paintParts"></param>
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            string colorName = value == null ? string.Empty : value.ToString();
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value,
                formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            Rectangle itemBound = cellBounds;
            if (!string.IsNullOrWhiteSpace(colorName))
            {
                // clear all data
                graphics.FillRectangle(Brushes.White, itemBound.X, itemBound.Y, itemBound.Width - 1, itemBound.Height - 1);

                // draw color pie
                int iconSize = 12;
                int middleY = itemBound.Y + (itemBound.Height - iconSize) / 2;
                int paddingLeft = 4;
                Brush brush = new SolidBrush(Color.FromName(colorName));
                graphics.FillRectangle(brush, itemBound.X + paddingLeft, middleY, iconSize, iconSize);

                // write string
                graphics.DrawString(colorName, cellStyle.Font, Brushes.Black,
                    new RectangleF(itemBound.X + iconSize + paddingLeft, middleY, itemBound.Width, itemBound.Height));
            }
        }
    }

    public class ComboboxColor : ComboBox, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public ComboboxColor()
        {
            // bind data
            this.Items.Add(string.Empty);
            this.Items.Add("Yellow");
            this.Items.Add("BrightGreen");
            this.Items.Add("Turquoise");
            this.Items.Add("Pink");
            this.Items.Add("Blue");
            this.Items.Add("Red");
            this.Items.Add("DarkBlue");
            this.Items.Add("Teal");
            this.Items.Add("Green");
            this.Items.Add("Violet");
            this.Items.Add("DarkRed");
            this.Items.Add("DarkYellow");
            this.Items.Add("Gray50");
            this.Items.Add("Gray25");
            this.Items.Add("Black");

            // raise event
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DrawItem += new DrawItemEventHandler(ComboboxColor_DrawItem);
            this.SelectedIndexChanged += new EventHandler(ComboboxColor_SelectedIndexChanged);
        }

        public object EditingControlFormattedValue
        {
            get
            {
                return this.FormatString;
            }
            set
            {
                if (value is String)
                {
                    try
                    {
                        this.FormatString = (string)value;
                    }
                    catch
                    {
                        this.FormatString = string.Empty;
                    }
                }
            }
        }

        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        void ComboboxColor_DrawItem(object sender, DrawItemEventArgs e)
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

        void ComboboxColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells != null && dataGridView.SelectedCells.Count > 0)
                dataGridView.SelectedCells[0].Value = this.Text;
        }
    }
}
