using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal static class DictionaryExtensions
    {
        public static List<TKey> RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var result = new List<TKey>();
            foreach (var key in source)
            {
                if (dictionary.Remove(key))
                {
                    result.Add(key);
                }
            }

            return result;
        }

        public static bool Set<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            var isNew = !dictionary.ContainsKey(key);
            dictionary[key] = value;
            return isNew;
        }

        public static Dictionary<TKey, TValue> SetAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var result = new Dictionary<TKey, TValue>();
            foreach (var pair in source)
            {
                var key = pair.Key;
                var value = pair.Value;
                if (dictionary.Set(key, value))
                {
                    result.Add(key, value);
                }
            }

            return result;
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary is ConcurrentDictionary<TKey, TValue> concurrentDictionary)
            {
                return concurrentDictionary.TryAdd(key, value);
            }
            // TODO: Dictionary<TKey, TValue>.TryAdd exists in .NET Core 2.0 or newer and .NET Standard 2.1 or newer (presumably)
            try
            {
                dictionary.Add(key, value);
                return true;
            }
            catch (ArgumentOutOfRangeException e)
            {
                var _ = e;
                return false;
            }
        }
    }
}