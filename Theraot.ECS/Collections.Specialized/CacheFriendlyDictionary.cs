// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Class:  CacheFriendlyDictionary
**
** Purpose: Represents a collection of key/value pairs
**          that are sorted by the keys and are accessible
**          by key and by index.
**
===========================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.ECS;

namespace Theraot.Collections.Specialized
{
    // The CacheFriendlyDictionary class implements a cache friendly dictionary of keys and values. Entries in
    // a cache friendly dictionary are sorted by their keys and are accessible both by key and by
    // index. The keys of a cache friendly dictionary can be ordered either according to a
    // specific IComparer implementation given when the cache friendly dictionary is
    // instantiated, or according to the IComparable implementation provided
    // by the keys themselves. In either case, a cache friendly dictionary does not allow entries
    // with duplicate keys.
    //
    // A cache friendly dictionary internally maintains two arrays that store the keys and
    // values of the entries. The capacity of a cache friendly dictionary is the allocated
    // length of these internal arrays. As elements are added to a cache friendly dictionary, the
    // capacity of the cache friendly dictionary is automatically increased as required by
    // reallocating the internal arrays.  The capacity is never automatically
    // decreased, but users can call either TrimToSize or
    // Capacity explicitly.
    //
    // The GetKeyList and GetValueList methods of a cache friendly dictionary
    // provides access to the keys and values of the cache friendly dictionary in the form of
    // List implementations. The List objects returned by these
    // methods are aliases for the underlying cache friendly dictionary, so modifications
    // made to those lists are directly reflected in the cache friendly dictionary, and vice
    // versa.
    //
    // The CacheFriendlyDictionary class provides a convenient way to create a sorted
    // copy of another dictionary, such as a Hashtable. For example:
    //
    // Hashtable h = new Hashtable();
    // h.Add(...);
    // h.Add(...);
    // ...
    // CacheFriendlyDictionary s = new CacheFriendlyDictionary(h);
    //
    // The last line above creates a cache friendly dictionary that contains a copy of the keys
    // and values stored in the hashtable. In this particular example, the keys
    // will be ordered according to the IComparable interface, which they
    // all must implement. To impose a different ordering, CacheFriendlyDictionary also
    // has a constructor that allows a specific IComparer implementation to
    // be specified.
    //
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public sealed class CacheFriendlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICloneable
    {
        private IComparer<TKey> _comparer;
        private KeyList _keyList;
        private TKey[] _keys;
        private ValueList _valueList;
        private TValue[] _values;

        // Constructs a new cache friendly dictionary. The cache friendly dictionary is initially empty and has
        // a capacity of zero. Upon adding the first element to the cache friendly dictionary the
        // capacity is increased to 16, and then increased in multiples of two as
        // required. The elements of the cache friendly dictionary are ordered according to the
        // IComparable interface, which must be implemented by the keys of
        // all entries added to the cache friendly dictionary.
        public CacheFriendlyDictionary()
        {
            Init();
        }

        // Constructs a new cache friendly dictionary. The cache friendly dictionary is initially empty and has
        // a capacity of zero. Upon adding the first element to the cache friendly dictionary the
        // capacity is increased to 16, and then increased in multiples of two as
        // required. The elements of the cache friendly dictionary are ordered according to the
        // IComparable interface, which must be implemented by the keys of
        // all entries added to the cache friendly dictionary.
        //
        public CacheFriendlyDictionary(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Non-negative number required.");
            }

            _keys = new TKey[initialCapacity];
            _values = new TValue[initialCapacity];
            _comparer = Comparer<TKey>.Default;
        }

        // Constructs a new cache friendly dictionary with a given IComparer
        // implementation. The cache friendly dictionary is initially empty and has a capacity of
        // zero. Upon adding the first element to the cache friendly dictionary the capacity is
        // increased to 16, and then increased in multiples of two as required. The
        // elements of the cache friendly dictionary are ordered according to the given
        // IComparer implementation. If comparer is null, the
        // elements are compared to each other using the IComparable
        // interface, which in that case must be implemented by the keys of all
        // entries added to the cache friendly dictionary.
        //
        public CacheFriendlyDictionary(IComparer<TKey> comparer)
            : this()
        {
            if (comparer != null)
            {
                _comparer = comparer;
            }
        }

