namespace Pdw.Managers.Hcl
{
	partial class BookmarkControl
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
            this.components = new System.ComponentModel.Container();
            this.txtBookmarkName = new System.Windows.Forms.TextBox();
            this.lstBookmarks = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmsDelete = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsGoTo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsHighlight = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rdbStartWith = new System.Windows.Forms.RadioButton();
            this.rdbContain = new System.Windows.Forms.RadioButton();
            this.cmdDelete = new SplitButtonDemo.ButtonDropDown();
            this.cmdGoto = new SplitButtonDemo.ButtonDropDown();
            this.cmdHightlight = new SplitButtonDemo.ButtonDropDown();
            this.SuspendLayout();
            // 
            // txtBookmarkName
            // 
            this.txtBookmarkName.Location = new System.Drawing.Point(12, 59);
            this.txtBookmarkName.Name = "txtBookmarkName";
            this.txtBookmarkName.Size = new System.Drawing.Size(230, 20);
            this.txtBookmarkName.TabIndex = 0;
            this.txtBookmarkName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtBookmarkName_KeyUp);
            // 
            // lstBookmarks
            // 
            this.lstBookmarks.FormattingEnabled = true;
            this.lstBookmarks.Location = new System.Drawing.Point(12, 84);
            this.lstBookmarks.Name = "lstBookmarks";
            this.lstBookmarks.Size = new System.Drawing.Size(230, 238);
            this.lstBookmarks.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(248, 296);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 26);
            this.button1.TabIndex = 2;
            this.button1.Text = "&Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Bookmark Name:";
            // 
            // cmsDelete
            // 
            this.cmsDelete.Name = "contextMenuStrip1";
            this.cmsDelete.Size = new System.Drawing.Size(61, 4);
            // 
            // cmsGoTo
            // 
            this.cmsGoTo.Name = "cmsGoTo";
            this.cmsGoTo.Size = new System.Drawing.Size(61, 4);
            // 
            // cmsHighlight
            // 
            this.cmsHighlight.Name = "cmsHighlight";
            this.cmsHighlight.Size = new System.Drawing.Size(61, 4);
            // 
            // rdbStartWith
            // 
            this.rdbStartWith.AutoSize = true;
            this.rdbStartWith.Checked = true;
            this.rdbStartWith.Location = new System.Drawing.Point(104, 11);
            this.rdbStartWith.Name = "rdbStartWith";
            this.rdbStartWith.Size = new System.Drawing.Size(69, 17);
            this.rdbStartWith.TabIndex = 7;
            this.rdbStartWith.TabStop = true;
            this.rdbStartWith.Text = "Start with";
            this.rdbStartWith.UseVisualStyleBackColor = true;
            // 
            // rdbContain
            // 
            this.rdbContain.AutoSize = true;
            this.rdbContain.Location = new System.Drawing.Point(104, 34);
            this.rdbContain.Name = "rdbContain";
            this.rdbContain.Size = new System.Drawing.Size(61, 17);
            this.rdbContain.TabIndex = 8;
            this.rdbContain.Text = "Contain";
            this.rdbContain.UseVisualStyleBackColor = true;
            // 
            // cmdDelete
            // 
            this.cmdDelete.ClickedImage = "Clicked";
            this.cmdDelete.ContextMenuLeft = 97;
            this.cmdDelete.DisabledImage = "Disabled";
            this.cmdDelete.FocusedImage = "Focused";
            this.cmdDelete.HoverImage = "Hover";
            this.cmdDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdDelete.ImageKey = "Normal";
            this.cmdDelete.Location = new System.Drawing.Point(248, 84);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.NormalImage = "Normal";
            this.cmdDelete.Size = new System.Drawing.Size(86, 25);
            this.cmdDelete.TabIndex = 4;
            this.cmdDelete.Text = "    Delete";
            this.cmdDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.ButtonClick += new System.EventHandler(this.cmdDelete_ButtonClick);
            // 
            // cmdGoto
            // 
            this.cmdGoto.ClickedImage = "Clicked";
            this.cmdGoto.ContextMenuLeft = 97;
            this.cmdGoto.DisabledImage = "Disabled";
            this.cmdGoto.FocusedImage = "Focused";
            this.cmdGoto.HoverImage = "Hover";
            this.cmdGoto.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdGoto.ImageKey = "Normal";
            this.cmdGoto.Location = new System.Drawing.Point(248, 115);
            this.cmdGoto.Name = "cmdGoto";
            this.cmdGoto.NormalImage = "Normal";
            this.cmdGoto.Size = new System.Drawing.Size(86, 26);
            this.cmdGoto.TabIndex = 5;
            this.cmdGoto.Text = "     Go to";
            this.cmdGoto.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdGoto.UseVisualStyleBackColor = true;
            this.cmdGoto.ButtonClick += new System.EventHandler(this.cmdGoto_ButtonClick);
            this.cmdGoto.Click += new System.EventHandler(this.cmdGoto_Click);
            // 
            // cmdHightlight
            // 
            this.cmdHightlight.ClickedImage = "Clicked";
            this.cmdHightlight.ContextMenuLeft = 97;
            this.cmdHightlight.DisabledImage = "Disabled";
            this.cmdHightlight.FocusedImage = "Focused";
            this.cmdHightlight.HoverImage = "Hover";
            this.cmdHightlight.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdHightlight.ImageKey = "Normal";
            this.cmdHightlight.Location = new System.Drawing.Point(248, 147);
            this.cmdHightlight.Name = "cmdHightlight";
            this.cmdHightlight.NormalImage = "Normal";
            this.cmdHightlight.Size = new System.Drawing.Size(86, 27);
            this.cmdHightlight.TabIndex = 6;
            this.cmdHightlight.Text = "  HighLight";
            this.cmdHightlight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdHightlight.UseVisualStyleBackColor = true;
            this.cmdHightlight.Visible = false;
            this.cmdHightlight.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmdHightlight_MouseClick);
            // 
            // BookmarkControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 334);
            this.Controls.Add(this.txtBookmarkName);
            this.Controls.Add(this.rdbStartWith);
            this.Controls.Add(this.cmdDelete);
            this.Controls.Add(this.lstBookmarks);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rdbContain);
            this.Controls.Add(this.cmdGoto);
            this.Controls.Add(this.cmdHightlight);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BookmarkControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bookmark Control";
            this.Load += new System.EventHandler(this.BookmarkControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.TextBox txtBookmarkName;
        private System.Windows.Forms.ListBox lstBookmarks;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private SplitButtonDemo.ButtonDropDown cmdDelete;
        private SplitButtonDemo.ButtonDropDown cmdGoto;
        private SplitButtonDemo.ButtonDropDown cmdHightlight;
        private System.Windows.Forms.ContextMenuStrip cmsDelete;
        private System.Windows.Forms.ContextMenuStrip cmsGoTo;
        private System.Windows.Forms.ContextMenuStrip cmsHighlight;
        private System.Windows.Forms.RadioButton rdbStartWith;
        private System.Windows.Forms.RadioButton rdbContain;
	}
}