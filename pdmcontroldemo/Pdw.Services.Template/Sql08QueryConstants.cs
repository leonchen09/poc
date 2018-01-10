
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject;

namespace Pdw.Services.Template
{
    class Sql08QueryConstants : QueryConstants
    {
        public class XmlAuto
        {
            /// <summary>
            ///  {0} as {1}
            /// </summary>
            public const string SQLFieldFormat = " {0} as {1}";

            /// <summary>
            ///  For xml auto , root('{0}')
            /// </summary>
            public const string ForXMLAuto = " For xml auto , root('{0}')";

            /// <summary>
            ///  For xml auto , root(''{0}'')
            /// </summary>
            public const string ForXMLAutoIn = " For xml auto , root(''{0}'')";

            /// <summary>
            ///  FOR XML RAW ( '{0}' ), root('{1}')
            /// </summary>
            public const string ForXMLRaw = " FOR XML RAW ( '{0}' ), root('{1}')";

            /// <summary>
            ///  FOR XML RAW ( ''{0}'' ), root(''{1}'')
            /// </summary>
            public const string ForXMLRawIn = " FOR XML RAW ( ''{0}'' ), root(''{1}'')";
        }

        /// <summary>
        ///  @{0} as {1}
        /// </summary>
        public const string SQLSystemInfo = " @{0} as {1}";

        /// <summary>
        /// Getdate()
        /// </summary>
        public const string SQLSystemParamGeneratedTime = "Getdate() ";

        /// <summary>
        ///  @{0}
        /// </summary>
        public const string SQLSystemParam = " @{0}";

        /// <summary>
        ///  '+ @{0} +'
        /// </summary>
        public const string SQLSystemParamWithIn = " '+ @{0} +'";

        /// <summary>
        ///  '+ @{0} +' as {1}
        /// </summary>
        public const string SQLSystemInfoWithIn = " '+ @{0} +' as {1}";

        /// <summary>
        ///  Getdate() as {0}
        /// </summary>
        public const string SQLGeneratedTime = " Getdate() as {0}";

        /// <summary>
        /// , binary base64 
        /// </summary>
        public const string SQLBase64 = ", binary base64 ";

        /// <summary>
        /// exec ('{0}')
        /// </summary>
        public const string SQLWithIn = "exec ('{0}')";

        /// <summary>
        /// for xml path('{0}'), root('{1}') 
        /// </summary>
        public const string PathForXmlPathRoot = "for xml path('{0}'), root('{1}') ";

        public const string ForXMLAuto = " For xml auto , root('" + FrameworkConstants.PdwDataRootName + "')";
        public const string ForXMLAutoIn = " For xml auto , root(''" + FrameworkConstants.PdwDataRootName + "'')";
        public const string ForXMLRaw = " FOR XML RAW ( '{0}' ), root('" + FrameworkConstants.PdwDataRootName + "')";
        public const string ForXMLRawIn = " FOR XML RAW ( ''{0}'' ), root(''" + FrameworkConstants.PdwDataRootName + "'')";

        /// <summary>
        /// for xml path('{0}'), type
        /// </summary>
        public const string ForXmlPathType = "for xml path('{0}'), type ";

        /// <summary>
        /// for xml path('{0}'), root('PdwData')
        /// </summary>
        public const string ForXmlPathRoot = "for xml path('{0}'), root('" + FrameworkConstants.PdwDataRootName + "') ";

        /// <summary>
        /// Dictionary of paramameters for Select clause
        /// key: FrameworkConstants.PdwWatermark and ProntoDoc.Framework.CoreObject.PluginSystemInfo.* (Except GeneratedTime)
        /// value: param 
        /// </summary>
        public static Dictionary<string, string> DicSelectClauseParamName = new Dictionary<string, string>
        {
            {FrameworkConstants.PdwWatermark, "@" + FrameworkConstants.PdwWatermark.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.ComputerName, "@" + FrameworkConstants.PluginSystemInfo.ComputerName.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.RenderRequestID, "@" + FrameworkConstants.PluginSystemInfo.RenderRequestID.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.TemplateID, "@" + FrameworkConstants.PluginSystemInfo.TemplateID.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.TemplateName, "@" + FrameworkConstants.PluginSystemInfo.TemplateName.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.TemplateVersion, "@" + FrameworkConstants.PluginSystemInfo.TemplateVersion.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.UserId, "@" + FrameworkConstants.PluginSystemInfo.UserId.Replace(" ", "")},            
            {FrameworkConstants.RenderArgumentX, "@" + FrameworkConstants.RenderArgumentX},
            {FrameworkConstants.RenderArgumentY, "@" + FrameworkConstants.RenderArgumentY},
        };
    }
}
