using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestWinForm
{
    class ParentThread 
    {
        public static Object obj = new Object();
        public void run()
        {
            Console.WriteLine("parent start:" + System.DateTime.UtcNow);
            Thread th = new Thread(new ThreadStart(new ChildThread().run));
            th.Start();
            lock (obj)
            {
                Monitor.Wait(obj);
            }
            Console.WriteLine("parent end:" + System.DateTime.UtcNow);
        }
    }
}
