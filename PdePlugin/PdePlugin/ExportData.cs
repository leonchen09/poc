using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using word = Microsoft.Office.Interop.Word;
using System.Windows.Forms;
using Pdw.Managers.Hcl;
using sch = System.Xml.Schema;
using System.IO;
using System.Xml;
using ProntoDoc.Framework.CoreObject;
using Microsoft.Office.Core;
using System.Reflection;
using PdePlugin.control;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;

namespace PdePlugin
{
    public class ExportData
    {
        public const string ROOT_ELEMENT = "PdeExpData";
        private const string PDE_EXP_XSD_MAP_NAME = "pde_exp_xsd_map";
        private const string ns = "http://www.w3.org/2001/XMLSchema";

        public List<ExportItemMap> exportItems { get; set; }

        public ExportData()
        {
            exportItems = new List<ExportItemMap>();
        }

        public Boolean exportItem(Workbook workbook, ProntoExcelMarkup proMarkupCtrl)
        {
            //make sure current selection is cell.
            Range curCell = workbook.Application.ActiveCell;
            curCell.Select();
            
            string excelName = GetNameForRange(workbook.Application);
            if (checkDuplicate(excelName))
            {
                MessageBox.Show("This cell already exported.");
                return false;
            }

            string type = getDataType(curCell);

            TreeNode node = proMarkupCtrl.treeView2.Nodes[0].Nodes.Add(excelName);

            ExportItemMap itemMap = new ExportItemMap();
            itemMap.mapType = ProntoDoc.Framework.CoreObject.MapType.singleCell;
            itemMap.excelName = excelName;
            itemMap.treeNodeName = excelName; //default value is excle range name;
            itemMap.treeNodeIndex = node.Index;
            itemMap.dataType = type;
            itemMap.treeNodePath = node.FullPath;
            exportItems.Add(itemMap);

            //hightlight the cell
            highlightRange(curCell);

            //refresh word tree automatic.
            string fullName = workbook.FullName;
            String subTemplateName = null;
            Globals.ThisAddIn.subTempMaps.TryGetValue(fullName, out subTemplateName);
            addWordItem(subTemplateName, itemMap);

            return true;
        }

        public bool exportTable(Workbook workbook, ProntoExcelMarkup proMarkupCtrl)
        {
            //check whether current selection is table.
            Worksheet curSheet = workbook.ActiveSheet;
            Range actCell = workbook.Application.ActiveCell;
            ListObject lst = null;
            try
            {
                lst = actCell.ListObject;
            }
            catch
            {
            }
            if (lst == null)
            {
                MessageBox.Show("Please select a table in excel");
                return false;
            }
            //show window to select and define business tag.
            ExpTableForm expFrm = new ExpTableForm();
            expFrm.initDataSource(lst.ListColumns);
            expFrm.ShowDialog();
            List<TableColumnMap> cols = expFrm.cols;
            //expFrm.Close();
            if (cols == null || cols.Count < 1)
                return false;

            //update tree
            TreeNode node = proMarkupCtrl.treeView2.Nodes[0].Nodes.Add(lst.Name);
            foreach (TableColumnMap col in cols)
            {
                node.Nodes.Add(col.treeNodeName);
                //highlight exported column in table
                Range r = lst.ListColumns.get_Item(col.columnName).Range;
                highlightRange(r);
            }
            //save mapinformation
            ExportItemMap item = new ExportItemMap();
            item.excelName = workbook.ActiveSheet.Name + "!"+ lst.Name;
            item.mapType = ProntoDoc.Framework.CoreObject.MapType.Table;
            item.treeNodeName = node.Text;
            item.treeNodePath = node.FullPath;
            item.treeNodeIndex = node.Index;
            item.tabCols = cols;
            exportItems.Add(item);

            //update the tree in word.
            string fullName = workbook.FullName;
            String subTemplateName = null;
            Globals.ThisAddIn.subTempMaps.TryGetValue(fullName, out subTemplateName);
            addWordItem(subTemplateName, item);

            return true;
        }

        private string GetNameForRange(Microsoft.Office.Interop.Excel.Application excel)
        {
            String rangeName;
            try
            {
                rangeName = excel.Selection.Name.Name;
                return rangeName;
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
            }
            DateTime dt = DateTime.Now;
            rangeName = "pde_" + dt.ToString("yyyyMMddHHmmss") + excel.Names.Count;
            excel.Names.Add(rangeName, excel.Selection);

            return rangeName;
        }

        private string getDataType(Range cell)
        {
            string result = "";
            WorksheetFunction func = cell.Application.WorksheetFunction;
            if (func.IsLogical(cell))
                result = "boolean";
            else if (func.IsNumber(cell))
            {
                try
                {
                    Convert.ToDateTime(cell.Text);
                    result = "date";
                }
                catch
                {
                    result = "decimal";
                }
            }
            else //if (func.IsText(cell))
                result = "string"; //default type is string
            return result;
        }

