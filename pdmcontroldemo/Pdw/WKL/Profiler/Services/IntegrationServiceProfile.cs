using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pdw.Core;
using Pdwx.DataObjects;

namespace Pdw.WKL.Profiler.Services
{
    public class IntegrationServiceProfile:BaseProfile
    {
        public string CheckCorrectContent_IFilePath { get; set; }

        public List<DomainMatch> CheckMatchWithDomain_OListMatch { get; set; }

        public Dictionary<string, string> ValidateInternalBM_IListBM { get; set; }
        public bool                       ValidateInternalBM_IIsUpdate { get; set; }
        public List<string>               ValidateInternalBM_OListError { get; set; }

        public InternalBookmarkItem AddInternalBM_IBookmark { get; set; }

        public string TemplateFileName { get; set; }

        public PdwrInfo PdwrInfo { get; set; }

        public USCItem UscItem { get; set; }
    }
}
