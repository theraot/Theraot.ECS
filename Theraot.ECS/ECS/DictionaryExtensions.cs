using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal static class DictionaryExtensions
    {
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
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