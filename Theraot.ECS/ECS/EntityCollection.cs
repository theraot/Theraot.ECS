using System.Collections;
using System.Collections.Generic;

namespace Theraot.ECS
{
    public sealed class EntityCollection<TEntity> : IEnumerable<TEntity>
    {
        private readonly HashSet<TEntity> _hashSet;

        internal EntityCollection()
        {
            _hashSet = new HashSet<TEntity>();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            foreach (var entity in _hashSet)
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
            _hashSet.Add(entity);
        }

        public void Remove(TEntity entity)
        {
            _hashSet.Remove(entity);
        }
    }
}