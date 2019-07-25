using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal static class DictionaryExtensions
    {
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