        // Constructs a new cache friendly dictionary with a given IComparer
        // implementation and a given initial capacity. The cache friendly dictionary is
        // initially empty, but will have room for the given number of elements
        // before any reallocations are required. The elements of the cache friendly dictionary
        // are ordered according to the given IComparer implementation. If
        // comparer is null, the elements are compared to each other using
        // the IComparable interface, which in that case must be implemented
        // by the keys of all entries added to the cache friendly dictionary.
        //
        public CacheFriendlyDictionary(IComparer<TKey> comparer, int capacity)
            : this(comparer)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Capacity = capacity;
        }

        // Constructs a new cache friendly dictionary containing a copy of the entries in the
        // given dictionary. The elements of the cache friendly dictionary are ordered according
        // to the IComparable interface, which must be implemented by the
        // keys of all entries in the given dictionary as well as keys
        // subsequently added to the cache friendly dictionary.
        //
        public CacheFriendlyDictionary(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
            // Empty
        }

        // Constructs a new cache friendly dictionary containing a copy of the entries in the
        // given dictionary. The elements of the cache friendly dictionary are ordered according
        // to the given IComparer implementation. If comparer is
        // null, the elements are compared to each other using the
        // IComparable interface, which in that case must be implemented
        // by the keys of all entries in the given dictionary as well as keys
        // subsequently added to the cache friendly dictionary.
        //
        public CacheFriendlyDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
            : this(comparer, dictionary?.Count ?? 0)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "Dictionary cannot be null.");
            }

            dictionary.Keys.CopyTo(_keys, 0);
            dictionary.Values.CopyTo(_values, 0);

            // Array.Sort(Array keys, Array values, IComparer comparer) does not exist in System.Runtime contract v4.0.10.0.
            // This works around that by sorting only on the keys and then assigning values accordingly.
            Array.Sort(_keys, comparer);
            for (var index = 0; index < _keys.Length; index++)
            {
                _values[index] = dictionary[_keys[index]];
            }
            Count = dictionary.Count;
        }

        // Returns the capacity of this cache friendly dictionary. The capacity of a cache friendly dictionary
        // represents the allocated length of the internal arrays used to store the
        // keys and values of the list, and thus also indicates the maximum number
        // of entries the list can contain before a reallocation of the internal
        // arrays is required.
        //
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

        // Returns the number of entries in this cache friendly dictionary.
        //
        public int Count { get; private set; }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        // Returns a collection representing the keys of this cache friendly dictionary. This
        // method returns the same object as GetKeyList, but typed as an
        // ICollection instead of an IList.
        //
        public ICollection<TKey> Keys => GetKeyList();

        // Returns a collection representing the values of this cache friendly dictionary. This
        // method returns the same object as GetValueList, but typed as an
        // ICollection instead of an IList.
        //
        public ICollection<TValue> Values => GetValueList();

        // Returns the value associated with the given key. If an entry with the
        // given key is not found, the returned value is null.
        //
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

        // Adds an entry with the given key and value to this cache friendly dictionary. An
        // ArgumentException is thrown if the key is already present in the cache friendly dictionary.
        //
        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            var index = Array.BinarySearch(_keys, 0, Count, key, _comparer);
            if (index >= 0)
            {
                throw new ArgumentException("Item has already been added. Key in dictionary: '{GetKey(i)}'  Key being added: '{key}'");
            }

            Insert(~index, key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        // Removes all entries from this cache friendly dictionary.
        public void Clear()
        {
            // clear does not change the capacity
            Array.Clear(_keys, 0, Count); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            Array.Clear(_values, 0, Count); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            Count = 0;
        }

        // Makes a virtually identical copy of this CacheFriendlyDictionary.  This is a shallow
        // copy.  IE, the Objects in the CacheFriendlyDictionary are not cloned - we copy the
        // references to those objects.
        public object Clone()
        {
            var clone = new CacheFriendlyDictionary<TKey, TValue>(Count);
            Array.Copy(_keys, 0, clone._keys, 0, Count);
            Array.Copy(_values, 0, clone._values, 0, Count);
            clone.Count = Count;
            clone._comparer = _comparer;
            // Don't copy keyList nor valueList.
            return clone
;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            var index = IndexOfKey(item.Key);
            return index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], item.Value);
        }

        // Checks if this cache friendly dictionary contains an entry with the given key.
        //
        public bool ContainsKey(TKey key)
        {
            // Yes, this is a SPEC'ed duplicate of Contains().
            return IndexOfKey(key) >= 0;
        }

        // Checks if this cache friendly dictionary contains an entry with the given value. The
        // values of the entries of the cache friendly dictionary are compared to the given value
        // using the Object.Equals method. This method performs a linear
        // search and is substantially slower than the Contains
        // method.
        //
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
                var entry = new DictionaryEntry(_keys[index], _values[index]);
                array.SetValue(entry, index + arrayIndex);
            }
        }

        // Returns the value of the entry at the given index.
        //
        public TValue GetByIndex(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            return _values[index];
        }

        // Returns an IDictionaryEnumerator for this cache friendly dictionary.  If modifications
        // made to the cache friendly dictionary while an enumeration is in progress,
        // the MoveNext and Remove methods
        // of the enumerator will throw an exception.
        //
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (var index = Count - 1; index >= 0; index--)
            {
                yield return new KeyValuePair<TKey, TValue>(_keys[index], _values[index]);
            }
        }

        // Returns an IEnumerator for this cache friendly dictionary.  If modifications
        // made to the cache friendly dictionary while an enumeration is in progress,
        // the MoveNext and Remove methods
        // of the enumerator will throw an exception.
        //
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Returns the key of the entry at the given index.
        //
        public TKey GetKey(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            return _keys[index];
        }

        // Returns an IList representing the keys of this cache friendly dictionary. The
        // returned list is an alias for the keys of this cache friendly dictionary, so
        // modifications made to the returned list are directly reflected in the
        // underlying cache friendly dictionary, and vice versa. The elements of the returned
        // list are ordered in the same way as the elements of the cache friendly dictionary. The
        // returned list does not support adding, inserting, or modifying elements
        // (the Add, AddRange, Insert, InsertRange,
        // Reverse, Set, SetRange, and Sort methods
        // throw exceptions), but it does allow removal of elements (through the
        // Remove and RemoveRange methods or through an enumerator).
        // Null is an invalid key value.
        //
        public IList<TKey> GetKeyList()
        {
            return _keyList ?? (_keyList = new KeyList(this));
        }

        // Returns an IList representing the values of this cache friendly dictionary. The
        // returned list is an alias for the values of this cache friendly dictionary, so
        // modifications made to the returned list are directly reflected in the
        // underlying cache friendly dictionary, and vice versa. The elements of the returned
        // list are ordered in the same way as the elements of the cache friendly dictionary. The
        // returned list does not support adding or inserting elements (the
        // Add, AddRange, Insert and InsertRange
        // methods throw exceptions), but it does allow modification and removal of
        // elements (through the Remove, RemoveRange, Set and
        // SetRange methods or through an enumerator).
        //
        public IList<TValue> GetValueList()
        {
            return _valueList ?? (_valueList = new ValueList(this));
        }

        // Returns the index of the entry with a given key in this cache friendly dictionary. The
        // key is located through a binary search, and thus the average execution
        // time of this method is proportional to Log2(size), where
        // size is the size of this cache friendly dictionary. The returned value is -1 if
        // the given key does not occur in this cache friendly dictionary. Null is an invalid
        // key value.
        //
        public int IndexOfKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            var ret = Array.BinarySearch(_keys, 0, Count, key, _comparer);
            return ret >= 0 ? ret : -1;
        }

        // Returns the index of the first occurrence of an entry with a given value
        // in this cache friendly dictionary. The entry is located through a linear search, and
        // thus the average execution time of this method is proportional to the
        // size of this cache friendly dictionary. The elements of the list are compared to the
        // given value using the Object.Equals method.
        //
        public int IndexOfValue(TValue value)
        {
            return Array.IndexOf(_values, value, 0, Count);
        }

        // Removes an entry from this cache friendly dictionary. If an entry with the specified
        // key exists in the cache friendly dictionary, it is removed. An ArgumentException is
        // thrown if the key is null.
        //
        public bool Remove(TKey key)
        {
            var index = IndexOfKey(key);
            if (index < 0)
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var index = IndexOfKey(item.Key);
            if (index < 0 || !EqualityComparer<TValue>.Default.Equals(_values[index], item.Value))
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }

        // Removes the entry at the given index. The size of the cache friendly dictionary is
        // decreased by one.
        //
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            Count--;
            if (index < Count)
            {
                Array.Copy(_keys, index + 1, _keys, index, Count - index);
                Array.Copy(_values, index + 1, _values, index, Count - index);
            }
            _keys[Count] = default;
            _values[Count] = default;
        }

        // Sets the capacity of this cache friendly dictionary to the size of the cache friendly dictionary.
        // This method can be used to minimize a cache friendly dictionary's memory overhead once
        // it is known that no new elements will be added to the cache friendly dictionary. To
        // completely clear a cache friendly dictionary and release all memory referenced by the
        // cache friendly dictionary, execute the following statements:
        //
        // cacheFriendlyDictionary.Clear();
        // cacheFriendlyDictionary.TrimToSize();
        //
        public void TrimToSize()
        {
            Capacity = Count;
        }

        public bool TryGetValue(TKey key, out TValue value)
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

        // Copies the values in this CacheFriendlyDictionary to an KeyValuePairs array.
        // KeyValuePairs is different from Dictionary Entry in that it has special
        // debugger attributes on its fields.
        // Ensures that the capacity of this cache friendly dictionary is at least the given
        // minimum value. If the current capacity of the list is less than
        // min, the capacity is increased to twice the current capacity or
        // to min, whichever is larger.
        private void EnsureCapacity(int min)
        {
            var newCapacity = _keys.Length == 0 ? 16 : _keys.Length * 2;
            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
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

        private void Init()
        {
            _keys = EmptyArray<TKey>.Instance;
            _values = EmptyArray<TValue>.Instance;
            Count = 0;
            _comparer = Comparer<TKey>.Default;
        }

        // Inserts an entry with a given key and value at a given index.
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

        private sealed class KeyList : IList<TKey>
        {
            private readonly CacheFriendlyDictionary<TKey, TValue> _dictionary;

            internal KeyList(CacheFriendlyDictionary<TKey, TValue> dictionary)
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

            public TKey this[int index] => _dictionary.GetKey(index);

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
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
                foreach (var key in _dictionary._keys)
                {
                    yield return key;
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
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }

            public bool Remove(TKey item)
            {
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }

            void IList<TKey>.RemoveAt(int index)
            {
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }
        }

        private sealed class ValueList : IList<TValue>
        {
            private readonly CacheFriendlyDictionary<TKey, TValue> _dictionary;

            internal ValueList(CacheFriendlyDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public int Count => _dictionary.Count;

            public bool IsReadOnly => true;

            public TValue this[int index] => _dictionary.GetByIndex(index);

            TValue IList<TValue>.this[int index]
            {
                get => this[index];
                set => throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }

            void ICollection<TValue>.Add(TValue value)
            {
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }

            public bool Contains(TValue value)
            {
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
                foreach (var value in _dictionary._values)
                {
                    yield return value;
                }
            }

            public int IndexOf(TValue value)
            {
                return Array.IndexOf(_dictionary._values, value, 0, _dictionary.Count);
            }

            public void Insert(int index, TValue value)
            {
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }

            public bool Remove(TValue value)
            {
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("This operation is not supported on CacheFriendlyDictionary nested types because they require modifying the original CacheFriendlyDictionary.");
            }
        }
    }
}