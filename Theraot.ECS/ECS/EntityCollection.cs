#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.ECS
{
    /// <summary>
    /// Represents a readonly view of entities from the <see cref="Scope{TEntity, TComponentType}"/> from which it was created.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities.</typeparam>
    /// <typeparam name="TComponentType">The type uses to represent component types.</typeparam>
    public sealed partial class EntityCollection<TEntity, TComponentType> : ICollection<TEntity>
    {
        private readonly IComponentReferenceAccess<TEntity, TComponentType> _componentReferenceAccess;

        private readonly HashSet<TEntity> _wrapped;

        internal EntityCollection(IComponentReferenceAccess<TEntity, TComponentType> componentReferenceAccess, IEqualityComparer<TEntity> entityEqualityComparer)
        {
            _componentReferenceAccess = componentReferenceAccess;
            _wrapped = new HashSet<TEntity>(entityEqualityComparer);
        }

        /// <summary>
        /// The number of entities in this instance.
        /// </summary>
        public int Count => _wrapped.Count;

        /// <summary>
        /// Verifies if this instance contains the provided entity.
        /// </summary>
        /// <param name="item">The entity to search for.</param>
        /// <returns>true if the entity is found; otherwise, false.</returns>
        public bool Contains(TEntity item)
        {
            return _wrapped.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            _wrapped.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<TEntity> GetEnumerator()
        {
            foreach (var entity in _wrapped)
            {
                yield return entity;
            }
        }

        internal void Add(TEntity entity)
        {
            _wrapped.Add(entity);
            OnAddedEntity(entity);
        }

        internal void Remove(TEntity entity)
        {
            _wrapped.Remove(entity);
            OnRemovedEntity(entity);
        }
    }

    public sealed partial class EntityCollection<TEntity, TComponentType>
    {
        /// <summary>
        /// Occurs when an entity is added.
        /// </summary>
        public event EventHandler<EntityCollectionChangeEventArgs<TEntity>> AddedEntity;

        /// <summary>
        /// Occurs when an entity is removed.
        /// </summary>
        public event EventHandler<EntityCollectionChangeEventArgs<TEntity>> RemovedEntity;

        private void OnAddedEntity(TEntity entity)
        {
            AddedEntity?.Invoke(this, EntityCollectionChangeEventArgs.CreateAdd(entity));
        }

        private void OnRemovedEntity(TEntity entity)
        {
            RemovedEntity?.Invoke(this, EntityCollectionChangeEventArgs.CreateRemove(entity));
        }
    }

    public sealed partial class EntityCollection<TEntity, TComponentType>
    {
        /// <summary>
        /// Executes a callback over all the entities in this instance.
        /// </summary>
        /// <param name="callback">The callback to execute.</param>
        /// <exception cref="ArgumentNullException">The callback is null.</exception>
        public void ForEach
        (
            Action<TEntity> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entity in _wrapped)
            {
                callback
                (
                    entity
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
            ActionRef<TEntity, TComponent1> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entity in _wrapped)
            {
                _componentReferenceAccess.With(entity, componentType1, callback);
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
            ActionRef<TEntity, TComponent1, TComponent2> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entity in _wrapped)
            {
                _componentReferenceAccess.With(entity, componentType1, componentType2, callback);
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
            ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entity in _wrapped)
            {
                _componentReferenceAccess.With(entity, componentType1, componentType2, componentType3, callback);
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
            ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entity in _wrapped)
            {
                _componentReferenceAccess.With(entity, componentType1, componentType2, componentType3, componentType4, callback);
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
            ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback
        )
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entity in _wrapped)
            {
                _componentReferenceAccess.With(entity, componentType1, componentType2, componentType3, componentType4, componentType5, callback);
            }
        }
    }

    public sealed partial class EntityCollection<TEntity, TComponentType>
    {
        bool ICollection<TEntity>.IsReadOnly => true;

        void ICollection<TEntity>.Add(TEntity item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TEntity>.Clear()
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<TEntity>.Remove(TEntity item)
        {
            throw new NotSupportedException();
        }
    }
}