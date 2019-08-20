using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Theraot.ECS
{
    public sealed partial class EntityCollection<TEntity> : ICollection<TEntity>
    {
        private readonly HashSet<TEntity> _wrapped;

        internal EntityCollection()
        {
            _wrapped = new HashSet<TEntity>();
            _removedEntity = new HashSet<EventHandler<CollectionChangeEventArgs<TEntity>>>();
            _addedEntity = new HashSet<EventHandler<CollectionChangeEventArgs<TEntity>>>();
        }

        public int Count => _wrapped.Count;

        public bool Contains(TEntity item)
        {
            return _wrapped.Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
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

    public sealed partial class EntityCollection<TEntity>
    {
        private readonly HashSet<EventHandler<CollectionChangeEventArgs<TEntity>>> _addedEntity;
        private readonly HashSet<EventHandler<CollectionChangeEventArgs<TEntity>>> _removedEntity;

        public event EventHandler<CollectionChangeEventArgs<TEntity>> AddedEntity
        {
            add => _addedEntity.Add(value);
            remove => _addedEntity.Remove(value);
        }

        public event EventHandler<CollectionChangeEventArgs<TEntity>> RemovedEntity
        {
            add => _removedEntity.Add(value);
            remove => _removedEntity.Remove(value);
        }

        private void OnAddedEntity(TEntity entity)
        {
            foreach (var handler in _addedEntity)
            {
                handler.Invoke(this, new CollectionChangeEventArgs<TEntity>(CollectionChangeAction.Add, entity));
            }
        }

        private void OnRemovedEntity(TEntity entity)
        {
            foreach (var handler in _removedEntity)
            {
                handler.Invoke(this, new CollectionChangeEventArgs<TEntity>(CollectionChangeAction.Remove, entity));
            }
        }
    }

    public sealed partial class EntityCollection<TEntity>
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