
using System;
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject.DataSegment;

using Pdw.SharedMemoryWrapper;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Core.Kernal
{
    public class DataSegmentHelper
    {
        #region get list of domain name
        public static void GetListDomain()
        {
            try
            {
                if (Wkl.MainCtrl.CommonCtrl.CommonProfile.Classifiers == null)
                {
                    Wkl.MainCtrl.CommonCtrl.CommonProfile.Classifiers = new Dictionary<string, Classifer>();
                    Wkl.MainCtrl.CommonCtrl.CommonProfile.ListDomains = new Dictionary<string, bool>();
                    Wkl.MainCtrl.CommonCtrl.CommonProfile.DataSegmentInfo = 
                        MemorySegment.GetAllDomain(AssetManager.FileAdapter.DataSegmentDllPath);

                    if (Wkl.MainCtrl.CommonCtrl.CommonProfile.DataSegmentInfo != null)
                    {
                        foreach (DSHeaderInfo domain in Wkl.MainCtrl.CommonCtrl.CommonProfile.DataSegmentInfo.DSHeaderInfos)
                        {
                            string classifier = (domain.Classifier == null) ? string.Empty : domain.Classifier;
                            string domainName = domain.Name;
                            if (!Wkl.MainCtrl.CommonCtrl.CommonProfile.Classifiers.ContainsKey(classifier))
                            {
                                Wkl.MainCtrl.CommonCtrl.CommonProfile.Classifiers.Add(classifier,
                                    new Classifer(classifier));
                            }

                            Wkl.MainCtrl.CommonCtrl.CommonProfile.Classifiers[classifier].DomainNames.Add(domainName);
                            if (!Wkl.MainCtrl.CommonCtrl.CommonProfile.ListDomains.ContainsKey(domainName))
                                Wkl.MainCtrl.CommonCtrl.CommonProfile.ListDomains.Add(domainName, domain.IsMultiSection);
                        }
                    }
                }
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_GetListDomainError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                Services.ServiceException srvExp = new Services.ServiceException(ErrorCode.ipe_GetListDomainError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetListDomainError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }
        #endregion

        #region load domain information from datasegment and bind to tree
        public static void LoadDomainData(string domainName)
        {
            try
            {
                //If not existed in Domain WKL
                if (!Wkl.MainCtrl.CommonCtrl.CommonProfile.DomainInfos.ContainsKey(domainName))
                {
                    DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.CreateDomainInfo(domainName);

                    DSDomain domainData = MemorySegment.GetDomain(AssetManager.FileAdapter.DataSegmentDllPath, 
                        domainName);
                    domainInfo.DSDomainData = domainData;
                }
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_LoadDomainDataError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;                
            }
            catch (Exception ex)
            {
                Services.ServiceException srvExp = new Services.ServiceException(ErrorCode.ipe_LoadDomainDataError,
                    MessageUtils.Expand(Properties.Resources.ipe_LoadDomainDataError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }
        #endregion
             
        #region Get PlugIn Information
        public static DSPlugin GetPlugInInfo()
        {
            try
            {
                return MemorySegment.GetPluginInfo(AssetManager.FileAdapter.DataSegmentDllPath);
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_GetPluginInfoError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                Services.ServiceException srvExp = new Services.ServiceException(ErrorCode.ipe_GetPluginInfoError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetPluginInfoError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }
        #endregion
    }
}