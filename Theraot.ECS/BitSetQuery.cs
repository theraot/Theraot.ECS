using ComponentType = System.Int32;

namespace Theraot.ECS
{
    public class BitSetQuery
    {
        public BitSet All { get; }

        public BitSet Any { get; }

        public BitSet None { get; }

        public BitSetQuery(int length, ComponentType[] all, ComponentType[] any, ComponentType[] none)
        {
            All = ToBitSet(all, length);
            Any = ToBitSet(any, length);
            None = ToBitSet(none, length);
        }

        private static BitSet ToBitSet(ComponentType[] collection, int length)
        {
            var result = new BitSet(length);
            result.UnionWith(collection);
            return result;
        }
    }
}
