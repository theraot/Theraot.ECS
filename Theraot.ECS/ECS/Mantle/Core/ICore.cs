using System;
using System.Collections.Generic;

namespace Theraot.ECS.Mantle.Core
{
    internal interface ICore<TEntity, TComponentType, TComponentTypeSet> : ICommon<TEntity, TComponentType>, IComponentReferenceAccessProvider<TEntity, TComponentType>
    {
        event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> AddedComponents;

        event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> RemovedComponents;

        IEnumerable<TEntity> AllEntities { get; }

        TComponentTypeSet GetComponentTypes(TEntity entity);

        bool RegisterEntity(TEntity entity, TComponentTypeSet componentTypeSet);
    }
}