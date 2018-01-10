using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using Pdw.Properties;

namespace Pdw.Managers.Hcl
{
    public partial class CollapsiblePanel : Panel
    {
        #region Private properties
        private bool collapse = false;
        private string headerText;
        private Color headerForeColor;
        private Font headerFont;
        private Position headerTextPos = Position.Left;
        #endregion

        #region Public properties
        [DefaultValue(false)]
        [Description("Set true if collapse")]
        public bool Collapse
        {
            get { return collapse; }
            set
            {
                collapse = value;
                CollapseOrExpand();
                //Refresh();
            }
        }

        [Description("Header text")]
        public string HeaderText
        {
            get { return headerText; }
            set
            {
                headerText = value;
                //Refresh();
            }
        }

        [Description("Header text position")]
        public Position HeaderTextPosition
        {
            get { return headerTextPos; }
            set
            {
                headerTextPos = value;
                //Refresh();
            }
        }

        [Description("Color of header text")]
        public Color HeaderTextColor
        {
            get { return headerForeColor; }
            set
            {
                headerForeColor = value;
                //Refresh();
            }
        }

        [Description("Font of header text")]
        public Font HeaderFont
        {
            get { return headerFont; }
            set
            {
                headerFont = value;
                //Refresh();
            }
        }
        #endregion

        private int CurrentHeight = 0;
        public delegate void CollapseEventHandler(EventArgs e);
        public event CollapseEventHandler CollapseExpand;

        #region Constrcutor
        public CollapsiblePanel()
        {
            InitializeComponent();
            this.pnlHeader.Width = this.Width - 1;

            headerFont = new Font(Font, FontStyle.Bold);
            headerForeColor = Color.Black;
        }
        #endregion

        #region Overwrite methods (OnPaint, OnResize, OnSizeChange)
        /// <summary>
        /// On paint method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawHeader(e);
        }

        /// <summary>
        /// Resize panel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.pnlHeader.Width = this.Width - 1;
            //Refresh();
        }

        /// <summary>
        /// Change size
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.pnlHeader.Width = this.Width - 1;
            //Refresh();
        }
        #endregion

        #region Events (expand-collapse click, mouse enter-leave to collapse-expand icon)
        /// <summary>
        /// Change collapse status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxExpandCollapse_Click(object sender, EventArgs e)
        {
            Collapse = !Collapse;
            if (CollapseExpand != null)
                CollapseExpand(e);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxExpandCollapse_MouseEnter(object sender, EventArgs e)
        {
            UpdateCollapseExpandIcon(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxExpandCollapse_MouseLeave(object sender, EventArgs e)
        {
            UpdateCollapseExpandIcon(false);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Draw header (frame and text)
        /// </summary>
        /// <param name="e"></param>
        private void DrawHeader(PaintEventArgs e)
        {
            Rectangle headerRect = pnlHeader.ClientRectangle;

            DrawHeaderFrame(e.Graphics, headerRect);

            if (!String.IsNullOrEmpty(headerText))
                DrawHeaderText(e.Graphics, headerRect);
        }

        /// <summary>
        /// Draw frame of header
        /// </summary>
        /// <param name="g"></param>
        /// <param name="headerRect"></param>
        private void DrawHeaderFrame(Graphics g, Rectangle headerRect)
        {
            pictureBoxExpandCollapse.Location = new Point(2, 4);
        }

        /// <summary>
        /// Draw header text
        /// </summary>
        /// <param name="g"></param>
        /// <param name="headerRect"></param>
        private void DrawHeaderText(Graphics g, Rectangle headerRect)
        {
            int padding = 2;
            int startX = pictureBoxExpandCollapse.Location.X + pictureBoxExpandCollapse.Width + padding;

            PointF headerTextPosition = new PointF();
            Size headerTextSize = TextRenderer.MeasureText(headerText, headerFont);

            if (headerTextSize.Width >= headerRect.Width - startX) // too long header text
            {
                RectangleF rectLayout =
                    new RectangleF(startX,
                    (float)(headerRect.Height - headerTextSize.Height) / 2,
                    (float)headerRect.Width - startX,
                    (float)headerTextSize.Height);
                StringFormat format = new StringFormat();
                format.Trimming = StringTrimming.EllipsisWord;
                g.DrawString(headerText, headerFont, new SolidBrush(headerForeColor),
                    rectLayout, format);
            }
            else
            {
                switch (headerTextPos)
                {
                    case Position.Left:
                        headerTextPosition.X = startX;
                        break;
                    case Position.Middle:
                        headerTextPosition.X = (headerRect.Width - startX - headerTextSize.Width) / 2;
                        break;
                    case Position.Right:
                        headerTextPosition.X = Math.Max(startX, headerRect.Width - headerTextSize.Width - padding);
                        break;
                }

                headerTextPosition.Y = (headerRect.Height - headerTextSize.Height) / 2;
                g.DrawString(headerText, headerFont, new SolidBrush(headerForeColor),
                    headerTextPosition);
            }
        }

        /// <summary>
        /// Collapse or expand panel
        /// </summary>
        private void CollapseOrExpand()
        {
            // ngocbv: update for content is panel
            foreach (Control ctrlChild in this.Controls)
            {
                if (ctrlChild != pnlHeader)
                    ctrlChild.Visible = !Collapse;
            }

            UpdateCollapseExpandIcon(false);
        }

        private void UpdateCollapseExpandIcon(bool isHover)
        {
            if (collapse)
            {
                CurrentHeight = this.Height;
                this.Height = pnlHeader.Height + 3;
                pictureBoxExpandCollapse.Image = isHover ? Resources.CollapseHover : Resources.Collapse;
            }
            else
            {
                this.Height = CurrentHeight;
                pictureBoxExpandCollapse.Image = isHover ? Resources.ExpandHover : Resources.Expand;
            }
        }
        #endregion
    }

    public enum Position
    {
        Left,
        Middle,
        Right
    }
}