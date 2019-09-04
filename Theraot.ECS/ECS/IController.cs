using System.Collections.Generic;

namespace Theraot.ECS
{
    internal interface IController<TEntityId, TComponentKind>
    {
        EntityCollection<TEntityId, TComponentKind> GetAllEntities(IComponentReferenceAccess<TEntityId, TComponentKind> componentReferenceAccess);

        EntityCollection<TEntityId, TComponentKind> GetEntityCollection(IEnumerable<TComponentKind> all, IEnumerable<TComponentKind> any, IEnumerable<TComponentKind> none, IComponentReferenceAccess<TEntityId, TComponentKind> componentReferenceAccess);

        void RegisterEntity(TEntityId entityId);

        void SubscribeTo(EntityComponentEventDispatcher<TEntityId, TComponentKind> entityComponentEventDispatcher);
    }
}