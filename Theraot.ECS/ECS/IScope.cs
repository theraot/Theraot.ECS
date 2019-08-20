using System;
using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    internal interface IComponentRefSource<TEntity, in TComponentType>
    {
        void With<TComponent1>
        (
            TEntity entity,
            TComponentType componentType1,
            ActionRef<TEntity, TComponent1> callback
        );

        void With<TComponent1, TComponent2>
        (
            TEntity entity,
            TComponentType componentType1,
            TComponentType componentType2,
            ActionRef<TEntity, TComponent1, TComponent2> callback
        );

        void With<TComponent1, TComponent2, TComponent3>
        (
            TEntity entity,
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback
        );

        void With<TComponent1, TComponent2, TComponent3, TComponent4>
        (
            TEntity entity,
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            TComponentType componentType4,
            ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback
        );

        void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>
        (
            TEntity entity,
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            TComponentType componentType4,
            TComponentType componentType5,
            ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback
        );
    }

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