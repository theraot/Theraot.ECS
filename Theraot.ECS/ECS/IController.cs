using System.Collections.Generic;
using Theraot.ECS.Core;

namespace Theraot.ECS.Mantle
{
    internal interface IController<TEntity, TComponentType>
    {
        EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none, IComponentReferenceAccess<TEntity, TComponentType> componentReferenceAccess);

        void RegisterEntity(TEntity entity);

        void SubscribeTo(EntityComponentEventDispatcher<TEntity, TComponentType> entityComponentEventDispatcher);
    }
}