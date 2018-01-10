using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ActiveXDemo
{

    
    [Guid("1494F1B5-6127-462C-8E1F-FFAB148AB82E")]
    public partial class UserControl1 : UserControl
    {
        [DllImport("DllTest.dll")]
        public static extern int DllRegisterServer();//注册时用
        [DllImport("DllTest.dll")]
        public static extern int DllUnregisterServer();//取消注册时用
        public UserControl1()
        {
            int i = DllRegisterServer();
            InitializeComponent();
        }
    }
}
