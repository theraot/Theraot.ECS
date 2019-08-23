using System.Collections.Generic;
using Theraot.ECS.Mantle.Core;

namespace Theraot.ECS.Mantle
{
    internal interface IScopeMantle<TEntity, TComponentType> : IScopeCommon<TEntity, TComponentType>, IComponentRefScopeProvider<TEntity, TComponentType>
    {
        TEntity CreateEntity();

        EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);
    }
}