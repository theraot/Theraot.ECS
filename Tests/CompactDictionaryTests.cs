using NUnit.Framework;
using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Tests
{
    public static class CompactDictionaryTests
    {
        [Test]
        public static void AddingBeyondCapacityGrows()
        {
            var dict = new CompactDictionary<string, int>(null, 4)
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
                {"d", 4}
            };
            Assert.AreEqual(4, dict.Capacity);
            dict.Add("e", 5);
            Assert.GreaterOrEqual(dict.Capacity, 5);
        }

        [Test]
        public static void FindValue()
        {
            var dict = new CompactDictionary<string, int>(null, 16)
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
                {"d", 4}
            };
            Assert.AreEqual(true, dict.ContainsValue(3));
            Assert.AreEqual(false, dict.ContainsValue(5));
        }

        [Test]
        public static void RemoveAll()
        {
            var dict = new CompactDictionary<string, int>(null, 16)
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
                {"d", 4}
            };
            var result1 = dict.RemoveAll(new[] { "a", "c", "e" }, out var removedKeys1, out var removedValues1);
            Assert.AreEqual(2, result1);
            Assert.AreEqual(false, dict.ContainsValue(1));
            Assert.AreEqual(true, dict.ContainsValue(2));
            Assert.AreEqual(false, dict.ContainsValue(3));
            Assert.AreEqual(true, dict.ContainsValue(4));
            Assert.AreEqual(false, dict.ContainsKey("a"));
            Assert.AreEqual(true, dict.ContainsKey("b"));
            Assert.AreEqual(false, dict.ContainsKey("c"));
            Assert.AreEqual(true, dict.ContainsKey("d"));
            Assert.AreEqual(new[] { "a", "c" }, removedKeys1);
            Assert.AreEqual(new[] { 1, 3 }, removedValues1);
            var result2 = dict.RemoveAll(new[] { "a", "c", "e" }, out var removedKeys2, out var removedValues2);
            Assert.AreEqual(0, result2);
            Assert.AreEqual(Array.Empty<string>(), removedKeys2);
            Assert.AreEqual(Array.Empty<int>(), removedValues2);
            var result3 = dict.RemoveAll(new[] { "b" }, out var removedKeys3, out var removedValues3);
            Assert.AreEqual(1, result3);
            Assert.AreEqual(false, dict.ContainsValue(2));
            Assert.AreEqual(false, dict.ContainsKey("b"));
            Assert.AreEqual(new[] { "b" }, removedKeys3);
            Assert.AreEqual(new[] { 2 }, removedValues3);
        }

        [Test]
        public static void SetAllReturnsTheNewKeys()
        {
            var dict = new CompactDictionary<string, int>(null, 16)
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
                {"d", 4}
            };
            var result1 = dict.SetAll(new[] { "a", "b", "e" }, new[] { 42, 84, 5 });
            Assert.AreEqual(new[] { "e" }, result1);
            Assert.AreEqual(42, dict["a"]);
            Assert.AreEqual(84, dict["b"]);
            Assert.AreEqual(5, dict["e"]);
            var sourceA = new Dictionary<string, int>
            {
                {"e", 5},
                {"f", 6},
                {"g", 7}
            };
            var result2 = dict.SetAll(sourceA);
            Assert.AreEqual(new[] { "f", "g" }, result2);
            Assert.AreEqual(5, dict["e"]);
            Assert.AreEqual(6, dict["f"]);
            Assert.AreEqual(7, dict["g"]);
            var sourceB = new Dictionary<string, int>
            {
                {"g", 7},
                {"h", 8},
                {"i", 9}
            };
            var result3 = dict.SetAll(sourceB.Keys, key => sourceB[key], pair => pair.Value * 2);
            Assert.AreEqual(new[] { "h", "i" }, result3);
            Assert.AreEqual(14, dict["g"]);
            Assert.AreEqual(8, dict["h"]);
            Assert.AreEqual(9, dict["i"]);
        }

        [Test]
        public static void SetReturnTrueOnNewItems()
        {
            var dict = new CompactDictionary<string, int>(null, 16)
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
                {"d", 4}
            };
            Assert.AreEqual(1, dict["a"]);
            dict["a"] = 42;
            Assert.AreEqual(42, dict["a"]);
            dict["e"] = 42;
            Assert.AreEqual(false, dict.Set("e", 5));
            Assert.AreEqual(5, dict["e"]);
            Assert.AreEqual(true, dict.Set("f", 6));
            Assert.AreEqual(6, dict["f"]);
        }

        [Test]
        public static void Trim()
        {
            var dict = new CompactDictionary<string, int>(null, 16)
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
                {"d", 4}
            };
            Assert.AreEqual(16, dict.Capacity);
            dict.TrimToSize();
            Assert.AreEqual(4, dict.Capacity);
        }

        [Test]
        public static void TryAdd()
        {
            var dict = new CompactDictionary<string, int>(null, 16)
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
                {"d", 4}
            };
            Assert.AreEqual(false, dict.TryAdd("a", 0));
            Assert.AreEqual(true, dict.TryAdd("e", 5));
            Assert.AreEqual(false, dict.TryAdd("e", 5));
        }

        [Test]
        public static void UnableToReduceCapacityBelowCount()
        {
            var dict = new CompactDictionary<string, int>(null, 16)
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
                {"d", 4}
            };
            Assert.Throws<ArgumentOutOfRangeException>(() => dict.Capacity = 2);
            Assert.AreEqual(16, dict.Capacity);
            dict.Capacity = 5;
            Assert.AreEqual(5, dict.Capacity);
        }
    }
}