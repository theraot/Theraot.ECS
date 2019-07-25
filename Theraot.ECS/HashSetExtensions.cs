using System.Collections.Generic;

namespace Theraot.ECS
{
    internal static class HashSetExtensions
    {
        public static IEnumerable<T> AddAll<T>(this HashSet<object> hashSet, T[] items)
        {
            foreach (var item in items)
            {
                if (hashSet.Add(items))
                {
                    yield return item;
                }
            }
        }

        public static bool ContainsAll<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (!hashSet.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainsAny<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (hashSet.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
