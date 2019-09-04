using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal class EntityComponentEventDispatcher<TEntityId, TComponentType>
    {
        public event EventHandler<EntityComponentsChangeEventArgs<TEntityId, TComponentType>> AddedComponents;

        public event EventHandler<EntityComponentsChangeEventArgs<TEntityId, TComponentType>> RemovedComponents;

        internal void NotifyAddedComponents(TEntityId entityId, IList<TComponentType> componentTypes)
        {
            AddedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntityId, TComponentType>(CollectionChangeActionEx.Add, entityId, componentTypes));
        }

        internal void NotifyRemovedComponents(TEntityId entityId, IList<TComponentType> componentTypes)
        {
            RemovedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntityId, TComponentType>(CollectionChangeActionEx.Remove, entityId, componentTypes));
        }
    }
}