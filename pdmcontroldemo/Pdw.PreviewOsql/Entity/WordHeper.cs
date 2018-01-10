using Microsoft.Office.Interop.Word;
using ProntoDoc.Framework.CoreObject.PdwxObjects;
using ProntoDoc.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Pdw.PreviewOsql.Entity
{
    public class WordAppProvider
    {
        private static object _locker = new object();
        private static Application _wApp = null;
        // 1. initialize word application
        protected static Application wApp
        {
            get
            {
                lock (_locker)
                {
                    if (_wApp == null)
                    {
                        _wApp = new Application();
                        wApp.Visible = false;
                    }

                    try
                    {
                        int o = _wApp.Documents.Count;
                    }
                    catch
                    {
                        _wApp = new Application();
                        wApp.Visible = false;
                    }
                    return _wApp;
                }
            }
        }
      
        protected static Document GetWDoc(string FileName = null, WdOpenFormat? Format = null, string XMLTransform = null)
        {
            if (FileName == null && Format == null && XMLTransform == null)
                return wApp.Documents.Add();

            if (FileName != null && Format == null && XMLTransform == null)
                return wApp.Documents.Open(FileName: FileName);

            if (FileName != null && Format != null && XMLTransform == null)
                return wApp.Documents.Open(FileName: FileName, Format: Format);

            if (FileName != null && Format != null && XMLTransform != null)
                return wApp.Documents.Open(FileName: FileName, Format: Format, XMLTransform: XMLTransform);
       
            return null;
        }

        protected static void CloseWDoc(Document wDoc, bool isRemove = false)
        {
            object missing = System.Type.Missing;

            // 1. close document
            if (wDoc != null)
            {
                if (!wDoc.Saved)
                    wDoc.Saved = true;
                ((_Document)wDoc).Close(missing, missing, missing);
            }

            //2. Remove from Document collection
            try
            {
                if (isRemove)
                    wApp.Documents.Close(RouteDocument: wDoc);
            }
            catch (Exception ex)
            {
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }

            // 3. release com object
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wDoc);
            }
            catch (Exception ex)
            {
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }
        }
    }

    public class WordHeper : WordAppProvider
    {
        private const string ProtectBookmarkPassword = "ngocbv_bookmark";
        private const string ProtectDocumentPassword = "ngocbv_fulldoc";

        #region public methods
        /// <summary>
        /// remove protect password in document
        /// </summary>
        /// <param name="wDoc">Word document</param>
        /// <param name="level">Level of protected to remove</param>
        /// <returns>Level of removed protection (0: bookmark, 1: document, other: -1)</returns>
        internal static ProtectLevel RemoveProtectPassword(Document wDoc, ProtectLevel level)
        {
            ProtectLevel removedLevel = ProtectLevel.None;

            if (wDoc.ProtectionType == WdProtectionType.wdAllowOnlyReading)
            {
                object password = ProtectBookmarkPassword;
                if (level == ProtectLevel.Bookmark || level == ProtectLevel.All)
                {
                    try
                    {
                        wDoc.Unprotect(ref password);
                        removedLevel = ProtectLevel.Bookmark;
                    }
                    catch (Exception ex)
                    {
                        SxLogger.LogInfo(LogLevel.Error, ex.Message);
                    }
                }
                password = ProtectDocumentPassword;
                if (level == ProtectLevel.Document || level == ProtectLevel.All)
                {
                    try
                    {
                        wDoc.Unprotect(ref password);
                        removedLevel = ProtectLevel.Document;
                    }
                    catch (Exception ex)
                    {
                        SxLogger.LogInfo(LogLevel.Error, ex.Message);
                    }
                }
            }

            return removedLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="xslFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="mediaType"></param>
        internal static void GenerateFile(string xmlFilePath, string xslFilePath, string outputFilePath,
            RenderSettings.MediaType mediaType, string watermark = "")
        {
            System.Exception exception = null;

            // 2. open xml file
            Document wDoc = GetWDoc(FileName: xmlFilePath,
                                    Format: WdOpenFormat.wdOpenFormatXML,
                                    XMLTransform: xslFilePath);

            // 3. generate file
            try
            {
                // 3.1. add water mark
                if (!string.IsNullOrEmpty(watermark))
                {
                    RemoveProtectPassword(wDoc, ProtectLevel.All);
                    AddWatermark(wApp, watermark);
                }

                // 3.2. save as new file
                switch (mediaType)
                {
                    case RenderSettings.MediaType.Pdf:
                        wDoc.ExportAsFixedFormat(outputFilePath, WdExportFormat.wdExportFormatPDF);
                        break;
                    case RenderSettings.MediaType.Xps:
                        wDoc.ExportAsFixedFormat(outputFilePath, WdExportFormat.wdExportFormatXPS);
                        break;
                    case RenderSettings.MediaType.Docx:
                        wDoc.SaveAs(outputFilePath, FileFormat: WdSaveFormat.wdFormatXMLDocument);
                        break;
                    case RenderSettings.MediaType.Mht:
                        wDoc.SaveAs(outputFilePath, WdSaveFormat.wdFormatWebArchive);
                        break;
                }

                wDoc.Saved = true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }

            CloseWDoc(wDoc);

            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdwFilePath"></param>
        /// <param name="isHighlightBookmarks"></param>
        /// <param name="isAddWaterMark"></param>
        /// <param name="pdfOutputFile"></param>
        internal static void GenerateFile(string pdwFilePath, bool isHighlightBookmarks, string watermark, string pdfOutputFile,
            Dictionary<string, string> bookmarks = null,
            Dictionary<string, string> datatagColors = null, Dictionary<string, string> documentSpecificColors = null)
        {
            System.Exception exception = null;

            // 2. open pdw file
            Document wDoc = GetWDoc(pdwFilePath);

            // 3. generate file
            try
            {
                // 3.1. add water mark
                if (!string.IsNullOrEmpty(watermark))
                {
                    RemoveProtectPassword(wDoc, ProtectLevel.All);
                    AddWatermark(wApp, watermark);
                }

                // 3.2. highlight all bookmark
                if (isHighlightBookmarks)
                    HighlightBookmarks(wDoc, bookmarks, datatagColors, documentSpecificColors);

                // 3.3. save as new file
                object pdfFormat = WdSaveFormat.wdFormatPDF;
                wDoc.SaveAs(pdfOutputFile, pdfFormat);
                wDoc.Saved = true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }

            CloseWDoc(wDoc);

            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordXmlFile"></param>
        /// <param name="mediaType"></param>
        /// <param name="outputFile"></param>
        internal static void GenerateFile(string wordXmlFile, RenderSettings.MediaType mediaType, string watermark, string outputFile)
        {
            System.Exception exception = null;

            // 2. open word document xml file
            Document wDoc = GetWDoc(FileName: wordXmlFile, Format: WdOpenFormat.wdOpenFormatXML);

            // 3. generate to output file
            try
            {
                // 3.1. add water mark
                if (!string.IsNullOrEmpty(watermark))
                {
                    RemoveProtectPassword(wDoc, ProtectLevel.All);
                    AddWatermark(wApp, watermark);
                }

                // 3.2. export file
                switch (mediaType)
                {
                    case RenderSettings.MediaType.Pdf:
                        wDoc.ExportAsFixedFormat(outputFile, WdExportFormat.wdExportFormatPDF);
                        break;
                    case RenderSettings.MediaType.Xps:
                        wDoc.ExportAsFixedFormat(outputFile, WdExportFormat.wdExportFormatXPS);
                        break;
                    case RenderSettings.MediaType.Docx:
                        break;
                }

                wDoc.Saved = true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }

            // 4. close
            CloseWDoc(wDoc);

            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// close word document and word application
        /// </summary>
        /// <param name="wApp"></param>
        /// <param name="wDoc"></param>
        internal static void Close(Application wApp, Document wDoc)
        {
            object missing = System.Type.Missing;

            // 1. close document
            if (wDoc != null)
            {
                if (!wDoc.Saved)
                    wDoc.Saved = true;
                ((_Document)wDoc).Close(missing, missing, missing);
            }

            // 2. quit application
            if (wApp != null)
                ((_Application)wApp).Quit(missing, missing, missing);

            // 3. release com object
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wDoc);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wApp);
            }
            catch (Exception ex)
            {
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }
        }

        #endregion

        #region private methods
        /// <summary>
        /// add a watermark string into docx file
        /// </summary>
        /// <param name="wApp"></param>
        /// <param name="WatermarkText"></param>
        private static void AddWatermark(Application wApp, string watermark)
        {
            //todo: Lucky removed try/catch
            //try
            //{
            Selection Selection = wApp.Selection;
            Shape wmShape;
            Selection.Select();

            // set focus on header to put watermark
            wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;

            //Create the watermar shape
            wmShape = Selection.HeaderFooter.Shapes.AddTextEffect(
                        Microsoft.Office.Core.MsoPresetTextEffect.msoTextEffect1,
                        watermark, "Times New Roman", 1,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        0, 0);

            //Set all of the attributes of the watermark
            wmShape.Select();
            wmShape.Name = "PowerPlusWaterMarkObject11";
            wmShape.TextEffect.NormalizedHeight = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Fill.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
            wmShape.Fill.Solid();
            wmShape.Fill.ForeColor.RGB = (int)WdColor.wdColorGray25;
            wmShape.Fill.Transparency = 0.5f;
            wmShape.Rotation = 315.0f;
            wmShape.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
            wmShape.Height = wApp.InchesToPoints(2.82f);
            wmShape.Width = wApp.InchesToPoints(5.64f);
            wmShape.WrapFormat.AllowOverlap = -1; //true
            wmShape.WrapFormat.Side = WdWrapSideType.wdWrapBoth;
            wmShape.WrapFormat.Type = WdWrapType.wdWrapNone;  //3
            wmShape.RelativeHorizontalPosition =
                        WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
            wmShape.RelativeVerticalPosition =
                        WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
            wmShape.Left = (float)WdShapePosition.wdShapeCenter;
            wmShape.Top = (float)WdShapePosition.wdShapeCenter;

            //set focus back to document
            wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
            //}
            //catch { }
        }

        /// <summary>
        /// highlight all bookmarks in docx file
        /// </summary>
        /// <param name="wordDoc"></param>
        private static void HighlightBookmarks(Document wordDoc, Dictionary<string, string> bookmarks,
            Dictionary<string, string> datatagColors, Dictionary<string, string> documentSpecificColors)
        {
            RemoveProtectPassword(wordDoc, ProtectLevel.All);
            if (bookmarks == null)
                return;
            foreach (Bookmark bm in wordDoc.Bookmarks)
            {
                string name = bm.Name;
                if (!bookmarks.ContainsKey(name))
                    continue;
                string domainName = bookmarks[name];
                string colorName = string.Empty;
                WdColorIndex defaultColor = WdColorIndex.wdBlue;
                if (name.Contains("Sys"))
                {
                    defaultColor = WdColorIndex.wdBlue;
                    if (documentSpecificColors != null && documentSpecificColors.ContainsKey(domainName))
                        colorName = documentSpecificColors[domainName];
                }
                else
                {
                    defaultColor = WdColorIndex.wdYellow;
                    if (datatagColors != null && datatagColors.ContainsKey(domainName))
                        colorName = datatagColors[domainName];
                }
                bm.Range.Font.Color = WdColor.wdColorBlack;

                bm.Range.HighlightColorIndex = GetWdColorIndex(colorName, defaultColor);
            }
        }

        private static WdColorIndex GetWdColorIndex(string colorName, WdColorIndex defaultColor)
        {
            if (string.IsNullOrEmpty(colorName))
                return defaultColor;

            switch (colorName.Trim().ToLower())
            {
                case "byauthor":
                    return WdColorIndex.wdByAuthor;
                case "auto":
                    return WdColorIndex.wdAuto;
                case "nohighlight":
                    return WdColorIndex.wdNoHighlight;
                case "black":
                    return WdColorIndex.wdBlack;
                case "blue":
                    return WdColorIndex.wdBlue;
                case "turquoise":
                    return WdColorIndex.wdTurquoise;
                case "brightgreen":
                    return WdColorIndex.wdBrightGreen;
                case "pink":
                    return WdColorIndex.wdPink;
                case "red":
                    return WdColorIndex.wdRed;
                case "yellow":
                    return WdColorIndex.wdYellow;
                case "white":
                    return WdColorIndex.wdWhite;
                case "darkblue":
                    return WdColorIndex.wdDarkBlue;
                case "teal":
                    return WdColorIndex.wdTeal;
                case "green":
                    return WdColorIndex.wdGreen;
                case "violet":
                    return WdColorIndex.wdViolet;
                case "darkred":
                    return WdColorIndex.wdDarkRed;
                case "darkyellow":
                    return WdColorIndex.wdDarkYellow;
                case "gray50":
                    return WdColorIndex.wdGray50;
                case "gray25":
                    return WdColorIndex.wdGray25;
                default:
                    return defaultColor;
            }
        }
        #endregion
    }

    public class WordHeper1
    {
        private const string ProtectBookmarkPassword = "ngocbv_bookmark";
        private const string ProtectDocumentPassword = "ngocbv_fulldoc";

        #region public methods
        /// <summary>
        /// remove protect password in document
        /// </summary>
        /// <param name="wDoc">Word document</param>
        /// <param name="level">Level of protected to remove</param>
        /// <returns>Level of removed protection (0: bookmark, 1: document, other: -1)</returns>
        internal static ProtectLevel RemoveProtectPassword(Document wDoc, ProtectLevel level)
        {
            ProtectLevel removedLevel = ProtectLevel.None;

            if (wDoc.ProtectionType == WdProtectionType.wdAllowOnlyReading)
            {
                object password = ProtectBookmarkPassword;
                if (level == ProtectLevel.Bookmark || level == ProtectLevel.All)
                {
                    try
                    {
                        wDoc.Unprotect(ref password);
                        removedLevel = ProtectLevel.Bookmark;
                    }
                    catch (Exception ex)
                    {
                        SxLogger.LogInfo(LogLevel.Error, ex.Message);
                    }
                }
                password = ProtectDocumentPassword;
                if (level == ProtectLevel.Document || level == ProtectLevel.All)
                {
                    try
                    {
                        wDoc.Unprotect(ref password);
                        removedLevel = ProtectLevel.Document;
                    }
                    catch (Exception ex)
                    {
                        SxLogger.LogInfo(LogLevel.Error, ex.Message);
                    }
                }
            }

            return removedLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="xslFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="mediaType"></param>
        internal static void GenerateFile(string xmlFilePath, string xslFilePath, string outputFilePath,
            RenderSettings.MediaType mediaType, string watermark = "")
        {
            System.Exception exception = null;

            // 1. initialize word application
            Application wApp = new Application();
            wApp.Visible = false;

            // 2. open xml file
            Document wDoc = wApp.Documents.Open(
                    FileName: xmlFilePath,
                    Format: WdOpenFormat.wdOpenFormatXML,
                    XMLTransform: xslFilePath);

            // 3. generate file
            try
            {
                // 3.1. add water mark
                if (!string.IsNullOrEmpty(watermark))
                {
                    RemoveProtectPassword(wDoc, ProtectLevel.All);
                    AddWatermark(wApp, watermark);
                }

                // 3.2. save as new file
                switch (mediaType)
                {
                    case RenderSettings.MediaType.Pdf:
                        wDoc.ExportAsFixedFormat(outputFilePath, WdExportFormat.wdExportFormatPDF);
                        break;
                    case RenderSettings.MediaType.Xps:
                        wDoc.ExportAsFixedFormat(outputFilePath, WdExportFormat.wdExportFormatXPS);
                        break;
                    case RenderSettings.MediaType.Docx:
                        wDoc.SaveAs(outputFilePath, FileFormat: WdSaveFormat.wdFormatXMLDocument);
                        break;
                    case RenderSettings.MediaType.Mht:
                        wDoc.SaveAs(outputFilePath, WdSaveFormat.wdFormatWebArchive);
                        break;
                }

                wDoc.Saved = true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }

            Close(wApp, wDoc);

            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdwFilePath"></param>
        /// <param name="isHighlightBookmarks"></param>
        /// <param name="isAddWaterMark"></param>
        /// <param name="pdfOutputFile"></param>
        internal static void GenerateFile(string pdwFilePath, bool isHighlightBookmarks, string watermark, string pdfOutputFile,
            string datatagColor = "", string documentSpecificColor = "")
        {
            System.Exception exception = null;

            // 1. initialize word application
            Application wApp = new Application();
            wApp.Visible = false;

            // 2. open pdw file
            Document wDoc = wApp.Documents.Open(pdwFilePath);

            // 3. generate file
            try
            {
                // 3.1. add water mark
                if (!string.IsNullOrEmpty(watermark))
                {
                    RemoveProtectPassword(wDoc, ProtectLevel.All);
                    AddWatermark(wApp, watermark);
                }

                // 3.2. highlight all bookmark
                if (isHighlightBookmarks)
                    HighlightBookmarks(wDoc, datatagColor, documentSpecificColor);

                // 3.3. save as new file
                object pdfFormat = WdSaveFormat.wdFormatPDF;
                wDoc.SaveAs(pdfOutputFile, pdfFormat);
                wDoc.Saved = true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }

            Close(wApp, wDoc);

            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordXmlFile"></param>
        /// <param name="mediaType"></param>
        /// <param name="outputFile"></param>
        internal static void GenerateFile(string wordXmlFile, RenderSettings.MediaType mediaType, string watermark, string outputFile)
        {
            System.Exception exception = null;

            // 1. initialize word application
            Application wApp = new Application();
            wApp.Visible = false;

            // 2. open word document xml file
            Document wDoc = wApp.Documents.Open(FileName: wordXmlFile, Format: WdOpenFormat.wdOpenFormatXML);

            // 3. generate to output file
            try
            {
                // 3.1. add water mark
                if (!string.IsNullOrEmpty(watermark))
                {
                    RemoveProtectPassword(wDoc, ProtectLevel.All);
                    AddWatermark(wApp, watermark);
                }

                // 3.2. export file
                switch (mediaType)
                {
                    case RenderSettings.MediaType.Pdf:
                        wDoc.ExportAsFixedFormat(outputFile, WdExportFormat.wdExportFormatPDF);
                        break;
                    case RenderSettings.MediaType.Xps:
                        wDoc.ExportAsFixedFormat(outputFile, WdExportFormat.wdExportFormatXPS);
                        break;
                    case RenderSettings.MediaType.Docx:
                        break;
                }

                wDoc.Saved = true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }

            // 4. close
            Close(wApp, wDoc);

            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// close word document and word application
        /// </summary>
        /// <param name="wApp"></param>
        /// <param name="wDoc"></param>
        internal static void Close(Application wApp, Document wDoc)
        {
            object missing = System.Type.Missing;

            // 1. close document
            if (wDoc != null)
            {
                if (!wDoc.Saved)
                    wDoc.Saved = true;
                ((_Document)wDoc).Close(missing, missing, missing);
            }

            // 2. quit application
            if (wApp != null)
                ((_Application)wApp).Quit(missing, missing, missing);

            // 3. release com object
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wDoc);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wApp);
            }
            catch (Exception ex)
            {
                SxLogger.LogInfo(LogLevel.Error, ex.Message);
            }
        }

        #endregion

        #region private methods
        /// <summary>
        /// add a watermark string into docx file
        /// </summary>
        /// <param name="wApp"></param>
        /// <param name="WatermarkText"></param>
        private static void AddWatermark(Application wApp, string watermark)
        {
            //todo: Lucky removed try/catch
            //try
            //{
            Selection Selection = wApp.Selection;
            Shape wmShape;
            Selection.Select();

            // set focus on header to put watermark
            wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;

            //Create the watermar shape
            wmShape = Selection.HeaderFooter.Shapes.AddTextEffect(
                        Microsoft.Office.Core.MsoPresetTextEffect.msoTextEffect1,
                        watermark, "Times New Roman", 1,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        0, 0);

            //Set all of the attributes of the watermark
            wmShape.Select();
            wmShape.Name = "PowerPlusWaterMarkObject11";
            wmShape.TextEffect.NormalizedHeight = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
            wmShape.Fill.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
            wmShape.Fill.Solid();
            wmShape.Fill.ForeColor.RGB = (int)WdColor.wdColorGray25;
            wmShape.Fill.Transparency = 0.5f;
            wmShape.Rotation = 315.0f;
            wmShape.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
            wmShape.Height = wApp.InchesToPoints(2.82f);
            wmShape.Width = wApp.InchesToPoints(5.64f);
            wmShape.WrapFormat.AllowOverlap = -1; //true
            wmShape.WrapFormat.Side = WdWrapSideType.wdWrapBoth;
            wmShape.WrapFormat.Type = WdWrapType.wdWrapNone;  //3
            wmShape.RelativeHorizontalPosition =
                        WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
            wmShape.RelativeVerticalPosition =
                        WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
            wmShape.Left = (float)WdShapePosition.wdShapeCenter;
            wmShape.Top = (float)WdShapePosition.wdShapeCenter;

            //set focus back to document
            wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
            //}
            //catch { }
        }

        /// <summary>
        /// highlight all bookmarks in docx file
        /// </summary>
        /// <param name="wordDoc"></param>
        private static void HighlightBookmarks(Document wordDoc, string datatagColor, string documentSpecificColor)
        {
            RemoveProtectPassword(wordDoc, ProtectLevel.All);
            foreach (Bookmark bm in wordDoc.Bookmarks)
            {
                string name = bm.Name;
                bool isCondition = false;
                if (name.Contains("Sys"))
                    isCondition = true;
                bm.Range.Font.Color = WdColor.wdColorBlack;
                // bm.Range.HighlightColorIndex = isCondition ? WdColorIndex.wdBlue : WdColorIndex.wdYellow;
                bm.Range.HighlightColorIndex = isCondition ? GetWdColorIndex(documentSpecificColor, WdColorIndex.wdBlue) :
                    GetWdColorIndex(datatagColor, WdColorIndex.wdYellow);
            }
        }

        private static WdColorIndex GetWdColorIndex(string colorName, WdColorIndex defaultColor)
        {
            if (string.IsNullOrEmpty(colorName))
                return defaultColor;

            switch (colorName.Trim().ToLower())
            {
                case "byauthor":
                    return WdColorIndex.wdByAuthor;
                case "auto":
                    return WdColorIndex.wdAuto;
                case "nohighlight":
                    return WdColorIndex.wdNoHighlight;
                case "black":
                    return WdColorIndex.wdBlack;
                case "blue":
                    return WdColorIndex.wdBlue;
                case "turquoise":
                    return WdColorIndex.wdTurquoise;
                case "brightgreen":
                    return WdColorIndex.wdBrightGreen;
                case "pink":
                    return WdColorIndex.wdPink;
                case "red":
                    return WdColorIndex.wdRed;
                case "yellow":
                    return WdColorIndex.wdYellow;
                case "white":
                    return WdColorIndex.wdWhite;
                case "darkblue":
                    return WdColorIndex.wdDarkBlue;
                case "teal":
                    return WdColorIndex.wdTeal;
                case "green":
                    return WdColorIndex.wdGreen;
                case "violet":
                    return WdColorIndex.wdViolet;
                case "darkred":
                    return WdColorIndex.wdDarkRed;
                case "darkyellow":
                    return WdColorIndex.wdDarkYellow;
                case "gray50":
                    return WdColorIndex.wdGray50;
                case "gray25":
                    return WdColorIndex.wdGray25;
                default:
                    return defaultColor;
            }
        }
        #endregion
    }
}
