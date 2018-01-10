namespace Pdw.Core
{
    partial class TreeViewSpliter
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.spcTreeView = new System.Windows.Forms.SplitContainer();
            this.trvTop = new System.Windows.Forms.TreeView();
            this.trvBottom = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.spcTreeView)).BeginInit();
            this.spcTreeView.Panel1.SuspendLayout();
            this.spcTreeView.Panel2.SuspendLayout();
            this.spcTreeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // spcTreeView
            // 
            this.spcTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcTreeView.Location = new System.Drawing.Point(0, 0);
            this.spcTreeView.Name = "spcTreeView";
            this.spcTreeView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcTreeView.Panel1
            // 
            this.spcTreeView.Panel1.AutoScroll = true;
            this.spcTreeView.Panel1.Controls.Add(this.trvTop);
            // 
            // spcTreeView.Panel2
            // 
            this.spcTreeView.Panel2.AutoScroll = true;
            this.spcTreeView.Panel2.Controls.Add(this.trvBottom);
            this.spcTreeView.Panel2MinSize = 0;
            this.spcTreeView.Size = new System.Drawing.Size(229, 281);
            this.spcTreeView.SplitterDistance = 276;
            this.spcTreeView.SplitterWidth = 2;
            this.spcTreeView.TabIndex = 0;
            this.spcTreeView.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.spcTreeView_SplitterMoving);
            this.spcTreeView.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.spcTreeView_SplitterMoved);
            // 
            // trvTop
            // 
            this.trvTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvTop.Location = new System.Drawing.Point(0, 0);
            this.trvTop.Name = "trvTop";
            this.trvTop.Size = new System.Drawing.Size(229, 276);
            this.trvTop.TabIndex = 0;
            // 
            // trvBottom
            // 
            this.trvBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvBottom.Location = new System.Drawing.Point(0, 0);
            this.trvBottom.Name = "trvBottom";
            this.trvBottom.Size = new System.Drawing.Size(229, 3);
            this.trvBottom.TabIndex = 0;
            // 
            // TreeViewSpliter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spcTreeView);
            this.Name = "TreeViewSpliter";
            this.Size = new System.Drawing.Size(229, 281);
            this.spcTreeView.Panel1.ResumeLayout(false);
            this.spcTreeView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcTreeView)).EndInit();
            this.spcTreeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spcTreeView;
        private System.Windows.Forms.TreeView trvTop;
        private System.Windows.Forms.TreeView trvBottom;

    }
}
