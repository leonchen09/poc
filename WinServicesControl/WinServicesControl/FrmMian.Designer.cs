namespace WinServicesControl
{
    partial class FrmMian
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMian));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.启动ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.停止ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.暂停ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.恢复暂停ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开执行文件目录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.属性ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置运行方式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.自动ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.手动ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.禁用ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加到系统启动项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.取消系统启动项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.启动ToolStripMenuItem,
            this.停止ToolStripMenuItem,
            this.暂停ToolStripMenuItem,
            this.恢复暂停ToolStripMenuItem,
            this.打开执行文件目录ToolStripMenuItem,
            this.属性ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(167, 158);
            // 
            // 启动ToolStripMenuItem
            // 
            this.启动ToolStripMenuItem.Name = "启动ToolStripMenuItem";
            this.启动ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.启动ToolStripMenuItem.Text = "启动";
            this.启动ToolStripMenuItem.Click += new System.EventHandler(this.启动ToolStripMenuItem_Click);
            // 
            // 停止ToolStripMenuItem
            // 
            this.停止ToolStripMenuItem.Name = "停止ToolStripMenuItem";
            this.停止ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.停止ToolStripMenuItem.Text = "停止";
            this.停止ToolStripMenuItem.Click += new System.EventHandler(this.停止ToolStripMenuItem_Click);
            // 
            // 暂停ToolStripMenuItem
            // 
            this.暂停ToolStripMenuItem.Name = "暂停ToolStripMenuItem";
            this.暂停ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.暂停ToolStripMenuItem.Text = "暂停";
            this.暂停ToolStripMenuItem.Click += new System.EventHandler(this.暂停ToolStripMenuItem_Click);
            // 
            // 恢复暂停ToolStripMenuItem
            // 
            this.恢复暂停ToolStripMenuItem.Name = "恢复暂停ToolStripMenuItem";
            this.恢复暂停ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.恢复暂停ToolStripMenuItem.Text = "恢复暂停";
            this.恢复暂停ToolStripMenuItem.Click += new System.EventHandler(this.恢复暂停ToolStripMenuItem_Click);
            // 
            // 打开执行文件目录ToolStripMenuItem
            // 
            this.打开执行文件目录ToolStripMenuItem.Name = "打开执行文件目录ToolStripMenuItem";
            this.打开执行文件目录ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.打开执行文件目录ToolStripMenuItem.Text = "打开执行文件目录";
            this.打开执行文件目录ToolStripMenuItem.Click += new System.EventHandler(this.打开执行文件目录ToolStripMenuItem_Click);
            // 
            // 属性ToolStripMenuItem
            // 
            this.属性ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置运行方式ToolStripMenuItem,
            this.添加到系统启动项ToolStripMenuItem,
            this.取消系统启动项ToolStripMenuItem});
            this.属性ToolStripMenuItem.Name = "属性ToolStripMenuItem";
            this.属性ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.属性ToolStripMenuItem.Text = "属性";
            // 
            // 设置运行方式ToolStripMenuItem
            // 
            this.设置运行方式ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.自动ToolStripMenuItem,
            this.手动ToolStripMenuItem,
            this.禁用ToolStripMenuItem});
            this.设置运行方式ToolStripMenuItem.Name = "设置运行方式ToolStripMenuItem";
            this.设置运行方式ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.设置运行方式ToolStripMenuItem.Text = "设置运行方式";
            // 
            // 自动ToolStripMenuItem
            // 
            this.自动ToolStripMenuItem.Name = "自动ToolStripMenuItem";
            this.自动ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.自动ToolStripMenuItem.Text = "自动";
            this.自动ToolStripMenuItem.Click += new System.EventHandler(this.自动ToolStripMenuItem_Click);
            // 
            // 手动ToolStripMenuItem
            // 
            this.手动ToolStripMenuItem.Name = "手动ToolStripMenuItem";
            this.手动ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.手动ToolStripMenuItem.Text = "手动";
            this.手动ToolStripMenuItem.Click += new System.EventHandler(this.手动ToolStripMenuItem_Click);
            // 
            // 禁用ToolStripMenuItem
            // 
            this.禁用ToolStripMenuItem.Name = "禁用ToolStripMenuItem";
            this.禁用ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.禁用ToolStripMenuItem.Text = "禁用";
            this.禁用ToolStripMenuItem.Click += new System.EventHandler(this.禁用ToolStripMenuItem_Click);
            // 
            // 添加到系统启动项ToolStripMenuItem
            // 
            this.添加到系统启动项ToolStripMenuItem.Name = "添加到系统启动项ToolStripMenuItem";
            this.添加到系统启动项ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.添加到系统启动项ToolStripMenuItem.Text = "添加到系统启动项";
            this.添加到系统启动项ToolStripMenuItem.Click += new System.EventHandler(this.添加到系统启动项ToolStripMenuItem_Click);
            // 
            // 取消系统启动项ToolStripMenuItem
            // 
            this.取消系统启动项ToolStripMenuItem.Name = "取消系统启动项ToolStripMenuItem";
            this.取消系统启动项ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.取消系统启动项ToolStripMenuItem.Text = "取消系统启动项";
            this.取消系统启动项ToolStripMenuItem.Click += new System.EventHandler(this.取消系统启动项ToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "lv.ico");
            this.imageList1.Images.SetKeyName(1, "hong.ico");
            this.imageList1.Images.SetKeyName(2, "huang.ico");
            this.imageList1.Images.SetKeyName(3, "hui.ico");
            // 
            // FrmMian
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(187, 98);
            this.Name = "FrmMian";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 启动ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 停止ToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem 暂停ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 恢复暂停ToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem 属性ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 添加到系统启动项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 取消系统启动项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置运行方式ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 自动ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 手动ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 禁用ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开执行文件目录ToolStripMenuItem;
    }
}

