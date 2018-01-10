
namespace Pdw.Core
{
    public class ErrorCode
    {
        private ErrorCode() { }

        #region managers
        public const long ipe_SaveFileError = 0x0101000100000001;
        public const long ipe_SavePdwError = 0x0101000100000002;
        public const long ipe_ReconstructError = 0x0101000100000003;
        public const long ipe_ChangeFontError = 0x0101000100000004;
        public const long ipe_OpenPdwrError = 0x0101000100000005;
        public const long ipe_OpenDocumentError = 0x0101000100000006;
        public const long ipe_SaveDocumentError = 0x0101000100000007;
        public const long ipe_LoadResourceError = 0x0101000100000008;
        public const long ipe_AddForechTagError = 0x0101000100000009;

        #region pronto doc markup
        public const long ipe_ChangeDomainError = 0x0101000100000010;
        public const long ipe_AddDataTagError = 0x0101000100000011;
        public const long ipe_RemoveUSCItemError = 0x0101000100000012;
        public const long ipe_AddUSCItemError = 0x0101000100000013;
        public const long ipe_TagUSCItemError = 0x0101000100000014;
        public const long ipe_SelectUSCItemError = 0x0101000100000015;
        #endregion

        #region base
        public const long ipe_ProtectBookmarkError = 1;
        public const long ipe_UnProtectBookmarkError = 1;
        public const long ipe_ProtectDocumentError = 1;
        public const long ipe_UnProtectDocumentError = 1;
        #endregion

        #region context
        public const long ipe_CheckIbmAndWbmError = 0x0102000100000001;
        public const long ipe_CheckWbmWithDatasegment = 0x0102000100000002;
        public const long ipe_UpdateStatusBarError = 0x0102000100000003;
        public const long ipe_SavePdwInfoError = 0x0102000100000004;
        public const long ipe_SendMailError = 0x0102000100000005;
        public const long ipe_SendOutlookMailError = 0x0102000100000006;
        #endregion

        #region validator
        public const long ipe_ValidateDataError = 0x0102000200000001;
        public const long ipe_ValidateBookmarkError = 0x0102000200000002;
        public const long ipe_RemoveBookmarkError = 0x0102000200000003;
        #endregion

        #region dataintegration
        public const long ipe_AddWordBookmarkError = 0x0102000300000001;
        public const long ipe_HighlightBookmarkError = 0x0102000300000002;
        public const long ipe_GenerateImageError = 0x0102000300000003;
        public const long ipe_DeleteBookmarkError = 0x0102000300000004;
        public const long ipe_UnHighlightWordBookmarkError = 0x0102000300000005;
        public const long ipe_UpdateWordBookmarkError = 0x0102000300000006;
        public const long ipe_GetWordBookmarkByPositionError = 0x0102000300000007;
        #endregion

        #region integration
        public const long ipe_GetInternalBookmarkError = 0x0102000400000001;
        public const long ipe_SaveInternalBookmarkError = 0x0102000400000002;
        public const long ipe_GetPdwInfoError = 0x0102000400000003;
        #endregion

        #endregion

        #region services
        #region service (content)
        public const long ipe_GenOsqlError = 0x0101000200000001;
        public const long ipe_GenXsltError = 0x0101000200000002;
        public const long ipe_GenChecksumError = 0x0101000200000003;
        public const long ipe_AddBookmarkError = 0x0101000200000004;
        public const long ipe_ValidateWordBookmarkError = 0x0101000200000005;
        public const long ipe_ValidateWordBookmarkWithDomainError = 0x0101000200000006;
        public const long ipe_GetWordBookmarksError = 0x0101000200000007;
        public const long ipe_GetDistinctBookmarkError = 0x0101000200000008;
        #endregion

        #region service (integration)
        public const long ipe_LoadInternalBookmarkError = 0x0101000300000001;
        public const long ipe_ValidateStructError = 0x0101000300000002;
        public const long ipe_ValidateIbmWithDomainError = 0x0101000300000003;
        public const long ipe_ValidateIbmsError = 0x0101000300000004;
        public const long ipe_AddIbmItemError = 0x0101000300000005;
        public const long ipe_UpdateIbmItemError = 0x0101000300000006;
        public const long ipe_GetChecksumError = 0x0101000300000007;
        public const long ipe_UpdateUSCItemError = 0x0101000300000008;
        public const long ipe_GetListDomainError = 0x0101000300000009;
        public const long ipe_LoadDomainDataError = 0x0101000300000010;
        public const long ipe_GetPluginInfoError = 0x0101000300000011;
        #endregion

        #region service (IntegrityService)
        public const long ipe_ValidateChecksumError = 0x0101000400000001;
        #endregion

        #region service (template)
        public const long ipe_CheckWordStructError = 0x0101000500000001;
        public const long ipe_RepairWordStructError = 0x0101000500000002;
        public const long ipe_GetXslContentError = 0x0101000500000003;
        public const long ipe_BindTreeError = 0x0101000500000004;
        #endregion
        #endregion
    }
}
