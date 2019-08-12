using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal static class SetCheck
    {
        public static bool IsSubsetOf<T>(Func<T, bool> contains, int count, IEnumerable<T> other, bool proper)
        {
            var elementCount = 0;
            var matchCount = 0;
            foreach (var item in other)
            {
                elementCount++;
                if (contains(item))
                {
                    matchCount++;
                }
            }

            if (proper)
            {
                return matchCount == count && elementCount > count;
            }

            return matchCount == count;
        }

        public static bool IsSupersetOf<T>(Func<T, bool> contains, int count, IEnumerable<T> other, bool proper)
        {
            var elementCount = 0;
            foreach (var item in other)
            {
                elementCount++;
                if (!contains(item))
                {
                    return false;
                }
            }

            if (proper)
            {
                return elementCount < count;
            }

            return true;
        }
    }
}