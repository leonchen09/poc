
using Pdw.Core;

namespace Pdw.Services.Integrity
{
    public class IntegrityService
    {
        private MainService mainService = new MainService();

        public bool IsProntoDoc()
        {
            try
            {
                ProntoDoc.Framework.CoreObject.PdwxObjects.ChecksumInfo checksum = mainService.PropertyService.GetChecksum();

                return IsProntoDoc(checksum);
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_ValidateChecksumError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (System.Exception ex)
            {
                ServiceException srvExp = new ServiceException(Core.ErrorCode.ipe_ValidateChecksumError,
                    Core.MessageUtils.Expand(Properties.Resources.ipe_ValidateChecksumError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }

        private bool IsProntoDoc(ProntoDoc.Framework.CoreObject.PdwxObjects.ChecksumInfo checksum)
        {
            Validator validatorService = new Validator();

            return validatorService.IsValid(checksum.PluginId, checksum.PluginName, checksum.UserName, 
                checksum.InternalBookmark, checksum.Osql, checksum.ChecksumString);
        }
    }
}
