using System.Collections.Generic;
using ComponentType = System.Type;

namespace Theraot.ECS
{
    public class Query
    {
        public readonly HashSet<ComponentType> All;
        public readonly HashSet<ComponentType> Any;
        public readonly HashSet<ComponentType> None;

        public Query(HashSet<ComponentType> all, HashSet<ComponentType> any, HashSet<ComponentType> none)
        {
            All = all;
            Any = any;
            None = none;
        }
    }
}
