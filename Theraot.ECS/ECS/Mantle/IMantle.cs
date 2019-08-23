using System.Collections.Generic;
using Theraot.ECS.Mantle.Core;

namespace Theraot.ECS.Mantle
{
    internal interface IMantle<TEntity, TComponentType> : ICommon<TEntity, TComponentType>, IComponentReferenceAccessProvider<TEntity, TComponentType>
    {
        TEntity CreateEntity();

        EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);
    }
}