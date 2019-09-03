using NUnit.Framework;
using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Tests
{
    public static class IndexedCollectionTests
    {
        [Test]
        public static void Adding()
        {
            var x = new IntKeyCollection<string>(16)
            {
                "a",
                "b",
                "c",
                "d"
            };
            Assert.AreEqual(4, x.Count);
            Assert.AreEqual(4, x.Add("a"));
            Assert.AreEqual(5, x.Count);
        }

        [Test]
        public static void AddRange()
        {
            var collection = new IntKeyCollection<string>(16);
            Assert.AreEqual(16, collection.Capacity);
            var abc = new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            var result = collection.AddRange(abc);
            Assert.GreaterOrEqual(collection.Capacity, abc.Length);
            var ind = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            Assert.AreEqual(ind, result);
        }

        [Test]
        public static void Contains()
        {
            var x = new IntKeyCollection<string>(16)
            {
                "a",
                "b",
                "c",
                "d"
            };
            Assert.AreEqual(true, x.Contains("a"));
            Assert.AreEqual(false, x.Contains("e"));
            Assert.AreEqual(true, x.ContainsKey(0));
            Assert.AreEqual(false, x.ContainsKey(4));
        }

        [Test]
        public static void IndexedAccess()
        {
            var x = new IntKeyCollection<string>(16)
            {
                "a",
                "b",
                "c",
                "d"
            };
            Assert.AreEqual("a", x[0]);
            Assert.AreEqual("b", x[1]);
            Assert.AreEqual("c", x[2]);
            Assert.AreEqual("d", x[3]);
            // ReSharper disable once AssignmentIsFullyDiscarded
            Assert.Throws<KeyNotFoundException>(() => _ = x[4]);
        }

        [Test]
        public static void RemoveAll()
        {
            var x = new IntKeyCollection<string>(16)
            {
                "a",
                "b",
                "c",
                "d"
            };
            var result = x.RemoveAll(new[] { 0, 2, 4 });
            Assert.AreEqual(new[] { 0, 2 }, result);
        }

        [Test]
        public static void Trim()
        {
            var x = new IntKeyCollection<string>(16)
            {
                "a",
                "b",
                "c",
                "d"
            };
            Assert.AreEqual(16, x.Capacity);
            x.TrimToSize();
            Assert.AreEqual(4, x.Capacity);
        }

        [Test]
        public static void UnableToReduceCapacityBelowCount()
        {
            var x = new IntKeyCollection<string>(16)
            {
                "a",
                "b",
                "c",
                "d"
            };
            Assert.Throws<ArgumentOutOfRangeException>(() => x.Capacity = 2);
            Assert.AreEqual(16, x.Capacity);
            x.Capacity = 5;
            Assert.AreEqual(5, x.Capacity);
        }
    }
}