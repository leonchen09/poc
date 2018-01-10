using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdePlugin
{
    public class CustomEvents
    {
        public delegate void CallBackParam(string key);
        public delegate void CustomButton1EventHandler(String message, CallBackParam cbp);
    }
    public class CustomService
    {
        public CustomEvents.CustomButton1EventHandler Btn1Event;
        //public event PdePlugin.CustomEvents.CustomButton1EventHandler Btn1Event;

        //public void TriggerBnt1Event(string key)
        //{
        //    Btn1Event(key);
        //}
    }

}
