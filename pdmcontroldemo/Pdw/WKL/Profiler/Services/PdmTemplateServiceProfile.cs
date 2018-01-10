using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pdw.FormControls;
using Pdw.Core;

namespace Pdw.WKL.Profiler.Services
{
    /// <summary>
    /// HACK:FORM CONTROLS
    /// </summary>
    public class PdmServiceProfile : BaseProfile
    {
        public ControlBase Control { get; set; }

        public FormControlType ControlType { get; set; }

        public string DatabindingPath { get; set; }

        public InternalBookmarkItem BindingBookmark { get; set; }

        public string CurrentPropertyName { get; set; }

        public bool IsAdding { get; set; }
    }
}