        public Boolean updateName(string nodePath, string newName, string newNodePath)
        {
            string fullName = Globals.ThisAddIn.Application.ActiveWorkbook.FullName;
            string subTemplateName = null;
            Globals.ThisAddIn.subTempMaps.TryGetValue(fullName, out subTemplateName);
            foreach (ExportItemMap itemMap in exportItems)
            {
                if (nodePath.Equals(itemMap.treeNodePath))
                {
                    itemMap.treeNodeName = newName;
                    itemMap.treeNodePath = newNodePath;
                }
            }
            updateWordItem(subTemplateName, nodePath, newName);

            return true;
        }

        private bool updateWordItem(String subTemplateName, string treeNodePath, string newName)
        {
            word.Application Word = Globals.ThisAddIn.wordApp;
            if (Word == null || Word.Visible == false)
            {
                MessageBox.Show("word doesn't link, this just for debug");
                return false;//do not link, just return.
            }
            COMAddIns addins = Word.COMAddIns;
            foreach (COMAddIn addin in addins)
            {
                if (addin.Description.Equals("Pdw"))
                {
                    Object pdwApp = addin.Object;
                    object[] invokeArgs = { subTemplateName, treeNodePath, newName };
                    object retVal = pdwApp.GetType().InvokeMember("updatePdeItem",
                        System.Reflection.BindingFlags.InvokeMethod, null, pdwApp, invokeArgs);
                    return Boolean.Parse(retVal.ToString());
                }
            }
            MessageBox.Show("Cannot found pdw plugin in word, please check it!");
            return false;
        }

        private bool addWordItem(String subTemplateName, ExportItemMap itemMap)
        {
            word.Application Word = Globals.ThisAddIn.wordApp;
            if (Word == null || Word.Visible == false)
            {
                MessageBox.Show("word doesn't link, this just for debug");
                return false;//do not link, just return.
            }
            COMAddIns addins = Word.COMAddIns;
            foreach (COMAddIn addin in addins)
            {
                if (addin.Description.Equals("Pdw"))
                {
                    Object pdwApp = addin.Object;
                    //object[] invokeArgs = { subTemplateName, itemMap.treeNodeIndex, itemMap.treeNodeName, itemMap.type };
                    int[] indexs ;
                    string[] names;
                    string[] types;
                    if (itemMap.mapType == ProntoDoc.Framework.CoreObject.MapType.singleCell)
                    {
                        indexs = new int[1];
                        names = new string[1];
                        types = new string[1];
                    }
                    else
                    {
                        int count = itemMap.tabCols.Count + 1;
                        indexs = new int[count];
                        names = new string[count];
                        types = new string[count];
                        int i = 1;
                        foreach (TableColumnMap col in itemMap.tabCols)
                        {
                            indexs[i] = 0;//
                            names[i] = col.treeNodeName;
                            types[i] = col.dataType;
                            i++;
                        }
                    }
                    indexs[0] = itemMap.treeNodeIndex;
                    names[0] = itemMap.treeNodeName;
                    types[0] = itemMap.dataType;
                    
                    object[] invokeArgs = { subTemplateName, indexs, names, types };
                    try
                    {
                        object retVal = pdwApp.GetType().InvokeMember("addPdeItem",
                            System.Reflection.BindingFlags.InvokeMethod, null, pdwApp, invokeArgs);
                        return Boolean.Parse(retVal.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.StackTrace);
                    }
                }
            }
            return false;
        }

        public XmlMap ExportXsd(Workbook workbook)
        {
            //clear the old pde xsd map if it existed.
            XmlMaps xms = workbook.XmlMaps;
            foreach (XmlMap map in xms)
            {
                if (PDE_EXP_XSD_MAP_NAME.Equals(map.Name))
                {
                    map.Delete();
                    break;
                }
            }
            //import new xsd
            XmlMap xmap = xms.Add(genExpXsd());
            xmap.Name = PDE_EXP_XSD_MAP_NAME;
            //DO NOT link the excel cell or table to xsd now, we will link them after render and delete the import xsd.
            //linkToXsd(workbook);
            return xmap;
        }

        public bool linkToXsd(Workbook workbook)
        {
            XmlMap xmlMap = null;
            foreach (XmlMap map in workbook.XmlMaps)
            {
                if (PDE_EXP_XSD_MAP_NAME.Equals(map.Name))
                {
                    xmlMap = map;
                    break;
                }
            }
            if (xmlMap == null)
                return false;

            foreach (ExportItemMap itemMap in exportItems)
            {
                if (itemMap.mapType == ProntoDoc.Framework.CoreObject.MapType.singleCell)
                {
                    Range c = workbook.Application.get_Range(itemMap.excelName);
                    c.XPath.SetValue(xmlMap, "/" + ROOT_ELEMENT + "/" + itemMap.treeNodeName);
                }
                else
                {
                    string path = "/" + ROOT_ELEMENT + "/" + itemMap.treeNodeName;
                    string[] target = itemMap.excelName.Split('!');
                    Worksheet worksheet = workbook.Sheets[target[0]];
                    ListObject lst = worksheet.ListObjects[target[1]];
                    foreach (TableColumnMap col in itemMap.tabCols)
                    {
                        lst.ListColumns[col.columnName].XPath.SetValue(xmlMap, path + "/" + col.treeNodeName);
                    }
                }
            }
            return true;
        }

