﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinServicesControl
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FrmMian());
            Application.Run(new Form1());
        }
    }
}