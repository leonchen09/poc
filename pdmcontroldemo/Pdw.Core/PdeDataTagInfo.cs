
namespace Pdw.Core
{
    [System.Serializable]
    public class PdeDataTagInfo
    {
        public string ExcelName { get; set; }
        public string DisplayName { get; set; }
        public string BookmarkName { get; set; }
        public string STN { get; set; }
        public string ParentName { get; set; }
        public ProntoDoc.Framework.CoreObject.PdwxObjects.MapType MapType { get; set; }

        public PdeDataTagInfo()
        {
            MapType = ProntoDoc.Framework.CoreObject.PdwxObjects.MapType.SingleCell;
        }

        public PdeDataTagInfo(ProntoDoc.Framework.CoreObject.PdwxObjects.MapType mapType, 
            string stn, string parentName, string excelName, string displayName, string bmName)
        {
            STN = stn;
            ParentName = parentName;
            ExcelName = excelName;
            DisplayName = displayName;
            BookmarkName = bmName;
            MapType = mapType;
        }

        #region ICloneable Members

        public PdeDataTagInfo Clone()
        {
            return MemberwiseClone() as PdeDataTagInfo;
        }

        #endregion
    }
}
