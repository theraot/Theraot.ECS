using System.Collections.Generic;
using System.Linq;

namespace Theraot.ECS
{
    internal static class EnumerableHelper
    {
        public static ICollection<T> AsICollection<T>(IEnumerable<T> enumerable)
        {
            return enumerable is ICollection<T> allCollection ? allCollection : enumerable.ToList();
        }
    }
}