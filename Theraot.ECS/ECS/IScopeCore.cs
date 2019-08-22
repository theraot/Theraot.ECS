using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal interface IScopeCore<TEntity, TComponentType, TComponentTypeSet> : IScopeCommon<TEntity, TComponentType>
    {
        event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> AddedComponents;

        event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> RemovedComponents;

        IEnumerable<TEntity> AllEntities { get; }

        TComponentTypeSet GetComponentTypes(TEntity entity);

        void RegisterEntity(TEntity entity, TComponentTypeSet componentTypeSet);
    }
}