
using System.Collections.Generic;

using Microsoft.Office.Interop.Word;

using Pdw.Core;
using Pdwx.DataObjects;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using Pdw.FormControls;

namespace Pdw.WKL.Profiler.Manager
{
    public class ManagerProfile : BaseProfile
    {
        public List<string> WbmKeys { get; set; }
        public bool IsCorrect { get; set; }
        public Document WDoc { get; set; }
        
        public PdwInfo PdwInfo { get; set; }
        
        public int CharCounts { get; set; }
        public bool IsMovetoLeft { get; set; }        

        public InternalBookmark Ibm { get; set; }

        public string WbmKey { get; set; }
        public string WbmValue { get; set; }
        public string AlternativeText { get; set; }

        public InternalBookmarkItem IbmItem { get; set; }

        public List<USCItem> UscItems { get; set; }

        public List<XmlObject> XmlObjects { get; set; }

        public PdwrInfo PdwrInfo { get; set; }
        public PdeContent PdeContent { get; set; }

        #region Content Service Object Transport.

        public string DeletedBookmarkName { get; set; }
        public bool DeleteWholeBookmark { get; set; }

        public string HighlightBookmarkName { get; set; }

        public string UnHighlightBookmarkName { get; set; }

        public bool MarkProntDoc { get; set; }

        public string AddToSelection_Text { get; set; }

        public bool AddToSelection_IsSelected { get; set; }

        public string AddBookmark_Name { get; set; }
        public Bookmark AddBookmark_BookmarkReturn { get; set; }

        public int MoveChars_Count { get; set; }
        public bool MoveChars_IsLeft { get; set; }
        public bool? MoveChars_IsExtend { get; set; }

        public Microsoft.Office.Interop.Word.Selection WSelection { get; set; }
        #endregion

        public string OldKey { get; set; }
        public string NewKey { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }

        public string HighlightBM_IName { get; set; }
        public WdColorIndex WdColorIndex { get; set; }
        public bool IsSaveDocument { get; set; }

        public TemplateType TemplateType { get; set; }

        public Dictionary<string, Bookmark> DicBookmarks { get; set; }

        //HACK:FORM CONTROLS - ManagerProfile
        public ControlBase PdmControl { get; set; }
        public bool IsAddingPdmControl { get; set; }

        public ManagerProfile()
        {
            this.WdColorIndex = ProntoMarkup.BackgroundHighLight;
            this.IsSaveDocument = true;
        }
    }
}
