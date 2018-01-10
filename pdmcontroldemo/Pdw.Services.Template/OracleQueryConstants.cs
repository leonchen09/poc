
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject;

namespace Pdw.Services.Template
{
    class OracleQueryConstants : QueryConstants
    {
        /// <summary>
        ///  :{0} as {1}
        /// </summary>
        public const string SQLSystemInfo = " :{0} as {1}";

        /// <summary>
        /// sysdate
        /// </summary>
        public const string SQLSystemParamGeneratedTime = "sysdate ";

        /// <summary>
        ///  :{0}
        /// </summary>
        public const string SQLSystemParam = " :{0}";

        /// <summary>
        ///  '+ :{0} +'
        /// </summary>
        public const string SQLSystemParamWithIn = " '+ :{0} +'";

        /// <summary>
        ///  '+ :{0} +' as {1}
        /// </summary>
        public const string SQLSystemInfoWithIn = " '+ :{0} +' as {1}";

        /// <summary>
        ///  Getdate() as {0}
        /// </summary>
        public const string SQLGeneratedTime = " sysdate as {0}";

        /// <summary>
        /// Dictionary of paramameters for Select clause
        /// key: FrameworkConstants.PdwWatermark and ProntoDoc.Framework.CoreObject.PluginSystemInfo.* (Except GeneratedTime)
        /// value: param 
        /// </summary>
        public static Dictionary<string, string> DicSelectClauseParamName = new Dictionary<string, string>
        {
            {FrameworkConstants.PdwWatermark, ":" + FrameworkConstants.PdwWatermark.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.ComputerName, ":" + FrameworkConstants.PluginSystemInfo.ComputerName.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.RenderRequestID, ":" + FrameworkConstants.PluginSystemInfo.RenderRequestID.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.TemplateID, ":" + FrameworkConstants.PluginSystemInfo.TemplateID.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.TemplateName, ":" + FrameworkConstants.PluginSystemInfo.TemplateName.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.TemplateVersion, ":" + FrameworkConstants.PluginSystemInfo.TemplateVersion.Replace(" ", "")},
            {FrameworkConstants.PluginSystemInfo.UserId, ":" + FrameworkConstants.PluginSystemInfo.UserId.Replace(" ", "")},            
            {FrameworkConstants.RenderArgumentX, ":" + FrameworkConstants.RenderArgumentX},
            {FrameworkConstants.RenderArgumentY, ":" + FrameworkConstants.RenderArgumentY},
        };
    }
}
