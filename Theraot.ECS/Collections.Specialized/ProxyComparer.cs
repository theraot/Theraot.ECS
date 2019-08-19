#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    internal sealed class ProxyComparer<T> : IComparer<T>
    {
        private readonly IEqualityComparer<T> _equalityComparer;

        public ProxyComparer(IEqualityComparer<T> equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public int Compare(T x, T y)
        {
            var left = x == null ? 0 : _equalityComparer.GetHashCode(x);
            var right = y == null ? 0 : _equalityComparer.GetHashCode(y);
            return left.CompareTo(right);
        }
    }
}