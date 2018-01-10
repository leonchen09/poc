
namespace Pdw.Services.Template
{
    class QueryConstants
    {
        /// <summary>
        ///  Select 
        /// </summary>
        public const string SelectClause = " Select ";

        /// <summary>
        ///  {0} as {1}
        /// </summary>
        public const string SQLFieldFormat = " {0} as {1}";

        ///// <summary>
        /////  {0} as '@{1}'{2}
        ///// </summary>
        //public const string SQLFieldFormatAttribute = "{0} as '@{1}'{2}";

        /// <summary>
        ///  ?
        /// </summary>
        public const string JSQLSystemParam = " ?";

        /// <summary>
        ///  '+ ? +'
        /// </summary>
        public const string JSQLSystemParamWithIn = " '+ ? +'";

        /// <summary>
        /// (case when ({0}) then 1 else 0 end)
        /// </summary>
        public const string PathSQLConditionFormat = "(case when ({0}) then 1 else 0 end)";

        /// <summary>
        /// ,(
        /// </summary>
        public const string OpenSubSelect = ",(";

        /// <summary>
        /// )
        /// </summary>
        public const string CloseSubSelect = ")";

        /// <summary>
        ///  (case when ({0}) then 1 else 0 end) as {1}
        /// </summary>
        public const string SQLConditionFormat = " (case when ({0}) then 1 else 0 end) as {1}";

        /// <summary>
        ///  '+ ? +' as {0}
        /// </summary>
        public const string JSQLSystemInfoWithIn = " '+ ? +' as {0}";

        /// <summary>
        ///  ? as {0}
        /// </summary>
        public const string JSQLSystemInfo = " ? as {0}";        

        /// <summary>
        ///  ({0}) as {1}
        /// </summary>
        public const string SQLUDFFormat = " ({0}) as {1}";

        /// <summary>
        ///  From 
        /// </summary>
        public const string FromClause = " From ";

        /// <summary>
        /// From [{0}] {1} 
        /// </summary>
        public const string FromClauseAttribute = "From [{0}] {1} ";

        /// <summary>
        ///  Where 
        /// </summary>
        public const string WhereClause = " Where ";

        /// <summary>
        ///  Where {0} 
        /// </summary>
        public const string WhereClauseAttribute = " Where {0} ";

        /// <summary>
        /// ,
        /// </summary>
        public const string SQLComma = ",";

        /// <summary>
        /// pdx_imgChunking({0})
        /// </summary>
        public const string UdfTruncImage = " dbo.pdx_imgChunking({0}) ";
    }
}