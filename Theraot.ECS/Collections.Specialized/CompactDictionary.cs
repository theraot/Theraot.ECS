// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable RECS0017 // Possible compare of value type with 'null'
#pragma warning disable RECS0096 // Type parameter is never used
// ReSharper disable MemberCanBePrivate.Local

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Collections.Specialized
{
    /// <summary>
    /// Represents a dictionary that keeps keys and values compact in memory.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public sealed partial class CompactDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : notnull
    {
        private readonly IComparer<TKey> _comparer;
        private KeyList? _keyList;
        private TKey[] _keys;
        private ValueList? _valueList;
        private TValue[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactDictionary{TKey, TValue}"/> class, with the indicated <see cref="IComparer{T}"/> <paramref name="comparer"/> and <paramref name="initialCapacity"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> used to sort the keys.</param>
        /// <param name="initialCapacity">The initial capacity of the dictionary.</param>
        public CompactDictionary(IComparer<TKey> comparer, int initialCapacity)
        {
            _comparer = comparer ?? Comparer<TKey>.Default;
            Count = 0;
            _keys = new TKey[initialCapacity];
            _values = new TValue[initialCapacity];
        }

        /// <summary>
        /// Gets or sets the current capacity of the dictionary.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When attempting to set a capacity smaller than the current number of elements in the dictionary.</exception>
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
                    var newKeys = new TKey[value];
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
                    _keys = EmptyArray<TKey>.Instance;
                    _values = EmptyArray<TValue>.Instance;
                }
            }
        }

        /// <inheritdoc />
        public int Count { get; private set; }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        /// <inheritdoc />
        public ICollection<TKey> Keys => GetKeyList();

        /// <inheritdoc />
        public ICollection<TValue> Values => GetValueList();

        /// <inheritdoc />
        public TValue this[TKey key]
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
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key), "Key cannot be null.");
                }

                var index = Array.BinarySearch(_keys, 0, Count, key, _comparer);
                if (index >= 0)
                {
                    _values[index] = value;
                    return;
                }
                Insert(~index, key, value);
            }
        }

        /// <inheritdoc />
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
            {
                throw new ArgumentException("Item has already been added.");
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            Array.Clear(_keys, 0, Count);
            Array.Clear(_values, 0, Count);
            Count = 0;
        }

        /// <summary>
        /// Gets a new <see cref="CompactDictionary{TKey, TValue}"/> copy of this instance.
        /// </summary>
        public CompactDictionary<TKey, TValue> Clone()
        {
            var clone = new CompactDictionary<TKey, TValue>(_comparer, Count);
            Array.Copy(_keys, 0, clone._keys, 0, Count);
            Array.Copy(_values, 0, clone._values, 0, Count);
            clone.Count = Count;
            return clone;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            var index = IndexOfKey(item.Key);
            return index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], item.Value);
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            return IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// Determines whether the <see cref="CompactDictionary{TKey, TValue}"/> contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        public bool ContainsValue(TValue value)
        {
            return IndexOfValue(value) >= 0;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Array cannot be null.");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number required.");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
            }

            for (var index = 0; index < Count; index++)
            {
                array[index + arrayIndex] = new KeyValuePair<TKey, TValue>(_keys[index], _values[index]);
            }
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (var index = Count - 1; index >= 0; index--)
            {
                yield return new KeyValuePair<TKey, TValue>(_keys[index], _values[index]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            var index = IndexOfKey(key);
            if (index < 0)
            {
                return false;
            }

            RemoveAtExtracted(index);
            return true;
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="CompactDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="removedValue">The value that was removed</param>
        /// <returns>true if the value was removed; otherwise, false.</returns>
        public bool Remove(TKey key, out TValue removedValue)
        {
            var index = IndexOfKey(key);
            if (index < 0)
            {
                removedValue = default!;
                return false;
            }

            removedValue = _values[index];
            RemoveAtExtracted(index);
            return true;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var index = IndexOfKey(item.Key);
            if (index < 0 || !EqualityComparer<TValue>.Default.Equals(_values[index], item.Value))
            {
                return false;
            }

            RemoveAtExtracted(index);
            return true;
        }

        /// <summary>
        /// Removes the values with the specified keys from the <see cref="CompactDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="source">The collection of keys to remove.</param>
        /// <param name="removedKeys">The list of keys that were removed.</param>
        /// <param name="removedValues">The list of values that were removed.</param>
        /// <returns>The number of removed elements.</returns>
        /// <remarks>The keys in <paramref name="removedKeys"/> and the values in <paramref name="removedValues"/> are in the same order.</remarks>
        public int RemoveAll(IEnumerable<TKey> source, out List<TKey> removedKeys, out List<TValue> removedValues)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            removedKeys = new List<TKey>();
            removedValues = new List<TValue>();
            var count = 0;
            foreach (var key in source)
            {
                if (!Remove(key, out var removedValue))
                {
                    continue;
                }

                removedKeys.Add(key);
                removedValues.Add(removedValue);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Sets the value to be associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">The value associated with the specified key.</param>
        /// <returns>true if key is new; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">When the provided key does not exist.</exception>
        public bool Set(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            var index = Array.BinarySearch(_keys, 0, Count, key, _comparer);
            if (index >= 0)
            {
                _values[index] = value;
                return false;
            }

            Insert(~index, key, value);
            return true;
        }

        /// <summary>
        /// Sets multiple values from a pair of lists of keys and values.
        /// </summary>
        /// <param name="keys">The list of keys to which to set.</param>
        /// <param name="values">The list of values to set.</param>
        /// <returns>A list containing the keys that were new.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="keys"/> or <paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="keys"/> and <paramref name="values"/> have different number of elements.</exception>
        /// <remarks>The elements are paired in the order in which they are provided.</remarks>
        public List<TKey> SetAll(IList<TKey> keys, IList<TValue> values)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (values.Count != keys.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Count does not match");
            }

            var result = new List<TKey>();
            for (var index = 0; index < keys.Count; index++)
            {
                var key = keys[index];
                var value = values[index];
                if (Set(key, value))
                {
                    result.Add(key);
                }
            }

            return result;
        }

        /// <summary>
        /// Sets multiple values from a list of <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        /// <param name="source">The list of <see cref="KeyValuePair{TKey, TValue}"/>.</param>
        /// <returns>A list containing the keys that were new.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is null.</exception>
        public List<TKey> SetAll(IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var result = new List<TKey>();
            foreach (var pair in source)
            {
                var key = pair.Key;
                var value = pair.Value;
                if (Set(key, value))
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
        /// Attempts to add the specified key and value.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>true if the element was added; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">When the specified key is null.</exception>
        public bool TryAdd(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            var index = Array.BinarySearch(_keys, 0, Count, key, _comparer);
            if (index >= 0)
            {
                return false;
            }

            Insert(~index, key, value);
            return true;
        }

        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key by which to find the value.</param>
        /// <param name="value">The retrieved value.</param>
        /// <returns>true if the key was found; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
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

        private IList<TKey> GetKeyList()
        {
            return _keyList ??= new KeyList(this);
        }

        private IList<TValue> GetValueList()
        {
            return _valueList ??= new ValueList(this);
        }

        private int IndexOfKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            var ret = Array.BinarySearch(_keys, 0, Count, key, _comparer);
            return ret >= 0 ? ret : -1;
        }

        private int IndexOfValue(TValue value)
        {
            return Array.IndexOf(_values, value, 0, Count);
        }

        private void Insert(int index, TKey key, TValue value)
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

        /// <inheritdoc />
        private sealed class KeyList : IList<TKey>
        {
            private readonly CompactDictionary<TKey, TValue> _dictionary;

            internal KeyList(CompactDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public int Count => _dictionary.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            TKey IList<TKey>.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
            }

            public TKey this[int index]
            {
                get
                {
                    if (index < 0 || index >= _dictionary.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                    }

                    return _dictionary._keys[index];
                }
            }

            void ICollection<TKey>.Add(TKey item)
            {
                _ = item;
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }

            public bool Contains(TKey item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                return _dictionary.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                Array.Copy(_dictionary._keys, 0, array, arrayIndex, _dictionary.Count);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                for (var index = _dictionary.Count - 1; index >= 0; index--)
                {
                    yield return _dictionary._keys[index];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int IndexOf(TKey item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item), "Key cannot be null.");
                }

                var index = Array.BinarySearch(_dictionary._keys, 0, _dictionary.Count, item, _dictionary._comparer);
                if (index >= 0)
                {
                    return index;
                }

                return -1;
            }

            void IList<TKey>.Insert(int index, TKey item)
            {
                _ = index;
                _ = item;
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }

            public bool Remove(TKey item)
            {
                _ = item;
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }

            void IList<TKey>.RemoveAt(int index)
            {
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }
        }

        /// <inheritdoc />
        private sealed class ValueList : IList<TValue>
        {
            private readonly CompactDictionary<TKey, TValue> _dictionary;

            internal ValueList(CompactDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public int Count => _dictionary.Count;

            public bool IsReadOnly => true;

            public TValue this[int index]
            {
                get
                {
                    if (index < 0 || index >= _dictionary.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
                    }

                    return _dictionary._values[index];
                }
            }

            TValue IList<TValue>.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }

            void ICollection<TValue>.Add(TValue value)
            {
                _ = value;
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }

            public bool Contains(TValue value)
            {
                _ = value;
                return _dictionary.ContainsValue(value);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                Array.Copy(_dictionary._values, 0, array, arrayIndex, _dictionary.Count);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                for (var index = _dictionary.Count - 1; index >= 0; index--)
                {
                    yield return _dictionary._values[index];
                }
            }

            public int IndexOf(TValue value)
            {
                return Array.IndexOf(_dictionary._values, value, 0, _dictionary.Count);
            }

            public void Insert(int index, TValue value)
            {
                _ = index;
                _ = value;
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }

            public bool Remove(TValue value)
            {
                _ = value;
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }

            public void RemoveAt(int index)
            {
                _ = index;
                throw new NotSupportedException("This operation is not supported on CompactDictionary nested types because they require modifying the original CompactDictionary.");
            }
        }
    }

#if LESSTHAN_NET35

    public sealed partial class CompactDictionary<TKey, TValue>
    {
        public bool Set(TKey key, Converter<TKey, TValue> addValueFactory, Converter<KeyValuePair<TKey, TValue>, TValue> updateValueFactory)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }
            if (addValueFactory == null)
            {
                throw new ArgumentNullException(nameof(addValueFactory));
            }
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }

            var index = Array.BinarySearch(_keys, 0, Count, key, _comparer);
            if (index >= 0)
            {
                _values[index] = updateValueFactory(new KeyValuePair<TKey, TValue>(key, _values[index]));
                return false;
            }

            Insert(~index, key, addValueFactory(key));
            return true;
        }

        public List<TKey> SetAll(IEnumerable<TKey> keys, Converter<TKey, TValue> addValueFactory, Converter<KeyValuePair<TKey, TValue>, TValue> updateValueFactory)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (addValueFactory == null)
            {
                throw new ArgumentNullException(nameof(addValueFactory));
            }
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }

            var result = new List<TKey>();
            foreach (var key in keys)
            {
                if (Set(key, addValueFactory, updateValueFactory))
                {
                    result.Add(key);
                }
            }

            return result;
        }
    }

