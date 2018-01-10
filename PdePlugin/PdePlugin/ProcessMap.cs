using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;
using System.Windows.Forms;
using PdePlugin.control;
using System.IO;

namespace PdePlugin
{
    public class ProcessMap
    {
        private MapInfo mapInfo;
        public List<TabCol> mapCols { get; set; }

        public ProcessMap()
        {
            if (mapInfo == null)
                mapInfo = new MapInfo();
        }

        public void MapCell(Workbook workbook, TreeNode selectedNode)
        {
            //make sure current selection is cell.
            Range curCell = workbook.Application.ActiveCell;
            curCell.Select();
            //check whether the cell has been mapped.


            //check selected node is single leaf node.
            if (selectedNode == null || selectedNode.Tag == null || !(selectedNode.Tag as string).Equals("1"))
            {
                MessageBox.Show("Please select single node in tree");
                return;
            }

            //save the map info
            MapNode mn = new MapNode();
            mn.treeNode = selectedNode.FullPath;
            mn.type = MapType.SingleCell;
            mn.target = GetNameForRange(workbook.Application);
            mapInfo.Maps.Add(mn);

            //blod the tree text
            System.Drawing.Font nf = new System.Drawing.Font("SimSun", 9, System.Drawing.FontStyle.Bold);
            selectedNode.NodeFont = nf;

            //hightlight the cell
            highlightRange(curCell);
        }

        private string GetNameForRange(Microsoft.Office.Interop.Excel.Application excel)
        {
            String rangName;
            try
            {
                rangName = excel.Selection.Name.Name;
                return rangName;
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {

            }
            DateTime dt = DateTime.Now;
            rangName = "pde_" + dt.ToString("yyyyMMddHHmmss") + excel.Names.Count;
            excel.Names.Add(rangName, excel.Selection);

            return rangName;
        }

        public void MapTable(Workbook workbook, TreeNode selectedNode)
        {
            //check selected node is multi node and not leaf node.
            if (selectedNode == null || selectedNode.Tag == null || !(selectedNode.Tag as string).Equals("99"))
            {
                MessageBox.Show("Please select multi parent node in tree");
                return;
            }
            //check whether the node has been mapped.

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
            if (lst == null)//there is no table exist, we create one for current selection
            {
                //create table
                Range selection = workbook.Application.Selection;
                lst = curSheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange, selection, XlYesNoGuess.xlYes);
                lst.Name = GenTableName(workbook.Application);
            }

            //show window to map column.
            TableMapForm form = new TableMapForm();
            form.InitDataSource(lst.ListColumns, selectedNode.Nodes);
            form.parentNodePath = selectedNode.FullPath;
            form.processMap = this;
            form.ShowDialog();

            if (mapCols == null || mapCols.Count < 1)
                return;

            //save the mapinfo.
            MapNode mn = new MapNode();
            mn.treeNode = selectedNode.FullPath;
            mn.type = MapType.Table;
            mn.target= curSheet.Name + "!" + lst.Name;
            mn.columns = mapCols;
            mapInfo.Maps.Add(mn);

            //hightlight the treenode and mapped column.
            System.Drawing.Font nf = new System.Drawing.Font("SimSun", 9, System.Drawing.FontStyle.Bold);
            foreach( TabCol tc in mapCols)
            {
                Range r = lst.ListColumns.get_Item(tc.columnName).Range;
                highlightRange(r);
                TreeNodeCollection tnc = selectedNode.Nodes;
                for (int i = 0; i < tnc.Count; i++)
                {
                    if (tnc[i].FullPath.Equals(tc.treeNode))
                    {
                        tnc[i].NodeFont = nf;
                        break;
                    }
                }
            }

            //reset value
            mapCols = null;
        }

        private void highlightRange(Range range)
        {
            range.Cells.Borders[XlBordersIndex.xlEdgeTop].ColorIndex = 8;
            range.Cells.Borders[XlBordersIndex.xlEdgeLeft].ColorIndex = 8;
            range.Cells.Borders[XlBordersIndex.xlEdgeRight].ColorIndex = 8;
            range.Cells.Borders[XlBordersIndex.xlEdgeBottom].ColorIndex = 8;
        }

