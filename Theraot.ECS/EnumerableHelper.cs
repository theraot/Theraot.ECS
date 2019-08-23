using System.Collections.Generic;
using System.Linq;

namespace Theraot
{
    internal static class EnumerableHelper
    {
        public static ICollection<T> AsICollection<T>(IEnumerable<T> enumerable)
        {
            switch (enumerable)
            {
                case ICollection<T> allCollection:
                    return allCollection;

                default:
                    return enumerable.ToList();
            }
        }

        public static IList<T> AsIList<T>(IEnumerable<T> enumerable)
        {
            switch (enumerable)
            {
                case IList<T> allCollection:
                    return allCollection;

                case ICollection<T> collection when collection.Count == 0:
                    return EmptyArray<T>.Instance;

                case ICollection<T> collection:
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return result;

                default:
                    return enumerable.ToList();
            }
        }
    }
}