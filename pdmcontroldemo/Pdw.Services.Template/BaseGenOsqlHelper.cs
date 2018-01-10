
using System;
using System.Linq;
using System.Collections.Generic;

using ProntoDoc.Framework.Utils;
using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.CoreObject.DataSegment;
using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdw.Core;
using ProntoDoc.Framework.Expression;

namespace Pdw.Services.Template
{
    abstract class BaseGenOsqlHelper
    {
        protected const int JAVA_LIST_BASE_INDEX = 1;
        public ChecksumInfoItem ChecksumInfoItem { get; protected set; }
        public OsqlXmlItem OsqlXmlItem { get; protected set; }

        public virtual void GenOsql(DomainInfo domain, InternalBookmark ibm, int domainIndex)
        {
        }

        #region update checksuminfo
        protected void UpdateChecksumInfo(ref GenOsqlParamInfo paramInfo, Dictionary<string, string> dicSelectClauseParams, int dbType)
        {
            ChecksumInfoItem = new ProntoDoc.Framework.CoreObject.PdwxObjects.ChecksumInfoItem();
            ChecksumInfoItem.AppID = paramInfo.DomainInfo.DSDomainData.AppID;
            ChecksumInfoItem.Classifier = paramInfo.DomainInfo.DSDomainData.Classifier;
            ChecksumInfoItem.DBID = paramInfo.DomainInfo.DSDomainData.DBID;
            ChecksumInfoItem.DBType = dbType;
            ChecksumInfoItem.DomainAlias = paramInfo.DomainInfo.DSDomainData.Alias;
            ChecksumInfoItem.DomainName = paramInfo.DomainInfo.DSDomainData.Name;
            ChecksumInfoItem.DDName = paramInfo.DomainInfo.DSDomainData.DDName;
            ChecksumInfoItem.DDNameNormalised = paramInfo.DomainInfo.DSDomainData.DDNormalisedName;
            ChecksumInfoItem.HightlightColor = paramInfo.IbmDomain.Color;
            
            if (paramInfo.IbmDomain.InternalBookmarkItems != null)
                ChecksumInfoItem.BookmarkNames = paramInfo.IbmDomain.InternalBookmarkItems.Select(ibmItem => ibmItem.Key).ToList();
            else
                ChecksumInfoItem.BookmarkNames = new List<string>();
            ChecksumInfoItem.HasDataTag = paramInfo.IbmDomain.HasDataTag;
            // ChecksumInfoItem.HasDocumentSpecific = paramInfo.IbmDomain.HasDocumentSpecific;

            // render argument
            bool hasRenderX = ((paramInfo.RenderXY & 1) == 1) ? true : false;
            bool hasRenderY = ((paramInfo.RenderXY & 2) == 2) ? true : false;
            ChecksumInfoItem.RenderArgument = ConvertHelper.ConvertDataAgrumentToSXRenderArgument(
                paramInfo.DomainInfo.DomainArgument.Parameters, hasRenderX, hasRenderY);

            //Remove previous parameters of the old Select clause that has built before
            if (paramInfo.DomainInfo.DomainArgument.JParameters == null)
                paramInfo.DomainInfo.DomainArgument.JParameters = new List<JParameter>();
            else
                paramInfo.DomainInfo.DomainArgument.JParameters.RemoveAll(c => dicSelectClauseParams.ContainsValue(c.ParamName));
            //Insert System Argument form current Select clause
            paramInfo.DomainInfo.DomainArgument.JParameters.InsertRange(0, paramInfo.JParameters);
            //Reindex the JParams
            for (int i = 0; i < paramInfo.DomainInfo.DomainArgument.JParameters.Count; i++)
                paramInfo.DomainInfo.DomainArgument.JParameters[i].Index = i + JAVA_LIST_BASE_INDEX;
            ChecksumInfoItem.JRenderArgument = ConvertHelper.ConvertDataAgrumentToSXJRenderArgument(
                paramInfo.DomainInfo.DomainArgument.JParameters, paramInfo.DomainInfo.DomainArgument.RenderArgumentX,
                paramInfo.DomainInfo.DomainArgument.RenderArgumentY);
            ChecksumInfoItem.SystemParameter = paramInfo.SysParameter;
        }
        #endregion

