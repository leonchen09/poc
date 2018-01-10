using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinServicesControl
{
    public partial class FrmMian : Form
    {
        string ServiceName;

        Icon icolv;
        Icon icohong;
        Icon icohuang;
        Icon icohui;
        public FrmMian()
        {
            InitializeComponent();

            ServiceName = System.Configuration.ConfigurationManager.AppSettings["ServiceName"].ToString();

            icolv = GetIconImg(0);
            icohong = GetIconImg(1);
            icohuang = GetIconImg(2);
            icohui = GetIconImg(3);

            notifyIcon1.Icon = icohui;
            notifyIcon1.Text = ServiceName;

            启动ToolStripMenuItem.Text = "启动/重启:" + ServiceName;
            停止ToolStripMenuItem.Text = "停止:" + ServiceName;
            暂停ToolStripMenuItem.Text = "暂停:" + ServiceName;
            恢复暂停ToolStripMenuItem.Text = "恢复暂停:" + ServiceName;

            
        }
        private Icon GetIconImg(int index)
        {
            Image img = imageList1.Images[index];
            Bitmap b = new Bitmap(img);
            Icon icon = Icon.FromHandle(b.GetHicon());
            return icon;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            string ServState = ClServCtl.GetServState(ServiceName);
            string RunType = ClServCtl.GetServRunType(ServiceName);
            switch (ServState)
            {
                case""://没有服务
                    notifyIcon1.Icon = icohui ;
                    notifyIcon1.Text = "没有找到服务:" + ServiceName;
                    break;
                case "Stopped"://停止
                    notifyIcon1.Icon = icohong ;
                    notifyIcon1.Text = ServiceName + " 服务已停止("+ RunType +")";
                    break;
                case "Running"://运行
                    notifyIcon1.Icon = icolv ;
                    notifyIcon1.Text = ServiceName + " 服务正在运行(" + RunType + ")";
                    break;
                case "StartPending"://正在启动
                    notifyIcon1.Icon = icolv;
                    notifyIcon1.Text = ServiceName + " 服务正在尝试启动(" + RunType + ")";
                    break;
                case "StopPending"://正在停止
                    notifyIcon1.Icon = icohong;
                    notifyIcon1.Text = ServiceName + " 服务正在尝试停止(" + RunType + ")";
                    break;
                case "Paused":
                    notifyIcon1.Icon = icohong;
                    notifyIcon1.Text = ServiceName + " 服务已暂停(" + RunType + ")";
                    break;
                case "PausePending":
                    notifyIcon1.Icon = icohong;
                    notifyIcon1.Text = ServiceName + " 服务正在尝试暂停(" + RunType + ")";
                    break;
                case "ContinuePending":
                    notifyIcon1.Icon = icohong;
                    notifyIcon1.Text = ServiceName + " 服务正在尝试恢复运行(" + RunType + ")";
                    break;

            }
        }

        private void 启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ClServCtl.RunService(ServiceName))
            {
                MessageBox.Show("启动/重启操作失败");
            }
            
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ClServCtl.StopService(ServiceName))
            {
                MessageBox.Show("停止操作失败");
            }
        }

        private void 暂停ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ClServCtl.PauseService(ServiceName))
            {
                MessageBox.Show("暂停操作失败");
            }
        }

        private void 恢复暂停ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ClServCtl.ResumeService(ServiceName))
            {
                MessageBox.Show("恢复暂停操作失败");
            }
        }



        private void 添加到系统启动项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClServCtl.SetAutoRun("AutoRun" + ServiceName, Application.ExecutablePath))
            {
                MessageBox.Show("操作成功");
            }
            else
            {
                MessageBox.Show("操作失败");
            }
        }

        private void 取消系统启动项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClServCtl.DeleteAutoRun("AutoRun" + ServiceName);
        }

        private void 打开执行文件目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filepath = "";
            string filename = ClServCtl.GetFilePath(ServiceName);
            if (filename == "")
            {
                MessageBox.Show("操作失败,没有找到路径");
                return;
            }
            for (int i = filename.Length - 1; i > 0; i--)
            {
                if (filename.Substring(i, 1) == @"\")
                {
                    filepath = filename.Substring(0, i);
                    break;
                }
            }
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", filepath);
            }
            catch
            {
                MessageBox.Show("操作失败,没有找到合法路径");
            }
        }

        private void 自动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClServCtl.SetServRunType(ServiceName, 2);
        }

        private void 手动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClServCtl.SetServRunType(ServiceName, 3);
        }

        private void 禁用ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClServCtl.SetServRunType(ServiceName, 4);
        }



    }
}