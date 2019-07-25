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

            if (!(other is BitSet bitSet))
            {
                return this.IsSubsetOf(other, true);
            }

            var a = _wrapped;
            var b = bitSet._wrapped;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * b.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/###/\   \##|
             * |#|###|  |   |#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * a.And(b.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\   \  |
             * | |###|  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, it means a is a subset of b
             *
             * a.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/   /\###\##|
             * |#|   |  |###|#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * b.And(a.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\###\  |
             * | |   |  |###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If also this is not empty, it means a is a proper subset of b
             */
            return IsEmpty(a.And(b.Not())) && !IsEmpty(b.And(a.Not()));
        }

        public bool IsProperSupersetOf(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (!(other is BitSet bitSet))
            {
                return this.IsSupersetOf(other, true);
            }

            var a = _wrapped;
            var b = bitSet._wrapped;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/   /\###\##|
             * |#|   |  |###|#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * b.And(a.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\###\  |
             * | |   |  |###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, it means a is superset of b
             *
             * b.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/###/\   \##|
             * |#|###|  |   |#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * a.And(b.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\   \  |
             * | |###|  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If also this is not empty, it means a is a proper superset of b
             */
            return IsEmpty(b.And(a.Not())) && !IsEmpty(a.And(b.Not()));
        }

        public bool IsSubsetOf(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (!(other is BitSet bitSet))
            {
                return this.IsSubsetOf(other, false);
            }

            var a = _wrapped;
            var b = bitSet._wrapped;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * b.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/###/\   \##|
             * |#|###|  |   |#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * a.And(b.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\   \  |
             * | |###|  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, it means a is a subset of b
             */
            return IsEmpty(a.And(b.Not()));
        }

        public bool IsSupersetOf(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (!(other is BitSet bitSet))
            {
                return this.IsSupersetOf(other, false);
            }

            var a = _wrapped;
            var b = bitSet._wrapped;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/   /\###\##|
             * |#|   |  |###|#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * b.And(a.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\###\  |
             * | |   |  |###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, it means a is superset of b
             */
            return IsEmpty(b.And(a.Not()));
        }

        public bool Overlaps(IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (!(other is BitSet bitSet))
            {
                return other.Any(Contains);
            }
            var a = _wrapped;
            var b = bitSet._wrapped;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.And(b)
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\   \  |
             * | |   |##|   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             */
            return IsEmpty(a.And(b));
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
            if (!(other is BitSet bitSet))
            {
                var otherAsICollection = other is ICollection<int> otherCollection ? otherCollection : other.ToArray();
                return otherAsICollection.All(Contains) && this.All(input => otherAsICollection.Contains(input));
            }
            var a = _wrapped;
            var b = bitSet._wrapped;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.Or(b)
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\###\  |
             * | |###|##|###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.And(b)
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\   \  |
             * | |   |##|   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.And(b).Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/###/\###\##|
             * |#|###|  |###|#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * a.Or(b).And(a.And(b).Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\###\  |
             * | |###|  |###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, the sets are equal
             */
            return IsEmpty(a.Or(b).And(a.And(b).Not()));
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

        private static bool IsEmpty(BitArray result)
        {
            var integers = new int[(result.Count >> 5) + 1];
            result.CopyTo(integers, 0);
            foreach (var integer in integers)
            {
                if (integer != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}