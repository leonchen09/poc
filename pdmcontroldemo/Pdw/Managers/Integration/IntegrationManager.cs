using System;
using System.Collections.Generic;
using Microsoft.Office.Core;
using Pdw.Core;
using Pdw.WKL.Profiler.Manager;
using Pdwx.DataObjects;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;
using Wkl = Pdw.WKL.DataController.MainController;
using System.Linq;

namespace Pdw.Managers.Integration
{
    /// <summary>
    /// Working with properties collection in word
    /// </summary>
    public class IntegrationManager : BaseManager
    {
        #region markup properties
        ///// <summary>
        ///// Get value of custom property
        ///// </summary>
        ///// <param name="name">Name of custom property</param>
        ///// <returns>Value of custom property if find</returns>
        //private void GetCustomProperty(string key)
        //{
        //    ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
        //    mgrPro.PropertyValue = GetPropertyValue(mgrPro.PropertyName);
        //}

        private string GetPropertyValue(string propertyName)
        {
            foreach (DocumentProperty docPro in CommonProfile.CustomProperties)
            {
                if (docPro.Name == propertyName)
                    return docPro.Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Add Custom property
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value. Only string with length less equal than 256</param>
        private void AddCustomProperty(string name, string value)
        {
            if (GetPropertyValue(name) != null)
                CommonProfile.CustomProperties[name].Delete();

            CommonProfile.CustomProperties.Add(name, false, MsoDocProperties.msoPropertyTypeString, value, Type.Missing);
        }
        #endregion

        #region get internal bookmark object
        /// <summary>
        /// Get internal bookmark object of active document
        /// </summary>
        /// <returns></returns>
        public void GetInternalBookmark(string key)
        {
            try
            {
                ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);

                if (CurrentTemplateInfo.InternalBookmark.InternalBookmarkDomains.Count == 0)
                {
                    string customXmlPartId = GetCustomPartId();
                    string content = GetCustomXmlPartContent(customXmlPartId);
                    if (string.IsNullOrEmpty(content))
                    {
                        CurrentTemplateInfo.InternalBookmark = new Core.InternalBookmark(true);
                        return;
                    }

                    XmlObject xmlObject = ObjectSerializeHelper.Deserialize<XmlObject>(content);
                    if (xmlObject == null)
                    {
                        CurrentTemplateInfo.InternalBookmark = new Core.InternalBookmark(true);
                        return;
                    }

                    content = xmlObject.Content;
                    if (!string.IsNullOrEmpty(content))
                        CurrentTemplateInfo.InternalBookmark =
                            ObjectSerializeHelper.Deserialize<Pdw.Core.InternalBookmark>(content);
                    else
                        CurrentTemplateInfo.InternalBookmark = new Core.InternalBookmark(true);
                }

                mgrPro.Ibm = CurrentTemplateInfo.InternalBookmark;
                mgrPro.PdeContent = GetPdeContent();
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_GetInternalBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetInternalBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// get custom xml part id of internal bookmark
        /// </summary>
        /// <returns></returns>
        private string GetCustomPartId()
        {
            return GetPropertyValue(Pdw.Core.ProntoMarkup.InternalBMCustomXmlPartId);
        }

        /// <summary>
        /// Get xml content of CustomXmlPart with id = customXmlPartId in current selected word document
        /// </summary>
        /// <param name="customXmlPartId"></param>
        /// <returns></returns>
        private string GetCustomXmlPartContent(string customXmlPartId)
        {
            string content = string.Empty;

            if (string.IsNullOrEmpty(customXmlPartId))
                return content;

            CustomXMLPart xmlPart = CommonProfile.ActiveDoc.CustomXMLParts.SelectByID(customXmlPartId);
            if (xmlPart != null)
                content = xmlPart.XML;

            return content;
        }

        /// <summary>
        /// Get all xml content of CustomXmlParts in current selected word document
        /// </summary>
        /// <returns></returns>
        private List<string> GetCustomXmlPartContents()
        {
            List<string> contents = new List<string>();

            try
            {
                foreach (CustomXMLPart xmlPart in CommonProfile.ActiveDoc.CustomXMLParts)
                    contents.Add(xmlPart.XML);
            }
            catch { }

            return contents;
        }

        private PdeContent GetPdeContent()
        {
            string pdeContentId = GetPropertyValue(PdwInfo.PdeContentXmlPartId);
            if (pdeContentId == null)
                return null;

            string pdeContentData = GetCustomXmlPartContent(pdeContentId);
            XmlObject xmlObject = ObjectSerializeHelper.Deserialize<XmlObject>(pdeContentData);
            PdeContent pdeContent = ObjectSerializeHelper.Deserialize<PdeContent>(xmlObject.Content);

            return pdeContent;
        }
        #endregion

        #region save internal bookmark object
        /// <summary>
        /// Save internal bookmark object into document
        /// </summary>
        public void SaveInternalBookmark()
        {
            // 1. Get xml content
            // 2. Save custom xml part
            // 3. Save custom xml part id
            try
            {
                string content = string.Empty;
                string id = string.Empty;

                if (CurrentTemplateInfo.InternalBookmark != null)
                    content = ObjectSerializeHelper.SerializeToString<InternalBookmark>(CurrentTemplateInfo.InternalBookmark);
                else
                    content = ObjectSerializeHelper.SerializeToString<InternalBookmark>(new Pdw.Core.InternalBookmark());

                XmlObject xmlObject = new XmlObject(content, ContentType.InternalBookmark);
                content = ObjectSerializeHelper.SerializeToString<XmlObject>(xmlObject);
                id = GetCustomPartId();
                id = AddCustomXmlPart(content, id);
                AddCustomProperty(Pdw.Core.ProntoMarkup.InternalBMCustomXmlPartId, id);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SaveInternalBookmarkError,
                    MessageUtils.Expand(Properties.Resources.ipe_SaveInternalBookmarkError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        /// <summary>
        /// Add custom xml part into active document
        /// </summary>
        /// <param name="customXmlPartContent"></param>
        /// <param name="partId"></param>
        /// <returns></returns>
        private string AddCustomXmlPart(string customXmlPartContent, string partId)
        {
            string id = string.Empty;

            try
            {
                ProtectLevel protectLevel = CurrentTemplateInfo.ProtectLevel;
                if (protectLevel == ProtectLevel.Bookmark)
                    UnprotectBookmark();
                if (protectLevel == ProtectLevel.Document)
                    UnprotectDocument();

                if (!string.IsNullOrEmpty(partId))
                {
                    CustomXMLPart oldXmlPart = CommonProfile.ActiveDoc.CustomXMLParts.SelectByID(partId);

                    if (oldXmlPart != null)
                        oldXmlPart.Delete();
                }

                CustomXMLPart xmlPart = CommonProfile.ActiveDoc.CustomXMLParts.Add();
                xmlPart.LoadXML(customXmlPartContent);

                id = xmlPart.Id;
                if (protectLevel == ProtectLevel.Bookmark)
                    ProtectBookmark();
                if (protectLevel == ProtectLevel.Document)
                    ProtectDocument();
            }
            catch { }

            return id;
        }
        #endregion

        #region update internal bookmark
        public void UpdateInternalBookmarkCollection(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            ChangeTemplateInfoKey(mgrPro.OldKey, mgrPro.NewKey);
        }
        #endregion

        #region add and remove internal bookmark item
        /// <summary>
        /// remove internal bookmark item out of internal bookmark object
        /// </summary>
        /// <param name="key"></param>
        public void RemoveInternalBookmarkItem(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            GetInternalBookmark(key);
            CurrentTemplateInfo.InternalBookmark.RemoveInternalBookmarkItem(mgrPro.WbmKey);
        }

        public void AddInternalBookmarkItem(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            InternalBookmarkDomain ibmDomain = CurrentTemplateInfo.InternalBookmark.GetInternalBookmarkDomain(mgrPro.IbmItem.DomainName);
            if (ibmDomain == null)
               ibmDomain = CurrentTemplateInfo.InternalBookmark.CreateInternalBookmarkDomain(mgrPro.IbmItem.DomainName);
            ibmDomain.InternalBookmarkItems.Add(mgrPro.IbmItem);
        }
        #endregion

        #region add and remove UscItem
        /// <summary>
        /// add aa UscItems into internal bookmark
        /// </summary>
        /// <param name="key"></param>
        public void UpdateUscItems(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            GetInternalBookmark(key);
            foreach (USCItem uscItem in mgrPro.UscItems)
            {
                InternalBookmarkDomain ibmDomain = CurrentTemplateInfo.InternalBookmark.GetInternalBookmarkDomain(uscItem.DomainName);
                if (ibmDomain == null)
                    ibmDomain = CurrentTemplateInfo.InternalBookmark.CreateInternalBookmarkDomain(uscItem.DomainName);
                if (ibmDomain.USCItems == null)
                    ibmDomain.USCItems = new List<USCItem>();
                ibmDomain.USCItems.Add(uscItem);
            }
        }
        #endregion

        #region pdw information (osql, xslt, checksum)
        public void SavePdwInfo(string key)
        {
            try
            {
                ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
                XmlObject osql = new XmlObject(mgrPro.PdwInfo.OsqlString, ContentType.Osql);
                XmlObject xslt = new XmlObject(mgrPro.PdwInfo.XsltString, ContentType.Xslt);
                XmlObject chks = new XmlObject(mgrPro.PdwInfo.ChecksumString, ContentType.Checksum);
                XmlObject pdeContent = new XmlObject(mgrPro.PdwInfo.PdeContent, ContentType.PdeContent);

                LogUtils.CreateFile("input.xsl", mgrPro.PdwInfo.XsltString);
                LogUtils.CreateFile("input.osql.xml", mgrPro.PdwInfo.OsqlString);

                string osqlId = GetPropertyValue(PdwInfo.OsqlCustomXmlPartId);
                string xsltId = GetPropertyValue(PdwInfo.XsltCustomXmlPartId);
                string chksId = GetPropertyValue(PdwInfo.ChksCustomXmlPartId);
                string pdeContentId = GetPropertyValue(PdwInfo.PdeContentXmlPartId);

                osqlId = AddCustomXmlPart(ObjectSerializeHelper.SerializeToString<XmlObject>(osql), osqlId);
                xsltId = AddCustomXmlPart(ObjectSerializeHelper.SerializeToString<XmlObject>(xslt), xsltId);
                chksId = AddCustomXmlPart(ObjectSerializeHelper.SerializeToString<XmlObject>(chks), chksId);
                pdeContentId = AddCustomXmlPart(ObjectSerializeHelper.SerializeToString<XmlObject>(pdeContent), pdeContentId);

                AddCustomProperty(PdwInfo.OsqlCustomXmlPartId, osqlId);
                AddCustomProperty(PdwInfo.XsltCustomXmlPartId, xsltId);
                AddCustomProperty(PdwInfo.ChksCustomXmlPartId, chksId);
                AddCustomProperty(PdwInfo.PdeContentXmlPartId, pdeContentId);
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_SavePdwInfoError,
                    MessageUtils.Expand(Properties.Resources.ipe_SavePdwInfoError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }

        public void GetPdwInformation(string key)
        {
            try
            {
                ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
                List<XmlObject> contents = new List<XmlObject>();

                string osqlId = GetPropertyValue(PdwInfo.OsqlCustomXmlPartId);
                string xsltId = GetPropertyValue(PdwInfo.XsltCustomXmlPartId);
                string chksId = GetPropertyValue(PdwInfo.ChksCustomXmlPartId);

                string osql = GetCustomXmlPartContent(osqlId);
                string xslt = GetCustomXmlPartContent(xsltId);
                string chks = GetCustomXmlPartContent(chksId);

                contents.Add(ObjectSerializeHelper.Deserialize<XmlObject>(osql));
                contents.Add(ObjectSerializeHelper.Deserialize<XmlObject>(xslt));
                contents.Add(ObjectSerializeHelper.Deserialize<XmlObject>(chks));

                if (mgrPro.TemplateType == TemplateType.Pdm)
                {
                    string controlsId = GetPropertyValue(PdwInfo.PdmFormControlXmlPartId);
                    string controls = GetCustomXmlPartContent(controlsId);
                    contents.Add(ObjectSerializeHelper.Deserialize<XmlObject>(controls));
                }

                mgrPro.XmlObjects = contents;
            }
            catch (Exception ex)
            {
                ManagerException mgrExp = new ManagerException(ErrorCode.ipe_GetPdwInfoError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetPdwInfoError, ex.Message), ex.StackTrace);

                throw mgrExp;
            }
        }
        #endregion

        #region pdwr information (xml, xslt, setting)
        public void GetPdwrInformation(string key)
        {
            ManagerProfile mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            PdwrInfo pdwrInfo = new PdwrInfo();

            foreach (CustomXMLPart xmlPart in CommonProfile.ActiveDoc.CustomXMLParts)
            {
                string content = xmlPart.XML;
                try
                {
                    XmlObject xmlObject = ObjectSerializeHelper.Deserialize<XmlObject>(content);
                    switch (xmlObject.ContentType)
                    {
                        case ContentType.PdwrSettings:
                            pdwrInfo.SettingString = xmlObject.Content;
                            break;
                        case ContentType.PdwrXml:
                            pdwrInfo.XmlString = xmlObject.Content;
                            break;
                        case ContentType.PdwrXsl:
                            pdwrInfo.XsltString = xmlObject.Content;
                            break;
                        case ContentType.PdeContent:
                            pdwrInfo.PdeContent = xmlObject.Content;
                            break;
                        default:
                            break;
                    }
                }
                catch { }
            }

            mgrPro.PdwrInfo = pdwrInfo;
        }
        #endregion
    }
}