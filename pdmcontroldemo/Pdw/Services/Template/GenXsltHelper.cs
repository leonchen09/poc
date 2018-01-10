
using System.IO;
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Pdw.Managers;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;
using Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;
using System;

namespace Pdw.Services.Template
{
    class GenXsltHelper
    {
        private string PathFormat = "{0}\\{1}{2}";

        /// <summary>
        /// gen xsl from docx with [Word Xml Document] format
        /// </summary>
        /// <param name="key"></param>
        public void GenXsl2007(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

            switch (srvPro.TemplateType)
            {
                case TemplateType.Pdw:
                    GenXslForPdw(key);
                    break;
                case TemplateType.Pdh:
                    GenXslForPdh(key);
                    break;
                case TemplateType.Pdz:
                    GenXslForPdx(key);
                    break;
                case TemplateType.Pdm:
                    GenXslForPdm(key);
                    break;
                default:
                    break;
            }
        }

        #region generate xsl
        private void GenXslForPdw(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

            // create new docx file path
            string folderPath = Path.GetDirectoryName(srvPro.FullDocName);
            string fileName = System.Guid.NewGuid().ToString();
            string newDocxPath = string.Format(PathFormat, folderPath, fileName, FileExtension.Docx);

            // copy docx file
            File.Copy(srvPro.FullDocName, newDocxPath, true);

            // save as word xml document (xml 2007)
            string firstDomainName = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.FirstDomain;
            Core.InternalBookmarkDomain firstDomain = srvPro.Ibm.GetInternalBookmarkDomain(firstDomainName);
            string xmlFilePath = SaveAsWordXmlForPdw(newDocxPath, firstDomain);

            try
            {
                // get word xml document (xml 2007)
                string xmlContent = GetWordXmlDocument(xmlFilePath);

                // get xslt
                Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.XmlContent = xmlContent;
                srvPro.IsXsl2007Format = true;

                MainManager mainManager = new MainManager();
                mainManager.RegisterEvents();
                mainManager.MainService.TemplateService.GetXslContent(key);

                // correct xsl content
                srvPro.XsltString = CorrectXslContent(srvPro.XsltString);
            }
            catch (System.Exception ex) { Core.LogUtils.Log("GenXsl", ex); }
            finally
            {
                // remove temp file
                System.IO.File.Delete(xmlFilePath);
                System.IO.File.Delete(newDocxPath);
            }
        }

        private void GenXslForPdh(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

            // create new docx file path
            string folderPath = Path.GetDirectoryName(srvPro.FullDocName);
            string fileName = System.Guid.NewGuid().ToString();
            string newDocxPath = string.Format(PathFormat, folderPath, fileName, FileExtension.Docx);

            // copy docx file
            File.Copy(srvPro.FullDocName, newDocxPath, true);

            // save as word xml document (xml 2007)
            string firstDomainName = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.FirstDomain;
            Core.InternalBookmarkDomain firstDomain = srvPro.Ibm.GetInternalBookmarkDomain(firstDomainName);
            string xmlFilePath = SaveAsMhtForPdh(newDocxPath, firstDomain);

            try
            {
                srvPro.IsXsl2007Format = true;
                Mht.XslGenerator xslGenerator = new Mht.XslGenerator(xmlFilePath);
                srvPro.XsltString = xslGenerator.GenXsl(key);
            }
            catch (System.Exception ex) { Core.LogUtils.Log("GenXsl", ex); }
            finally
            {
                // remove temp file
                System.IO.File.Delete(xmlFilePath);
                System.IO.File.Delete(newDocxPath);
            }
        }

        private void GenXslForPdx(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

            // create new docx file path
            string folderPath = Path.GetDirectoryName(srvPro.FullDocName);
            string fileName = System.Guid.NewGuid().ToString();
            string newDocxPath = string.Format(PathFormat, folderPath, fileName, FileExtension.Docx);

            // copy docx file
            File.Copy(srvPro.FullDocName, newDocxPath, true);

            // save as word xml document (xml 2007)
            string content = SaveAsTextForPdx(newDocxPath);

            try
            {
                srvPro.IsXsl2007Format = true;
                Pdx.XslGenerator xslGenerator = new Pdx.XslGenerator(content);
                srvPro.XsltString = xslGenerator.GenXsl(key);
            }
            catch (System.Exception ex) { Core.LogUtils.Log("GenXsl", ex); }
            finally
            {
                System.IO.File.Delete(newDocxPath);
            }
        }

