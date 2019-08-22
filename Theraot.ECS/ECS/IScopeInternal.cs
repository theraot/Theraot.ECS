using System.Collections.Generic;

namespace Theraot.ECS
{
    internal interface IScopeInternal<TEntity, TComponentType> : IScopeCommon<TEntity, TComponentType>, IComponentRefScopeProvider<TEntity, TComponentType>
    {
        TEntity CreateEntity();

        EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);
    }
}