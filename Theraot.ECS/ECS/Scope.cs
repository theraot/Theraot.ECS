using System;
using System.Collections.Generic;
using Theraot.ECS.Mantle;
using Component = System.Object;

namespace Theraot.ECS
{
    public static class Scope
    {
        public static Scope<TEntity, TComponentType> CreateScope<TEntity, TComponentType, TComponentTypeSet>(Func<TEntity> entityIdFactory, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            var scopeInternal = new ScopeMantle<TEntity, TComponentType, TComponentTypeSet>(entityIdFactory, componentTypeManager);
            return new Scope<TEntity, TComponentType>(scopeInternal);
        }
    }

    public sealed partial class Scope<TEntity, TComponentType>
    {
        private readonly IScopeMantle<TEntity, TComponentType> _scopeMantle;

        internal Scope(IScopeMantle<TEntity, TComponentType> scopeMantle)
        {
            _scopeMantle = scopeMantle;
        }

        public TEntity CreateEntity()
        {
            return _scopeMantle.CreateEntity();
        }

        public TComponent GetComponent<TComponent>(TEntity entity, TComponentType componentType)
        {
            if (_scopeMantle.TryGetComponent<TComponent>(entity, componentType, out var component))
            {
                return component;
            }

            throw new KeyNotFoundException();
        }

        public EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            return _scopeMantle.GetEntityCollection(all, any, none);
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _scopeMantle.GetRegisteredComponentType(componentType);
        }

        public void SetComponent<TComponent>(TEntity entity, TComponentType type, TComponent component)
        {
            _scopeMantle.SetComponent(entity, type, component);
        }

        public void SetComponents(TEntity entity, Dictionary<TComponentType, Component> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            _scopeMantle.SetComponents(entity, components.Keys, type => components[type]);
        }

        public void SetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            if (componentSelector == null)
            {
                throw new ArgumentNullException(nameof(componentSelector));
            }

            _scopeMantle.SetComponents(entity, componentTypes, componentSelector);
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
            _scopeMantle.SetComponents(entity, componentTypes, _ => components[index++]);
        }

        public bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component)
        {
            return _scopeMantle.TryGetComponent(entity, componentType, out component);
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType)
        {
            return _scopeMantle.TryRegisterComponentType<TComponent>(componentType);
        }

        public void UnsetComponent(TEntity entity, TComponentType componentType)
        {
            _scopeMantle.UnsetComponent(entity, componentType);
        }

        public void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            _scopeMantle.UnsetComponents(entity, componentTypes);
        }

        public void UnsetComponents(TEntity entity, params TComponentType[] componentTypes)
        {
            _scopeMantle.UnsetComponents(entity, componentTypes);
        }
    }

    public sealed partial class Scope<TEntity, TComponentType>
    {
        public void With<TComponent1>(TEntity entity, TComponentType componentType1, ActionRef<TEntity, TComponent1> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _scopeMantle.GetComponentRefScope().With(entity, componentType1, callback);
        }

        public void With<TComponent1, TComponent2>(TEntity entity, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntity, TComponent1, TComponent2> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _scopeMantle.GetComponentRefScope().With(entity, componentType1, componentType2, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _scopeMantle.GetComponentRefScope().With(entity, componentType1, componentType2, componentType3, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _scopeMantle.GetComponentRefScope().With(entity, componentType1, componentType2, componentType3, componentType4, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _scopeMantle.GetComponentRefScope().With(entity, componentType1, componentType2, componentType3, componentType4, componentType5, callback);
        }
    }
}