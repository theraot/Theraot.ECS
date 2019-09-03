#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.ECS
{
    public sealed partial class EntityCollection<TEntity, TComponentType> : ICollection<TEntity>
    {
        private readonly IComponentReferenceAccess<TEntity, TComponentType> _componentReferenceAccess;

        private readonly HashSet<TEntity> _wrapped;

        internal EntityCollection(IComponentReferenceAccess<TEntity, TComponentType> componentReferenceAccess, IEqualityComparer<TEntity> entityEqualityComparer)
        {
            _componentReferenceAccess = componentReferenceAccess;
            _wrapped = new HashSet<TEntity>(entityEqualityComparer);
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
        public event EventHandler<EntityCollectionChangeEventArgs<TEntity>> AddedEntity;

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
                _componentReferenceAccess.With(entity, componentType1, callback);
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
                _componentReferenceAccess.With(entity, componentType1, componentType2, callback);
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
                _componentReferenceAccess.With(entity, componentType1, componentType2, componentType3, callback);
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
                _componentReferenceAccess.With(entity, componentType1, componentType2, componentType3, componentType4, callback);
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