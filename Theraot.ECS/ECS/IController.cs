using System.Collections.Generic;

namespace Theraot.ECS
{
    internal interface IController<TEntityId, TComponentType>
    {
        EntityCollection<TEntityId, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none, IComponentReferenceAccess<TEntityId, TComponentType> componentReferenceAccess);

        void RegisterEntity(TEntityId entityId);

        void SubscribeTo(EntityComponentEventDispatcher<TEntityId, TComponentType> entityComponentEventDispatcher);
    }
}