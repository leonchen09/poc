using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Office.Core;
using System.IO;
using System.Text.RegularExpressions;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;
using Pdw.Services;
using System.Xml;

namespace PdwPlugin
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //Application.DocumentBeforeClose += new Word.ApplicationEvents4_DocumentBeforeCloseEventHandler(Application_DocumentBeforeClose);
            Application.DocumentOpen += new Word.ApplicationEvents4_DocumentOpenEventHandler(ThisDocument_DocumentOpen);
        }
        private void Application_DocumentBeforeClose(Word.Document Doc, ref bool Cancel)
        {

        }
        private void ThisDocument_DocumentOpen(Word.Document Doc)
        {
            string fullFileName = Doc.FullName;
            if (fullFileName.ToLower().EndsWith(".pdw"))
            {
                if (Doc.ReadOnly)//cannot modify the template, so just return.
                    return;
                string path = Doc.Path;
                Word.Application word = Doc.Application;
                //read all the excel file name which linked to this document.
                string tempFolder = Pdw.AssetManager.FileAdapter.TemporaryFolderPath + "\\" + Doc.Name;

                //Get old imported excel file name and prepare new file name.
                Dictionary<string, string> excelFiles = null;//PdeService.ReadImportedFileName(Doc);
                if (excelFiles == null || excelFiles.Count < 1)//there is no excel in the document, just return.
                {
                    return;
                }
                //word.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone;
                Doc.Close(Word.WdSaveOptions.wdDoNotSaveChanges);

                //Unzip the .pdw file to tempory folder.
                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(fullFileName, tempFolder, "");

                //store new excel file name which has been unzip to temporty folder.
                Dictionary<string, string> newFileNames = new Dictionary<string, string>();
                //Prepare new excel file name and save them into dictionary with same key of old file name dictionary.
                Dictionary<string, string>.Enumerator em = excelFiles.GetEnumerator();
                while (em.MoveNext())
                {
                    newFileNames.Add(em.Current.Key, Pdw.AssetManager.FileAdapter.TemporaryFolderPath + "\\" + System.Guid.NewGuid() + ".xlsx");
                }

                //replace all the excel path which used in chart.
                replaceChartFile(tempFolder, excelFiles, newFileNames);

                //Change the excel path in document. Replace all the excle file which used in table copy.
                replaceDocumentFile(tempFolder + "\\word\\document.xml", excelFiles, newFileNames);

                //Replace excel path in custom properties.
                replaceCustomProperties(tempFolder, excelFiles, newFileNames);

                //Decode the excel file to current folder.
                unZipExcelFile(tempFolder, excelFiles, newFileNames);

                //create new document with separable xml files
                fastZip.CreateZip(fullFileName, tempFolder, true, "");
                
                Doc = word.Documents.Open(fullFileName);

                //Remove all the tempory file. 
                Directory.Delete(tempFolder,true);
            }
        }

        private void replaceDocumentFile(string file, Dictionary<string, string> oldFileNames, Dictionary<string, string> newFileNames)
        {
            StreamReader sr = new StreamReader(file);
            string fileContent = sr.ReadToEnd();
            sr.Close();
            sr = null;

            Dictionary<string, string>.Enumerator em = oldFileNames.GetEnumerator();
            while (em.MoveNext())
            {
                string oldFileName = null;
                string newFileName = null;
                oldFileName = em.Current.Value;
                newFileNames.TryGetValue(em.Current.Key, out newFileName);
                oldFileName = oldFileName.Replace("\\", "\\\\");
                newFileName = newFileName.Replace("\\", "\\\\");
                fileContent = fileContent.Replace(oldFileName, newFileName);
            }
            StreamWriter sw = new StreamWriter(file);
            sw.Write(fileContent);
            sw.Close();
            sw = null;
        }


        private void replaceChartFile(string tempFolder, Dictionary<string, string> oldFileNames, Dictionary<string, string> newFileNames)
        {
            string[] chartFilenames = null;
            if (Directory.Exists(tempFolder + "\\word\\charts\\_rels\\"))
                chartFilenames = Directory.GetFileSystemEntries(tempFolder + "\\word\\charts\\_rels\\");
            foreach (string file in chartFilenames)
            {
                StreamReader sr = new StreamReader(file);
                string fileContent = sr.ReadToEnd();
                sr.Close();
                sr = null;
                Dictionary<string, string>.Enumerator em = oldFileNames.GetEnumerator();
                while (em.MoveNext())
                {
                    string oldFileName = null;
                    string newFileName = null;
                    oldFileName = em.Current.Value;
                    newFileNames.TryGetValue(em.Current.Key, out newFileName);
                    fileContent = fileContent.Replace(oldFileName, newFileName);
                }
                StreamWriter sw = new StreamWriter(file);
                sw.Write(fileContent);
                sw.Close();
                sw = null;
            }
        }

        private void replaceCustomProperties(string tempFolder, Dictionary<string, string> oldFileNames, Dictionary<string, string> newFileNames)
        {
            string file = tempFolder + "\\docProps\\custom.xml";
            StreamReader sr = new StreamReader(file);
            string fileContent = sr.ReadToEnd();
            sr.Close();
            sr = null;

            Dictionary<string, string>.Enumerator em = oldFileNames.GetEnumerator();
            while (em.MoveNext())
            {
                string oldFileName = null;
                string newFileName = null;
                oldFileName = em.Current.Value;
                newFileNames.TryGetValue(em.Current.Key, out newFileName);
                fileContent = fileContent.Replace(oldFileName, newFileName);
            }
            StreamWriter sw = new StreamWriter(file);
            sw.Write(fileContent);
            sw.Close();
            sw = null;
        }

        private void unZipExcelFile(string tempFolder, Dictionary<string, string> oldFileNames, Dictionary<string, string> newFileNames)
        {
            string[] customeXmlFiles = Directory.GetFileSystemEntries(tempFolder + "\\customXml\\", "*.xml");
            Regex custXmlRegex = new Regex(@"item\d*.xml");//we need item*.xml only, do not need imemProps*.xml.
            foreach (string custXmlFile in customeXmlFiles)
            {
                if (custXmlRegex.IsMatch(custXmlFile))
                {
                    StreamReader sr = new StreamReader(custXmlFile);
                    string content = sr.ReadToEnd();
                    sr.Close();
                    sr = null;
                    XmlObject custXml = ObjectSerializeHelper.Deserialize<XmlObject>(content);
                    if (custXml.ContentType == ContentType.ImportedPde)
                    {
                        PdeContent pdeImported = ObjectSerializeHelper.Deserialize<PdeContent>(custXml.Content);
                        string importedFileName = pdeImported.FullFileName;
                        string oldFileName = null;
                        string newFileName = null;
                        //get new excel file name which has been updated into the document.
                        Dictionary<string, string>.Enumerator em = oldFileNames.GetEnumerator();
                        while (em.MoveNext())
                        {
                            oldFileName = null;
                            newFileName = null;
                            oldFileName = em.Current.Value;
                            if (pdeImported.FullFileName.Equals(oldFileName))
                            {
                                newFileNames.TryGetValue(em.Current.Key, out newFileName);
                                break;
                            }
                        }
                        FileHelper.ExcelFromBase64(pdeImported.FileContent, newFileName);

                        //replace file name with new name.
                        //content = content.Replace(oldFileName, newFileName);
                        //StreamWriter sw = new StreamWriter(custXmlFile);
                        //sw.Write(content);
                        //sw.Close();
                        //sw = null;
                        pdeImported.FullFileName = newFileName;
                        custXml.Content = ObjectSerializeHelper.SerializeToString<PdeContent>(pdeImported);
                        content = ObjectSerializeHelper.SerializeToString<XmlObject>(custXml);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(content);
                        xmlDoc.Save(custXmlFile);
                    }

                }
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
