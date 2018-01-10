
using System.Xml;
using System.Linq;
using System.Collections.Generic;

using Pdw.Core;
using Wkl = Pdw.WKL.DataController.MainController;
using ProntoDoc.Framework.CoreObject.PdwxObjects;

namespace Pdw.Services.Template.Base
{
    public class BaseXslGenerator
    {
        #region protected variants
        protected List<ForeachItem> _foreach = null;
        protected InternalBookmark _ibm;
        protected bool _isAddSection = false;
        #endregion

        #region get xsl path
        protected string GetXslPath(BaseBookmark xmlBm, string bizName, bool isIf, List<string> selectedTables,
            Relations relations, XmlNode startNode = null)
        {
            BookmarkType type = xmlBm == null ? BookmarkType.Select : xmlBm.Type;
            switch (type)
            {
                case BookmarkType.PdeTag:
                case BookmarkType.PdeChart:
                    return GetPdeXslPath(bizName, selectedTables);
                case BookmarkType.StartForeach:
                    if (_foreach[_foreach.Count - 1].IsContainPdeTag)
                        return GetPdeXslPath(bizName, selectedTables);
                    else
                        return GetPdwXslPath(bizName, isIf, selectedTables, relations, startNode);
                default:
                    return GetPdwXslPath(bizName, isIf, selectedTables, relations, startNode);
            }
        }

        private string GetPdwXslPath(string bizName, bool isIf, List<string> selectedTables,
            Relations relations, XmlNode startNode = null)
        {
            string xslPath = string.Empty;
            List<string> path = new List<string>();
            List<string> parentPath = new List<string>();
            bool isForeachTag = false;

            if (relations.MapInfos.ContainsKey(bizName))
            {
                string tableName = relations.MapInfos[bizName].TableName;
                if (tableName.StartsWith("[") && tableName.EndsWith("]"))
                    tableName = tableName.Substring(1, tableName.Length - 2);
                path = GetPath(selectedTables, tableName);
                bizName = "@" + MarkupUtilities.XmlEncode(bizName);
                if (isIf) bizName = bizName + "= 1";
                if (_foreach.Count > 0) // has foreach parent
                    parentPath = _foreach[_foreach.Count - 1].Path;
            }
            else
            {
                if (bizName == ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark)
                {
                    xslPath = GetSectionPath() + "/";
                    bizName = "@" + Pdw.Core.MarkupUtilities.XmlEncode(bizName);
                }
                else
                {
                    if (_foreach.Count > 0)
                    {
                        isForeachTag = true;
                        path = _foreach[_foreach.Count - 1].Path;
                        if (_foreach.Count > 1) // has foreach parent
                            parentPath = _foreach[_foreach.Count - 2].Path;
                    }
                }
            }

            // build virtual path
            for (int i = 0; i < parentPath.Count - path.Count; i++)
                if (MarkupUtilities.IsExistOnList(selectedTables, parentPath[i]))
                    xslPath = "../" + xslPath;

            // build path
            for (int i = parentPath.Count; i < path.Count; i++)
                if (MarkupUtilities.IsExistOnList(selectedTables, path[i]))
                    xslPath = xslPath + MarkupUtilities.XmlEncode(path[i]) + "/"; // ngocbv: add encode

            xslPath = xslPath + bizName;
            if (xslPath.EndsWith("/"))
                xslPath = xslPath.Remove(xslPath.Length - 1);

            if (string.IsNullOrEmpty(xslPath) && path.Count > 0)
                xslPath = "../" + MarkupUtilities.XmlEncode(path[path.Count - 1]);

            // process add section (put virutal path at the root table)
            if (_isAddSection)
            {
                if ((_foreach.Count == 1 && isForeachTag) || // root for-each tag (add virtual path in the first)
                    (_foreach.Count == 0 && selectedTables.Count > 0)) // root select (remove the first path)
                {
                    string sectionPath = GetSectionPath();
                    int index = IndexOfSecondChar(xslPath, '/', false); // second because the first is DomainName
                    if (index > 0)
                    {
                        xslPath = xslPath.Substring(index + 1);
                        if (string.IsNullOrEmpty(xslPath) && selectedTables.Count > 0) // using for for-each of root after for-each multi section
                            xslPath = "../../" + sectionPath;
                    }
                    else
                    {
                        if (selectedTables.Count > 0 && xslPath == sectionPath) // in case user tag for-each root node
                            xslPath = "../../" + sectionPath;
                    }
                }
            }

            // process when tag in header or footer
            if (startNode != null)
            {
                if (selectedTables.Count > 0)
                {
                    bool isInFooterOrHeader = IsInFooterOrHeader(startNode);
                    if (isInFooterOrHeader)
                    {
                        string sectionPath = GetSectionPath();
                        if (!xslPath.StartsWith(sectionPath))
                            xslPath = sectionPath + "/" + xslPath;
                    }
                }
            }

            return xslPath;
        }

