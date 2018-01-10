using System.Collections.Generic;

using Microsoft.Office.Core;

namespace Pdw.Core
{
    #region task panel
    public delegate void ShowPanelEnventHandler();

    public delegate void HidePanelEventHandler();

    public delegate void DisableRibbonEventHandler();

    public delegate void VisiableReconstructEventHandler();

    public delegate void VisiablePreviewOsqlEventHandler();

    public delegate void ForceDisablePreviewOsqlEventHandler();

    public delegate void InvisiableReconstructEventHandler();

    public delegate void EnableComboboxDomainEventHandler();

    #endregion

    #region properties   

    #region internal bookmark
    
    public delegate void GetInternalBookmarkEventHandler(string key);
    public delegate void SaveInternalBookmarkEventHandler();
    
    public delegate void AddInternalBookmarkItemEventHandler(string key);
    public delegate void RemoveInternalBookmarkItemEventHandler(string key);

    public delegate void UpdateUscItemsEventHandler(string key);
    #endregion

    #region pdw, pdwr
    public delegate void SavePdwInfoEventHandler(string key);
    public delegate void GetPdwInformationEventHandler(string key);
    public delegate void GetPdwrInformationEventHandler(string key);
    #endregion
    
    #endregion

    #region bookmarks
   
    public delegate void DeleteBookmarkEventHandler(string key);   
    
    public delegate void AddBookmarkEventHandler(string key);

    public delegate void AddBookmarkImageEventHandler(string key);

    public delegate void HighlightBookmarkEventHandler(string key);

    public delegate void GetForeachTagsBoundCurrentPosEventHandler(string key);
    #endregion

    #region helper events for word
    
    public delegate void MarkProntoDocEventHandler(string key);

    public delegate void AddTextToCurrentSelectionEventHandler(string key);

    public delegate void MoveLinesEventHandler(string key);

    public delegate void MoveCharactersEventHandler(string key);

    public delegate void DIGetCurrentSelectionEventHandler(string key);

    public delegate void SetFontForCurrentSelectionEventHandler(string key);

    public delegate void DIProtectBookmarkEventHandler();
    public delegate void DIUnProtectBookmarkEventHandler();

    public delegate void DIUnProtectWordEventHandler();
    public delegate void DIProtectWordEventHandler();
    
    #endregion

    public delegate void MainEventHandler(string key);

    public delegate void ControlEventBindingEventHandler(string key);
}
