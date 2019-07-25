﻿using System.Collections.Generic;
using ComponentType = System.Type;

namespace Theraot.ECS
{
    public class TypeHashSetQuery
    {
        public HashSet<ComponentType> All { get; }

        public HashSet<ComponentType> Any { get; }

        public HashSet<ComponentType> None { get; }

        public TypeHashSetQuery(ComponentType[] all, ComponentType[] any, ComponentType[] none)
        {
            All = new HashSet<ComponentType>(all);
            Any = new HashSet<ComponentType>(any);
            None = new HashSet<ComponentType>(none);
        }
    }
}
