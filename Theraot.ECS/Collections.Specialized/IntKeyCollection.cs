// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Collections.Specialized
{
    /// <summary>
    /// Represents a collection operated by int keys, which keeps the values compact in memory
    /// </summary>
    /// <typeparam name="TValue">The type of the values</typeparam>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public sealed
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
        partial
#endif
        class IntKeyCollection<TValue> : ICollection<TValue>, IIntKeyCollection<TValue>
    {
        private int _key;
        private int[] _keys;
        private TValue[] _values;

        /// <summary>
        /// Initializes a new instance of <see cref="IntKeyCollection{TValue}"/> with the provided initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity for the new instance.</param>
        public IntKeyCollection(int initialCapacity)
        {
            Count = 0;
            _keys = new int[initialCapacity];
            _values = new TValue[initialCapacity];
        }

        /// <summary>
        /// Gets or sets the capacity of this instance
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When attempting to set the capacity smaller than the number of items in the collection.</exception>
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

        /// <inheritdoc />
        public int Count { get; private set; }

        bool ICollection<TValue>.IsReadOnly => false;

        /// <summary>
        /// Gets a value by its int key.
        /// </summary>
        /// <param name="key">The key associate with the value to retrieve.</param>
        /// <exception cref="KeyNotFoundException">When the specified key is not found in this instance.</exception>
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

        void ICollection<TValue>.Add(TValue item)
        {
            Add(item);
        }

        /// <inheritdoc />
        public int Add(TValue value)
        {
            var key = _key++;
            Insert(key, key, value);
            return key;
        }

        /// <summary>
        /// Adds multiple values to the collection.
        /// </summary>
        /// <param name="values">The collection of values to add.</param>
        /// <returns>The list of int keys that was given to the added values, in the same order.</returns>
        /// <exception cref="ArgumentNullException">When the collection of values to add is null.</exception>
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

        /// <inheritdoc />
        public void Clear()
        {
            Array.Clear(_keys, 0, Count);
            Array.Clear(_values, 0, Count);
            Count = 0;
        }

        /// <summary>
        /// Creates a new <see cref="IntKeyCollection{TValue}"/> containing the same values as this one.
        /// </summary>
        /// <returns></returns>
        public IntKeyCollection<TValue> Clone()
        {
            var clone = new IntKeyCollection<TValue>(Count);
            Array.Copy(_keys, 0, clone._keys, 0, Count);
            Array.Copy(_values, 0, clone._values, 0, Count);
            clone.Count = Count;
            return clone;
        }

        /// <summary>
        /// Verifies if the current instance contains the provided value.
        /// </summary>
        /// <param name="item">The value to search for.</param>
        /// <returns>true if the value was found; otherwise, false.</returns>
        public bool Contains(TValue item)
        {
            return IndexOfValue(item) >= 0;
        }

        /// <summary>
        /// Verifies if the current instance contains the provided int key.
        /// </summary>
        /// <param name="key">The int key to search for.</param>
        /// <returns>true if the key was found; otherwise, false.</returns>
        public bool ContainsKey(int key)
        {
            return IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// Copies the values to the provided array.
        /// </summary>
        /// <param name="array">The array to copy the values to.</param>
        /// <param name="arrayIndex">The starting index at which to copy the values.</param>
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            Array.Copy(_values, 0, array, arrayIndex, Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var value in _values)
            {
                yield return value;
            }
        }

        /// <inheritdoc />
        public ref TValue GetRef(int key)
        {
            var index = IndexOfKey(key);
            if (index >= 0)
            {
                return ref _values[index];
            }

            throw new KeyNotFoundException();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <summary>
        /// Remove all the values associated with the provided keys.
        /// </summary>
        /// <param name="source">The collection of keys to remove.</param>
        /// <returns>The list of keys that were removed.</returns>
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

        /// <summary>
        /// Sets the capacity to the number of elements in this instance.
        /// </summary>
        public void TrimToSize()
        {
            Capacity = Count;
        }

        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key by which to find the value.</param>
        /// <param name="value">The retrieved value.</param>
        /// <returns>true if the key was found; otherwise, false.</returns>
        public bool TryGetValue(int key, out TValue value)
        {
            var index = IndexOfKey(key);
            if (index >= 0)
            {
                value = _values[index];
                return true;
            }

            value = default!;
            return false;
        }

        /// <inheritdoc />
        public int Update(int key, TValue value)
        {
            var index = IndexOfKey(key);
            if (index < 0)
            {
                throw new KeyNotFoundException();
            }

            _values[index] = value;
            return key;
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

        private int IndexOfValue(TValue value)
        {
            return Array.IndexOf(_values, value, 0, Count);
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

            _keys[Count] = default!;
            _values[Count] = default!;
        }
    }

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

    public sealed partial class IntKeyCollection<TValue> : ICloneable
    {
        object ICloneable.Clone()
        {
            return Clone();
        }
    }

#endif
}