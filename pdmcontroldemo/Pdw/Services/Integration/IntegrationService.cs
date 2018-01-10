
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;

using ProntoDoc.Framework.Utils;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.CoreObject.DataSegment;

using Pdw.Core;
using Pdw.Core.Kernal;
using Pdw.AssetManager;
using Pdw.Services.Content;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Services.Integration
{
    /// <summary>
    /// Working with Properties, InternalBookMark
    /// </summary>
    public class IntegrationService
    {
        #region Events
        // internal bookmark
        public event GetInternalBookmarkEventHandler GetInternalBookmark;
        public event SaveInternalBookmarkEventHandler SaveInternalBookmark;

        // internal bookmark item
        public event AddInternalBookmarkItemEventHandler AddInternalBookmarkItem;
        public event RemoveInternalBookmarkItemEventHandler RemoveInternalBookmarkItem;

        // usc
        public event UpdateUscItemsEventHandler UpdateUscItems;

        // pdw info
        public event SavePdwInfoEventHandler SavePdwInfo;
        public event GetPdwInformationEventHandler GetPdwInformation;

        // pdwr info
        public event GetPdwrInformationEventHandler GetPdwrInformation;
        #endregion

        #region get internal bookmark
        public string GetInternalBookmarkString()
        {
            try
            {
                string srvKey = string.Empty;
                ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
                GetInternalBookmark(srvKey);
                InternalBookmark internalBm = srvPro.Ibm;
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);

                return ObjectSerializeHelper.SerializeToString<InternalBookmark>(internalBm);
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_LoadInternalBookmarkError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_LoadInternalBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_LoadInternalBookmarkError, ex.Message), ex.StackTrace);

                throw srvExp;
            }
        }
        #endregion

        #region Validate Structure Properties

        public void IsCorrectFileStructure(string key)
        {
            try
            {
                IntegrationServiceProfile integrationPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).IntegrationService;
                string filePath = integrationPro.CheckCorrectContent_IFilePath;

                bool hasOsql = true;
                bool hasXslt = true;
                bool hasChks = true;
                bool hasInternalBookmark = false;
                bool passPdmControlChecking = true;

                string srvKey = string.Empty;
                ServicesProfile srvPro = null;

                bool isPdm = MarkupUtilities.GetTemplateType(filePath) == TemplateType.Pdm;

                if (Path.GetExtension(filePath).ToLower() == FileExtension.ProntoDocumenentWord || isPdm)
                {
                    hasOsql = false;
                    hasXslt = false;
                    hasChks = false;

                    srvKey = string.Empty;
                    srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);

                    if (isPdm)
                    {
                        passPdmControlChecking = false;
                        srvPro.TemplateType = TemplateType.Pdm;
                    }

                    GetPdwInformation(srvKey);
                    
                    Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);
                    List<XmlObject> xmlObjects = srvPro.XmlObjects;

                    foreach (XmlObject xmlObject in xmlObjects)
                    {
                        if (xmlObject == null)
                            continue;
                        switch (xmlObject.ContentType)
                        {
                            case ContentType.Osql:
                                hasOsql = true;
                                break;
                            case ContentType.Xslt:
                                hasXslt = true;
                                break;
                            case ContentType.Checksum:
                                hasChks = true;
                                break;
                            case ContentType.FormControls:
                                passPdmControlChecking = true;
                                break;
                            default:
                                break;
                        }
                    }
                }

                srvKey = string.Empty;
                srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
                GetInternalBookmark(srvKey);
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);

                hasInternalBookmark = srvPro.Ibm == null ? false : !srvPro.Ibm.IsNull();

                if (hasOsql && hasXslt && hasChks && hasInternalBookmark && passPdmControlChecking)
                {
                    integrationPro.Result = true;
                }
                else
                    integrationPro.Result = false;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_ValidateStructError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_ValidateStructError,
                    MessageUtils.Expand(Properties.Resources.ipe_ValidateStructError, ex.Message), ex.StackTrace);

                throw srvExp;
            }
        }

        #endregion

        #region Validate With Domain

        /// <summary>
        /// Validate Internal Bookmark is match with any Domain. Key is full name of document
        /// </summary>
        /// <param name="fullDocName"></param>
        public void IsInternalBMMatchWithDomain(string key)
        {
            //1.Get InternalBM
            //2.Get Checksum information
            //3.Load Data from datasegment.
            //4.Check match
            try
            {
                IntegrationServiceProfile integrationProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).IntegrationService;
                TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(integrationProfile.TemplateFileName);

                string srvKey = string.Empty;
                ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
                GetInternalBookmark(srvKey);
                templateInfo.InternalBookmark = srvPro.Ibm;
                templateInfo.UpdateDomainNames();
                templateInfo.PdeContent = srvPro.PdeContent;
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);

                //Get Checksum Info.
                ChecksumInfo checksum = GetChecksum();

                integrationProfile.CheckMatchWithDomain_OListMatch = new List<DomainMatch>();
                integrationProfile.Result = true;
                foreach (InternalBookmarkDomain ibmDomain in templateInfo.InternalBookmark.InternalBookmarkDomains)
                {
                    DomainMatchItem domainMatchItem = IsMatchWithDataSegment(ibmDomain, ibmDomain.DomainName);
                    if (domainMatchItem != null && (!domainMatchItem.IsMatch || !domainMatchItem.IsMatchRelationOn)) // not match
                    {
                        DomainMatch domainMatch = FindTheNearestDomain(ibmDomain, domainMatchItem, ibmDomain.DomainName);
                        integrationProfile.CheckMatchWithDomain_OListMatch.Add(domainMatch);
                        integrationProfile.Result = false;
                    }
                }
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_ValidateIbmWithDomainError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_ValidateIbmWithDomainError,
                    MessageUtils.Expand(Properties.Resources.ipe_ValidateIbmWithDomainError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }

        #region helper methods
        /// <summary>
        /// check internal bookmark is match with domain data in datasegment or no
        /// = 0: match (that mean same where clause and all field (biz name and unique name))
        /// > 0: number of field in internal not match with datasegment
        /// &lt; 0: where clause is not match
        /// </summary>
        /// <param name="ibmDomain"></param>
        /// <param name="domainName"></param>
        /// <returns>
        /// </returns>
        private DomainMatchItem IsMatchWithDataSegment(InternalBookmarkDomain ibmDomain, string domainName)
        {
            DataSegmentHelper.LoadDomainData(domainName);
            DomainMatchItem matchResult = new DomainMatchItem(domainName);
            matchResult.IsMatchWhereClause = true;
            DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName);
            if (domainInfo == null || domainInfo.DSDomainData == null)
                return null;

            #region 1. Compare fields
            #region 1.2. get internal bookmark item collection from internal bookmark object
            List<InternalBookmarkItem> iItems = GetInternalBookmarkItemCollection(ibmDomain);
            #endregion

            #region 1.3. compare datasegment's field collection with internal bookmark's field collection
            foreach (InternalBookmarkItem item in iItems)
            {
                if (IsMatchWithDataSegment(item, domainInfo.Fields))
                    matchResult.MatchedFields.Add(item.BizName);
                else
                {
                    matchResult.NotMatchFields.Add(item.BizName);
                    if (item.Type == XsltType.Select)
                        matchResult.SetSingleBiz(item.Key);
                    else
                        matchResult.SetDoubleBiz(item.Key);
                }
            }
            #endregion

            if (!matchResult.IsMatch)
                return matchResult;
            #endregion

            #region 2. Compare RelationOn
            foreach (RelationOn relationOn in ibmDomain.RelationOns)
            {
                DSOnClause item = domainInfo.DSDomainData.OnClauses.Items.FirstOrDefault(c =>
                    string.Equals(relationOn.UniqueName, c.UniqueName) &&
                    string.Equals(relationOn.OnClause, c.ExClause));
                if (item == null)
                {
                    matchResult.IsMatchRelationOn = false;
                    return matchResult;
                }
            }
            #endregion

            #region 3. compare where clause
            if (ibmDomain.WhereClause != domainInfo.DSDomainData.WhereClause.Clause)
            {
                matchResult.IsMatchWhereClause = false;
            }
            #endregion
            return matchResult;
        }

        /// <summary>
        /// item match with datasegment when it (bizname, uniquename, DataType.Name) exist in datasegment
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sFields"></param>
        /// <returns></returns>
        private bool IsMatchWithDataSegment(InternalBookmarkItem item, Dictionary<string, DSTreeView> sFields)
        {
            string bizName = item.BizName;

            if (sFields.ContainsKey(bizName))
            {
                DSTreeView sField = sFields[bizName];

                if (sField.Type == DSIconType.SystemInfo)
                    return ((item.BizName == sField.Text) && (item.TechName == sField.TechName) && sField.Visible);
                if (sField.Type == DSIconType.RenderXY)
                {
                    string techname = sField.TechName.Replace("@", "");
                    return ((item.BizName == sField.Text) && (item.TechName == techname) && sField.Visible);
                }

                return (item.OriginalUniqueName == System.Xml.XmlConvert.DecodeName(sField.UniqueName) &&
                    item.DataType.Name == sField.DataType.Name && sField.Visible);
            }

            return false;
        }

        /// <summary>
        /// get all internal bookmark item (include USC) from document
        /// </summary>
        /// <param name="ibmDomain"></param>
        /// <returns></returns>
        private List<InternalBookmarkItem> GetInternalBookmarkItemCollection(InternalBookmarkDomain ibmDomain)
        {
            List<InternalBookmarkItem> items = new List<InternalBookmarkItem>();

            // internal bookmark item
            foreach (InternalBookmarkItem item in ibmDomain.InternalBookmarkItems)
            {
                InternalBookmarkItem newItem = null;
                if (item.Key.EndsWith(ProntoMarkup.KeySelect))
                    newItem = new InternalBookmarkItem(MarkupUtilities.RemoveChars(item.BizName, 1, 2), item.UniqueName, item.DataType, XsltType.Select);
                else if (item.Key.EndsWith(ProntoMarkup.KeyStartIf) && item.ItemType == DSIconType.Condition.ToString())
                    newItem = new InternalBookmarkItem(MarkupUtilities.RemoveChars(item.BizName, 1, 1), item.UniqueName, item.DataType, XsltType.If);

                if (newItem != null)
                {
                    if (string.IsNullOrEmpty(newItem.UniqueName)) // ngocbv: make sure unique name not is null in case UDF
                        newItem.UniqueName = item.TechName;
                    newItem.Key = newItem.BizName; // ngocbv: keep key for highlight
                    newItem.TechName = item.TechName;
                    items.Add(newItem);
                }
            }

            // usc
            foreach (USCItem usc in ibmDomain.USCItems)
            {
                foreach (USCItem item in usc.Fields)
                {
                    InternalBookmarkItem newItem = new InternalBookmarkItem(item.BusinessName, item.UniqueName, item.DataType, XsltType.If);
                    newItem.Key = usc.BusinessName; // ngocbv: keep key for highlight                    
                    items.Add(newItem);
                }
            }

            return items;
        }

        /// <summary>
        /// find the domain has max match with domain name
        /// </summary>
        /// <param name="ibmDomain"></param>
        /// <param name="matched"></param>
        /// <param name="domainName"></param>
        /// <returns></returns>
        private DomainMatch FindTheNearestDomain(InternalBookmarkDomain ibmDomain, DomainMatchItem matched, string domainName)
        {
            DomainMatch matchResult = new DomainMatch() { DomainName = domainName };
            matchResult.DomainMatchItems = new Dictionary<string, DomainMatchItem>();
            if(matched != null)
                matchResult.DomainMatchItems.Add(domainName, matched);

            // check all domain
            DataSegmentHelper.GetListDomain();
            List<string> domains = Wkl.MainCtrl.CommonCtrl.CommonProfile.ListDomains.Keys.ToList();
            foreach (string domain in domains)
            {
                if (domainName != domain)
                {
                    DomainMatchItem result = IsMatchWithDataSegment(ibmDomain, domain);
                    if (result != null)
                        matchResult.DomainMatchItems.Add(domain, result);
                }
            }

            return matchResult;
        }
        #endregion
        #endregion

        #region validate
        /// <summary>
        /// Delete internal bookmark that not exist in word bookmark
        /// </summary>
        public void ValidateInternalBookmarkCollection(string key)
        {
            // 1. Get internal bookmark object
            // 2. Get all key of internal bookmark item that not exist in word bookmark collection
            // 3. Remove all internal bookmark item has key in key collection above
            try
            {
                IntegrationServiceProfile integrationProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).IntegrationService;
                List<string> removed = new List<string>();
                InternalBookmark iBms = null;
                List<string> removeKeys = new List<string>();

                string srvKey = string.Empty;
                ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
                GetInternalBookmark(srvKey);
                iBms = srvPro.Ibm;
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);

                foreach (InternalBookmarkDomain ibmDomain in iBms.InternalBookmarkDomains)
                {
                    ValidateInternalBookmarkDomain(ibmDomain, integrationProfile.ValidateInternalBM_IListBM,
                        ref removed, ref removeKeys);
                }

                if (integrationProfile.ValidateInternalBM_IIsUpdate)
                {
                    srvKey = string.Empty;
                    srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
                    foreach (string bmKey in removeKeys)
                    {
                        srvPro.WbmKey = bmKey;
                        RemoveInternalBookmark(srvKey);
                    }
                    Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);
                }

                integrationProfile.ValidateInternalBM_OListError = removed;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_ValidateIbmsError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_ValidateIbmsError,
                    MessageUtils.Expand(Properties.Resources.ipe_ValidateIbmsError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }

        private void ValidateInternalBookmarkDomain(InternalBookmarkDomain ibmDomain, Dictionary<string, string> dicIbmItem,
            ref List<string> removed, ref List<string> removeKeys)
        {
            foreach (InternalBookmarkItem item in ibmDomain.InternalBookmarkItems)
            {
                if (!dicIbmItem.ContainsKey(item.Key) || dicIbmItem[item.Key] != item.BizName) // compare key and value
                {
                    removed.Add(item.BizName);
                    removeKeys.Add(item.Key);
                }
            }
        }
        #endregion

        #region InternalBookMark
        /// <summary>
        /// Add internal bookmark item
        /// </summary>
        /// <param name="bm"></param>
        public void AddInternalBookmark(string key)
        {
            try
            {
                ServicesProfile serviceProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);
                IntegrationServiceProfile integrationPro = serviceProfile.IntegrationService;
                InternalBookmarkItem ibmItem = integrationPro.AddInternalBM_IBookmark;
                if (string.IsNullOrWhiteSpace(ibmItem.DomainName))
                    ibmItem.DomainName = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.SelectedDomainName;
                switch (ibmItem.Type)
                {
                    case XsltType.Foreach:
                    case XsltType.If:
                        InternalBookmarkItem start = ibmItem.Clone();
                        InternalBookmarkItem end = ibmItem.Clone();

                        start.BizName = MarkupUtilities.GenTextXslTag(ibmItem.BizName, ibmItem.Type, true);
                        start.Key = MarkupUtilities.GenKeyForXslTag(ibmItem.Key, ibmItem.Type, true);

                        serviceProfile.IbmItem = start;
                        AddInternalBookmarkItem(key);

                        end.BizName = MarkupUtilities.GenTextXslTag(ibmItem.BizName, ibmItem.Type, false);
                        end.Key = MarkupUtilities.GenKeyForXslTag(ibmItem.Key, ibmItem.Type, false);
                        serviceProfile.IbmItem = end;
                        AddInternalBookmarkItem(key);
                        break;
                    case XsltType.Select:
                        InternalBookmarkItem select = ibmItem.Clone();
                        select.BizName = MarkupUtilities.GenTextXslTag(select.BizName, select.Type, true);
                        select.Key = ibmItem.IsImage() ? select.Key + ProntoMarkup.KeyImage
                            : MarkupUtilities.GenKeyForXslTag(select.Key, select.Type, true);

                        serviceProfile.IbmItem = select;
                        AddInternalBookmarkItem(key);
                        break;
                    case XsltType.Comment:
                        InternalBookmarkItem comment = ibmItem.Clone();
                        comment.BizName = MarkupUtilities.GenTextXslTag(comment.BizName, comment.Type, true);
                        comment.Key = ibmItem.IsImage() ? comment.Key + ProntoMarkup.KeyImage
                            : MarkupUtilities.GenKeyForXslTag(comment.Key, comment.Type, true);

                        serviceProfile.IbmItem = comment;
                        AddInternalBookmarkItem(key);
                        break;
                    default:
                        break;
                }
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_AddIbmItemError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_AddIbmItemError,
                    MessageUtils.Expand(Properties.Resources.ipe_AddIbmItemError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }

        public void RemoveInternalBookmark(string key)
        {
            RemoveInternalBookmarkItem(key);
        }

        /// <summary>
        /// update table index and relation for internal bookmark
        /// </summary>
        /// <param name="key"></param>
        public void UpdateInternalBookmark(string key)
        {
            try
            {
                TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(key);
                foreach (string domainName in templateInfo.DomainNames)
                {
                    DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName);
                    InternalBookmarkDomain ibmDomain = templateInfo.InternalBookmark.GetInternalBookmarkDomain(domainName);
                    foreach (InternalBookmarkItem iBmItem in ibmDomain.InternalBookmarkItems)
                    {
                        if (domainInfo.Fields.ContainsKey(iBmItem.BizName))
                        {
                            iBmItem.TableIndex = domainInfo.Fields[iBmItem.BizName].TableIndex;
                            iBmItem.Relation = domainInfo.Fields[iBmItem.BizName].Relation;
                            iBmItem.DomainName = domainName;
                        }
                    }
                }
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_UpdateIbmItemError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_UpdateIbmItemError,
                    MessageUtils.Expand(Properties.Resources.ipe_UpdateIbmItemError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }
        #endregion

        #region Get checksum information
        public ChecksumInfo GetChecksum()
        {
            try
            {
                string srvKey = string.Empty;
                ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
                GetPdwInformation(srvKey);
                Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);
                List<XmlObject> customXmls = srvPro.XmlObjects;
                ChecksumInfo checksum = null;
                string osql = string.Empty;

                foreach (XmlObject xmlObject in customXmls)
                {
                    if (xmlObject == null)
                        continue;

                    if (xmlObject.ContentType == ContentType.Checksum)
                    {
                        checksum = ObjectSerializeHelper.Deserialize<ChecksumInfo>(xmlObject.Content);
                        checksum.InternalBookmark = GetInternalBookmarkString();
                        checksum.Osql = osql;
                    }
                    else if (xmlObject.ContentType == ContentType.Osql)
                    {
                        osql = xmlObject.Content;
                        if (checksum != null)
                            checksum.Osql = osql;
                    }
                }

                return checksum != null ? checksum : new ChecksumInfo();
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_GetChecksumError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_GetChecksumError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetChecksumError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }
        #endregion

        #region usc
        /// <summary>
        /// update usc item collection
        /// key: service key
        /// value: ServiceProfile.UscItems
        /// </summary>
        /// <param name="items"></param>
        public void UpdateUscItemCollection(List<USCItem> items)
        {
            string srvKey = string.Empty;
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.CreateProfile(out srvKey);
            srvPro.UscItems = items;

            UpdateUscItems(srvKey);
            Wkl.MainCtrl.ServiceCtrl.RemoveDataObject(srvKey);
        }

        /// <summary>
        /// Add new a User Specificed Condition.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="listFields"></param>
        /// <returns></returns>
        public USCItem CreateUscItem(string name, string value, Dictionary<string, USCItem> listFields, ref string errorMessage)
        {
            List<USCItem> returnFields = new List<USCItem>();
            USCItem item = null;

            if (ValidateCondition(value, listFields, ref returnFields, ref errorMessage))
            {
                item = new USCItem(name, value);
                if (returnFields != null && returnFields.Count > 0)
                    item.Fields = returnFields;
            }

            return item;
        }

        /// <summary>
        /// Validate Expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="listFields"></param>
        /// <param name="returnFields"></param>
        /// <returns></returns>
        private bool ValidateCondition(string expression, Dictionary<string, USCItem> listFields,
            ref List<USCItem> returnFields, ref string errorMessage)
        {
            ValidateConditionHelper expressionHelper = new ValidateConditionHelper(expression, listFields,
                ref returnFields);
            bool isValid = expressionHelper.IsValid();
            errorMessage = expressionHelper.MessageContent;
            return isValid;
        }

        /// <summary>
        /// update unique name for all fields in UscItem before add to internal bookmark when tagging
        /// ngocbv: need to confirm with Mery
        /// </summary>
        /// <param name="item"></param>
        /// <param name="domainName"></param>
        public void UpdateUniqueNameForUscItem(string key)
        {
            try
            {
                IntegrationServiceProfile inteProfile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).IntegrationService;

                if (inteProfile.UscItem != null && inteProfile.UscItem.Fields != null)
                {
                    DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(inteProfile.UscItem.DomainName);
                    foreach (USCItem field in inteProfile.UscItem.Fields)
                    {
                        if (domainInfo.Fields.ContainsKey(field.BusinessName))
                            field.UniqueName = domainInfo.Fields[field.BusinessName].UniqueName;
                    }
                }
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_UpdateUSCItemError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_UpdateUSCItemError,
                    MessageUtils.Expand(Properties.Resources.ipe_UpdateUSCItemError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }
        #endregion

        public void SaveInterBookmark()
        {
            SaveInternalBookmark();
        }

        #region pdw, pdwr
        public void SavePdwInformation(string key)
        {
            SavePdwInfo(key);
        }

        public void GetPdwInfos(string key)
        {
            GetPdwInformation(key);
        }

        public void GetPdwrInfos(string key)
        {
            GetPdwrInformation(key);
        }
        #endregion

        #region User Data

        public UserData GetLastSelectedDomain()
        {
            try
            {
                string data = FileAdapter.ReadUserData();

                UserData userData = string.IsNullOrWhiteSpace(data) ? null :
                    ObjectSerializeHelper.Deserialize<UserData>(data);

                return userData;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_LoadResourceError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(Core.ErrorCode.ipe_LoadResourceError,
                    Core.MessageUtils.Expand(Properties.Resources.ipe_LoadResourceError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }

        public void SaveSelectedDomainToFile(UserData userData)
        {
            try
            {
                string dataSerialize = ProntoDoc.Framework.Utils.ObjectSerializeHelper.SerializeToString(userData);
                FileAdapter.SaveUserData(dataSerialize);
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_SaveFileError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(Core.ErrorCode.ipe_SaveFileError,
                    Core.MessageUtils.Expand(Properties.Resources.ipe_SaveFileError, ex.Message), ex.StackTrace);

                throw srvExp;
            }
        }
        #endregion

        #region intergrate with excel
        private PdeService _pdeService = null;
        private PdeService PdeService
        {
            get
            {
                if(_pdeService == null)
                    _pdeService = new PdeService();

                return _pdeService;
            }
        }

        public void ImportPde(PdeContentItem pdeContentItem, Document wDoc)
        {
            PdeService.ImportPde(pdeContentItem, wDoc);
        }

        public void ImportPdeTable(Microsoft.Office.Interop.Excel.Range eRange, Document wDoc)
        {
            PdeService.ImportPdeTable(eRange, wDoc);
        }

        public void ImportPdeChart(Microsoft.Office.Interop.Excel.Chart eChart, Document wDoc)
        {
            PdeService.ImportPdeChart(eChart, wDoc);
        }

        public void CloseExcel()
        {
            PdeService.CloseExcel();
        }

        public void CheckPdeContent(Document wDoc, PdeContent pdeContent)
        {
            PdeService.CheckPdeContent(wDoc, pdeContent);
        }

        public List<string> RenderPdeInPdw(Document wDoc, PdeContent pdeContent)
        {
            return PdeService.RenderPdeInPdw(wDoc, pdeContent);
        }
        #endregion
    }
}