using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Security.Policy;
using System.Security;
using System.IO;


namespace PdeInstall
{
    [RunInstaller(true)]
    public partial class PdeInstall : System.Configuration.Install.Installer
    {
     
        public PdeInstall()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            bool bAllUsers = Context.Parameters[Constants.Parameters.IsAllUsers] == Constants.IsAllUsers;
            string sTargetDir = Context.Parameters[Constants.Parameters.TargetDir].Trim();
            string sWinDir = Environment.GetEnvironmentVariable(Constants.Environment.WinDir).Trim() + Constants.Slash;
            sWinDir = sWinDir.Replace(Constants.Slash + Constants.Slash, Constants.Slash);

            //if (bAllUsers)
            //{
                string sScriptPath = sTargetDir + Constants.File.RegScriptFile;
                string sFileUrl = string.Format(Constants.Registry.VstoFilePath, sTargetDir.Replace(Constants.Slash, Constants.Slash + Constants.Slash));
                string sRegiterScript = string.Format(Script.InstallScript, sFileUrl);

                FileHelper.WriteFile(sScriptPath, sRegiterScript);
                FileHelper.ExecuteFile(sWinDir + Constants.File.RegeditApp, Constants.RegImportParams + Constants.Quote + sScriptPath + Constants.Quote);
            //}

            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            bool bAllUsers = Context.Parameters[Constants.Parameters.IsAllUsers] == Constants.IsAllUsers;
            string sTargetDir = Context.Parameters[Constants.Parameters.TargetDir].Trim();

            if (!bAllUsers)
            {
                string sAppData = Environment.GetEnvironmentVariable(Constants.Environment.AppData) + Constants.Slash;
                sAppData = sAppData.Replace(Constants.Slash + Constants.Slash, Constants.Slash);
                sAppData += Constants.ApplicationFolder + Constants.Slash;
                Directory.CreateDirectory(sAppData);

                //Copy datasegment file to application folder of the profile
                DirectoryInfo oInstallFolder = new DirectoryInfo(sTargetDir);
                if (oInstallFolder.Exists)
                {
                    //FileInfo[] arrFiles = oInstallFolder.GetFiles(Constants.File.DSPatternFile, SearchOption.TopDirectoryOnly);
                    //if (arrFiles.Length > 0)
                    //{
                    //    arrFiles[0].MoveTo(sAppData + arrFiles[0].Name);
                    //}
                }
            }

            base.Commit(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                bool bAllUsers = Context.Parameters[Constants.Parameters.IsAllUsers] == Constants.IsAllUsers;
                string sTargetDir = Context.Parameters[Constants.Parameters.TargetDir].Trim();
                string sWinDir = Environment.GetEnvironmentVariable(Constants.Environment.WinDir).Trim();
                sWinDir += Constants.Slash;
                sWinDir = sWinDir.Replace(Constants.Slash + Constants.Slash, Constants.Slash);

                if (bAllUsers)
                {
                    string sScriptPath = sTargetDir + Constants.File.RegScriptFile;

                    FileHelper.WriteFile(sScriptPath, Script.UninstallScript);
                    FileHelper.ExecuteFile(sWinDir + Constants.File.RegeditApp, Constants.RegImportParams + Constants.Quote + sScriptPath + Constants.Quote);
                }
                else
                {
                    string sAppData = Environment.GetEnvironmentVariable(Constants.Environment.AppData) + Constants.Slash;
                    sAppData = sAppData.Replace(Constants.Slash + Constants.Slash, Constants.Slash);
                    sAppData += Constants.ApplicationFolder + Constants.Slash;
                    DirectoryInfo dirAppData = new DirectoryInfo(sAppData);

                    if (dirAppData.Exists)
                    {
                        //FileInfo[] arrFiles = dirAppData.GetFiles(Constants.File.DSPatternFile, SearchOption.TopDirectoryOnly);
                        //if (arrFiles.Length > 0)
                        //{
                        //    for (int i = arrFiles.Length - 1; i >= 0; i--)
                        //    {
                        //        arrFiles[i].Delete();
                        //    }
                        //}
                    }
                }
            }
            finally
            {
                base.Uninstall(savedState);
            }
        }
    }
}
