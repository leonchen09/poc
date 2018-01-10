
using System;
using System.Linq;
using Schema = System.Xml.Schema;

using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdw.Core;

namespace Pdw.Services.Integration
{
    public class PdeService
    {
        private static Excel.Application _eApp = null;
        private static Word.Document _wDoc = null;
        private const string ChartExtension = "jpg";

        #region import data
        /// <summary>
        /// open pde template and make link with excel add-in
        /// </summary>
        /// <param name="pdeFilePath"></param>
        /// <param name="wDoc"></param>
        public void ImportPde(PdeContentItem pdeContentItem, Word.Document wDoc)
        {
            _wDoc = wDoc;
            Excel.Workbook workBook = OpenExcelFileToImport(pdeContentItem.FilePath);

            pdeContentItem.ExportData = GetExportedData(workBook);

            CloseExcel();
        }

        /// <summary>
        /// import table from pde into pdw directly from excel add-in
        /// </summary>
        /// <param name="eRange"></param>
        /// <param name="wDoc"></param>
        public void ImportPdeTable(Excel.Range eRange, Word.Document wDoc)
        {
            eRange.Copy();
            wDoc.Application.Selection.PasteExcelTable(true, false, false);

            // to switch show value/lin
            //k press Alt + F9. (ShowFieldCodes = false/true)
            wDoc.ActiveWindow.View.ShowFieldCodes = false;
            System.Windows.Forms.Clipboard.Clear();
            wDoc.UndoClear();
        }

        /// <summary>
        /// import chart from pde into pdw directly from excel add-in
        /// </summary>
        /// <returns></returns>
        public void ImportPdeChart(Excel.Chart eChart, Word.Document wDoc)
        {
            if (eChart == null)
                return;

            eChart.ChartArea.Copy();
            wDoc.Application.Selection.Paste();
        }

        /// <summary>
        /// close the pde template
        /// </summary>
        public void CloseExcel()
        {
            try
            {
                if (IsExcelOpenning())
                {
                    _eApp.DisplayAlerts = false;
                    if (_eApp.Workbooks != null)
                        _eApp.Workbooks.Close();

                    if (_eApp.Workbooks == null || _eApp.Workbooks.Count == 0)
                        _eApp.Quit();

                    _eApp = null;
                }
            }
            catch { }
        }

        /// <summary>
        /// check the usage of pde content and update status
        /// </summary>
        /// <param name="pdeContent"></param>
        public void CheckPdeContent(Word.Document wDoc, PdeContent pdeContent)
        {
            if (pdeContent == null || pdeContent.Items == null || pdeContent.Items.Count == 0)
                return;

            CheckPdeLinkSource(wDoc, pdeContent);
        }

        public string ImportPdeTag(Word.Document wDoc, PdeContentItem pdeContentItem,
            string domainName, string tableExcelName, string columnExcelName, string displayName)
        {
            string bmName = string.IsNullOrWhiteSpace(columnExcelName) ? tableExcelName : columnExcelName;
            bmName = ImportPdeTag(wDoc, bmName, displayName);

            if (!string.IsNullOrWhiteSpace(bmName))
                ImportPdeTag(pdeContentItem, domainName, tableExcelName, columnExcelName);

            return bmName;
        }

