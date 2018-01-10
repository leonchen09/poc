
namespace Pdw.Core
{
    public class BaseConstants
    {
        #region constant for ProntoDocMarkup
        public class ProntoDocMarkup
        {
            public const string FieldFormat = "[{0}]";
            public const string DisplayInListBox = "BusinessName";
        }
        #endregion

        #region constant for OrderBy
        public class OrderBy
        {
            /// <summary>
            /// A
            /// </summary>
            public const string AscMark = "A";

            /// <summary>
            /// D
            /// </summary>
            public const string DescMark = "D";

            /// <summary>
            /// &lt;ForEach Sort: 
            /// </summary>
            public const string ForeachSortMark = BaseProntoMarkup.TagStartSForeach + BaseProntoMarkup.TagContentSForeach + " Sort: ";

            /// <summary>
            /// &gt;
            /// </summary>
            public const string CloseTag = BaseProntoMarkup.TagEndSForeah;

            /// <summary>
            /// "; "
            /// </summary>
            public const string Delimiter = "; ";

            /// <summary>
            /// -
            /// </summary>
            public const string Concat = "-";

            /// <summary>
            /// Text
            /// </summary>
            public const string ContextMenuName = "Text";

            /// <summary>
            /// Pronto_Define order by
            /// </summary>
            public const string OrderByMenuTag = "Pronto_Define order by";
        }
        #endregion
    }
}