        /// <summary>
        /// HACK:FORM CONTROLS - GenXslForPdm
        /// </summary>
        /// <param name="key"></param>
        private void GenXslForPdm(string key)
        {
            ServicesProfile srvPro = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

            string firstDomainName = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo.FirstDomain;
            Core.InternalBookmarkDomain firstDomain = srvPro.Ibm.GetInternalBookmarkDomain(firstDomainName);
            string htmlFileName = SaveAsHtmlForPdm(srvPro.FullDocName, firstDomain);

            try
            {
                srvPro.IsXsl2007Format = true;

                Pdm.XslGenerator generator = new Pdm.XslGenerator(srvPro.Ibm, htmlFileName);

                srvPro.XsltString = generator.GenXsl();
            }
            catch (System.Exception ex)
            {
                //TODO:FORM CONTROLS - EXCEPTION
                Core.LogUtils.Log("GenXsl", ex);
                throw ex;
            }
            finally
            {
                File.Delete(htmlFileName);
            }
        }

        #endregion

        #region helper methods (SaveAsXml, RemoveCustomXmlParts, GetXml, GetXsl)
        private string SaveAsWordXmlForPdw(string docxFilePath, Core.InternalBookmarkDomain firstDomain)
        {
            object missing = System.Type.Missing;
            Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document wDoc = null;

            string xmlFilePath = Path.ChangeExtension(docxFilePath, FileExtension.Xml);

            try
            {
                // open word document
                wApp.Visible = false;
                wDoc = wApp.Documents.Open(docxFilePath, AddToRecentFiles: false);

                // remove custom xml part
                RemoveCustomXmlParts(wDoc);

                // save as word xml document
                Microsoft.Office.Interop.Word.WdSaveFormat fileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatFlatXML;
                MakePdwWatermark(wApp, TemplateType.Pdw, firstDomain);
                wDoc.SaveAs(FileName: xmlFilePath, FileFormat: fileFormat, AddToRecentFiles: false);

                // close document
                ((Microsoft.Office.Interop.Word._Document)wDoc).Close();
            }
            catch { }
            finally
            {
                ((Microsoft.Office.Interop.Word._Application)wApp).Quit(missing, missing, missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wDoc);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wApp);
            }

            return xmlFilePath;
        }

        private string SaveAsMhtForPdh(string docxFilePath, Core.InternalBookmarkDomain firstDomain)
        {
            object missing = System.Type.Missing;
            Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document wDoc = null;

            string xmlFilePath = Path.ChangeExtension(docxFilePath, FileExtension.Xml);

            try
            {
                // open word document
                wApp.Visible = false;
                wDoc = wApp.Documents.Open(docxFilePath, AddToRecentFiles: false);

                // remove custom xml part
                RemoveCustomXmlParts(wDoc);

                // put watermark tag
                MakePdwWatermark(wApp, TemplateType.Pdh, firstDomain);

                // pre-process
                ProcessHeaderFooter(wApp, wDoc);
                ProcessWaterMark(wDoc);
                ProcessBookmarkForMhtFile(wDoc);

                // save as word xml document
                Microsoft.Office.Interop.Word.WdSaveFormat fileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatFlatXML;
                xmlFilePath = Path.ChangeExtension(xmlFilePath, FileExtension.Mht);
                fileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatWebArchive;

                wDoc.WebOptions.Encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
                wDoc.SaveAs(FileName: xmlFilePath, FileFormat: fileFormat,
                    Encoding: Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8, AddToRecentFiles: false);

                // close document
                ((Microsoft.Office.Interop.Word._Document)wDoc).Close();
            }
            catch { }
            finally
            {
                ((Microsoft.Office.Interop.Word._Application)wApp).Quit(missing, missing, missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wDoc);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wApp);
            }

