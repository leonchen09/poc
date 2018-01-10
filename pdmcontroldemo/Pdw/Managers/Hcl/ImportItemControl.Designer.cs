namespace Pdw.Managers.Hcl
{
    partial class frmImportItem
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
            this.dgvImportItems = new System.Windows.Forms.DataGridView();
            this.ColSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColDomainName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColItemType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvImportItems)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvImportItems
            // 
            this.dgvImportItems.AllowUserToAddRows = false;
            this.dgvImportItems.AllowUserToDeleteRows = false;
            this.dgvImportItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvImportItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColSelect,
            this.ColDomainName,
            this.ColItemType,
            this.ColItemName});
            this.dgvImportItems.Location = new System.Drawing.Point(12, 12);
            this.dgvImportItems.Name = "dgvImportItems";
            this.dgvImportItems.Size = new System.Drawing.Size(623, 345);
            this.dgvImportItems.TabIndex = 0;
            // 
            // ColSelect
            // 
            this.ColSelect.HeaderText = "Select";
            this.ColSelect.Name = "ColSelect";
            this.ColSelect.Width = 50;
            // 
            // ColDomainName
            // 
            this.ColDomainName.HeaderText = "DomainName";
            this.ColDomainName.Name = "ColDomainName";
            // 
            // ColItemType
            // 
            this.ColItemType.HeaderText = "Item Type";
            this.ColItemType.Name = "ColItemType";
            this.ColItemType.Width = 150;
            // 
            // ColItemName
            // 
            this.ColItemName.HeaderText = "Name";
            this.ColItemName.Name = "ColItemName";
            this.ColItemName.Width = 280;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(456, 363);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(86, 30);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(548, 363);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(86, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmImportItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 405);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dgvImportItems);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Item";
            this.Load += new System.EventHandler(this.frmImportItem_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImportItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvImportItems;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDomainName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColItemType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColItemName;
    }
}