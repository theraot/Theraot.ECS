using System.Collections.Generic;
using Theraot.ECS.Mantle.Core;

namespace Theraot.ECS.Mantle
{
    internal interface IMantle<TEntity, TComponentType>
    {
        EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none, IComponentReferenceAccess<TEntity, TComponentType> componentRefScope);

        void RegisterEntity(TEntity entity);

        void SubscribeToCore(ICore<TEntity, TComponentType> core);
    }
}