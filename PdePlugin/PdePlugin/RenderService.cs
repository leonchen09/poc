using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;
using System.Windows.Forms;

namespace PdePlugin
{
    public class RenderService
    {
        //check whether the .pder file has been merge data and set xpath already.
        private bool isFirstOpen = true;

        /* step1: merge data.
         * step2: run condition goal seek.
         * step3: export data to xml.
         */
        public bool Render(Workbook workbook, string exportFileName)
        {
            if( !mergeData(workbook) )
                return false;

            execCondGoalSeek(workbook);

            if(!exportData(workbook, exportFileName))
                return false;

            return true;
        }

        //extract the xml data file from excel archive file and import it into excel
        public bool mergeData(Workbook workbook)
        {
            String xmlcontent = null;
            XmlMap impMap = null;
            foreach (XmlMap xmlmap in workbook.XmlMaps)
            {
                if (xmlmap.Name.Equals(GenXsd.PDE_XSD_MAP_NAME))
                {
                    impMap = xmlmap;
                    break;
                }
            }

            if (impMap == null)//cannot found import xsd, meaning this has been merged.
            {
                isFirstOpen = false;
                return true;
            }

            foreach (CustomXMLPart curXmlPart in workbook.CustomXMLParts)
            {
                if (curXmlPart.BuiltIn)
                    continue;
                XmlObject customObject = ObjectSerializeHelper.Deserialize<XmlObject>(curXmlPart.XML);
                if (customObject.ContentType == ContentType.PderXml)
                {
                    xmlcontent = customObject.Content;
                    break;
                }
            }
            if (xmlcontent == null || xmlcontent.Length < 1)
            {
                MessageBox.Show("The file format has been broken!");
                workbook.Close();
                return false;
            }

            impMap.ImportXml(xmlcontent);
            //delete the xsd for import data 
            impMap.Delete();
            workbook.Save();
            return true;
        }

        public bool execCondGoalSeek(Workbook workbook)
        {
            ConditionGoalSeek seek = new ConditionGoalSeek();
            seek.ReadCGSInfo(workbook);
            return seek.runCondGS();
        }

        //read export item map information and put the xpath value of exported cell and table.
        public bool exportData(Workbook workbook, string exportFileName)
        {
            ExportData export = new ExportData();
            if (isFirstOpen)// when first open .pder, we need to set xpath for exported cell and table.
            {
                //get the export item information
                export.ReadExportInfo(workbook);
                //link the cell or table to xsd.
                export.linkToXsd(workbook);
                //save the exported xsd map
                workbook.Save();
            }
            //export xml file from the workbook after merge.
            export.exportXmlData(workbook, exportFileName);
            return false;
        }
    }
}