        #region analyze data
        /// <summary>
        /// get list of data tag fields, data tag others and renderxy
        /// </summary>
        /// <param name="paramInfo"></param>
        protected void AnalyzeDataTag(ref GenOsqlParamInfo paramInfo)
        {
            paramInfo.RenderXY = 0;
            paramInfo.UpdateUSCItemsDictionary();

            int itemIndex = 0;
            Dictionary<string, bool> dataTagChecker = new Dictionary<string, bool>();
            while (itemIndex < paramInfo.IbmDomain.InternalBookmarkItems.Count)
            {
                InternalBookmarkItem ibmItem = paramInfo.IbmDomain.InternalBookmarkItems[itemIndex];
                DSIconType type = ibmItem.DSIconType;
                string itemName = BaseMarkupUtilities.GetOriginalBizName(ibmItem.Key, ibmItem.BizName);
                DSTreeView dsObject = paramInfo.DomainInfo.GetField(itemName);

                if (dsObject != null)
                    paramInfo.RenderXY = paramInfo.RenderXY | CheckRenderXY(dsObject.RenderXYOrder);
                else
                    paramInfo.RenderXY = paramInfo.RenderXY | CheckRenderXY("0");

                itemIndex++;
                if (ProcessEndForeachIfTag(ref paramInfo, ibmItem.Key, itemName, dataTagChecker))
                    continue;

                if (ProcessStartForeachTag(ibmItem.Key, ibmItem.BizName, ref paramInfo, ref dataTagChecker))
                    continue;

                if (ProcessUSCTag(ibmItem, type, itemName, ref paramInfo, ref dataTagChecker))
                    continue;

                ProcessSelectTag(ibmItem, type, itemName, ref paramInfo, ref dataTagChecker);
            }
        }

        private bool ProcessSelectTag(InternalBookmarkItem ibmItem, DSIconType dataTagType, string itemName,
            ref GenOsqlParamInfo paramInfo, ref Dictionary<string, bool> dataTagChecker)
        {
            if ((ibmItem.TableIndex >= 0 && dataTagType != DSIconType.USC) ||
                (dataTagType == DSIconType.SystemInfo) || (dataTagType == DSIconType.Condition) || (dataTagType == DSIconType.RenderXY))
            {
                if (dataTagType == DSIconType.SystemInfo || (dataTagType == DSIconType.Condition) || (dataTagType == DSIconType.RenderXY))
                    ibmItem.TableIndex = Math.Max(0, ibmItem.TableIndex);
                dataTagChecker.Add(itemName, true);
                bool isUdf = dataTagType == DSIconType.UDF || dataTagType == DSIconType.Condition;
                paramInfo.AddDataTags(ibmItem, isUdf);
                return true;
            }

            return false;
        }

        /// <summary>
        /// remove Foreach tag and If tag out of internal bookmark object
        /// </summary>
        /// <param name="ibmDomain"></param>
        /// <param name="itemKey"></param>
        /// <param name="itemName"></param>
        /// <param name="dataTagChecker"></param>
        /// <returns></returns>
        private bool ProcessEndForeachIfTag(ref GenOsqlParamInfo paramInfo, string itemKey, string itemName,
            Dictionary<string, bool> dataTagChecker)
        {
            if (itemKey.Contains(BaseProntoMarkup.KeyEndForeach) ||
                    itemKey.Contains(BaseProntoMarkup.KeyEndIf) ||
                    (dataTagChecker.ContainsKey(itemName)))
            {
                return true;
            }

            return false;
        }

