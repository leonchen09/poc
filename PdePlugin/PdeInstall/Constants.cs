namespace PdeInstall
{
    public class Constants
    {
        public class Parameters
        {
            public const string IsAllUsers = "ALLUSERS";

            public const string TargetDir = "TargetDir";
        }

        public class Registry
        {
            public const string VstoSetupKey = "SOFTWARE\\Microsoft\\VSTO Runtime Setup\\v4";

            public const string VstoFilePath = "file:///{0}PdePlugin.vsto|vstolocal";

            public const string Manifest = "\"Manifest\"=\"{0}\"";

            public const string Url = "\"Url\"=\"{0}\"";
        }

        public class Environment
        {
            public const string WinDir = "WinDir";

            public const string AppData = "APPDATA";
        }

        public class File
        {
            public const string RegeditApp = "regedit.exe";

            public const string RegScriptFile = "Install.reg";

            //public const string DSPatternFile = "????????-????-????-????-????????????DS.dll";
        }

        public const string RegImportParams = " /c /s ";

        public const string Slash = "\\";

        public const string Quote = "\"";

        public const string IsAllUsers = "1";

        public const string ApplicationFolder = "ProntoDocConfigurer";
    }
}
