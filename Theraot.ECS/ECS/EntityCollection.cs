#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections;
using System.Collections.Generic;
using Theraot.ECS.Mantle;

namespace Theraot.ECS
{
    public sealed partial class EntityCollection<TEntity, TComponentType> : ICollection<TEntity>
    {
        private readonly IComponentReferenceAccess<TEntity, TComponentType> _componentRefScope;

        private readonly HashSet<TEntity> _wrapped;

        internal EntityCollection(IComponentReferenceAccess<TEntity, TComponentType> componentRefScope, IEqualityComparer<TEntity> entityEqualityComparer)
        {
            _componentRefScope = componentRefScope;
            _wrapped = new HashSet<TEntity>(entityEqualityComparer);
            _removedEntity = new HashSet<EventHandler<EntityCollectionChangeEventArgs<TEntity>>>();
            _addedEntity = new HashSet<EventHandler<EntityCollectionChangeEventArgs<TEntity>>>();
        }

        public int Count => _wrapped.Count;

        public bool Contains(TEntity item)
        {
            return _wrapped.Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            _wrapped.CopyTo(array, arrayIndex);
        }

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
        private readonly HashSet<EventHandler<EntityCollectionChangeEventArgs<TEntity>>> _addedEntity;
        private readonly HashSet<EventHandler<EntityCollectionChangeEventArgs<TEntity>>> _removedEntity;

        public event EventHandler<EntityCollectionChangeEventArgs<TEntity>> AddedEntity
        {
            add => _addedEntity.Add(value);
            remove => _addedEntity.Remove(value);
        }

        public event EventHandler<EntityCollectionChangeEventArgs<TEntity>> RemovedEntity
        {
            add => _removedEntity.Add(value);
            remove => _removedEntity.Remove(value);
        }

        private void OnAddedEntity(TEntity entity)
        {
            foreach (var handler in _addedEntity)
            {
                handler.Invoke(this, EntityCollectionChangeEventArgs.CreateAdd(entity));
            }
        }

        private void OnRemovedEntity(TEntity entity)
        {
            foreach (var handler in _removedEntity)
            {
                handler.Invoke(this, EntityCollectionChangeEventArgs.CreateRemove(entity));
            }
        }
    }

    public sealed partial class EntityCollection<TEntity, TComponentType>
    {
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
                _componentRefScope.With(entity, componentType1, callback);
            }
        }

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
                _componentRefScope.With(entity, componentType1, componentType2, callback);
            }
        }

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
                _componentRefScope.With(entity, componentType1, componentType2, componentType3, callback);
            }
        }

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
                _componentRefScope.With(entity, componentType1, componentType2, componentType3, componentType4, callback);
            }
        }

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
                _componentRefScope.With(entity, componentType1, componentType2, componentType3, componentType4, componentType5, callback);
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