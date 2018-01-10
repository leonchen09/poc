
using Microsoft.Win32;
using System.Collections.Generic;

using Word = Microsoft.Office.Interop.Word;
using Outlook = Microsoft.Office.Interop.Outlook;

using ProntoDoc.Framework.CoreObject.PdwxObjects;

namespace Pdw.Core
{
    class MailHelper
    {
        internal static void SendEmail(Word.Document wDoc, RenderSettings renderSetting, ref List<string> tempFiles)
        {
            MailType mailType = GetDefaultEmailClientProgram();

            switch (mailType)
            {
                case MailType.OutLook:
                    bool isSuccess = false;
                    try
                    {
                        isSuccess = OutLookMail.SendEmail(wDoc, renderSetting, ref tempFiles);
                    }
                    catch (BaseException baseExp)
                    {
                        Managers.ManagerException mgrExp = new Managers.ManagerException(ErrorCode.ipe_SendMailError);
                        baseExp.Errors.Add(baseExp);

                        throw mgrExp;
                    }
                    catch (System.Exception ex)
                    {
                        Managers.ManagerException mgrExp = new Managers.ManagerException(ErrorCode.ipe_SendMailError,
                            MessageUtils.Expand(Properties.Resources.ipe_SendMailError, ex.Message), ex.StackTrace);

                        throw mgrExp;
                    }

                    if (!isSuccess)
                    {
                        try
                        {
                            WordMail.SendEmail(wDoc, renderSetting);
                        }
                        catch (System.Exception ex)
                        {
                            Managers.ManagerException mgrExp = new Managers.ManagerException(ErrorCode.ipe_SendMailError,
                                MessageUtils.Expand(Properties.Resources.ipe_SendMailError, ex.Message), ex.StackTrace);

                            throw mgrExp;
                        }
                    }
                    break;
                case MailType.Unknow:
                    try
                    {
                        WordMail.SendEmail(wDoc, renderSetting);
                    }
                    catch (System.Exception ex)
                    {
                        Managers.ManagerException mgrExp = new Managers.ManagerException(ErrorCode.ipe_SendMailError,
                            MessageUtils.Expand(Properties.Resources.ipe_SendMailError, ex.Message), ex.StackTrace);

                        throw mgrExp;
                    }
                    break;
                default:
                    break;
            }

        }

        //internal static void Test(Word.Document wDoc, ref List<string> tempFiles)
        //{
        //    RenderSettings renderSetting = new RenderSettings();
        //    renderSetting.Channel = RenderSettings.ChannelType.Attachment;
        //    renderSetting.Media = RenderSettings.MediaType.MsXml;
        //    EmailInfo emailInfo = new EmailInfo();
        //    emailInfo.To.Add("nqtngoc@yahoo.com");
        //    emailInfo.To.Add("to1@yahoo.com");
        //    emailInfo.Cc.Add("cc1@yahoo.com");
        //    emailInfo.Cc.Add("cc2@yahoo.com");
        //    emailInfo.Bcc.Add("bcc1@yahoo.com");
        //    emailInfo.Bcc.Add("bcc2@yahoo.com");
        //    emailInfo.Subject = "test send mail";
        //    emailInfo.Body1 = "Dear Sam,";
        //    emailInfo.FormatOption = BodyFormat.RichText;
        //    emailInfo.AttachmentName = "ngocbv attachment name";
        //    emailInfo.Body2 = "Thanks";
        //    renderSetting.ChannelSpecificInfo = emailInfo;

        //    SendEmail(wDoc, renderSetting, ref tempFiles);
        //}

        #region helper methods
        /// <summary>
        /// this computer has outlook or no
        /// </summary>
        /// <returns>true if has</returns>
        private static MailType GetDefaultEmailClientProgram()
        {
            object regValue = null;

            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\mailto\DefaultIcon");
                regValue = regKey.GetValue("");
            }
            catch { }

            if (regValue != null && !string.IsNullOrEmpty(regValue.ToString()))
            {
                string lowerPath = regValue.ToString().ToLower();
                if (lowerPath.Contains("outlook.exe"))
                    return MailType.OutLook;
            }

