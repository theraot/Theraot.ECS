#pragma warning disable RECS0096 // Type parameter is never used
#pragma warning disable S4144 // Methods should not have identical implementations

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.ECS
{
    /// <summary>
    /// Represents a readonly view of entities from the <see cref="Scope{TEntityId, TComponentKind}"/> from which it was created.
    /// </summary>
    /// <typeparam name="TEntityId">The type of the entity ids.</typeparam>
    /// <typeparam name="TComponentKind">The type uses to represent component kinds.</typeparam>
    public sealed partial class EntityCollection<TEntityId, TComponentKind> : ICollection<TEntityId>
    {
        private readonly Predicate<TEntityId> _add;

        private readonly IComponentReferenceAccess<TEntityId, TComponentKind> _componentReferenceAccess;

        private readonly Predicate<TEntityId> _remove;

        private readonly ICollection<TEntityId> _wrapped;

        internal EntityCollection(IComponentReferenceAccess<TEntityId, TComponentKind> componentReferenceAccess, ICollection<TEntityId> wrapped, Predicate<TEntityId> add, Predicate<TEntityId> remove)
        {
            _componentReferenceAccess = componentReferenceAccess;
            _wrapped = wrapped;
            _add = add;
            _remove = remove;
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
            if (_add(entityId))
            {
                OnAddedEntity(entityId);
            }
        }

        internal void Remove(TEntityId entityId)
        {
            if (_remove(entityId))
            {
                OnRemovedEntity(entityId);
            }
        }
    }

    public sealed partial class EntityCollection<TEntityId, TComponentKind>
    {
        /// <summary>
        /// Occurs when an entity is added.
        /// </summary>
        public event EventHandler<EntityCollectionChangeEventArgs<TEntityId>>? AddedEntity;

        /// <summary>
        /// Occurs when an entity is removed.
        /// </summary>
        public event EventHandler<EntityCollectionChangeEventArgs<TEntityId>>? RemovedEntity;

        private void OnAddedEntity(TEntityId entityId)
        {
            AddedEntity?.Invoke(this, EntityCollectionChangeEventArgs.CreateAdd(entityId));
        }

        private void OnRemovedEntity(TEntityId entityId)
        {
            RemovedEntity?.Invoke(this, EntityCollectionChangeEventArgs.CreateRemove(entityId));
        }
    }

    public sealed partial class EntityCollection<TEntityId, TComponentKind>
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
        /// <typeparam name="TComponentValue1">The type of the component value.</typeparam>
        /// <param name="componentKind1">A component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void ForEach<TComponentValue1>
        (
            TComponentKind componentKind1,
            ActionRef<TEntityId, TComponentValue1> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentKind1, callback);
            }
        }

        /// <summary>
        /// Executes a callback with references to components over all the entities in this instance.
        /// </summary>
        /// <typeparam name="TComponentValue1">The type of the first component value.</typeparam>
        /// <typeparam name="TComponentValue2">The type of the second component value.</typeparam>
        /// <param name="componentKind1">The first component kind to which to get a reference.</param>
        /// <param name="componentKind2">The second component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void ForEach<TComponentValue1, TComponentValue2>
        (
            TComponentKind componentKind1,
            TComponentKind componentKind2,
            ActionRef<TEntityId, TComponentValue1, TComponentValue2> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentKind1, componentKind2, callback);
            }
        }

        /// <summary>
        /// Executes a callback with references to components over all the entities in this instance.
        /// </summary>
        /// <typeparam name="TComponentValue1">The type of the first component value.</typeparam>
        /// <typeparam name="TComponentValue2">The type of the second component value.</typeparam>
        /// <typeparam name="TComponentValue3">The type of the third component value.</typeparam>
        /// <param name="componentKind1">The first component kind to which to get a reference.</param>
        /// <param name="componentKind2">The second component kind to which to get a reference.</param>
        /// <param name="componentKind3">The third component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void ForEach<TComponentValue1, TComponentValue2, TComponentValue3>
        (
            TComponentKind componentKind1,
            TComponentKind componentKind2,
            TComponentKind componentKind3,
            ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentKind1, componentKind2, componentKind3, callback);
            }
        }

        /// <summary>
        /// Executes a callback with references to components over all the entities in this instance.
        /// </summary>
        /// <typeparam name="TComponentValue1">The type of the first component value.</typeparam>
        /// <typeparam name="TComponentValue2">The type of the second component value.</typeparam>
        /// <typeparam name="TComponentValue3">The type of the third component value.</typeparam>
        /// <typeparam name="TComponentValue4">The type of the fourth component value.</typeparam>
        /// <param name="componentKind1">The first component kind to which to get a reference.</param>
        /// <param name="componentKind2">The second component kind to which to get a reference.</param>
        /// <param name="componentKind3">The third component kind to which to get a reference.</param>
        /// <param name="componentKind4">The fourth component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void ForEach<TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4>
        (
            TComponentKind componentKind1,
            TComponentKind componentKind2,
            TComponentKind componentKind3,
            TComponentKind componentKind4,
            ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentKind1, componentKind2, componentKind3, componentKind4, callback);
            }
        }

        /// <summary>
        /// Executes a callback with references to components over all the entities in this instance.
        /// </summary>
        /// <typeparam name="TComponentValue1">The type of the first component value.</typeparam>
        /// <typeparam name="TComponentValue2">The type of the second component value.</typeparam>
        /// <typeparam name="TComponentValue3">The type of the third component value.</typeparam>
        /// <typeparam name="TComponentValue4">The type of the fourth component value.</typeparam>
        /// <typeparam name="TComponentValue5">The type of the fifth component value.</typeparam>
        /// <param name="componentKind1">The first component kind to which to get a reference.</param>
        /// <param name="componentKind2">The second component kind to which to get a reference.</param>
        /// <param name="componentKind3">The third component kind to which to get a reference.</param>
        /// <param name="componentKind4">The fourth component kind to which to get a reference.</param>
        /// <param name="componentKind5">The fifth component kind to which to get a reference.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        /// <exception cref="KeyNotFoundException">A component kind was not found.</exception>
        /// <exception cref="ArgumentException">A type does not match the component kind.</exception>
        public void ForEach<TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4, TComponentValue5>
        (
            TComponentKind componentKind1,
            TComponentKind componentKind2,
            TComponentKind componentKind3,
            TComponentKind componentKind4,
            TComponentKind componentKind5,
            ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4, TComponentValue5> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entityId in _wrapped)
            {
                _componentReferenceAccess.With(entityId, componentKind1, componentKind2, componentKind3, componentKind4, componentKind5, callback);
            }
        }
    }

    public sealed partial class EntityCollection<TEntityId, TComponentKind>
    {
        bool ICollection<TEntityId>.IsReadOnly => true;

        void ICollection<TEntityId>.Add(TEntityId item)
        {
            _ = item;
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
            _ = item;
            throw new NotSupportedException();
        }
    }
}