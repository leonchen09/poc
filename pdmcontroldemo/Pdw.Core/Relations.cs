
using System.Collections.Generic;

using ProntoDoc.Framework.Utils;

namespace Pdw.Core
{
    public class Relations
    {
        /// <summary>
        /// list of table name (alias) is orderd asc by level
        /// </summary>
        public List<string> TableNames { get; set; }

        /// <summary>
        /// Matrix of relation (has penalty relation or itseft then mark 1 else mark 0)
        /// </summary>
        public Dictionary<string, List<long>> Matrix { get; set; }

        /// <summary>
        /// mapping beetween biz name and alias name
        /// key: biz name
        /// value: mapping object
        /// </summary>
        public Dictionary<string, Mapping> MapInfos { get; set; }

        public List<string> TableInSelectClause { get; set; }

        /// <summary>
        /// get level of table in matrix (number of cell that has value is 1)
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int GetLevel(string tableName)
        {
            tableName = string.Format(BaseConstants.ProntoDocMarkup.FieldFormat, tableName).Trim();
            if (Matrix.ContainsKey(tableName))
            {
                long level = 0;
                foreach (long mark in Matrix[tableName])
                {
                    long relation = mark;
                    while (relation != 0)
                    {
                        level += relation % 2;
                        relation = BitHelper.ShiftRight(relation, 1);
                    }

                }

                return (int)level;
            }

            return 0;
        }

        /// <summary>
        /// Get path (list of table in path) go to the table
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <returns>Ex: [Employee][History]</returns>
        public List<string> GetPath(string tableName)
        {
            List<string> path = new List<string>();
            tableName = string.Format(BaseConstants.ProntoDocMarkup.FieldFormat, tableName).Trim();
            if (Matrix.ContainsKey(tableName))
            {
                List<long> row = Matrix[tableName];
                int i = 0;
                for (int index = 0; index < row.Count; index++)
                {
                    long relation = row[index];
                    while (relation != 0)
                    {
                        i += 1;
                        if (relation % 2 == 1)
                            path.Add(BaseMarkupUtilities.XmlEncode(TableNames[i - 1]));

                        relation = BitHelper.ShiftRight(relation, 1);
                    }
                }
            }

            return path;
        }

        /// <summary>
        /// Get table of of bizName
        /// </summary>
        /// <param name="bizName"></param>
        /// <returns></returns>
        public string GetTableName(string bizName)
        {
            if (MapInfos.ContainsKey(bizName))
                return MapInfos[bizName].TableName;

            return bizName;
        }

        /// <summary>
        /// Get column name of biz name
        /// </summary>
        /// <param name="bizName"></param>
        /// <returns></returns>
        public string GetColumnName(string bizName)
        {
            if (MapInfos.ContainsKey(bizName))
                return MapInfos[bizName].ColumnName;

            return bizName;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public Relations()
        {
            TableNames = new List<string>();
            MapInfos = new Dictionary<string, Mapping>();
            Matrix = new Dictionary<string, List<long>>();
            TableInSelectClause = new List<string>();
        }
    }
}