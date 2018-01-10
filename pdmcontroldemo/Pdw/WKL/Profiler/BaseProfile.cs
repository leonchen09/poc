using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pdw.Core;

namespace Pdw.WKL.Profiler
{
    public class BaseProfile
    {       

        public object Result { get; set; }

        public object Message { get; set; }

        public object Error { get; set; }

        public ServiceType ServiceType{get;set;}

        public EventType EventType {get;set;}
    }
}
