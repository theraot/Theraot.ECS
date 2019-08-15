// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.ECS;

namespace Theraot.Collections.Specialized
{
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public sealed class IndexedCollection<TValue> : ICollection<TValue>, ICloneable
    {
        private int _key;
        private int[] _keys;
        private TValue[] _values;

        public IndexedCollection(int initialCapacity)
        {
            Count = 0;
            _keys = new int[initialCapacity];
            _values = new TValue[initialCapacity];
        }

        public int Capacity
        {
            get => _keys.Length;
            set
            {
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");
                }

                if (value == _keys.Length)
                {
                    return;
                }

                if (value > 0)
                {
                    var newKeys = new int[value];
                    var newValues = new TValue[value];
                    if (Count > 0)
                    {
                        Array.Copy(_keys, 0, newKeys, 0, Count);
                        Array.Copy(_values, 0, newValues, 0, Count);
                    }
                    _keys = newKeys;
                    _values = newValues;
                }
                else
                {
                    // size can only be zero here.
                    Debug.Assert(Count == 0, "Size is not zero");
                    _keys = EmptyArray<int>.Instance;
                    _values = EmptyArray<TValue>.Instance;
                }
            }
        }

        public int Count { get; private set; }

        bool ICollection<TValue>.IsReadOnly => false;

        public TValue this[int key]
        {
            get
            {
                var index = IndexOfKey(key);
                if (index >= 0)
                {
                    return _values[index];
                }

                throw new KeyNotFoundException();
            }
        }

        void ICollection<TValue>.Add(TValue value)
        {
            Add(value);
        }

        public int Add(TValue value)
        {
            var key = _key++;
            Insert(key, key, value);
            return key;
        }

        public List<int> AddRange(IList<TValue> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var result = new List<int>(values.Count);
            foreach (var value in values)
            {
                result.Add(Add(value));
            }

            return result;
        }

        public void Clear()
        {
            Array.Clear(_keys, 0, Count);
            Array.Clear(_values, 0, Count);
            Count = 0;
        }

        public object Clone()
        {
            var clone = new IndexedCollection<TValue>(Count);
            Array.Copy(_keys, 0, clone._keys, 0, Count);
            Array.Copy(_values, 0, clone._values, 0, Count);
            clone.Count = Count;
            return clone;
        }

        public bool Contains(TValue item)
        {
            return IndexOfValue(item) >= 0;
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            Array.Copy(_values, 0, array, arrayIndex, Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var value in _values)
            {
                yield return value;
            }
        }

        public int IndexOfValue(TValue value)
        {
            return Array.IndexOf(_values, value, 0, Count);
        }

        public bool Remove(int key)
        {
            var index = IndexOfKey(key);
            if (index < 0)
            {
                return false;
            }

            RemoveAtExtracted(index);
            return true;
        }

        public bool Remove(TValue item)
        {
            var index = IndexOfValue(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAtExtracted(index);
            return true;
        }

        public List<int> RemoveAll(IEnumerable<int> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var result = new List<int>();
            foreach (var key in source)
            {
                if (Remove(key))
                {
                    result.Add(key);
                }
            }

            return result;
        }

        public int Set(int key, TValue value)
        {
            var index = IndexOfKey(key);
            if (index < 0)
            {
                throw new KeyNotFoundException();
            }

            _values[index] = value;
            return key;
        }

        public void TrimToSize()
        {
            Capacity = Count;
        }

        public bool TryGetValue(int key, out TValue value)
        {
            var index = IndexOfKey(key);
            if (index >= 0)
            {
                value = _values[index];
                return true;
            }

            value = default;
            return false;
        }

        private void EnsureCapacity(int min)
        {
            var newCapacity = _keys.Length == 0 ? 16 : _keys.Length * 2;
            if ((uint)newCapacity > 0X7FEFFFFF)
            {
                newCapacity = 0X7FEFFFFF;
            }

            if (newCapacity < min)
            {
                newCapacity = min;
            }

            Capacity = newCapacity;
        }

        private int IndexOfKey(int key)
        {
            var ret = Array.BinarySearch(_keys, 0, Count, key);
            return ret >= 0 ? ret : -1;
        }

        private void Insert(int index, int key, TValue value)
        {
            if (Count == _keys.Length)
            {
                EnsureCapacity(Count + 1);
            }

            if (index < Count)
            {
                Array.Copy(_keys, index, _keys, index + 1, Count - index);
                Array.Copy(_values, index, _values, index + 1, Count - index);
            }
            _keys[index] = key;
            _values[index] = value;
            Count++;
        }

        private void RemoveAtExtracted(int index)
        {
            Count--;
            if (index < Count)
            {
                Array.Copy(_keys, index + 1, _keys, index, Count - index);
                Array.Copy(_values, index + 1, _values, index, Count - index);
            }

            _keys[Count] = default;
            _values[Count] = default;
        }
    }
}