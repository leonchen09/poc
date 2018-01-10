
namespace Pdw.Core
{
    public class MarkupConstant
    {
        /// <summary>
        /// S_Foreach
        /// </summary>
        public const string MarkupStartForeach = "S_Foreach";

        /// <summary>
        /// E_Foreach
        /// </summary>
        public const string MarkupEndForeach = "E_Foreach";

        /// <summary>
        /// S_If
        /// </summary>
        public const string MarkupStartIf = "S_If";

        /// <summary>
        /// E_If
        /// </summary>
        public const string MarkupEndIf = "E_If";

        /// <summary>
        /// Select
        /// </summary>
        public const string MarkupSelect = "Select";

        /// <summary>
        /// Comment
        /// </summary>
        public const string MarkupComment = "Comment";

        /// <summary>
        /// pde_
        /// </summary>
        public const string MarkupPdeTag = "pde_";

        /// <summary>
        /// Chart
        /// </summary>
        public const string MarkupPdeChart = "_Chart";

        /// <summary>
        /// Image
        /// </summary>
        public const string MarkupImage = "Image";

        /// <summary>
        /// pdwImg
        /// </summary>
        public const string XslVariableImage = "pdwImg";

        /// <summary>
        /// pdwSection
        /// </summary>
        public const string XslVariableSection = "pdwSection";

        /// <summary>
        /// position()
        /// </summary>
        public const string XslPosition = "position()";

        /// <summary>
        /// 10*position()
        /// </summary>
        public const string XslMultiPosition = "100*position()";

        /// <summary>
        ///  + position()
        /// </summary>
        public const string XslPlusPosition = " + position()";

        /// <summary>
        /// $
        /// </summary>
        public const string XslPrefixVariant = "$";

        /// <summary>
        /// $pdwImg{0} + position()
        /// </summary>
        public static string XslVariantImageFormat = XslPrefixVariant + XslVariableImage + "{0}" + XslPlusPosition;

        private static string RandomId = System.DateTime.Now.ToString("yyyyMMddhhmmssff");

        public static string PrefixWithRandomId(bool hasSection)
        {
            return (hasSection ? "{$" + Core.MarkupConstant.XslVariableSection + "}" : "") + RandomId;
        }

        public static string SpaceInBookmark = System.Xml.XmlConvert.EncodeLocalName(((char)14).ToString());
        public const string Space = " ";

        public static string PrefixBookmark = "pdw_" + System.Xml.XmlConvert.EncodeLocalName(((char)13).ToString()) + "_";
        public const string SuffixEndBookmark = "_pdw";
        public const string SplitCharBookmark = "#";

        public const string PdeExportField = "Fields";
        public const string PdeExportTable = "Tables";
        public const string PdeExportChart = "Charts";
    }
}
