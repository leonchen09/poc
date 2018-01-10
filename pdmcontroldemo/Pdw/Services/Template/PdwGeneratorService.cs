
using System;
using System.Collections.Generic;

using ProntoDoc.Framework.Utils;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.CoreObject.DataSegment;

using Pdw.Core;
using Pdwx.DataObjects;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Services.Template
{
    /// <summary>
    /// Gen pdw file
    /// </summary>
    public class PdwGeneratorService
    {
        #region generate pdw file
        /// <summary>
        /// new: gen pdw file with word xml document (save docx as xml file for gen xslt)
        /// </summary>
        /// <param name="fullDocName"></param>
        /// <param name="isFull"></param>
        /// <returns></returns>
        public void GetPdwInfo(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            List<ChecksumInfoItem> checksumInfoItems = new List<ChecksumInfoItem>();

            srvPro.PdwInfo = new PdwInfo();
            TemplateInfo template = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(srvPro.FullDocName);
            if (srvPro.IsFullDoc)
            {
                #region gen osql
                GenOsqlHelper genOsqlHelper = new GenOsqlHelper();
                try
                {
                    srvPro.DomainInfos = new List<DomainInfo>();
                    int domainCount = template.DomainNames.Count;
                    int domainIndex = 0;
                    while(domainIndex < domainCount)
                    {
                        string domainName = template.DomainNames[domainIndex];
                        InternalBookmarkDomain ibmDomain = template.InternalBookmark.GetInternalBookmarkDomain(domainName);
                        if (ibmDomain != null && ibmDomain.InternalBookmarkItems != null &&
                            ibmDomain.InternalBookmarkItems.Count > 0)
                        {
                            srvPro.DomainInfos.Add(Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName));
                            domainIndex++;
                        }
                        else
                        {
                            template.RemoveDomainAt(domainIndex);
                            domainCount--;
                        }
                    }
                    genOsqlHelper.GenOsql(srvPro.DomainInfos, template.InternalBookmark);
                    srvPro.PdwInfo.OsqlString = genOsqlHelper.OsqlString;
                    checksumInfoItems = genOsqlHelper.ChecksumInfoItems;
                }
                catch (BaseException srvExp)
                {
                    Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_GenOsqlError);
                    newSrvExp.Errors.Add(srvExp);
                    LogUtils.Log("GenOsql_ServiceException", srvExp);
                    throw newSrvExp;
                }
                catch (Exception ex)
                {
                    ServiceException srvExp = new ServiceException(
                        ErrorCode.ipe_GenOsqlError,
                        MessageUtils.Expand(Properties.Resources.ipe_GenOsqlError, ex.Message), ex.StackTrace);
                    LogUtils.Log("GenOsql_SystemException", ex);
                    throw srvExp;
                }
                #endregion

                #region gen xsl
                GenXsltHelper genXsltHelper = new GenXsltHelper();
                try
                {
                    srvPro.Ibm = genOsqlHelper.Ibm;
                    genXsltHelper.GenXsl2007(key);
                    srvPro.PdwInfo.XsltString = srvPro.XsltString;
                }
                catch (BaseException srvExp)
                {
                    Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_GenXsltError);
                    newSrvExp.Errors.Add(srvExp);

                    throw newSrvExp;
                }
                catch (Exception ex)
                {
                    ServiceException srvExp = new ServiceException(
                        ErrorCode.ipe_GenXsltError,
                        MessageUtils.Expand(Properties.Resources.ipe_GenXsltError, ex.Message), ex.StackTrace);
                    throw srvExp;
                }
                #endregion

                // update RelationOn into InternalBookmark
                UpdateRelationOns(key);

                // pde content
                Integration.PdeService pdeService = new Integration.PdeService();
                pdeService.AddExportXSD(template.PdeContent);
                foreach (PdeContentItem item in template.PdeContent.Items)
                {
                    if (System.IO.File.Exists(item.FilePath))
                    {
                        try
                        {
                            string pdeFileContent = FileHelper.ExcelToBase64(item.FilePath);
                            item.FileContent = pdeFileContent;
                        }
                        catch { }
                    }
                }
                srvPro.PdwInfo.PdeContent = ObjectSerializeHelper.SerializeToString<PdeContent>(template.PdeContent);
            }

            // get checksum
            GetCheckSum(srvPro, checksumInfoItems, template.InternalBookmark.DocumentSpecificColor, template.InternalBookmark.HasDocumentSpecific);
        }

        /// <summary>
        /// update relationons in internalbookmark
        /// </summary>
        /// <param name="selectedTables"></param>
        private void UpdateRelationOns(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
            TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(srvPro.FullDocName);
            foreach (string domainName in templateInfo.DomainNames)
            {
                DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName);
                List<RelationOn> relationOns = new List<RelationOn>();
                List<string> selectedTables = srvPro.Ibm.GetInternalBookmarkDomain(domainName).GetAllSelectedTables();

                foreach (DSOnClause onClause in domainInfo.DSDomainData.OnClauses.Items)
                {
                    foreach (string alias in selectedTables)
                    {
                        if (onClause.Alias == alias)
                            relationOns.Add(new RelationOn() { UniqueName = onClause.UniqueName, OnClause = onClause.ExClause });
                    }
                }

                templateInfo.InternalBookmark.GetInternalBookmarkDomain(domainName).RelationOns = relationOns;
            }
        }

        /// <summary>
        /// Gen check sum
        /// </summary>
        /// <param name="srvPro"></param>
        /// <param name="lstJParam">List JParameter just buit with select (-> has new order)</param>
        private void GetCheckSum(ServicesProfile srvPro, List<ChecksumInfoItem> checksumItems, string dscColor, bool hasDsc)
        {
            try
            {
                GenChecksumHelper genChkHelper = new GenChecksumHelper();
                srvPro.PdwInfo.ChecksumString = genChkHelper.GenChecksum(
                    srvPro.PdwInfo.OsqlString, checksumItems, srvPro.TemplateType, dscColor, hasDsc, srvPro.FullDocName);
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_GenChecksumError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(
                        ErrorCode.ipe_GenChecksumError,
                        MessageUtils.Expand(Properties.Resources.ipe_GenChecksumError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }
        #endregion
    }
}