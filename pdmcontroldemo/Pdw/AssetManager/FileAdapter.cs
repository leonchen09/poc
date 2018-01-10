
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using ProntoDoc.Framework.Utils;
using ProntoDoc.Framework.CoreObject;

namespace Pdw.AssetManager
{
    public class FileAdapter
    {
        private static string UserDataPath
        {
            get
            {
                return TemporaryFolderPath + "\\UserData.usd";
            }
        }

        /// <summary>
        /// read user data
        /// </summary>
        /// <returns></returns>
        public static string ReadUserData()
        {
            if (File.Exists(UserDataPath))
                return FileHelper.ReadFile(UserDataPath);
            else
                return string.Empty;
        }

        /// <summary>
        /// save user data
        /// </summary>
        /// <param name="content"></param>
        public static void SaveUserData(string content)
        {
            FileHelper.CreateFile(UserDataPath, content, false);
        }

        /// <summary>
        /// ..AppData\\Omni Apps\\ProntoDoc for Word
        /// </summary>
        public static string TemporaryFolderPath
        {
            get
            {
                string appData = Environment.GetEnvironmentVariable(FrameworkConstants.EnvironmentKeyUerProfile);
                
                string omniApp = appData + "\\Omni Apps";
                if (!Directory.Exists(omniApp))
                    Directory.CreateDirectory(omniApp);

                string pronto = omniApp + "\\ProntoDoc for Word";
                if (!Directory.Exists(pronto))
                    Directory.CreateDirectory(pronto);

                return pronto;
            }
        }

        /// <summary>
        /// get data segment dll path
        /// </summary>
        public static string DataSegmentDllPath
        {
            get
            {
                string searchDataSegmentPattern = "????????????????????????????????DS.dll";//guidDS.dll
                string folder = AppDomain.CurrentDomain.BaseDirectory;// check everyone case

                if (Directory.Exists(folder))
                {
                    IEnumerable<string> files = Directory.EnumerateFiles(folder, searchDataSegmentPattern);
                    if (files.Count() > 0)
                        return files.ElementAt(0);
                }

                folder = TemporaryFolderPath;
                if (Directory.Exists(folder))
                {
                    IEnumerable<string> files = Directory.EnumerateFiles(folder, searchDataSegmentPattern);
                    if (files.Count() > 0)
                        return files.ElementAt(0);
                }

                throw new Exception(Properties.Resources.ipe_LoadDatasegmentError);
            }
        }

        public static string GetFilePath(string filePathWithoutExtension, string extension)
        {
            int fileIndex = 0;
            string filePath = string.Format("{0}{1}", filePathWithoutExtension, extension);
            while (System.IO.File.Exists(filePath))
            {
                fileIndex = fileIndex + 1;
                filePath = string.Format("{0}({1}){2}", filePathWithoutExtension, fileIndex, extension);
            }

            return filePath;
        }

        public static string GenRandomFilePath(string extension)
        {
            string ext = extension.StartsWith(".") ? extension : "." + extension;
            return string.Format("{0}\\{1}{2}", TemporaryFolderPath, Guid.NewGuid().ToString(), ext);
        }
    }
}
