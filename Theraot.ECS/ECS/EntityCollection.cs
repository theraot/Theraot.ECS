using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Theraot.ECS
{
    public sealed class EntityCollection<TEntity> : IEnumerable<TEntity>
    {
        private readonly HashSet<EventHandler<CollectionChangeEventArgs<TEntity>>> _addedEntity;
        private readonly HashSet<EventHandler<CollectionChangeEventArgs<TEntity>>> _removedEntity;
        private readonly HashSet<TEntity> _wrapped;

        internal EntityCollection()
        {
            _wrapped = new HashSet<TEntity>();
            _removedEntity = new HashSet<EventHandler<CollectionChangeEventArgs<TEntity>>>();
            _addedEntity = new HashSet<EventHandler<CollectionChangeEventArgs<TEntity>>>();
        }

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

        public IEnumerator<TEntity> GetEnumerator()
        {
            foreach (var entity in _wrapped)
            {
                yield return entity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
}