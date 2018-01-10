
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject;
using ProntoDoc.Framework.CoreObject.DataSegment;

namespace Pdw.Core
{
    public class DomainInfo
    {
        //DomainName
        public string BusinessName { get; set; }

        // dsdomain data        
        public DSDomain DSDomainData { get; set; }

        #region methods
        /// <summary>
        /// Binding all fields from TreeView to Dictionary. This is used for validating expression.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, USCItem> _fieldLists;
        public Dictionary<string, USCItem> FieldLists
        {
            get
            {
                if (_fieldLists == null)
                {
                    _fieldLists = new Dictionary<string, USCItem>();

                    if (DSDomainData != null && DSDomainData.MetaData != null && DSDomainData.MetaData.Items != null)
                    {
                        foreach (DSTreeView field in DSDomainData.MetaData.Items)
                        {
                            if (!string.IsNullOrWhiteSpace(field.Text) && !_fieldLists.ContainsKey(field.Text))
                            {
                                USCItem uscItem = new USCItem(field.Text, field.TechName, field.DataType, (DSIconType)field.Type);

                                _fieldLists.Add(field.Text.ToLower(), uscItem);
                            }
                        }
                    }
                }
                return _fieldLists;
            }
        }

        /// <summary>
        /// Get all fields information of DsDomain
        /// </summary>
        private Dictionary<string, DSTreeView> _fields;
        public Dictionary<string, DSTreeView> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = new Dictionary<string, DSTreeView>();
                    if (DSDomainData != null && DSDomainData.MetaData != null && DSDomainData.MetaData.Items != null)
                    {
                        foreach (DSTreeView field in DSDomainData.MetaData.Items)
                        {
                            if (!string.IsNullOrWhiteSpace(field.Text) && !_fields.ContainsKey(field.Text))
                            {
                                DSTreeView trvItem = field.Clone();
                                // move expression from technicalname to uniquename for condition and udf
                                if (trvItem.Type == DSIconType.Condition || trvItem.Type == DSIconType.UDF)
                                    trvItem.UniqueName = trvItem.TechName;

                                _fields.Add(trvItem.Text, trvItem);
                            }
                        }
                    }
                }

                return _fields;
            }
        }

        /// <summary>
        /// Get relation of DsDomain
        /// </summary>
        private Relations _relations;
        public Relations Relations
        {
            get
            {
                if (_relations == null)
                {
                    _relations = BindRelations();

                }
                return _relations;
            }
        }

        private Relations BindRelations()
        {
            Relations relation = new Relations();
            if (DSDomainData != null)
            {
                //Binding tables
                if (DSDomainData.OnClauses == null || DSDomainData.OnClauses.Items.Count <= 0 ||
                    DSDomainData.TableRelations.Rows.Items.Count <= 0)
                    return null;

                foreach (DSOnClause table in DSDomainData.OnClauses.Items)
                {
                    relation.TableNames.Add(table.Alias);
                }

                //Binding Metrix
                foreach (DSRelationRow row in DSDomainData.TableRelations.Rows.Items)
                    relation.Matrix.Add(row.Name, row.Data);


                //Binding MappInfo
                DSDomainData.MetaData.Items.RemoveAt(0);
                foreach (DSTreeView field in DSDomainData.MetaData.Items)
                {
                    Mapping map = new Mapping();
                    map.BizName = field.Text;

                    if (field.Type == DSIconType.Field || field.Type == DSIconType.Condition || field.Type == DSIconType.UDF)
                    {
                        int itemIndex = System.Math.Max(field.TableIndex, 0);
                        DSOnClause table = DSDomainData.OnClauses.Items[itemIndex];
                        map.TableName = table.Alias;
                    }

                    if (field.Type == DSIconType.SystemInfo || field.Type == DSIconType.RenderXY)
                        if (relation != null)
                            map.TableName = relation.TableNames[0];

                    if (relation != null && !relation.MapInfos.ContainsKey(field.Text))
                        relation.MapInfos.Add(field.Text, map);
                }

            }

            return relation;
        }

        public DSTreeView GetField(string bizName)
        {
            if(Fields != null && bizName != null && Fields.ContainsKey(bizName))
                return Fields[bizName];

            return null;
        }

        public string GetUdfSortedBizName(string udfBizName)
        {
            try
            {
                DSTreeView item = GetField(udfBizName);

                if (item != null && item.Type == DSIconType.UDF)
                {
                    string sortedBizName = item.OrderByDate;
                    return string.IsNullOrEmpty(sortedBizName) ? udfBizName : sortedBizName;
                }
            }
            catch { }

            return udfBizName;
        }

        /// <summary>
        /// Get Domain Data Argument.
        /// </summary>
        public DataAgrument DomainArgument
        {
            get
            {
                return BindingDataArgs(); ;
            }
        }
        private DataAgrument BindingDataArgs()
        {
            DataAgrument dataAgr = new DataAgrument();

            foreach (DSArgument dsArg in DSDomainData.WhereClause.Args.Items)
            {
                // 1. Get technical name
                string techName = GetTechName(dsArg, DSDomainData.WhereClause.Fields.Items);

                // 2. Build paramater object
                Parameter paramater = new Parameter();
                paramater.BizName = dsArg.Text;
                paramater.DataType = dsArg.DataType;
                paramater.IsShow = dsArg.IsVisibleOnPlugin;
                paramater.ParamName = dsArg.Parameter;
                paramater.TechName = techName;

                dataAgr.Parameters.Add(paramater);
            }

            foreach (DSArgument dsArg in DSDomainData.WhereClause.JavaArgs.Items)
            {
                // 1. Get technical name
                string techName = GetTechName(dsArg, DSDomainData.WhereClause.Fields.Items);

                // 2. Build paramater object
                JParameter jParam = new JParameter();
                jParam.BizName = dsArg.Text;
                jParam.DataType = dsArg.DataType;
                jParam.IsShow = dsArg.IsVisibleOnPlugin;
                jParam.ParamName = dsArg.Parameter;
                jParam.TechName = techName;
                jParam.Index = dsArg.Index;

                dataAgr.JParameters.Add(jParam);
            }

            return dataAgr;
        }

        private string GetTechName(DSArgument dsArg, List<DSField> lstDSFields)
        {
            string techName = string.Empty;
            foreach (DSField dsField in lstDSFields)
            {
                if (dsArg.Text.Equals(dsField.Text, System.StringComparison.OrdinalIgnoreCase))
                {
                    techName = dsField.TechnicalName;
                    break;
                }
            }
            return techName;
        }
        #endregion

        public Dictionary<string, string> MapBizTreeNodeNames { get; set; }
        public void AddMapBizTreeNodeNames(string bizName, string treeNodeName)
        {
            if (MapBizTreeNodeNames == null)
                MapBizTreeNodeNames = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(bizName))
            {
                if (!MapBizTreeNodeNames.ContainsKey(bizName))
                    MapBizTreeNodeNames.Add(bizName, treeNodeName);
                else
                    MapBizTreeNodeNames[bizName] = treeNodeName;
            }
        }
        public string GetTreeNodeName(string bizName)
        {
            if (MapBizTreeNodeNames != null && !string.IsNullOrWhiteSpace(bizName) && MapBizTreeNodeNames.ContainsKey(bizName))
                return MapBizTreeNodeNames[bizName];

            return string.Empty;
        }
    }
}