        private bool ProcessStartForeachTag(string ibmItemKey, string ibmItemBizName,
            ref GenOsqlParamInfo paramInfo, ref Dictionary<string, bool> dataTagChecker)
        {
            if (ibmItemKey.Contains(BaseProntoMarkup.KeyStartForeach)) // update for add sort item in for-each
            {
                Dictionary<string, OrderByType> sorteds = BaseMarkupUtilities.GetOldOrderBy(ibmItemBizName, false);
                if (sorteds.Count > 0) // has sort in for-each
                {
                    foreach (string orderBizName in sorteds.Keys)
                    {
                        string sortBizName = paramInfo.DomainInfo.GetUdfSortedBizName(orderBizName);
                        if (sortBizName != orderBizName) // has order by bizname
                        {
                            if (dataTagChecker.ContainsKey(sortBizName))
                                continue;

                            InternalBookmarkItem bm = AddUdfFields(sortBizName, ref paramInfo);
                            dataTagChecker.Add(BaseMarkupUtilities.GetOriginalBizName(bm.Key, bm.BizName), true);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        private bool ProcessUSCTag(InternalBookmarkItem ibmItem, DSIconType dataTagType, string itemName,
            ref GenOsqlParamInfo paramInfo, ref Dictionary<string, bool> dataTagChecker)
        {
            if (dataTagType == DSIconType.USC)
            {
                GetTableIndexForUSC(ibmItem, ref paramInfo);
                USCItem usc = paramInfo.UscItems[itemName];
                string orderFieldName = usc.BaseOnField;

                if (!string.IsNullOrWhiteSpace(orderFieldName) && !dataTagChecker.ContainsKey(orderFieldName))
                {
                    InternalBookmarkItem exist = (from c in paramInfo.IbmDomain.InternalBookmarkItems
                                                  where (c.TableIndex == usc.TableIndex) && (string.Equals(c.ItemType, DSIconType.Field.ToString()))
                                                  select c).FirstOrDefault();
                    //If existed one data field in selected tag, shouldnot include data field in udf
                    if (exist == null || string.IsNullOrEmpty(exist.BizName))
                    {
                        InternalBookmarkItem bm = AddUdfFields(orderFieldName, ref paramInfo);
                        dataTagChecker.Add(BaseMarkupUtilities.GetOriginalBizName(bm.Key, bm.BizName), true);
                    }
                    else
                    {
                        dataTagChecker.Add(itemName, true);
                        paramInfo.AddDataTags(ibmItem, true);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// extract all field in UDF tag
        /// </summary>
        /// <param name="bizName"></param>
        /// <param name="dataTagFields"></param>
        /// <param name="dataTagOthers"></param>
        /// <returns></returns>
        private InternalBookmarkItem AddUdfFields(string bizName, ref GenOsqlParamInfo paramInfo)
        {
            string bookmarkKey = DateTime.Now.ToString(BaseProntoMarkup.BookmarkKeyFormat);
            DSTreeView item = paramInfo.DomainInfo.GetField(bizName);
            InternalBookmarkItem bm = new InternalBookmarkItem(bookmarkKey, item.Text, item.TechName,
                item.Type.ToString(), item.TableIndex, item.Relation, 1);
            bm.UniqueName = item.UniqueName;
            bm.DataType = item.DataType;
            bm.Type = XsltType.Select;
            paramInfo.AddDataTags(bm, true);

            return bm;
        }

        private int GetTableIndexForUSC(InternalBookmarkItem ibmItem, ref GenOsqlParamInfo paramInfo)
        {
            if (paramInfo.UscItems == null || paramInfo.UscItems.Count <= 0)
                return -1;
            string itemName = BaseMarkupUtilities.GetOriginalBizName(ibmItem.Key, ibmItem.BizName);
            USCItem item = paramInfo.UscItems[itemName];
            int maxIndex = -1;
            if (item.Fields.Count > 0)
                foreach (USCItem subField in item.Fields)
                {
                    if (paramInfo.DomainInfo.Fields.Keys.Contains(subField.BusinessName))
                    {
                        if (maxIndex < paramInfo.DomainInfo.Fields[subField.BusinessName].TableIndex)
                        {
                            maxIndex = paramInfo.DomainInfo.Fields[subField.BusinessName].TableIndex;
                            item.BaseOnField = string.IsNullOrWhiteSpace(paramInfo.DomainInfo.Fields[subField.BusinessName].OrginalField) ?
                                paramInfo.DomainInfo.Fields[subField.BusinessName].Text : paramInfo.DomainInfo.Fields[subField.BusinessName].OrginalField;
                        }

                    }
                }
            ibmItem.TableIndex = maxIndex;
            item.TableIndex = maxIndex;

            //Update to Relation
            DSOnClause table = paramInfo.DomainInfo.DSDomainData.OnClauses.Items[maxIndex];
            string tableName = table.Alias;
            if (!paramInfo.DomainInfo.Relations.MapInfos.ContainsKey(itemName))
                paramInfo.DomainInfo.Relations.MapInfos.Add(itemName, new Mapping(itemName, tableName, itemName));
            else
                paramInfo.DomainInfo.Relations.MapInfos[itemName].TableName = tableName;

            return maxIndex;
        }

        private int CheckRenderXY(string renderXY)
        {
            int value = 0;
            if (string.IsNullOrEmpty(renderXY))
                return 0;

            if (renderXY.Contains('1'))
                value = value | 1;
            if (renderXY.Contains('2'))
                value = value | 2;
            return value;
        }
        #endregion

        #region gen select clause for fields
        protected string GenSelectClauseForUSC(InternalBookmarkItem ibmItem, ref GenOsqlParamInfo paramInfo)
        {
            //1.Find USC bookmark in the list Conditions to get actual USC.
            //2.Build SQl
            string sqlClause = string.Empty;
            string itemName = BaseMarkupUtilities.GetOriginalBizName(ibmItem.Key, ibmItem.BizName);
            if (paramInfo.UscItems == null || paramInfo.UscItems.Count <= 0 || ibmItem.Key.EndsWith(BaseProntoMarkup.KeyEndIf))
                return sqlClause;

            USCItem uscItem = null;
            if (paramInfo.UscItems.Keys.Contains(itemName))
                uscItem = paramInfo.UscItems[itemName];

            //
            Dictionary<string, DSTreeView> lowerFields = new Dictionary<string, DSTreeView>();
            foreach (string key in paramInfo.DomainInfo.Fields.Keys)
                lowerFields.Add(key.ToLower(), paramInfo.DomainInfo.Fields[key]);

            //Validate and Build SQl
            if (uscItem != null)
            {
                List<Token> lstTokens = new List<Token>();
                string error = string.Empty;
                ExpressionProvider expressProvider = new ExpressionProvider();
                expressProvider.DoTokenize(uscItem.SQLExpression, ref lstTokens, ref error);

                foreach (Token token in lstTokens)
                {
                    string key = string.Empty;
                    if (token.Name.Contains("[") && token.Name.Contains("]"))
                    {
                        key = token.Name.Substring(1, token.Name.Length - 2).ToLower();
                        if (lowerFields.ContainsKey(key))
                        {
                            token.Name = lowerFields[key].TechName;

                            if (paramInfo.DSRelationRow == null)
                                paramInfo.DSRelationRow = lowerFields[key].Relation.Clone();
                            else
                                paramInfo.DSRelationRow.OrRow(lowerFields[key].Relation);
                        }
                    }
                    sqlClause += token.Name + " ";
                }
            }
            return sqlClause.Trim();
        }
        #endregion

        #region merge select, from, where clause
        protected void MergeSelectFormWhereClause(ref GenOsqlParamInfo paramInfo, Dictionary<string, string> selectClauseParams)
        {
            AddItemToListJParameters(FrameworkConstants.PdwWatermark, ref paramInfo, selectClauseParams, true);

            for (int itemIndex = 0; itemIndex < paramInfo.OsqlStringBuilders.Count; itemIndex++)
            {
                if (!string.IsNullOrWhiteSpace(paramInfo.FromClauses[itemIndex]))
                {
                    paramInfo.OsqlStringBuilders[itemIndex].AppendLine(Sql08QueryConstants.FromClause);
                    paramInfo.OsqlStringBuilders[itemIndex].AppendLine(paramInfo.FromClauses[itemIndex]);
                }

                if (!string.IsNullOrWhiteSpace(paramInfo.WhereClauses[itemIndex]))
                {
                    paramInfo.OsqlStringBuilders[itemIndex].AppendLine(Sql08QueryConstants.WhereClause);
                    paramInfo.OsqlStringBuilders[itemIndex].Osql.AppendLine(paramInfo.WhereClauses[itemIndex]);
                    paramInfo.OsqlStringBuilders[itemIndex].JOsql.AppendLine(paramInfo.JWhereClauses[itemIndex]);
                }
            }
        }

        protected void AddItemToListJParameters(string norTechName, ref GenOsqlParamInfo paramInfo,
            Dictionary<string, string> selectClauseParams, bool insertAtStart = false)
        {
            if (!selectClauseParams.ContainsKey(norTechName))
                return;
            string paramName = selectClauseParams[norTechName]; //string paramName = "@" + norTechName;
            bool isNotExist = paramInfo.JParameters.Count(c => string.Compare(c.ParamName, paramName, true) == 0) == 0;
            if (isNotExist)
            {
                JParameter newJParam = new JParameter();//Value of Index property will be set later, before save                
                newJParam.ParamName = paramName;
                newJParam.DataType = new SQLDBType(SQLTypeName.NVARCHAR);

                if (insertAtStart)
                    paramInfo.JParameters.Insert(0, newJParam);
                else
                    paramInfo.JParameters.Add(newJParam);
            }
        }

        protected void AddItemToSystemParams(string techName, ref GenOsqlParamInfo paramInfo)
        {
            switch (techName)
            {
                case ProntoDoc.Framework.CoreObject.FrameworkConstants.PluginSystemInfo.TemplateName:
                    paramInfo.SysParameter.HasTemplateName = true;
                    break;
                case ProntoDoc.Framework.CoreObject.FrameworkConstants.PluginSystemInfo.TemplateVersion:
                    paramInfo.SysParameter.HasTemplateVersion = true;
                    break;
                case ProntoDoc.Framework.CoreObject.FrameworkConstants.PluginSystemInfo.TemplateID:
                    paramInfo.SysParameter.HasTemplateId = true;
                    break;
                case ProntoDoc.Framework.CoreObject.FrameworkConstants.PluginSystemInfo.UserId:
                    paramInfo.SysParameter.HasUserId = true;
                    break;
                case ProntoDoc.Framework.CoreObject.FrameworkConstants.PluginSystemInfo.ComputerName:
                    paramInfo.SysParameter.HasComputerName = true;
                    break;
                case ProntoDoc.Framework.CoreObject.FrameworkConstants.PluginSystemInfo.GeneratedTime:
                    paramInfo.SysParameter.HasGeneratedTime = true;
                    break;
                case ProntoDoc.Framework.CoreObject.FrameworkConstants.PluginSystemInfo.RenderRequestID:
                    paramInfo.SysParameter.HasRenderRequestID = true;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region gen where clause
        protected void GenWhereClause(ref GenOsqlParamInfo paramInfo)
        {
            if (paramInfo.WhereClauses == null)
                paramInfo.WhereClauses = new List<string>();
            if (paramInfo.JWhereClauses == null)
                paramInfo.JWhereClauses = new List<string>();
            if (paramInfo.DomainInfo.DSDomainData.WhereClause.DSJoins == null ||
                paramInfo.DomainInfo.DSDomainData.WhereClause.DSJoins.Count == 0)
            {
                paramInfo.WhereClauses.Add(paramInfo.DomainInfo.DSDomainData.WhereClause.Clause);
                paramInfo.JWhereClauses.Add(paramInfo.DomainInfo.DSDomainData.WhereClause.JavaClause);
            }
            else
            {
                paramInfo.DSWhereClauseRelationRow = null;
                string whereClause = string.Empty;
                string jWhereClause = string.Empty;
                string logicType = string.Empty;
                bool hasRootJoin = false;
                foreach (DSJoin dsJoin in paramInfo.DomainInfo.DSDomainData.WhereClause.DSJoins)
                {
                    string usedTables = BitHelper.ToBinaryFormat(dsJoin.Tables.Data, true);
                    usedTables = BaseMarkupUtilities.ReverseString(usedTables);
                    bool isUseDSJoin = true;

                    #region check use or no
                    for (int index = 0; index < usedTables.Length; index++)
                    {
                        if (usedTables[index] == '1')
                        {
                            if (!paramInfo.SelectedTableIndexes.Contains(index))
                            {
                                isUseDSJoin = false;
                                break;
                            }
                        }
                    }
                    if (!isUseDSJoin)
                        continue;
                    #endregion

                    #region build where clause
                    if ("and".Equals(dsJoin.LogicType, StringComparison.OrdinalIgnoreCase) ||
                        "or".Equals(dsJoin.LogicType, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!hasRootJoin)
                        {
                            whereClause = dsJoin.LogicType + " " + dsJoin.Condition + " " + whereClause;
                            jWhereClause = dsJoin.LogicType + " " + dsJoin.JavaCondition + " " + jWhereClause;
                        }
                        else
                        {
                            whereClause = whereClause + " " + dsJoin.LogicType + " " + dsJoin.Condition;
                            jWhereClause = jWhereClause + " " + dsJoin.LogicType + " " + dsJoin.JavaCondition;
                        }
                        logicType = dsJoin.LogicType;
                    }
                    else
                    {
                        whereClause = dsJoin.Condition + " " + whereClause;
                        jWhereClause = dsJoin.JavaCondition + " " + jWhereClause;
                        hasRootJoin = true;
                    }
                    #endregion

                    #region mark used for gen from clause
                    if (paramInfo.DSWhereClauseRelationRow == null)
                        paramInfo.DSWhereClauseRelationRow = dsJoin.Tables.Clone();
                    else
                        paramInfo.DSWhereClauseRelationRow.OrRow(dsJoin.Tables);
                    #endregion
                }
                
                if (!hasRootJoin && !string.IsNullOrWhiteSpace(logicType))
                {
                    whereClause = whereClause.Substring(logicType.Length);
                    jWhereClause = jWhereClause.Substring(logicType.Length);
                }
                paramInfo.WhereClauses.Add(whereClause);
                paramInfo.JWhereClauses.Add(jWhereClause);
                if (paramInfo.DSWhereClauseRelationRow == null)
                {
                    DSRelationRow templateRow = paramInfo.DomainInfo.DSDomainData.WhereClause.DSJoins[0].Tables;
                    paramInfo.DSWhereClauseRelationRow = templateRow.Clone();
                    for(int index = 0; index < paramInfo.DSWhereClauseRelationRow.Data.Count; index++)
                        paramInfo.DSWhereClauseRelationRow.Data[index] = paramInfo.DSWhereClauseRelationRow.Data[index] & 0;
                    foreach (int tableIndex in paramInfo.SelectedTableIndexes)
                    {
                        int index = (tableIndex / 64) + (tableIndex % 64 != 0 ? 1 : 0);
                        index--;
                        if (index >= 0 && index < paramInfo.DSWhereClauseRelationRow.Data.Count)
                        {
                            int position = tableIndex % 64;
                            paramInfo.DSWhereClauseRelationRow.Data[index] =
                                paramInfo.DSWhereClauseRelationRow.TurnOnBit(paramInfo.DSWhereClauseRelationRow.Data[index], position);
                        }
                    }
                }
            }
        }
        #endregion

        #region split to more Path
        /// <summary>
        /// separate osql to multi paths
        /// </summary>
        /// <param name="fullItems"></param>
        /// <param name="dsDomain"></param>
        /// <returns></returns>
        protected List<List<InternalBookmarkItem>> GetPaths(ref GenOsqlParamInfo paramInfo,
            List<InternalBookmarkItem> fullItems, DSDomain dsDomain)
        {
            List<List<InternalBookmarkItem>> paths = new List<List<InternalBookmarkItem>>();

            #region calculate used table indexes
            Dictionary<int, List<InternalBookmarkItem>> usedTableIndexes = new Dictionary<int, List<InternalBookmarkItem>>();
            List<int> sortedTableIndexes = new List<int>();
            foreach (InternalBookmarkItem ibmItem in fullItems)
            {
                if (!usedTableIndexes.ContainsKey(ibmItem.TableIndex))
                {
                    usedTableIndexes.Add(ibmItem.TableIndex, new List<InternalBookmarkItem>());
                    sortedTableIndexes.Add(ibmItem.TableIndex);
                }

                usedTableIndexes[ibmItem.TableIndex].Add(ibmItem);
            }
            if (!usedTableIndexes.ContainsKey(0))
            {
                usedTableIndexes.Add(0, new List<InternalBookmarkItem>());
                sortedTableIndexes.Add(0);
            }
            sortedTableIndexes = sortedTableIndexes.OrderBy(c => c).ToList();
            #endregion

            #region get paths
            string whereClauseName = "WhereClause";
            Dictionary<int, bool> checker = new Dictionary<int, bool>();
            List<DSRelationRow> dsRelationRows = dsDomain.TableRelations.Rows.Items;
            DSRelationRow dsWhereRelationRow = dsRelationRows.FirstOrDefault(
                c => whereClauseName.Equals(c.Name, StringComparison.OrdinalIgnoreCase));
            string whereRelations = BitHelper.ToBinaryFormat(dsWhereRelationRow.Data, true);
            whereRelations = BaseMarkupUtilities.ReverseString(whereRelations);
            foreach (DSRelationRow dsRelationRow in dsRelationRows)
            {
                if (whereClauseName.Equals(dsRelationRow.Name, StringComparison.OrdinalIgnoreCase))
                {
                    paramInfo.DSWhereClauseRelationRow = dsRelationRow;
                    continue;
                }

                List<InternalBookmarkItem> path = new List<InternalBookmarkItem>();
                string relations = BitHelper.ToBinaryFormat(dsRelationRow.Data, true);
                relations = BaseMarkupUtilities.ReverseString(relations);
                bool hasTable = false;
                foreach (int tableIndex in sortedTableIndexes)
                {
                    int index = tableIndex;
                    if (relations.Length > index && relations[index] == '1')
                    {
                        path.AddRange(usedTableIndexes[tableIndex]);
                        if (!checker.ContainsKey(tableIndex))
                        {
                            hasTable = true;
                            checker.Add(tableIndex, true);
                        }
                    }
                }

                if (hasTable)
                {
                    int parentTableIndex = GetParentTableInexInJoinClause(dsWhereRelationRow, dsRelationRow);
                    if (!usedTableIndexes.ContainsKey(parentTableIndex))
                        usedTableIndexes.Add(parentTableIndex, new List<InternalBookmarkItem>());
                    InternalBookmarkItem ibmItem = usedTableIndexes[parentTableIndex].FirstOrDefault(
                        c => c.TableIndex == parentTableIndex && c.DSIconType == DSIconType.Field);
                    if (ibmItem == null)
                    {
                        DSTreeView firstField = paramInfo.DomainInfo.Fields.Values.FirstOrDefault(
                            c => c.TableIndex == parentTableIndex && c.Type == DSIconType.Field);
                        if (firstField != null)
                        {
                            ibmItem = new InternalBookmarkItem()
                            {
                                BizName = firstField.Text,
                                ItemType = "Field",
                                JavaName = firstField.JavaClause,
                                Key = Guid.NewGuid().ToString(),
                                OrderNo = 1,
                                TableIndex = parentTableIndex,
                                TechName = firstField.TechName,
                                Type = XsltType.Select
                            };

                            usedTableIndexes[parentTableIndex].Insert(0, ibmItem);
                            path.Insert(0, ibmItem);
                        }
                    }

                    paths.Add(path);
                }
            }
            #endregion

            return paths;
        }

        private int GetParentTableInexInJoinClause(DSRelationRow whereRelation, DSRelationRow tableRelation)
        {
            // or relation
            DSRelationRow relation = whereRelation;
            if (relation != null)
            {
                relation = relation.Clone();
                if (tableRelation != null) relation.OrRow(tableRelation);
            }

            // check index
            for (int i = 0; i < relation.Data.Count; i++)
            {
                long num = relation.Data[i];
                int index = i * 64;
                do
                {
                    int bit = (int)num % 2;
                    if (bit == 1)
                        return index;

                    index++;
                    num = num >> 1; //ShiftRight(num, 1);
                }
                while (num > 0);
            }

            return -1;
        }
        #endregion

        protected void UpdateRelationInfo(ref GenOsqlParamInfo paramInfo)
        {
            paramInfo.PutRootSelectedTable(paramInfo.DomainInfo.DSDomainData.Name);
            paramInfo.IbmDomain.SelectedTables = paramInfo.SelectedTables;
            paramInfo.IbmDomain.Relations = paramInfo.DomainInfo.Relations;
        }
    }
}
