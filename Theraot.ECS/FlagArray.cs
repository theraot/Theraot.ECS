using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    public sealed class FlagArray : IList<bool>, ICloneable
    {
        private readonly int[] _entries;

        public FlagArray(FlagArray prototype)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype), $"{nameof(prototype)} is null.");
            }
            Capacity = prototype.Capacity;
            _entries = new int[GetLength(Capacity)];
            prototype._entries.CopyTo(_entries, 0);
        }

        public FlagArray(int capacity, FlagArray prototype)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype), $"{nameof(prototype)} is null.");
            }
            if (capacity < prototype.Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"{nameof(capacity)} < {nameof(prototype)}.{nameof(Capacity)}.");
            }
            Capacity = capacity;
            _entries = new int[GetLength(Capacity)];
            prototype._entries.CopyTo(_entries, 0);
        }

        public FlagArray(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"{nameof(capacity)} < 0");
            }
            Capacity = capacity;
            _entries = new int[GetLength(Capacity)];
        }

        public FlagArray(int capacity, bool defaultValue)
            : this(capacity)
        {
            if (defaultValue)
            {
                SetAll(true);
            }
        }

        public int Capacity { get; }

        public int Count
        {
            get
            {
                var count = 0;
                foreach (var entry in _entries)
                {
                    count += PopulationCount(entry);
                }
                return count;
            }
        }

        public IEnumerable<int> Flags
        {
            get
            {
                var bitIndex = 0;
                foreach (var entry in _entries)
                {
                    if (entry == 0)
                    {
                        bitIndex += 32;
                        if (bitIndex >= Capacity)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        foreach (var bit in BitsBinary(BinaryReverse(entry)))
                        {
                            if (bit == 1)
                            {
                                yield return bitIndex;
                            }
                            bitIndex++;
                            if (bitIndex == Capacity)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }
        }

        bool ICollection<bool>.IsReadOnly => false;

        public bool this[int index]
        {
            get
            {
                var entryIndex = index >> 5;
                var bit = index & 31;
                var mask = 1 << bit;
                return GetBit(entryIndex, mask);
            }
            set
            {
                if (index > Capacity)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                var entryIndex = index >> 5;
                var bit = index & 31;
                var mask = 1 << bit;
                if (value)
                {
                    SetBit(entryIndex, mask);
                }
                else
                {
                    UnsetBit(entryIndex, mask);
                }
            }
        }

        void ICollection<bool>.Add(bool item)
        {
            throw new NotSupportedException();
        }

        public FlagArray And(FlagArray other)
        {
            var order = Capacity > other.Capacity;
            var b = order ? _entries : other._entries;
            var a = order ? other._entries : _entries;
            var result = new FlagArray(order ? other.Capacity : Capacity);
            for (var index = 0; index < a.Length; index++)
            {
                result._entries[index] = a[index] & b[index];
            }
            return result;
        }

        void ICollection<bool>.Clear()
        {
            throw new NotSupportedException();
        }

        public FlagArray Clone()
        {
            return new FlagArray(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public bool Contains(bool item)
        {
            var nextBitIndex = 0;
            var check = item ? 0 : -1;
            foreach (var entry in _entries)
            {
                nextBitIndex += 32;
                if (nextBitIndex <= Capacity)
                {
                    if (entry != check)
                    {
                        return true;
                    }
                }
                else
                {
                    var mask = GetMask(Capacity);
                    if ((mask & entry) != (mask & check))
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        public bool Contains(bool item, IEqualityComparer<bool> comparer)
        {
            return Enumerable.Contains(this, item, comparer);
        }

        public void CopyTo(bool[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }
            if (Capacity > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }
            try
            {
                var index = arrayIndex;
                foreach (var item in (IEnumerable<bool>)this)
                {
                    array[index] = item;
                    index++;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException(exception.Message, nameof(array));
            }
        }

        public void CopyTo(bool[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (Capacity > array.Length)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }
            try
            {
                var index = 0;
                foreach (var item in (IEnumerable<bool>)this)
                {
                    array[index] = item;
                    index++;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException(exception.Message, nameof(array));
            }
        }

        public void CopyTo(bool[] array, int arrayIndex, int countLimit)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }
            if (countLimit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(countLimit), "Non-negative number is required.");
            }
            if (countLimit > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }
            var source = (this).Take(countLimit);
            try
            {
                var index = arrayIndex;
                foreach (var item in source)
                {
                    array[index] = item;
                    index++;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException(exception.Message, nameof(array));
            }
        }

        public IEnumerator<bool> GetEnumerator()
        {
            var index = 0;
            foreach (var entry in _entries)
            {
                foreach (var bit in BitsBinary(BinaryReverse(entry)))
                {
                    yield return bit == 1;
                    index++;
                    if (index == Capacity)
                    {
                        yield break;
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(bool item)
        {
            var currentIndex = 0;
            var comparer = EqualityComparer<bool>.Default;
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }

                    currentIndex++;
                }

                return -1;
            }
        }

        void IList<bool>.Insert(int index, bool item)
        {
            throw new NotSupportedException();
        }

        public FlagArray Not()
        {
            var result = new FlagArray(Capacity);
            for (var index = 0; index < _entries.Length; index++)
            {
                result._entries[index] = ~_entries[index];
            }
            var mask = GetMask(Capacity);
            if (mask != 0)
            {
                result._entries[result._entries.Length - 1] &= mask;
            }
            return result;
        }

        public FlagArray Or(FlagArray other)
        {
            var order = Capacity > other.Capacity;
            var b = order ? _entries : other._entries;
            var a = order ? other._entries : _entries;
            var result = new FlagArray(order ? Capacity : other.Capacity);
            for (var index = 0; index < a.Length; index++)
            {
                result._entries[index] = a[index] | b[index];
            }
            for (var index = a.Length; index < b.Length; index++)
            {
                result._entries[index] = b[index];
            }
            return result;
        }

        bool ICollection<bool>.Remove(bool item)
        {
            throw new NotSupportedException();
        }

        void IList<bool>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void SetAll(bool value)
        {
            var entryValue = value ? unchecked((int)0xffffffff) : 0;
            for (var index = 0; index < _entries.Length; index++)
            {
                _entries[index] = entryValue;
            }
            var mask = GetMask(Capacity);
            if (mask != 0)
            {
                _entries[_entries.Length - 1] &= mask;
            }
        }

        public FlagArray Xor(FlagArray other)
        {
            var order = Capacity > other.Capacity;
            var b = order ? _entries : other._entries;
            var a = order ? other._entries : _entries;
            var result = new FlagArray(order ? Capacity : other.Capacity);
            for (var index = 0; index < a.Length; index++)
            {
                result._entries[index] = a[index] ^ b[index];
            }
            for (var index = a.Length; index < b.Length; index++)
            {
                result._entries[index] = b[index];
            }
            return result;
        }

        private static int BinaryReverse(int value)
        {
            unchecked
            {
                return (int)BinaryReverse((uint)value);
            }
        }

        private static uint BinaryReverse(uint value)
        {
            value = ((value & 0xaaaaaaaa) >> 1) | ((value & 0x55555555) << 1);
            value = ((value & 0xcccccccc) >> 2) | ((value & 0x33333333) << 2);
            value = ((value & 0xf0f0f0f0) >> 4) | ((value & 0x0f0f0f0f) << 4);
            value = ((value & 0xff00ff00) >> 8) | ((value & 0x00ff00ff) << 8);
            return (value >> 16) | (value << 16);
        }

        private static IEnumerable<int> BitsBinary(int value)
        {
            unchecked
            {
                var check = (uint)1 << 31;
                var log2 = 32;
                var tmp = (uint)value;
                do
                {
                    if ((tmp & check) != 0)
                    {
                        yield return 1;
                    }
                    else
                    {
                        yield return 0;
                    }

                    check >>= 1;
                    log2--;
                } while (log2 > 0);
            }
        }

        private static int GetLength(int capacity)
        {
            return (capacity >> 5) + ((capacity & 31) == 0 ? 0 : 1);
        }

        private static int GetMask(int capacity)
        {
            return (1 << (capacity & 31)) - 1;
        }

        // Gem from Hacker's Delight
        // Returns the number of bits set in @value
        private static int PopulationCount(uint value)
        {
            value -= (value >> 1) & 0x55555555;
            value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
            value = (value + (value >> 4)) & 0x0F0F0F0F;
            value += value >> 8;
            value += value >> 16;
            return (int)(value & 0x0000003F);
        }

        private static int PopulationCount(int value)
        {
            unchecked
            {
                return PopulationCount((uint)value);
            }
        }

        private bool GetBit(int index, int mask)
        {
            return (Volatile.Read(ref _entries[index]) & mask) != 0;
        }

        private void SetBit(int index, int mask)
        {
            while (true)
            {
                var read = Volatile.Read(ref _entries[index]);
                if ((read & mask) != 0 || Interlocked.CompareExchange(ref _entries[index], read | mask, read) == read)
                {
                    return;
                }
            }
        }

        private void UnsetBit(int index, int mask)
        {
            while (true)
            {
                var read = Volatile.Read(ref _entries[index]);
                if ((read & mask) == 0 || Interlocked.CompareExchange(ref _entries[index], read & ~mask, read) == read)
                {
                    return;
                }
            }
        }
    }
}