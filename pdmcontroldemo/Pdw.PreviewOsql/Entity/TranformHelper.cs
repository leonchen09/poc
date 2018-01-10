using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Office.Interop.Word;
using Pdw.PreviewOsql.Entity;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;

namespace Pdw.PreviewOsql.Entity
{
    public class TranformHelper
    {
        public static void RenderFile(XmlDocument xmlDocument, string xslt)
        {
            string fileName = Guid.NewGuid().ToString();
            string outPutFile = Constants.temporaryFolderPath + fileName + FileExtension.Pdf;
            string tempWordXmlFile = Constants.temporaryFolderPath + Guid.NewGuid().ToString() + Constants.xmlExtension;
            string xmlFilePath = string.Empty;
            string xsltFilePath = string.Empty;

            //Create file template
            CreatTempFiles(ref xmlFilePath, ref xsltFilePath, xmlDocument, xslt, fileName);

            //Tranform xslt and xml file
            bool isSuccess = Transform(xmlFilePath, xsltFilePath, tempWordXmlFile);

            if (isSuccess)
            {
                WordHelper.GenerateFile(tempWordXmlFile, RenderSettings.MediaType.Pdf, "", outPutFile);
                File.Delete(tempWordXmlFile);
            }

            //Delete xml and xslt file
            DeleteTempFiles(xmlFilePath, xsltFilePath);

            //Open file
            FileInfo fPdwFile = new FileInfo(outPutFile);
            if (fPdwFile.Exists)
                Process.Start(outPutFile);
        }

        private static bool Transform(string xmlPath, string xslPath, string outputPath)
        {
            try
            {
                System.Xml.Xsl.XslCompiledTransform transformer = new System.Xml.Xsl.XslCompiledTransform();
                transformer.Load(xslPath);
                transformer.Transform(xmlPath, outputPath);

                return true;
            }
            catch (Exception ex)
            {
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }

            return false;
        }

        private static void CreatTempFiles(ref string xmlFilePath, ref string xsltFilePath, XmlDocument xmlDocument, string xsltContent, string fileName)
        {
            if (!Directory.Exists(Constants.temporaryFolderPath))
                throw new Exception("Not exist folder : " + Constants.temporaryFolderPath);
            // 1. create xml file
            xmlFilePath = Constants.temporaryFolderPath + fileName + FileExtension.Xml;
            xmlDocument.Save(xmlFilePath);

            // 2. create xsl file
            xsltFilePath = Constants.temporaryFolderPath + fileName + FileExtension.Xsl;
            FileHelper.CreateFile(xsltFilePath, xsltContent, false);
        }

        private static void DeleteTempFiles(string xmlFilePath, string xslFilePath)
        {
            FileHelper.DeleteFile(xmlFilePath);
            FileHelper.DeleteFile(xslFilePath);
        }
    }
}
