#if LESSTHAN_NET35

#pragma warning disable CA1031 // Do not catch general exception types
#pragma warning disable IDE0041 // Usar comprobación "is null"

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot
{
    internal class HashSet<T> : ICollection<T>
    {
        private readonly Dictionary<T, object> _dictionary;
        private bool _containsNull;

        public HashSet(IEqualityComparer<T> equalityComparer)
        {
            _dictionary = new Dictionary<T, object>(equalityComparer ?? EqualityComparer<T>.Default);
        }

        public HashSet()
        {
            _dictionary = new Dictionary<T, object>(EqualityComparer<T>.Default);
        }

        public int Count => _dictionary.Count + (_containsNull ? 0 : 1);

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item)
        {
            if (ReferenceEquals(item, null))
            {
                _containsNull = true;
            }
            else
            {
                _dictionary.Add(item, null);
            }
        }

        public bool Add(T item)
        {
            if (ReferenceEquals(item, null))
            {
                var result = !_containsNull;
                _containsNull = true;
                return result;
            }

            try
            {
                _dictionary.Add(item, null);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public void Clear()
        {
            _dictionary.Clear();
            _containsNull = false;
        }

        public bool Contains(T item)
        {
            return ReferenceEquals(item, null) ? _containsNull : _dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_containsNull)
            {
                array[arrayIndex] = default;
                _dictionary.Keys.CopyTo(array, arrayIndex + 1);
            }
            else
            {
                _dictionary.Keys.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_containsNull)
            {
                yield return default;
            }
            foreach (var entry in _dictionary.Keys)
            {
                yield return entry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            foreach (var item in EnumerableHelper.AsICollection(other))
            {
                if (!Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            foreach (var entry in other)
            {
                if (Contains(entry))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Remove(T item)
        {
            if (!ReferenceEquals(item, null))
            {
                return _dictionary.Remove(item);
            }
            if (!_containsNull)
            {
                return false;
            }
            _containsNull = false;
            return true;
        }

        public bool SetEquals(HashSet<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (_containsNull != other._containsNull)
            {
                return false;
            }

            foreach (var entry in other._dictionary.Keys)
            {
                if (!Contains(entry))
                {
                    return false;
                }
            }
            foreach (var entry in _dictionary.Keys)
            {
                if (!other.Contains(entry))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

#endif