        private string GetPdeXslPath(string bizName, List<string> selectedTables)
        {
            string xslPath = string.Empty;
            List<string> path = selectedTables;
            List<string> parentPath = new List<string>();
            bool isForeachTag = false;

            if (!string.IsNullOrWhiteSpace(bizName)) // data tag
            {
                if (_foreach.Count > 0)
                {
                    string colKey = _foreach[_foreach.Count - 1].Path[0];
                    parentPath = GetPdeSelectedTables(colKey, false);
                }
            }
            else // for-each
            {
                if (_foreach.Count > 0)
                {
                    string colKey = _foreach[_foreach.Count - 1].Path[0];
                    path = selectedTables = GetPdeSelectedTables(colKey, false);

                    if (_foreach.Count > 1) // has foreach parent
                    {
                        colKey = _foreach[_foreach.Count - 2].Path[0];
                        parentPath = GetPdeSelectedTables(colKey, false);
                    }
                }
            }

            // build virtual path
            for (int i = 0; i < parentPath.Count - path.Count; i++)
                if (MarkupUtilities.IsExistOnList(selectedTables, parentPath[i]))
                    xslPath = "../" + xslPath;

            // build path
            for (int i = parentPath.Count; i < path.Count; i++)
                if (MarkupUtilities.IsExistOnList(selectedTables, path[i]))
                    xslPath = xslPath + path[i] + "/";

            if (xslPath.EndsWith("/"))
                xslPath = xslPath.Remove(xslPath.Length - 1);

            if (string.IsNullOrEmpty(xslPath) && path.Count > 0)
                xslPath = "../" + MarkupUtilities.XmlEncode(path[path.Count - 1]);

            // process add section (put virutal path at the root table)
            if (_isAddSection)
            {
                if ((_foreach.Count == 1 && isForeachTag) || // root for-each tag (add virtual path in the first)
                    (_foreach.Count == 0 && selectedTables.Count > 0)) // root select (remove the first path)
                {
                    string sectionPath = GetSectionPath();
                    int index = IndexOfSecondChar(xslPath, '/', false); // second because the first is DomainName
                    if (index > 0)
                    {
                        xslPath = xslPath.Substring(index + 1);
                        if (string.IsNullOrEmpty(xslPath) && selectedTables.Count > 0) // using for for-each of root after for-each multi section
                            xslPath = "../../" + sectionPath;
                    }
                    else
                    {
                        if (selectedTables.Count > 0 && xslPath == sectionPath) // in case user tag for-each root node
                            xslPath = "../../" + sectionPath;
                    }
                }
            }

            return xslPath;
        }

        protected string GetSectionPath()
        {
            InternalBookmarkDomain ibmDomain = _ibm.InternalBookmarkDomains[0];
            return ibmDomain.RootTablePath;
        }

        private bool IsInFooterOrHeader(XmlNode startNode)
        {
            if (startNode == null)
                return false;

            XmlNode parent = startNode.ParentNode;
            while (parent != null)
            {
                if (parent.Name == "pkg:part")
                {
                    XmlAttribute att = parent.Attributes["pkg:contentType"];
                    if (att != null && "application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml".Equals(att.Value))
                        return true;
                    if (att != null && "application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml".Equals(att.Value))
                        return true;
                }

                parent = parent.ParentNode;
            }

            return false;
        }

        private List<string> GetPath(List<string> selectedTables, string tableName)
        {
            List<string> path = new List<string>();
            int level = -1;

            for (int index = 0; index < selectedTables.Count; index++)
            {
                if (selectedTables[index] == tableName)
                {
                    if (index > level)
                        level = index;

                    break;
                }
            }

            if (level == -1)
                path = selectedTables;
            else
            {
                for (int i = 0; i <= level; i++)
                    path.Add(selectedTables[i]);
            }

            return path;
        }

