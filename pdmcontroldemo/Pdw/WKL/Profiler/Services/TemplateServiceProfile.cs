using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pdw.WKL.Profiler.Services
{
    public class TemplateServiceProfile: BaseProfile
    {
       
        public List<string> CheckWordBody_ODeletedTags { get; set; }

        public string XmlContent { get; set; }

        public string FilePath { get; set; }

        public string TemplateName { get; set; }
      
    }
}