            return MailType.Unknow;
        }
        #endregion

        #region helper classes

        #region 0. email client program type
        private enum MailType
        {
            Unknow = 0,
            OutLook = 1
        }
        #endregion

        #region 1. word object
        /// <summary>
        /// send email by word
        /// </summary>
        private class WordMail
        {
            public static void SendEmail(Word.Document wDoc, RenderSettings renderSetting)
            {
                switch (renderSetting.Channel)
                {
                    case RenderSettings.ChannelType.Email:
                        SendMail(wDoc, false);
                        break;
                    case RenderSettings.ChannelType.Attachment:
                        switch (renderSetting.Media)
                        {
                            case RenderSettings.MediaType.Docx: // msxml
                                SendMail(wDoc, true);
                                break;
                            case RenderSettings.MediaType.Pdf:
                                wDoc.Application.CommandBars.ExecuteMso(Constants.SendEmailPdfAsAtt);
                                break;
                            case RenderSettings.MediaType.Xps:
                                wDoc.Application.CommandBars.ExecuteMso(Constants.SendEmailXpsAsAtt);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            private static void SendMail(Word.Document wDoc, bool sendMailAttach)
            {
                bool backupOption = wDoc.Application.Options.SendMailAttach;

                wDoc.Application.Options.SendMailAttach = sendMailAttach;
                wDoc.SendMail();

                wDoc.Application.Options.SendMailAttach = backupOption;
            }
        }
        #endregion

        #region 2. outlook object
        /// <summary>
        /// send email by outlook
        /// </summary>
        private class OutLookMail
        {
            public static bool SendEmail(Word.Document wDoc, RenderSettings renderSetting, ref List<string> tempFiles)
            {
                try
                {
                    #region 1. get email info
                    EmailInfo emailInfo = renderSetting.ChannelSpecificInfo as EmailInfo;
                    if (emailInfo == null)
                        return false;
                    #endregion

                    #region 2. initialize outlook object
                    Outlook.Application oApp = new Outlook.Application();
                    Outlook.MailItem oMailItem = oApp.CreateItem(Outlook.OlItemType.olMailItem);
                    #endregion

                    #region 3. set email address and subject
                    string addresses = GetEmailListString(emailInfo.To);
                    if (!string.IsNullOrEmpty(addresses))
                        oMailItem.To = addresses;

                    addresses = GetEmailListString(emailInfo.Cc);
                    if (!string.IsNullOrEmpty(addresses))
                        oMailItem.CC = addresses;

                    addresses = GetEmailListString(emailInfo.Bcc);
                    if (!string.IsNullOrEmpty(addresses))
                        oMailItem.BCC = addresses;

                    oMailItem.Subject = emailInfo.Subject;
                    #endregion

                    #region 4. set email body and attachment
                    switch (renderSetting.Channel)
                    {
                        case RenderSettings.ChannelType.Email:
                            AddDocumentContent(ref oMailItem, wDoc, emailInfo.Body1, emailInfo.Body2);
                            switch (emailInfo.FormatOption)
                            {
                                case BodyFormat.HTML:
                                    oMailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                                    break;
                                case BodyFormat.PlainText:
                                    oMailItem.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;
                                    break;
                                case BodyFormat.RichText:
                                    oMailItem.BodyFormat = Outlook.OlBodyFormat.olFormatRichText;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case RenderSettings.ChannelType.Attachment:
                            oMailItem.Body = string.Format("{0}\r\n{1}", emailInfo.Body1, emailInfo.Body2);
                            AddAttachment(ref oMailItem, wDoc, renderSetting.Media, emailInfo.ValidAttachmentName, ref tempFiles);
                            break;
                        default:
                            break;
                    }
                    #endregion

                    #region 5. send email
                    ((Outlook._MailItem)oMailItem).Display();
                    if (emailInfo.PressSend == SendMode.Auto)
                        ((Outlook._MailItem)oMailItem).Send();
                    #endregion

                    #region 6. release email object
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oMailItem);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oApp);
                    #endregion

                    return true;
                }
                catch (System.Exception ex)
                {
                    Managers.ManagerException mgrExp = new Managers.ManagerException(ErrorCode.ipe_SendOutlookMailError,
                        MessageUtils.Expand(Properties.Resources.ipe_SendOutlookMailError, ex.Message), ex.StackTrace);
                    throw mgrExp;
                }
            }

            private static string GetEmailListString(List<string> emails)
            {
                string addresses = string.Empty;

                if (emails != null && emails.Count > 0)
                {
                    foreach (string email in emails)
                        addresses = addresses + email + ";";
                }

                return addresses;
            }

            private static void AddDocumentContent(ref Outlook.MailItem email, Word.Document wDoc, string body1, string body2)
            {
                if (email == null || wDoc == null)
                    throw new System.Exception("Email or Document is null");

                // initalize outlook object
                Outlook.Inspector oInspector = email.GetInspector;
                if (oInspector == null)
                    throw new System.Exception("Cannot get inspector of email");

                Word.Document oEditor = oInspector.WordEditor;

                if (oEditor == null)
                    throw new System.Exception("Cannot get editor of email");

                // clean clipboard
                System.Windows.Forms.Clipboard.Clear();

                // copy content from word document to clipboard
                wDoc.Range().Select();
                wDoc.Range().Copy();
                wDoc.Range().Move(Word.WdUnits.wdCharacter, 1);

                // paste content from clipboard to mail
                oEditor.Range().PasteAndFormat(Word.WdRecoveryType.wdFormatOriginalFormatting);

                // insert body1
                if (!string.IsNullOrEmpty(body1))
                    oEditor.Range().InsertBefore(body1 + "\r\n");

                // insert body2
                if (!string.IsNullOrEmpty(body2))
                    oEditor.Range().InsertAfter(body2);

                // release
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oEditor);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oInspector);

                // clean clipboard
                System.Windows.Forms.Clipboard.Clear();
            }

