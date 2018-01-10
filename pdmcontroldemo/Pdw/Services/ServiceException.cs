
using Pdw.Core;

namespace Pdw.Services
{
    public class ServiceException : BaseException
    {
        public ServiceException(long errorCode)
            : base(errorCode)
        {
        }

        public ServiceException(long errorCode, string errorMessage, string errorDetail)
            : base(errorCode, errorMessage, errorDetail)
        {
        }
    }
}
