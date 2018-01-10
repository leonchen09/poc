
namespace Pdw.Core
{
    public class Mapping
    {
        public string BizName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }

        public Mapping() { }
        public Mapping(string bizName, string tableName, string colName)
        {
            BizName = bizName;
            TableName = tableName;
            ColumnName = colName;
        }
    }
}