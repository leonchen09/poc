using Pdw.WKL.Profiler.Services;

namespace Pdw.WKL.DataController
{
    public class ServicesController : BaseController
    {       
        #region TemplateService 

        public TemplateServiceProfile GetTemplateServiceProfile(string key)
        {
            return GetDataObject<TemplateServiceProfile>(key);
        }

        public TemplateServiceProfile CreateTemplateServiceProfile(string key)
        {
            TemplateServiceProfile profile = new TemplateServiceProfile();
            AddDataObject(key, profile);
            return profile;
        }

        #endregion

        public ServicesProfile CreateProfile(out string key)
        {
            ServicesProfile srvPro = new ServicesProfile();
            key = AddDataObject(srvPro);

            return srvPro;
        }

        public ServicesProfile GetProfile(string key)
        {
            return GetDataObject<ServicesProfile>(key);
        }
    }
}