        /// <summary>
        /// get second index of character [value] in string [input]. 
        /// If not find second position then return index of first position where isReverseFirstIndex is set
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <param name="isReverseFirstIndex"></param>
        /// <returns></returns>
        private int IndexOfSecondChar(string input, char value, bool isReverseFirstIndex)
        {
            int index = -1;
            if (!string.IsNullOrEmpty(input))
            {
                int count = 0;
                for (int charIndex = 0; charIndex < input.Length; charIndex++)
                {
                    if (input[charIndex] == value)
                    {
                        if (count > 0)
                            return charIndex;
                        else
                        {
                            if (isReverseFirstIndex)
                                index = charIndex;
                            count++;
                        }
                    }
                }
            }

            return index;
        }
        #endregion

        #region process bookmark tag
        protected string GetSortXsl(string foreachBizName, string domainName, List<string> selectedTables,
            Relations relations, bool isBreakLine)
        {
            string sortXsl = string.Empty;

            foreachBizName = "<" + foreachBizName + ">";
            Dictionary<string, Core.OrderByType> sorteds = Core.MarkupUtilities.GetOldOrderBy(foreachBizName, false);
            if (sorteds.Count > 0)
            {
                Pdw.Core.DomainInfo domainInfo = Wkl.MainCtrl.CommonCtrl.GetDomainInfo(domainName);
                int itemIndex = 0;
                int endIndex = sorteds.Count - 1;
                foreach (string orderBizName in sorteds.Keys)
                {
                    string sortBizName = domainInfo.GetUdfSortedBizName(orderBizName);
                    string sortPath = GetXslPath(null, sortBizName, false, selectedTables, relations);
                    string sortOrder = (sorteds[orderBizName] == Core.OrderByType.Asc) ? "ascending" : "descending";

                    sortXsl = sortXsl + Mht.XslContent.XslSortTag(sortPath, sortOrder, isBreakLine ? (itemIndex < endIndex) : false);
                    itemIndex++;
                }
            }

            return sortXsl;
        }

        protected List<string> GetSelectedTables(BaseBookmark pbmItem)
        {
            if (pbmItem != null)
            {
                switch (pbmItem.Type)
                {
                    case BookmarkType.PdeTag:
                    case BookmarkType.PdeChart:
                        return GetPdeSelectedTables(pbmItem.Key);
                    default:
                        return GetPdwSelectedTables(pbmItem);
                }
            }

            return new List<string>();
        }

        private List<string> GetPdwSelectedTables(BaseBookmark pbmItem)
        {
            string ibmItemKey = pbmItem.Key;
            InternalBookmarkDomain ibmDomain = _ibm.GetInternalBookmarkDomainByItemKey(ibmItemKey);
            string bizName = pbmItem.BizName;
            if (string.IsNullOrWhiteSpace(bizName))
            {
                InternalBookmarkItem ibmItem = ibmDomain.InternalBookmarkItems.FirstOrDefault(c => c.Key == ibmItemKey);
                bizName = ibmItem.OrginalBizName;
            }
            Relations relations = GetRelations(pbmItem);
            if (relations != null && relations.MapInfos.ContainsKey(bizName))
            {
                string tableName = relations.MapInfos[bizName].TableName;
                return ibmDomain.GetSelectedTabledOfPath(tableName);
            }
            return ibmDomain.GetAllSelectedTables();
        }

        private List<string> GetPdeSelectedTables(string key, bool includeKey = true)
        {
            List<string> paths = new List<string>();
            PdeDataTagInfo pdeTagInfo = _ibm.GetPdeDataTagInfo(key);

            // root node
            paths.Add(ProntoDoc.Framework.CoreObject.FrameworkConstants.PdeExportedRootName);

            // stn node
            paths.Add(pdeTagInfo.STN);

            // kind of data
            switch (pdeTagInfo.MapType)
            {
                case MapType.Table:
                    paths.Add(MarkupConstant.PdeExportTable);
                    paths.Add(pdeTagInfo.ParentName);
                    break;
                case MapType.SingleCell:
                    paths.Add(MarkupConstant.PdeExportField);
                    break;
                case MapType.Chart:
                    paths.Add(MarkupConstant.PdeExportChart);
                    break;
                default:
                    break;
            }

            // its path
            if (includeKey)
                paths.Add(pdeTagInfo.MapType == MapType.Chart ? BaseMarkupUtilities.XmlEncode(pdeTagInfo.ExcelName) : pdeTagInfo.ExcelName);

            return paths;
        }

