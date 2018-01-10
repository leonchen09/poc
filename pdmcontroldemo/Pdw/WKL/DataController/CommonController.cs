
using Pdw.Core;
using Pdw.WKL.Profiler;

namespace Pdw.WKL.DataController
{
    public class CommonController
    {
        private static CommonProfile _commonProfile = null;

        public CommonController()
        {
            if (_commonProfile == null)
                _commonProfile = new CommonProfile();
        }

        public CommonProfile CommonProfile
        {
            get { return _commonProfile; }
        }

        #region 1. Template information
        public TemplateInfo CreateTemplateInfo(string fullDocName)
        {
            if (_commonProfile.TemplateInfos.ContainsKey(fullDocName))
                return _commonProfile.TemplateInfos[fullDocName];
            else
            {
                TemplateInfo templateInfo = new TemplateInfo(fullDocName);
                _commonProfile.TemplateInfos.Add(fullDocName, templateInfo);

                return templateInfo;
            }
        }

        /// <summary>
        /// Get template infomation
        /// </summary>
        /// <param name="fullDocName"></param>
        /// <returns>null if not exist</returns>
        public TemplateInfo GetTemplateInfo(string fullDocName)
        {
            return _commonProfile.TemplateInfos.ContainsKey(fullDocName) ?
                _commonProfile.TemplateInfos[fullDocName] : null;
        }

        /// <summary>
        /// remove template if it is exist
        /// </summary>
        /// <param name="fullDocName"></param>
        /// <returns>true if exist and remove success</returns>
        public bool RemoveTemplateInfo(string fullDocName)
        {
            if (_commonProfile.TemplateInfos.ContainsKey(fullDocName))
            {
                _commonProfile.TemplateInfos.Remove(fullDocName);
                return true;
            }

            return false;
        }

        public void ChangeTemplateInfoKey(string oldKey, string newKey)
        {
            if (oldKey != newKey && !string.IsNullOrEmpty(newKey))
            {
                if (_commonProfile.TemplateInfos.ContainsKey(oldKey))
                {
                    if (_commonProfile.TemplateInfos.ContainsKey(newKey))
                        _commonProfile.TemplateInfos[newKey] = _commonProfile.TemplateInfos[oldKey];
                    else
                        _commonProfile.TemplateInfos.Add(newKey, _commonProfile.TemplateInfos[oldKey]);

                    _commonProfile.TemplateInfos.Remove(oldKey);
                }
                else
                {
                    if (_commonProfile.TemplateInfos.ContainsKey(newKey))
                        _commonProfile.TemplateInfos[newKey] = new TemplateInfo(newKey);
                    else
                        _commonProfile.TemplateInfos.Add(newKey, new TemplateInfo(newKey));
                }
            }
        }
        #endregion

        #region 2. Domain information
        public DomainInfo CreateDomainInfo(string domainName)
        {
            if (_commonProfile.DomainInfos.ContainsKey(domainName))
                return _commonProfile.DomainInfos[domainName];
            else
            {
                DomainInfo domainInfo = new DomainInfo();
                domainInfo.BusinessName = domainName;
                _commonProfile.DomainInfos.Add(domainName, domainInfo);

                return domainInfo;
            }
        }

        /// <summary>
        /// get domain information
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns>null if not exist</returns>
        public DomainInfo GetDomainInfo(string domainName)
        {
            if (string.IsNullOrEmpty(domainName))
                return null;
            return _commonProfile.DomainInfos.ContainsKey(domainName) ?
                _commonProfile.DomainInfos[domainName] : null;
        }

        /// <summary>
        /// remove domain information if it is exist
        /// </summary>
        /// <param name="fullDocName"></param>
        /// <returns>true if exist and remove success</returns>
        public bool RemoveDomainInfo(string domainName)
        {
            if (_commonProfile.DomainInfos.ContainsKey(domainName))
            {
                _commonProfile.DomainInfos.Remove(domainName);
                return true;
            }

            return false;
        }

        public void ChangeDomainInfoKey(string oldKey, string newKey)
        {
            if (oldKey != newKey && !string.IsNullOrEmpty(newKey))
            {
                if (_commonProfile.DomainInfos.ContainsKey(oldKey))
                {
                    if (_commonProfile.DomainInfos.ContainsKey(newKey))
                        _commonProfile.DomainInfos[newKey] = _commonProfile.DomainInfos[oldKey];
                    else
                        _commonProfile.DomainInfos.Add(newKey, _commonProfile.DomainInfos[oldKey]);

                    _commonProfile.DomainInfos.Remove(oldKey);
                }
                else
                {
                    if (_commonProfile.DomainInfos.ContainsKey(newKey))
                        _commonProfile.DomainInfos[newKey] = new DomainInfo();
                    else
                        _commonProfile.DomainInfos.Add(newKey, new DomainInfo());
                }
            }
        }
        #endregion
    }
}
