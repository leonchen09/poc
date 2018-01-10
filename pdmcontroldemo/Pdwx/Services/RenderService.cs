//using System;
//using System.IO;

//using Pdwx.DataObjects;

//namespace Pdwx.Services
//{
//    public class RenderService
//    {
//        /// <summary>
//        /// Render pdw or pdwr to another format
//        /// </summary>
//        /// <param name="inputFilePath">The path file of pdw file or pdwr file (pdw, pdwr)</param>
//        /// <param name="xmlFilePath">The path of xml file (data)</param>
//        /// <param name="outputFilePath">The path of output file (docx, pdf, pdwr)</param>
//        /// <param name="mode">RenderMode</param>
//        public bool Render(RenderInfo renderInfo)
//        {
//            try
//            {
//                switch (renderInfo.RenderMode)
//                {
//                    case RenderMode.ApprovalMockup:
//                        ApprovalMockup(renderInfo);
//                        break;
//                    case RenderMode.ApprovalRaw:
//                        ApprovalRaw(renderInfo);
//                        break;
//                    case RenderMode.Normal:
//                        Normal(renderInfo);
//                        break;
//                }

//                return true;
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        #region render mode detail

//        #region 1. Mockup: ren with data
//        /// <summary>
//        /// Render test mode (export to pdf with data)
//        /// </summary>
//        /// <param name="renderInfo"></param>
//        private void ApprovalMockup(RenderInfo renderInfo)
//        {
//            // 1. validate output format
//            if (!FileExtension.Pdf.Equals(Path.GetExtension(renderInfo.OutputFilePath), StringComparison.OrdinalIgnoreCase))
//                throw new InvalidOutputExtension("Output file extension must be pdf. Current is " + renderInfo.OutputFilePath);

//            // 2. validate xslt file
//            if (renderInfo.XslContent == null)
//                throw new InvalidOutputExtension();

//            // 3. validate xml data
//            if (renderInfo.XmlDoc == null)
//                return;

//            // 4. Translate: xml + xsl -> pdf
//            renderInfo.CreatTempFiles();
//            string tempWordXmlFile = renderInfo.OutputFilePath + ".xml";
//            bool isSuccess = TransformHelper.Transform(renderInfo.XmlFilePath, renderInfo.XslFilePath, tempWordXmlFile);

//            if (isSuccess)
//                WordHeper.GenerateFile(tempWordXmlFile, RenderSettings.MediaType.Pdf, true, renderInfo.OutputFilePath);
//            else
//                WordHeper.GenerateFile(renderInfo.XmlFilePath, renderInfo.XslFilePath, renderInfo.OutputFilePath,
//                    RenderSettings.MediaType.Pdf, true);

//            // 5. remove temporary files
//            if (isSuccess)
//                System.IO.File.Delete(tempWordXmlFile);
//            renderInfo.DeleteTempFiles();
//        }
//        #endregion

//        #region 2. Raw: ren without data
//        /// <summary>
//        /// Render raw mode (export pdf without data; highlight bookmarks)
//        /// </summary>
//        /// <param name="renderInfo"></param>
//        private void ApprovalRaw(RenderInfo renderInfo)
//        {
//            // 1. check existing of pdw file
//            if (!File.Exists(renderInfo.PdwFilePath))
//                throw new FileNotExistException();

//            // 2. check extension of pdw file
//            if (!FileExtension.ProntoDocumenentWord.Equals(Path.GetExtension(renderInfo.PdwFilePath), StringComparison.OrdinalIgnoreCase))
//                throw new InvalidInputExtension();

//            // 3. check extension of output file path
//            if (!FileExtension.Pdf.Equals(Path.GetExtension(renderInfo.OutputFilePath), StringComparison.OrdinalIgnoreCase))
//                throw new InvalidOutputExtension("Output file extension must be pdf. Current is " + renderInfo.OutputFilePath);

//            // 4. convert docx to pdf (without data; highlight bookmark)
//            WordHeper.GenerateFile(renderInfo.PdwFilePath, true, true, renderInfo.OutputFilePath);
//        }
//        #endregion

//        #region 3. Normal: ren pdwr
//        /// <summary>
//        /// Render pdwr mode (gen pdwr file: docx, xsl, xml data)
//        /// </summary>
//        /// <param name="renderInfo"></param>
//        private void Normal(RenderInfo renderInfo)
//        {
//            // 1. validate xslt file
//            if (renderInfo.XslContent == null)
//                throw new InvalidOutputExtension("XslContent must be not empty");

//            // 2. validate xml file
//            // if (renderInfo.XmlData == null)
//            if (renderInfo.XmlDoc == null)
//                throw new InvalidOutputExtension("XmlContent must be not empty");

//            // 3. validate RenderSettings
//            if (renderInfo.RenderSettings == null)
//                throw new InvalidRenderSetting();

//            // 4. render
//            switch (renderInfo.RenderSettings.Channel)
//            {
//                case RenderSettings.ChannelType.Display:
//                    NormalDisplay(renderInfo);
//                    break;
//                case RenderSettings.ChannelType.Attachment:
//                case RenderSettings.ChannelType.Email:
//                case RenderSettings.ChannelType.Fax:
//                    NormalOther(renderInfo);
//                    break;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="renderInfo"></param>
//        private void NormalDisplay(RenderInfo renderInfo)
//        {
//            // 1. at client
//            if (FileExtension.Pdwr.Equals(Path.GetExtension(renderInfo.OutputFilePath), StringComparison.CurrentCultureIgnoreCase))
//            {
//                NormalOther(renderInfo);
//                return;
//            }

//            // 2. at server
//            renderInfo.CreatTempFiles();
//            WordHeper.GenerateFile(renderInfo.XmlFilePath, renderInfo.XslFilePath, renderInfo.OutputFilePath, renderInfo.RenderSettings.Media);
//            renderInfo.DeleteTempFiles();
//        }

//        /// <summary>
//        /// Create pdwr and put xml, xsl, render setting also
//        /// </summary>
//        /// <param name="renderInfo"></param>
//        private void NormalOther(RenderInfo renderInfo)
//        {
//            switch (renderInfo.RenderSettings.Channel)
//            {
//                case RenderSettings.ChannelType.Attachment:
//                case RenderSettings.ChannelType.Email:
//                    if (renderInfo.RenderSettings.ChannelSpecificInfo == null || !(renderInfo.RenderSettings.ChannelSpecificInfo is EmailInfo))
//                        throw new InvalidRenderSetting();
//                    break;
//                case RenderSettings.ChannelType.Fax:
//                    if (renderInfo.RenderSettings.ChannelSpecificInfo == null || !(renderInfo.RenderSettings.ChannelSpecificInfo is FaxInfo))
//                        throw new InvalidRenderSetting();
//                    break;
//                default:
//                    break;
//            }

//            if (!renderInfo.OutputFilePath.ToLower().EndsWith(FileExtension.Pdwr))
//                throw new InvalidOutputExtension("Output file extension must be pdwr. Current is " + renderInfo.OutputFilePath);

//            PdwrWriter pdwrWriter = new PdwrWriter(renderInfo.XslContent, renderInfo.XmlDoc, renderInfo.RenderSettings);
//            pdwrWriter.Save(renderInfo.OutputFilePath);
//        }
//        #endregion

//        #endregion
//    }
//}