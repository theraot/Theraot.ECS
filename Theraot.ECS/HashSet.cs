#if LESSTHAN_NET35

using System.Collections;
using System.Collections.Generic;

namespace Theraot
{
    internal class HashSet<T> : IEnumerable<T>
    {
        public HashSet(IEqualityComparer<T> entityEqualityComparer)
        {
            throw new System.NotImplementedException();
        }

        public HashSet()
        {
            throw new System.NotImplementedException();
        }

        public int Count { get; }

        public bool Contains(T item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public void Add(T entity)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(T entity)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSupersetOf(HashSet<T> other)
        {
            throw new System.NotImplementedException();
        }

        public bool SetEquals(HashSet<T> hashSet)
        {
            throw new System.NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> componentTypes)
        {
            throw new System.NotImplementedException();
        }
    }
}

#endif