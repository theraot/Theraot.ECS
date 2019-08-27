namespace Theraot
{
    internal static class EmptyArray<T>
    {
        internal static T[] Instance { get; }
#if LESSTHAN_NET46
            = new T[0];

#else
            = System.Array.Empty<T>();

#endif
    }
}