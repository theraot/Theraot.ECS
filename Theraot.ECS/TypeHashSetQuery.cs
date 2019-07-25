using System.Collections.Generic;
using ComponentType = System.Type;

namespace Theraot.ECS
{
    public class TypeHashSetQuery
    {
        public HashSet<ComponentType> All { get; }

        public HashSet<ComponentType> Any { get; }

        public HashSet<ComponentType> None { get; }

        public TypeHashSetQuery(HashSet<ComponentType> all, HashSet<ComponentType> any, HashSet<ComponentType> none)
        {
            All = all;
            Any = any;
            None = none;
        }
    }
}
