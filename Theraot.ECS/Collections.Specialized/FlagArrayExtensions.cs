using System;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    public static class FlagArrayExtensions
    {
        public static bool IsEmpty(this FlagArray flagArray)
        {
            if (flagArray == null)
            {
                throw new ArgumentNullException(nameof(flagArray));
            }
            return !flagArray.Contains(true);
        }

        public static bool Overlaps(this FlagArray flagArray, IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return other.Any(index => flagArray[index]);
        }
    }
}