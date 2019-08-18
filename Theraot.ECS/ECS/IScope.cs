using System;
using System.Collections.Generic;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal interface IScope<TEntity, TComponentType>
    {
        TEntity CreateEntity();

        QueryId CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);

        TComponent GetComponent<TComponent>(TEntity entity, TComponentType componentType);

        IEnumerable<TEntity> GetEntities(QueryId queryId);

        void SetComponent<TComponent>(TEntity entity, TComponentType type, TComponent component);

        void SetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector);

        bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component);

        void UnsetComponent<TComponent>(TEntity entity, TComponentType componentType);

        void UnsetComponents<TComponent>(TEntity entity, IEnumerable<TComponentType> componentTypes);
    }
}