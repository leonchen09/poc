
using System.Collections.Generic;

using ProntoDoc.Framework.Utils;
using ProntoDoc.Framework.CoreObject.PdwxObjects;

using Wkl = Pdw.WKL.DataController.MainController;

namespace Pdw.Services.Template
{
    class GenChecksumHelper
    {
        public string GenChecksum(string osqlString, List<ChecksumInfoItem> checksumItems, TemplateType templateType, 
            string dscColor, bool hasDsc, string filePath)
        {
            // 1. prepare service
            MainService mainService = new MainService();
            Integrity.Validator validatorService = new Integrity.Validator();

            // 2. Plugin Id
            string pluginId = Wkl.MainCtrl.CommonCtrl.CommonProfile.PluginInfo.Id + " | " + Wkl.MainCtrl.CommonCtrl.CommonProfile.PluginInfo.Version;

            // 3. Plugin Name
            string pluginName = Wkl.MainCtrl.CommonCtrl.CommonProfile.PluginInfo.Name;

            // 4. User name
            string userName = validatorService.GetCurrentUserName();

            // 5. Checksum content
            string strInternalBm = mainService.PropertyService.GetInternalBookmarkString();
            string strChecksum = validatorService.GenCheckSum(pluginId, pluginName, userName, strInternalBm, osqlString);

            // 6. make checksum object
            ChecksumInfo objChecksum = new ChecksumInfo(pluginId, pluginName, userName, strChecksum);
            
            // 7. add other information
            objChecksum.ChecksumInfoItems = checksumItems;

            // 8. template type
            objChecksum.TemplateType = templateType;

            // 9. add computer name
            objChecksum.ComputerName = validatorService.GetComputerName();

            objChecksum.DocumentSpecificColor = dscColor;
            objChecksum.HasDocumentSpecific = hasDsc;
            objChecksum.FilePath = filePath;

            return ObjectSerializeHelper.SerializeToString<ChecksumInfo>(objChecksum);
        }
    }
}
