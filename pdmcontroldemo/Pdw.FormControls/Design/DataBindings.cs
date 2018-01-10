using System;
using System.Collections;
using System.Linq;
using Pdw.FormControls.Extension;

namespace Pdw.FormControls.Design
{
    public sealed class DatabindingCollection : CollectionBase
    {
        public void Add(string name, string expression)
        {
            bool isExist = List.Cast<DataBinding>().Any(b => b.Name.Is(name));

            if (isExist)
            {
                throw new DuplicateWaitObjectException();
            }

            if (string.IsNullOrEmpty(expression) || string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            List.Add(new DataBinding { Name = name, Expression = expression });
        }

        public DataBinding this[string name]
        {
            get
            {
                return List.Cast<DataBinding>().SingleOrDefault(b => b.Name.Is(name));
            }
        }
    }

    public sealed class DataBinding
    {
        public string Name { get; set; }

        public string Expression { get; set; }

        internal string[] Split(char spliter)
        {
            int index = Expression.LastIndexOf(spliter);

            return new[] { Expression.Substring(0, index), Expression.Substring(index + 1) };
        }
    }
}
