namespace Pdw.Managers.Hcl
{
    partial class DomainSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHighlight = new System.Windows.Forms.Button();
            this.btnUnhighlight = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboDSCColor = new System.Windows.Forms.ComboBox();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.ColDomain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColColor = new Pdw.Core.DataGridViewColorBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 277);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnHighlight
            // 
            this.btnHighlight.Location = new System.Drawing.Point(45, 277);
            this.btnHighlight.Name = "btnHighlight";
            this.btnHighlight.Size = new System.Drawing.Size(92, 23);
            this.btnHighlight.TabIndex = 2;
            this.btnHighlight.Text = "Highlight";
            this.btnHighlight.UseVisualStyleBackColor = true;
            this.btnHighlight.Click += new System.EventHandler(this.btnHightlight_Click);
            // 
            // btnUnhighlight
            // 
            this.btnUnhighlight.Location = new System.Drawing.Point(143, 277);
            this.btnUnhighlight.Name = "btnUnhighlight";
            this.btnUnhighlight.Size = new System.Drawing.Size(92, 23);
            this.btnUnhighlight.TabIndex = 4;
            this.btnUnhighlight.Text = "Un-Highlight";
            this.btnUnhighlight.UseVisualStyleBackColor = true;
            this.btnUnhighlight.Click += new System.EventHandler(this.btnUnhighlight_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Document-specific Condition color";
            // 
            // cboDSCColor
            // 
            this.cboDSCColor.FormattingEnabled = true;
            this.cboDSCColor.Location = new System.Drawing.Point(183, 10);
            this.cboDSCColor.Name = "cboDSCColor";
            this.cboDSCColor.Size = new System.Drawing.Size(150, 21);
            this.cboDSCColor.TabIndex = 6;
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColDomain,
            this.ColColor});
            this.dgvData.Location = new System.Drawing.Point(8, 37);
            this.dgvData.Name = "dgvData";
            this.dgvData.RowHeadersVisible = false;
            this.dgvData.Size = new System.Drawing.Size(324, 234);
            this.dgvData.TabIndex = 7;
            // 
            // ColDomain
            // 
            this.ColDomain.HeaderText = "Domain Name";
            this.ColDomain.Name = "ColDomain";
            this.ColDomain.ReadOnly = true;
            this.ColDomain.Width = 171;
            // 
            // ColColor
            // 
            this.ColColor.HeaderText = "Data Tag Color";
            this.ColColor.Name = "ColColor";
            this.ColColor.Width = 150;
            // 
            // DomainSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 305);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.cboDSCColor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnUnhighlight);
            this.Controls.Add(this.btnHighlight);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DomainSelector";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select color for highlight/un-highlight bookmarks";
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnHighlight;
        private System.Windows.Forms.Button btnUnhighlight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDSCColor;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDomain;
        private Core.DataGridViewColorBoxColumn ColColor;
    }
}