
using System;
using System.Linq;
using System.Collections.Generic;

using ProntoDoc.Framework.CoreObject.DataSegment;

namespace Pdw.SharedMemoryWrapper
{
    public class DataSegmentInfo
    {
        public List<DSHeaderInfo> DSHeaderInfos { get; set; }
        public List<Icon> Icons { get; set; }
        public List<LegendInfo> LegendInfos { get; set; }

        public DataSegmentInfo()
        {
            DSHeaderInfos = new List<DSHeaderInfo>();
            Icons = new List<Icon>();
            LegendInfos = new List<LegendInfo>();
        }

        public LegendInfo GetLegendByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || LegendInfos == null)
                return null;

            return LegendInfos.FirstOrDefault(lg => name.Equals(lg.ButtonName, StringComparison.OrdinalIgnoreCase));
        }

        public Icon GetIconByName(string dbid, string name)
        {
            if (Icons == null || string.IsNullOrWhiteSpace(dbid) || string.IsNullOrWhiteSpace(name))
                return null;

            return Icons.FirstOrDefault(ic => name.Equals(ic.Name, StringComparison.OrdinalIgnoreCase) &&
                dbid.Equals(ic.DBID));
        }
    }
}
