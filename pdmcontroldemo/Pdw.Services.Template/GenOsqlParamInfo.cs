
using System.Linq;
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.CoreObject.DataSegment;
using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdw.Core;

namespace Pdw.Services.Template
{
    class GenOsqlParamInfo
    {
        public List<InternalBookmarkItem> DataTagFields { get; private set; }
        public Dictionary<string, USCItem> UscItems { get; private set; }
        public DomainInfo DomainInfo { get; set; }
        public int IndexOfDomain { get; set; }
        public InternalBookmarkDomain IbmDomain { get; set; }
        public bool IsNeedInKeyword { get; private set; }
        public DSRelationRow DSRelationRow { get; set; }
        public DSRelationRow DSWhereClauseRelationRow { get; set; }
        public List<JParameter> JParameters { get; set; }
        public List<List<string>> SelectedTables { get; private set; }
        public List<int> SelectedTableIndexes { get; set; }
        public List<int> TablesHasField { get; set; }
        public bool HasField { get; set; }
        public bool HasImage { get; set; }
        public int RenderXY { get; set; }
        public OsqlStringBuilder OsqlStringBuilder { get; set; }
        public OsqlXmlItem OsqlXmlItem { get; set; }
        public List<OsqlStringBuilder> OsqlStringBuilders { get; set; }
        public List<string> FromClauses { get; set; }
        public List<string> WhereClauses { get; set; }
        public List<string> JWhereClauses { get; set; }
        public int MinTableIndex { get; set; }
        public SystemParameter SysParameter { get; set; }

        public string CurrentTableName { get; set; }
        public DataAgrument DomainArgument { get; set; }

        public GenOsqlParamInfo()
        {
            DataTagFields = new List<InternalBookmarkItem>();
            UscItems = new Dictionary<string, USCItem>();
            IsNeedInKeyword = false;
            JParameters = new List<JParameter>();
            SelectedTables = new List<List<string>>();
            TablesHasField = new List<int>();
            HasField = false;
            HasImage = false;
            RenderXY = 0;
            SysParameter = new SystemParameter();
            SysParameter.HasComputerName = false;
            SysParameter.HasGeneratedTime = false;
            SysParameter.HasRenderRequestID = false;
            SysParameter.HasTemplateId = false;
            SysParameter.HasTemplateName = false;
            SysParameter.HasTemplateVersion = false;
            SysParameter.HasUserId = false;
        }

        public List<InternalBookmarkItem> GetDataTagFieldsByTableIndex(int tableIndex)
        {
            List<InternalBookmarkItem> items = new List<InternalBookmarkItem>();

            if (DataTagFields != null)
            {
                foreach (InternalBookmarkItem item in DataTagFields)
                {
                    bool isAdd = (item.TableIndex == tableIndex) || (tableIndex == 0 && item.TableIndex < 0);
                    if (isAdd)
                        items.Add(item);
                }
            }

            return items;
        }

        public void CheckIsNeedInKeyword()
        {
            IsNeedInKeyword = false;

            foreach (DSArgument arg in DomainInfo.DSDomainData.WhereClause.Args.Items)
            {
                if (arg.DataType.Name == SQLTypeName.ARRAY)
                {
                    IsNeedInKeyword = true;
                    return;
                }
            }
        }

        public void UpdateUSCItemsDictionary()
        {
            UscItems = new Dictionary<string, USCItem>();

            foreach (USCItem item in IbmDomain.USCItems)
            {
                if (!UscItems.ContainsKey(item.BusinessName))
                    UscItems.Add(item.BusinessName, item);
            }
        }

        public void AddDataTags(InternalBookmarkItem ibmItem, bool isUsc)
        {
            if (ibmItem == null)
                return;
            if (DataTagFields == null)
                DataTagFields = new List<InternalBookmarkItem>();

            ibmItem.OrderNo = isUsc ? 0 : 1;
            DataTagFields.Add(ibmItem);
        }

        public void PutSelectedTable(string tableAlias, bool createNewPath)
        {
            if (SelectedTables == null)
                SelectedTables = new List<List<string>>();

            if (SelectedTables.Count == 0)
            {
                List<string> path = new List<string>();
                if (!string.IsNullOrWhiteSpace(tableAlias))
                    path.Add(tableAlias);
                SelectedTables.Add(path);
            }
            else
            {
                if (createNewPath)
                {
                    List<string> path = new List<string>();
                    if (!string.IsNullOrWhiteSpace(tableAlias))
                        path.Add(tableAlias);
                    SelectedTables.Add(path);
                }
                else
                {
                    int index = SelectedTables.Count - 1;
                    List<string> path = SelectedTables[index];
                    if (path == null)
                    {
                        path = new List<string>();
                        if (!string.IsNullOrWhiteSpace(tableAlias))
                            path.Add(tableAlias);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(tableAlias) && !BaseMarkupUtilities.IsExistOnList(path, tableAlias))
                            // path.Insert(0, tableAlias);
                            path.Add(tableAlias);
                    }
                    SelectedTables[index] = path;
                }
            }
        }

        public void PutRootSelectedTable(string tableAlias)
        {
            if (SelectedTables == null)
                SelectedTables = new List<List<string>>();

            for (int index = 0; index < SelectedTables.Count; index++ )
            {
                List<string> path = SelectedTables[index];
                if (path == null)
                {
                    path = new List<string>();
                    path.Add(tableAlias);
                }
                else
                {
                    if (!BaseMarkupUtilities.IsExistOnList(path, tableAlias))
                        path.Insert(0, tableAlias);
                }
                SelectedTables[index] = path;
            }
        }

        public void OrderDataTagFieldsByTableIndex()
        {
            if (DataTagFields == null || DataTagFields.Count < 1)
                return;
            DataTagFields = (from dtf in DataTagFields orderby dtf.TableIndex ascending, dtf.OrderNo descending select dtf).ToList();
        }

        public enum AddType
        {
            JOSql,
            Osql,
            All,
        }
    }
}
