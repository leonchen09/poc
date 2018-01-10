using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CPUMonitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = (int)(performanceCounter1.NextValue());            
            label1.Text = "Processor Time: " + progressBar1.Value.ToString() + "%";
            //label2.Text = "Total memory usage: ";
            //long totalMemory = 0;
            //System.Diagnostics.Process[] MyProcesses = Process.GetProcesses();
            //  foreach (Process MyProcess in MyProcesses)
            //  {
            //      //label2.Text += MyProcess.ProcessName + ":" + MyProcess.WorkingSet64 / 1024 + "Kb;";
            //      totalMemory += MyProcess.WorkingSet64;
            //  }
            //  label2.Text += totalMemory / 1024 + "kb";
              Getmemory();
        }
        public void Getmemory()
        {
            MEMORY_INFO MemInfo;
            MemInfo = new MEMORY_INFO();
            getmemory.GlobalMemoryStatus(ref MemInfo);

            label3.Text = MemInfo.dwMemoryLoad.ToString() + "%的内存正在使用 ";
            label4.Text = "物理内存共有 " + MemInfo.dwTotalPhys /1024 + "kb ";
            label5.Text = "可使用的物理内存有 " + MemInfo.dwAvailPhys / 1024 + "kb ";
            label6.Text = "交换文件总大小为 " + MemInfo.dwTotalPageFile / 1024 + "kb ";
            label7.Text = "尚可交换文件大小为 " + MemInfo.dwAvailPageFile / 1024 + "kb ";
            label8.Text = "总虚拟内存有 " + MemInfo.dwTotalVirtual / 1024 + "kb ";
        }
    }
    public struct MEMORY_INFO
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public uint dwTotalPhys;
        public uint dwAvailPhys;
        public uint dwTotalPageFile;
        public uint dwAvailPageFile;
        public uint dwTotalVirtual;
        public uint dwAvailVirtual;
    }
    public class getmemory
    {
        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);

    }
}
