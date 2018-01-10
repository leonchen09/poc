using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Security;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        [DllImport("user32.dll")]
        static extern int GetDesktopWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetProcessWindowStation();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern IntPtr GetThreadDesktop(IntPtr dwThread);

        [DllImport("user32.dll")]
        static extern IntPtr OpenWindowStation(string a, bool b, int c);

        [DllImport("user32.dll")]
        static extern IntPtr OpenDesktop(string lpszDesktop, uint dwFlags,
        bool fInherit, uint dwDesiredAccess);

        [DllImport("user32.dll")]
        static extern IntPtr CloseDesktop(IntPtr p);

        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern IntPtr RpcImpersonateClient(int i);


        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern IntPtr RpcRevertToSelf();

        [DllImport("user32.dll")]
        static extern IntPtr SetThreadDesktop(IntPtr a);

        [DllImport("user32.dll")]
        static extern IntPtr SetProcessWindowStation(IntPtr a);
        [DllImport("user32.dll")]
        static extern IntPtr CloseWindowStation(IntPtr a);

        void FormShow()
        {

            GetDesktopWindow();
            IntPtr hwinstaSave = GetProcessWindowStation();
            IntPtr dwThreadId = GetCurrentThreadId();
            IntPtr hdeskSave = GetThreadDesktop(dwThreadId);
            IntPtr hwinstaUser = OpenWindowStation("WinSta0", false, 33554432);
            if (hwinstaUser == IntPtr.Zero)
            {
                RpcRevertToSelf();
                return;
            }
            SetProcessWindowStation(hwinstaUser);
            IntPtr hdeskUser = OpenDesktop("Default", 0, false, 33554432);
            RpcRevertToSelf();
            if (hdeskUser == IntPtr.Zero)
            {
                SetProcessWindowStation(hwinstaSave);
                CloseWindowStation(hwinstaUser);
                return;
            }
            SetThreadDesktop(hdeskUser);

            IntPtr dwGuiThreadId = dwThreadId;

            Form1 f = new Form1();
            System.Windows.Forms.Application.Run(f);


            dwGuiThreadId = IntPtr.Zero;
            SetThreadDesktop(hdeskSave);
            SetProcessWindowStation(hwinstaSave);
            CloseDesktop(hdeskUser);
            CloseWindowStation(hwinstaUser);
        }
 


        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Console.Write("service starting");
            Thread thread = new Thread(new ThreadStart(StartTray));
            thread.Start();
            Application word = new Application();
            word.Visible = true;
            //StartTray2();
        }

        private void StartTray()
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
            info.FileName = @"D:\work\source\POC\WinServicesControl\WinServicesControl\bin\Release\WinServicesControl.exe";
            //info.FileName = @"C:\Windows\System32\notepad.exe";
            //info.Arguments = "";
            info.WindowStyle = ProcessWindowStyle.Normal;
            info.UseShellExecute = false;
            Process pro = Process.Start(info);
            pro.WaitForExit(); 
        }


        protected override void OnStop()
        {
        }
    }
}
