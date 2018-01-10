using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security;
using WindowsService1;

namespace WinServicesControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ApplicationLoader.PROCESS_INFORMATION procInfo;
            ApplicationLoader.StartProcessAndBypassUAC(@"C:\Windows\System32\notepad.exe", out procInfo);

            return;
            ProcessStartInfo info = new ProcessStartInfo();
            //info.WorkingDirectory = @"D:\work\source\POC\WinServicesControl\WinServicesControl\bin\Release\";
            //info.CreateNoWindow = true;
            //info.UserName = "ProntoDoc";
            //string strPWD = "pronto";
            //SecureString password = new SecureString();
            //foreach (char c in strPWD.ToCharArray())
            //{ password.AppendChar(c); }
            //info.Password = password;
            //info.FileName = @"D:\work\source\POC\WinServicesControl\WinServicesControl\bin\Release\WinServicesControl.exe";
            info.FileName = @"C:\Windows\System32\notepad.exe";
            //info.Arguments = "";
            info.WindowStyle = ProcessWindowStyle.Normal;
            info.UseShellExecute = false;
            Process pro = Process.Start(info);
            pro.WaitForExit(); 

        }
    }
}
