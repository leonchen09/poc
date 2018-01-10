namespace Pdw.Managers.Hcl
{
    partial class ChooseDomain
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
            this.tabDomains = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tabDomains
            // 
            this.tabDomains.Location = new System.Drawing.Point(-1, 0);
            this.tabDomains.Name = "tabDomains";
            this.tabDomains.SelectedIndex = 0;
            this.tabDomains.Size = new System.Drawing.Size(462, 316);
            this.tabDomains.TabIndex = 11;
            // 
            // ChooseDomain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 314);
            this.Controls.Add(this.tabDomains);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseDomain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose domain dialog - Close for apply all accepted";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabDomains;

    }
}