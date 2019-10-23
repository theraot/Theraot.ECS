// ReSharper disable AssignmentIsFullyDiscarded

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections.Specialized;

namespace Tests
{
    public static class FlagArrayTests
    {
        [Test]
        public static void And()
        {
            var a = new FlagArray(6)
            { [0] = false, [1] = false, [2] = true, [3] = true, [4] = false, [5] = true };
            var b = new FlagArray(4)
            { [0] = false, [1] = true, [2] = false, [3] = true };
            var c = a.And(b);
            Assert.AreEqual(false, c[0]);
            Assert.AreEqual(false, c[1]);
            Assert.AreEqual(false, c[2]);
            Assert.AreEqual(true, c[3]);
            Assert.AreEqual(4, c.Capacity);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = c[c.Capacity]);
        }

        [Test]
        public static void Clone()
        {
            var x = new FlagArray(16)
            {
                [5] = true,
                [9] = true
            };
            var y = x.Clone();
            Assert.AreEqual(16, y.Capacity);
            Assert.AreEqual(true, y[5]);
            Assert.AreEqual(true, y[9]);
        }

        [Test]
        public static void CopyTo()
        {
            var x = new FlagArray(new[] { 5, 9, 15 });
            var bits1 = new bool[x.Capacity];
            x.CopyTo(bits1);
            var test1 = new bool[x.Capacity];
            test1[5] = true;
            test1[9] = true;
            test1[15] = true;
            Assert.AreEqual(test1, bits1);
            var bits2 = new bool[15];
            x.CopyTo(bits2, 5, 10);
            var test2 = new bool[15];
            test2[10] = true;
            test2[14] = true;
            Assert.AreEqual(test2, bits2);
        }

