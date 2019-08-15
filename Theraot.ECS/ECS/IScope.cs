using System;
using System.Collections.Generic;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public interface IScope<TEntity, TComponentType>
    {
        TEntity CreateEntity();

        QueryId CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);

        TComponent GetComponent<TComponent>(TEntity entity, TComponentType componentType);

        IEnumerable<TEntity> GetEntities(QueryId query);

        void SetComponent<TComponent>(TEntity entity, TComponentType type, TComponent component);

        void SetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector);

        bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component);

        void UnsetComponent(TEntity entity, TComponentType componentType);

        void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes);
    }

    public static class ScopeExtensions
    {
        public static void UnsetComponents<TEntity, TComponentType>(this IScope<TEntity, TComponentType> scope, TEntity entity, params TComponentType[] componentTypes)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            scope.UnsetComponents(entity, componentTypes);
        }

        public static void SetComponents<TEntity, TComponentType>(this IScope<TEntity, TComponentType> scope, TEntity entity, IList<TComponentType> componentTypes, IList<Component> components)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }
            if (components.Count != componentTypes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(components), "Count does not match");
            }

            var index = 0;
            scope.SetComponents(entity, componentTypes, _ => components[index++]);
        }

        public static void SetComponents<TEntity, TComponentType>(this IScope<TEntity, TComponentType> scope, TEntity entity, Dictionary<TComponentType, Component> components)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            scope.SetComponents(entity, components.Keys, type => components[type]);
        }
    }
}