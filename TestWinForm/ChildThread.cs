using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestWinForm
{
    class ChildThread
    {
        public void run()
        {
            Console.WriteLine("child start:" + System.DateTime.UtcNow);
            Thread.Sleep(3000);
            Console.WriteLine("child end:" + System.DateTime.UtcNow);    
            lock(ParentThread.obj){
                Monitor.Pulse(ParentThread.obj);
		    }
        }
    }
}
