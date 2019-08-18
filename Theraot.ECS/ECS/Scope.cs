using System;
using System.Collections.Generic;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public static class Scope
    {
        public static Scope<TEntity, TComponentType> CreateScope<TEntity, TComponentType, TComponentTypeSet>(Func<TEntity> entityIdFactory, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            var scopeInternal = new ScopeInternal<TEntity, TComponentType, TComponentTypeSet>(entityIdFactory, componentTypeManager);
            return new Scope<TEntity, TComponentType>(scopeInternal);
        }
    }

    public sealed class Scope<TEntity, TComponentType> : IScope<TEntity, TComponentType>
    {
        private readonly IScope<TEntity, TComponentType> _scopeInternal;

        internal Scope(IScope<TEntity, TComponentType> scopeInternal)
        {
            _scopeInternal = scopeInternal;
        }

        public TEntity CreateEntity()
        {
            return _scopeInternal.CreateEntity();
        }

        public QueryId CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            return _scopeInternal.CreateQuery(all, any, none);
        }

        public TComponent GetComponent<TComponent>(TEntity entity, TComponentType componentType)
        {
            return _scopeInternal.GetComponent<TComponent>(entity, componentType);
        }

        public IEnumerable<TEntity> GetEntities(QueryId queryId)
        {
            return _scopeInternal.GetEntities(queryId);
        }

        public void SetComponent<TComponent>(TEntity entity, TComponentType type, TComponent component)
        {
            _scopeInternal.SetComponent(entity, type, component);
        }

        public void SetComponents(TEntity entity, Dictionary<TComponentType, Component> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            SetComponents(entity, components.Keys, type => components[type]);
        }

        public void SetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, object> componentSelector)
        {
            _scopeInternal.SetComponents(entity, componentTypes, componentSelector);
        }

        public void SetComponents(TEntity entity, IList<TComponentType> componentTypes, IList<Component> components)
        {
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
            SetComponents(entity, componentTypes, _ => components[index++]);
        }

        public bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component)
        {
            return _scopeInternal.TryGetComponent(entity, componentType, out component);
        }

        public void UnsetComponent<TComponent>(TEntity entity, TComponentType componentType)
        {
            _scopeInternal.UnsetComponent<TComponent>(entity, componentType);
        }

        public void UnsetComponents<TComponent>(TEntity entity, IEnumerable<TComponentType> componentTypes)
        {
            _scopeInternal.UnsetComponents<TComponent>(entity, componentTypes);
        }

        public void UnsetComponents<TComponent>(TEntity entity, params TComponentType[] componentTypes)
        {
            UnsetComponents<TComponent>(entity, (IEnumerable<TComponentType>)componentTypes);
        }
    }
}