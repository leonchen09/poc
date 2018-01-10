using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Test
{
    class ChildThread
    {
        protected EventWaitHandle _threadWait;
        protected Thread _thread { get; set; }

        public ChildThread(string threadName)
        {
            _thread = new Thread(new ThreadStart(MainProcess));
            _thread.Name = threadName;
            _threadWait = new AutoResetEvent(false);
        }

        public void Start()
        {
                _thread.Start();
        }


        public void MainProcess()
        {
            Console.WriteLine("child process begin, name:" + _thread.Name + ",time" + DateTime.Now);
            Thread.Sleep(1000);
            Console.WriteLine("child process end, name:" + _thread.Name + ",time" + DateTime.Now);
            Wait();
        }

        public void Wait()
        {
            Console.WriteLine("Thread wait, name:" + _thread.Name + ",time"+ DateTime.Now);
            _threadWait.WaitOne();
        }

        public void Resume()
        {
            Console.WriteLine("Thread set, name:" + _thread.Name + ",time" + DateTime.Now);
            _threadWait.Set();            
        }


    }
}
