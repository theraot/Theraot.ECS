using System;

namespace Theraot
{
    internal static class EmptyArray<T>
    {
        internal static T[] Instance { get; } = Array.Empty<T>();
    }
}