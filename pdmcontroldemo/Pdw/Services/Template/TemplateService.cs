
using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.CoreObject.DataSegment;

using Pdw.Core;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;
using ProntoDoc.Framework.CoreObject.PdwxObjects;

namespace Pdw.Services.Template
{
    /// <summary>
    /// Check pdw docx content and re-construct
    /// </summary>
    public class TemplateService : Base.BaseXslGenerator
    {
        private WordXml _wXml;
        private List<XmlNode> _wBookmarks = null;
        private List<XmlBookmarkItem> _xmlBookmarks = null;
        private List<XmlNode> _deletedBmTags = null;

        #region check struct
        public void CheckWordBodyStructure(bool isRepairAtt = false)
        {
            try
            {
                _wXml = new WordXml(Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.XmlContent);
                _isAddSection = Wkl.MainCtrl.CommonCtrl.CommonProfile.IsAddSection;
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.WordXmlNode = _wXml.WDoc;

                FindWBookmarks(isRepairAtt);

                FindXmlBookmarks();

                CheckXslStruct();

                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.DeletedTags = _deletedBmTags;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_CheckWordStructError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_CheckWordStructError,
                    MessageUtils.Expand(Properties.Resources.ipe_CheckWordStructError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }

        private void FindWBookmarks(bool isRepairAtt)
        {
            _wBookmarks = new List<XmlNode>();

            XmlNodeList nodes = _wXml.WXml.GetElementsByTagName("*");
            foreach (XmlNode node in nodes)
            {
                if (node.Name == Constant.NodeNameStartBookmark ||
                    node.Name == Constant.NodeNameEndBookmark)
                    _wBookmarks.Add(node);
            }
        }

        private void FindXmlBookmarks()
        {
            _xmlBookmarks = new List<XmlBookmarkItem>();
            _deletedBmTags = new List<XmlNode>();

            int index = 0;
            List<int> removedIndexMarkup = new List<int>();
            while (index < _wBookmarks.Count - 1)
            {
                XmlBookmarkItem xmlBookmark = new XmlBookmarkItem(_wBookmarks[index], _wBookmarks[index + 1]);
                if (xmlBookmark.IsMatch())
                {
                    UpdateXmlBookmarks(xmlBookmark);
                    _xmlBookmarks.Add(xmlBookmark);

                    index += 2;
                }
                else
                {
                    index++;
                    if (_ibm != null) // todo: ngocbv_add => confirm to remove export item
                    {
                        InternalBookmarkItem ibmItem = _ibm.GetInternalBookmarkItem(xmlBookmark.Key);
                        if (ibmItem == null)
                            continue;
                    }
                    removedIndexMarkup.Add(index);
                }
            }

            for (int removeIndex = removedIndexMarkup.Count - 1; removeIndex >= 0; removeIndex--)
                _deletedBmTags.Add(_wBookmarks[removedIndexMarkup[removeIndex]]);
        }

        private void UpdateXmlBookmarks(XmlBookmarkItem xmlBookmark)
        {
            XmlNode nextSibling = xmlBookmark.StartNode.NextSibling;

            while (nextSibling != xmlBookmark.EndNode)
            {
                XmlNodeList child = nextSibling.ChildNodes;
                foreach (XmlNode children in child)
                {
                    if (children.Name == Constant.NodeNameText)
                        xmlBookmark.Text.Add(children);
                    else if (children.Name == Constant.NodeNameDrawing)
                        xmlBookmark.Drawing = children;
                }

                nextSibling = nextSibling.NextSibling;
            }
            if (xmlBookmark.Type == BookmarkType.PdeTag)
                xmlBookmark.BizName = MarkupUtilities.GetOriginalBizName(xmlBookmark.Value);
        }

        /// <summary>
        /// verify xsl struct of xml bookmark collection
        /// </summary>
        private void CheckXslStruct()
        {
            List<XmlBookmarkItem> ifCollection = new List<XmlBookmarkItem>();
            List<XmlBookmarkItem> foreachCollection = new List<XmlBookmarkItem>();

            foreach (XmlBookmarkItem bm in _xmlBookmarks)
            {
                switch (bm.Type)
                {
                    case Core.BookmarkType.StartForeach:
                        foreachCollection.Add(bm);
                        break;
                    case Core.BookmarkType.EndForeach:
                        if (foreachCollection.Count == 0)
                        {
                            _deletedBmTags.Add(bm.StartNode);
                            _deletedBmTags.Add(bm.EndNode);
                        }
                        else
                            foreachCollection.RemoveAt(foreachCollection.Count - 1);
                        break;
                    case Core.BookmarkType.StartIf:
                        ifCollection.Add(bm);
                        break;
                    case Core.BookmarkType.EndIf:
                        if (ifCollection.Count == 0)
                        {
                            _deletedBmTags.Add(bm.StartNode);
                            _deletedBmTags.Add(bm.EndNode);
                        }
                        else
                            ifCollection.RemoveAt(ifCollection.Count - 1);
                        break;
                    default:
                        break;
                }
            }

            foreach (XmlBookmarkItem bm in ifCollection)
            {
                _deletedBmTags.Add(bm.StartNode);
                _deletedBmTags.Add(bm.EndNode);
            }

            foreach (XmlBookmarkItem bm in foreachCollection)
            {
                _deletedBmTags.Add(bm.StartNode);
                _deletedBmTags.Add(bm.EndNode);
            }
        }
        #endregion

        #region repair struct
        public void Repair(string key, bool isReloadWordXml = true)
        {
            try
            {
                TemplateServiceProfile templateServicePro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).TemplateService;
                TemplateInfo templateInfo;

                if (!string.IsNullOrWhiteSpace(Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentFullDocName))
                    templateInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
                else
                    templateInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(templateServicePro.TemplateName);

                if (isReloadWordXml)
                    _wXml = new WordXml(templateInfo.XmlContent);

                foreach (XmlNode node in templateInfo.DeletedTags)
                {
                    if (node.ParentNode != null)
                        node.ParentNode.RemoveChild(node);
                }

                if (!string.IsNullOrEmpty(templateServicePro.FilePath))
                {
                    Reconstruct(templateServicePro.FilePath);
                }

                templateInfo.IsReconstruct = false;
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_RepairWordStructError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_RepairWordStructError,
                    MessageUtils.Expand(Properties.Resources.ipe_RepairWordStructError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }

        private void Reconstruct(string docxFile)
        {
            if (string.IsNullOrEmpty(_wXml.WDoc.InnerXml))
                return;

            using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument output =
                DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(docxFile, true))
            {
                DocumentFormat.OpenXml.Wordprocessing.Body body =
                    new DocumentFormat.OpenXml.Wordprocessing.Body(_wXml.WDoc.InnerXml);

                output.MainDocumentPart.Document.Body = body;
                output.MainDocumentPart.Document.Save();
            }
        }
        #endregion

        #region get xsl content
        public void GetXslContent(string key)
        {
            try
            {
                ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

                // prepare data
                _ibm = srvPro.Ibm;

                // check struct of xml file
                CheckWordBodyStructure(true);

                // repair (remove some incorrect nodes)
                Repair(key, false);

                // replace bookmarks by xslt tags
                ReplaceBookmarkTag();

                // process imported data
                if (srvPro.TemplateType == ProntoDoc.Framework.CoreObject.PdwxObjects.TemplateType.Pdw)
                    ProcessImportedLinks();

                if (_isAddSection)
                    AddSection();

                // build xsl string (put header, body, footer)
                srvPro.XsltString = GenXsl(srvPro.IsXsl2007Format);
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_GetXslContentError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_GetXslContentError,
                    MessageUtils.Expand(Properties.Resources.ipe_GetXslContentError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }

        #region replace bookmark by xsl markup (prepare xsl data)
        private void ReplaceBookmarkTag()
        {
            _foreach = new List<ForeachItem>();
            for (int index = 0; index < _xmlBookmarks.Count; index++)
            {
                XmlBookmarkItem xmlBookmark = _xmlBookmarks[index];
                Core.BookmarkType type = xmlBookmark.Type;
                string bizName = string.Empty;
                List<string> selectedTable = GetSelectedTables(xmlBookmark);
                Relations relations = GetRelations(xmlBookmark);
                XmlNode startNode = xmlBookmark != null ? xmlBookmark.StartNode : null;

                switch (type)
                {
                    case Core.BookmarkType.StartForeach:
                        UpdateForeach(index);
                        xmlBookmark.BizName = bizName;
                        xmlBookmark.XslPath = GetXslPath(xmlBookmark, bizName, false, selectedTable, relations, startNode);
                        IsSameParagraph(xmlBookmark, index);
                        if (!xmlBookmark.IsSameParent)
                            ProcessOpenXslTag(xmlBookmark, Core.BookmarkType.StartForeach);
                        else
                            ProcessOpenXslTagSameParent(xmlBookmark, Core.BookmarkType.StartForeach);
                        break;
                    case Core.BookmarkType.StartIf:
                        bizName = MarkupUtilities.RemoveChars(xmlBookmark.Value, 1, 1);
                        xmlBookmark.BizName = bizName;
                        xmlBookmark.XslPath = GetXslPath(xmlBookmark, bizName, true, selectedTable, relations, startNode);
                        IsSameParagraph(xmlBookmark, index);
                        if (!xmlBookmark.IsSameParent)
                            ProcessOpenXslTag(xmlBookmark, Core.BookmarkType.StartIf);
                        else
                            ProcessOpenXslTagSameParent(xmlBookmark, Core.BookmarkType.StartIf);
                        break;
                    case Core.BookmarkType.Select:
                    case BookmarkType.PdeTag:
                        bizName = MarkupUtilities.RemoveChars(xmlBookmark.Value, 1, 2);
                        xmlBookmark.BizName = bizName;
                        xmlBookmark.XslPath = GetXslPath(xmlBookmark, bizName, false, selectedTable, relations, startNode);
                        ProcessXslSelectTag(xmlBookmark);
                        break;
                    case Core.BookmarkType.Image:
                    case Core.BookmarkType.PdeChart:
                        xmlBookmark.ImageMapping = _wXml.GetImageMapping(xmlBookmark.Drawing);
                        if (!string.IsNullOrEmpty(xmlBookmark.ImageMapping.BizName))
                        {
                            bizName = MarkupUtilities.RemoveChars(xmlBookmark.ImageMapping.BizName, 1, 2);
                            xmlBookmark.BizName = bizName;
                            xmlBookmark.XslPath = GetXslPath(xmlBookmark, bizName, false, selectedTable, relations, startNode);
                            ProcessImage(xmlBookmark);
                        }
                        break;
                    case Core.BookmarkType.EndForeach:
                        if (!xmlBookmark.IsSameParent)
                            ProcessCloseXslTag(xmlBookmark);
                        else
                            ProcessCloseXslTagSameParent(xmlBookmark);
                        _foreach.RemoveAt(_foreach.Count - 1);
                        break;
                    case Core.BookmarkType.EndIf:
                        if (!xmlBookmark.IsSameParent)
                            ProcessCloseXslTag(xmlBookmark);
                        else
                            ProcessCloseXslTagSameParent(xmlBookmark);
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateForeach(int index)
        {
            InternalBookmarkDomain ibmDomain = _ibm.GetInternalBookmarkDomainByItemKey(_xmlBookmarks[index].Key);
            ForeachItem foreachItem = new ForeachItem(index, _xmlBookmarks.Cast<Base.BaseBookmark>(), GetRelations(index),
                ibmDomain.SelectedTables, 0, "", string.Empty);
            _foreach.Add(foreachItem);
        }

        private void IsSameParagraph(XmlBookmarkItem item, int index)
        {
            string endKey = string.Empty;
            if (item.Type == Core.BookmarkType.StartForeach)
                endKey = item.Key.Replace(Core.ProntoMarkup.KeyStartForeach, Core.ProntoMarkup.KeyEndForeach);
            else
                endKey = item.Key.Replace(Core.ProntoMarkup.KeyStartIf, Core.ProntoMarkup.KeyEndIf);

            for (int i = index + 1; i < _xmlBookmarks.Count; i++)
            {
                if (_xmlBookmarks[i].Key == endKey)
                {
                    bool isSame = (item.StartNode.ParentNode == _xmlBookmarks[i].StartNode.ParentNode);
                    _xmlBookmarks[i].IsSameParent = isSame;
                    item.IsSameParent = isSame;
                    return;
                }
            }
        }

        private List<string> GetSelectedTables(int xbmItemIndex)
        {
            return GetSelectedTables(_xmlBookmarks[xbmItemIndex]);
        }

        private Relations GetRelations(int xbmItemIndex)
        {
            return GetRelations(_xmlBookmarks[xbmItemIndex]);
        }

        #region process open xsl tag
        /// <summary>
        /// replace parent paragraph by markup start foreach
        /// </summary>
        /// <param name="item"></param>
        private void ProcessOpenXslTag(XmlBookmarkItem item, Core.BookmarkType type)
        {
            // 1. find parent paragraph
            XmlNode parentPara = FindParentParagraph(item.StartNode);

            // 2. insert xsl tag after parent paragraph
            if (type == Core.BookmarkType.StartForeach)
            {
                ForeachItem foreachItem = _foreach[_foreach.Count - 1];
                string variantName = string.Empty;
                XmlNode foreachNode = InsertBefore(parentPara, item.XslTag(), Constant.AttNameSelect, item.XslPath);
                List<XmlNode> sortNodes = ProcessXslSortTag(foreachNode, item);
                XmlNode variantNode = AddVariantNode(foreachNode, _foreach.Count - 1, out variantName);
                foreachItem.XslTag = foreachNode.CloneNode(true);

                // variant (variant node must be follow after sort node)
                foreachNode.ParentNode.InsertAfter(variantNode, foreachNode);
                foreachItem.XslTag.AppendChild(variantNode.CloneNode(true));
                _foreach[_foreach.Count - 1].VariantName = variantName;

                // sort (sort node must be follow after for-each node)
                foreach (XmlNode sortNode in sortNodes)
                {
                    foreachNode.ParentNode.InsertAfter(sortNode, foreachNode);
                    foreachItem.XslTag.AppendChild(sortNode.CloneNode(true));
                }
            }
            else
                InsertBefore(parentPara, item.XslTag(), Constant.AttNameTest, item.XslPath);

            // 3. remove all node between start node and end node of bookmark
            RemoveSiblingNode(item.StartNode, item.EndNode);

            // 4. remove start and node of bookmark
            if (item.StartNode.ParentNode != null)
                item.StartNode.ParentNode.RemoveChild(item.StartNode);
            if (item.EndNode.ParentNode != null)
                item.EndNode.ParentNode.RemoveChild(item.EndNode);

            // 5. if parent paragraph not contain another bookmark or w:r node that has inner text not equal empty then remove it
            if (CheckDeleteParagraph(parentPara))
            {
                if (parentPara != null && parentPara.ParentNode != null)
                    parentPara.ParentNode.RemoveChild(parentPara);
            }
        }

        private void ProcessOpenXslTagSameParent(XmlBookmarkItem item, Core.BookmarkType type)
        {
            // 1. remove w:r
            foreach (XmlNode node in item.Text)
                node.ParentNode.ParentNode.RemoveChild(node.ParentNode);

            // 2. replace start node by xsl tag
            XmlNode newNode = item.StartNode.OwnerDocument.CreateElement(item.XslTag(), Constant.XslNamespace);
            string attName = string.Empty;
            if (type == Core.BookmarkType.StartForeach)
                attName = Constant.AttNameSelect;
            else
                attName = Constant.AttNameTest;

            XmlAttribute xmlAtt = item.StartNode.OwnerDocument.CreateAttribute(attName);
            xmlAtt.Value = GetAttributeValue(item.XslPath);
            newNode.Attributes.Append(xmlAtt);

            XmlAttribute markupAtt = item.StartNode.OwnerDocument.CreateAttribute(Constant.AttNamePronto);
            markupAtt.Value = Constant.AttValuePronto;
            newNode.Attributes.Append(markupAtt);

            if (item.StartNode.ParentNode != null)
                item.StartNode.ParentNode.ReplaceChild(newNode, item.StartNode);
            if (type == Core.BookmarkType.StartForeach)
            {
                ForeachItem foreachItem = _foreach[_foreach.Count - 1];
                string variantName = string.Empty;
                List<XmlNode> sortNodes = ProcessXslSortTag(newNode, item);
                XmlNode variantNode = AddVariantNode(newNode, _foreach.Count - 1, out variantName);
                foreachItem.XslTag = newNode.CloneNode(true);

                // variant (variant node must be follow after sort node)
                newNode.ParentNode.InsertAfter(variantNode, newNode);
                foreachItem.XslTag.AppendChild(variantNode.CloneNode(true));
                _foreach[_foreach.Count - 1].VariantName = variantName;

                // sort (sort node must be follow after for-each node)
                foreach (XmlNode sortNode in sortNodes)
                {
                    newNode.ParentNode.InsertAfter(sortNode, newNode);
                    foreachItem.XslTag.AppendChild(sortNode.CloneNode(true));
                }
            }

            // 3. remove end node
            if (item.EndNode.ParentNode != null)
                item.EndNode.ParentNode.RemoveChild(item.EndNode);
        }

        /// <summary>
        /// Insert sort tag after foreachNode
        /// </summary>
        /// <param name="foreachNode"></param>
        /// <param name="foreachItem"></param>
        private List<XmlNode> ProcessXslSortTag(XmlNode foreachNode, XmlBookmarkItem foreachItem)
        {
            // // <xsl:sort order="ascending" select="t2/@t2id"/>
            List<XmlNode> sortNodes = new List<XmlNode>();
            Dictionary<string, Core.OrderByType> sorteds = MarkupUtilities.GetOldOrderBy(foreachItem.Value, false);
            if (sorteds.Count > 0)
            {
                string ibmItemKey = foreachItem.Key;
                InternalBookmarkItem ibmItem = _ibm.GetInternalBookmarkItem(ibmItemKey);
                DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(ibmItem.DomainName);
                string[] keys = new string[sorteds.Keys.Count];

                List<string> selectedTables = GetSelectedTables(foreachItem);
                Relations relations = GetRelations(foreachItem);
                XmlNode startNode = foreachItem != null ? foreachItem.StartNode : null;

                sorteds.Keys.CopyTo(keys, 0);
                for (int index = keys.Length - 1; index >= 0; index--)
                {
                    string bizName = keys[index];
                    string originalBizName = domainInfo.GetUdfSortedBizName(bizName);
                    string sortPath = GetXslPath(null, originalBizName, false, selectedTables, relations, startNode);
                    string sortOrder = (sorteds[bizName] == Core.OrderByType.Asc) ? "ascending" : "descending";

                    XmlNode sortNode = CreateXslNode(foreachNode, "xsl:sort", "select", sortPath);
                    AppdendAttribute(sortNode, "order", sortOrder);

                    sortNodes.Add(sortNode);
                }
            }

            return sortNodes;
        }

        private XmlNode CreateXslNode(XmlNode node, string nodeName, string attName = "", string attValue = "",
            string namespaceURI = Constant.XslNamespace)
        {
            XmlNode newNode = node.OwnerDocument.CreateElement(nodeName, namespaceURI);

            if (!string.IsNullOrEmpty(attName))
            {
                XmlAttribute att = CreateXslAttribute(node, attName, attValue);
                newNode.Attributes.Append(att);
            }

            return newNode;
        }

        private XmlAttribute CreateXslAttribute(XmlNode node, string attName, string attValue)
        {
            XmlAttribute att = node.OwnerDocument.CreateAttribute(attName);
            att.Value = attValue;

            return att;
        }

        private XmlNode CreateWordNode(XmlNode node, string nodeName, string attName = "", string attValue = "")
        {
            string wordNamespaceURI = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            XmlNode newNode = node.OwnerDocument.CreateElement(nodeName, wordNamespaceURI);

            if (!string.IsNullOrEmpty(attName))
            {
                XmlAttribute att = CreateWordAttribute(node, attName, attValue);
                newNode.Attributes.Append(att);
            }

            return newNode;
        }

        private XmlAttribute CreateWordAttribute(XmlNode node, string attName, string attValue)
        {
            string wordNamespaceURI = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            XmlAttribute att = node.OwnerDocument.CreateAttribute(attName, wordNamespaceURI);
            att.Value = attValue;

            return att;
        }

        private void AppdendAttribute(XmlNode node, string attName, string attValue)
        {
            if (!string.IsNullOrEmpty(attName))
            {
                XmlAttribute att = node.OwnerDocument.CreateAttribute(attName);
                att.Value = attValue;
                node.Attributes.Append(att);
            }
        }

        private XmlNode FindParentParagraph(XmlNode node)
        {
            if (node != null)
            {
                XmlNode parent = node.ParentNode;
                while (parent != null && parent.Name != Constant.NodeNameParagraph)
                    parent = parent.ParentNode;

                if (parent != null && parent.Name == Constant.NodeNameParagraph)
                    return parent;
            }

            return null;
        }

        private XmlNode InsertBefore(XmlNode node, string nodeName, string attName, string attValue)
        {
            XmlNode newNode = node.OwnerDocument.CreateElement(nodeName, Constant.XslNamespace);

            XmlAttribute xmlAtt = node.OwnerDocument.CreateAttribute(attName);
            xmlAtt.Value = GetAttributeValue(attValue);
            newNode.Attributes.Append(xmlAtt);

            XmlAttribute markupAtt = node.OwnerDocument.CreateAttribute(Constant.AttNamePronto);
            markupAtt.Value = Constant.AttValuePronto;
            newNode.Attributes.Append(markupAtt);

            if (node.ParentNode != null)
                node.ParentNode.InsertBefore(newNode, node);

            return newNode;
        }

        private string GetAttributeValue(string value)
        {
            return value;
        }

        private void RemoveSiblingNode(XmlNode startNode, XmlNode endNode)
        {
            XmlNode nextSibling = startNode.NextSibling;

            while (nextSibling != endNode)
            {
                // check: if first space of boomark is remove then we need keep w:r of its
                if (nextSibling.Name == Constant.NodeNameRun &&
                    (startNode.PreviousSibling == null || startNode.PreviousSibling.Name != Constant.NodeNameRun))
                {
                    if (!string.IsNullOrEmpty(nextSibling.InnerText))
                    {
                        nextSibling.InnerText = string.Empty;
                        nextSibling = nextSibling.NextSibling;

                        continue;
                    }
                }

                XmlNode nextNode = nextSibling.NextSibling;
                if (startNode.ParentNode != null)
                    startNode.ParentNode.RemoveChild(nextSibling);
                nextSibling = nextNode;
            }
        }

        private bool CheckDeleteParagraph(XmlNode wpNode)
        {
            // make sure not delete paragraph node in column node
            if (wpNode != null && wpNode.ParentNode != null && wpNode.ParentNode.Name == Constant.NodeNameColumn)
                return false;

            foreach (XmlNode node in wpNode.ChildNodes)
            {
                if (node.Name == Constant.NodeNameStartBookmark)
                    return false;
                if (node.Name == Constant.NodeNameRun)
                {
                    if (!string.IsNullOrEmpty(node.InnerText.Trim()))
                        return false;
                }
            }

            return true;
        }
        #endregion

        #region process select tag
        private void ProcessXslSelectTag(XmlBookmarkItem item)
        {
            // 1. find parent paragraph
            XmlNode parentPara = FindParentParagraph(item.StartNode);

            // 2. remove all node between start node and end node of bookmark
            if (item.Text != null)
            {
                for (int nodeTextIndex = 1; nodeTextIndex < item.Text.Count; nodeTextIndex++)
                {
                    XmlNode nodeText = item.Text[nodeTextIndex];
                    if (nodeText != null && nodeText.ParentNode != null)
                        nodeText.ParentNode.RemoveChild(nodeText);
                }
            }
            else
                RemoveSiblingNode(item.StartNode, item.EndNode);

            // 3. replace start node of bookmark by xsl tag
            bool isReplaceStartNode = false;
            if (item.Text != null && item.Text.Count > 0 && item.Text[0] != null)
            {
                XmlNode node = item.Text[0];

                XmlNode newNode = node.OwnerDocument.CreateElement(item.XslTag(), Constant.XslNamespace);
                XmlAttribute xmlAtt = node.OwnerDocument.CreateAttribute(Constant.AttNameSelect);
                xmlAtt.Value = GetAttributeValue(item.XslPath);
                newNode.Attributes.Append(xmlAtt);
                node.InnerText = "";
                node.AppendChild(newNode);
            }
            else
                isReplaceStartNode = ReplaceNode(item.StartNode, item.XslTag(), Constant.AttNameSelect, item.XslPath);

            // 4. remove start node and end node of bookmark
            if (!isReplaceStartNode && item.StartNode.ParentNode != null)
                item.StartNode.ParentNode.RemoveChild(item.StartNode);
            if (item.EndNode.ParentNode != null)
                item.EndNode.ParentNode.RemoveChild(item.EndNode);
        }

        private bool ReplaceNode(XmlNode node, string nodeName, string attName, string attValue)
        {
            XmlNode newNode = node.OwnerDocument.CreateElement(nodeName, Constant.XslNamespace);

            XmlAttribute xmlAtt = node.OwnerDocument.CreateAttribute(attName);
            xmlAtt.Value = GetAttributeValue(attValue);
            newNode.Attributes.Append(xmlAtt);

            XmlNode rNode = node.PreviousSibling;
            while (rNode != null && rNode.Name != Constant.NodeNameRun)
            {
                rNode = rNode.PreviousSibling;
            }

            XmlNode tNode = null;
            if (rNode != null && rNode.Name == Constant.NodeNameRun)
            {
                foreach (XmlNode childNode in rNode.ChildNodes)
                {
                    if (childNode.Name == Constant.NodeNameText)
                    {
                        tNode = childNode;
                        break;
                    }
                }
            }

            if (tNode != null)
                tNode.AppendChild(newNode);
            else
            {
                // add for case remove first space of bookmark
                if (node.NextSibling.Name == Constant.NodeNameRun)
                {
                    XmlNode wR = node.NextSibling;
                    XmlNode wT = null;
                    foreach (XmlNode childNode in wR.ChildNodes)
                    {
                        if (childNode.Name == Constant.NodeNameText)
                        {
                            wT = childNode;
                            break;
                        }
                    }
                    if (wT != null)
                    {
                        wT.InnerText = string.Empty;
                        wT.AppendChild(newNode);
                    }
                }
                else if (node.ParentNode != null)
                {
                    node.ParentNode.ReplaceChild(newNode, node);
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region process close xsl tag
        private void ProcessCloseXslTag(XmlBookmarkItem item)
        {
            // 1. find parent paragraph
            XmlNode parentPara = FindParentParagraph(item.StartNode);

            // 2. insert xsl tag before parent paragraph
            // InsertAfter(parentPara, item.XslTag());
            InsertBefore(parentPara, item.XslTag()); // change to fix <xsl:if> directly afer <xsl:fore-each> (my staff 2)

            // 3. remove all node between start node and end node of bookmark
            RemoveSiblingNode(item.StartNode, item.EndNode);

            // 4. remove start and node of bookmark
            if (item.StartNode.ParentNode != null)
                item.StartNode.ParentNode.RemoveChild(item.StartNode);
            if (item.EndNode.ParentNode != null)
                item.EndNode.ParentNode.RemoveChild(item.EndNode);

            // 5. if parent paragraph not contain another bookmark or w:r node that has inner text not equal empty then remove it
            if (CheckDeleteParagraph(parentPara))
            {
                if (parentPara != null && parentPara.ParentNode != null)
                    parentPara.ParentNode.RemoveChild(parentPara);
            }
        }

        private void ProcessCloseXslTagSameParent(XmlBookmarkItem item)
        {
            // 1. remove w:r
            foreach (XmlNode node in item.Text)
            {
                node.ParentNode.ParentNode.RemoveChild(node.ParentNode);
            }

            // 2. replace start node by xsl tag
            XmlNode newNode = item.EndNode.OwnerDocument.CreateElement(item.XslTag(), Constant.XslNamespace);

            if (item.EndNode.ParentNode != null)
                item.EndNode.ParentNode.ReplaceChild(newNode, item.EndNode);

            // 3. remove end node
            if (item.StartNode.ParentNode != null)
                item.StartNode.ParentNode.RemoveChild(item.StartNode);
        }

        private void InsertAfter(XmlNode node, string nodeName)
        {
            XmlNode newNode = node.OwnerDocument.CreateElement(nodeName, Constant.XslNamespace);

            if (node.ParentNode != null)
                node.ParentNode.InsertAfter(newNode, node);
        }
        private void InsertBefore(XmlNode node, string nodeName)
        {
            XmlNode newNode = node.OwnerDocument.CreateElement(nodeName, Constant.XslNamespace);

            if (node.ParentNode != null)
                node.ParentNode.InsertBefore(newNode, node);
        }
        #endregion

        #region process image
        /// <summary>
        /// Process image item
        /// 1. Process relation
        /// 2. Process bookmark
        /// 3. Process package data
        /// </summary>
        /// <param name="item"></param>
        private void ProcessImage(XmlBookmarkItem item)
        {
            XmlNode startTag = null; // start for-each tag
            XmlNode lastTag = null;  // end for-each tag
            XmlNode relation = null;

            string prefix = Core.MarkupConstant.PrefixWithRandomId(_isAddSection);
            string rId = "r" + prefix + _wXml.ImageCounter;
            string imgPath = "media/image" + prefix + _wXml.ImageCounter;

            _wXml.ImageCounter = _wXml.ImageCounter + 1;

            #region 0. verify for-each tag
            if (_foreach.Count > 0) // image is on for-each tag
            {
                startTag = _foreach[0].XslTag.CloneNode(true);

                for (int index = 1; index < _foreach.Count; index++)
                    lastTag = (lastTag == null ? startTag : lastTag).AppendChild(_foreach[index].XslTag.CloneNode(true));
            }
            if (lastTag == null && startTag != null)
                lastTag = startTag;
            #endregion

            #region 1. Process relation (replace rId)
            if (lastTag != null)
            {
                rId = rId + "{$" + _foreach[_foreach.Count - 1].VariantName + "}";
                item.ImageMapping.Relation.Attributes["Id"].Value = rId;

                imgPath = imgPath + "{$" + _foreach[_foreach.Count - 1].VariantName + "}.jpeg";
                item.ImageMapping.Relation.Attributes["Target"].Value = imgPath;

                relation = item.ImageMapping.Relation.CloneNode(true);
                lastTag.AppendChild(relation);
                XmlNode newRelation = startTag.CloneNode(true);
                if (_isAddSection)
                {
                    XmlNode sectionForeach = CreateSelectionForeachTag(item.ImageMapping.Relation);
                    XmlAttribute prontoAtt = newRelation.Attributes[Constant.AttNamePronto];
                    if (prontoAtt != null)
                        newRelation.Attributes.Remove(prontoAtt);
                    sectionForeach.AppendChild(newRelation);

                    XmlNode oldRelation = item.ImageMapping.Relation;
                    oldRelation.ParentNode.ReplaceChild(sectionForeach, oldRelation);
                }
                else
                {
                    XmlNode oldRelation = item.ImageMapping.Relation;
                    oldRelation.ParentNode.ReplaceChild(newRelation, oldRelation);
                }
            }
            else if (_isAddSection)
            {
                item.ImageMapping.Relation.Attributes["Id"].Value = rId;
                imgPath = imgPath + ".jpeg";
                item.ImageMapping.Relation.Attributes["Target"].Value = imgPath;

                XmlNode sectionForeach = CreateSelectionForeachTag(item.ImageMapping.Relation);
                sectionForeach.AppendChild(item.ImageMapping.Relation.CloneNode(true));

                XmlNode oldRelation = item.ImageMapping.Relation;
                oldRelation.ParentNode.ReplaceChild(sectionForeach, oldRelation);
            }

            #endregion

            #region 2. process bookmark (remove start and end node of word bookmark)
            if (item.StartNode.ParentNode != null)
                item.StartNode.ParentNode.RemoveChild(item.StartNode);
            if (item.EndNode.ParentNode != null)
                item.EndNode.ParentNode.RemoveChild(item.EndNode);
            #endregion

            #region 3. replace embedId in blip fill tag
            if (lastTag != null || _isAddSection)
                item.ImageMapping.PictureBlip.Attributes[Constant.AttNameEmbed].Value = rId;
            #endregion

            #region 4. replace picture data by xsl tag
            // 4.1. replace package image binary
            XmlNode picData = item.ImageMapping.Picture.FirstChild;
            picData.InnerText = "";

            // create xsl:value-of tag and replace to binary data
            XmlNode newNode = picData.OwnerDocument.CreateElement(item.XslTag(), Constant.XslNamespace);
            XmlAttribute xmlAtt = picData.OwnerDocument.CreateAttribute(Constant.AttNameSelect);
            xmlAtt.Value = GetAttributeValue(item.XslPath);
            newNode.Attributes.Append(xmlAtt);
            picData.AppendChild(newNode);

            // 4.2. update package image
            if (lastTag != null)
            {
                item.ImageMapping.Picture.Attributes["pkg:name"].Value = "/word/" + imgPath;

                lastTag.RemoveChild(relation);

                lastTag.AppendChild(item.ImageMapping.Picture.CloneNode(true));
                if (_isAddSection)
                {
                    XmlNode sectionForeach = CreateSelectionForeachTag(item.ImageMapping.Picture);
                    XmlNode newPic = startTag.CloneNode(true);
                    XmlAttribute prontoAtt = newPic.Attributes[Constant.AttNamePronto];
                    if (prontoAtt != null)
                        newPic.Attributes.Remove(prontoAtt);
                    sectionForeach.AppendChild(newPic);

                    item.ImageMapping.Picture.ParentNode.ReplaceChild(sectionForeach, item.ImageMapping.Picture);
                }
                else
                    item.ImageMapping.Picture.ParentNode.ReplaceChild(startTag.CloneNode(true), item.ImageMapping.Picture);
            }
            else if (_isAddSection)
            {
                item.ImageMapping.Picture.Attributes["pkg:name"].Value = "/word/" + imgPath;

                XmlNode sectionForeach = CreateSelectionForeachTag(item.ImageMapping.Picture);
                sectionForeach.AppendChild(item.ImageMapping.Picture.CloneNode(true));

                item.ImageMapping.Picture.ParentNode.ReplaceChild(sectionForeach, item.ImageMapping.Picture);
            }
            #endregion
        }

        private XmlNode AddVariantNode(XmlNode foreachNode, int index, out string variantName)
        {
            XmlNode variant = CreateXslNode(foreachNode, Constant.XslVariant);

            variantName = Core.MarkupConstant.XslVariableImage + (index + 1).ToString();
            XmlAttribute varName = CreateXslAttribute(foreachNode, Constant.AttNameName, variantName);
            variant.Attributes.Append(varName);

            XmlAttribute varSelect = CreateXslAttribute(foreachNode, Constant.AttNameSelect,
                index == 0 ? Core.MarkupConstant.XslMultiPosition : string.Format(Core.MarkupConstant.XslVariantImageFormat, index));
            variant.Attributes.Append(varSelect);

            return variant;
        }
        #endregion
        #endregion

        #region process xsl data
        private string GenXsl(bool isXsl2007)
        {
            System.Text.StringBuilder xslContent = new System.Text.StringBuilder();

            // put header
            if (!isXsl2007)
            {
                xslContent.AppendLine(Constant.XmlHeader);
                xslContent.AppendLine(Constant.XslOpen);
                xslContent.AppendLine(Constant.XslOutput);
                xslContent.AppendLine(Constant.XslOpenTemplate);
            }

            // put body
            string boby = string.Empty;
            if (!isXsl2007)
                boby = _wXml.WDoc.OuterXml;
            else
                boby = _wXml.WXml.OuterXml;

            boby = boby.Replace(Constant.OpenTagMarkup, Constant.CloseTag);
            boby = boby.Replace(Constant.CloseForeachMarkup, Constant.XslCloseTagForeach);
            boby = boby.Replace(Constant.CloseIfMarkup, Constant.XslCloseTagIf);
            boby = boby.Replace("Pronto=\"Pronto\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"", "");

            //if (_isAddSection)
            //    boby = boby.Replace(Constant.PdwPageBreakMarkup, Constant.XslPageBreak); // break-page

            xslContent.AppendLine(boby);

            // put footer
            if (!isXsl2007)
            {
                xslContent.AppendLine(Constant.XslCloseTemplate);
                xslContent.AppendLine(Constant.XslClose);
            }

            return xslContent.ToString();
        }
        #endregion

        #region add section
        private XmlNode CreateSelectionForeachTag(XmlNode node, bool hasVariant = true)
        {
            XmlNode newNode = node.OwnerDocument.CreateElement(Constant.XslTagForeach, Constant.XslNamespace);

            XmlAttribute xmlAtt = node.OwnerDocument.CreateAttribute(Constant.AttNameSelect);
            xmlAtt.Value = GetAttributeValue(GetSectionPath());
            newNode.Attributes.Append(xmlAtt);

            XmlAttribute markupAtt = node.OwnerDocument.CreateAttribute(Constant.AttNamePronto);
            markupAtt.Value = Constant.AttValuePronto;
            newNode.Attributes.Append(markupAtt);

            if (hasVariant)
            {
                // 2.1. create variant
                XmlNode variant = CreateSectionVariant(node);

                newNode.AppendChild(variant);
            }

            return newNode;
        }

        private XmlNode CreateSectionVariant(XmlNode node)
        {
            XmlNode variant = CreateXslNode(node, Constant.XslVariant);

            // add name att for variant
            XmlAttribute varName = CreateXslAttribute(node, Constant.AttNameName, Core.MarkupConstant.XslVariableSection);
            variant.Attributes.Append(varName);

            // add select att for variant
            XmlAttribute varSelect = CreateXslAttribute(node, Constant.AttNameSelect, Core.MarkupConstant.XslPosition);
            variant.Attributes.Append(varSelect);

            return variant;
        }

        private void AddSection()
        {
            if (_ibm.InternalBookmarkDomains == null || _ibm.InternalBookmarkDomains.Count < 1 || _wXml.WBody == null)
                return;

            // 0. find w:sectPr
            XmlNode sectPr = null; // (*)
            foreach (XmlNode xmlNode in _wXml.WBody.ChildNodes)
            {
                if (xmlNode.Name == "w:sectPr")
                    sectPr = xmlNode;
            }

            if (sectPr == null)
                return;

            // 2. add xsl:for-each select="[rootTable]" after w:body tag
            XmlNode xnForeach = CreateSelectionForeachTag(_wXml.WBody.FirstChild, false);
            if (_wXml.WBody.FirstChild.ParentNode != null)
                _wXml.WBody.FirstChild.ParentNode.InsertBefore(xnForeach, _wXml.WBody.FirstChild);
            // create variant
            XmlNode variant = CreateSectionVariant(xnForeach);
            _wXml.WBody.FirstChild.ParentNode.InsertAfter(variant, xnForeach);

            // 3. add /xsl:for-each before /w:body tag
            InsertAfter(_wXml.WBody.LastChild, Constant.XslTagForeach);

            // 4. add (*) after /xsl:for-each tag
            XmlNode wpgNumberType = CreateWordNode(_wXml.WBody, "w:pgNumType", "w:start", "1");
            sectPr.AppendChild(wpgNumberType);
            _wXml.WBody.AppendChild(sectPr.CloneNode(true));

            // 5. prevent display empty page at the end of document
            XmlNode xslIf = CreateXslNode(_wXml.WBody, Constant.XslTagIf, Constant.AttNameTest, "position()!=last()");
            XmlNode wpNode = CreateWordNode(_wXml.WBody, "w:p");
            XmlNode wpPrNode = CreateWordNode(_wXml.WBody, "w:pPr");
            xslIf.AppendChild(wpNode);
            wpPrNode.AppendChild(sectPr.CloneNode(true));
            wpNode.AppendChild(wpPrNode);
            sectPr.ParentNode.ReplaceChild(xslIf, sectPr);

            if (_wXml.WRelationships != null)
            {
                string footerTarget = string.Empty;

                // 6. find footer relationship
                foreach (XmlNode xmlNode in _wXml.WRelationships.ChildNodes)
                {
                    if (xmlNode.Attributes["Type"] == null || xmlNode.Attributes["Target"] == null)
                        continue;

                    if ("http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer".Equals(
                        xmlNode.Attributes["Type"].Value))
                    {
                        footerTarget = "/word/" + xmlNode.Attributes["Target"].Value;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(footerTarget))
                    return;

                // find footer package
                if (_wXml.Packages != null)
                {
                    XmlNode footerPackage = null;
                    foreach (XmlNode xmlNode in _wXml.Packages.ChildNodes)
                    {
                        if (xmlNode.Attributes["pkg:name"] != null && footerTarget == xmlNode.Attributes["pkg:name"].Value)
                        {
                            footerPackage = xmlNode;
                            break;
                        }
                    }
                    if (footerPackage == null)
                        return;

                    ProcessFooterPackage(footerPackage);
                }
            }
        }

        /// <summary>
        /// replace NUMPAGES by SECTIONPAGES &lt;w:instrText>NUMPAGES&lt;/w:instrText> 
        /// or &lt;w:fldSimple w:instr=" NUMPAGES   \* MERGEFORMAT ">
        /// </summary>
        /// <param name="footerPackage">footer package node</param>
        private void ProcessFooterPackage(XmlNode footerPackage)
        {
            foreach (XmlNode childNode in footerPackage.ChildNodes)
                ReplacePageNumberBySectionNumber(childNode);
        }

        private void ReplacePageNumberBySectionNumber(XmlNode node)
        {
            if (node != null)
            {
                if ("w:instrText" == node.Name) // <w:instrText>NUMPAGES</w:instrText>
                {
                    if ("NUMPAGES" == node.InnerText)
                        node.InnerText = "SECTIONPAGES";
                }

                if ("w:fldSimple" == node.Name) // <w:fldSimple w:instr=" NUMPAGES   \* MERGEFORMAT ">
                {
                    if (node.Attributes["w:instr"] != null && node.Attributes["w:instr"].Value.Contains("NUMPAGES"))
                    {
                        node.Attributes["w:instr"].Value = node.Attributes["w:instr"].Value.Replace("NUMPAGES", "SECTIONPAGES");
                    }
                }
            }

            foreach (XmlNode childNode in node.ChildNodes)
                ReplacePageNumberBySectionNumber(childNode);
        }
        #endregion

        #region process link field, table, chart
        private void ProcessImportedLinks()
        {
            ProcessImportedFields();

            ProcessImportedCharts();
        }

        private void ProcessImportedFields()
        {
            XmlNodeList nodes = _wXml.WXml.GetElementsByTagName(Constant.NodeNameLinkField);
            if (nodes == null || nodes.Count == 0)
                return;

            // foreach (XmlNode node in nodes)
            for (int index = 0; index < nodes.Count; index++)
            {
                XmlNode node = nodes[index];
                string text = node.InnerText;
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                // check seperate
                XmlNode rNode = node.ParentNode;
                XmlNode pNode = rNode.ParentNode;
                foreach (XmlNode rChild in pNode.ChildNodes)
                {
                    foreach (XmlNode insNode in rChild.ChildNodes)
                    {
                        if (insNode.Name == Constant.NodeNameLinkField)
                        {
                            if (insNode != node)
                            {
                                node.InnerText = node.InnerText + insNode.InnerText;
                                insNode.InnerText = string.Empty;
                            }
                        }
                    }
                }

                bool isExcelLink = text.Trim().StartsWith("Excel.", StringComparison.OrdinalIgnoreCase);
                if (!isExcelLink)
                    isExcelLink = text.Trim().StartsWith("LINK Excel.", StringComparison.OrdinalIgnoreCase);
                if (isExcelLink) // is link with excel (pde)
                {
                    string[] contents = text.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries);
                    if (contents.Length < 5)
                        continue;
                    string filePath = contents[1];
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    fileName = fileName.Substring(Guid.NewGuid().ToString().Length + 1) +
                        ProntoDoc.Framework.CoreObject.PdwxObjects.FileExtension.Xlsx;

                    XmlNode node1 = CreateXslNode(node, Constant.XslText);
                    node1.InnerText = string.Format("{0}\"", contents[0]); // LINK Excel.Sheet.12 "

                    XmlNode node2 = CreateXslNode(node, Constant.XslTagSelect,
                        Constant.AttNameSelect, "@" + FrameworkConstants.PdeFolderPathAttribute);

                    XmlNode node3 = CreateXslNode(node, Constant.XslText);
                    string padding = contents[3].EndsWith("_Select") ? "" : "_Data";
                    node3.InnerText = string.Format("{0}\"{1}\"{2}{3}\"{4}", fileName, contents[2], contents[3], padding, contents[4]);
                    // STN.xlsx" "SheetName!Range or Name's name" \a \f 4 \h 

                    node.InnerText = string.Empty;
                    node.AppendChild(node1);
                    node.AppendChild(node2);
                    node.AppendChild(node3);
                }
            }
        }

        private void ProcessImportedCharts()
        {
            XmlNodeList nodes = _wXml.WXml.GetElementsByTagName(Constant.NodeNameLinkChart);
            if (nodes == null || nodes.Count == 0)
                return;

            foreach (XmlNode node in nodes)
            {
                // node: <Relationship Id="rId1" Target="file:///D:\Test\Data\PdeTemplate.pde" ...../>
                // check target
                XmlAttribute attribute = node.Attributes[Constant.AttNameTarget];
                if (attribute == null)
                    continue;
                string target = attribute.Value;
                string prefix = "file:///";
                if (string.IsNullOrWhiteSpace(target) || !target.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                // remove target attribute
                node.Attributes.Remove(attribute);

                // add target attribute by xsl tag
                string fileName = target.Substring(prefix.Length);
                fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                fileName = fileName.Substring(Guid.NewGuid().ToString().Length + 1) +
                        ProntoDoc.Framework.CoreObject.PdwxObjects.FileExtension.Xlsx;
                XmlNode nodeTarget = CreateXslNode(node, Constant.XslTagAttribute, Constant.AttNameName, Constant.AttNameTarget);
                // result: <xsl:attribute name="Target">
                node.AppendChild(nodeTarget);
                XmlNode nodeTargetPath = CreateXslNode(node, Constant.XslTagSelect, Constant.AttNameSelect,
                    string.Format("concat('{0}', @{1}, '{2}')", prefix, FrameworkConstants.PdeFolderPathAttribute, fileName));
                // result: <xsl:value-of select="concat('file:///',@PdeFolderPath, 'STN.xlsx')" />
                nodeTarget.AppendChild(nodeTargetPath);
            }
        }
        #endregion
        #endregion

        #region build tree data for template

        public void BindTreeViewAndUpdateInternalBM(string key)
        {
            try
            {
                TemplateInfo templateInfo = Wkl.MainCtrl.CommonCtrl.GetTemplateInfo(key);
                if (templateInfo == null || string.IsNullOrWhiteSpace(templateInfo.SelectedDomainName))
                    return;

                DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(templateInfo.SelectedDomainName);
                string dbid = domainInfo.DSDomainData.DBID;
                if (templateInfo.TreeViewData.Nodes.Count == 0)
                    BindingTreeNode(dbid, templateInfo.TreeViewData, null, domainInfo.DSDomainData.TreeView.ChildNodes[0], domainInfo);

                // Update Relation and Index of Internal Bookmark
                InternalBookmarkDomain ibmDomain = templateInfo.InternalBookmark.GetInternalBookmarkDomain(templateInfo.SelectedDomainName);
                if (ibmDomain == null)
                    return;
                foreach (InternalBookmarkItem iBmItem in ibmDomain.InternalBookmarkItems)
                {
                    if (domainInfo.Fields.ContainsKey(iBmItem.BizName))
                    {
                        iBmItem.TableIndex = domainInfo.Fields[iBmItem.BizName].TableIndex;
                        iBmItem.Relation = domainInfo.Fields[iBmItem.BizName].Relation;
                    }
                }
            }
            catch (BaseException srvExp)
            {
                Services.ServiceException newSrvExp = new Services.ServiceException(ErrorCode.ipe_BindTreeError);
                newSrvExp.Errors.Add(srvExp);

                throw newSrvExp;
            }
            catch (Exception ex)
            {
                ServiceException srvExp = new ServiceException(ErrorCode.ipe_BindTreeError,
                    MessageUtils.Expand(Properties.Resources.ipe_BindTreeError, ex.Message), ex.StackTrace);
                throw srvExp;
            }
        }

        /// <summary>
        /// Bind domain data into tree
        /// </summary>
        /// <param name="trnNodes"></param>
        /// <param name="trvTreeViewItem"></param>
        private void BindingTreeNode(string dbid, TreeViewSpliter tvsDomain,
            TreeViewSpliter.TreeViewSpliterItem parent, DSTreeView domainData, DomainInfo domainInfo)
        {
            if (domainData == null)
                return;

            TreeViewSpliter.TreeViewSpliterItem tsiItem = null;
            if (domainData.Visible)
            {
                string customerIconID = domainData.CustomIconID;
                int imageIndex = GetImageIndex(dbid, customerIconID, domainData.Type, domainData.DataType);
                if (parent == null) // the first node in tree
                {
                    tsiItem = tvsDomain.AddNode(domainData.Text, imageIndex, imageIndex, domainData.ToolTip, domainData.Text);
                }
                else
                {
                    tsiItem = tvsDomain.AddNode(parent, domainData.Text, imageIndex, imageIndex, domainData.ToolTip, domainData.Text);
                }
                domainInfo.AddMapBizTreeNodeNames(tsiItem.TreeNodeTop.Text, tsiItem.TreeNodeTop.Name);
            }

            if (domainData.ChildNodes.Count > 0)
            {
                for (int i = 0; i < domainData.ChildNodes.Count; i++)
                {
                    BindingTreeNode(dbid, tvsDomain, tsiItem == null ? null : tsiItem, domainData.ChildNodes[i], domainInfo);
                }
            }
        }

        private DSTreeView CopyDsTreeView(DSTreeView domainData, string fullDocName)
        {
            DSTreeView cloneDsTree = new DSTreeView();
            domainData.Clone();

            cloneDsTree.DataType = domainData.DataType;
            cloneDsTree.TechName = domainData.TechName;
            cloneDsTree.Text = domainData.Text;
            cloneDsTree.ToolTip = domainData.ToolTip;
            cloneDsTree.Type = domainData.Type;
            cloneDsTree.UniqueName = System.Xml.XmlConvert.EncodeName(domainData.UniqueName);
            cloneDsTree.Relation = domainData.Relation;
            cloneDsTree.TableIndex = domainData.TableIndex;
            cloneDsTree.CustomIconID = domainData.CustomIconID;

            return cloneDsTree;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static int GetImageIndex(string dbid, string customerIconID, DSIconType type, SQLDBType dataType)
        {
            IconInfo iconInfo = Wkl.MainCtrl.CommonCtrl.CommonProfile.GetIconInfo(dbid, customerIconID);
            if (iconInfo != null)
                return iconInfo.Index;
            //Field = 0,
            //Group = 1,
            //Table = 2,
            //Condition = 3,
            //UDF = 4,
            //ForEach = 5,
            //USC = 6,
            //SystemInfo = 7,
            //Unknown = 1000,

            int start = type == DSIconType.UDF ? 15 : 0;
            switch (type)
            {
                case DSIconType.Table:
                    return 1;
                case DSIconType.Condition:
                    return 2;
                case DSIconType.ForEach:
                    return 3;
                case DSIconType.Group:
                    return 4;
                case DSIconType.SystemInfo:
                case DSIconType.RenderXY:
                case DSIconType.Field:
                case DSIconType.UDF:
                    switch (dataType.Name)
                    {
                        case SQLTypeName.BINARY:
                            return start + 5;
                        case SQLTypeName.BIT:
                            return start + 6;
                        case SQLTypeName.BOOLEAN:
                            return start + 7;
                        case SQLTypeName.DECIMAL:
                        case SQLTypeName.NUMERIC:
                            return start + 8;
                        case SQLTypeName.FLOAT:
                            return start + 9;
                        case SQLTypeName.IMAGE:
                        case SQLTypeName.ASIMAGE:
                            return start + 10;
                        case SQLTypeName.REAL:
                            return start + 11;
                        case SQLTypeName.INT:
                        case SQLTypeName.BIGINT:
                        case SQLTypeName.SMALLINT:
                        case SQLTypeName.TINYINT:
                            return start + 12;
                        case SQLTypeName.MONEY:
                        case SQLTypeName.SMALLMONEY:
                            return start + 13;
                        case SQLTypeName.DATETIME:
                        case SQLTypeName.SMALLDATETIME:
                        case SQLTypeName.TIMESTAMP:
                            return start + 14;
                        case SQLTypeName.CHAR:
                        case SQLTypeName.NCHAR:
                        case SQLTypeName.NTEXT:
                        case SQLTypeName.NVARCHAR:
                        case SQLTypeName.VARCHAR:
                        case SQLTypeName.TEXT:
                            return start + 15;
                        case SQLTypeName.SQL_VARIANT:
                            return start + 16;
                        case SQLTypeName.VARBINARY:
                            return start + 17;
                        case SQLTypeName.XML:
                            return start + 18;
                        case SQLTypeName.UNIQUEIDENTIFIER:
                            return start + 19;
                        default:
                            break;
                    }
                    break;
            }

            return 0;
        }
        #endregion

        #region helper classes
        private class XmlBookmarkItem : Base.BaseBookmark
        {
            public XmlNode StartNode { get; private set; }
            public XmlNode EndNode { get; private set; }

            public List<XmlNode> Text { get; set; }
            public bool IsSameParent { get; set; }
            public string XslPath { get; set; }

            public WordMapping ImageMapping { get; set; }
            public XmlNode Drawing { get; set; }

            public XmlBookmarkItem()
            {
                Text = new List<XmlNode>();
            }

            public XmlBookmarkItem(XmlNode start, XmlNode end)
                : this()
            {
                StartNode = start;
                EndNode = end;

                // auto calculate properties
                // value in w:name attribute (it is name of work bookmark. ex: P_201115121126078713_S_Foreach)
                Key = Utilities.GetAttribute(StartNode, Constant.AttNameWName);

                // Type
                Type = Core.BookmarkType.None;
                if (StartNode != null)
                {
                    string bookmarkName = Key;
                    if (bookmarkName != null)
                    {
                        if (bookmarkName.StartsWith(Core.MarkupConstant.MarkupPdeTag)) // pde
                        {
                            if (bookmarkName.EndsWith(Core.MarkupConstant.MarkupSelect))
                                Type = Core.BookmarkType.PdeTag;
                            if (bookmarkName.EndsWith(Core.MarkupConstant.MarkupPdeChart))
                                Type = Core.BookmarkType.PdeChart;
                        }
                        else // pdw
                        {
                            if (bookmarkName.EndsWith(Core.MarkupConstant.MarkupStartForeach))
                                Type = Core.BookmarkType.StartForeach;
                            if (bookmarkName.EndsWith(Core.MarkupConstant.MarkupEndForeach))
                                Type = Core.BookmarkType.EndForeach;
                            if (bookmarkName.EndsWith(Core.MarkupConstant.MarkupStartIf))
                                Type = Core.BookmarkType.StartIf;
                            if (bookmarkName.EndsWith(Core.MarkupConstant.MarkupEndIf))
                                Type = Core.BookmarkType.EndIf;
                            if (bookmarkName.EndsWith(Core.MarkupConstant.MarkupSelect))
                                Type = Core.BookmarkType.Select;
                            if (bookmarkName.EndsWith(Core.MarkupConstant.MarkupImage))
                                Type = Core.BookmarkType.Image;
                            if (bookmarkName.EndsWith(Core.MarkupConstant.MarkupComment))
                                Type = Core.BookmarkType.Comment;
                        }                        
                    }
                }
            }

            /// <summary>
            /// inner text of it and its child. (it is value of work bookmark. ex: Employee Name
            /// </summary>
            public string Value
            {
                get
                {
                    string strValue = string.Empty;
                    foreach (XmlNode text in Text)
                        strValue += text.InnerText;

                    return strValue;
                }
            }

            public bool IsMatch()
            {
                if (StartNode == null || EndNode == null)
                    return false;

                string startName = StartNode.Name;
                string endName = EndNode.Name;

                if (startName != Constant.NodeNameStartBookmark)
                    return false;
                if (endName != Constant.NodeNameEndBookmark)
                    return false;
                if (Type == Core.BookmarkType.None)
                    return false;

                string startId = Utilities.GetAttribute(StartNode, Constant.AttNameId);
                string endId = Utilities.GetAttribute(EndNode, Constant.AttNameId);
                if (startId != null && endId != null && startId == endId)
                    return true;

                return false;
            }

            public string XslTag()
            {
                Core.BookmarkType type = Type;
                switch (type)
                {
                    case Core.BookmarkType.StartForeach:
                    case Core.BookmarkType.EndForeach:
                        return Constant.XslTagForeach;
                    case Core.BookmarkType.EndIf:
                    case Core.BookmarkType.StartIf:
                        return Constant.XslTagIf;
                    case Core.BookmarkType.Select:
                    case Core.BookmarkType.Image:
                    case BookmarkType.PdeTag:
                    case BookmarkType.PdeChart:
                        return Constant.XslTagSelect;
                    default:
                        return string.Empty;
                }
            }
        }

        private class WordMapping
        {
            public string RelationId { get; set; }
            public string BizName { get; set; }

            /// <summary>
            /// image package data
            /// </summary>
            public XmlNode Picture { get; set; }

            /// <summary>
            /// a:blip of image
            /// </summary>
            public XmlNode PictureBlip { get; set; }

            /// <summary>
            /// image relation
            /// </summary>
            public XmlNode Relation { get; set; }
        }

        private class WordXml
        {
            public int ImageCounter { get; set; }

            #region 1. properties
            /// <summary>
            /// word xml document
            /// </summary>
            public XmlDocument WXml { get; private set; }

            /// <summary>
            /// relationships of document
            /// </summary>
            public XmlNode WRelationships { get; private set; }

            /// <summary>
            /// word body of document
            /// </summary>
            public XmlNode WDoc { get; private set; }

            /// <summary>
            /// pictures data of document
            /// </summary>
            public List<XmlNode> WPictures { get; private set; }

            public int RelationNumber { get; private set; }

            /// <summary>
            /// w:body node
            /// </summary>
            public XmlNode WBody { get; private set; }

            /// <summary>
            /// pkg:package
            /// </summary>
            public XmlNode Packages { get; private set; }
            #endregion

            #region 2. constructor
            public WordXml(string wordXmlDocContent)
            {
                // 1. load document
                WXml = new XmlDocument();
                WXml.LoadXml(wordXmlDocContent);
                ImageCounter = 1;

                // 2. get package
                Packages = WXml.GetElementsByTagName("pkg:package")[0];

                // 3. get information
                WPictures = new List<XmlNode>();
                if (Packages != null)
                {
                    foreach (XmlNode node in Packages.ChildNodes)
                    {
                        if (node.Name != "pkg:part")
                            continue;
                        string partName = Utilities.GetAttribute(node, "pkg:name");

                        // 3.1. Relationships
                        if (partName == "/word/_rels/document.xml.rels")
                        {
                            WRelationships = node.FirstChild.FirstChild; // pkg:part/pkg:xmlData/Relationships
                            RelationNumber = WRelationships.ChildNodes.Count;
                            continue;
                        }

                        // 3.2. documet body
                        if (partName == "/word/document.xml")
                        {
                            WDoc = node.FirstChild.FirstChild; // pkg:part/pkg:xmlData/w:document
                            continue;
                        }

                        // 3.3. Pictures
                        if (partName.StartsWith("/word/media/"))
                        {
                            if ("image/jpeg".Equals(Utilities.GetAttribute(node, "pkg:contentType")))
                                WPictures.Add(node); // pkg:part
                        }
                    }
                }
                else
                    WDoc = WXml.DocumentElement;

                if (WDoc != null)
                    WBody = WDoc.FirstChild;
            }
            #endregion

            #region 3. methods
            public WordMapping GetImageMapping(XmlNode nodeDrawing)
            {
                if (nodeDrawing != null)
                {
                    XmlNode nodeInline = Utilities.GetChildNode(nodeDrawing, Constant.NodeNameInline);
                    if (nodeInline != null)
                    {
                        WordMapping imgMap = new WordMapping();
                        XmlNode node = Utilities.GetChildNode(nodeInline, Constant.NodeNameDocPro);
                        if (node != null)
                        {
                            string desc = Utilities.GetAttribute(node, Constant.AttNameDesc);
                            if (!string.IsNullOrEmpty(desc) && desc.StartsWith(Pdw.Core.ProntoMarkup.ImageHolderKey))
                                imgMap.BizName = MarkupUtilities.GetBizName(desc);
                            else
                                return imgMap;
                        }

                        node = Utilities.GetChildNode(nodeInline, Constant.NodeNameGraphic);
                        if (node != null)
                        {
                            XmlNode nodeGraphicData = Utilities.GetChildNode(node, Constant.NodeNameGraphicData);
                            if (nodeGraphicData != null)
                            {
                                XmlNode nodePic = Utilities.GetChildNode(nodeGraphicData, Constant.NodeNamePic);
                                if (nodePic != null)
                                {
                                    XmlNode blipFill = Utilities.GetChildNode(nodePic, Constant.NodeNameBlipFill);
                                    if (blipFill != null)
                                    {
                                        XmlNode blip = Utilities.GetChildNode(blipFill, Constant.NodeNameBlip);
                                        if (blip != null)
                                        {
                                            imgMap.RelationId = Utilities.GetAttribute(blip, Constant.AttNameEmbed);
                                            imgMap.PictureBlip = blip;
                                            foreach (XmlNode rNode in WRelationships.ChildNodes)
                                            {
                                                if (imgMap.RelationId == Utilities.GetAttribute(rNode, "Id"))
                                                {
                                                    imgMap.Relation = rNode;
                                                    break;
                                                }
                                            }
                                            if (imgMap.Relation != null)
                                            {
                                                string relTarget = Utilities.GetAttribute(imgMap.Relation, "Target");
                                                foreach (XmlNode picNode in WPictures)
                                                {
                                                    string partName = Utilities.GetAttribute(picNode, "pkg:name");

                                                    if (partName.EndsWith(relTarget))
                                                    {
                                                        imgMap.Picture = picNode;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return imgMap;
                    }
                }
                return null;
            }
            #endregion
        }
        #endregion

        #region constants
        private class Constant
        {
            #region constant for attribute
            /// <summary>
            /// w:id
            /// </summary>
            public const string AttNameId = "w:id";

            /// <summary>
            /// w:name
            /// </summary>
            public const string AttNameWName = "w:name";

            /// <summary>
            /// select
            /// </summary>
            public const string AttNameSelect = "select";

            /// <summary>
            /// test
            /// </summary>
            public const string AttNameTest = "test";

            /// <summary>
            /// name
            /// </summary>
            public const string AttNameName = "name";

            /// <summary>
            /// Pronto
            /// </summary>
            public const string AttNamePronto = "Pronto";

            /// <summary>
            /// Pronto
            /// </summary>
            public const string AttValuePronto = "Pronto";

            /// <summary>
            /// descr
            /// </summary>
            public const string AttNameDesc = "descr";

            /// <summary>
            /// r:embed
            /// </summary>
            public const string AttNameEmbed = "r:embed";

            /// <summary>
            /// Target
            /// </summary>
            public const string AttNameTarget = "Target";
            #endregion

            #region constant for xsl markup tag
            /// <summary>
            /// Pronto="Pronto" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" />
            /// </summary>
            public const string OpenTagMarkup = "Pronto=\"Pronto\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" />";

            /// <summary>
            /// &lt;xsl:if xmlns:xsl="http://www.w3.org/1999/XSL/Transform" />
            /// </summary>
            public const string CloseIfMarkup = "<xsl:if xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" />";

            /// <summary>
            /// &lt;xsl:for-each xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" />
            /// </summary>
            public const string CloseForeachMarkup = "<xsl:for-each xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" />";
            #endregion

            #region word node name
            /// <summary>
            /// w:bookmarkStart
            /// </summary>
            public const string NodeNameStartBookmark = "w:bookmarkStart";

            /// <summary>
            /// w:bookmarkEnd
            /// </summary>
            public const string NodeNameEndBookmark = "w:bookmarkEnd";

            /// <summary>
            /// w:t
            /// </summary>
            public const string NodeNameText = "w:t";

            /// <summary>
            /// w:p
            /// </summary>
            public const string NodeNameParagraph = "w:p";

            /// <summary>
            /// w:r
            /// </summary>
            public const string NodeNameRun = "w:r";

            /// <summary>
            /// w:document
            /// </summary>
            public const string NodeNameDocument = "w:document";

            /// <summary>
            /// w:tc
            /// </summary>
            public const string NodeNameColumn = "w:tc";

            /// <summary>
            /// w:drawing
            /// </summary>
            public const string NodeNameDrawing = "w:drawing";

            /// <summary>
            /// wp:inline
            /// </summary>
            public const string NodeNameInline = "wp:inline";

            /// <summary>
            /// wp:docPr
            /// </summary>
            public const string NodeNameDocPro = "wp:docPr";

            /// <summary>
            /// a:graphic
            /// </summary>
            public const string NodeNameGraphic = "a:graphic";

            /// <summary>
            /// a:graphicData
            /// </summary>
            public const string NodeNameGraphicData = "a:graphicData";

            /// <summary>
            /// pic:pic
            /// </summary>
            public const string NodeNamePic = "pic:pic";

            /// <summary>
            /// pic:blipFill
            /// </summary>
            public const string NodeNameBlipFill = "pic:blipFill";

            /// <summary>
            /// a:blip
            /// </summary>
            public const string NodeNameBlip = "a:blip";

            /// <summary>
            /// w:instrText
            /// </summary>
            public const string NodeNameLinkField = "w:instrText";

            /// <summary>
            /// Relationship
            /// </summary>
            public const string NodeNameLinkChart = "Relationship";
            #endregion

            #region xsl tag and namespace
            /// <summary>
            /// xsl:for-each
            /// </summary>
            public const string XslTagForeach = "xsl:for-each";

            /// <summary>
            /// xsl:if
            /// </summary>
            public const string XslTagIf = "xsl:if";

            /// <summary>
            /// xsl:value-of
            /// </summary>
            public const string XslTagSelect = "xsl:value-of";

            /// <summary>
            /// xsl:attribute
            /// </summary>
            public const string XslTagAttribute = "xsl:attribute";

            /// <summary>
            /// xsl:variable
            /// </summary>
            public const string XslVariant = "xsl:variable";

            /// <summary>
            /// http://www.w3.org/1999/XSL/Transform
            /// </summary>
            public const string XslNamespace = "http://www.w3.org/1999/XSL/Transform";

            /// <summary>
            /// &lt;?xml version=\"1.0\" encoding=\"utf-8\"?>
            /// </summary>
            public const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

            /// <summary>
            /// &lt;xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">
            /// </summary>
            public const string XslOpen = "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">";

            /// <summary>
            /// &lt;xsl:output encoding=\"utf-8\" method=\"xml\" />
            /// </summary>
            public const string XslOutput = "<xsl:output encoding=\"utf-8\" method=\"xml\" />";

            /// <summary>
            /// &lt;xsl:template match="PdwData">
            /// </summary>
            public const string XslOpenTemplate = "<xsl:template match=\"/" + ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwDataRootName + "\">";

            /// <summary>
            /// </xsl:template>
            /// </summary>
            public const string XslCloseTemplate = "</xsl:template>";

            /// <summary>
            /// </xsl:stylesheet>
            /// </summary>
            public const string XslClose = "</xsl:stylesheet>";

            /// <summary>
            /// >
            /// </summary>
            public const string CloseTag = ">";

            /// <summary>
            /// &lt;/xsl:if>
            /// </summary>
            public const string XslCloseTagIf = "</xsl:if>";

            /// <summary>
            /// &lt;/xsl:for-each>
            /// </summary>
            public const string XslCloseTagForeach = "</xsl:for-each>";

            /// <summary>
            /// xsl:text
            /// </summary>
            public const string XslText = "xsl:text";
            #endregion

            /// <summary>
            /// pdw_hard_page_break
            /// </summary>
            public const string PdwPageBreak = "pdw_hard_page_break";

            /// <summary>
            /// &lt;pdw_hard_page_break />
            /// </summary>
            public const string PdwPageBreakMarkup = "<pdw_hard_page_break />";

            /// <summary>
            /// &lt;?hard-pagebreak?>
            /// </summary>
            public const string XslPageBreak = "<?hard-pagebreak?>";
        }
        #endregion

        #region Utilities
        private static class Utilities
        {
            public static string GetAttribute(XmlNode node, string attName)
            {
                if (node != null && node.Attributes[attName] != null)
                    return node.Attributes[attName].Value;

                return null;
            }

            public static XmlNode GetChildNode(XmlNode parentNode, string childNodeName)
            {
                if (parentNode != null)
                {
                    foreach (XmlNode node in parentNode.ChildNodes)
                        if (node.Name == childNodeName)
                            return node;
                }

                return null;
            }
        }
        #endregion
    }
}