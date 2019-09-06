using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.ECS
{
    /// <summary>
    /// Allow sto create see <see cref="Scope{TEntityId, TComponentKind}"/>
    /// </summary>
    public static class Scope
    {
        /// <summary>
        /// Creates a new instance of <see cref="Scope{TEntityId, TComponentKind}"/>
        /// </summary>
        /// <typeparam name="TEntityId">The type of the entity ids.</typeparam>
        /// <typeparam name="TComponentKind">The type used to represent component kinds.</typeparam>
        /// <typeparam name="TComponentKindSet">The type used to store sets of component kinds.</typeparam>
        /// <param name="entityEqualityComparer">The equality comparer used to compare entity ids.</param>
        /// <param name="componentKindManager">The managers for component kinds and sets component kinds.</param>
        public static Scope<TEntityId, TComponentKind> CreateScope<TEntityId, TComponentKind, TComponentKindSet>(IEqualityComparer<TEntityId> entityEqualityComparer, IComponentKindManager<TComponentKind, TComponentKindSet> componentKindManager)
        {
            if (componentKindManager == null)
            {
                throw new ArgumentNullException(nameof(componentKindManager));
            }
            if (entityEqualityComparer == null)
            {
                entityEqualityComparer = EqualityComparer<TEntityId>.Default;
            }

            var controller = new Controller<TEntityId, TComponentKind, TComponentKindSet>
            (
                entityEqualityComparer,
                componentKindManager
            );
            return new Scope<TEntityId, TComponentKind>(controller, componentKindManager.ComponentKindEqualityComparer, entityEqualityComparer);
        }
    }

    /// <summary>
    /// Represents the environment in which entities exist.
    /// </summary>
    /// <typeparam name="TEntityId">The type of the entity ids.</typeparam>
    /// <typeparam name="TComponentKind">The type used to represent component kinds.</typeparam>
    public sealed partial class Scope<TEntityId, TComponentKind>
    {
        private readonly ComponentKindRegistry<TComponentKind> _componentKindRegistry;

        private readonly ComponentStorage<TEntityId, TComponentKind> _componentStorage;

        private readonly IController<TEntityId, TComponentKind> _controller;

        internal Scope(IController<TEntityId, TComponentKind> controller, IEqualityComparer<TComponentKind> componentKindEqualityComparer, IEqualityComparer<TEntityId> entityEqualityComparer)
        {
            _controller = controller;
            var entityComponentEventDispatcher = new EntityComponentEventDispatcher<TEntityId, TComponentKind>();
            controller.SubscribeTo(entityComponentEventDispatcher);
            _componentKindRegistry = new ComponentKindRegistry<TComponentKind>(componentKindEqualityComparer);
            _componentStorage = new ComponentStorage<TEntityId, TComponentKind>
            (
                componentKindEqualityComparer,
                entityEqualityComparer,
                _componentKindRegistry,
                entityComponentEventDispatcher
            );
        }

        /// <summary>
        /// Removes all components for an entity id and unregisters it.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <returns>true if the entity id was registered; otherwise, false.</returns>
        public bool DestroyEntity(TEntityId entityId)
        {
            if (!_componentStorage.DestroyEntity(entityId))
            {
                return false;
            }
            _controller.DestroyEntity(entityId);
            return true;
        }

        /// <summary>
        /// Gets a <see cref="EntityCollection{TEntityId, TComponentKind}"/> with all the entities.
        /// </summary>
        public EntityCollection<TEntityId, TComponentKind> GetAllEntities()
        {
            return _controller.GetAllEntities(_componentStorage);
        }

        /// <summary>
        /// Retrieves a component by its component kind for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id for which to get the component.</param>
        /// <param name="componentKind">The kind of the component to retrieve.</param>
        /// <exception cref="KeyNotFoundException">The entity has not been registered or the component was not found.</exception>
        /// <exception cref="ArgumentNullException">The component kind is null.</exception>
        /// <exception cref="ArgumentException">The type does not match the component kind.</exception>
        public TComponentValue GetComponent<TComponentValue>(TEntityId entityId, TComponentKind componentKind)
        {
            if (_componentStorage.TryGetComponent<TComponentValue>(entityId, componentKind, out var component))
            {
                return component;
            }

            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Retrieves a collection with the component kind associated with the entity
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        /// <remarks>This does not return a snapshot.</remarks>
        public ICollection<TComponentKind> GetComponentKinds(TEntityId entityId)
        {
            return _componentStorage.GetComponentKinds(entityId);
        }

        /// <summary>
        /// Gets or creates a <see cref="EntityCollection{TEntityId, TComponentKind}"/> for all entities that contain all the component kinds in <paramref name="all"/> at least one component kind in <paramref name="any"/> and none of the component kinds in <paramref name="none"/>.
        /// </summary>
        /// <param name="all">The collection of the component kinds from which the entities must have all.</param>
        /// <param name="any">The collection of the component kinds from which the entities must have at least one.</param>
        /// <param name="none">The collection of the component kinds from which the entities must have none.</param>
        /// <returns>A <see cref="EntityCollection{TEntityId, TComponentKind}"/> that holds a view of the entities that matches the specified conditions.</returns>
        /// <remarks>The returned <see cref="EntityCollection{TEntityId, TComponentKind}"/> is not an snapshot.</remarks>
        public EntityCollection<TEntityId, TComponentKind> GetEntityCollection(IEnumerable<TComponentKind> all, IEnumerable<TComponentKind> any, IEnumerable<TComponentKind> none)
        {
            return _controller.GetEntityCollection(all, any, none, _componentStorage);
        }

        /// <summary>
        /// Gets the type that is associated with a component kind.
        /// </summary>
        /// <param name="componentKind">The component kind to query.</param>
        /// <exception cref="KeyNotFoundException">The component kind has not been registered.</exception>
        public Type GetRegisteredType(TComponentKind componentKind)
        {
            return _componentKindRegistry.GetRegisteredType(componentKind);
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
        /// Sets a component by its component kind associated for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id to set the component to.</param>
        /// <param name="kind">The component kind.</param>
        /// <param name="component">The component value.</param>
        /// <exception cref="ArgumentException">The type does not match the component kind.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        /// <remarks>If the component kind has not been registered, it is registered with the provided type and a default container.</remarks>
        public void SetComponent<TComponentValue>(TEntityId entityId, TComponentKind kind, TComponentValue component)
        {
            _componentStorage.SetComponent(entityId, kind, component);
        }

        /// <summary>
        /// Sets a components by their component kind associated for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity to set the component to.</param>
        /// <param name="components">A dictionary of component kinds and values.</param>
        /// <exception cref="ArgumentException">The type does not match the component kind.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        /// <remarks>If a component kind has not been registered, it is registered with the provided type and a default container.</remarks>
        public void SetComponents<TComponentValue>(TEntityId entityId, Dictionary<TComponentKind, TComponentValue> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            _componentStorage.SetComponents(entityId, components.Keys, type => components[type]);
        }

        /// <summary>
        /// Sets a components by their component kind associated for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id to set the component to.</param>
        /// <param name="componentKinds">The list of component kinds.</param>
        /// <param name="components">The list of component values.</param>
        /// <exception cref="ArgumentNullException">Either <paramref name="componentKinds"/> or <paramref name="components"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="componentKinds"/> and <paramref name="components"/> do not have the same number of elements.</exception>
        /// <exception cref="ArgumentException">The type does not match the component kind.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        /// <remarks>The component kinds and values are taken in order. If a component kind has not been registered, it is registered with the provided type and a default container.</remarks>
        public void SetComponents<TComponentValue>(TEntityId entityId, IList<TComponentKind> componentKinds, IList<TComponentValue> components)
        {
            if (componentKinds == null)
            {
                throw new ArgumentNullException(nameof(componentKinds));
            }

            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            if (components.Count != componentKinds.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(components), "Count does not match");
            }

            var index = 0;
            _componentStorage.SetComponents(entityId, componentKinds, _ => components[index++]);
        }

        /// <summary>
        /// Attempts to retrieve a component by its component kind for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id for which to get the component.</param>
        /// <param name="componentKind">The component kind to retrieve.</param>
        /// <param name="component">The retrieved component value.</param>
        /// <returns>true if the component was retrieved; otherwise, false.</returns>
        public bool TryGetComponent<TComponentValue>(TEntityId entityId, TComponentKind componentKind, out TComponentValue component)
        {
            return _componentStorage.TryGetComponent(entityId, componentKind, out component);
        }

        /// <summary>
        /// Attempts to register the type for a component kind, and the default container.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="componentKind">The component kind.</param>
        /// <returns>true if the type was registered; otherwise, false.</returns>
        public bool TryRegisterType<TComponentValue>(TComponentKind componentKind)
        {
            return _componentKindRegistry.TryRegisterType(componentKind, new IntKeyCollection<TComponentValue>(16));
        }

        /// <summary>
        /// Attempts to register the type for a component kind, and a custom container.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="componentKind">The component kind.</param>
        /// <param name="container">The custom container.</param>
        /// <returns>true if the type was registered; otherwise, false.</returns>
        public bool TryRegisterType<TComponentValue>(TComponentKind componentKind, IIntKeyCollection<TComponentValue> container)
        {
            return _componentKindRegistry.TryRegisterType(componentKind, container);
        }

        /// <summary>
        /// Removes all components for an entity id.
        /// </summary>
        /// <param name="entityId">The entity id for which to remove the component.</param>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        public void UnsetAllComponents(TEntityId entityId)
        {
            _componentStorage.UnsetAllComponents(entityId);
        }

        /// <summary>
        /// Removes a component by its component kind for an entity id.
        /// </summary>
        /// <param name="entityId">The entity id for which to remove the component.</param>
        /// <param name="componentKind">The component kind to remove.</param>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        public void UnsetComponent(TEntityId entityId, TComponentKind componentKind)
        {
            _componentStorage.UnsetComponent(entityId, componentKind);
        }

        /// <summary>
        /// Removes components by their component kind for an entity id.
        /// </summary>
        /// <param name="entityId">The entity id for which to remove the component.</param>
        /// <param name="componentKinds">The collection of component kinds to remove.</param>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        public void UnsetComponents(TEntityId entityId, IEnumerable<TComponentKind> componentKinds)
        {
            if (componentKinds == null)
            {
                throw new ArgumentNullException(nameof(componentKinds));
            }

            _componentStorage.UnsetComponents(entityId, componentKinds);
        }

        /// <summary>
        /// Removes components by their component kind for an entity id.
        /// </summary>
        /// <param name="entityId">The entity id for which to remove the component.</param>
        /// <param name="componentKinds">The collection of component kinds to remove.</param>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        public void UnsetComponents(TEntityId entityId, params TComponentKind[] componentKinds)
        {
            _componentStorage.UnsetComponents(entityId, componentKinds);
        }
    }

    public sealed partial class Scope<TEntityId, TComponentKind>
    {
        /// <summary>
        /// Executes a callback with references to components of the specified entity in this instance.
        /// </summary>
        /// <typeparam name="TComponentValue1">The type of the first component value.</typeparam>
        /// <param name="entityId">The id if the entity of which to get the components.</param>
        /// <param name="componentKind1">The first component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered, or a component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void With<TComponentValue1>(TEntityId entityId, TComponentKind componentKind1, ActionRef<TEntityId, TComponentValue1> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentKind1, callback);
        }

        /// <summary>
        /// Executes a callback with references to components of the specified entity in this instance.
        /// </summary>
        /// <typeparam name="TComponentValue1">The type of the first component value.</typeparam>
        /// <typeparam name="TComponentValue2">The type of the second component value.</typeparam>
        /// <param name="entityId">The id if the entity of which to get the components.</param>
        /// <param name="componentKind1">The first component kind to which to get a reference.</param>
        /// <param name="componentKind2">The second component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered, or a component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void With<TComponentValue1, TComponentValue2>(TEntityId entityId, TComponentKind componentKind1, TComponentKind componentKind2, ActionRef<TEntityId, TComponentValue1, TComponentValue2> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentKind1, componentKind2, callback);
        }

        /// <summary>
        /// Executes a callback with references to components of the specified entity in this instance.
        /// </summary>
        /// <typeparam name="TComponentValue1">The type of the first component value.</typeparam>
        /// <typeparam name="TComponentValue2">The type of the second component value.</typeparam>
        /// <typeparam name="TComponentValue3">The type of the third component value.</typeparam>
        /// <param name="entityId">The id if the entity of which to get the components.</param>
        /// <param name="componentKind1">The first component kind to which to get a reference.</param>
        /// <param name="componentKind2">The second component kind to which to get a reference.</param>
        /// <param name="componentKind3">The third component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered, or a component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void With<TComponentValue1, TComponentValue2, TComponentValue3>(TEntityId entityId, TComponentKind componentKind1, TComponentKind componentKind2, TComponentKind componentKind3, ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentKind1, componentKind2, componentKind3, callback);
        }

        /// <summary>
        /// Executes a callback with references to components of the specified entity in this instance.
        /// </summary>
        /// <typeparam name="TComponentValue1">The type of the first component value.</typeparam>
        /// <typeparam name="TComponentValue2">The type of the second component value.</typeparam>
        /// <typeparam name="TComponentValue3">The type of the third component value.</typeparam>
        /// <typeparam name="TComponentValue4">The type of the fourth component value.</typeparam>
        /// <param name="entityId">The id if the entity of which to get the components.</param>
        /// <param name="componentKind1">The first component kind to which to get a reference.</param>
        /// <param name="componentKind2">The second component kind to which to get a reference.</param>
        /// <param name="componentKind3">The third component kind to which to get a reference.</param>
        /// <param name="componentKind4">The fourth component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered, or a component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void With<TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4>(TEntityId entityId, TComponentKind componentKind1, TComponentKind componentKind2, TComponentKind componentKind3, TComponentKind componentKind4, ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentKind1, componentKind2, componentKind3, componentKind4, callback);
        }

        /// <summary>
        /// Executes a callback with references to components of the specified entity in this instance.
        /// </summary>
        /// <typeparam name="TComponentValue1">The type of the first component value.</typeparam>
        /// <typeparam name="TComponentValue2">The type of the second component value.</typeparam>
        /// <typeparam name="TComponentValue3">The type of the third component value.</typeparam>
        /// <typeparam name="TComponentValue4">The type of the fourth component value.</typeparam>
        /// <typeparam name="TComponentValue5">The type of the fifth component value.</typeparam>
        /// <param name="entityId">The id if the entity of which to get the components.</param>
        /// <param name="componentKind1">The first component kind to which to get a reference.</param>
        /// <param name="componentKind2">The second component kind to which to get a reference.</param>
        /// <param name="componentKind3">The third component kind to which to get a reference.</param>
        /// <param name="componentKind4">The fourth component kind to which to get a reference.</param>
        /// <param name="componentKind5">The fifth component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered, or a component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void With<TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4, TComponentValue5>(TEntityId entityId, TComponentKind componentKind1, TComponentKind componentKind2, TComponentKind componentKind3, TComponentKind componentKind4, TComponentKind componentKind5, ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4, TComponentValue5> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _componentStorage.With(entityId, componentKind1, componentKind2, componentKind3, componentKind4, componentKind5, callback);
        }
    }