            private static void AddAttachment(ref Outlook.MailItem email, Word.Document wDoc,
                RenderSettings.MediaType mediaType, string attachName, ref List<string> tempFiles)
            {
                if (wDoc == null || email == null)
                    return;

                string filePath = string.IsNullOrEmpty(attachName) ? wDoc.FullName : attachName + FileExtension.Xml; // mwxml
                object missing = System.Type.Missing;

                switch (mediaType)
                {
                    case RenderSettings.MediaType.Docx: // msxml
                        filePath = AssetManager.FileAdapter.GetFilePath(
                            string.Format("{0}\\{1}",
                                AssetManager.FileAdapter.TemporaryFolderPath,
                                System.IO.Path.GetFileNameWithoutExtension(filePath)),
                            FileExtension.Docx); // msxml
                        wDoc.SaveAs(filePath, Word.WdSaveFormat.wdFormatXMLDocument);
                        tempFiles.Add(filePath);
                        break;
                    case RenderSettings.MediaType.Pdf:
                        filePath = AssetManager.FileAdapter.GetFilePath(
                            string.Format("{0}\\{1}",
                                AssetManager.FileAdapter.TemporaryFolderPath,
                                System.IO.Path.GetFileNameWithoutExtension(filePath)),
                            FileExtension.Pdf);
                        wDoc.ExportAsFixedFormat(filePath, Word.WdExportFormat.wdExportFormatPDF);
                        tempFiles.Add(filePath);
                        break;
                    case RenderSettings.MediaType.Xps:
                        filePath = AssetManager.FileAdapter.GetFilePath(
                            string.Format("{0}\\{1}",
                                AssetManager.FileAdapter.TemporaryFolderPath,
                                System.IO.Path.GetFileNameWithoutExtension(filePath)),
                            FileExtension.Xps);
                        wDoc.ExportAsFixedFormat(filePath, Word.WdExportFormat.wdExportFormatXPS);
                        tempFiles.Add(filePath);
                        break;
                    default:
                        filePath = string.Empty;
                        break;
                }

                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                    email.Attachments.Add(filePath, missing, missing, missing);
            }
        }
        #endregion

        #endregion
    }
}
