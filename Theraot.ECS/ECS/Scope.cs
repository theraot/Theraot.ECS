using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.ECS
{
    /// <summary>
    /// Allow sto create see <see cref="Scope{TEntityId, TComponentType}"/>
    /// </summary>
    public static class Scope
    {
        /// <summary>
        /// Creates a new instance of <see cref="Scope{TEntityId, TComponentType}"/>
        /// </summary>
        /// <typeparam name="TEntityId">The type of the entities.</typeparam>
        /// <typeparam name="TComponentType">The type used to represent component types.</typeparam>
        /// <typeparam name="TComponentTypeSet">The type used to store sets of component type.</typeparam>
        /// <param name="entityEqualityComparer"></param>
        /// <param name="componentTypeManager"></param>
        /// <returns></returns>
        public static Scope<TEntityId, TComponentType> CreateScope<TEntityId, TComponentType, TComponentTypeSet>(IEqualityComparer<TEntityId> entityEqualityComparer, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            if (componentTypeManager == null)
            {
                throw new ArgumentNullException(nameof(componentTypeManager));
            }
            if (entityEqualityComparer == null)
            {
                entityEqualityComparer = EqualityComparer<TEntityId>.Default;
            }

            var controller = new Controller<TEntityId, TComponentType, TComponentTypeSet>
            (
                entityEqualityComparer,
                componentTypeManager
            );
            return new Scope<TEntityId, TComponentType>(controller, componentTypeManager.ComponentTypEqualityComparer, entityEqualityComparer);
        }
    }

    /// <summary>
    /// Represents the environment in which entities exist.
    /// </summary>
    /// <typeparam name="TEntityId">The type of the entities.</typeparam>
    /// <typeparam name="TComponentType">The type used to represent component types.</typeparam>
    public sealed partial class Scope<TEntityId, TComponentType>
    {
        private readonly ComponentStorage<TEntityId, TComponentType> _componentStorage;

        private readonly ComponentTypeRegistry<TComponentType> _componentTypeRegistry;

        private readonly IController<TEntityId, TComponentType> _controller;

        internal Scope(IController<TEntityId, TComponentType> controller, IEqualityComparer<TComponentType> componentTypeEqualityComparer, IEqualityComparer<TEntityId> entityEqualityComparer)
        {
            _controller = controller;
            var entityComponentEventDispatcher = new EntityComponentEventDispatcher<TEntityId, TComponentType>();
            controller.SubscribeTo(entityComponentEventDispatcher);
            _componentTypeRegistry = new ComponentTypeRegistry<TComponentType>(componentTypeEqualityComparer);
            _componentStorage = new ComponentStorage<TEntityId, TComponentType>
            (
                componentTypeEqualityComparer,
                entityEqualityComparer,
                _componentTypeRegistry,
                entityComponentEventDispatcher
            );
        }

        /// <summary>
        /// Retrieves a component by its component type for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id for which to get the component.</param>
        /// <param name="componentType">The type of the component to retrieve.</param>
        /// <exception cref="KeyNotFoundException">The component was not found.</exception>
        /// <exception cref="ArgumentNullException">The component type is null.</exception>
        /// <exception cref="ArgumentException">The the type of the component value does not match the component type.</exception>
        public TComponentValue GetComponent<TComponentValue>(TEntityId entityId, TComponentType componentType)
        {
            if (_componentStorage.TryGetComponent<TComponentValue>(entityId, componentType, out var component))
            {
                return component;
            }

            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Gets or creates a <see cref="EntityCollection{TEntityId, TComponentType}"/> for all the entities that contain all the component types in <paramref name="all"/> at least one component type in <paramref name="any"/> and none of the components in <paramref name="none"/>.
        /// </summary>
        /// <param name="all">The collection of the component types from which the entities must have all.</param>
        /// <param name="any">The collection of the component types from which the entities must have at least one.</param>
        /// <param name="none">The collection of the component types from which the entities must have none.</param>
        /// <returns>A <see cref="EntityCollection{TEntityId, TComponentType}"/> that holds a view of the entities that matches the specified conditions.</returns>
        /// <remarks>The returned <see cref="EntityCollection{TEntityId, TComponentType}"/> is not an snapshot.</remarks>
        public EntityCollection<TEntityId, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            return _controller.GetEntityCollection(all, any, none, _componentStorage);
        }

        /// <summary>
        /// Gets the component value type that is associated with a component type.
        /// </summary>
        /// <param name="componentType">The component type to query.</param>
        /// <exception cref="KeyNotFoundException">The component type has not been registered.</exception>
        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _componentTypeRegistry.GetRegisteredComponentType(componentType);
        }

        /// <summary>
        /// Creates an entity with the provided id.
        /// </summary>
        /// <param name="entityId">The entity id to add.</param>
        /// <returns>true if the entity is new; otherwise, false.</returns>
        public bool RegisterEntity(TEntityId entityId)
        {
            if (!_componentStorage.RegisterEntity(entityId))
            {
                return false;
            }
            _controller.RegisterEntity(entityId);
            return true;
        }

        /// <summary>
        /// Sets a component by its component type associated for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id to set the component to.</param>
        /// <param name="type">The component type.</param>
        /// <param name="component">The component value.</param>
        /// <exception cref="ArgumentException">The type of the component value does not match the component type.</exception>
        /// <remarks>If the component type has not been registered, it is registered with the provided type of component value and a default container.</remarks>
        public void SetComponent<TComponentValue>(TEntityId entityId, TComponentType type, TComponentValue component)
        {
            _componentStorage.SetComponent(entityId, type, component);
        }

        /// <summary>
        /// Sets a components by their component type associated for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity to set the component to.</param>
        /// <param name="components">A dictionary of component types and values.</param>
        /// <exception cref="ArgumentException">The type of the component value does not match the component type.</exception>
        /// <remarks>If a component type has not been registered, it is registered with the provided type of component value and a default container.</remarks>
        public void SetComponents<TComponentValue>(TEntityId entityId, Dictionary<TComponentType, TComponentValue> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            _componentStorage.SetComponents(entityId, components.Keys, type => components[type]);
        }

        /// <summary>
        /// Sets a components by their component type associated for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id to set the component to.</param>
        /// <param name="componentTypes">The list of component types.</param>
        /// <param name="components">The list of component values.</param>
        /// <exception cref="ArgumentNullException">Either <paramref name="componentTypes"/> or <paramref name="components"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="componentTypes"/> and <paramref name="components"/> do not have the same number of elements.</exception>
        /// <exception cref="ArgumentException">The type of the component value does not match the component type.</exception>
        /// <remarks>The component types and values are taken in order. If a component type has not been registered, it is registered with the provided type of component value and a default container.</remarks>
        public void SetComponents<TComponentValue>(TEntityId entityId, IList<TComponentType> componentTypes, IList<TComponentValue> components)
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
            _componentStorage.SetComponents(entityId, componentTypes, _ => components[index++]);
        }

        /// <summary>
        /// Attempts to retrieve a component by its component type for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id for which to get the component.</param>
        /// <param name="componentType">The component type to retrieve.</param>
        /// <param name="component">The retrieved component value.</param>
        /// <returns>true if the component was retrieved; otherwise, false.</returns>
        public bool TryGetComponent<TComponentValue>(TEntityId entityId, TComponentType componentType, out TComponentValue component)
        {
            return _componentStorage.TryGetComponent(entityId, componentType, out component);
        }

        /// <summary>
        /// Attempts to register the type of component value for a component type, and the default container.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="componentType">The component type.</param>
        /// <returns>true if the component type was registered; otherwise, false.</returns>
        public bool TryRegisterComponentType<TComponentValue>(TComponentType componentType)
        {
            return _componentTypeRegistry.TryRegisterComponentType(componentType, new IntKeyCollection<TComponentValue>(16));
        }

        /// <summary>
        /// Attempts to register the type of component value for a component type, and a custom container.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="componentType">The component type.</param>
        /// <param name="storage">The custom container.</param>
        /// <returns>true if the component type was registered; otherwise, false.</returns>
        public bool TryRegisterComponentType<TComponentValue>(TComponentType componentType, IIntKeyCollection<TComponentValue> storage)
        {
            return _componentTypeRegistry.TryRegisterComponentType(componentType, storage);
        }

        /// <summary>
        /// Removes a component by its component type for an entity id.
        /// </summary>
        /// <param name="entityId">The entity id for which to remove the component.</param>
        /// <param name="componentType">The component type to remove.</param>
        public void UnsetComponent(TEntityId entityId, TComponentType componentType)
        {
            _componentStorage.UnsetComponent(entityId, componentType);
        }

        /// <summary>
        /// Removes components by their component type for an entity id.
        /// </summary>
        /// <param name="entityId">The entity id for which to remove the component.</param>
        /// <param name="componentTypes">The collection of component types to remove.</param>
        public void UnsetComponents(TEntityId entityId, IEnumerable<TComponentType> componentTypes)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            _componentStorage.UnsetComponents(entityId, componentTypes);
        }

        /// <summary>
        /// Removes components by their component type for an entity id.
        /// </summary>
        /// <param name="entityId">The entity id for which to remove the component.</param>
        /// <param name="componentTypes">The collection of component types to remove.</param>
        public void UnsetComponents(TEntityId entityId, params TComponentType[] componentTypes)
        {
            _componentStorage.UnsetComponents(entityId, componentTypes);
        }
    }

    public sealed partial class Scope<TEntityId, TComponentType>
    {
        public void With<TComponent1>(TEntityId entityId, TComponentType componentType1, ActionRef<TEntityId, TComponent1> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentType1, callback);
        }

        public void With<TComponent1, TComponent2>(TEntityId entityId, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntityId, TComponent1, TComponent2> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentType1, componentType2, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntityId entityId, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntityId, TComponent1, TComponent2, TComponent3> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentType1, componentType2, componentType3, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntityId entityId, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentType1, componentType2, componentType3, componentType4, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntityId entityId, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentType1, componentType2, componentType3, componentType4, componentType5, callback);
        }
    }

#if LESSTHAN_NET35

    public sealed partial class Scope<TEntityId, TComponentType>
    {
        public void SetComponents<TComponentValue>(TEntityId entityId, IEnumerable<TComponentType> componentTypes, Converter<TComponentType, TComponentValue> componentSelector)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            if (componentSelector == null)
            {
                throw new ArgumentNullException(nameof(componentSelector));
            }

            _componentStorage.SetComponents(entityId, componentTypes, componentSelector);
        }
    }

#else

    public sealed partial class Scope<TEntityId, TComponentType>
    {
        public void SetComponents<TComponentValue>(TEntityId entityId, IEnumerable<TComponentType> componentTypes, Func<TComponentType, TComponentValue> componentSelector)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            if (componentSelector == null)
            {
                throw new ArgumentNullException(nameof(componentSelector));
            }

            _componentStorage.SetComponents(entityId, componentTypes, componentSelector);
        }
    }

#endif
}