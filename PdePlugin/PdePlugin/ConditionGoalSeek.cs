using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using PdePlugin.control;
using Microsoft.Office.Core;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;

namespace PdePlugin
{
    public class ConditionGoalSeek
    {
        public CondGoalSeekInfo cgsInfo ;
        public Workbook workbook { get; set; }

        public ConditionGoalSeek()
        {
            cgsInfo = new CondGoalSeekInfo();
        }

        public bool createCGS(Workbook workbook)
        {
            this.workbook = workbook;
            ConditionGSForm form = new ConditionGSForm();
            form.condGS = this;
            //form.Show();
            form.ShowDialog();
            return true;
        }

        public bool listCGS(Workbook workbook)
        {
            ConditionGSListForm form = new ConditionGSListForm();
            form.condGS = this;
            form.initData();
            form.ShowDialog();
            return true;
        }

        public bool ReadCGSInfo(Workbook workbook)
        {
            this.workbook = workbook;
            foreach (CustomXMLPart curXmlPart in workbook.CustomXMLParts)
            {
                if (curXmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(curXmlPart.XML);
                if (customObject.ContentType == ContentType.PdeCondGoalSeekInfo)
                {
                    String xmlcontent = customObject.Content;
                    CondGoalSeekInfo CGSInfo = ObjectSerializeHelper.Deserialize<CondGoalSeekInfo>(xmlcontent);
                    this.cgsInfo = CGSInfo;
                    return true;
                }
            }
            return false;
        }

        public bool SaveCGSInfo(Workbook workbook)
        {
            //clear old export item information xml
            foreach (CustomXMLPart curXmlPart in workbook.CustomXMLParts)
            {
                if (curXmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(curXmlPart.XML);
                if (customObject.ContentType == ContentType.PdeCondGoalSeekInfo)
                {
                    curXmlPart.Delete();
                    break;
                }
            }
            //add new information
            CustomXMLPart xmlPart = workbook.CustomXMLParts.Add();
            string xmlContent = ObjectSerializeHelper.SerializeToString<CondGoalSeekInfo>(this.cgsInfo);
            XmlObject excelContent = new XmlObject(xmlContent, ContentType.PdeCondGoalSeekInfo);
            xmlPart.LoadXML(ObjectSerializeHelper.SerializeToString<XmlObject>(excelContent));

            return true;
        }

        public bool runCondGS()
        {
            foreach (CondGoalSeekItem CGSItem in this.cgsInfo.CGSInfos)
            {
                bool result = true;
                foreach (CondidtionItem condition in CGSItem.conditionList)
                {
                    double condCell = workbook.Application.get_Range(condition.condCell).Value;
                    string operatorStr = condition.operatorStr;
                    double condValue = float.Parse(condition.condValue);
                    result = false;
                    switch (operatorStr)
                    {
                        case ">":
                            result = (condCell > condValue);
                            break;
                        case ">=":
                            result = (condCell >= condValue);
                            break;
                        case "=":
                            result = (condCell == condValue);
                            break;
                        case "<=":
                            result = (condCell <= condValue);
                            break;
                        case "<":
                            result = (condCell < condValue);
                            break;
                        case "!=":
                            result = (condCell != condValue);
                            break;
                    }
                    if (!result)//this condition cannot matched.
                        break;
                }
                if (!result)//this goal seek cannot matched, we goto next seek item.
                    continue;

                seek(CGSItem.targetCell, CGSItem.variableCell, CGSItem.targetValue);

            }
            return false;
        }

        public bool seek(string targetCellstr, string variabelCellstr, string targetValuestr)
        {
            Range targetCell = workbook.Application.get_Range(targetCellstr);
            Range varCell = workbook.Application.get_Range(variabelCellstr);
            targetCell.GoalSeek(targetValuestr, varCell);
            
            return true;
        }

        public string GetNameForRange(Microsoft.Office.Interop.Excel.Application excel)
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
    }
}
