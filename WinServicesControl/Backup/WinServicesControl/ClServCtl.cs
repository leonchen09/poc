using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using Microsoft.Win32;


namespace WinServicesControl
{
    static  class  ClServCtl
    {
        /// <summary>
        /// 设置服务的启动方式
        /// </summary>
        /// <param name="key">2自动,3手动,4禁用</param>
        /// <returns></returns>
        public static bool SetServRunType(string servname, int k)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + servname, true);
                key.SetValue("Start", k);
                key.Close();
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// 获得服务的启动方式
        /// </summary>
        /// <returns></returns>
        public static string GetServRunType(string servname)
        {
            string runtype="";
            try
            {                
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + servname, true);
                runtype = key.GetValue("Start").ToString();
                key.Close();
                switch (runtype)
                {
                    case"2":
                        runtype = "自动";
                        break;
                    case "3":
                        runtype = "手动";
                        break;
                    case "4":
                        runtype = "禁用";
                        break;
                }
            }
            catch { }
            return runtype;
        }


        /// <summary>
        /// 把本应用程序添加到系统启动项
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool SetAutoRun(string keyName, string filePath)
        {
            try
            {
                RegistryKey runKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
                runKey.SetValue(keyName, filePath);
                runKey.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 取消系统启动项
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool DeleteAutoRun(string keyName)
        {
            try
            {
                RegistryKey runKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                //runKey.DeleteSubKey(keyName,true);
                runKey.SetValue(keyName, "");
                runKey.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获取服务的可执行文件路径
        /// </summary>
        /// <param name="servername"></param>
        /// <returns></returns>
        public static string GetFilePath(string servername)
        {
            RegistryKey _Key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Services\" + servername);
            if (_Key != null)
            {
                object _ObjPath = _Key.GetValue("ImagePath");
                if (_ObjPath != null)
                    return _ObjPath.ToString();
            }
            return "";            
        }


        /// <summary>
        /// 获取一个服务的状态
        /// </summary>
        /// <param name="ServName"></param>
        /// <returns></returns>
        public static string GetServState(string servername)
        {
            try
            {
                ServiceController sc = new ServiceController(servername);
                return sc.Status.ToString();
            }
            catch
            { 
                
                return "";
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="servername"></param>
        /// <returns></returns>
        public static bool RunService(string servername)
        {
            try
            {
                ServiceController sc = new ServiceController(servername);
                ServiceControllerStatus st = sc.Status;
                switch (st)
                {
                    case ServiceControllerStatus.Running:
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.Paused:
                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.ContinuePending:
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        sc.Start();
                        break;
                    case ServiceControllerStatus.StopPending:
                    case ServiceControllerStatus.Stopped:
                        sc.Start();
                        break;
                    default: break;
                }
                sc.WaitForStatus(ServiceControllerStatus.Running);
                st = sc.Status;//再次获取服务状态
                if (st == ServiceControllerStatus.Running)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="servername"></param>
        /// <returns></returns>
        public static bool StopService(string servername)
        {
            try
            {
                ServiceController sc = new ServiceController(servername);
                ServiceControllerStatus st = sc.Status;
                switch (st)
                {
                    case ServiceControllerStatus.Running:
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.Paused:
                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.ContinuePending:
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        break;
                }
                st = sc.Status;//再次获取服务状态
                if (st == ServiceControllerStatus.Stopped)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }
        /// <summary>
        /// 暂停服务
        /// </summary>
        /// <param name="servername"></param>
        /// <returns></returns>
        public static bool PauseService(string servername)
        {
            try
            {
                ServiceController sc = new ServiceController(servername);
                ServiceControllerStatus st = sc.Status;
                switch (st)
                {
                    case ServiceControllerStatus.Running:
                    case ServiceControllerStatus.StartPending:
                        sc.Pause();
                        sc.WaitForStatus(ServiceControllerStatus.Paused);
                        break;
                }
                st = sc.Status;//再次获取服务状态
                if (st == ServiceControllerStatus.Paused)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }
        /// <summary>
        /// 恢复暂停的服务
        /// </summary>
        /// <param name="servername"></param>
        /// <returns></returns>
        public static bool ResumeService(string servername)
        {
            try
            {
                ServiceController sc = new ServiceController(servername);
                ServiceControllerStatus st = sc.Status;
                switch (st)
                {
                    case ServiceControllerStatus.Paused:
                    case ServiceControllerStatus.PausePending:
                        sc.Continue();
                        sc.WaitForStatus(ServiceControllerStatus.Running);
                        break;
                }
                st = sc.Status;//再次获取服务状态
                if (st == ServiceControllerStatus.Running)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }

    }
}
