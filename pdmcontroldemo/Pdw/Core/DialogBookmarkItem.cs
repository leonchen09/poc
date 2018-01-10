using System.Collections.Generic;

using Microsoft.Office.Interop.Word;

namespace Pdw.Core
{ 

    /// <summary>
    /// This class is used for BookmarkControl. To binding to listbox.
    /// </summary>
    public class BookmarkItem
    {
        /// <summary>
        /// Name of bookmark
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Expression or TechName
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Display text. Remove <>, </>
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Type of bookmark: Select, Foreach, If.
        /// </summary>
        public XsltType Type { get { return GetXslType();} }

        /// <summary>
        /// Count the number of bookmark with the same name.
        /// </summary>
        public int NumberEntry { get; set; }   
    
        /// <summary>
        /// Keep bookmarks at the same name
        /// </summary>
        public List<Bookmark> Items { get; set; }

        public BookmarkItem(string name, string value, string displayName, Bookmark item)
        {
            Name = name;
            Value = value;
            DisplayName = displayName;
            
            NumberEntry = 1;
            Items = new List<Bookmark>();
            Items.Add(item);
        }

        private XsltType GetXslType()
        {
            if (Name.Contains(ProntoMarkup.KeyStartIf) || Name.Contains(ProntoMarkup.KeyStartForeach))
                return XsltType.If;
            else
                return XsltType.Select;
        }

    }
}
