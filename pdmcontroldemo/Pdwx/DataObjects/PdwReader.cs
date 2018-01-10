using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Microsoft.Office.Interop.Word;
using ProntoDoc.Framework.CoreObject.PdwxObjects;

namespace Pdwx.DataObjects
{
    public class PdwReader
    {
        #region private properties
        private string _filePath;
        private string _guid;

        private string _checksum;
        private string _internalBm;
        private string _osql;
        private string _xslt;

        private ChecksumInfo _checksumInfo;

        /// <summary>
        /// &lt;InternalBookmark xmlns:xsi
        /// </summary>
        private const string MarkupInternal = "<InternalBookmark xmlns:xsi";
        #endregion

        #region public properties
        /// <summary>
        /// get pdw file path
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
        }

        /// <summary>
        /// get Guid (file name without extension) of extracted files
        /// </summary>
        public string Guid { get { return _guid; } }

        /// <summary>
        /// get checksum string (return null if can not get checksum string or not using ExtracAll method)
        /// </summary>
        public ChecksumInfo CheckSum
        {
            get
            {
                return _checksumInfo;
            }
        }

        /// <summary>
        /// get osql content
        /// </summary>
        public List<string> Osql
        {
            get
            {
                List<string> osql = new List<string>();
                try
                {
                    string[] osqlArr = _osql.Split(new char[] { '\n' });
                    for (int index = 0; index < osqlArr.Length; index++)
                        osql.Add(osqlArr[index]);
                }
                catch
                {
                    osql.Add(_osql);
                }

                return osql;
            }
        }

        /// <summary>
        /// get full osql string
        /// </summary>
        public string FullOsql { get { return _osql; } }

        /// <summary>
        /// get xsl content
        /// </summary>
        public string Xsl
        {
            get { return _xslt; }
        }
        #endregion

        #region constrcutor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pdwFilePath"></param>
        /// <exception>FileNotExistException: the pdw file path is not exist</exception>
        /// <exception>NotIsOpenXmlFormatException: the pdw file not has openxml format</exception>
        /// <exception>NotIsPdwFormatException: the pdw file path not has pdw format</exception>
        public PdwReader(string pdwFilePath)
        {
            if (!File.Exists(pdwFilePath))
                throw new FileNotExistException();

            try
            {
                _filePath = pdwFilePath;
                _guid = System.Guid.NewGuid().ToString();

                _checksum = string.Empty;
                _internalBm = string.Empty;
                _osql = string.Empty;
                _xslt = string.Empty;

                _checksumInfo = null;

                if (!ReadCustomXmlparts())
                    throw new NotIsPdwFormatException();

                SetChecksumInfo();
            }
            catch
            {
                throw new NotIsOpenXmlFormatException();
            }
        }

        #region helper methods
        private bool ReadCustomXmlparts()
        {
            object missing = System.Type.Missing;
            Application wApp = new Application();
            int partNumber = 0;

            try
            {
                wApp.Visible = false;
                wApp.Documents.Open(_filePath);

                foreach (Microsoft.Office.Core.CustomXMLPart xmlPart in wApp.ActiveDocument.CustomXMLParts)
                {
                    if (!xmlPart.BuiltIn && ReadCustomXmlPart(xmlPart))
                        partNumber++;
                }
            }
            catch { }
            finally
            {
                ((_Application)wApp).Quit(missing, missing, missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wApp);
            }

            return (partNumber == 4);
        }

        private bool ReadCustomXmlPart(Microsoft.Office.Core.CustomXMLPart xmlPart)
        {
            if (xmlPart != null)
            {
                string content = xmlPart.XML;
                if (content.Contains(MarkupInternal))
                {
                    _internalBm = content;
                    return true;
                }
                else
                    return ProcessXmlObject(content);
            }

            return false;
        }

        private bool ProcessXmlObject(string serializedXmlObject)
        {
            try
            {
                XmlObject xmlObject = Utilities.Deserialize<XmlObject>(serializedXmlObject);

                switch (xmlObject.ContentType)
                {
                    case ContentType.Checksum:
                        if (string.IsNullOrEmpty(_checksum))
                            _checksum = xmlObject.Content;
                        return true;
                    case ContentType.Osql:
                        if (string.IsNullOrEmpty(_osql))
                            _osql = xmlObject.Content;
                        return true;
                    case ContentType.Xslt:
                        if (string.IsNullOrEmpty(_xslt))
                            _xslt = xmlObject.Content;
                        return true;
                    default:
                        return false;
                }
            }
            catch { }

            return false;
        }

        private void SetChecksumInfo()
        {
            try
            {
                if (_checksumInfo == null)
                {
                    _checksumInfo = Utilities.Deserialize<ChecksumInfo>(_checksum);
                    _checksumInfo.InternalBookmark = _internalBm;
                    _checksumInfo.Osql = _osql;
                }
            }
            catch { }
        }
        #endregion
        #endregion

        #region public methods

        #region extract docx file (ExtractDocx)