        protected Relations GetRelations(BaseBookmark pbmItem)
        {
            if (pbmItem != null)
            {
                switch (pbmItem.Type)
                {
                    case BookmarkType.PdeTag:
                        return new Relations();
                    case BookmarkType.PdeChart:
                        return new Relations();
                    default:
                        string ibmItemKey = pbmItem.Key;
                        InternalBookmarkDomain ibmDomain = _ibm.GetInternalBookmarkDomainByItemKey(ibmItemKey);
                        return ibmDomain.Relations;
                }
            }

            return new Relations();
        }
        #endregion

        #region helper class
        protected class ForeachItem
        {
            private List<string> _path;
            private List<List<string>> _selectedTable;
            private string _variantName;

            public bool IsContainPdeTag { get; private set; }
            public List<string> Path { get { return _path; } }
            public string XslString { get; set; }
            public string VariantName
            {
                get { return string.IsNullOrEmpty(_variantName) ? "" : _variantName; }
                set { _variantName = value; }
            }
            public XmlNode XslTag { get; set; }

            public int ForeachIndex { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="index">Index of bookmark item</param>
            /// <param name="bookmarks"></param>
            /// <param name="relation"></param>
            /// <param name="selectedTables"></param>
            /// <param name="forechIndex">Index of foreach item</param>
            /// <param name="variant"></param>
            /// <param name="xslString"></param>
            public ForeachItem(int index, IEnumerable<BaseBookmark> bookmarks, Core.Relations relation, List<List<string>> selectedTables,
                int forechIndex, string variant, string xslString)
            {
                _path = new List<string>();
                _selectedTable = selectedTables;
                IsContainPdeTag = false;

                List<string> tableNames = FindTableNames(index, bookmarks, relation);
                _path = GetPath(_selectedTable, tableNames);

                ForeachIndex = forechIndex;
                VariantName = variant;
                XslString = xslString;
            }

            private List<string> FindTableNames(int index, IEnumerable<BaseBookmark> bookmarks, Pdw.Core.Relations relation)
            {
                int boltForeach = 1;
                List<string> tableNames = new List<string>();

                for (int i = index + 1; i < bookmarks.Count(); i++)
                {
                    BaseBookmark item = bookmarks.ElementAt(i);

                    // to setup commanding position on stop lookup children
                    if (boltForeach < 1)
                        break;

                    switch (item.Type)
                    {
                        case Core.BookmarkType.Select:
                        case Core.BookmarkType.StartIf:
                            if (boltForeach == 1)
                                tableNames.Add(GetTableName(item, relation));
                            break;
                        case BookmarkType.PdeTag:
                            if (boltForeach == 1)
                                tableNames.Add(item.Key);
                            _selectedTable = new List<List<string>>();
                            _selectedTable.Add(tableNames);
                            IsContainPdeTag = true;
                            break;
                        case Core.BookmarkType.StartForeach:
                            boltForeach++;
                            break;
                        case Core.BookmarkType.EndForeach:
                            boltForeach--;
                            break;
                        default:
                            break;
                    }
                }

                return tableNames;
            }

            private string GetTableName(BaseBookmark item, Pdw.Core.Relations relation)
            {
                // find biz name
                string bizName = item.BizName;

                // find table name
                string tableName = string.Empty;

                if (relation.MapInfos.ContainsKey(bizName))
                    tableName = relation.MapInfos[bizName].TableName;

                return tableName;
            }

            private List<string> GetPath(List<List<string>> selectedTables, List<string> tableNames)
            {
                Dictionary<int, int> checker = new Dictionary<int, int>();
                for (int index = 0; index < selectedTables.Count; index++)
                {
                    int level = -1;
                    foreach (string tableName in tableNames)
                    {
                        for (int tableIndex = 0; tableIndex < selectedTables[index].Count; tableIndex++)
                        {
                            if (selectedTables[index][tableIndex] == tableName)
                            {
                                if (tableIndex > level)
                                    level = tableIndex;

                                break;
                            }
                        }
                    }

                    if (level > -1)
                        checker.Add(index, level);
                }

                int pathIndex = -1;
                int pathLength = -1;
                foreach (KeyValuePair<int, int> item in checker)
                {
                    if (item.Value > pathLength)
                    {
                        pathLength = item.Value;
                        pathIndex = item.Key;
                    }
                }
                if (pathIndex == -1)
                {
                    pathIndex = 0;
                    pathLength = selectedTables[pathIndex].Count - 1;
                }

                return selectedTables[pathIndex].Take(pathLength + 1).ToList();
            }
        }
        #endregion
    }
}
