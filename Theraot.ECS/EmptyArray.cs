using System;

namespace Theraot.ECS
{
    internal static class EmptyArray<T>
    {
        internal static T[] Instance { get; } = Array.Empty<T>();
    }
}