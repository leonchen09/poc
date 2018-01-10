using System;
using System.IO;
using System.Xml.Xsl;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.Collections.Generic;

namespace TestWinForm
{
    public partial class TransformForm : Form
    {
        private Stopwatch timer;

        public TransformForm()
        {
            InitializeComponent();
        }

        private void btnTransform_Click(object sender, EventArgs e)
        {
            File.Delete(txtOutputXml20000.Text);
            File.Delete(txtOutputXml30000.Text);

            timer = new Stopwatch();
            timer.Start();
            Transform(txtInputXml20000.Text, txtInputXsl.Text, txtOutputXml20000.Text);
            timer.Stop();

            txtMessage.Text = string.Format("{0}: {1} ms", txtInputXml20000.Text, 
                timer.Elapsed.TotalMilliseconds.ToString("n"));

            timer = new Stopwatch();
            timer.Start();
            Transform(txtInputXml30000.Text, txtInputXsl.Text, txtOutputXml30000.Text);
            timer.Stop();

            txtMessage.Text += string.Format("\r\n{0}: {1} ms", txtInputXml30000.Text,
                timer.Elapsed.TotalMilliseconds.ToString("n"));
        }

        private void Transform(string xmlFile, string xslFile, string outFile)
        {
            try
            {
                XslCompiledTransform transformer = new XslCompiledTransform();
                transformer.Load(xslFile);
                transformer.Transform(xmlFile, outFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(@"E:\ProntoDir\pocdata.xml");
            XmlNode mainnode = xmldoc.SelectSingleNode("pdepoc");
            //string s = mainnode.InnerText;
            string s = "";
            s += mainnode.ChildNodes.Count;
            label5.Text = s;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(new ThreadStart(new ParentThread().run));
            th.Start();
            Console.ReadLine();
        }

        Dictionary<int, int> dic = new Dictionary<int, int>();

        private void button3_Click(object sender, EventArgs e)
        {
            dic.Clear();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Method1), new object());
            ThreadPool.QueueUserWorkItem(new WaitCallback(Method2), new object());
  
        }

        void Method1(object o)
        {

            for (int i = 0; i < 5000; i++)
            {
                dic.Add(i, i);
            }
            Console.WriteLine("Method1 Count:" + dic.Count);


        }

        void Method2(object o)
        {

            for (int i = 5000; i < 10000; i++)
            {
                dic.Add(i, i);
            }
            Console.WriteLine("Method2 Count:" + dic.Count);


        }
        int theResource = 0;
        ReaderWriterLock rwl = new ReaderWriterLock();
        
        private void button4_Click(object sender, EventArgs e)
        {
            Thread tr0 = new Thread(new ThreadStart(Read)); 
            Thread tr1 = new Thread(new ThreadStart(Read)); 
            Thread tr2 = new Thread(new ThreadStart(Write)); 
            Thread tr3 = new Thread(new ThreadStart(Write)); 
            tr0.Start();
            tr1.Start();
            tr2.Start();
            tr3.Start();            
            //等待线程执行完毕            
            tr0.Join();
            tr1.Join();
            tr2.Join();
            tr3.Join();
        }
         void Read()        {            
             for (int i = 0; i < 10; i++)            {               
                 try                {                    
                     //申请读操作锁，如果在1000ms内未获取读操作锁，则放弃      
                     rwl.AcquireReaderLock(3000);                    
                     Console.WriteLine("开始读取数据，theResource = {0}", theResource);
                     rwl.AcquireReaderLock(2000);
                     Console.WriteLine("获得写锁，theResource = {0}", theResource);
                     Thread.Sleep(10);                    
                     Console.WriteLine("读取数据结束，theResource = {0}", theResource);                   
                     //释放读操作锁   
                     rwl.ReleaseReaderLock();
                     rwl.ReleaseReaderLock();                }                
                 catch (ApplicationException e)                {
                     Console.Write(e.Message);          
                 }            
             }       
         }       
        //写数据        
        void Write()       
        {            
            for (int i = 0; i < 10; i++)            
            {                
                try                
                {                    
                    //申请写操作锁，如果在1000ms内未获取写操作锁，则放弃       
                    rwl.AcquireWriterLock(3000);                   
                    Console.WriteLine("开始写数据，theResource = {0}", theResource);                   
                    //将theResource加1                    
                    theResource++;                   
                    Thread.Sleep(1000);                    
                    Console.WriteLine("写数据结束，theResource = {0}", theResource);                    
                    //释放写操作锁                    
                    rwl.ReleaseWriterLock();               
                }                
                catch (ApplicationException)               
                {                    
                    //获取写操作锁失败               
                }            
            }       
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int command = int.Parse(textBox1.Text);
            switch (command)
            {
                case 1:
                    {
                        Console.WriteLine("case 1");
                    }
                        break;
                    
                case 2:
                    {
                        Console.WriteLine("case 2");
                    }
                        break;
                    
                case 3:
                    {
                        Console.WriteLine("case 3");
                    }
                        break;
                    
                default:
                    {
                        Console.WriteLine("default");
                    }
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<string> a = null;
            foreach (string s in a)
            {
                Console.WriteLine(s);
            }
        }   
    }
}
