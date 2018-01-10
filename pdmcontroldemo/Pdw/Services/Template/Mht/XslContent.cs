
using System.Text;

namespace Pdw.Services.Template.Mht
{
    class XslContent
    {
        public const string PrefixVariant = "$";
        /// <summary>
        /// add [rootElement] into template match path
        /// </summary>
        /// <param name="rootElement">PdwData</param>
        /// <returns></returns>
        public static string XslHeader(string rootElement = ProntoDoc.Framework.CoreObject.FrameworkConstants.PdwDataRootName,
            bool isBreakLine = true)
        {
            StringBuilder builder = new StringBuilder();

            if (isBreakLine)
            {
                builder.AppendLine(XslFormat.Xml);
                builder.AppendLine(XslFormat.StartStylesheet);
                builder.AppendLine(XslFormat.Output);
            }
            else
            {
                builder.Append(XslFormat.Xml);
                builder.Append(XslFormat.StartStylesheet);
                builder.Append(XslFormat.Output);
            }
            builder.Append(string.Format(XslFormat.StartTemplate, rootElement) + XslFormat.MarkStartXslString);

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string XslFooter(bool isBreakLine = true)
        {
            StringBuilder builder = new StringBuilder();

            if (isBreakLine)
            {
                builder.AppendLine(XslFormat.MarkEndXslString);
                builder.AppendLine(XslFormat.EndTemplate);
            }
            else
            {
                builder.Append(XslFormat.MarkEndXslString);
                builder.Append(XslFormat.EndTemplate);
            }
            builder.Append(XslFormat.EndStylesheet);

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string XslValueOfTag(string path, bool isBreakLine = true, bool isVariant = false)
        {
            StringBuilder builder = new StringBuilder();
            path = isVariant ? PrefixVariant + path : path;

            if (isBreakLine)
            {
                builder.AppendLine(XslFormat.MarkEndXslString);
                builder.AppendLine(string.Format(XslFormat.XslValueOf, path));
                builder.Append(XslFormat.MarkStartXslString);
            }
            else
            {
                builder.Append(XslFormat.MarkEndXslString);
                builder.Append(string.Format(XslFormat.XslValueOf, path));
                builder.Append(XslFormat.MarkStartXslString);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string XslStartForeachTag(string path, bool isBreakLine = true)
        {
            StringBuilder builder = new StringBuilder();

            if (isBreakLine)
            {
                builder.AppendLine(XslFormat.MarkEndXslString);
                builder.AppendLine(string.Format(XslFormat.XslStartForeach, path));
            }
            else
            {
                builder.Append(XslFormat.MarkEndXslString);
                builder.Append(string.Format(XslFormat.XslStartForeach, path));
            }
            builder.Append(XslFormat.MarkStartXslString);

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="variantName"></param>
        /// <param name="variantValue"></param>
        /// <param name="isBreakLine"></param>
        /// <returns></returns>
        public static string XslStartForeachTag(string path, string variantName, string variantValue, bool isBreakLine = true)
        {
            StringBuilder builder = new StringBuilder();

            if (isBreakLine)
            {
                builder.AppendLine(XslFormat.MarkEndXslString);
                builder.AppendLine(string.Format(XslFormat.XslStartForeach, path));
                builder.AppendLine(string.Format(XslFormat.XslVariant, variantName, variantValue));
            }
            else
            {
                builder.Append(XslFormat.MarkEndXslString);
                builder.Append(string.Format(XslFormat.XslStartForeach, path));
                builder.Append(string.Format(XslFormat.XslVariant, variantName, variantValue));
            }
            builder.Append(XslFormat.MarkStartXslString);

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="variantName"></param>
        /// <param name="variantValue"></param>
        /// <param name="sort"></param>
        /// <param name="isBreakLine"></param>
        /// <returns></returns>
        public static string XslStartForeachTag(string path, string variantName, string variantValue, string sort, bool isBreakLine = true)
        {
            StringBuilder builder = new StringBuilder();

            if (isBreakLine)
            {
                builder.AppendLine(XslFormat.MarkEndXslString);
                builder.AppendLine(string.Format(XslFormat.XslStartForeach, path));
                if (!string.IsNullOrEmpty(sort))
                    builder.AppendLine(sort);
                builder.AppendLine(string.Format(XslFormat.XslVariant, variantName, variantValue));
            }
            else
            {
                builder.Append(XslFormat.MarkEndXslString);
                builder.Append(string.Format(XslFormat.XslStartForeach, path));
                if (!string.IsNullOrEmpty(sort))
                    builder.Append(sort);
                builder.Append(string.Format(XslFormat.XslVariant, variantName, variantValue));
            }
            builder.Append(XslFormat.MarkStartXslString);

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string XslSortTag(string path, string order, bool isBreakLine = true)
        {
            StringBuilder builder = new StringBuilder();

            if (isBreakLine)
                builder.AppendLine(string.Format(XslFormat.XslSort, path, order));
            else
                builder.Append(string.Format(XslFormat.XslSort, path, order));

            return builder.ToString();
        }

        /// <summary>
        /// &lt;xsl:if test="[test]">
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public static string XslStartIfTag(string test, bool isBreakLine = true)
        {
            StringBuilder builder = new StringBuilder();
            if (isBreakLine)
            {
                builder.AppendLine(XslFormat.MarkEndXslString);
                builder.AppendLine(string.Format(XslFormat.XslStartIf, test));
            }
            else
            {
                builder.Append(XslFormat.MarkEndXslString);
                builder.Append(string.Format(XslFormat.XslStartIf, test));
            }
            builder.Append(XslFormat.MarkStartXslString);

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isBefore"></param>
        /// <returns></returns>
        public static string InsertBreakLine(string content, bool isBefore)
        {
            StringBuilder builder = new StringBuilder();
            if (isBefore)
            {
                builder.AppendLine("");
                builder.Append(content);
            }
            else
            {
                builder.AppendLine(content);
            }

            return builder.ToString();
        }

        //
        public class XslFormat
        {
            /// <summary>
            /// &lt;?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>
            /// </summary>
            public const string Xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";

            /// <summary>
            /// &lt;xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">
            /// </summary>
            public const string StartStylesheet = "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">";

            /// <summary>
            /// &lt;/xsl:stylesheet>
            /// </summary>
            public const string EndStylesheet = "</xsl:stylesheet>";

            /// <summary>
            /// &lt;xsl:output method=\"text\" />
            /// </summary>
            public const string Output = "<xsl:output method=\"text\" />";

            /// <summary>
            /// &lt;xsl:template match="/{0}">
            /// </summary>
            public const string StartTemplate = "<xsl:template match=\"/{0}\">";

            /// <summary>
            /// &lt;/xsl:template>
            /// </summary>
            public const string EndTemplate = "</xsl:template>";

            /// <summary>
            /// &lt;![CDATA[
            /// </summary>
            public const string MarkStartXslString = "<![CDATA[";

            /// <summary>
            /// ]]>
            /// </summary>
            public const string MarkEndXslString = "]]>";

            /// <summary>
            /// &lt;xsl:for-each select="{0}">
            /// </summary>
            public const string XslStartForeach = "<xsl:for-each select=\"{0}\">";

            /// <summary>
            /// &lt;xsl:sort select="{0}" order="{1}" />
            /// </summary>
            public const string XslSort = "<xsl:sort select=\"{0}\" order=\"{1}\" />";

            /// <summary>
            /// ]]>&lt;/xsl:for-each><![CDATA[
            /// </summary>
            public const string XslEndForeach = "]]></xsl:for-each><![CDATA[";

            /// <summary>
            /// &lt;xsl:if test="{0}">
            /// </summary>
            public const string XslStartIf = "<xsl:if test=\"{0}\">";

            /// <summary>
            /// ]]>&lt;//xsl:if><![CDATA[
            /// </summary>
            public const string XslEndIf = "]]></xsl:if><![CDATA[";

            /// <summary>
            /// &lt;xsl:value-of select="{0}" />
            /// </summary>
            public const string XslValueOf = "<xsl:value-of select=\"{0}\" />";

            /// <summary>
            /// &lt;xsl:variable name="{0}" select="{1}"/>
            /// </summary>
            public const string XslVariant = "<xsl:variable name=\"{0}\" select=\"{1}\"/>";

            /// <summary>
            /// /
            /// </summary>
            public const string XslPath = "/";
        }
    }
}