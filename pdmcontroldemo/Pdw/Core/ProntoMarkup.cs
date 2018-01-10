namespace Pdw.Core
{
    public class ProntoMarkup : BaseProntoMarkup
    {
        #region internal bookmark properties
        /// <summary>
        /// Pronto_InternalBMCustomXmlPartId
        /// </summary>
        public const string InternalBMCustomXmlPartId = "Pronto_InternalBMCustomXmlPartId";
        #endregion

        #region bookmark color
        /// <summary>
        /// WdColor.wdColorBlack
        /// </summary>
        // public const Microsoft.Office.Interop.Word.WdColor ForeachColor = Microsoft.Office.Interop.Word.WdColor.wdColorViolet;
        public const Microsoft.Office.Interop.Word.WdColor ForeachColor = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;

        /// <summary>
        /// WdColor.wdColorBlue
        /// </summary>
        public const Microsoft.Office.Interop.Word.WdColor ForeachConditionColor = Microsoft.Office.Interop.Word.WdColor.wdColorBlue;

        /// <summary>
        /// WdColor.wdColorDarkRed
        /// </summary>
        // public const Microsoft.Office.Interop.Word.WdColor IfColor = Microsoft.Office.Interop.Word.WdColor.wdColorDarkRed;
        public const Microsoft.Office.Interop.Word.WdColor IfColor = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;

        /// <summary>
        /// WdColor.wdColorBlack
        /// </summary>
        public const Microsoft.Office.Interop.Word.WdColor SelectColor = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;

        /// <summary>
        /// Microsoft.Office.Interop.Word.WdColorIndex.wdYellow
        /// </summary>
        public const Microsoft.Office.Interop.Word.WdColorIndex BackgroundHighLight = Microsoft.Office.Interop.Word.WdColorIndex.wdYellow;

        public const Microsoft.Office.Interop.Word.WdColorIndex BackgroundUnHighLight = Microsoft.Office.Interop.Word.WdColorIndex.wdWhite;

        /// <summary>
        /// Microsoft.Office.Interop.Word.WdColorIndex.wdBlack
        /// </summary>
        public const Microsoft.Office.Interop.Word.WdColorIndex ForeColorHighLight = Microsoft.Office.Interop.Word.WdColorIndex.wdBlack;

        public static readonly System.Collections.Generic.Dictionary<string, Microsoft.Office.Interop.Word.WdColorIndex> Colors =
                new System.Collections.Generic.Dictionary<string, Microsoft.Office.Interop.Word.WdColorIndex>(){
                    {"Yellow", Microsoft.Office.Interop.Word.WdColorIndex.wdYellow},
                    {"BrightGreen", Microsoft.Office.Interop.Word.WdColorIndex.wdBrightGreen},
                    {"Turquoise", Microsoft.Office.Interop.Word.WdColorIndex.wdTurquoise},
                    {"Pink", Microsoft.Office.Interop.Word.WdColorIndex.wdPink},
                    {"Blue", Microsoft.Office.Interop.Word.WdColorIndex.wdBlue},
                    {"Red", Microsoft.Office.Interop.Word.WdColorIndex.wdRed},
                    {"DarkBlue", Microsoft.Office.Interop.Word.WdColorIndex.wdDarkBlue},
                    {"Teal", Microsoft.Office.Interop.Word.WdColorIndex.wdTeal},
                    {"Green", Microsoft.Office.Interop.Word.WdColorIndex.wdGreen},
                    {"Violet", Microsoft.Office.Interop.Word.WdColorIndex.wdViolet},
                    {"DarkRed", Microsoft.Office.Interop.Word.WdColorIndex.wdDarkRed},
                    {"DarkYellow", Microsoft.Office.Interop.Word.WdColorIndex.wdDarkYellow},
                    {"Gray50", Microsoft.Office.Interop.Word.WdColorIndex.wdGray50},
                    {"Gray25", Microsoft.Office.Interop.Word.WdColorIndex.wdGray25},
                    {"Black", Microsoft.Office.Interop.Word.WdColorIndex.wdBlack}
            };
        #endregion

        /// <summary>
        /// 5
        /// </summary>
        public const int SaveInterval = 5;

        /// <summary>
        /// PdwImageHolder_
        /// </summary>
        public const string ImageHolderKey = "PdwImageHolder_";
    }
}