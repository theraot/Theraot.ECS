using System.Collections.Generic;

namespace Theraot.ECS
{
    internal static class ISetExtensions
    {
        public static bool ContainsAll<T>(this ISet<T> hashSet, IEnumerable<T> items)
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

        public static bool ContainsAny<T>(this ISet<T> hashSet, IEnumerable<T> items)
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
