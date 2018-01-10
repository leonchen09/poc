
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
    partial class GenOsqlSql08Helper : BaseGenOsqlHelper
    {
        public override void GenOsql(DomainInfo domain, InternalBookmark ibm, int domainIndex)
        {
            GenOsqlParamInfo paramInfo = new GenOsqlParamInfo();
            paramInfo.DomainInfo = domain;
            paramInfo.IbmDomain = ibm.GetInternalBookmarkDomain(domain.DSDomainData.Name);
            paramInfo.IndexOfDomain = domainIndex;
            GenOsql(ref paramInfo);

            MergeOsqlPaths(ref paramInfo);
            paramInfo.OsqlXmlItem.DatabaseType = 1;
            OsqlXmlItem = paramInfo.OsqlXmlItem;

            UpdateChecksumInfo(ref paramInfo, Sql08QueryConstants.DicSelectClauseParamName, PdxDbTypes.ipx_sqlserver);
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
            MergeSelectFormWhereClause(ref paramInfo, Sql08QueryConstants.DicSelectClauseParamName);

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
                    paramInfo.OsqlStringBuilder.AppendLine(Sql08QueryConstants.SelectClause);
                    paramInfo.DSRelationRow = null;
                    // add paramater for watermark at start of query for osql & jsql
                    paramInfo.OsqlStringBuilder.Osql.AppendLine(string.Format(Sql08QueryConstants.SQLSystemInfo,
                        ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark,
                        ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark) + Sql08QueryConstants.SQLComma);
                    paramInfo.OsqlStringBuilder.JOsql.AppendLine(string.Format(Sql08QueryConstants.JSQLSystemInfo,
                         ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark) + Sql08QueryConstants.SQLComma);
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
                            GenSelectClauseItem(type, sqlUSC, string.Empty,
                                BaseMarkupUtilities.GetOriginalBizName(item.Key, item.BizName), false, true, ref paramInfo);
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
            DSIconType type = bmItem.DSIconType;

            double i = Math.Log((double)bmItem.TableIndex, (double)2);
            DSOnClause table = paramInfo.DomainInfo.DSDomainData.OnClauses.Items[bmItem.TableIndex];
            string tableName = table.Alias;
            paramInfo.PutSelectedTable(tableName, false);
            paramInfo.SelectedTableIndexes.Add(item.TableIndex);

            InternalBookmarkItem existedItem = paramInfo.DataTagFields.FirstOrDefault(a => a.TableIndex == item.TableIndex && a.DSIconType == DSIconType.Field);
            bool isImageTrunc = bmItem.IsTruncImage();
            if (isImageTrunc && existedItem != null && existedItem.BizName == bmItem.BizName && !paramInfo.TablesHasField.Contains(item.TableIndex))
            { // this table has only image that is chunked then we need to add one default column
                GenSelectClauseItem(DSIconType.Field, item.TechName,
                    string.Empty, Guid.NewGuid().ToString(), false, true, ref paramInfo);
                paramInfo.TablesHasField.Add(item.TableIndex);
                paramInfo.HasImage = true;
            }
            GenSelectClauseItem(type, bmItem.TechName, item.JavaClause, itemName, isImageTrunc, true, ref paramInfo);

            if (!string.IsNullOrEmpty(item.RenderXYOrder))
            {
                List<string> renderParam = item.RenderXYOrder.Split(';').ToList<string>();
                for (int j = 0; j < renderParam.Count; j++)
                {
                    AddItemToListJParameters((renderParam[j] == "1") ? FrameworkConstants.RenderArgumentX : FrameworkConstants.RenderArgumentY,
                        ref paramInfo, Sql08QueryConstants.DicSelectClauseParamName);
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
        #endregion

        #region gen select clause item
        private void GenSelectClauseItem(DSIconType type, string techName, string javaName, string name, bool isImage,
            bool isField, ref GenOsqlParamInfo paramInfo)
        {
            name = SqlHelper.NormalizeOperandName(name);
            switch (type)
            {
                case DSIconType.UDF:
                    {
                        paramInfo.OsqlStringBuilder.Osql.AppendLine(string.Format(Sql08QueryConstants.SQLUDFFormat, techName, name) + Sql08QueryConstants.SQLComma);
                        paramInfo.OsqlStringBuilder.JOsql.AppendLine(string.Format(Sql08QueryConstants.SQLUDFFormat, javaName, name) + Sql08QueryConstants.SQLComma);
                    }
                    break;
                case DSIconType.Field:
                    {
                        string select = string.Format(Sql08QueryConstants.SQLFieldFormat,
                                                      isImage ? string.Format(Sql08QueryConstants.UdfTruncImage, techName) : techName, name)
                                                      + Sql08QueryConstants.SQLComma;
                        paramInfo.OsqlStringBuilder.Osql.AppendLine(select);
                        select = string.Format(Sql08QueryConstants.SQLFieldFormat,
                                                      isImage ? string.Format(Sql08QueryConstants.UdfTruncImage, javaName) : javaName, name)
                                                      + Sql08QueryConstants.SQLComma;
                        paramInfo.OsqlStringBuilder.JOsql.AppendLine(select);
                    }
                    break;
                case DSIconType.Condition:
                case DSIconType.USC:
                    if (!string.IsNullOrEmpty(techName))
                    {
                        string select = string.Format(Sql08QueryConstants.SQLConditionFormat, techName, name) + Sql08QueryConstants.SQLComma;
                        paramInfo.OsqlStringBuilder.AppendLine(select);
                    }
                    break;
                case DSIconType.SystemInfo:
                case DSIconType.RenderXY:
                    if (string.Compare(techName, ProntoDoc.Framework.CoreObject.FrameworkConstants.PluginSystemInfo.GeneratedTime) == 0)
                    {
                        string select = string.Format(Sql08QueryConstants.SQLGeneratedTime, name) + Sql08QueryConstants.SQLComma;
                        paramInfo.OsqlStringBuilder.AppendLine(select);
                    }
                    else
                    {
                        AddItemToListJParameters(techName, ref paramInfo, Sql08QueryConstants.DicSelectClauseParamName);
                        string norTechName = techName.Replace(" ", "");
                        string select;
                        string jSelect;
                        if (!paramInfo.IsNeedInKeyword)
                        {
                            select = string.Format(Sql08QueryConstants.SQLSystemInfo, norTechName, name) + Sql08QueryConstants.SQLComma;
                            jSelect = string.Format(Sql08QueryConstants.JSQLSystemInfo, name) + Sql08QueryConstants.SQLComma;
                        }
                        else
                        {
                            select = string.Format(Sql08QueryConstants.SQLSystemInfoWithIn, norTechName, name) + Sql08QueryConstants.SQLComma;
                            jSelect = string.Format(Sql08QueryConstants.JSQLSystemInfoWithIn, name) + Sql08QueryConstants.SQLComma;
                        }

                        paramInfo.OsqlStringBuilder.Osql.AppendLine(select);
                        paramInfo.OsqlStringBuilder.JOsql.AppendLine(jSelect);
                    }
                    AddItemToSystemParams(techName, ref paramInfo);

                    break;
            }
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
                paramInfo.DSWhereClauseRelationRow, paramInfo.DSRelationRow));
        }
        #endregion
        #endregion

        #region gen from clause
        //private void GenFromClause(ref GenOsqlParamInfo paramInfo)
        //{
        //    paramInfo.FormClause = paramInfo.DomainInfo.DSDomainData.GetFromClause(paramInfo.DSRelationRow);
        //}
        #endregion

        #region gen for xml clause
        private void GenForXmlClause(ref GenOsqlParamInfo paramInfo)
        {
            //For XML Auto
            string forXml = string.Empty;
            string domainEncoded = BaseMarkupUtilities.XmlEncode(paramInfo.IbmDomain.DomainName);
            if (paramInfo.HasField)
            {
                if (!paramInfo.IsNeedInKeyword)
                    forXml = string.Format(Sql08QueryConstants.XmlAuto.ForXMLAuto, domainEncoded);
                else
                    forXml = string.Format(Sql08QueryConstants.XmlAuto.ForXMLAutoIn, domainEncoded);
            }
            else
            {
                string rootTable = paramInfo.SelectedTables[0][0];
                if (!paramInfo.IsNeedInKeyword)
                    forXml = string.Format(Sql08QueryConstants.XmlAuto.ForXMLAutoIn, rootTable, domainEncoded);
                else
                    forXml = string.Format(Sql08QueryConstants.XmlAuto.ForXMLRawIn, rootTable, domainEncoded);
            }

            paramInfo.OsqlXmlItem = new OsqlXmlItem();
            paramInfo.OsqlXmlItem.Paths = new List<OsqlXmlItemPath>();
            foreach (OsqlStringBuilder item in paramInfo.OsqlStringBuilders)
            {
                item.AppendLine(forXml);

                //Prepare for return
                if (paramInfo.HasImage)
                    item.Append(Sql08QueryConstants.SQLBase64);

                OsqlXmlItemPath osqlXmlItemPath = new OsqlXmlItemPath();
                osqlXmlItemPath.Osql = item.Osql.ToString().TrimEnd();
                osqlXmlItemPath.JSql = item.JOsql.ToString().TrimEnd();

                if (paramInfo.IsNeedInKeyword)
                {
                    osqlXmlItemPath.Osql = string.Format(Sql08QueryConstants.SQLWithIn, osqlXmlItemPath.Osql);
                    osqlXmlItemPath.JSql = string.Format(Sql08QueryConstants.SQLWithIn, osqlXmlItemPath.JSql);
                }
                paramInfo.OsqlXmlItem.Paths.Add(osqlXmlItemPath);
            }
            paramInfo.OsqlXmlItem.Index = paramInfo.IndexOfDomain;
            paramInfo.OsqlXmlItem.DomainName = paramInfo.DomainInfo.DSDomainData.Name;
            paramInfo.OsqlXmlItem.EncodeDomainName = BaseMarkupUtilities.XmlEncode(paramInfo.DomainInfo.DSDomainData.Name);
        }
        #endregion

        #region merge all osql paths into one
        /// <summary>
        /// using for merge all osql paths into one to post processing get data
        /// </summary>
        /// <param name="paramInfo"></param>
        private void MergeOsqlPaths(ref GenOsqlParamInfo paramInfo)
        {
            System.Text.StringBuilder osqlDeclareClause = new System.Text.StringBuilder();
            System.Text.StringBuilder osqlSetClause = new System.Text.StringBuilder();
            System.Text.StringBuilder osqlSelectClause = new System.Text.StringBuilder();

            System.Text.StringBuilder jsqlDeclareClause = new System.Text.StringBuilder();
            System.Text.StringBuilder jsqlSetClause = new System.Text.StringBuilder();
            System.Text.StringBuilder jsqlSelectClause = new System.Text.StringBuilder();

            osqlSelectClause.Append("Select ");
            jsqlSelectClause.Append("Select ");
            if (paramInfo.OsqlXmlItem.Paths != null)
            {
                int index = 1;
                foreach (OsqlXmlItemPath path in paramInfo.OsqlXmlItem.Paths)
                {
                    string osql = path.Osql;
                    string jsql = path.Osql;
                    string variant = string.Format("@pdw_xml_var{0}", index);

                    osqlDeclareClause.AppendLine(string.Format("DECLARE {0} XML;", variant));
                    osqlSetClause.AppendLine(string.Format("SET {0} = ({1});", variant, osql));
                    osqlSelectClause.Append(string.Format("{0},", variant));

                    jsqlDeclareClause.AppendLine(string.Format("DECLARE {0} XML;", variant));
                    jsqlSetClause.AppendLine(string.Format("SET {0} = ({1});", variant, jsql));
                    jsqlSelectClause.Append(string.Format("{0},", variant));
                    index++;
                }

                if (osqlSelectClause.Length > 0)
                    osqlSelectClause = osqlSelectClause.Remove(osqlSelectClause.Length - 1, 1);
                if (jsqlSelectClause.Length > 0)
                    jsqlSelectClause = jsqlSelectClause.Remove(jsqlSelectClause.Length - 1, 1);
                paramInfo.OsqlXmlItem.Osql = osqlDeclareClause.ToString() + osqlSetClause.ToString() + osqlSelectClause.ToString();
                paramInfo.OsqlXmlItem.Jsql = jsqlDeclareClause.ToString() + jsqlSetClause.ToString() + jsqlSelectClause.ToString();
            }
        }
        #endregion
    }
}