        [Test]
        public static void EnumerableConstructor()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new FlagArray(new[] { -1 }));
            var a = new FlagArray(new[] { 2, 3, 5 });
            Assert.AreEqual(false, a[0]);
            Assert.AreEqual(false, a[1]);
            Assert.AreEqual(true, a[2]);
            Assert.AreEqual(true, a[3]);
            Assert.AreEqual(false, a[4]);
            Assert.AreEqual(true, a[5]);
            Assert.AreEqual(6, a.Capacity);

            var b = new FlagArray(new[] { 1, 3 });
            Assert.AreEqual(false, b[0]);
            Assert.AreEqual(true, b[1]);
            Assert.AreEqual(false, b[2]);
            Assert.AreEqual(true, b[3]);
            Assert.AreEqual(4, b.Capacity);
        }

        [Test]
        public static void EnumerableConstructorWithCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new FlagArray(8, new[] { -1 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new FlagArray(8, new[] { 9 }));

            var a = new FlagArray(8, new[] { 2, 3, 5 });
            Assert.AreEqual(false, a[0]);
            Assert.AreEqual(false, a[1]);
            Assert.AreEqual(true, a[2]);
            Assert.AreEqual(true, a[3]);
            Assert.AreEqual(false, a[4]);
            Assert.AreEqual(true, a[5]);
            Assert.AreEqual(false, a[6]);
            Assert.AreEqual(false, a[7]);
            Assert.AreEqual(8, a.Capacity);

            var b = new FlagArray(8, new[] { 1, 3 });
            Assert.AreEqual(false, b[0]);
            Assert.AreEqual(true, b[1]);
            Assert.AreEqual(false, b[2]);
            Assert.AreEqual(true, b[3]);
            Assert.AreEqual(false, b[4]);
            Assert.AreEqual(false, b[5]);
            Assert.AreEqual(false, b[6]);
            Assert.AreEqual(false, b[7]);
            Assert.AreEqual(8, b.Capacity);
        }

        [Test]
        public static void Flags()
        {
            var x = new FlagArray(new[] { 5, 9 });
            Assert.AreEqual(new[] { 5, 9 }, x.Flags);
        }

        [Test]
        public static void IsProperSubsetOf()
        {
            BuildSetsAndFlagArrays(out var sets, out var flagArrays);

            for (var indexFirst = 0; indexFirst < sets.Count; indexFirst++)
            {
                for (var indexSecond = 0; indexSecond < sets.Count; indexSecond++)
                {
                    Assert.AreEqual(
                        sets[indexFirst].IsProperSubsetOf(sets[indexSecond]),
                        flagArrays[indexFirst].IsProperSubsetOf(flagArrays[indexSecond])
                    );
                }
            }
        }

        [Test]
        public static void IsProperSupersetOf()
        {
            BuildSetsAndFlagArrays(out var sets, out var flagArrays);

            for (var indexFirst = 0; indexFirst < sets.Count; indexFirst++)
            {
                for (var indexSecond = 0; indexSecond < sets.Count; indexSecond++)
                {
                    Assert.AreEqual(
                        sets[indexFirst].IsProperSupersetOf(sets[indexSecond]),
                        flagArrays[indexFirst].IsProperSupersetOf(flagArrays[indexSecond])
                    );
                }
            }
        }

        [Test]
        public static void IsSubsetOf()
        {
            BuildSetsAndFlagArrays(out var sets, out var flagArrays);

            for (var indexFirst = 0; indexFirst < sets.Count; indexFirst++)
            {
                for (var indexSecond = 0; indexSecond < sets.Count; indexSecond++)
                {
                    Assert.AreEqual(
                        sets[indexFirst].IsSubsetOf(sets[indexSecond]),
                        flagArrays[indexFirst].IsSubsetOf(flagArrays[indexSecond])
                    );
                }
            }
        }

        [Test]
        public static void IsSupersetOf()
        {
            BuildSetsAndFlagArrays(out var sets, out var flagArrays);

            for (var indexFirst = 0; indexFirst < sets.Count; indexFirst++)
            {
                for (var indexSecond = 0; indexSecond < sets.Count; indexSecond++)
                {
                    Assert.AreEqual(
                        sets[indexFirst].IsSupersetOf(sets[indexSecond]),
                        flagArrays[indexFirst].IsSupersetOf(flagArrays[indexSecond])
                    );
                }
            }
        }

        [Test]
        public static void MinusLonger()
        {
            var a = new FlagArray(4)
            { [0] = false, [1] = true, [2] = false, [3] = true };
            var b = new FlagArray(6)
            { [0] = false, [1] = false, [2] = true, [3] = true, [4] = false, [5] = true };
            var c = a.Minus(b);
            Assert.AreEqual(false, c[0]);
            Assert.AreEqual(true, c[1]);
            Assert.AreEqual(false, c[2]);
            Assert.AreEqual(false, c[3]);
            Assert.AreEqual(4, c.Capacity);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = c[c.Capacity]);
        }

        [Test]
        public static void MinusShorter()
        {
            var a = new FlagArray(6)
            { [0] = false, [1] = false, [2] = true, [3] = true, [4] = false, [5] = true };
            var b = new FlagArray(4)
            { [0] = false, [1] = true, [2] = false, [3] = true };
            var c = a.Minus(b);
            Assert.AreEqual(false, c[0]);
            Assert.AreEqual(false, c[1]);
            Assert.AreEqual(true, c[2]);
            Assert.AreEqual(false, c[3]);
            Assert.AreEqual(false, c[4]);
            Assert.AreEqual(true, c[5]);
            Assert.AreEqual(6, c.Capacity);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = c[c.Capacity]);
        }

        [Test]
        public static void Not()
        {
            var b = new FlagArray(4)
            { [0] = false, [1] = true, [2] = false, [3] = true };
            var c = b.Not();
            Assert.AreEqual(true, c[0]);
            Assert.AreEqual(false, c[1]);
            Assert.AreEqual(true, c[2]);
            Assert.AreEqual(false, c[3]);
            Assert.AreEqual(4, c.Capacity);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = c[c.Capacity]);
        }

        [Test]
        public static void Or()
        {
            var a = new FlagArray(6)
            { [0] = false, [1] = false, [2] = true, [3] = true, [4] = false, [5] = true };
            var b = new FlagArray(4)
            { [0] = false, [1] = true, [2] = false, [3] = true };
            var c = a.Or(b);
            Assert.AreEqual(false, c[0]);
            Assert.AreEqual(true, c[1]);
            Assert.AreEqual(true, c[2]);
            Assert.AreEqual(true, c[3]);
            Assert.AreEqual(false, c[4]);
            Assert.AreEqual(true, c[5]);
            Assert.AreEqual(6, c.Capacity);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = c[c.Capacity]);
        }

        [Test]
        public static void Overlaps()
        {
            BuildSetsAndFlagArrays(out var sets, out var flagArrays);

            for (var indexFirst = 0; indexFirst < sets.Count; indexFirst++)
            {
                for (var indexSecond = 0; indexSecond < sets.Count; indexSecond++)
                {
                    Assert.AreEqual(
                        sets[indexFirst].Overlaps(sets[indexSecond]),
                        flagArrays[indexFirst].Overlaps(flagArrays[indexSecond])
                    );
                }
            }
        }

        [Test]
        public static void SetAllAndContains()
        {
            var x = new FlagArray(new[] { 5, 9 });
            Assert.AreEqual(true, x[5]);
            Assert.AreEqual(true, x[9]);
            Assert.AreEqual(2, x.Count);
            Assert.AreEqual(true, x.Contains(true));
            Assert.AreEqual(true, x.Contains(false));
            x.SetAll(false);
            Assert.AreEqual(false, x.Contains(true));
            Assert.AreEqual(true, x.Contains(false));
            Assert.AreEqual(false, x[5]);
            Assert.AreEqual(false, x[9]);
            Assert.AreEqual(0, x.Count);
            x.SetAll(true);
            Assert.AreEqual(true, x.Contains(true));
            Assert.AreEqual(false, x.Contains(false));
            Assert.AreEqual(x.Capacity, x.Count);
        }

        [Test]
        public static void SetEquals()
        {
            BuildSetsAndFlagArrays(out var sets, out var flagArrays);

            for (var indexFirst = 0; indexFirst < sets.Count; indexFirst++)
            {
                for (var indexSecond = 0; indexSecond < sets.Count; indexSecond++)
                {
                    Assert.AreEqual(
                        sets[indexFirst].SetEquals(sets[indexSecond]),
                        flagArrays[indexFirst].SetEquals(flagArrays[indexSecond])
                    );
                }
            }
        }

        [Test]
        public static void Xor()
        {
            var a = new FlagArray(6)
            { [0] = false, [1] = false, [2] = true, [3] = true, [4] = false, [5] = true };
            var b = new FlagArray(4)
            { [0] = false, [1] = true, [2] = false, [3] = true };
            var c = a.Xor(b);
            Assert.AreEqual(false, c[0]);
            Assert.AreEqual(true, c[1]);
            Assert.AreEqual(true, c[2]);
            Assert.AreEqual(false, c[3]);
            Assert.AreEqual(false, c[4]);
            Assert.AreEqual(true, c[5]);
            Assert.AreEqual(6, c.Capacity);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = c[c.Capacity]);
        }

        private static void BuildSetsAndFlagArrays(out List<HashSet<int>> sets, out List<FlagArray> flagArrays)
        {
            var arrays = new[]
                        {
                new[] { 2, 3, 5 },
                new[] { 1, 3 },
                Array.Empty<int>(),
                Array.Empty<int>(),
                new [] { 1, 2, 3, 4, 5 },
                new [] { 0, 1, 2, 3 },
                new [] { 0, 1, 2, 3 },
                new [] { 1, 3, 120 }
            };
            var capacities = new[] { 6, 4, 6, 4, 6, 4, 128, 128 };
            sets = (from array in arrays select new HashSet<int>(array)).ToList();
            flagArrays = arrays.Select((t, index) => new FlagArray(capacities[index], t)).ToList();
        }
    }
}