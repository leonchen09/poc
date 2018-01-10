
using System.Collections.Generic;

namespace Pdw.Core
{
    public class Classifer
    {
        public string Name { get; set; }
        public List<string> DomainNames { get; set; }

        public Classifer()
        {
            DomainNames = new List<string>();
        }

        public Classifer(string name):this()
        {
            Name = name;
        }
    }
}
