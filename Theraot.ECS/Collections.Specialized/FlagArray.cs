#pragma warning disable CA1710 // Los identificadores deben tener un sufijo correcto
#pragma warning disable CC0031 // Check for null before calling a delegate

using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.Specialized
{
    /// <summary>
    /// Represents a fixed capacity collection of binary flags.
    /// </summary>
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11

    [Serializable]
#endif
    public sealed partial class FlagArray : IList<bool>
    {
        private const int _sizeOfEntry = 32;
        private const int _sizeOfEntryLog2 = 5;
        private readonly int[] _entries;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagArray"/> class, with the flags at the provided indexes set.
        /// </summary>
        /// <param name="indexes">The collection of indexes of the flags to set.</param>
        /// <exception cref="ArgumentNullException">The collection of indexes in null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The collection of indexes contains negatives.</exception>
        /// <remarks>The indexes are zero based. The capacity of the flag array will be the least needed to contain the flags of the provided indexes.</remarks>
        public FlagArray(IEnumerable<int> indexes)
        {
            if (indexes == null)
            {
                throw new ArgumentNullException(nameof(indexes), $"{nameof(indexes)} is null.");
            }
            var indexesList = new List<int>();
            var max = 0;
            foreach (var index in indexes)
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(indexes), "Negative index found");
                }
                if (index > max)
                {
                    max = index;
                }
                indexesList.Add(index);
            }

            var capacity = max + 1;
            var length = GetLength(capacity);
            _entries = new int[length];
            Capacity = capacity;
            foreach (var index in indexesList)
            {
                this[index] = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagArray"/> class, with the specified capacity and the flags at the provided indexes set.
        /// </summary>
        /// <param name="capacity">The capacity of the flag array.</param>
        /// <param name="indexes">The collection of indexes of the flags to set.</param>
        /// <exception cref="ArgumentNullException">The collection of indexes in null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The capacity is negative, or the collection of indexes contains negatives or or indexes outside of the capacity.</exception>
        /// <remarks>The indexes are zero based. For example, a capacity of 10 means that the valid indexes go from 0 to 9.</remarks>
        public FlagArray(int capacity, IEnumerable<int> indexes)
        {
            if (indexes == null)
            {
                throw new ArgumentNullException(nameof(indexes), $"{nameof(indexes)} is null.");
            }
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"{nameof(capacity)} < 0");
            }
            var indexesList = new List<int>();
            var max = 0;
            foreach (var index in indexes)
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(indexes), "Negative index found");
                }
                if (index >= capacity)
                {
                    throw new ArgumentOutOfRangeException(nameof(capacity));
                }
                if (index > max)
                {
                    max = index;
                }
                indexesList.Add(index);
            }

            var length = GetLength(capacity);
            _entries = new int[length];
            Capacity = capacity;
            foreach (var index in indexesList)
            {
                this[index] = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagArray"/> class, with the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity of the flag array.</param>
        /// <exception cref="ArgumentOutOfRangeException">The capacity is negative.</exception>
        public FlagArray(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"{nameof(capacity)} < 0");
            }
            var length = GetLength(capacity);
            _entries = new int[length];
            Capacity = capacity;
        }

        private FlagArray(FlagArray prototype)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype), $"{nameof(prototype)} is null.");
            }
            var length = prototype._entries.Length;
            _entries = new int[length];
            Capacity = prototype.Capacity;
            prototype._entries.CopyTo(_entries, 0);
        }

        /// <summary>
        /// Gets the capacity of this instance.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Gets the number of flags that are set in this instance.
        /// </summary>
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

        /// <summary>
        /// Get a collection of the indexes of the flags that are set on this instance.
        /// </summary>
        public IEnumerable<int> Flags
        {
            get
            {
                var bitIndex = 0;
                foreach (var entry in _entries)
                {
                    if (entry == 0)
                    {
                        bitIndex += _sizeOfEntry;
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

        /// <summary>
        /// Gets or sets a flag by the provided index.
        /// </summary>
        /// <param name="index">The index of the flag to get or set.</param>
        public bool this[int index]
        {
            get
            {
                if (index >= Capacity)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                var entryIndex = index >> _sizeOfEntryLog2;
                var bitIndex = index & (_sizeOfEntry - 1);
                var bitMask = 1 << bitIndex;
                return GetBit(entryIndex, bitMask);
            }
            set
            {
                if (index >= Capacity)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                var entryIndex = index >> _sizeOfEntryLog2;
                var bitIndex = index & (_sizeOfEntry - 1);
                var bitMask = 1 << bitIndex;
                if (value)
                {
                    SetBit(entryIndex, bitMask);
                }
                else
                {
                    UnsetBit(entryIndex, bitMask);
                }
            }
        }

        void ICollection<bool>.Add(bool item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Resets all flags.
        /// </summary>
        /// <remarks>All flags will be set to false.</remarks>
        public void Clear()
        {
            SetAll(false);
        }

        /// <summary>
        /// Returns a new <see cref="FlagArray"/> with the same flags sets as this instance.
        /// </summary>
        /// <returns></returns>
        public FlagArray Clone()
        {
            return new FlagArray(this);
        }

        /// <summary>
        /// Checks if this instance contains at least one flag that matches the provided item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>true if a flag matching the item was found; otherwise, false.</returns>
        public bool Contains(bool item)
        {
            var nextBitIndex = 0;
            var check = item ? 0 : -1;
            foreach (var entry in _entries)
            {
                nextBitIndex += _sizeOfEntry;
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

        /// <summary>
        /// Copies all the flags to an array of bool.
        /// </summary>
        /// <param name="array">The array to which to copy to.</param>
        /// <param name="arrayIndex">The starting index for the copy.</param>
        /// <exception cref="ArgumentNullException">When the provided array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When the starting index is negative.</exception>
        /// <exception cref="ArgumentException">When there is not enough space to copy all the flags.</exception>
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
                throw new ArgumentException(exception.Message, nameof(array), exception);
            }
        }

        /// <summary>
        /// Copies all the flags to an array of bool.
        /// </summary>
        /// <param name="array">The array to which to copy to.</param>
        /// <exception cref="ArgumentNullException">When the provided array is null.</exception>
        /// <exception cref="ArgumentException">When there is not enough space to copy all the flags.</exception>
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

        /// <summary>
        /// Copies a fraction of the flags to an array of bool.
        /// </summary>
        /// <param name="array">The array to which to copy to.</param>
        /// <param name="arrayIndex">The starting index for the copy.</param>
        /// <param name="countLimit">The number of flags to copy.</param>
        /// <exception cref="ArgumentNullException">When the provided array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When either the starting index or the number of flags to copy are negative.</exception>
        /// <exception cref="ArgumentException">When there is not enough space to copy the flags.</exception>
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
            var source = EnumerableHelper.Take(this, countLimit);
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

        /// <inheritdoc />
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

        /// <summary>
        /// Recovers the index of the first flag that matches the provided item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>the index of the found flag, if any; otherwise, -1.</returns>
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

        /// <summary>
        /// Returns a new <see cref="FlagArray"/> with all the flags negated.
        /// </summary>
        /// <remarks>The returned <see cref="FlagArray"/> is of the same capacity.</remarks>
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

        bool ICollection<bool>.Remove(bool item)
        {
            throw new NotSupportedException();
        }

        void IList<bool>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets all the flags to the specified value.
        /// </summary>
        /// <param name="value">The value to set all flags to.</param>
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
                var check = (uint)1 << (_sizeOfEntry - 1);
                var log2 = _sizeOfEntry;
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
            return (capacity >> _sizeOfEntryLog2) + ((capacity & (_sizeOfEntry - 1)) == 0 ? 0 : 1);
        }

        private static int GetMask(int capacity)
        {
            return (1 << (capacity & (_sizeOfEntry - 1))) - 1;
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
            return (Interlocked.CompareExchange(ref _entries[index], 0, 0) & mask) != 0;
        }

        private void SetBit(int index, int mask)
        {
            while (true)
            {
                var read = Interlocked.CompareExchange(ref _entries[index], 0, 0);
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
                var read = Interlocked.CompareExchange(ref _entries[index], 0, 0);
                if ((read & mask) == 0 || Interlocked.CompareExchange(ref _entries[index], read & ~mask, read) == read)
                {
                    return;
                }
            }
        }
    }

    public sealed partial class FlagArray
    {
        /// <summary>
        /// Returns a new <see cref="FlagArray"/> resulting from a bitwise and operation (set intersection).
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to operate with.</param>
        /// <remarks>The returned <see cref="FlagArray"/> is of the capacity of the shorter of the two.</remarks>
        public FlagArray And(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return Build(Operation(Paired(this, other, PairMode.Shorter, out var capacity), And), capacity);
        }

        /// <summary>
        /// Determines whether this instance is a proper subset of the specified <see cref="FlagArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to compare with.</param>
        public bool IsProperSubsetOf(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var equals = true;

            int Operate(Pair pair)
            {
                equals &= pair.Left == pair.Right;
                return pair.Left & ~pair.Right;
            }

            foreach (var entry in Operation(Paired(this, other, PairMode.Longer, out _), Operate))
            {
                if (entry == 0)
                {
                    continue;
                }

                return false;
            }

            return !equals;
        }

        /// <summary>
        /// Determines whether this instance is a proper superset of the specified <see cref="FlagArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to compare with.</param>
        public bool IsProperSupersetOf(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var equals = true;

            int Operate(Pair pair)
            {
                equals &= pair.Left == pair.Right;
                return pair.Left & ~pair.Right;
            }

            foreach (var entry in Operation(Paired(other, this, PairMode.Longer, out _), Operate))
            {
                if (entry == 0)
                {
                    continue;
                }

                return false;
            }

            return !equals;
        }

        /// <summary>
        /// Determines whether this instance is a subset of the specified <see cref="FlagArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to compare with.</param>
        public bool IsSubsetOf(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return IsEmpty(Operation(Paired(this, other, PairMode.Left, out _), Minus));
        }

        /// <summary>
        /// Determines whether this instance is a superset of the specified <see cref="FlagArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to compare with.</param>
        public bool IsSupersetOf(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return IsEmpty(Operation(Paired(other, this, PairMode.Left, out _), Minus));
        }

        /// <summary>
        /// Returns a new <see cref="FlagArray"/> with the set difference with another <see cref="FlagArray"/> (bitwise converse implication).
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to operate with.</param>
        /// <remarks>The returned <see cref="FlagArray"/> is of the capacity of this instance.</remarks>
        public FlagArray Minus(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return Build(Operation(Paired(this, other, PairMode.Left, out var capacity), Minus), capacity);
        }

        /// <summary>
        /// Returns a new <see cref="FlagArray"/> resulting from a bitwise or operation (set union).
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to operate with.</param>
        /// <remarks>The returned <see cref="FlagArray"/> is of the capacity of the longer of the two.</remarks>
        public FlagArray Or(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return Build(Operation(Paired(this, other, PairMode.Longer, out var capacity), Or), capacity);
        }

        /// <summary>
        /// Determines whether this instance overlaps the specified <see cref="FlagArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to compare with.</param>
        public bool Overlaps(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return !IsEmpty(Operation(Paired(this, other, PairMode.Shorter, out _), And));
        }

        /// <summary>
        /// Determines whether this instance is equivalent to the specified <see cref="FlagArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to compare with.</param>
        public bool SetEquals(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return IsEmpty(Operation(Paired(this, other, PairMode.Longer, out _), Xor));
        }

        /// <summary>
        /// Returns a new <see cref="FlagArray"/> resulting from a bitwise xor operation.
        /// </summary>
        /// <param name="other">The <see cref="FlagArray"/> to operate with.</param>
        /// <remarks>The returned <see cref="FlagArray"/> is of the capacity of the longer of the two.</remarks>
        public FlagArray Xor(FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return Build(Operation(Paired(this, other, PairMode.Longer, out var capacity), Xor), capacity);
        }

        private static int And(Pair pair)
        {
            return pair.Left & pair.Right;
        }

        private static int Minus(Pair pair)
        {
            return pair.Left & ~pair.Right;
        }

        private static int Or(Pair pair)
        {
            return pair.Left | pair.Right;
        }

        private static int Xor(Pair pair)
        {
            return pair.Left ^ pair.Right;
        }
    }

    public sealed partial class FlagArray
    {
        private enum Ordering
        {
            LeftIsLonger,
            RightIsLonger
        }

        private enum PairMode
        {
            Shorter,
            Longer,
            Left,
            Right
        }

        private static FlagArray Build(IEnumerable<int> enumerable, int capacity)
        {
            var result = new FlagArray(capacity);
            var index = 0;
            foreach (var entry in enumerable)
            {
                result._entries[index] = entry;
                index++;
            }

            return result;
        }

        private static bool IsEmpty(IEnumerable<int> enumerable)
        {
            var result = true;
            foreach (var entry in enumerable)
            {
                if (entry == 0)
                {
                    continue;
                }

                result = false;
                break;
            }

            return result;
        }

        private static IEnumerable<Pair> Iterator(int[] leftEntries, int[] rightEntries, int length)
        {
            for (var index = 0; index < length; index++)
            {
                yield return new Pair(leftEntries[index], rightEntries[index]);
            }
        }

        private static IEnumerable<Pair> IteratorExtendedToMatchLeft(int[] leftEntries, int[] rightEntries, int shorterLength, int longerLength)
        {
            for (var index = 0; index < shorterLength; index++)
            {
                yield return new Pair(leftEntries[index], rightEntries[index]);
            }
            for (var index = shorterLength; index < longerLength; index++)
            {
                yield return new Pair(leftEntries[index], 0);
            }
        }

        private static IEnumerable<Pair> IteratorExtendedToMatchRight(int[] leftEntries, int[] rightEntries, int shorterLength, int longerLength)
        {
            for (var index = 0; index < shorterLength; index++)
            {
                yield return new Pair(leftEntries[index], rightEntries[index]);
            }
            for (var index = shorterLength; index < longerLength; index++)
            {
                yield return new Pair(0, rightEntries[index]);
            }
        }

        private static IEnumerable<Pair> Paired(FlagArray left, FlagArray right, PairMode pairMode, out int capacity)
        {
            var settings = new IterationSettings(pairMode, left, right);
            return PairedExtracted(in settings, out capacity);
        }

        private static IEnumerable<Pair> PairedExtracted(in IterationSettings settings, out int capacity)
        {
            switch (settings.PairMode)
            {
                case PairMode.Longer:
                    capacity = settings.Longer.Capacity;
                    return settings.Order == Ordering.LeftIsLonger
                        ? IteratorExtendedToMatchLeft(settings.Left._entries, settings.Right._entries, settings.Shorter._entries.Length, settings.Longer._entries.Length)
                        : IteratorExtendedToMatchRight(settings.Left._entries, settings.Right._entries, settings.Shorter._entries.Length, settings.Longer._entries.Length);

                case PairMode.Shorter:
                    capacity = settings.Shorter.Capacity;
                    return Iterator(settings.Left._entries, settings.Right._entries, settings.Shorter._entries.Length);

                case PairMode.Left:
                    capacity = settings.Left.Capacity;
                    return settings.Order == Ordering.LeftIsLonger
                        ? IteratorExtendedToMatchLeft(settings.Left._entries, settings.Right._entries, settings.Shorter._entries.Length, settings.Left._entries.Length)
                        : Iterator(settings.Left._entries, settings.Right._entries, settings.Left._entries.Length);

                case PairMode.Right:
                    capacity = settings.Right.Capacity;
                    return settings.Order == Ordering.RightIsLonger
                        ? IteratorExtendedToMatchRight(settings.Left._entries, settings.Right._entries, settings.Shorter._entries.Length, settings.Right._entries.Length)
                        : Iterator(settings.Left._entries, settings.Right._entries, settings.Right._entries.Length);

                default:
                    capacity = 0;
                    return EmptyArray<Pair>.Instance;
            }
        }

        private readonly ref struct IterationSettings
        {
            public readonly FlagArray Left;

            public readonly FlagArray Longer;

            public readonly Ordering Order;

            public readonly PairMode PairMode;

            public readonly FlagArray Right;

            public readonly FlagArray Shorter;

            public IterationSettings(PairMode pairMode, FlagArray left, FlagArray right)
            {
                PairMode = pairMode;
                Order = left.Capacity > right.Capacity ? Ordering.LeftIsLonger : Ordering.RightIsLonger;
                Left = left;
                Right = right;
                Longer = Order == Ordering.LeftIsLonger ? Left : Right;
                Shorter = Order == Ordering.LeftIsLonger ? Right : Left;
            }
        }

        private readonly struct Pair
        {
            public readonly int Left;

            public readonly int Right;

            public Pair(int left, int right)
            {
                Left = left;
                Right = right;
            }
        }
    }

#if LESSTHAN_NET35

    public sealed partial class FlagArray
    {
        private static IEnumerable<int> Operation(IEnumerable<Pair> paired, Converter<Pair, int> operation)
        {
            foreach (var pair in paired)
            {
                yield return operation(pair);
            }
        }
    }

#else

    public sealed partial class FlagArray
    {
        private static IEnumerable<int> Operation(IEnumerable<Pair> paired, Func<Pair, int> operation)
        {
            foreach (var pair in paired)
            {
                yield return operation(pair);
            }
        }
    }

#endif

    public sealed partial class FlagArray : IEquatable<FlagArray>
    {
        public static bool operator !=(FlagArray left, FlagArray right)
        {
            return !(left == right);
        }

        public static bool operator ==(FlagArray x, FlagArray y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.SetEquals(y);
        }

        /// <inheritdoc />
        public bool Equals(FlagArray other)
        {
            return SetEquals(other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as FlagArray);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                foreach (var element in _entries)
                {
                    hash = (hash * 31) + element;
                }
                return hash;
            }
        }
    }

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

    public sealed partial class FlagArray : ICloneable
    {
        object ICloneable.Clone()
        {
            return Clone();
        }
    }

#endif
}