        //export the data into xml file, then merge to word.
        //this method call after pde render and before pdw render.
        public string exportXmlData(Workbook workbook, string xmlFileName)
        {
            XmlMap xm = null;
            foreach (XmlMap map in workbook.XmlMaps)
            {
                if (PDE_EXP_XSD_MAP_NAME.Equals(map.Name))
                {
                    xm = map;
                    break;
                }
            }
            if(xm == null || !xm.IsExportable)
                return "This excel do not contain a xsd to export data.";
            workbook.SaveAsXMLData(xmlFileName, xm);
            return "";
        }

        public bool ReadExportInfo(Workbook workbook)
        {
            foreach (CustomXMLPart curXmlPart in workbook.CustomXMLParts)
            {
                if (curXmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(curXmlPart.XML);
                if (customObject.ContentType == ContentType.PdeExportItem)
                {
                    String xmlcontent = customObject.Content;
                    PdeExports pdeExports = ObjectSerializeHelper.Deserialize<PdeExports>(xmlcontent);
                    this.exportItems = pdeExports.items;
                    return true;
                }
            }
            return false;
        }

        public bool SaveExportInfo(Workbook workbook)
        {
            //clear old export item information xml
            foreach (CustomXMLPart curXmlPart in workbook.CustomXMLParts)
            {
                if (curXmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(curXmlPart.XML);
                if (customObject.ContentType == ContentType.PdeExportItem)
                {
                    curXmlPart.Delete();
                    break;
                }
            }
            //add new information
            CustomXMLPart xmlPart = workbook.CustomXMLParts.Add();
            string xmlContent = ObjectSerializeHelper.SerializeToString<PdeExports>(new PdeExports(exportItems));
            XmlObject excelContent = new XmlObject(xmlContent, ContentType.PdeExportItem);
            xmlPart.LoadXML(ObjectSerializeHelper.SerializeToString<XmlObject>(excelContent));

            return true;
        }

        private string genExpXsd()
        {
            sch.XmlSchema expSchema = new sch.XmlSchema();

            //Create the PdeData element
            sch.XmlSchemaElement rootElement = new sch.XmlSchemaElement();
            rootElement.Name = ROOT_ELEMENT;
            expSchema.Items.Add(rootElement);
            sch.XmlSchemaComplexType rootType = new sch.XmlSchemaComplexType();
            rootType.IsMixed = false;
            sch.XmlSchemaSequence rootSequence = new sch.XmlSchemaSequence();
            rootType.Particle = rootSequence;
            rootElement.SchemaType = rootType;

            foreach (ExportItemMap itemMap in exportItems)
            {
                if (itemMap.mapType == ProntoDoc.Framework.CoreObject.MapType.singleCell)
                {
                    sch.XmlSchemaElement item = new sch.XmlSchemaElement();
                    item.Name = itemMap.treeNodeName;
                    item.SchemaTypeName = new XmlQualifiedName(itemMap.dataType, ns);
                    rootSequence.Items.Add(item);
                }
                else
                {
                    sch.XmlSchemaElement tabElement = new sch.XmlSchemaElement();
                    tabElement.MinOccurs = 0;
                    tabElement.MaxOccursString = "unbounded";
                    tabElement.Name = itemMap.treeNodeName;
                    rootSequence.Items.Add(tabElement);

                    sch.XmlSchemaComplexType tabType = new sch.XmlSchemaComplexType();
                    tabType.IsMixed = false;
                    sch.XmlSchemaAll tabAll = new sch.XmlSchemaAll();
                    tabType.Particle = tabAll;
                    tabElement.SchemaType = tabType;

                    //generate children node
                    foreach (TableColumnMap col in itemMap.tabCols)
                    {
                        sch.XmlSchemaElement item = new sch.XmlSchemaElement();
                        item.Name = col.treeNodeName;
                        item.SchemaTypeName = new XmlQualifiedName(col.dataType, ns);
                        tabAll.Items.Add(item);
                    }
                }
            }

            expSchema.Compile(new sch.ValidationEventHandler(ValidationEventHandler));
            FileStream stream = new FileStream("e:\\tempout.xsd", FileMode.Create);

            //Write the file
            expSchema.Write(stream);
            stream.Close();

            return "e:\\tempout.xsd";
        }

        void ValidationEventHandler(object sender, sch.ValidationEventArgs args)
        {

        } 
        private bool checkDuplicate(string excelName)
        {
            foreach (ExportItemMap itemMap in exportItems)
            {
                if(excelName.Equals(itemMap.excelName))
                    return true;
            }
            return false;
        }

        private void highlightRange(Range range)
        {
            range.Cells.Borders[XlBordersIndex.xlEdgeTop].ColorIndex = 9;
            range.Cells.Borders[XlBordersIndex.xlEdgeLeft].ColorIndex = 9;
            range.Cells.Borders[XlBordersIndex.xlEdgeRight].ColorIndex = 9;
            range.Cells.Borders[XlBordersIndex.xlEdgeBottom].ColorIndex = 9;
        }
        
    }

}
