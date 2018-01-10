
using System.Text;
namespace Pdw.Core
{
    class LogUtils
    {
        private static volatile object _locker = new object();
        public static void LogManagerError(Managers.ManagerException exception)
        {
            System.Windows.Forms.MessageBox.Show(exception.Message, "Error",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

            // log to file
            StringBuilder message = new StringBuilder();
            StringBuilder stackTrace = new StringBuilder();
            GetExceptionInfo(exception, ref message, ref stackTrace);
            Log("LogManagerError_Message", message.ToString());
            Log("LogManagerError_Stack", stackTrace.ToString());
        }

        private static void GetExceptionInfo(System.Exception ex, ref StringBuilder message, ref StringBuilder stackTrace)
        {
            if (ex == null)
                return;
            if (message == null)
                message = new StringBuilder();
            if (stackTrace == null)
                stackTrace = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(ex.Message))
                message.AppendLine(ex.Message);
            if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                stackTrace.AppendLine(ex.StackTrace);
            if (ex is Managers.ManagerException)
            {
                Managers.ManagerException mgrEx = ex as Managers.ManagerException;
                if (mgrEx != null && mgrEx.Errors != null)
                {
                    foreach (System.Exception errEx in mgrEx.Errors)
                        GetExceptionInfo(errEx, ref message, ref stackTrace);
                }
            }
        }

        private static void Log(string function, string message)
        {
            try
            {
                string fileName = System.IO.Path.Combine(AssetManager.FileAdapter.TemporaryFolderPath,
                    System.DateTime.Now.ToString("yyyyMMddhh") + "_pdw.log");

                lock (_locker)
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, true))
                    {
                        string tempMsg = string.IsNullOrEmpty(message) ? "" : message;
                        string tempFuc = string.IsNullOrEmpty(function) ? "" : function;
                        writer.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " " + tempFuc + ": " + tempMsg);
                    }
                }
            }
            catch { }
        }

        public static void Log(string function, System.Exception exception)
        {
            Log(function, exception.Message);
            Log(function, exception.StackTrace);
        }

        /// <summary>
        /// using for debug
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public static void CreateFile(string fileName, string content)
        {
            string filePath = System.IO.Path.Combine(AssetManager.FileAdapter.TemporaryFolderPath, fileName);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
            {
                writer.Write(content);
            }
        }
    }
}
