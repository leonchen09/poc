#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
#endregion

namespace PdeInstall
{
    public class FileHelper
    {
        public static void ExecuteFile(string filepath, string args)
        {
            //Check exist of the execute file
            if (File.Exists(filepath))
            {
                //Create a processInfo
                ProcessStartInfo process = new ProcessStartInfo(filepath, args);

                //Start process
                Process.Start(process).Start();
            }
        }

        public static string ReadFile(string filepath)
        {
            StreamReader sr = new StreamReader(filepath);
            string content = sr.ReadToEnd();
            sr.Close();

            return content;
        }

        public static void WriteFile(string filepath, string content)
        {
            using (StreamWriter sw = new StreamWriter(filepath, false))
            {
                sw.Write(content);
            }
        }
    }
}