            return xmlFilePath;
        }

        private string SaveAsTextForPdx(string docxFilePath)
        {
            string content = string.Empty;
            object missing = System.Type.Missing;
            Microsoft.Office.Interop.Word.Application wApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document wDoc = null;

            try
            {
                // open word document
                wApp.Visible = false;
                wDoc = wApp.Documents.Open(docxFilePath, AddToRecentFiles: false);

                // remove custom xml part
                RemoveCustomXmlParts(wDoc);

                // process bookmark
                ProcessBookmarkForMhtFile(wDoc);

                content = Core.MarkupUtilities.GetRangeText(wDoc.Content);

                // close document
                ((Microsoft.Office.Interop.Word._Document)wDoc).Close();
            }
            catch { }
            finally
            {
                ((Microsoft.Office.Interop.Word._Application)wApp).Quit(missing, missing, missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wDoc);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wApp);
            }

            return content;
        }

        private string SaveAsHtmlForPdm(string filePath, Core.InternalBookmarkDomain firstDomain)
        {
            string tempCopyFileName = Path.ChangeExtension(Path.GetTempFileName(), FileExtension.Docx);
            string tempHtmlName = Path.ChangeExtension(Path.GetTempFileName(), FileExtension.Html);

            Word.Application word = null;
            Word.Document newDoc = null;

            File.Copy(filePath, tempCopyFileName, true);

            try
            {
                word = new Microsoft.Office.Interop.Word.Application();

                word.DefaultWebOptions().Encoding = MsoEncoding.msoEncodingUTF8;

                newDoc = word.Documents.Open(tempCopyFileName, Visible: false, AddToRecentFiles: false, Encoding: MsoEncoding.msoEncodingUTF8);

                RemoveCustomXmlParts(newDoc);
                MakePdwWatermark(word, TemplateType.Pdm, firstDomain);

                newDoc.SaveAs(tempHtmlName, Word.WdSaveFormat.wdFormatFilteredHTML, AddToRecentFiles: false, Encoding: MsoEncoding.msoEncodingUTF8);
                ((Word._Document)newDoc).Close();
            }
            finally
            {
                ((Word._Application)word).Quit(Type.Missing, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(word);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(newDoc);

                File.Delete(tempCopyFileName);
            }

            return tempHtmlName;
        }

        #region wartermark
        private const string WaterMarkTextPrefix = "PowerPlusWaterMarkObject";
        private const string WaterMarkPicturePrefix = "WordPictureWatermark";
        private static string PdwWaterMarkName = WaterMarkTextPrefix + "1";
        private void MakePdwWatermark(Microsoft.Office.Interop.Word.Application wApp, TemplateType templateType,
            Core.InternalBookmarkDomain firstDomain)
        {
            try
            {
                Microsoft.Office.Interop.Word.Selection Selection = wApp.Selection;
                Microsoft.Office.Interop.Word.Shape wmShape;
                Selection.Select();

                // set focus on header to put watermark
                wApp.ActiveWindow.ActivePane.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;

                //Create the watermar shape
                wmShape = Selection.HeaderFooter.Shapes.AddTextEffect(
                            Microsoft.Office.Core.MsoPresetTextEffect.msoTextEffect1,
                            GenPdwWatermarkValue(templateType, firstDomain),
                            "Times New Roman", 1,
                            Microsoft.Office.Core.MsoTriState.msoFalse,
                            Microsoft.Office.Core.MsoTriState.msoFalse,
                            0, 0);

                //Set all of the attributes of the watermark
                wmShape.Select();
                wmShape.Name = PdwWaterMarkName;
                wmShape.TextEffect.NormalizedHeight = Microsoft.Office.Core.MsoTriState.msoFalse;
                wmShape.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
                wmShape.Fill.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
                wmShape.Fill.Solid();
                wmShape.Fill.ForeColor.RGB = (int)Microsoft.Office.Interop.Word.WdColor.wdColorGray25;
                wmShape.Fill.Transparency = 0.5f;
                wmShape.Rotation = GenPdwWatermarkRotation(templateType);
                wmShape.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
                wmShape.Height = GenPdwWatermarkHeight(wApp, templateType);
                wmShape.Width = wApp.InchesToPoints(7.64f);
                wmShape.WrapFormat.AllowOverlap = -1; //true
                wmShape.WrapFormat.Side = Microsoft.Office.Interop.Word.WdWrapSideType.wdWrapBoth;
                wmShape.WrapFormat.Type = Microsoft.Office.Interop.Word.WdWrapType.wdWrapNone;  //3
                wmShape.RelativeHorizontalPosition =
                            Microsoft.Office.Interop.Word.WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
                wmShape.RelativeVerticalPosition =
                            Microsoft.Office.Interop.Word.WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
                wmShape.Left = (float)Microsoft.Office.Interop.Word.WdShapePosition.wdShapeCenter;
                wmShape.Top = (float)Microsoft.Office.Interop.Word.WdShapePosition.wdShapeCenter;

                //set focus back to document
                wApp.ActiveWindow.ActivePane.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;
            }
            catch { }
        }

        private string GenPdwWatermarkValue(TemplateType templateType, Core.InternalBookmarkDomain firstDomain)
        {
            switch (templateType)
            {
                case TemplateType.Pdw:
                    return "{" + firstDomain.RootTablePath + "/@" + ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark + "}";
                case TemplateType.Pdh: // update XslGenerator
                    string bmName = System.DateTime.Now.ToString(Pdw.Core.ProntoMarkup.BookmarkKeyFormat);
                    bmName = Core.MarkupUtilities.GenKeyForXslTag(bmName, Core.XsltType.Select, true);
                    string bmValue = ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwWatermark;
                    firstDomain.InternalBookmarkItems.Add(new Core.InternalBookmarkItem(bmName, bmValue, bmValue, string.Empty));
                    return GenBookmarkText(bmName, bmValue);
                default:
                    return "";
            }
        }

        private float GenPdwWatermarkRotation(TemplateType templateType)
        {
            switch (templateType)
            {
                case TemplateType.Pdw:
                    return 315.0f;
                case TemplateType.Pdh:
                    return 315.0f;
                default:
                    return 315.0f;
            }
        }

        private float GenPdwWatermarkHeight(Microsoft.Office.Interop.Word.Application wApp, TemplateType templateType)
        {
            switch (templateType)
            {
                case TemplateType.Pdw:
                    return wApp.InchesToPoints(2.82f);
                case TemplateType.Pdh:
                    return 200;
                default:
                    return wApp.InchesToPoints(2.82f);
            }
        }

        private void ProcessWaterMark(Microsoft.Office.Interop.Word.Document wDoc)
        {
            foreach (Microsoft.Office.Interop.Word.Shape shape in wDoc.Shapes)
            {
                if (shape.Name.StartsWith(WaterMarkTextPrefix) || shape.Name.StartsWith(WaterMarkPicturePrefix))
                {
                    // top
                    float startOfPage = wDoc.PageSetup.TopMargin - wDoc.PageSetup.HeaderDistance;
                    float endOfPage = wDoc.PageSetup.BottomMargin - wDoc.PageSetup.FooterDistance;
                    float shapeHeight = shape.Height;
                    shape.IncrementTop((startOfPage + endOfPage - shapeHeight) / 2 - 100);

                    // left
                    float leftOfPage = wDoc.PageSetup.LeftMargin;
                    float rightOfPage = wDoc.PageSetup.RightMargin;
                    float shapeWidth = shape.Width;
                    shape.IncrementLeft(-(leftOfPage + rightOfPage - shapeWidth) / 2);
                }
            }
        }
        #endregion

        private void ProcessHeaderFooter(Microsoft.Office.Interop.Word.Application wApp, Microsoft.Office.Interop.Word.Document wDoc)
        {
            // copy header
            wApp.ActiveWindow.ActivePane.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdPrintView;
            wApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;
            Microsoft.Office.Interop.Word.Range wHeaderRange = wApp.Selection.HeaderFooter.Range;
            ProcessBookmarks(wHeaderRange.Bookmarks, wDoc);
            wHeaderRange.CopyAsPicture();

            // paste header
            wApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;
            wApp.Selection.HomeKey(Microsoft.Office.Interop.Word.WdUnits.wdStory);
            Microsoft.Office.Interop.Word.Paragraph wHeader = wDoc.Content.Paragraphs.Add();
            wApp.Selection.PasteAndFormat(Microsoft.Office.Interop.Word.WdRecoveryType.wdPasteDefault);

            // copy footer
            wApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter;
            Microsoft.Office.Interop.Word.Range wFooterRange = wApp.Selection.HeaderFooter.Range;
            ProcessBookmarks(wFooterRange.Bookmarks, wDoc);
            wFooterRange.CopyAsPicture();

            // paste footer
            wApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;
            wApp.Selection.EndKey(Microsoft.Office.Interop.Word.WdUnits.wdStory);
            Microsoft.Office.Interop.Word.Paragraph wFooter = wDoc.Content.Paragraphs.Add();
            wApp.Selection.PasteAndFormat(Microsoft.Office.Interop.Word.WdRecoveryType.wdPasteDefault);
        }

        /// <summary>
        /// remove custom xml part (internal bookmark, osql, checksum, comment)
        /// </summary>
        /// <param name="wDoc"></param>
        private void RemoveCustomXmlParts(Microsoft.Office.Interop.Word.Document wDoc)
        {
            try
            {
                Pdwx.Services.WordHeper.RemoveProtectPassword(wDoc, ProtectLevel.All);

                // accept all changes
                try
                {
                    if (wDoc.TrackRevisions)
                        wDoc.AcceptAllRevisions();
                }
                catch { }

                List<string> xmlPartIds = new List<string>();
                if (wDoc.CustomXMLParts != null)
                {
                    foreach (Microsoft.Office.Core.CustomXMLPart xmlPart in wDoc.CustomXMLParts)
                    {
                        if (!xmlPart.BuiltIn)
                            xmlPartIds.Add(xmlPart.Id);
                    }
                }
                foreach (string xmlPartId in xmlPartIds)
                {
                    Microsoft.Office.Core.CustomXMLPart oldXmlPart = wDoc.CustomXMLParts.SelectByID(xmlPartId);

                    if (oldXmlPart != null)
                        oldXmlPart.Delete();
                }

                if (wDoc.Bookmarks != null)
                {
                    foreach (Microsoft.Office.Interop.Word.Bookmark wBm in wDoc.Bookmarks)
                    {
                        if (Core.MarkupUtilities.IsProntoDocCommentBookmark(wBm.Name))
                            wBm.Range.Text = string.Empty; // remove the bookmark
                    }
                }
            }
            catch { }
        }

        private string GetWordXmlDocument(string xmlFilePath)
        {
            string content = string.Empty;
            if (System.IO.File.Exists(xmlFilePath))
            {
                if (!FileIsUsing(xmlFilePath))
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(xmlFilePath))
                    {
                        content = reader.ReadToEnd();
                        reader.Close();
                    }
                }
            }

            return content;
        }

        private bool FileIsUsing(string filePath)
        {
            bool isUsing = true;
            int checkingTime = 0;
            int maxCheckingTime = 5;

            while (isUsing)
            {
                if (checkingTime > maxCheckingTime)
                    break;
                try
                {
                    using (System.IO.StreamReader tryReader = new System.IO.StreamReader(filePath))
                    {
                        tryReader.ReadToEnd();
                        tryReader.Close();
                    }
                    isUsing = false;
                    System.Threading.Thread.Sleep(1000);
                    checkingTime++;
                }
                catch
                {
                    isUsing = true;
                }
            }

            return isUsing;
        }

        private string CorrectXslContent(string xmlString)
        {
            string xslString = string.Empty;

            if (!string.IsNullOrEmpty(xmlString))
            {
                // 1. replace header
                string xmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";
                string xslHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">" +
                    "<xsl:output method=\"xml\" indent=\"yes\" encoding=\"UTF-8\"/>" + // using for big image (process break line)
                    "<xsl:template match=\"/" + ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwDataRootName + "\">";
                xslString = xmlString.Replace(xmlHeader, xslHeader);

                // remove created date
                xslString = xslString.Replace("<dcterms:created", "<!-- dcterms:created");
                xslString = xslString.Replace("</dcterms:created>", "</dcterms:created -->");

                // remove modified date
                xslString = xslString.Replace("<dcterms:modified", "<!-- dcterms:modified");
                xslString = xslString.Replace("</dcterms:modified>", "</dcterms:modified -->");

                // replace { and } by {{ and }} in properties section (pkg:part pkg:name="/docProps/custom.xml")
                xslString = xslString.Replace("fmtid=\"{", "fmtid=\"{{"); // open property id
                xslString = xslString.Replace("}\" pid", "}}\" pid"); // end property id
                xslString = xslString.Replace("<vt:lpwstr>{", "<vt:lpwstr>{{"); // open property value
                xslString = xslString.Replace("}</vt:lpwstr>", "}}</vt:lpwstr>"); // end property value

                xslString = xslString.Replace("<ds:datastoreItem ds:itemID=\"{", "<ds:datastoreItem ds:itemID=\"{{"); // open property value
                xslString = xslString.Replace("}\" xmlns:ds=\"http://schemas.openxmlformats.org/officeDocument/2006/customXml\">",
                    "}}\" xmlns:ds=\"http://schemas.openxmlformats.org/officeDocument/2006/customXml\">"); // end property value

                // append footer
                string xslFooter = "</xsl:template></xsl:stylesheet>";
                xslString = xslString + xslFooter;
            }

            return xslString;
        }
        #endregion

        #region process mht, pdx file
        private void ProcessBookmarkForMhtFile(Microsoft.Office.Interop.Word.Document wDoc)
        {
            ProcessBookmarks(wDoc.Bookmarks, wDoc);
        }

        private void ProcessBookmarks(Microsoft.Office.Interop.Word.Bookmarks wBookmarks,
            Microsoft.Office.Interop.Word.Document wDoc)
        {
            if (wBookmarks == null)
                return;

            foreach (Microsoft.Office.Interop.Word.Bookmark bm in wBookmarks)
            {
                string name = bm.Name;

                if (name.EndsWith(Core.ProntoMarkup.KeyImage)) // is image then we only remove bookmark
                {
                    if (wDoc != null && wDoc.InlineShapes != null)
                    {
                        foreach (Microsoft.Office.Interop.Word.InlineShape inline in wDoc.InlineShapes)
                        {
                            if (inline.AlternativeText.StartsWith(Core.MarkupUtilities.CreateInlineKey(name)))
                            {
                                string value = Core.MarkupUtilities.GetBizName(inline.AlternativeText);
                                value = Core.MarkupUtilities.GetOriginalBizName(name, value);
                                value = value.Replace(Core.MarkupConstant.Space, Core.MarkupConstant.SpaceInBookmark);
                                inline.AlternativeText = GenBookmarkText(name, value);
                                break;
                            }
                        }

                        bm.Delete();
                    }
                }
                else
                {
                    string value = Core.MarkupUtilities.GetRangeText(bm.Range);
                    value = Core.MarkupUtilities.GetOriginalBizName(name, value);
                    value = value.Replace(Core.MarkupConstant.Space, Core.MarkupConstant.SpaceInBookmark);
                    bm.Range.Text = GenBookmarkText(name, value); // replace and remove bookmark
                }
            }
        }

        private string GenBookmarkText(string bmName, string bmValue)
        {
            string markFormat = Core.MarkupConstant.PrefixBookmark + "{0}#{1}_pdw";

            return string.Format(markFormat, bmName, bmValue);
        }
        #endregion
    }
}