using System.Collections.Generic;

namespace Theraot.ECS
{
    internal interface IScope<TEntity, TComponentType> : IScopeCommon<TEntity, TComponentType>
    {
        TEntity CreateEntity();

        EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);
    }
}