
using System;
using System.Linq;
using System.Collections.Generic;

using Pdw.Core;

using ProntoDoc.Framework.Utils;
using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.Expression;
using ProntoDoc.Framework.CoreObject.DataSegment;
using ProntoDoc.Framework.CoreObject.PdwxObjects;

namespace Pdw.Services.Template
{
    partial class GenOsqlOracleHelper : BaseGenOsqlHelper
    {
        public override void GenOsql(DomainInfo domain, InternalBookmark ibm, int domainIndex)
        {
            GenOsqlParamInfo paramInfo = new GenOsqlParamInfo();
            paramInfo.DomainInfo = domain;
            paramInfo.IbmDomain = ibm.GetInternalBookmarkDomain(domain.DSDomainData.Name);
            paramInfo.IndexOfDomain = domainIndex;
            GenOsql(ref paramInfo);

            paramInfo.OsqlXmlItem.DatabaseType = 2;
            OsqlXmlItem = paramInfo.OsqlXmlItem;

            UpdateChecksumInfo(ref paramInfo, OracleQueryConstants.DicSelectClauseParamName, PdxDbTypes.ipx_oracle);
        }

        private void GenOsql(ref GenOsqlParamInfo paramInfo)
        {
            paramInfo.CheckIsNeedInKeyword();

            // gen select clause
            GenSelectClause(ref paramInfo);

            // gen from clause
            // GenFromClause(ref paramInfo); // ngocbv: move to the end of GenSelectClause

            // gen where clause
            // GenWhereClause(ref paramInfo); // ngocbv: move to the end of GenSelectClause

            // merge select + from + where clause
            MergeSelectFormWhereClause(ref paramInfo, OracleQueryConstants.DicSelectClauseParamName);

            // gen for xml clause
            GenForXmlClause(ref paramInfo);
        }

        #region gen select clause
        private void GenSelectClause(ref GenOsqlParamInfo paramInfo)
        {
            if (paramInfo == null)
                paramInfo = new GenOsqlParamInfo();
            if (paramInfo.DomainInfo == null || paramInfo.IbmDomain == null)
            {
                paramInfo.OsqlXmlItem = new OsqlXmlItem();
                return;
            }

            paramInfo.RenderXY = 0;
            paramInfo.DSRelationRow = null;
            paramInfo.JParameters = new List<JParameter>();
            paramInfo.TablesHasField = new List<int>();

            AnalyzeDataTag(ref paramInfo);

            GenSelectClauseForFields(ref paramInfo);

            UpdateRelationInfo(ref paramInfo);
        }

        #region gen select clause for fields
        private void GenSelectClauseForFields(ref GenOsqlParamInfo paramInfo)
        {
            paramInfo.HasImage = false;
            paramInfo.HasField = false;
            if (paramInfo.DataTagFields != null && paramInfo.DataTagFields.Count > 0)
            {
                paramInfo.OrderDataTagFieldsByTableIndex();
                List<List<InternalBookmarkItem>> paths = GetPaths(ref paramInfo, paramInfo.DataTagFields, paramInfo.DomainInfo.DSDomainData);
                foreach (List<InternalBookmarkItem> path in paths)
                {
                    if (path.Count <= 0)
                        continue;

                    paramInfo.OsqlStringBuilder = new OsqlStringBuilder();
                    paramInfo.OsqlStringBuilder.AppendLine(OracleQueryConstants.SelectClause);
                    paramInfo.DSRelationRow = null;
                    // add paramater for watermark at start of query for osql & jsql
                    paramInfo.OsqlStringBuilder.Osql.AppendLine(string.Format(OracleQueryConstants.SQLSystemInfo,
                        ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark,
                        ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark) + OracleQueryConstants.SQLComma);
                    paramInfo.OsqlStringBuilder.JOsql.AppendLine(string.Format(OracleQueryConstants.JSQLSystemInfo,
                         ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark) + OracleQueryConstants.SQLComma);
                    paramInfo.OsqlStringBuilder.SelectedColumns = string.Empty;
                    paramInfo.CurrentTableName = string.Empty;
                    paramInfo.PutSelectedTable(string.Empty, true);
                    paramInfo.SelectedTableIndexes = new List<int>();
                    foreach (InternalBookmarkItem item in path)
                    {
                        DSIconType type = item.DSIconType;
                        if (type == DSIconType.Field)
                            paramInfo.HasField = true;

                        if (type == DSIconType.USC)
                        {
                            string sqlUSC = GenSelectClauseForUSC(item, ref paramInfo);
                            string itemBizName = BaseMarkupUtilities.GetOriginalBizName(item.Key, item.BizName);
                            GenSelectClauseItem(type, sqlUSC, string.Empty, itemBizName, false, true, ref paramInfo);

                            string tableName = paramInfo.SelectedTables[0][0];
                            UpdateSelectedColumns(ref paramInfo, tableName, itemBizName);
                        }
                        else
                        {
                            if (item.Relation != null)
                            {
                                if (paramInfo.DSRelationRow == null)
                                    paramInfo.DSRelationRow = item.Relation.Clone();
                                else
                                    paramInfo.DSRelationRow.OrRow(item.Relation);
                            }

                            GenSelectClauseForField(item, ref paramInfo);
                        }

                        if (item != null && item.DataType != null &&
                            (item.DataType.Name == SQLTypeName.BINARY || item.DataType.Name == SQLTypeName.VARBINARY))
                            paramInfo.HasImage = true;
                    }
                    UpdateOsqlXmlItem(ref paramInfo);
                }
            }
        }

