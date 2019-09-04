﻿#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.ECS
{
    /// <summary>
    /// Represents a readonly view of entities from the <see cref="Scope{TEntityId, TComponentType}"/> from which it was created.
    /// </summary>
    /// <typeparam name="TEntityId">The type of the entities.</typeparam>
    /// <typeparam name="TComponentType">The type uses to represent component types.</typeparam>
    public sealed partial class EntityCollection<TEntityId, TComponentType> : ICollection<TEntityId>
    {
        private readonly IComponentReferenceAccess<TEntityId, TComponentType> _componentReferenceAccess;

        private readonly HashSet<TEntityId> _wrapped;

        internal EntityCollection(IComponentReferenceAccess<TEntityId, TComponentType> componentReferenceAccess, IEqualityComparer<TEntityId> entityEqualityComparer)
        {
            _componentReferenceAccess = componentReferenceAccess;
            _wrapped = new HashSet<TEntityId>(entityEqualityComparer);
        }

        /// <summary>
        /// The number of entities in this instance.
        /// </summary>
        public int Count => _wrapped.Count;

        /// <summary>
        /// Verifies if this instance contains the provided entity id.
        /// </summary>
        /// <param name="item">The entity id to search for.</param>
        /// <returns>true if the entity id is found; otherwise, false.</returns>
        public bool Contains(TEntityId item)
        {
            return _wrapped.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(TEntityId[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            _wrapped.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<TEntityId> GetEnumerator()
        {
            foreach (var entityId in _wrapped)
            {
                yield return entityId;
            }
        }

        internal void Add(TEntityId entityId)
        {
            _wrapped.Add(entityId);
            OnAddedEntity(entityId);
        }

        internal void Remove(TEntityId entityId)
        {
            _wrapped.Remove(entityId);
            OnRemovedEntity(entityId);
        }
    }

    public sealed partial class EntityCollection<TEntityId, TComponentType>
    {
        /// <summary>
        /// Occurs when an entity is added.
        /// </summary>
        public event EventHandler<EntityCollectionChangeEventArgs<TEntityId>> AddedEntity;

        /// <summary>
        /// Occurs when an entity is removed.
        /// </summary>
        public event EventHandler<EntityCollectionChangeEventArgs<TEntityId>> RemovedEntity;

        private void OnAddedEntity(TEntityId entityId)
        {
            AddedEntity?.Invoke(this, EntityCollectionChangeEventArgs.CreateAdd(entityId));
        }

        private void OnRemovedEntity(TEntityId entityId)
        {
            RemovedEntity?.Invoke(this, EntityCollectionChangeEventArgs.CreateRemove(entityId));
        }
    }

    public sealed partial class EntityCollection<TEntityId, TComponentType>
    {
        /// <summary>
        /// Executes a callback over all the entities in this instance.
        /// </summary>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        public void ForEach
        (
            Action<TEntityId> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                callback
                (
                    entityId
                );
            }
        }

        /// <summary>
        /// Executes a callback with references to components over all the entities in this instance.
        /// </summary>
        /// <typeparam name="TComponent1">The actual type of the component type.</typeparam>
        /// <param name="componentType1">A component type to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component or component type was not found.</exception>
        /// <exception cref="ArgumentException">A component type does not match the specified actual type.</exception>
        public void ForEach<TComponent1>
        (
            TComponentType componentType1,
            ActionRef<TEntityId, TComponent1> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentType1, callback);
            }
        }

        /// <summary>
        /// Executes a callback with references to components over all the entities in this instance.
        /// </summary>
        /// <typeparam name="TComponent1">The actual type of the first component type.</typeparam>
        /// <typeparam name="TComponent2">The actual type of the second component type.</typeparam>
        /// <param name="componentType1">The first component type to which to get a reference.</param>
        /// <param name="componentType2">The second component type to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component or component type was not found.</exception>
        /// <exception cref="ArgumentException">A component type does not match the specified actual type.</exception>
        public void ForEach<TComponent1, TComponent2>
        (
            TComponentType componentType1,
            TComponentType componentType2,
            ActionRef<TEntityId, TComponent1, TComponent2> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentType1, componentType2, callback);
            }
        }

        /// <summary>
        /// Executes a callback with references to components over all the entities in this instance.
        /// </summary>
        /// <typeparam name="TComponent1">The actual type of the first component type.</typeparam>
        /// <typeparam name="TComponent2">The actual type of the second component type.</typeparam>
        /// <typeparam name="TComponent3">The actual type of the third component type.</typeparam>
        /// <param name="componentType1">The first component type to which to get a reference.</param>
        /// <param name="componentType2">The second component type to which to get a reference.</param>
        /// <param name="componentType3">The third component type to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component or component type was not found.</exception>
        /// <exception cref="ArgumentException">A component type does not match the specified actual type.</exception>
        public void ForEach<TComponent1, TComponent2, TComponent3>
        (
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            ActionRef<TEntityId, TComponent1, TComponent2, TComponent3> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentType1, componentType2, componentType3, callback);
            }
        }

        /// <summary>
        /// Executes a callback with references to components over all the entities in this instance.
        /// </summary>
        /// <typeparam name="TComponent1">The actual type of the first component type.</typeparam>
        /// <typeparam name="TComponent2">The actual type of the second component type.</typeparam>
        /// <typeparam name="TComponent3">The actual type of the third component type.</typeparam>
        /// <typeparam name="TComponent4">The actual type of the fourth component type.</typeparam>
        /// <param name="componentType1">The first component type to which to get a reference.</param>
        /// <param name="componentType2">The second component type to which to get a reference.</param>
        /// <param name="componentType3">The third component type to which to get a reference.</param>
        /// <param name="componentType4">The fourth component type to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component or component type was not found.</exception>
        /// <exception cref="ArgumentException">A component type does not match the specified actual type.</exception>
        public void ForEach<TComponent1, TComponent2, TComponent3, TComponent4>
        (
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            TComponentType componentType4,
            ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentType1, componentType2, componentType3, componentType4, callback);
            }
        }

        /// <summary>
        /// Executes a callback with references to components over all the entities in this instance.
        /// </summary>
        /// <typeparam name="TComponent1">The actual type of the first component type.</typeparam>
        /// <typeparam name="TComponent2">The actual type of the second component type.</typeparam>
        /// <typeparam name="TComponent3">The actual type of the third component type.</typeparam>
        /// <typeparam name="TComponent4">The actual type of the fourth component type.</typeparam>
        /// <typeparam name="TComponent5">The actual type of the fifth component type.</typeparam>
        /// <param name="componentType1">The first component type to which to get a reference.</param>
        /// <param name="componentType2">The second component type to which to get a reference.</param>
        /// <param name="componentType3">The third component type to which to get a reference.</param>
        /// <param name="componentType4">The fourth component type to which to get a reference.</param>
        /// <param name="componentType5">The fifth component type to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component or component type was not found.</exception>
        /// <exception cref="ArgumentException">A component type does not match the specified actual type.</exception>
        public void ForEach<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>
        (
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            TComponentType componentType4,
            TComponentType componentType5,
            ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentType1, componentType2, componentType3, componentType4, componentType5, callback);
            }
        }
    }

    public sealed partial class EntityCollection<TEntityId, TComponentType>
    {
        bool ICollection<TEntityId>.IsReadOnly => true;

        void ICollection<TEntityId>.Add(TEntityId item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TEntityId>.Clear()
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<TEntityId>.Remove(TEntityId item)
        {
            throw new NotSupportedException();
        }
    }
}