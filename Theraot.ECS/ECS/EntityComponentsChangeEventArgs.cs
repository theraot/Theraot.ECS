﻿using System.Collections.Generic;
using System.ComponentModel;

namespace Theraot.ECS
{
    public class EntityComponentsChangeEventArgs<TEntity, TComponentType> : CollectionChangeEventArgs
    {
        public EntityComponentsChangeEventArgs(CollectionChangeAction action, TEntity entity, ICollection<TComponentType> componentTypes)
            : base(action, componentTypes)
        {
            Entity = entity;
            ComponentTypes = componentTypes;
        }

        public ICollection<TComponentType> ComponentTypes { get; }

        public TEntity Entity { get; }
    }
}