        private void GenSelectClauseForField(InternalBookmarkItem bmItem, ref GenOsqlParamInfo paramInfo)
        {
            string itemName = BaseMarkupUtilities.GetOriginalBizName(bmItem.Key, bmItem.BizName);
            DSTreeView item = paramInfo.DomainInfo.GetField(itemName);
            DSIconType type = (DSIconType)Enum.Parse(typeof(DSIconType), bmItem.ItemType);

            double i = Math.Log((double)bmItem.TableIndex, (double)2);
            DSOnClause table = paramInfo.DomainInfo.DSDomainData.OnClauses.Items[bmItem.TableIndex];
            string tableName = table.Alias;
            paramInfo.PutSelectedTable(tableName, false);
            paramInfo.SelectedTableIndexes.Add(item.TableIndex);

            UpdateSelectedColumns(ref paramInfo, tableName, itemName);
            InternalBookmarkItem existedItem = paramInfo.DataTagFields.FirstOrDefault(a => a.TableIndex == item.TableIndex && a.DSIconType == DSIconType.Field);
            bool isImageTrunc = bmItem.IsTruncImage();
            if (isImageTrunc && existedItem != null && existedItem.BizName == bmItem.BizName && !paramInfo.TablesHasField.Contains(item.TableIndex))
            { // this table has only image that is chunked then we need to add one default column
                GenSelectClauseItem(DSIconType.Field, item.TechName,
                    string.Empty, Guid.NewGuid().ToString(), false, true, ref paramInfo);
                paramInfo.TablesHasField.Add(item.TableIndex);
                paramInfo.HasImage = true;
            }
            GenSelectClauseItem(type, bmItem.TechName, item.JavaClause, itemName, bmItem.IsTruncImage(), true, ref paramInfo);

            if (!string.IsNullOrEmpty(item.RenderXYOrder))
            {
                List<string> renderParam = item.RenderXYOrder.Split(';').ToList<string>();
                for (int j = 0; j < renderParam.Count; j++)
                {
                    AddItemToListJParameters((renderParam[j] == "1") ? FrameworkConstants.RenderArgumentX : FrameworkConstants.RenderArgumentY, 
                        ref paramInfo, OracleQueryConstants.DicSelectClauseParamName);
                }
            }

            #region Build Extra field
            if (existedItem == null && !string.IsNullOrEmpty(item.OrginalField) && !paramInfo.TablesHasField.Contains(item.TableIndex))
            {
                GenSelectClauseItem(DSIconType.Field, item.OrginalField, string.Empty, Guid.NewGuid().ToString(), false, true, ref paramInfo);
                paramInfo.TablesHasField.Add(item.TableIndex);
                return;
            }
            #endregion
        }

