

using Microsoft.Office.Interop.Word;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

namespace Pdwx.Services
{
    public class WordHeper
    {
        private const string ProtectBookmarkPassword = "ngocbv_bookmark";
        private const string ProtectDocumentPassword = "ngocbv_fulldoc";

        #region public methods: RemoveProtectPassword, ProtectDocument
        /// <summary>
        /// remove protect password in document
        /// </summary>
        /// <param name="wDoc">Word document</param>
        /// <param name="level">Level of protected to remove</param>
        /// <returns>Level of removed protection (0: bookmark, 1: document, other: -1)</returns>
        public static ProtectLevel RemoveProtectPassword(Document wDoc, ProtectLevel level)
        {
            ProtectLevel removedLevel = ProtectLevel.None;

            if (wDoc.ProtectionType == WdProtectionType.wdAllowOnlyReading)
            {
                object password = ProtectBookmarkPassword;
                if (level == ProtectLevel.Bookmark || level == ProtectLevel.All)
                {
                    wDoc.Unprotect(ref password);
                    removedLevel = ProtectLevel.Bookmark;
                }
                password = ProtectDocumentPassword;
                if (level == ProtectLevel.Document || level == ProtectLevel.All)
                {
                    wDoc.Unprotect(ref password);
                    removedLevel = ProtectLevel.Document;
                }
            }

            return removedLevel;
        }

        /// <summary>
        /// protect document with password
        /// </summary>
        /// <param name="wDoc"></param>
        /// <param name="level"></param>
        public static void ProtectDocument(Document wDoc, ProtectLevel level)
        {
            object missing = System.Type.Missing;
            object password = null;
            switch (level)
            {
                case ProtectLevel.All:
                case ProtectLevel.Document:
                    password = ProtectDocumentPassword;
                    break;
                case ProtectLevel.Bookmark:
                    password = ProtectBookmarkPassword;
                    break;
                default:
                    break;
            }
            if (password != null)
                wDoc.Protect(WdProtectionType.wdAllowOnlyReading, ref missing, ref password, ref missing, ref missing);
        }
        #endregion

        //#region internal methods
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="xmlFilePath"></param>
        ///// <param name="xslFilePath"></param>
        ///// <param name="outputFilePath"></param>
        ///// <param name="mediaType"></param>
        //internal static void GenerateFile(string xmlFilePath, string xslFilePath, string outputFilePath,
        //    RenderSettings.MediaType mediaType, bool isAddWaterMark = false)
        //{
        //    System.Exception exception = null;

        //    // 1. initialize word application
        //    Application wApp = new Application();
        //    wApp.Visible = false;

        //    // 2. open xml file
        //    Document wDoc = wApp.Documents.Open(
        //            FileName: xmlFilePath,
        //            Format: WdOpenFormat.wdOpenFormatXML,
        //            XMLTransform: xslFilePath);

        //    // 3. generate file
        //    try
        //    {
        //        // 3.1. add water mark
        //        if (isAddWaterMark)
        //            AddWatermark(wApp);

        //        // 3.2. save as new file
        //        switch (mediaType)
        //        {
        //            case RenderSettings.MediaType.Pdf:
        //                wDoc.ExportAsFixedFormat(outputFilePath, WdExportFormat.wdExportFormatPDF);
        //                break;
        //            case RenderSettings.MediaType.Xps:
        //                wDoc.ExportAsFixedFormat(outputFilePath, WdExportFormat.wdExportFormatXPS);
        //                break;
        //            case RenderSettings.MediaType.MsXml:
        //                wDoc.SaveAs(outputFilePath, WdSaveFormat.wdFormatXMLDocument);
        //                break;
        //        }

        //        wDoc.Saved = true;
        //    }
        //    catch (System.Exception ex) { exception = ex; }

        //    Close(wApp, wDoc);

        //    if (exception != null)
        //        throw exception;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="pdwFilePath"></param>
        ///// <param name="isHighlightBookmarks"></param>
        ///// <param name="isAddWaterMark"></param>
        ///// <param name="pdfOutputFile"></param>
        //internal static void GenerateFile(string pdwFilePath, bool isHighlightBookmarks, bool isAddWaterMark, string pdfOutputFile)
        //{
        //    System.Exception exception = null;

        //    // 1. initialize word application
        //    Application wApp = new Application();
        //    wApp.Visible = false;

        //    // 2. open pdw file
        //    Document wDoc = wApp.Documents.Open(pdwFilePath);

        //    // 3. generate file
        //    try
        //    {
        //        // 3.1. add water mark
        //        if (isAddWaterMark)
        //            AddWatermark(wApp);

        //        // 3.2. highlight all bookmark
        //        if (isHighlightBookmarks)
        //            HighlightBookmarks(wDoc);

        //        // 3.3. save as new file
        //        object pdfFormat = WdSaveFormat.wdFormatPDF;
        //        wDoc.SaveAs(pdfOutputFile, pdfFormat);
        //        wDoc.Saved = true;
        //    }
        //    catch (System.Exception ex) { exception = ex; }

        //    Close(wApp, wDoc);

