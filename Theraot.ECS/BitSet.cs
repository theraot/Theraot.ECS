using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.ECS
{
    public class BitSet : ISet<int>
    {
        private readonly int _capacity;
        private readonly BitArray _wrapped;
        private int _count;

        public BitSet(int capacity)
        {
            _capacity = capacity;
            _wrapped = new BitArray(capacity);
            _count = 0;
        }

        public int Count => _count;

        bool ICollection<int>.IsReadOnly => false;

        void ICollection<int>.Add(int item)
        {
            Add(item);
        }

        public bool Add(int item)
        {
            if (_wrapped[item])
            {
                return false;
            }
            _wrapped[item] = true;
            _count++;
            return true;
        }

        public void Clear()
        {
            _wrapped.SetAll(false);
            _count = 0;
        }

        public bool Contains(int item)
        {
            return _wrapped[item];
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (var index in other)
            {
                Remove(index);
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (var index = 0; index < _capacity; index++)
            {
                if (_wrapped[index])
                {
                    yield return index;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void IntersectWith(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            var otherAsICollection = other is ICollection<int> otherCollection ? otherCollection : other.ToArray();
            RemoveWhere(input => !otherAsICollection.Contains(input));
        }

        public bool IsProperSubsetOf(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return this.IsSubsetOf(other, true);
        }

        public bool IsProperSupersetOf(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return this.IsSupersetOf(other, true);
        }

        public bool IsSubsetOf(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return this.IsSubsetOf(other, false);
        }

        public bool IsSupersetOf(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return this.IsSupersetOf(other, false);
        }

        public bool Overlaps(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return other.Any(Contains);
        }

        public bool Remove(int item)
        {
            if (!_wrapped[item])
            {
                return false;
            }
            _wrapped[item] = false;
            _count--;
            return true;
        }

        public int RemoveWhere(Func<int, bool> predicate)
        {
            var count = 0;
            for (var index = 0; index < _capacity; index++)
            {
                if (!_wrapped[index] || !predicate(index))
                {
                    continue;
                }
                _wrapped[index] = false;
                _count--;
                count++;
            }

            return count;
        }

        public bool SetEquals(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            var otherAsICollection = other is ICollection<int> otherCollection ? otherCollection : other.ToArray();
            return otherAsICollection.All(Contains) && this.All(input => otherAsICollection.Contains(input));
        }

        public void SymmetricExceptWith(IEnumerable<int> other)
        {
            UnionWith(other.Distinct().Where(input => !Remove(input)));
        }

        public void UnionWith(IEnumerable<int> other)
        {
            foreach (var index in other)
            {
                Add(index);
            }
        }
    }
}