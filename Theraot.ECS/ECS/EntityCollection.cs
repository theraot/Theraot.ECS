﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Theraot.ECS
{
    public sealed partial class EntityCollection<TEntity, TComponentType> : ICollection<TEntity>
    {
        private readonly IScope<TEntity, TComponentType> _scope;
        private readonly HashSet<TEntity> _wrapped;

        internal EntityCollection(IScope<TEntity, TComponentType> scope)
        {
            _scope = scope;
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

    public sealed partial class EntityCollection<TEntity, TComponentType>
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
                callback
                (
                    entity,
                    ref _scope.GetComponentRef<TComponent1>(entity, componentType1)
                );
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
                callback
                (
                    entity,
                    ref _scope.GetComponentRef<TComponent1>(entity, componentType1),
                    ref _scope.GetComponentRef<TComponent2>(entity, componentType2)
                );
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
                callback
                (
                    entity,
                    ref _scope.GetComponentRef<TComponent1>(entity, componentType1),
                    ref _scope.GetComponentRef<TComponent2>(entity, componentType2),
                    ref _scope.GetComponentRef<TComponent3>(entity, componentType3)
                );
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
                callback
                (
                    entity,
                    ref _scope.GetComponentRef<TComponent1>(entity, componentType1),
                    ref _scope.GetComponentRef<TComponent2>(entity, componentType2),
                    ref _scope.GetComponentRef<TComponent3>(entity, componentType3),
                    ref _scope.GetComponentRef<TComponent4>(entity, componentType4)
                );
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
                callback
                (
                    entity,
                    ref _scope.GetComponentRef<TComponent1>(entity, componentType1),
                    ref _scope.GetComponentRef<TComponent2>(entity, componentType2),
                    ref _scope.GetComponentRef<TComponent3>(entity, componentType3),
                    ref _scope.GetComponentRef<TComponent4>(entity, componentType4),
                    ref _scope.GetComponentRef<TComponent5>(entity, componentType5)
                );
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