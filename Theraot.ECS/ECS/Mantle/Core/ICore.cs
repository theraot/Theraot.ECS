using System;

namespace Theraot.ECS.Mantle.Core
{
    internal interface ICore<TEntity, TComponentType> : ICommon<TEntity, TComponentType>, IComponentReferenceAccessProvider<TEntity, TComponentType>
    {
        event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> AddedComponents;

        event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> RemovedComponents;

        bool RegisterEntity(TEntity entity);
    }
}