        /// <summary>
        /// Extract docx file
        /// </summary>
        /// <param name="outputFolder"></param>
        /// <param name="isKeppPdwFormat">true if wnat to keep internal, osql, checksum, xsl</param>
        public string ExtractDocx(string outputFolder, bool isKeepPdwFormat = false)
        {
            if (!outputFolder.EndsWith("\\"))
                outputFolder += "\\";

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            string docxFilePath = string.Format(PdwxConstants.FilePathFormat, outputFolder, _guid, FileExtension.Docx);

            if (isKeepPdwFormat)
                File.Copy(_filePath, docxFilePath, true);
            else
            {
                Exception ex = null;
                RemovePdwFormat(docxFilePath, ref ex);

                if (ex != null)
                    throw ex;
            }

            return docxFilePath;
        }

        #region helper methods (RemovePdwFormat)

        private void RemovePdwFormat(string docxFilePath, ref Exception ex)
        {
            object missing = System.Type.Missing;
            Application wApp = new Application();

            try
            {
                wApp.Visible = false;
                wApp.Documents.Open(_filePath);
                Services.WordHeper.RemoveProtectPassword(wApp.ActiveDocument, ProntoDoc.Framework.CoreObject.PdwxObjects.ProtectLevel.All);
                List<string> partIds = new List<string>();
                foreach (Microsoft.Office.Core.CustomXMLPart xmlPart in wApp.ActiveDocument.CustomXMLParts)
                {
                    if (!xmlPart.BuiltIn)
                        partIds.Add(xmlPart.Id);
                }
                foreach (string partId in partIds)
                {
                    Microsoft.Office.Core.CustomXMLPart xmlPart = wApp.ActiveDocument.CustomXMLParts.SelectByID(partId);
                    if (xmlPart != null)
                        xmlPart.Delete();
                }

                wApp.ActiveDocument.SaveAs(docxFilePath);
            }
            catch (Exception exception) { ex = exception; }
            finally
            {
                ((_Application)wApp).Quit(missing, missing, missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wApp);
            }
        }
        #endregion

        #endregion

        #region extract all file: docx, osql, xsl

        /// <summary>
        /// extract all file (without checksum file)
        /// </summary>
        /// <param name="outputFolder">output folder. (eg: D:\\data\\)</param>
        /// <param name="isKeppPdwFormat">true if wnat to keep internal, osql, checksum, xsl</param>
        public void ExtractAll(string outputFolder, bool isKeppPdwFormat = false)
        {
            if (!outputFolder.EndsWith("\\"))
                outputFolder += "\\";

            // create docx
            ExtractDocx(outputFolder, isKeppPdwFormat);

            // create xsl file
            string xsltFilePath = string.Format(PdwxConstants.FilePathFormat, outputFolder, _guid, FileExtension.Xsl);
            Utilities.WriteFile(xsltFilePath, _xslt);

            // create osql
            string osqlFilePath = string.Format(PdwxConstants.FilePathFormat, outputFolder, _guid, FileExtension.Osql);
            Utilities.WriteFile(osqlFilePath, Osql);
        }

        #endregion

        //public ImageFieldCollection GetImageFields()
        //{
        //    ImageFieldCollection imgFields = new ImageFieldCollection();
        //    imgFields.Items = ImageFields();

        //    return imgFields;
        //}
        #endregion

        #region image fields
        ///// <summary>
        ///// key: NodeName or TableName
        ///// value: AttributeName or FieldName
        ///// </summary>
        ///// <returns></returns>
        //private List<ImageField> ImageFields()
        //{
        //    List<ImageField> imageFields = new List<ImageField>();

        //    if (!string.IsNullOrEmpty(_internalBm))
        //    {
        //        try
        //        {
        //            XmlDocument xDoc = new XmlDocument();
        //            xDoc.LoadXml(_internalBm);
        //            XmlNodeList ibms = xDoc.GetElementsByTagName(Constants.IbmXml.NodeName);

        //            foreach (XmlNode ibm in ibms)
        //            {
        //                string key = ibm.Attributes[Constants.IbmXml.AttKey].Value;

        //                if (IsImageField(key))
        //                {
        //                    string bizName = ibm.Attributes[Constants.IbmXml.AttBizName].Value;
        //                    bizName = RemoveChars(bizName, 1, 2);

        //                    string tableName = ibm.Attributes[Constants.IbmXml.AttTecName].Value;
        //                    tableName = tableName.Substring(0, tableName.IndexOf(Constants.IbmXml.SplitChar));

        //                    imageFields.Add(new ImageField(tableName, bizName));
        //                }
        //            }
        //        }
        //        catch { }
        //    }

        //    return imageFields;
        //}

        //private bool IsImageField(string key)
        //{
        //    if (!string.IsNullOrEmpty(key) && key.EndsWith(Constants.IbmXml.EndImage))
        //        return true;

        //    return false;
        //}

        //private string RemoveChars(string value, int left, int right)
        //{
        //    if (!string.IsNullOrEmpty(value) && value.Length > left)
        //    {
        //        string temp = value.Remove(0, left);
        //        if (temp.Length > right && right > 0)
        //            temp = temp.Remove(temp.Length - right);

        //        return temp;
        //    }

        //    return value;
        //}
        #endregion
    }
}