using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.ECS
{
    public static class Scope
    {
        public static Scope<TEntity, TComponentType> CreateScope<TEntity, TComponentType, TComponentTypeSet>(IEqualityComparer<TEntity> entityEqualityComparer, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            if (componentTypeManager == null)
            {
                throw new ArgumentNullException(nameof(componentTypeManager));
            }
            if (entityEqualityComparer == null)
            {
                entityEqualityComparer = EqualityComparer<TEntity>.Default;
            }

            var controller = new Controller<TEntity, TComponentType, TComponentTypeSet>
            (
                entityEqualityComparer,
                componentTypeManager
            );
            return new Scope<TEntity, TComponentType>(controller, componentTypeManager.ComponentTypEqualityComparer, entityEqualityComparer);
        }
    }

    public sealed partial class Scope<TEntity, TComponentType>
    {
        private readonly ComponentStorage<TEntity, TComponentType> _componentStorage;

        private readonly ComponentTypeRegistry<TComponentType> _componentTypeRegistry;

        private readonly IController<TEntity, TComponentType> _controller;

        internal Scope(IController<TEntity, TComponentType> controller, IEqualityComparer<TComponentType> componentTypeEqualityComparer, IEqualityComparer<TEntity> entityEqualityComparer)
        {
            _controller = controller;
            var entityComponentEventDispatcher = new EntityComponentEventDispatcher<TEntity, TComponentType>();
            controller.SubscribeTo(entityComponentEventDispatcher);
            _componentTypeRegistry = new ComponentTypeRegistry<TComponentType>(componentTypeEqualityComparer);
            _componentStorage = new ComponentStorage<TEntity, TComponentType>
            (
                componentTypeEqualityComparer,
                entityEqualityComparer,
                _componentTypeRegistry,
                entityComponentEventDispatcher
            );
        }

        public TComponent GetComponent<TComponent>(TEntity entity, TComponentType componentType)
        {
            if (_componentStorage.TryGetComponent<TComponent>(entity, componentType, out var component))
            {
                return component;
            }

            throw new KeyNotFoundException();
        }

        public EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            return _controller.GetEntityCollection(all, any, none, _componentStorage.GetComponentReferenceAccess());
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _componentTypeRegistry.GetRegisteredComponentType(componentType);
        }

        public bool RegisterEntity(TEntity entity)
        {
            if (!_componentStorage.RegisterEntity(entity))
            {
                return false;
            }
            _controller.RegisterEntity(entity);
            return true;
        }

        public void SetComponent<TComponent>(TEntity entity, TComponentType type, TComponent component)
        {
            _componentStorage.SetComponent(entity, type, component);
        }

        public void SetComponents<TComponent>(TEntity entity, Dictionary<TComponentType, TComponent> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            _componentStorage.SetComponents(entity, components.Keys, type => components[type]);
        }

        public void SetComponents<TComponent>(TEntity entity, IList<TComponentType> componentTypes, IList<TComponent> components)
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
            _componentStorage.SetComponents(entity, componentTypes, _ => components[index++]);
        }

        public bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component)
        {
            return _componentStorage.TryGetComponent(entity, componentType, out component);
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType)
        {
            return _componentTypeRegistry.TryRegisterComponentType(componentType, new IndexedCollection<TComponent>(16));
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType, IIntKeyCollection<TComponent> storage)
        {
            return _componentTypeRegistry.TryRegisterComponentType(componentType, storage);
        }

        public void UnsetComponent(TEntity entity, TComponentType componentType)
        {
            _componentStorage.UnsetComponent(entity, componentType);
        }

        public void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            _componentStorage.UnsetComponents(entity, componentTypes);
        }

        public void UnsetComponents(TEntity entity, params TComponentType[] componentTypes)
        {
            _componentStorage.UnsetComponents(entity, componentTypes);
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

            _componentStorage.GetComponentReferenceAccess().With(entity, componentType1, callback);
        }

        public void With<TComponent1, TComponent2>(TEntity entity, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntity, TComponent1, TComponent2> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.GetComponentReferenceAccess().With(entity, componentType1, componentType2, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.GetComponentReferenceAccess().With(entity, componentType1, componentType2, componentType3, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.GetComponentReferenceAccess().With(entity, componentType1, componentType2, componentType3, componentType4, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.GetComponentReferenceAccess().With(entity, componentType1, componentType2, componentType3, componentType4, componentType5, callback);
        }
    }

#if LESSTHAN_NET35

    public sealed partial class Scope<TEntity, TComponentType>
    {
        public void SetComponents<TComponent>(TEntity entity, IEnumerable<TComponentType> componentTypes, Converter<TComponentType, TComponent> componentSelector)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            if (componentSelector == null)
            {
                throw new ArgumentNullException(nameof(componentSelector));
            }

            _componentStorage.SetComponents(entity, componentTypes, componentSelector);
        }
    }

#else

    public sealed partial class Scope<TEntity, TComponentType>
    {
        public void SetComponents<TComponent>(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            if (componentSelector == null)
            {
                throw new ArgumentNullException(nameof(componentSelector));
            }

            _componentStorage.SetComponents(entity, componentTypes, componentSelector);
        }
    }

#endif
}