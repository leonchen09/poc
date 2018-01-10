
namespace Pdw.Core
{
    public class BaseProntoMarkup
    {
        /// <summary>
        /// Pronto_yyyyddMMhhmmssffff
        /// </summary>
        public const string BookmarkKeyFormat = "P_yyyyddMMhhmmssffff";

        #region define bookmark key and value format
        #region tag
        /// <summary>
        /// &lt;
        /// </summary>
        public const string TagOpen = "<";

        /// <summary>
        /// />
        /// </summary>
        public const string TagClose = "/>";

        /// <summary>
        /// &lt;/
        /// </summary>
        public const string TagOpenClose = "</";

        /// <summary>
        /// >
        /// </summary>
        public const string TagCloseClose = ">";

        /// <summary>
        /// &lt;
        /// </summary>
        public const string TagStartSelect = TagOpen;

        /// <summary>
        /// &lt;
        /// </summary>
        public const string TagStartSIf = TagStartSelect;

        /// <summary>
        /// &lt;
        /// </summary>
        public const string TagStartSForeach = TagStartSelect;

        /// <summary>
        /// &lt;/
        /// </summary>
        public const string TagStartEIf = TagOpenClose;

        /// <summary>
        /// &lt;/
        /// </summary>
        public const string TagStartEForeach = TagStartEIf;

        /// <summary>
        /// />
        /// </summary>
        public const string TagEndSelect = TagClose;

        /// <summary>
        /// >
        /// </summary>
        public const string TagEndSIf = TagCloseClose;

        /// <summary>
        /// >
        /// </summary>
        public const string TagEndSForeah = TagEndSIf;

        /// <summary>
        /// >
        /// </summary>
        public const string TagEndEIf = TagEndSIf;

        /// <summary>
        /// >
        /// </summary>
        public const string TagEndEForeach = TagEndSIf;

        /// <summary>
        /// ForEach
        /// </summary>
        public const string TagContentSForeach = "ForEach";

        /// <summary>
        /// ForEach
        /// </summary>
        public const string TagContentEForeach = TagContentSForeach;
        #endregion

        #region key
        /// <summary>
        /// _Comment
        /// </summary>
        public const string KeyComment = "_Comment";

        /// <summary>
        /// _Select
        /// </summary>
        public const string KeySelect = "_Select";

        /// <summary>
        /// _Image
        /// </summary>
        public const string KeyImage = "_Image";

        /// <summary>
        /// _S_If
        /// </summary>
        public const string KeyStartIf = "_S_If";

        /// <summary>
        /// _E_If
        /// </summary>
        public const string KeyEndIf = "_E_If";

        /// <summary>
        /// _Foreach
        /// </summary>
        public const string KeyForeach = "_Foreach";

        /// <summary>
        /// _S_Foreach
        /// </summary>
        public const string KeyStartForeach = "_S" + KeyForeach;

        /// <summary>
        /// _E_Foreach
        /// </summary>
        public const string KeyEndForeach = "_E" + KeyForeach;

        /// <summary>
        /// Sys
        /// </summary>
        public const string KeyMarkupDocumentSpecific = "Sys";

        public const string KeyTable = "_Table";
        #endregion

        #region value
        /// <summary>
        /// [{0}]
        /// </summary>
        public const string ValueSelect = TagStartSelect + "{0}" + TagEndSelect;

        /// <summary>
        /// [{0}]
        /// </summary>
        public const string ValueComment = "[!+][!+]";

        public const int ValueCommentTextPosition = 4;

        /// <summary>
        /// &lt;{0}>
        /// </summary>
        public const string ValueStartIf = TagStartSIf + "{0}" + TagEndSIf;

        /// <summary>
        /// &lt;/{0}>
        /// </summary>
        public const string ValueEndIf = TagStartEIf + "{0}" + TagEndEIf;

        /// <summary>
        /// &lt;{0}>
        /// </summary>
        public const string ValueStartForeach = TagStartSForeach + "{0}" + TagEndSForeah;

        /// <summary>
        /// &lt;/{0}>
        /// </summary>
        public const string ValueEndForeach = TagStartEForeach + "{0}" + TagEndEForeach;
        #endregion
        #endregion
    }
}