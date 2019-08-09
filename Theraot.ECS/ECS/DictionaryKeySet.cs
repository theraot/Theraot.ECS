#pragma warning disable CA1710 // Los identificadores deben tener un sufijo correcto
// ReSharper disable RedundantExtendsListEntry

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.ECS
{
    public sealed class DictionaryKeySet
    {
        public static DictionaryKeySet<TKey> CreateFrom<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            return new DictionaryKeySet<TKey>(dictionary.Keys, () => dictionary.Count, dictionary.ContainsKey);
        }
    }

    public sealed class DictionaryKeySet<T> : ISet<T>, IEnumerable<T>
    {
        private readonly Func<T, bool> _containsKey;

        private readonly Func<int> _count;

        private readonly IEnumerable<T> _wrapped;

        internal DictionaryKeySet(IEnumerable<T> wrapped, Func<int> count, Func<T, bool> containsKey)
        {
            _wrapped = wrapped;
            _count = count;
            _containsKey = containsKey;
        }

        public int Count => _count();

        bool ICollection<T>.IsReadOnly => true;

        public bool Contains(T item)
        {
            return _containsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _wrapped.ToArray().CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return this.IsSubsetOf(other, true);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return this.IsSupersetOf(other, true);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return this.IsSubsetOf(other, false);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return this.IsSupersetOf(other, false);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return other.Any(Contains);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            var otherAsICollection = other is ICollection<T> otherCollection ? otherCollection : other.ToArray();
            return otherAsICollection.All(Contains) && this.All(input => otherAsICollection.Contains(input));
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        bool ISet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }
    }
}