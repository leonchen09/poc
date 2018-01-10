using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterMark
{
    public class CallBack
    {
        public delegate void callBack1(string param);
        public delegate void callBack2();

        
        public void callBack1Imp(string param)
        {
            Console.WriteLine("value:" + param);
            MessageBox.Show("value:" + param);
        }

        public void callBack2Imp()
        {
            Console.WriteLine("callback2");
            MessageBox.Show("callback2");
        }
    }
}
