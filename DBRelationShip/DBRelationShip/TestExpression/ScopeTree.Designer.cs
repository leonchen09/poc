namespace TestExpression
{
    partial class ScopeTree
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Table5");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Table4", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Table3", new System.Windows.Forms.TreeNode[] {
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Table2", new System.Windows.Forms.TreeNode[] {
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Table1", new System.Windows.Forms.TreeNode[] {
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Table4");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Table3", new System.Windows.Forms.TreeNode[] {
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Table2", new System.Windows.Forms.TreeNode[] {
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Table7", new System.Windows.Forms.TreeNode[] {
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Table6", new System.Windows.Forms.TreeNode[] {
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Table3");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Table2", new System.Windows.Forms.TreeNode[] {
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Table0", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode10,
            treeNode12});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScopeTree));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(7, 7);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node5";
            treeNode1.Text = "Table5";
            treeNode1.ToolTipText = "Right Join";
            treeNode2.Name = "Node4";
            treeNode2.Text = "Table4";
            treeNode2.ToolTipText = "Inner Join";
            treeNode3.Name = "Node3";
            treeNode3.Text = "Table3";
            treeNode3.ToolTipText = "Inner Join";
            treeNode4.Name = "Node2";
            treeNode4.Text = "Table2";
            treeNode4.ToolTipText = "Left Join";
            treeNode5.Name = "Node1";
            treeNode5.Text = "Table1";
            treeNode5.ToolTipText = "Inner Join";
            treeNode6.Name = "Node10";
            treeNode6.Text = "Table4";
            treeNode6.ToolTipText = "Inner Join";
            treeNode7.Name = "Node9";
            treeNode7.Text = "Table3";
            treeNode7.ToolTipText = "Inner Join";
            treeNode8.Name = "Node8";
            treeNode8.Text = "Table2";
            treeNode8.ToolTipText = "Left Join";
            treeNode9.Name = "Node7";
            treeNode9.Text = "Table7";
            treeNode9.ToolTipText = "Inner Join";
            treeNode10.Name = "Node6";
            treeNode10.Text = "Table6";
            treeNode10.ToolTipText = "Inner Join";
            treeNode11.Name = "Node12";
            treeNode11.Text = "Table3";
            treeNode11.ToolTipText = "Inner Join";
            treeNode12.Name = "Node11";
            treeNode12.Text = "Table2";
            treeNode12.ToolTipText = "Right Join";
            treeNode13.Name = "Node0";
            treeNode13.Text = "Table0";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode13});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(243, 289);
            this.treeView1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "4kongcom-0007.png");
            this.imageList1.Images.SetKeyName(1, "4kongcom-0035.png");
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(186, 325);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 26);
            this.button1.TabIndex = 1;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ScopeTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 359);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.treeView1);
            this.Name = "ScopeTree";
            this.Text = "ScopeTree";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button button1;
    }
}