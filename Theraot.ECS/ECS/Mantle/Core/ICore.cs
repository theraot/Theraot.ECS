using System;
using System.Collections.Generic;

namespace Theraot.ECS.Mantle.Core
{
    internal partial interface ICore<TEntity, TComponentType> : IComponentReferenceAccessProvider<TEntity, TComponentType>
    {
        event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> AddedComponents;

        event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> RemovedComponents;

        Type GetRegisteredComponentType(TComponentType componentType);

        bool RegisterEntity(TEntity entity);

        void SetComponent<TComponent>(TEntity entity, TComponentType type, TComponent component);

        bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component);

        bool TryRegisterComponentType<TComponent>(TComponentType componentType);

        void UnsetComponent(TEntity entity, TComponentType componentType);

        void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes);
    }

#if LESSTHAN_NET35

    internal partial interface ICore<TEntity, TComponentType>
    {
        void SetComponents<TComponent>(TEntity entity, IEnumerable<TComponentType> componentTypes, Converter<TComponentType, TComponent> componentSelector);
    }

#else

    internal partial interface ICore<TEntity, TComponentType>
    {
        void SetComponents<TComponent>(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector);
    }

#endif
}