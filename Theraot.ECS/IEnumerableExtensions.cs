using System.Collections.Generic;

namespace Theraot.ECS
{
    internal static class IEnumerableExtensions
    {
        public static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
        {
            var @this = source is ISet<T> sourceSet ? sourceSet : new HashSet<T>(source);
            var that = other is ISet<T> otherSet ? otherSet : new HashSet<T>(other);
            var elementCount = 0;
            var matchCount = 0;
            foreach (var item in that)
            {
                elementCount++;
                if (@this.Contains(item))
                {
                    matchCount++;
                }
            }

            if (proper)
            {
                return matchCount == @this.Count && elementCount > @this.Count;
            }

            return matchCount == @this.Count;
        }

        public static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
        {
            var @this = source is ISet<T> sourceSet ? sourceSet : new HashSet<T>(source);
            var that = other is ISet<T> otherSet ? otherSet : new HashSet<T>(other);
            var elementCount = 0;
            foreach (var item in that)
            {
                elementCount++;
                if (!@this.Contains(item))
                {
                    return false;
                }
            }

            if (proper)
            {
                return elementCount < @this.Count;
            }

            return true;
        }
    }
}