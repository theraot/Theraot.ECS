using System.Collections.Generic;
using Theraot.ECS.Mantle.Core;

namespace Theraot.ECS.Mantle
{
    internal interface IMantle<TEntity, TComponentType> : ICommon<TEntity, TComponentType>, IComponentReferenceAccessProvider<TEntity, TComponentType>
    {
        EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);

        void RegisterEntity(TEntity entity);
    }
}