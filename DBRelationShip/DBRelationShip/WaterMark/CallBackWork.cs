using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterMark
{
    public class CallBackWork
    {
        private event WaterMark.CallBack.callBack1 cb11;
        private WaterMark.CallBack.callBack2 cb21;

        public void working(string parm1, WaterMark.CallBack.callBack1 cb1, WaterMark.CallBack.callBack1 cb101,WaterMark.CallBack.callBack2 cb2)
        {
            //cb1(parm1);
            //cb2();
            this.cb11 += cb1;
            this.cb11 += cb101;
            this.cb21 = cb2;
        }

        public void working2(string param1)
        {
            this.cb11(param1);
            this.cb21();
        }

    }
}