#else

    public sealed partial class CompactDictionary<TKey, TValue>
    {
        public bool Set(TKey key, Func<TKey, TValue> addValueFactory, Func<KeyValuePair<TKey, TValue>, TValue> updateValueFactory)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }
            if (addValueFactory == null)
            {
                throw new ArgumentNullException(nameof(addValueFactory));
            }
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }

            var index = Array.BinarySearch(_keys, 0, Count, key, _comparer);
            if (index >= 0)
            {
                _values[index] = updateValueFactory(new KeyValuePair<TKey, TValue>(key, _values[index]));
                return false;
            }

            Insert(~index, key, addValueFactory(key));
            return true;
        }

        public List<TKey> SetAll(IEnumerable<TKey> keys, Func<TKey, TValue> addValueFactory, Func<KeyValuePair<TKey, TValue>, TValue> updateValueFactory)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if (addValueFactory == null)
            {
                throw new ArgumentNullException(nameof(addValueFactory));
            }
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }

            var result = new List<TKey>();
            foreach (var key in keys)
            {
                if (Set(key, addValueFactory, updateValueFactory))
                {
                    result.Add(key);
                }
            }

            return result;
        }
    }

#endif

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

    public sealed partial class CompactDictionary<TKey, TValue> : ICloneable
    {
        object ICloneable.Clone()
        {
            return Clone();
        }
    }

#endif
}