        //    if (exception != null)
        //        throw exception;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="wordXmlFile"></param>
        ///// <param name="mediaType"></param>
        ///// <param name="outputFile"></param>
        //internal static void GenerateFile(string wordXmlFile, RenderSettings.MediaType mediaType, bool isAddWaterMark, string outputFile)
        //{
        //    System.Exception exception = null;

        //    // 1. initialize word application
        //    Application wApp = new Application();
        //    wApp.Visible = false;

        //    // 2. open word document xml file
        //    Document wDoc = wApp.Documents.Open(FileName: wordXmlFile, Format: WdOpenFormat.wdOpenFormatXML);

        //    // 3. generate to output file
        //    try
        //    {
        //        // 3.1. add water mark
        //        if(isAddWaterMark)
        //            AddWatermark(wApp);

        //        // 3.2. export file
        //        switch (mediaType)
        //        {
        //            case RenderSettings.MediaType.Pdf:
        //                wDoc.ExportAsFixedFormat(outputFile, WdExportFormat.wdExportFormatPDF);
        //                break;
        //            case RenderSettings.MediaType.Xps:
        //                wDoc.ExportAsFixedFormat(outputFile, WdExportFormat.wdExportFormatXPS);
        //                break;
        //            case RenderSettings.MediaType.MsXml:
        //                break;
        //        }

        //        wDoc.Saved = true;
        //    }
        //    catch (System.Exception ex) { exception = ex; }

        //    // 4. close
        //    Close(wApp, wDoc);

        //    if (exception != null)
        //        throw exception;
        //}

        ///// <summary>
        ///// close word document and word application
        ///// </summary>
        ///// <param name="wApp"></param>
        ///// <param name="wDoc"></param>
        //internal static void Close(Application wApp, Document wDoc)
        //{
        //    object missing = System.Type.Missing;

        //    // 1. close document
        //    if (wDoc != null)
        //    {
        //        if (!wDoc.Saved)
        //            wDoc.Saved = true;
        //        ((_Document)wDoc).Close(missing, missing, missing);
        //    }

        //    // 2. quit application
        //    if (wApp != null)
        //        ((_Application)wApp).Quit(missing, missing, missing);

        //    // 3. release com object
        //    try
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(wDoc);
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(wApp);
        //    }
        //    catch { }
        //}
        //#endregion

        //#region private methods
        ///// <summary>
        ///// add a watermark string into docx file
        ///// </summary>
        ///// <param name="wApp"></param>
        ///// <param name="WatermarkText"></param>
        //private static void AddWatermark(Application wApp)
        //{
        //    Selection Selection = wApp.Selection;
        //    Shape wmShape;
        //    Selection.Select();

        //    // set focus on header to put watermark
        //    wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekCurrentPageHeader;

        //    //Create the watermar shape
        //    wmShape = Selection.HeaderFooter.Shapes.AddTextEffect(
        //                Microsoft.Office.Core.MsoPresetTextEffect.msoTextEffect1,
        //                Constants.Watermark, "Times New Roman", 1,
        //                Microsoft.Office.Core.MsoTriState.msoFalse,
        //                Microsoft.Office.Core.MsoTriState.msoFalse,
        //                0, 0);

        //    //Set all of the attributes of the watermark
        //    wmShape.Select();
        //    wmShape.Name = "PowerPlusWaterMarkObject1";
        //    wmShape.TextEffect.NormalizedHeight = Microsoft.Office.Core.MsoTriState.msoFalse;
        //    wmShape.Line.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
        //    wmShape.Fill.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
        //    wmShape.Fill.Solid();
        //    wmShape.Fill.ForeColor.RGB = (int)WdColor.wdColorGray25;
        //    wmShape.Fill.Transparency = 0.5f;
        //    wmShape.Rotation = 315.0f;
        //    wmShape.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
        //    wmShape.Height = wApp.InchesToPoints(2.82f);
        //    wmShape.Width = wApp.InchesToPoints(5.64f);
        //    wmShape.WrapFormat.AllowOverlap = -1; //true
        //    wmShape.WrapFormat.Side = WdWrapSideType.wdWrapBoth;
        //    wmShape.WrapFormat.Type = WdWrapType.wdWrapNone;  //3
        //    wmShape.RelativeHorizontalPosition =
        //                WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
        //    wmShape.RelativeVerticalPosition =
        //                WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
        //    wmShape.Left = (float)WdShapePosition.wdShapeCenter;
        //    wmShape.Top = (float)WdShapePosition.wdShapeCenter;

        //    //set focus back to document
        //    wApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
        //}

        ///// <summary>
        ///// highlight all bookmarks in docx file
        ///// </summary>
        ///// <param name="wordDoc"></param>
        //private static void HighlightBookmarks(Document wordDoc)
        //{
        //    foreach (Bookmark bm in wordDoc.Bookmarks)
        //    {
        //        bm.Range.Font.Color = WdColor.wdColorBlack;
        //        bm.Range.HighlightColorIndex = WdColorIndex.wdYellow;
        //    }
        //}
        //#endregion

        //#region helper objects
        //public enum ProtectLevel
        //{
        //    None,
        //    Bookmark,
        //    Document,
        //    All,
        //}
        //#endregion
    }
}
