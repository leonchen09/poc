
namespace Pdw.Core
{
    public class RelationOn
    {
        public string UniqueName { get; set; }
        public string OnClause { get; set; }

        public RelationOn Clone()
        {
            RelationOn clone = MemberwiseClone() as RelationOn;

            return clone;
        }
    }
}