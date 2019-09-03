using System;
using System.Collections.Generic;

namespace Theraot.ECS.Mantle.Core
{
    internal class EntityComponentEventDispatcher<TEntity, TComponentType>
    {
        public event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> AddedComponents;

        public event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> RemovedComponents;

        internal void NotifyAddedComponents(TEntity entity, IList<TComponentType> componentTypes)
        {
            AddedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntity, TComponentType>(CollectionChangeActionEx.Add, entity, componentTypes));
        }

        internal void NotifyRemovedComponents(TEntity entity, IList<TComponentType> componentTypes)
        {
            RemovedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntity, TComponentType>(CollectionChangeActionEx.Remove, entity, componentTypes));
        }
    }
}