using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pdw.FormControls.Design;

namespace Pdw.FormControls
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISelectable
    {
        TemplateItemCollection Items { get; }

        string DataBindingSelectPath { get; set; }

        string DataBindingSelectKey { get; set; }
    }
}