        private string ImportPdeTag(Word.Document wDoc, string key, string value)
        {
            if (wDoc == null || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // add a space before text.
            wDoc.Application.Selection.TypeText(" ");

            // add select text
            string bmValue = MarkupUtilities.GenTextXslTag(value, XsltType.Select, true);
            wDoc.Application.Selection.TypeText(bmValue);
            wDoc.Application.Selection.MoveLeft(Word.WdUnits.wdCharacter, bmValue.Length, Word.WdMovementType.wdExtend);

            // add select bookmark
            string bmName = Core.MarkupConstant.MarkupPdeTag + key;
            Word.Bookmark oldBm = null;
            foreach (Word.Bookmark bookmark in wDoc.Bookmarks)
            {
                if (string.Equals(bmName, bookmark.Name, StringComparison.OrdinalIgnoreCase))
                {
                    oldBm = bookmark;
                    break;
                }
            }
            if (oldBm != null)
                oldBm.Delete();
            Word.Range range = wDoc.Application.Selection.Range;
            wDoc.Bookmarks.Add(bmName, range);
            wDoc.Bookmarks.DefaultSorting = Word.WdBookmarkSortBy.wdSortByLocation;
            wDoc.Bookmarks.ShowHidden = false;

            // set cursor after bookmark
            wDoc.Application.Selection.MoveRight(Word.WdUnits.wdCharacter, 1);

            // markup ProntoDoc;
            WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.IsProntoDoc = true;
            if (!System.IO.File.Exists(wDoc.FullName))
                wDoc.Application.Options.SaveInterval = 0;

            return bmName;
        }

        private bool ImportPdeTag(PdeContentItem pdeContentItem, string domainName, string tableExcelName, string columnExcelName)
        {
            if (pdeContentItem == null || pdeContentItem.ExportData == null || pdeContentItem.ExportData.Items == null)
                return false;

            // validate export domain information
            DomainExportItem expDomain = pdeContentItem.ExportData.Items.FirstOrDefault(
                c => string.Equals(c.DomainName, domainName, StringComparison.OrdinalIgnoreCase));
            if (expDomain == null || expDomain.Items == null)
                return false;

            // validate export item information
            ExportItem expItem = expDomain.Items.FirstOrDefault(
                c => string.Equals(c.ExcelName, tableExcelName, StringComparison.OrdinalIgnoreCase));
            if (expItem == null)
                return false;

            // validate export column
            if (!string.IsNullOrWhiteSpace(columnExcelName))
            {
                if (expItem.Columns == null)
                    return false;
                ColumnExportItem expColumn = expItem.Columns.FirstOrDefault(
                    c => string.Equals(c.ExcelName, columnExcelName, StringComparison.OrdinalIgnoreCase));
                if (expColumn == null)
                    return false;
                expColumn.IsUsed = true;
            }
            expItem.IsUsed = true;
            return true;
        }

        public string ImportPdeChart(Word.Document wDoc, PdeContentItem pdeContentItem,
            string domainName, string chartName, string chartContent)
        {
            string bmName = ImportPdeChart(wDoc, chartName, chartContent);

            ImportPdeChart(pdeContentItem, domainName, chartName);

            return bmName;
        }

        private string ImportPdeChart(Word.Document wDoc, string chartName, string chartContent)
        {
            string chartPath = AssetManager.FileAdapter.GenRandomFilePath(ChartExtension);
            string bmValue = MarkupUtilities.GenTextXslTag(chartName, XsltType.Select, true);
            string bmName = BaseMarkupUtilities.XmlEncode(chartName);

            // create temporary image
            bmName = string.Format("{0}{1}{2}", MarkupConstant.MarkupPdeTag, bmName, MarkupConstant.MarkupPdeChart);
            ProntoDoc.Framework.Utils.FileHelper.FileFromBase64(chartContent, chartPath);

            // delete old bm
            Word.Bookmark oldBm = null;
            foreach (Word.Bookmark bookmark in wDoc.Bookmarks)
            {
                if (string.Equals(bmName, bookmark.Name, StringComparison.OrdinalIgnoreCase))
                {
                    oldBm = bookmark;
                    break;
                }
            }
            if (oldBm != null)
                oldBm.Delete();

            // add image into word
            wDoc.Application.Selection.TypeText(" ");
            Word.InlineShape inlineShape = wDoc.Application.Selection.InlineShapes.AddPicture(chartPath, false, true);
            inlineShape.AlternativeText = MarkupUtilities.CreateAlternativeText(bmName, bmValue);

            // add bookmark
            wDoc.Application.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
            wDoc.Application.Selection.MoveRight(Word.WdUnits.wdCharacter, 1, Word.WdMovementType.wdExtend);
            wDoc.Bookmarks.Add(bmName, wDoc.Application.Selection.Range);
            wDoc.Application.Selection.MoveRight(Word.WdUnits.wdCharacter);

            // delete temporary file
            System.IO.File.Delete(chartPath);

            return bmName;
        }

        private bool ImportPdeChart(PdeContentItem pdeContentItem, string domainName, string chartName)
        {
            if (pdeContentItem == null || pdeContentItem.ExportData == null || pdeContentItem.ExportData.Items == null)
                return false;

            // validate export domain information
            DomainExportItem expDomain = pdeContentItem.ExportData.Items.FirstOrDefault(
                c => string.Equals(c.DomainName, domainName, StringComparison.OrdinalIgnoreCase));
            if (expDomain == null || expDomain.Items == null)
                return false;

            // validate export item information
            ExportItem expItem = expDomain.Items.FirstOrDefault(
                c => c.MapType == MapType.Chart && c.Chart != null &&
                    string.Equals(c.Chart.Name, chartName, StringComparison.OrdinalIgnoreCase));
            if (expItem == null)
                return false;

            expItem.IsUsed = true;

            return true;
        }

        #region helper methods
        public Excel.Workbook OpenExcelFileToImport(string filePath)
        {
            // close old file
            CloseExcel();

            // open new file
            _eApp = new Excel.Application();
            Excel.Workbook workBook = _eApp.Workbooks.Open(filePath, AddToMru: false);
            _eApp.Visible = false;
            return workBook;
        }

        private bool IsExcelOpenning()
        {
            if (_eApp == null)
                return false;

            return true;
        }

        private void IntergrateWithExcel(string methodName, object[] args)
        {
            Office.COMAddIns eAddIns = _eApp.COMAddIns;
            foreach (Office.COMAddIn eAddIn in eAddIns)
            {
                if ("Pde".Equals(eAddIn.Description, StringComparison.OrdinalIgnoreCase))
                {
                    Object ePlugin = eAddIn.Object;
                    ePlugin.GetType().InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, ePlugin, args);
                    break;
                }
            }
        }