#if LESSTHAN_NET35

    public sealed partial class Scope<TEntityId, TComponentKind>
    {
        /// <summary>
        /// Sets a components by their component kind associated for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id to set the component to.</param>
        /// <param name="componentKinds">The list of component kinds.</param>
        /// <param name="componentSelector">A function that returns a component value given a component kind.</param>
        /// <exception cref="ArgumentNullException">Either <paramref name="componentKinds"/> or <paramref name="componentSelector"/> is null.</exception>
        /// <exception cref="ArgumentException">The type does not match the component kind.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        /// <remarks>The component kinds and values are taken in order. If a component kind has not been registered, it is registered with the provided type and a default container.</remarks>
        public void SetComponents<TComponentValue>(TEntityId entityId, IEnumerable<TComponentKind> componentKinds, Converter<TComponentKind, TComponentValue> componentSelector)
        {
            if (componentKinds == null)
            {
                throw new ArgumentNullException(nameof(componentKinds));
            }

            if (componentSelector == null)
            {
                throw new ArgumentNullException(nameof(componentSelector));
            }

            _componentStorage.SetComponents(entityId, componentKinds, componentSelector);
        }
    }

#else

    public sealed partial class Scope<TEntityId, TComponentKind>
    {
        /// <summary>
        /// Sets a components by their component kind associated for an entity id.
        /// </summary>
        /// <typeparam name="TComponentValue">The type of the component value.</typeparam>
        /// <param name="entityId">The entity id to set the component to.</param>
        /// <param name="componentKinds">The list of component kinds.</param>
        /// <param name="componentSelector">A function that returns a component value given a component kind.</param>
        /// <exception cref="ArgumentNullException">Either <paramref name="componentKinds"/> or <paramref name="componentSelector"/> is null.</exception>
        /// <exception cref="ArgumentException">The type does not match the component kind.</exception>
        /// <exception cref="KeyNotFoundException">The entity has not been registered.</exception>
        /// <remarks>The component kinds and values are taken in order. If a component kind has not been registered, it is registered with the provided type and a default container.</remarks>
        public void SetComponents<TComponentValue>(TEntityId entityId, IEnumerable<TComponentKind> componentKinds, Func<TComponentKind, TComponentValue> componentSelector)
        {
            if (componentKinds == null)
            {
                throw new ArgumentNullException(nameof(componentKinds));
            }

            if (componentSelector == null)
            {
                throw new ArgumentNullException(nameof(componentSelector));
            }

            _componentStorage.SetComponents(entityId, componentKinds, componentSelector);
        }
    }

#endif
}