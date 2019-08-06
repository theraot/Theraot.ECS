using System.Collections.Generic;
using ComponentType = System.String;

namespace Theraot.ECS
{
    public sealed class TypeHashSetQuery
    {
        public TypeHashSetQuery(IEnumerable<ComponentType> all, IEnumerable<ComponentType> any, IEnumerable<ComponentType> none)
        {
            All = new HashSet<ComponentType>(all);
            Any = new HashSet<ComponentType>(any);
            None = new HashSet<ComponentType>(none);
        }

        public HashSet<ComponentType> All { get; }

        public HashSet<ComponentType> Any { get; }

        public HashSet<ComponentType> None { get; }
    }
}