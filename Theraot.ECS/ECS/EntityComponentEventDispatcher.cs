using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal class EntityComponentEventDispatcher<TEntityId, TComponentKind>
    {
        public event EventHandler<EntityComponentsChangeEventArgs<TEntityId, TComponentKind>> AddedComponents;

        public event EventHandler<EntityComponentsChangeEventArgs<TEntityId, TComponentKind>> RemovedComponents;

        internal void NotifyAddedComponents(TEntityId entityId, IList<TComponentKind> componentKinds)
        {
            AddedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntityId, TComponentKind>(CollectionChangeActionEx.Add, entityId, componentKinds));
        }

        internal void NotifyRemovedComponents(TEntityId entityId, IList<TComponentKind> componentKinds)
        {
            RemovedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntityId, TComponentKind>(CollectionChangeActionEx.Remove, entityId, componentKinds));
        }
    }
}