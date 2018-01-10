using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdePlugin;
using System.Windows.Forms;

namespace CustomButtonDemo
{
    public class Class1
    {
        public void CustomButtonInit(CustomService cs)
        {
            cs.Btn1Event += Bnt1EventImp1;
        }

        private void Bnt1EventImp1(String message, PdePlugin.CustomEvents.CallBackParam cbp)
        {
            MessageBox.Show("in class1: " + message);
            cbp("ddd");
        }
    }
}
