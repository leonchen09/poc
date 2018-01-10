
using System.Collections.Generic;

namespace Pdw.Core
{
    public class BaseMarkupUtilities
    {
        /// <summary>
        /// get original biz name
        /// </summary>
        /// <param name="bmValue"></param>
        /// <param name="bmName"></param>
        /// <returns></returns>
        public static string GetOriginalBizName(string bmName, string bmValue)
        {
            if (bmName.EndsWith(BaseProntoMarkup.KeyEndForeach))
                return RemoveChars(bmValue, 2, 1);
            else if (bmName.EndsWith(BaseProntoMarkup.KeyEndIf))
                return RemoveChars(bmValue, 2, 1);
            else if (bmName.EndsWith(BaseProntoMarkup.KeySelect) || bmName.EndsWith(BaseProntoMarkup.KeyImage))
                return RemoveChars(bmValue, 1, 2);
            else if (bmName.EndsWith(BaseProntoMarkup.KeyStartForeach))
                return RemoveChars(bmValue, 1, 1);
            else if (bmName.EndsWith(BaseProntoMarkup.KeyStartIf))
                return RemoveChars(bmValue, 1, 1);

            return bmValue;
        }

        /// <summary>
        /// get original biz name of select tag
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetOriginalBizName(string text)
        {
            string bizName = text;
            if (!string.IsNullOrEmpty(bizName))
            {
                if (bizName.StartsWith(BaseProntoMarkup.TagOpen))
                    bizName = RemoveChars(bizName, BaseProntoMarkup.TagOpen.Length, 0);
                if (bizName.EndsWith(BaseProntoMarkup.TagClose))
                    bizName = RemoveChars(bizName, 0, BaseProntoMarkup.TagClose.Length);
            }

            return bizName;
        }

        /// <summary>
        /// Remove some characters in left and right of string
        /// </summary>
        /// <param name="value">input string value</param>
        /// <param name="left">Number of characters in the left must be removed</param>
        /// <param name="right">Number of characters in the right must be removed</param>
        /// <returns>output string</returns>
        public static string RemoveChars(string value, int left, int right)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > left)
            {
                string temp = value.Remove(0, left);
                if (temp.Length > right && right > 0)
                    temp = temp.Remove(temp.Length - right);

                return temp;
            }