        private string GenTableName(Microsoft.Office.Interop.Excel.Application excel)
        {
            String rangName;
            try
            {
                rangName = excel.Selection.Name.Name;
                return rangName;
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {

            }
            DateTime dt = DateTime.Now;
            rangName = "table_" + dt.ToString("yyyyMMddHHmmss") + excel.Names.Count;
            excel.Names.Add(rangName, excel.Selection);

            return rangName;
        }

        public bool ReadMapInfo(Workbook workbook)
        {
            foreach (CustomXMLPart curXmlPart in workbook.CustomXMLParts)
            {
                if (curXmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(curXmlPart.XML);
                if (customObject.ContentType == ContentType.PdeMapInfo)
                {
                    String xmlcontent = customObject.Content;
                    mapInfo = ObjectSerializeHelper.Deserialize<MapInfo>(xmlcontent);
                    return true;
                }
            }
            return false;
        }

        public bool SaveMapInfo(Workbook workbook)
        {
            //clear old map information xml
            foreach (CustomXMLPart curXmlPart in workbook.CustomXMLParts)
            {
                if (curXmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(curXmlPart.XML);
                if (customObject.ContentType == ContentType.PdeMapInfo)
                {
                    curXmlPart.Delete();
                    break;
                }
            }
            //add new map information
            CustomXMLPart xmlPart = workbook.CustomXMLParts.Add();
            string xmlContent = ObjectSerializeHelper.SerializeToString<MapInfo>(mapInfo);
            XmlObject excelContent = new XmlObject(xmlContent, ContentType.PdeMapInfo);
            xmlPart.LoadXML(ObjectSerializeHelper.SerializeToString<XmlObject>(excelContent));

            //Generate new xsd file and import it.
            GenXsd genXsd = new GenXsd();
            prepareData(genXsd);
            XmlMap xmap = genXsd.ImportXsd(workbook);

            //add xpath for each mapped cell and table
            foreach (MapNode mn in mapInfo.Maps)
            {
                if (mn.type == MapType.SingleCell)
                {
                    Range c = workbook.Application.get_Range(mn.target);
                    c.XPath.SetValue(xmap, mn.xPath);
                }
                else
                {
                    string[] target = mn.target.Split('!');
                    Worksheet worksheet = workbook.Sheets[target[0]];
                    ListObject lst = worksheet.ListObjects[target[1]];
                    foreach (TabCol tc in mn.columns)
                    {
                        lst.ListColumns[tc.columnName].XPath.SetValue(xmap, tc.xPath);
                    }
                }
            }

            return true;
        }

        public void TestMacro(Workbook workbook, String macroName)
        {
            String rtnVal = workbook.Application.Run(macroName,"222");
            MessageBox.Show(rtnVal);
        }

        public void TestXml(int i)
        {
            if (i == 1)
            {
                ObjTest t = new ObjTest();
                t.age = 10;
                t.name = "张三";
                t.birthday = DateTime.Now;
                t.sex = false;
                string xmlContent = ObjectSerializeHelper.SerializeToString<ObjTest>(t);
                FileStream fout = new FileStream("e:\\1.xml", FileMode.Create);
                StreamWriter sw = new StreamWriter(fout);
                sw.Write(xmlContent);
                sw.Close();
                fout.Close();
            }
            else
            {
                StreamReader objReader = new StreamReader("e:\\1.xml");
                string sLine = "";
                string xmlcontent = "";
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null && !sLine.Equals(""))
                        xmlcontent += sLine;
                }
                objReader.Close();
                MapNode mn = ObjectSerializeHelper.Deserialize<MapNode>(xmlcontent);
            }
        }

        //this is tempory method just for test. acutally the data should be producted while generate OSQL
        private void prepareData(GenXsd genXsd)
        {
            genXsd.selectedTables = new List<string>();
            genXsd.selectedColumns = new List<List<string>>();
            genXsd.tabColsType = new List<List<string>>();

            List<string> emCols = new List<string>();
            List<string> emColTyps = new List<string>();
            emCols.Add("FirstName");
            emColTyps.Add("string");
            emCols.Add("LastName");
            emColTyps.Add("string");
            emCols.Add("Title");
            emColTyps.Add("string");
            emCols.Add("Grade");
            emColTyps.Add("string");
            emCols.Add("Salary");
            emColTyps.Add("decimal");

            List<string> tfCols2 = new List<string>();
            List<string> tfColTyps2 = new List<string>();
            tfCols2.Add("FirstName");
            tfColTyps2.Add("date");
            tfCols2.Add("LastName");
            tfColTyps2.Add("string");
            tfCols2.Add("Birthday");
            tfColTyps2.Add("date");
            tfCols2.Add("Title");
            tfColTyps2.Add("string");
            tfCols2.Add("Grade");
            tfColTyps2.Add("string");
            tfCols2.Add("Salary");
            tfColTyps2.Add("decimal");

            List<string> mxCols3 = new List<string>();
            List<string> mxColTyps3 = new List<string>();
            mxCols3.Add("Since");
            mxColTyps3.Add("date");
            mxCols3.Add("Title");
            mxColTyps3.Add("string");
            mxCols3.Add("Salary");
            mxColTyps3.Add("decimal");

            //xpath prepare

            foreach (MapNode mn in mapInfo.Maps)
            {
                if (mn.type == MapType.SingleCell)
                {
                    if (genXsd.selectedTables.IndexOf("Employee") < 0)
                    {
                        genXsd.selectedTables.Add("Employee");
                        genXsd.selectedColumns.Add(emCols);
                        genXsd.tabColsType.Add(emColTyps);
                    }
                }
                else
                {
                    if (mn.treeNode.IndexOf("\\My Staff") > 0)
                    {
                        if (genXsd.selectedTables.IndexOf("Staff") < 0)
                        {
                            genXsd.selectedTables.Add("Staff");
                            genXsd.selectedColumns.Add(tfCols2);
                            genXsd.tabColsType.Add(tfColTyps2);
                        }
                    }
                    else if (mn.treeNode.IndexOf("\\My Matrix") > 0)
                    {
                        if (genXsd.selectedTables.IndexOf("JobHistory") < 0)
                        {
                            genXsd.selectedTables.Add("JobHistory");
                            genXsd.selectedColumns.Add(mxCols3);
                            genXsd.tabColsType.Add(mxColTyps3);
                        }
                    }
                }
            }

            //genXsd.selectedTables.Add("Employee");
            //genXsd.selectedTables.Add("JobHistory");

            if (mapInfo.Maps.Count < 1)
                return;

            Dictionary<string, string> xPaths = new Dictionary<string, string>();
            xPaths.Add("Employee\\First Name", "/PdeData/Employee/FirstName");
            xPaths.Add("Employee\\Last Name", "/PdeData/Employee/LastName");
            xPaths.Add("Employee\\Title", "/PdeData/Employee/Title");
            xPaths.Add("Employee\\Grade", "/PdeData/Employee/Grade");
            xPaths.Add("Employee\\Salary", "/PdeData/Employee/Salary");
            xPaths.Add("Employee\\My Staff\\+First Name", "/PdeData/Employee/Staff/FirstName");
            xPaths.Add("Employee\\My Staff\\+Last Name", "/PdeData/Employee/Staff/LastName");
            xPaths.Add("Employee\\My Staff\\+Title", "/PdeData/Employee/Staff/Title");
            xPaths.Add("Employee\\My Staff\\+Grade", "/PdeData/Employee/Staff/Grade");
            xPaths.Add("Employee\\My Staff\\+Birthday", "/PdeData/Employee/Staff/Birthday");
            xPaths.Add("Employee\\My Staff\\+Salary", "/PdeData/Employee/Staff/Salary");
            xPaths.Add("Employee\\My Matrix\\_Since", "/PdeData/Employee/JobHistory/Since");
            xPaths.Add("Employee\\My Matrix\\_Title", "/PdeData/Employee/JobHistory/Title");
            xPaths.Add("Employee\\My Matrix\\_Salary", "/PdeData/Employee/JobHistory/Salary");

            foreach (MapNode mn in mapInfo.Maps)
            {
                string xp = "";
                if (mn.type == MapType.SingleCell)
                {
                    xPaths.TryGetValue(mn.treeNode, out xp);
                    mn.xPath = xp;
                }
                else
                {
                    foreach (TabCol tc in mn.columns)
                    {
                        xPaths.TryGetValue(tc.treeNode, out xp);
                        tc.xPath = xp;
                    }
                }
            }

            //mapInfo.Maps[0].xPath = "/PdeData/Employee/LastName";
            //if (mapInfo.Maps.Count < 2)
            //    return;
            //mapInfo.Maps[1].xPath = "/PdeData/Employee/FirstName";
            //if (mapInfo.Maps.Count < 3)
            //    return;
            //mapInfo.Maps[2].xPath = "/PdeData/Employee/Salary";
            //if (mapInfo.Maps.Count < 4 || mapInfo.Maps[3].columns.Count < 1)
            //    return;
            //mapInfo.Maps[3].columns[0].xPath = "/PdeData/Employee/JobHistory/Since";
            //if (mapInfo.Maps.Count < 4 || mapInfo.Maps[3].columns.Count < 2)
            //    return;
            //mapInfo.Maps[3].columns[1].xPath = "/PdeData/Employee/JobHistory/Title";
            //if (mapInfo.Maps.Count < 4 || mapInfo.Maps[3].columns.Count < 3)
            //    return;
            //mapInfo.Maps[3].columns[2].xPath = "/PdeData/Employee/JobHistory/Salary";

        }
    }
}
