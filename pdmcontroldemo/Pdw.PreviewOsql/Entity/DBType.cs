using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pdw.PreviewOsql.Entity
{
    /// <summary>
    /// Type of Database Sql, Oracle,...
    /// </summary>
    public enum DBType : int
    {
        Sql2008 = (int)DBTypeValue.ipx_sqlserver,
        Orc10g = (int)DBTypeValue.ipx_oracle,
        Sybase = (int)DBTypeValue.ipx_sybase,
        Informix = (int)DBTypeValue.ipx_informix,
        DB2 = (int)DBTypeValue.ipx_db2,
        MySQL = (int)DBTypeValue.ipx_mysql
    }

    public class DBTypeValue
    {
        public const Int64 ipx_sqlserver = 1;
        public const Int64 ipx_oracle = 2;
        public const Int64 ipx_sybase = 3;
        public const Int64 ipx_informix = 4;
        public const Int64 ipx_db2 = 5;
        public const Int64 ipx_mysql = 6;
    }
}
