using System;
using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS.Mantle.Core
{
    internal interface IScopeCommon<in TEntity, TComponentType>
    {
        Type GetRegisteredComponentType(TComponentType componentType);

        void SetComponent<TComponent>(TEntity entity, TComponentType type, TComponent component);

        void SetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector);

        bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component);

        bool TryRegisterComponentType<TComponent>(TComponentType componentType);

        void UnsetComponent(TEntity entity, TComponentType componentType);

        void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes);
    }
}