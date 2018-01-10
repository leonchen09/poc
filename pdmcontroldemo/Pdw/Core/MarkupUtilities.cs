
using System.Linq;
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject.DataSegment;
using ProntoDoc.Framework.CoreObject.PdwxObjects;

namespace Pdw.Core
{
    public class MarkupUtilities : BaseMarkupUtilities
    {
        /// <summary>
        /// Gen text (bookmark value) for xsl tag
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GenTextXslTag(string text, XsltType xslType, bool isOpenTag)
        {
            switch (xslType)
            {
                case XsltType.Foreach:
                    if (isOpenTag)
                        return string.Format(ProntoMarkup.ValueStartForeach, text);
                    else
                        return string.Format(ProntoMarkup.ValueEndForeach, text);
                case XsltType.If:
                    if (isOpenTag)
                        return string.Format(ProntoMarkup.ValueStartIf, text);
                    else
                        return string.Format(ProntoMarkup.ValueEndIf, text);
                case XsltType.Select:
                    return string.Format(ProntoMarkup.ValueSelect, text);
                case XsltType.Comment:
                    return ProntoMarkup.ValueComment;
                default:
                    break;
            }

            return text;
        }

        /// <summary>
        /// Gen xsl tag for bookmark
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefix"></param>
        /// <param name="xslType"></param>
        /// <returns></returns>
        public static string GenKeyForXslTag(string name, XsltType xslType, bool isOpenTag)
        {
            switch (xslType)
            {
                case XsltType.If:
                    if (isOpenTag)
                        return string.Format("{0}{1}", name, ProntoMarkup.KeyStartIf);
                    else
                        return string.Format("{0}{1}", name, ProntoMarkup.KeyEndIf);
                case XsltType.Foreach:
                    if (isOpenTag)
                        return string.Format("{0}{1}", name, ProntoMarkup.KeyStartForeach);
                    else
                        return string.Format("{0}{1}", name, ProntoMarkup.KeyEndForeach);
                case XsltType.Select:
                    return string.Format("{0}{1}", name, ProntoMarkup.KeySelect);
                case XsltType.Comment:
                    return string.Format("{0}{1}", name, ProntoMarkup.KeyComment);
            }

            return name;
        }

        /// <summary>
        /// check bookmark (with name and value) is exist on data domain or no
        /// </summary>
        /// <param name="bmName"></param>
        /// <param name="bmValue"></param>
        /// <param name="dataDomain"></param>
        /// <returns></returns>
        public static bool IsExistOnDomain(InternalBookmarkItem ibm, Dictionary<string, DSTreeView> dataDomain, bool checkUSC)
        {
            if (checkUSC && (ibm.Key.EndsWith(ProntoMarkup.KeyEndIf) || ibm.Key.EndsWith(ProntoMarkup.KeyStartIf)
                || ibm.Key.EndsWith(ProntoMarkup.KeyEndForeach) || ibm.Key.EndsWith(ProntoMarkup.KeyStartForeach)))
                return true;

            string bizName = MarkupUtilities.GetOriginalBizName(ibm.Key, ibm.BizName);

            if (dataDomain != null && dataDomain.ContainsKey(bizName))
            {
                return ((dataDomain[bizName].UniqueName == null) || (dataDomain[bizName].UniqueName.Equals(ibm.UniqueName)));

            }

            return false;
        }

        public static string CreateAlternativeText(string bmName, string bmValue)
        {
            string inlineKey = CreateInlineKey(bmName);

            return string.Format("{0}#{1}", inlineKey, bmValue);
        }

        public static string CreateInlineKey(string bmName)
        {
            return string.Format("{0}{1}", ProntoMarkup.ImageHolderKey, bmName);
        }

        public static string GetBizName(string alternativeText)
        {
            int index = alternativeText.IndexOf("#");
            if (index >= 0)
                return alternativeText.Substring(index + 1);

            return alternativeText;
        }

        /// <summary>
        /// get biz name of image bookmark
        /// </summary>
        /// <param name="bmName"></param>
        /// <param name="inlineShapes"></param>
        /// <returns>string.empty if not existed</returns>
        public static string GetBizNameOfBookmarkImage(string bmName, Microsoft.Office.Interop.Word.InlineShapes inlineShapes)
        {
            foreach (Microsoft.Office.Interop.Word.InlineShape inline in inlineShapes)
            {
                if (inline.AlternativeText.StartsWith(CreateInlineKey(bmName)))
                    return GetBizName(inline.AlternativeText);
            }

            return string.Empty;
        }

        public static string GetRangeText(Microsoft.Office.Interop.Word.Range range, bool isIncludeHidenText = true)
        {
            if (range == null)
                return string.Empty;

            if (isIncludeHidenText)
                range.TextRetrievalMode.IncludeHiddenText = true;

            return range.Text;
        }

        public static TemplateType GetTemplateType(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if(filePath.EndsWith(FileExtension.ProntoDocumentPdz, System.StringComparison.OrdinalIgnoreCase))
                    return TemplateType.Pdz;

                if(filePath.EndsWith(FileExtension.ProntoDocumenentWord, System.StringComparison.OrdinalIgnoreCase))
                    return TemplateType.Pdw;

                if(filePath.EndsWith(FileExtension.ProntoDocumentMht, System.StringComparison.OrdinalIgnoreCase))
                    return TemplateType.Pdh;

                if (filePath.EndsWith(FileExtension.ProntoDocumentMobile, System.StringComparison.OrdinalIgnoreCase))
                    return TemplateType.Pdm;
            }

            return TemplateType.None;
        }
    }
}
