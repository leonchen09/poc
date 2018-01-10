
namespace Pdwx.DataObjects
{
    public class PdwxConstants
    {
        public const string Watermark = "The copy of pronto document";
        public const string NoWatermark = "";

        public const string RenderError = "Has error when render: {0}";
        public const string InvalidInputExtension = "Only accept input file extension is pdw or pdwr";
        public const string InvalidOutputExtension = "Only accept output file extension is docx or pdf or pdwr depend on render mode";

        public const string InvalidRenderSetting = "RenderSetting is null or its value is incorrect.";

        /// <summary>
        /// docProps/custom.xml
        /// </summary>
        public const string CustomPropertyRelationTarget = "docProps/custom.xml";

        /// <summary>
        /// /docProps/custom.xml
        /// </summary>
        public const string CustomPropertyPartUri = "/docProps/custom.xml";

        /// <summary>
        /// {0}{1}{2}
        /// </summary>
        public const string FilePathFormat = "{0}{1}{2}";

        public class IbmXml
        {
            public const string NodeName = "InternalBookmarkItems";
            public const string AttKey = "Key";
            public const string AttBizName = "BusinessName";
            public const string AttTecName = "TechnicalName";
            public const char SplitChar = '.';
            public const string EndImage = "_Image";
        }
    }
}
