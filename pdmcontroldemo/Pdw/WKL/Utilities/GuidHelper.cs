using System;
using System.Collections.Generic;

using Pdw.WKL.Profiler;

namespace Pdw.WKL.Utilities
{
    class GuidHelper
    {
        public static string GenKey(Dictionary<string, BaseProfile> datas)
        {
            string key = Guid.NewGuid().ToString();

            if (datas != null && datas.Count > 0)
            {
                while(datas.ContainsKey(key))
                    key = Guid.NewGuid().ToString();
            }

            return key;
        }
    }
}