        private string GetRangeName()
        {
            string rangName;
            try
            {
                rangName = _eApp.Selection.Name.Name;

                return rangName;
            }
            catch { }

            // add new Name
            rangName = string.Format("pde_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), _eApp.Names.Count);
            _eApp.Names.Add(rangName, _eApp.Selection);

            // save the workbook which we has been add new name.
            _eApp.ActiveWorkbook.Save();
            return rangName;
        }

        private void CheckPdeLinkSource(Word.Document wDoc, PdeContent pdeContent)
        {
            if (wDoc == null)
                return;
            foreach (PdeContentItem item in pdeContent.Items)
            {
                bool isUse = IsUseInField(wDoc, item); // check Field

                if (!isUse)
                    isUse = IsUseInInlineShape(wDoc, item); // check Chart

                if (!isUse) // check Tag
                {
                    if (item.ExportData != null && item.ExportData.Items != null)
                    {
                        foreach (DomainExportItem expDomain in item.ExportData.Items)
                        {
                            if (expDomain.Items != null && expDomain.Items.Exists(c => c.IsUsed))
                            {
                                isUse = true;
                                break;
                            }
                        }
                    }
                }

                item.Status = isUse ? Core.Constants.ContextManager.UsingStatus : Core.Constants.ContextManager.NotUsingStatus;
            }
        }

        private bool IsUseInField(Word.Document wDoc, PdeContentItem pdeContentItem)
        {
            foreach (Word.Field field in wDoc.Fields)
            {
                if (field.Type == Word.WdFieldType.wdFieldLink)
                {
                    string linkSource = field.LinkFormat.SourceFullName;
                    if (linkSource.IndexOf(pdeContentItem.FilePath) > -1)
                        return true;
                }
            }

            return false;
        }

        private bool IsUseInInlineShape(Word.Document wDoc, PdeContentItem pdeContentItem)
        {
            foreach (Word.InlineShape shape in wDoc.InlineShapes)
            {
                if (shape.HasChart == Office.MsoTriState.msoTrue && shape.LinkFormat != null)
                {
                    string linkSource = shape.LinkFormat.SourceFullName;
                    if (linkSource.IndexOf(pdeContentItem.FilePath) > -1)
                        return true;
                }
            }

            return false;
        }

        private PdeExports GetExportedData(Excel.Workbook wExcel)
        {
            if (wExcel == null)
                return null;
            InternalBookmark ibm = GetPdeInternalBookmark(wExcel);
            PdeExports export = new PdeExports();
            DomainExportItem expDomain = new DomainExportItem();
            expDomain.DomainName = "DomainName";

            #region get tags
            foreach (Excel.Name name in wExcel.Names)
            {
                ExportItem expItem = new ExportItem();
                expItem.IsUsed = false; // default is not used
                bool isAdd = false;
                string strName = name.Name;
                if (string.IsNullOrWhiteSpace(strName))
                    continue;

                // update field
                if (strName.EndsWith(BaseProntoMarkup.KeySelect))
                {
                    expItem.MapType = MapType.SingleCell;
                    expItem.DataType = GetPdeDataType(strName, ibm);
                    isAdd = true;
                }

                // update table
                if (strName.EndsWith(BaseProntoMarkup.KeyTable))
                {
                    expItem.MapType = MapType.Table;
                    Excel.Range range = name.RefersToRange;
                    Excel.ListObject listColumn = range.ListObject;
                    expItem.Columns = new System.Collections.Generic.List<ColumnExportItem>();

                    // update columns
                    foreach (Excel.ListColumn lstCol in listColumn.ListColumns)
                    {
                        ColumnExportItem expColumn = new ColumnExportItem();
                        expColumn.ColumnName = lstCol.Name;
                        expColumn.ExcelName = lstCol.Range.Name.Name;
                        expColumn.TreeNodeName = expColumn.ExcelName;
                        expColumn.ExcelAddress = lstCol.Range.AddressLocal;
                        expColumn.ParentName = strName;
                        expColumn.DomainName = expDomain.DomainName;
                        expColumn.IsUsed = false;
                        expColumn.DataType = GetPdeDataType(expColumn.ExcelName, ibm);
                        expItem.Columns.Add(expColumn);
                    }
                    isAdd = true;
                }

                if (isAdd)
                {
                    expItem.ExcelSheetName = name.RefersToRange.Parent.Name;
                    expItem.ExcelName = strName;
                    expItem.ExcelAddress = name.RefersToRange.AddressLocal;
                    expItem.TreeNodeName = expItem.ExcelName;
                    expItem.ParentName = expDomain.DomainName;
                    expDomain.Items.Add(expItem);
                }
            }
            #endregion

            #region get charts
            foreach (Excel.Worksheet sheet in wExcel.Sheets)
            {
                Excel.ChartObjects charts = sheet.ChartObjects(Type.Missing);
                if (charts != null)
                {
                    foreach (Excel.ChartObject chart in charts)
                    {
                        ExportItem exportItem = new ExportItem();
                        exportItem.MapType = MapType.Chart;
                        exportItem.ExcelName = chart.Name;
                        exportItem.TreeNodeName = exportItem.ExcelName;
                        exportItem.ParentName = expDomain.DomainName;
                        exportItem.ExcelSheetName = sheet.Name;

                        exportItem.Chart = new ExportItemChart();
                        exportItem.Chart.Name = chart.Name;
                        string filePath = AssetManager.FileAdapter.GenRandomFilePath(ChartExtension);
                        chart.Chart.Export(filePath, "JPG");
                        exportItem.Chart.Content = ProntoDoc.Framework.Utils.FileHelper.FileToBase64(filePath);
                        System.IO.File.Delete(filePath);

                        expDomain.Items.Add(exportItem);
                    }
                }
            }
            #endregion

            export.Items = new System.Collections.Generic.List<DomainExportItem>();
            export.Items.Add(expDomain);
            return export;
        }

        private string GenNewPdeFilePath(PdeContentItem pdeContentItem)
        {
            return string.Format("{0}\\{1}_{2}.pde", AssetManager.FileAdapter.TemporaryFolderPath, Guid.NewGuid(), pdeContentItem.STN);
        }

        private InternalBookmark GetPdeInternalBookmark(Excel.Workbook workbook)
        {
            // get partID
            object objProperties = workbook.CustomDocumentProperties;
            Type objPropertiesType = objProperties.GetType();
            object objProperty = objPropertiesType.InvokeMember("Item",
                System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.GetProperty,
                null, objProperties, new object[] { ProntoMarkup.InternalBMCustomXmlPartId });
            Type objPropertyType = objProperty.GetType();
            string partID = objPropertyType.InvokeMember("Value",
                    System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.GetProperty,
                    null, objProperty, new object[] { }).ToString();
            if (string.IsNullOrWhiteSpace(partID))
                return null;

            // get XmlObject in CustomXmlPart
            Office.CustomXMLPart xmlPart = workbook.CustomXMLParts.SelectByID(partID);
            string xmlObjectString = xmlPart.XML;
            if (string.IsNullOrWhiteSpace(xmlObjectString))
                return null;
            XmlObject xmlObject = ProntoDoc.Framework.Utils.ObjectSerializeHelper.Deserialize<XmlObject>(xmlObjectString);

            // get InternalBookmark
            if (xmlObject == null || string.IsNullOrWhiteSpace(xmlObject.Content))
                return null;
            return ProntoDoc.Framework.Utils.ObjectSerializeHelper.Deserialize<InternalBookmark>(xmlObject.Content);
        }

        private string GetPdeDataType(string key, InternalBookmark ibm)
        {
            if (ibm == null || string.IsNullOrWhiteSpace(key))
                return string.Empty;

            InternalBookmarkItem ibmItem = ibm.GetInternalBookmarkItem(key);
            if (ibmItem == null)
                return string.Empty;

            return MarkupUtilities.SqlDbType2XsdDataType(ibmItem.DataType);
        }
        #endregion
        #endregion

        #region render data
        public System.Collections.Generic.List<string> RenderPdeInPdw(Word.Document wDoc, PdeContent pdeContent)
        {
            // validate
            System.Collections.Generic.List<string> tempFiles = new System.Collections.Generic.List<string>();
            if (pdeContent == null || pdeContent.Items == null || pdeContent.Items.Count == 0)
                return tempFiles;

            // extract and create excel files with new names
            string folder = System.IO.Path.GetDirectoryName(wDoc.FullName);
            System.Collections.Generic.Dictionary<string, string> dicLinks = RenderPdeContent(pdeContent, folder, ref tempFiles);

            // change link in Field with new name
            UpdateTableLink(wDoc, dicLinks);

            // change link in InlineShape(Chart) with new name
            UpdateChartLink(wDoc, dicLinks);

            // return list of temporaty files
            return tempFiles;
        }

        private System.Collections.Generic.Dictionary<string, string> RenderPdeContent(PdeContent pdeContent, string folder,
            ref System.Collections.Generic.List<string> tempFiles)
        {
            System.Collections.Generic.Dictionary<string, string> dicLinks = new System.Collections.Generic.Dictionary<string, string>();
            foreach (PdeContentItem item in pdeContent.Items)
            {
                string key = item.FilePath.Trim().ToLower();
                if (dicLinks.ContainsKey(key))
                    continue;

                string newFile = string.Format("{0}_{1}.xlsx", item.STN, Guid.NewGuid());
                newFile = System.IO.Path.Combine(folder, newFile);

                ProntoDoc.Framework.Utils.FileHelper.ExcelFromBase64(item.FileContent, newFile);
                dicLinks.Add(key, newFile);
                tempFiles.Add(newFile);
            }

            return dicLinks;
        }

        private void UpdateTableLink(Word.Document wDoc, System.Collections.Generic.Dictionary<string, string> dicLinks)
        {
            foreach (Word.Field field in wDoc.Fields)
            {
                if (field.Type == Word.WdFieldType.wdFieldLink && field.LinkFormat != null)
                {
                    string oldLink = field.LinkFormat.SourceFullName;
                    if (string.IsNullOrWhiteSpace(oldLink))
                        continue;

                    string key = oldLink.Trim().ToLower();
                    if (!dicLinks.ContainsKey(key))
                        continue;

                    field.LinkFormat.SourceFullName = dicLinks[key];
                    field.LinkFormat.Update();
                    field.ShowCodes = true;
                }
            }
        }

        private void UpdateChartLink(Word.Document wDoc, System.Collections.Generic.Dictionary<string, string> dicLinks)
        {
            foreach (Word.InlineShape shape in wDoc.InlineShapes)
            {
                if (shape.HasChart == Office.MsoTriState.msoTrue && shape.LinkFormat != null)
                {
                    string oldLink = shape.LinkFormat.SourceFullName;
                    if (string.IsNullOrWhiteSpace(oldLink))
                        continue;

                    string key = oldLink.Trim().ToLower();
                    if (!dicLinks.ContainsKey(key))
                        continue;

                    shape.LinkFormat.SourceFullName = dicLinks[key];
                    shape.LinkFormat.Update();
                }
            }
        }
        #endregion

        #region export data
        public void AddExportXSD(PdeContent pdeContent)
        {
            if (pdeContent == null || pdeContent.Items == null)
                return;

            foreach (PdeContentItem item in pdeContent.Items)
                AddExportXSD(item);
        }

        private void AddExportXSD(PdeContentItem pdeContentItem)
        {
            if (pdeContentItem == null || pdeContentItem.ExportData == null)
                return;

            string filePath = GenNewPdeFilePath(pdeContentItem);
            pdeContentItem.FilePath = filePath;
            ProntoDoc.Framework.Utils.FileHelper.ExcelFromBase64(pdeContentItem.FileContent, filePath);
            AddExportXSD(filePath, pdeContentItem.EncodeSTN, pdeContentItem.ExportData);

            // delete temporary file
            pdeContentItem.FileContent = ProntoDoc.Framework.Utils.FileHelper.ExcelToBase64(filePath);
            ProntoDoc.Framework.Utils.FileHelper.DeleteFile(filePath);
        }

        private void AddExportXSD(string pdeFilePath, string stnName, PdeExports exportData)
        {
            Excel.Application app = new Excel.Application();
            app.Visible = false;
            Excel.Workbook workbook = app.Workbooks.Open(pdeFilePath);

            try
            {
                Excel.XmlMaps maps = workbook.XmlMaps;

                // delete old mapp
                DeleteOldMap(maps, ProntoDoc.Framework.CoreObject.FrameworkConstants.PdeExportedMapName);

                // gen new xsd
                string xsdFile = GenExportedXSD(stnName, exportData);

                // add new map
                Excel.XmlMap map = maps.Add(xsdFile);
                map.Name = ProntoDoc.Framework.CoreObject.FrameworkConstants.PdeExportedMapName;

                // save
                workbook.Save();

                // delete xsd file
                ProntoDoc.Framework.Utils.FileHelper.DeleteFile(xsdFile);
            }
            finally
            {
                ((Excel._Workbook)workbook).Close();
                ((Excel._Application)app).Quit();
            }
        }

        private void DeleteOldMap(Excel.XmlMaps maps, string name)
        {
            foreach (Excel.XmlMap map in maps)
            {
                if (string.Equals(name, map.Name))
                {
                    map.Delete();
                    break;
                }
            }
        }

        private string GenExportedXSD(string stnName, PdeExports pdeExports)
        {
            // initialize schema
            string schemaNamespace = "http://www.w3.org/2001/XMLSchema";
            Schema.XmlSchema expSchema = new Schema.XmlSchema();

            // create STN node (root node)
            Schema.XmlSchemaSequence stnElementSequence = CreateSequenceElement(expSchema, stnName);
            Schema.XmlSchemaAll field = null;
            Schema.XmlSchemaAll table = null;

            foreach (DomainExportItem expDomain in pdeExports.Items)
            {
                if (expDomain.Items == null || expDomain.Items.Count == 0)
                    continue;

                foreach (ExportItem expItem in expDomain.Items)
                {
                    if (!expItem.IsUsed)
                        continue;

                    switch (expItem.MapType)
                    {
                        case MapType.SingleCell:
                            #region field
                            if (field == null)
                                field = CreateElement(stnElementSequence, MarkupConstant.PdeExportField);

                            // create field informations
                            Schema.XmlSchemaElement fieldElement = new Schema.XmlSchemaElement();
                            fieldElement.Name = expItem.EncodeTreeNodeName;
                            fieldElement.SchemaTypeName = new System.Xml.XmlQualifiedName(GetXsdDataType(expItem.DataType), schemaNamespace);
                            field.Items.Add(fieldElement);
                            #endregion
                            break;
                        case MapType.Table:
                            #region table
                            if (table == null)
                                table = CreateElement(stnElementSequence, MarkupConstant.PdeExportTable);

                            // create table informations
                            Schema.XmlSchemaElement tableElement = new Schema.XmlSchemaElement();
                            tableElement.MinOccurs = 0;
                            tableElement.MaxOccursString = "unbounded";
                            tableElement.Name = expItem.EncodeTreeNodeName;
                            table.Items.Add(tableElement);

                            Schema.XmlSchemaComplexType tableElementType = new Schema.XmlSchemaComplexType();
                            tableElementType.IsMixed = false;
                            Schema.XmlSchemaAll tableElementAll = new Schema.XmlSchemaAll();
                            tableElementType.Particle = tableElementAll;
                            tableElement.SchemaType = tableElementType;

                            // gen column informations
                            if (expItem.Columns != null && expItem.Columns.Count > 0)
                            {
                                foreach (ColumnExportItem col in expItem.Columns)
                                {
                                    if (!col.IsUsed)
                                        continue;
                                    Schema.XmlSchemaElement colElement = new Schema.XmlSchemaElement();
                                    colElement.Name = col.EncodeTreeNodeName;
                                    colElement.SchemaTypeName = new System.Xml.XmlQualifiedName(GetXsdDataType(col.DataType), schemaNamespace);
                                    tableElementAll.Items.Add(colElement);
                                }
                            }
                            #endregion
                            break;
                        case MapType.Chart:
                            break;
                        default:
                            break;
                    }
                }
            }

            // complie schema
            expSchema.Compile(new Schema.ValidationEventHandler(ValidationEventHandler));

            // write schem to xsd file
            string xsdFilePath = string.Format("{0}\\{1}.xsd", AssetManager.FileAdapter.TemporaryFolderPath, Guid.NewGuid().ToString());
            using (System.IO.FileStream stream = new System.IO.FileStream(xsdFilePath, System.IO.FileMode.Create))
            {
                expSchema.Write(stream);
                stream.Close();
            }

            return xsdFilePath;
        }

        private Schema.XmlSchemaSequence CreateSequenceElement(Schema.XmlSchema parent, string name)
        {
            Schema.XmlSchemaElement element = new Schema.XmlSchemaElement();
            element.Name = name;
            parent.Items.Add(element);
            Schema.XmlSchemaComplexType type = new Schema.XmlSchemaComplexType();
            type.IsMixed = false;
            Schema.XmlSchemaSequence sequence = new Schema.XmlSchemaSequence();
            type.Particle = sequence;
            element.SchemaType = type;

            return sequence;
        }

        private Schema.XmlSchemaAll CreateElement(Schema.XmlSchemaSequence parent, string name)
        {
            Schema.XmlSchemaElement element = new Schema.XmlSchemaElement();
            element.MinOccurs = 0;
            element.MaxOccursString = "unbounded";
            element.Name = name;
            parent.Items.Add(element);

            Schema.XmlSchemaComplexType type = new Schema.XmlSchemaComplexType();
            type.IsMixed = false;
            Schema.XmlSchemaAll elementAll = new Schema.XmlSchemaAll();
            type.Particle = elementAll;
            element.SchemaType = type;

            return elementAll;
        }

        private void ValidationEventHandler(object sender, Schema.ValidationEventArgs args)
        {
        }

        private string GetXsdDataType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return "string";

            return type.Trim();
        }
        #endregion
    }
}