            return value;
        }

        /// <summary>
        /// check existed of the value in list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsExistOnList(List<string> list, string value)
        {
            if (list != null)
                foreach (string item in list)
                    if (item == value)
                        return true;

            return false;
        }

        /// <summary>
        /// encode xmlValue for xml format
        /// </summary>
        /// <param name="xmlValue"></param>
        /// <returns></returns>
        public static string XmlEncode(string xmlValue)
        {
            return ProntoDoc.Framework.Utils.XmlHelper.EncodeLocalName(xmlValue);
        }

        public static Dictionary<string, OrderByType> GetOldOrderBy(string foreachText, bool keepOriginal)
        {
            Dictionary<string, OrderByType> sorted = new Dictionary<string, OrderByType>();

            string name = foreachText;
            if (name.StartsWith(BaseConstants.OrderBy.ForeachSortMark))
            {
                name = name.Substring(BaseConstants.OrderBy.ForeachSortMark.Length);
                name = name.Remove(name.Length - BaseConstants.OrderBy.CloseTag.Length);
                string[] items = name.Split(new string[] { BaseConstants.OrderBy.Delimiter }, System.StringSplitOptions.RemoveEmptyEntries);
                for (int index = 0; index < items.Length; index++)
                {
                    int splitIndex = items[index].LastIndexOf(BaseConstants.OrderBy.Concat);
                    string tagName = items[index].Substring(0, splitIndex);
                    if (!keepOriginal)
                        tagName = GetOriginalBizName(tagName);
                    if (!sorted.ContainsKey(tagName))
                    {
                        string orderByValue = items[index].Substring(splitIndex + 1);
                        switch (orderByValue)
                        {
                            case BaseConstants.OrderBy.AscMark:
                                sorted.Add(tagName, OrderByType.Asc);
                                break;
                            case BaseConstants.OrderBy.DescMark:
                                sorted.Add(tagName, OrderByType.Desc);
                                break;
                            default:
                                sorted.Add(tagName, OrderByType.None);
                                break;
                        }
                    }
                }
            }

            return sorted;
        }

        /// <summary>
        /// check bookmark is generate by plugin or no
        /// </summary>
        /// <param name="bmName"></param>
        /// <returns></returns>
        public static bool IsProntoDocBookmark(string bmName, InternalBookmark ibm, bool isIncludeComment = false)
        {
            if (string.IsNullOrWhiteSpace(bmName))
                return false;

            if (ibm != null)
            {
                InternalBookmarkItem ibmItem = ibm.GetInternalBookmarkItem(bmName);
                if (ibmItem != null)
                    return true;
            }

            if (isIncludeComment)
                return IsProntoDocCommentBookmark(bmName);

            return false;
        }

        /// <summary>
        /// check the bookmark is comment or no
        /// </summary>
        /// <param name="bmName"></param>
        /// <returns></returns>
        public static bool IsProntoDocCommentBookmark(string bmName)
        {
            if (string.IsNullOrWhiteSpace(bmName))
                return false;

            bool hasStart = bmName.StartsWith("P_");
            bool hasEnd = bmName.EndsWith(BaseProntoMarkup.KeyComment);

            return hasStart && hasEnd;
        }

        /// <summary>
        /// check the bookmark is pdetag or no
        /// </summary>
        /// <param name="bmName"></param>
        /// <returns></returns>
        public static bool IsPdeTagBookmark(string bmName)
        {
            if (string.IsNullOrWhiteSpace(bmName))
                return false;

            return bmName.StartsWith("pde_");
        }

        public static bool IsProntoDocDocumentSpecificBookmark(string bmName)
        {
            return !string.IsNullOrWhiteSpace(bmName) && bmName.Contains(BaseProntoMarkup.KeyMarkupDocumentSpecific);
        }

        public static string ReverseString(string text)
        {
            char[] array = text.ToCharArray();
            System.Array.Reverse(array);

            return (new string(array));
        }

        /// <summary>
        /// get xsd type
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string SqlDbType2XsdDataType(ProntoDoc.Framework.CoreObject.SQLDBType sqlType)
        {
            // refer: http://www.w3.org/2001/XMLSchema.xsd
            if (sqlType != null)
            {
                switch (sqlType.Name)
                {
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.CHAR:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.NCHAR:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.NTEXT:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.NVARCHAR:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.NVARCHAR2:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.TEXT:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.VARCHAR:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.VARCHAR2:
                        return "string";
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.BIT:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.BIGINT:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.INT:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.SMALLINT:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.SMALLMONEY:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.MONEY:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.TINYINT:
                        return "int";
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.DECIMAL:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.FLOAT:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.NUMBER:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.NUMERIC:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.REAL:
                        return "decimal";
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.DATE:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.DATETIME:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.SMALLDATETIME:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.TIMESTAMP:
                        return "date";
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.BOOLEAN:
                        return "boolean";
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.UNIQUEIDENTIFIER:
                        return "string";// todo: ngocbv_check type (keybase)
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.IMAGE:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.ASIMAGE:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.BINARY:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.VARBINARY:
                        return "string"; // todo: ngocbv_check type
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.ARRAY:
                        return "string"; // todo: ngocbv_check type
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.BLOB:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.CLOB:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.NCLOB:
                        return "string"; // todo: ngocbv_check type
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.PARAMETER:
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.SQL_VARIANT:
                        return "string"; // todo: ngocbv_check type
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.UNKNOW_TYPE:
                        return "string"; // todo: ngocbv_check type
                    case ProntoDoc.Framework.CoreObject.SQLTypeName.XML:
                        return "string"; // todo: ngocbv_check type
                }
            }

            return "string";
        }
    }
}
