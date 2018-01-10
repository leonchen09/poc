using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pdw.Core;
using Microsoft.Office.Interop.Word;

namespace Pdw.WKL.Profiler.Services
{
    public class ContentServiceProfile:BaseProfile
    {
        public bool         ValidateBookmark_IIsUpdate { get; set; }
        public string       ValidateBookmark_ITemplateName { get; set; }
        public List<string> ValidateBookmark_ORemovedList { get; set; }

        public Dictionary<string, string> GetBookmarks_OListBM { get; set; }

        public List<BookmarkItem> GetDistinctBM_OListBM { get; set; }

        public string HighlightBookmark_IBMName { get; set; }

        public string DeletedBookmarkName { get; set; }
        public bool DeleteWholeBookmark { get; set; }

        public string HighlightBookmarkName { get; set; }

        public bool MarkProntDoc { get; set; }

        public string AddToSelection_Text { get; set; }
        public bool AddToSelection_IsSelected { get; set; }

        public string AddBookmark_Name { get; set; }
        public Bookmark AddBookmark_BookmarkReturn { get; set; }

        public int MoveChars_Count { get; set; }
        public bool MoveChars_IsLeft { get; set; }
        public bool? MoveChars_IsExtend { get; set; }

        public Microsoft.Office.Interop.Word.Selection WSelection { get; set; }

        /// <summary>
        /// Use to add bookmark. Call from manager to service.
        /// </summary>
        public InternalBookmarkItem AddBookmark_IBookmark { get; set; }

        public List<string> UnMatchedFields { get; set; }

        // using for validate position of data tag with foreach tags
        public Dictionary<string, Bookmark> DicBookmarks { get; set; }
        public bool IsValid { get; set; }
        public string DomainName { get; set; }

        public WdColorIndex WdColorIndex { get; set; }
    }
}
