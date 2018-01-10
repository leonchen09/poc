using Pdw.WKL.Profiler.Manager;

namespace Pdw.WKL.DataController
{
    public class ManagerController:BaseController
    {
        public ManagerProfile CreateProfile(out string key)
        {
            ManagerProfile mgrPro = new ManagerProfile();
            key = AddDataObject(mgrPro);

            return mgrPro;
        }

        public ManagerProfile CreateProfile(string key)
        {
            ManagerProfile mgrPro = new ManagerProfile();
            AddDataObject(key, mgrPro);

            return mgrPro;
        }

        public ManagerProfile GetProfile(string key)
        {
            return GetDataObject<ManagerProfile>(key);
        }
    }
}
