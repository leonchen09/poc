
namespace Pdwx.DataObjects
{
    public class PdwInfo
    {
        /// <summary>
        /// Pronto_OsqlCustomXmlPartId
        /// </summary>
        public const string OsqlCustomXmlPartId = "Pronto_OsqlCustomXmlPartId";

        /// <summary>
        /// Pronto_XsltCustomXmlPartId
        /// </summary>
        public const string XsltCustomXmlPartId = "Pronto_XsltCustomXmlPartId";

        /// <summary>
        /// Pronto_ChksCustomXmlPartId
        /// </summary>
        public const string ChksCustomXmlPartId = "Pronto_ChksCustomXmlPartId";

        /// <summary>
        /// Pronto_PdeContentXmlPartId
        /// </summary>
        public const string PdeContentXmlPartId = "Pronto_PdeContentXmlPartId";

        /// <summary>
        /// Pronto_PdeGoalSeekXmlPartId
        /// </summary>
        public const string PdeGoalSeekXmlPartId = "Pronto_PdeGoalSeekXmlPartId";

        //HACK:FORM CONTROLS - PdmFormControlXmlPartId
        public const string PdmFormControlXmlPartId = "Pronto_FormControlXmlPartId";

        public string ChecksumString { get; set; }
        public string OsqlString { get; set; }
        public string XsltString { get; set; }
        public string PdeContent { get; set; }
    }
}
