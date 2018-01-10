using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pdw.Core
{
    class Constants : BaseConstants
    {
        #region constant for BookMarkControl
        public class BookMarkControl
        {
            public const string DisplayField = "DisplayName";
            public const string EndIfTag = "</";
            public const string EndDataTag = "/>";

        }
        #endregion

        #region constant for PreviewControl
        public class PreviewControl
        {
            public const string ColumnName = "Name";
            public const string ColumnValue = "Value";
            public const string ColumnDataType = "Data Type";
        }
        #endregion

        #region constant for PreviewOsqlControl
        public class PreviewOsqlControl
        {
            public const string DllName = "Pdw.PreviewOsql.dll";
        }
        #endregion

        #region constant for IntegrationService
        public class IntegrationService
        {
            public const string UserProfile = "USERPROFILE";
            public const string UserDataFolder = "\\Omni Apps\\UserData.usd";
        }
        #endregion

        #region constant for ContextManager
        public class ContextManager
        {
            public const int DefaultFontSize = 8;
            public const int MaxFontSize = 16;
            public const int MinFontSize = 7;
            public const int ItemHeight = 20;

            public const string UsingStatus = "In use";
            public const string NotUsingStatus = "Not use";
        }
        #endregion

        #region constant for word application command
        /// <summary>
        /// FileEmailAsPdfEmailAttachment
        /// </summary>
        public const string SendEmailPdfAsAtt = "FileEmailAsPdfEmailAttachment";

        /// <summary>
        /// FileEmailAsXpsEmailAttachment
        /// </summary>
        public const string SendEmailXpsAsAtt = "FileEmailAsXpsEmailAttachment";

        /// <summary>
        /// FileInternetFax
        /// </summary>
        public const string SendInternetFax = "FileInternetFax";
        #endregion
    }
}
