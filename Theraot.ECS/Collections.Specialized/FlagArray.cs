﻿#pragma warning disable CA1710 // Los identificadores deben tener un sufijo correcto

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    public sealed partial class FlagArray : IList<bool>, ICloneable
    {
        private int[] _entries;

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

        public void Clear()
        {
            _entries = new int[_entries.Length];
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

    public sealed partial class FlagArray
    {
        public FlagArray And(FlagArray other)
        {
            return Build(Operation(Paired(this, other, PairMode.Shorter, out var length), And), length);
        }

        public bool IsSubsetOf(FlagArray other)
        {
            return IsEmpty(Operation(Paired(this, other, PairMode.Left, out _), Minus));
        }

        public bool IsSupersetOf(FlagArray other)
        {
            return IsEmpty(Operation(Paired(other, this, PairMode.Left, out _), Minus));
        }

        public FlagArray Minus(FlagArray other)
        {
            return Build(Operation(Paired(this, other, PairMode.Left, out var length), Minus), length);
        }

        public FlagArray Or(FlagArray other)
        {
            return Build(Operation(Paired(this, other, PairMode.Longer, out var length), Or), length);
        }

        public bool Overlaps(FlagArray other)
        {
            return !IsEmpty(Operation(Paired(this, other, PairMode.Shorter, out _), And));
        }

        public bool SetEquals(FlagArray other)
        {
            return IsEmpty(Operation(Paired(this, other, PairMode.Longer, out _), Xor));
        }

        public FlagArray Xor(FlagArray other)
        {
            return Build(Operation(Paired(this, other, PairMode.Longer, out var length), Xor), length);
        }

        private static int And(int left, int right)
        {
            return left & right;
        }

        private static int Minus(int left, int right)
        {
            return left & ~right;
        }

        private static int Or(int left, int right)
        {
            return left | right;
        }

        private static int Xor(int left, int right)
        {
            return left ^ right;
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

        private static FlagArray Build(IEnumerable<int> enumerable, int length)
        {
            var result = new FlagArray(length);
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

        private static IEnumerable<int> Operation(IEnumerable<Pair> paired, Func<int, int, int> operation)
        {
            foreach (var pair in paired)
            {
                yield return operation(pair.Left, pair.Right);
            }
        }

        private static IEnumerable<Pair> Paired(FlagArray left, FlagArray right, PairMode pairMode, out int length)
        {
            var settings = new IterationSettings(pairMode, left, right);
            return PairedExtracted(in settings, out length);
        }

        private static IEnumerable<Pair> PairedExtracted(in IterationSettings settings, out int length)
        {
            switch (settings.PairMode)
            {
                case PairMode.Longer:
                    length = settings.LongerLength;
                    return settings.Order == Ordering.LeftIsLonger
                        ? IteratorExtendedToMatchLeft(settings.LeftEntries, settings.RightEntries, settings.ShorterLength, length)
                        : IteratorExtendedToMatchRight(settings.LeftEntries, settings.RightEntries, settings.ShorterLength, length);

                case PairMode.Shorter:
                    length = settings.ShorterLength;
                    return Iterator(settings.LeftEntries, settings.RightEntries, length);

                case PairMode.Left:
                    length = settings.LeftEntries.Length;
                    return settings.Order == Ordering.LeftIsLonger
                        ? IteratorExtendedToMatchLeft(settings.LeftEntries, settings.RightEntries, settings.ShorterLength, length)
                        : Iterator(settings.LeftEntries, settings.RightEntries, length);

                case PairMode.Right:
                    length = settings.RightEntries.Length;
                    return settings.Order == Ordering.RightIsLonger
                        ? IteratorExtendedToMatchRight(settings.LeftEntries, settings.RightEntries, settings.ShorterLength, length)
                        : Iterator(settings.LeftEntries, settings.RightEntries, length);

                default:
                    length = 0;
                    return Array.Empty<Pair>();
            }
        }

        private readonly ref struct IterationSettings
        {
            public readonly int[] LeftEntries;

            public readonly int LongerLength;

            public readonly Ordering Order;

            public readonly PairMode PairMode;

            public readonly int[] RightEntries;

            public readonly int ShorterLength;

            public IterationSettings(PairMode pairMode, FlagArray left, FlagArray right)
            {
                PairMode = pairMode;
                Order = left.Capacity > right.Capacity ? Ordering.LeftIsLonger : Ordering.RightIsLonger;
                LeftEntries = left._entries;
                RightEntries = right._entries;
                LongerLength = Order == Ordering.LeftIsLonger ? LeftEntries.Length : RightEntries.Length;
                ShorterLength = Order == Ordering.RightIsLonger ? RightEntries.Length : LeftEntries.Length;
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
}