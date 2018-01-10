using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pdw.FormControls
{
    internal class SharedConstants
    {
        #region Format

        internal const string SimpleStringConcatFormat = "{0}{1}";

        /// <summary>
        /// The format of object's full type name.
        /// </summary>
        internal const string TypeFullNameFormat = "{0}.{1}";

        #endregion

        internal const string Choice_Yes = "yes";
        internal const string Choice_No = "no";

        internal const string XpathFuncFormat_SubstringAfter = "substring-after({0}, '{1}')";
        internal const string XpathFuncFormat_SubstringBefore = "substring-before({0}, '{1}')";

        internal const string DataBinding_DefaultKey = "value";
        internal const string DataBinding_SelectKey = "select";

        internal const string Format_EqualExpression = "{0} = {1}";

        /// <summary>
        /// The UTF-8 Encoding instance with encoder not to emit utf-8 identifier.
        /// </summary>
        internal static readonly Encoding UTF8Encoding = new UTF8Encoding(false);
    }
}
