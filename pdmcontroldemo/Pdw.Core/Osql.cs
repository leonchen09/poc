
using System.Text;

namespace Pdw.Core
{
    public class OsqlStringBuilder
    {
        public StringBuilder Osql { get; set; }
        public StringBuilder JOsql { get; set; }
        public string SelectedColumns { get; set; }

        /// <summary>
        /// Append same command to both Osql and JOsql
        /// </summary>
        /// <param name="cmd"></param>
        public void Append(string cmd)
        {
            Osql.Append(cmd);
            JOsql.Append(cmd);
        }

        /// <summary>
        /// AppendLine same command to both Osql and JOsql
        /// </summary>
        /// <param name="cmd"></param>
        public void AppendLine(string cmd)
        {
            Osql.AppendLine(cmd);
            JOsql.AppendLine(cmd);
        }

        public OsqlStringBuilder()
        {
            Osql = new StringBuilder();
            JOsql = new StringBuilder();
        }
    }
}
