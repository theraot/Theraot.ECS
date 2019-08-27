using System;
using System.Collections.Generic;

namespace Theraot
{
    internal static class EnumerableHelper
    {
        public static bool Any<T>(IEnumerable<T> source, Predicate<T> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return true;
                }
            }

            return false;
        }

        public static ICollection<T> AsICollection<T>(IEnumerable<T> enumerable)
        {
            switch (enumerable)
            {
                case ICollection<T> allCollection:
                    return allCollection;

                default:
                    return ToIList(enumerable);
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
                    return ToIList(enumerable);
            }
        }

        public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] sources)
        {
            foreach (var enumerable in sources)
            {
                foreach (var item in enumerable)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> Take<T>(IEnumerable<T> source, int count)
        {
            foreach (var item in source)
            {
                yield return item;
                if (--count == 0)
                {
                    break;
                }
            }
        }

        private static IList<T> ToIList<T>(IEnumerable<T> source)
        {
            var result = source is ICollection<T> sourceAsCollection ? new List<T>(sourceAsCollection.Count) : new List<T>();
            foreach (var item in source)
            {
                result.Add(item);
            }

            return result;
        }
    }
}