using System;
using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    internal interface IScope<TEntity, TComponentType> : IComponentRefSource<TEntity, TComponentType>
    {
        TEntity CreateEntity();

        TComponent GetComponent<TComponent>(TEntity entity, TComponentType componentType);

        EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);

        Type GetRegisteredComponentType(TComponentType componentType);

        void SetComponent<TComponent>(TEntity entity, TComponentType type, TComponent component);

        void SetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector);

        bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component);

        bool TryRegisterComponentType<TComponent>(TComponentType componentType);

        void UnsetComponent(TEntity entity, TComponentType componentType);

        void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes);
    }
}