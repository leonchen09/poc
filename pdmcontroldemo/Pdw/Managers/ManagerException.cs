
using Pdw.Core;

namespace Pdw.Managers
{
    public class ManagerException : BaseException
    {
        public ManagerException(long errorCode)
            : base(errorCode)
        {
        }

        public ManagerException(long errorCode, string errorMessage, string errorDetail)
            : base(errorCode, errorMessage, errorDetail)
        {
        }
    }
}
