using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal static class DictionaryExtensions
    {
        public static bool Set<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            var isNew = dictionary.ContainsKey(key);
            dictionary[key] = value;
            return isNew;
        }

        public static IEnumerable<TValue> SetAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue[] values, Func<TValue, TKey> keySelector)
        {
            foreach (var value in values)
            {
                if (dictionary.Set(keySelector(value), value))
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<TValue> SetAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue[] values, TKey[] keys)
        {
            for (var index = 0; index < values.Length; index++)
            {
                var value = values[index];
                var key = keys[index];
                if (dictionary.Set(key, value))
                {
                    yield return value;
                }
            }
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