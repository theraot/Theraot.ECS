using System.Collections.Generic;

namespace Theraot.ECS
{
    internal interface IScopeShell<TEntity, TComponentType> : IScopeCommon<TEntity, TComponentType>
    {
        TEntity CreateEntity();

        EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);
    }

    internal interface IScope<TEntity, TComponentType> : IScopeShell<TEntity, TComponentType>, IComponentRefScope<TEntity, TComponentType>
    {
        // Empty
    }

    internal interface IScopeInternal<TEntity, TComponentType> : IScopeShell<TEntity, TComponentType>
    {
        IComponentRefScope<TEntity, TComponentType> GetComponentRefScope();
    }
}