        private void UpdateSelectedColumns(ref GenOsqlParamInfo paramInfo, string tableName, string bizName)
        {
            if (paramInfo.CurrentTableName != tableName)
            {
                paramInfo.OsqlStringBuilder.SelectedColumns = paramInfo.OsqlStringBuilder.SelectedColumns + string.Format(",[{0}]", tableName);
                paramInfo.CurrentTableName = tableName;
            }
            paramInfo.OsqlStringBuilder.SelectedColumns = paramInfo.OsqlStringBuilder.SelectedColumns + string.Format(",{0}", bizName);
        }
        #endregion

        #region gen select clause item
        private void GenSelectClauseItem(DSIconType type, string techName, string javaName, string name, bool isImage,
            bool isField, ref GenOsqlParamInfo paramInfo)
        {
            name = NormalizeBizName(name);
            switch (type)
            {
                case DSIconType.UDF:
                    {
                        paramInfo.OsqlStringBuilder.Osql.AppendLine(string.Format(OracleQueryConstants.SQLUDFFormat, techName, name) + OracleQueryConstants.SQLComma);
                        paramInfo.OsqlStringBuilder.JOsql.AppendLine(string.Format(OracleQueryConstants.SQLUDFFormat, javaName, name) + OracleQueryConstants.SQLComma);
                    }
                    break;
                case DSIconType.Field:
                    {
                        string select = string.Format(OracleQueryConstants.SQLFieldFormat,
                                                      isImage ? string.Format(OracleQueryConstants.UdfTruncImage, techName) : techName, name)
                                                      + OracleQueryConstants.SQLComma;
                        paramInfo.OsqlStringBuilder.Osql.AppendLine(select);
                        select = string.Format(OracleQueryConstants.SQLFieldFormat,
                                                      isImage ? string.Format(OracleQueryConstants.UdfTruncImage, javaName) : javaName, name)
                                                      + OracleQueryConstants.SQLComma;
                        paramInfo.OsqlStringBuilder.JOsql.AppendLine(select);
                    }
                    break;
                case DSIconType.Condition:
                case DSIconType.USC:
                    if (!string.IsNullOrEmpty(techName))
                    {
                        string select = string.Format(OracleQueryConstants.SQLConditionFormat, techName, name) + OracleQueryConstants.SQLComma;
                        paramInfo.OsqlStringBuilder.AppendLine(select);
                    }
                    break;
                case DSIconType.SystemInfo:
                case DSIconType.RenderXY:
                    if (string.Compare(techName, ProntoDoc.Framework.CoreObject.FrameworkConstants.PluginSystemInfo.GeneratedTime) == 0)
                    {
                        string select = string.Format(OracleQueryConstants.SQLGeneratedTime, name) + OracleQueryConstants.SQLComma;
                        paramInfo.OsqlStringBuilder.AppendLine(select);
                    }
                    else
                    {
                        AddItemToListJParameters(techName, ref paramInfo, OracleQueryConstants.DicSelectClauseParamName);
                        string norTechName = techName.Replace(" ", "");
                        string select;
                        string jSelect;
                        if (!paramInfo.IsNeedInKeyword)
                        {
                            select = string.Format(OracleQueryConstants.SQLSystemInfo, norTechName, name) + OracleQueryConstants.SQLComma;
                            jSelect = string.Format(OracleQueryConstants.JSQLSystemInfo, name) + OracleQueryConstants.SQLComma;
                        }
                        else
                        {
                            select = string.Format(OracleQueryConstants.SQLSystemInfoWithIn, norTechName, name) + OracleQueryConstants.SQLComma;
                            jSelect = string.Format(OracleQueryConstants.JSQLSystemInfoWithIn, name) + OracleQueryConstants.SQLComma;
                        }

                        paramInfo.OsqlStringBuilder.Osql.AppendLine(select);
                        paramInfo.OsqlStringBuilder.JOsql.AppendLine(jSelect);
                    }
                    AddItemToSystemParams(techName, ref paramInfo);

                    break;
            }
        }

