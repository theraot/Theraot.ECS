using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal class EntityComponentEventDispatcher<TEntityId, TComponentType>
    {
        public event EventHandler<EntityComponentsChangeEventArgs<TEntityId, TComponentType>> AddedComponents;

        public event EventHandler<EntityComponentsChangeEventArgs<TEntityId, TComponentType>> RemovedComponents;

        internal void NotifyAddedComponents(TEntityId entity, IList<TComponentType> componentTypes)
        {
            AddedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntityId, TComponentType>(CollectionChangeActionEx.Add, entity, componentTypes));
        }

        internal void NotifyRemovedComponents(TEntityId entity, IList<TComponentType> componentTypes)
        {
            RemovedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntityId, TComponentType>(CollectionChangeActionEx.Remove, entity, componentTypes));
        }
    }
}