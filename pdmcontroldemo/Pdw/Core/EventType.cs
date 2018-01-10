
namespace Pdw.Core
{
    public enum EventType
    {
        #region task panel
        ShowPanel,
        HidePanel,
        DisableRibbon,
        VisiableReconstruct,
        VisiablePreviewOsql,
        ForceDisablePreviewOsql,
        InvisiableReconstruct,

        //HACK:FORM CONTROLS - EventType.ShowControlPropertyPanel, RefreshControlPropertyPanel
        ShowControlPropertyPanel,
        RefreshControlPropertyPanel,
        #endregion

        #region bookmark (ContentService)
        /// <summary>
        /// (string)
        /// </summary>
        AddBookmarkInCurrentSelection,

        /// <summary>
        /// (string, bool)
        /// </summary>
        AddTextToCurrentSelection,

        AddBookmarkImage,

        GetForeachTagsBoundCurrentPos,
        
        /// <summary>
        /// (string)
        /// </summary>
        DeleteBookmark,
        
        /// <summary>
        /// ()
        /// </summary>
        GetBookmarks,

        /// <summary>
        /// ()
        /// </summary>
        GetCurrentSelection,

        SetFontForCurrentSelection,
        
        /// <summary>
        /// (string)
        /// </summary>
        HighlightBookmark,
        
        /// <summary>
        /// (bool)
        /// </summary>
        MarkBookmarkIsAdding,
        
        /// <summary>
        /// (bool)
        /// </summary>
        MarkProntoDoc,
        
        /// <summary>
        /// (int, bool)
        /// </summary>
        MoveChars,

        /// <summary>
        /// ()
        /// </summary>
        ProtectBookmark,

        /// <summary>
        /// ()
        /// </summary>
        UnProtectBookmark,

        ProtectWord,
        UnProtectWord,

        EnableComboboxDomain,

        //HACK:FORM CONTROLS - EventType.ControlEventBinding
        ControlEventBinding,
        #endregion

        #region property (IntegrationService)
        /// <summary>
        /// (string, string)
        /// </summary>
        AddProperty,

        /// <summary>
        /// (string)
        /// </summary>
        GetProperty,

        /// <summary>
        /// (void)
        /// </summary>
        GetInternalBookmark,

        /// <summary>
        /// (void)
        /// </summary>
        SaveInternalBookmark,

        /// <summary>
        /// (string)
        /// </summary>
        RemoveInternalBookmarkItem,

        /// <summary>
        /// (InternalBookmarkItem)
        /// </summary>
        AddInternalBookmarkItem,

        /// <summary>
        /// (List of UscItem)
        /// </summary>
        UpdateUscItems,

        /// <summary>
        /// (Pdwx.PdwInfo)
        /// </summary>
        SavePdwInfo,

        /// <summary>
        /// (List of XmlObject)
        /// </summary>
        GetPdwrInformation,

        /// <summary>
        /// (void)
        /// </summary>
        GetPdwInformation,

        /// <summary>
        /// (string, string)
        /// </summary>
        UpdateInternalBookmark
        #endregion
    }

    public enum ServiceType
    {
        /// <summary>
        /// Bookmark service
        /// </summary>
        ContentService,

        /// <summary>
        /// properties service
        /// </summary>
        IntegrationService,
    }
}
