using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.ECS
{
    internal sealed class DictionaryKeySet<T> : ISet<T>
    {
        private readonly Func<T, bool> _containsKey;
        private readonly Func<int> _count;
        private readonly IEnumerable<T> _wrapped;

        private DictionaryKeySet(IEnumerable<T> wrapped, Func<int> count, Func<T, bool> containsKey)
        {
            _wrapped = wrapped;
            _count = count;
            _containsKey = containsKey;
        }

        public int Count => _count();

        bool ICollection<T>.IsReadOnly => true;

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

        public bool Contains(T item)
        {
            return _containsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _wrapped.ToArray().CopyTo(array, arrayIndex);
        }

        public static DictionaryKeySet<T> CreateFrom<TValue>(Dictionary<T, TValue> dictionary)
        {
            return new  DictionaryKeySet<T>(dictionary.Keys, () => dictionary.Count, dictionary.ContainsKey);
        }

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return IsSubsetOf(this, other, true);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return IsSupersetOf(this, other, true);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return IsSubsetOf(this, other, false);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return IsSupersetOf(this, other, false);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return other.Any(Contains);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
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

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        private static bool IsSubsetOf(IEnumerable<T> source, IEnumerable<T> other, bool proper)
        {
            var @this = source is ISet<T> sourceSet ? sourceSet : new HashSet<T>(source);
            var that = other is ISet<T> otherSet ? otherSet : new HashSet<T>(other);
            var elementCount = 0;
            var matchCount = 0;
            foreach (var item in that)
            {
                elementCount++;
                if (@this.Contains(item))
                {
                    matchCount++;
                }
            }

            if (proper)
            {
                return matchCount == @this.Count && elementCount > @this.Count;
            }

            return matchCount == @this.Count;
        }

        private static bool IsSupersetOf(IEnumerable<T> source, IEnumerable<T> other, bool proper)
        {
            var @this = source is ISet<T> sourceSet ? sourceSet : new HashSet<T>(source);
            var that = other is ISet<T> otherSet ? otherSet : new HashSet<T>(other);
            var elementCount = 0;
            foreach (var item in that)
            {
                elementCount++;
                if (!@this.Contains(item))
                {
                    return false;
                }
            }

            if (proper)
            {
                return elementCount < @this.Count;
            }

            return true;
        }
    }
}