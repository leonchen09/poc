namespace Pdw.PreviewOsql
{
    partial class PreviewOsqlForm
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
            this.txtOsql = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabContent = new System.Windows.Forms.TabControl();
            this.tabOsql = new System.Windows.Forms.TabPage();
            this.tabJOsql = new System.Windows.Forms.TabPage();
            this.txtJOsql = new System.Windows.Forms.TextBox();
            this.tabXslt = new System.Windows.Forms.TabPage();
            this.txtXslt = new System.Windows.Forms.TextBox();
            this.tabRenderArgument = new System.Windows.Forms.TabPage();
            this.txtRenderArgument = new System.Windows.Forms.TextBox();
            this.tabJRenderArgument = new System.Windows.Forms.TabPage();
            this.txtJRenderArgument = new System.Windows.Forms.TextBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMDB = new System.Windows.Forms.TabPage();
            this.txtMDB = new System.Windows.Forms.TextBox();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabRenderArgumentDomainValue = new System.Windows.Forms.TabPage();
            this.txtRenderArgDomain = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabContent.SuspendLayout();
            this.tabOsql.SuspendLayout();
            this.tabJOsql.SuspendLayout();
            this.tabXslt.SuspendLayout();
            this.tabRenderArgument.SuspendLayout();
            this.tabJRenderArgument.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabMDB.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabRenderArgumentDomainValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtOsql
            // 
            this.txtOsql.BackColor = System.Drawing.SystemColors.Window;
            this.txtOsql.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOsql.Location = new System.Drawing.Point(3, 3);
            this.txtOsql.Multiline = true;
            this.txtOsql.Name = "txtOsql";
            this.txtOsql.ReadOnly = true;
            this.txtOsql.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOsql.Size = new System.Drawing.Size(496, 242);
            this.txtOsql.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Location = new System.Drawing.Point(442, 636);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 25);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tabContent
            // 
            this.tabContent.Controls.Add(this.tabOsql);
            this.tabContent.Controls.Add(this.tabJOsql);
            this.tabContent.Controls.Add(this.tabXslt);
            this.tabContent.Controls.Add(this.tabRenderArgument);
            this.tabContent.Controls.Add(this.tabJRenderArgument);
            this.tabContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabContent.Location = new System.Drawing.Point(0, 0);
            this.tabContent.Name = "tabContent";
            this.tabContent.SelectedIndex = 0;
            this.tabContent.Size = new System.Drawing.Size(510, 276);
            this.tabContent.TabIndex = 2;
            // 
            // tabOsql
            // 
            this.tabOsql.Controls.Add(this.txtOsql);
            this.tabOsql.Location = new System.Drawing.Point(4, 24);
            this.tabOsql.Name = "tabOsql";
            this.tabOsql.Padding = new System.Windows.Forms.Padding(3);
            this.tabOsql.Size = new System.Drawing.Size(502, 248);
            this.tabOsql.TabIndex = 0;
            this.tabOsql.Text = "Osql";
            this.tabOsql.UseVisualStyleBackColor = true;
            // 
            // tabJOsql
            // 
            this.tabJOsql.Controls.Add(this.txtJOsql);
            this.tabJOsql.Location = new System.Drawing.Point(4, 22);
            this.tabJOsql.Name = "tabJOsql";
            this.tabJOsql.Padding = new System.Windows.Forms.Padding(3);
            this.tabJOsql.Size = new System.Drawing.Size(502, 250);
            this.tabJOsql.TabIndex = 1;
            this.tabJOsql.Text = "JOsql";
            this.tabJOsql.UseVisualStyleBackColor = true;
            // 
            // txtJOsql
            // 
            this.txtJOsql.BackColor = System.Drawing.SystemColors.Window;
            this.txtJOsql.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtJOsql.Location = new System.Drawing.Point(3, 3);
            this.txtJOsql.Multiline = true;
            this.txtJOsql.Name = "txtJOsql";
            this.txtJOsql.ReadOnly = true;
            this.txtJOsql.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtJOsql.Size = new System.Drawing.Size(496, 244);
            this.txtJOsql.TabIndex = 0;
            // 
            // tabXslt
            // 
            this.tabXslt.Controls.Add(this.txtXslt);
            this.tabXslt.Location = new System.Drawing.Point(4, 22);
            this.tabXslt.Name = "tabXslt";
            this.tabXslt.Padding = new System.Windows.Forms.Padding(3);
            this.tabXslt.Size = new System.Drawing.Size(502, 250);
            this.tabXslt.TabIndex = 2;
            this.tabXslt.Text = "Xslt";
            this.tabXslt.UseVisualStyleBackColor = true;
            // 
            // txtXslt
            // 
            this.txtXslt.BackColor = System.Drawing.SystemColors.Window;
            this.txtXslt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtXslt.Location = new System.Drawing.Point(3, 3);
            this.txtXslt.Multiline = true;
            this.txtXslt.Name = "txtXslt";
            this.txtXslt.ReadOnly = true;
            this.txtXslt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtXslt.Size = new System.Drawing.Size(496, 244);
            this.txtXslt.TabIndex = 1;
            // 
            // tabRenderArgument
            // 
            this.tabRenderArgument.Controls.Add(this.txtRenderArgument);
            this.tabRenderArgument.Location = new System.Drawing.Point(4, 22);
            this.tabRenderArgument.Name = "tabRenderArgument";
            this.tabRenderArgument.Padding = new System.Windows.Forms.Padding(3);
            this.tabRenderArgument.Size = new System.Drawing.Size(502, 250);
            this.tabRenderArgument.TabIndex = 3;
            this.tabRenderArgument.Text = "RenderArgument";
            this.tabRenderArgument.UseVisualStyleBackColor = true;
            // 
            // txtRenderArgument
            // 
            this.txtRenderArgument.BackColor = System.Drawing.SystemColors.Window;
            this.txtRenderArgument.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRenderArgument.Location = new System.Drawing.Point(3, 3);
            this.txtRenderArgument.Multiline = true;
            this.txtRenderArgument.Name = "txtRenderArgument";
            this.txtRenderArgument.ReadOnly = true;
            this.txtRenderArgument.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRenderArgument.Size = new System.Drawing.Size(496, 244);
            this.txtRenderArgument.TabIndex = 2;
            // 
            // tabJRenderArgument
            // 
            this.tabJRenderArgument.Controls.Add(this.txtJRenderArgument);
            this.tabJRenderArgument.Location = new System.Drawing.Point(4, 22);
            this.tabJRenderArgument.Name = "tabJRenderArgument";
            this.tabJRenderArgument.Padding = new System.Windows.Forms.Padding(3);
            this.tabJRenderArgument.Size = new System.Drawing.Size(502, 250);
            this.tabJRenderArgument.TabIndex = 4;
            this.tabJRenderArgument.Text = "JRenderArgument";
            this.tabJRenderArgument.UseVisualStyleBackColor = true;
            // 
            // txtJRenderArgument
            // 
            this.txtJRenderArgument.BackColor = System.Drawing.SystemColors.Window;
            this.txtJRenderArgument.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtJRenderArgument.Location = new System.Drawing.Point(3, 3);
            this.txtJRenderArgument.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.txtJRenderArgument.Multiline = true;
            this.txtJRenderArgument.Name = "txtJRenderArgument";
            this.txtJRenderArgument.ReadOnly = true;
            this.txtJRenderArgument.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtJRenderArgument.Size = new System.Drawing.Size(496, 244);
            this.txtJRenderArgument.TabIndex = 3;
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPreview.Location = new System.Drawing.Point(7, 638);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 7;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabContent);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(510, 436);
            this.splitContainer1.SplitterDistance = 276;
            this.splitContainer1.TabIndex = 8;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabMDB);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(510, 156);
            this.tabControl1.TabIndex = 0;
            // 
            // tabMDB
            // 
            this.tabMDB.Controls.Add(this.txtMDB);
            this.tabMDB.Location = new System.Drawing.Point(4, 24);
            this.tabMDB.Name = "tabMDB";
            this.tabMDB.Padding = new System.Windows.Forms.Padding(3);
            this.tabMDB.Size = new System.Drawing.Size(502, 128);
            this.tabMDB.TabIndex = 1;
            this.tabMDB.Text = "MDB";
            this.tabMDB.UseVisualStyleBackColor = true;
            // 
            // txtMDB
            // 
            this.txtMDB.BackColor = System.Drawing.SystemColors.Window;
            this.txtMDB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMDB.Location = new System.Drawing.Point(3, 3);
            this.txtMDB.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.txtMDB.Multiline = true;
            this.txtMDB.Name = "txtMDB";
            this.txtMDB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMDB.Size = new System.Drawing.Size(496, 122);
            this.txtMDB.TabIndex = 3;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabRenderArgumentDomainValue);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(510, 179);
            this.tabControl2.TabIndex = 9;
            // 
            // tabRenderArgumentDomainValue
            // 
            this.tabRenderArgumentDomainValue.Controls.Add(this.txtRenderArgDomain);
            this.tabRenderArgumentDomainValue.Location = new System.Drawing.Point(4, 24);
            this.tabRenderArgumentDomainValue.Name = "tabRenderArgumentDomainValue";
            this.tabRenderArgumentDomainValue.Padding = new System.Windows.Forms.Padding(3);
            this.tabRenderArgumentDomainValue.Size = new System.Drawing.Size(502, 151);
            this.tabRenderArgumentDomainValue.TabIndex = 0;
            this.tabRenderArgumentDomainValue.Text = "RenderArgumentDomainValue";
            this.tabRenderArgumentDomainValue.UseVisualStyleBackColor = true;
            // 
            // txtRenderArgDomain
            // 
            this.txtRenderArgDomain.BackColor = System.Drawing.SystemColors.Window;
            this.txtRenderArgDomain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRenderArgDomain.Location = new System.Drawing.Point(3, 3);
            this.txtRenderArgDomain.Multiline = true;
            this.txtRenderArgDomain.Name = "txtRenderArgDomain";
            this.txtRenderArgDomain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRenderArgDomain.Size = new System.Drawing.Size(496, 145);
            this.txtRenderArgDomain.TabIndex = 5;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Location = new System.Drawing.Point(7, 11);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer2.Size = new System.Drawing.Size(510, 619);
            this.splitContainer2.SplitterDistance = 436;
            this.splitContainer2.TabIndex = 10;
            // 
            // PreviewOsqlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(524, 667);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.splitContainer2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(524, 642);
            this.Name = "PreviewOsqlForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Preview Osql Statement";
            this.Load += new System.EventHandler(this.PreviewOsql_Load);
            this.tabContent.ResumeLayout(false);
            this.tabOsql.ResumeLayout(false);
            this.tabOsql.PerformLayout();
            this.tabJOsql.ResumeLayout(false);
            this.tabJOsql.PerformLayout();
            this.tabXslt.ResumeLayout(false);
            this.tabXslt.PerformLayout();
            this.tabRenderArgument.ResumeLayout(false);
            this.tabRenderArgument.PerformLayout();
            this.tabJRenderArgument.ResumeLayout(false);
            this.tabJRenderArgument.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabMDB.ResumeLayout(false);
            this.tabMDB.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabRenderArgumentDomainValue.ResumeLayout(false);
            this.tabRenderArgumentDomainValue.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtOsql;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabControl tabContent;
        private System.Windows.Forms.TabPage tabOsql;
        private System.Windows.Forms.TabPage tabJOsql;
        private System.Windows.Forms.TabPage tabXslt;
        private System.Windows.Forms.TabPage tabRenderArgument;
        private System.Windows.Forms.TabPage tabJRenderArgument;
        private System.Windows.Forms.TextBox txtJOsql;
        private System.Windows.Forms.TextBox txtXslt;
        private System.Windows.Forms.TextBox txtRenderArgument;
        private System.Windows.Forms.TextBox txtJRenderArgument;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabMDB;
        private System.Windows.Forms.TextBox txtMDB;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabRenderArgumentDomainValue;
        private System.Windows.Forms.TextBox txtRenderArgDomain;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}