        private string NormalizeBizName(string bizName)
        {
            return string.Format("\"{0}\"", bizName.Replace("\"", "''"));
        }
        #endregion

        #region merge select fields with select others
        private void UpdateOsqlXmlItem(ref GenOsqlParamInfo paramInfo)
        {
            if (paramInfo.OsqlStringBuilders == null)
                paramInfo.OsqlStringBuilders = new List<OsqlStringBuilder>();
            if (paramInfo.FromClauses == null)
                paramInfo.FromClauses = new List<string>();

            // remove latest comma
            bool isRemove = false;
            int index = paramInfo.OsqlStringBuilder.Osql.Length - 1;
            while (!isRemove && index >= 0)
            {
                char ch = paramInfo.OsqlStringBuilder.Osql[index];
                if (ch != '\n' && ch != '\r' && ch != ' ' && ch != ',')
                    break;
                if (ch == ',')
                {
                    isRemove = true;
                    paramInfo.OsqlStringBuilder.Osql[index] = ' ';
                }
                index--;
            }
            isRemove = false;
            index = paramInfo.OsqlStringBuilder.JOsql.Length - 1;
            while (!isRemove && index >= 0)
            {
                char ch = paramInfo.OsqlStringBuilder.JOsql[index];
                if (ch != '\n' && ch != '\r' && ch != ' ' && ch != ',')
                    break;
                if (ch == ',')
                {
                    isRemove = true;
                    paramInfo.OsqlStringBuilder.JOsql[index] = ' ';
                }
                index--;
            }
            paramInfo.OsqlStringBuilders.Add(paramInfo.OsqlStringBuilder);

            GenWhereClause(ref paramInfo);
            //if (paramInfo.DSWhereClauseRelationRow != null)
            //{
            //    if (paramInfo.DSRelationRow == null)
            //        paramInfo.DSRelationRow = paramInfo.DSWhereClauseRelationRow.Clone();
            //    else
            //        paramInfo.DSRelationRow.OrRow(paramInfo.DSWhereClauseRelationRow);
            //}
            paramInfo.FromClauses.Add(paramInfo.DomainInfo.DSDomainData.GetFromClause(
                paramInfo.DSWhereClauseRelationRow, paramInfo.DSRelationRow, AppDbTypes.Oracle));
        }
        #endregion
        #endregion

        #region gen from clause
        //private void GenFromClause(ref GenOsqlParamInfo paramInfo)
        //{
        //    paramInfo.FormClause = paramInfo.DomainInfo.DSDomainData.GetFromClause(paramInfo.DSRelationRow, AppDbTypes.Oracle);
        //}
        #endregion

        #region gen for xml clause
        private void GenForXmlClause(ref GenOsqlParamInfo paramInfo)
        {
            paramInfo.OsqlXmlItem = new OsqlXmlItem();
            paramInfo.OsqlXmlItem.Paths = new List<OsqlXmlItemPath>();
            foreach (OsqlStringBuilder item in paramInfo.OsqlStringBuilders)
            {
                OsqlXmlItemPath osqlXmlItemPath = new OsqlXmlItemPath();
                osqlXmlItemPath.Osql = item.Osql.ToString().TrimEnd();
                osqlXmlItemPath.JSql = item.JOsql.ToString().TrimEnd();
                if (!string.IsNullOrWhiteSpace(item.SelectedColumns))
                {
                    if (item.SelectedColumns.StartsWith(","))
                        osqlXmlItemPath.SelectedColumns = item.SelectedColumns.Remove(0, 1);
                    else
                        osqlXmlItemPath.SelectedColumns = item.SelectedColumns;
                }

                paramInfo.OsqlXmlItem.Paths.Add(osqlXmlItemPath);
            }
            paramInfo.OsqlXmlItem.Index = paramInfo.IndexOfDomain;
            paramInfo.OsqlXmlItem.DomainName = paramInfo.DomainInfo.DSDomainData.Name;
            paramInfo.OsqlXmlItem.EncodeDomainName = BaseMarkupUtilities.XmlEncode(paramInfo.DomainInfo.DSDomainData.Name);
        }
        #endregion
    }
}
