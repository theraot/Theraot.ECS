using System.Collections;
using ComponentType = System.Int32;

namespace Theraot.ECS
{
    public class BitArrayQuery
    {
        public BitArray All { get; }

        public BitArray Any { get; }

        public BitArray None { get; }

        public BitArrayQuery(int length, ComponentType[] all, ComponentType[] any, ComponentType[] none)
        {
            All = ToBitArray(all, length);
            Any = ToBitArray(any, length);
            None = ToBitArray(none, length);
        }

        private static BitArray ToBitArray(ComponentType[] collection, int length)
        {
            var result = new BitArray(length);
            foreach (var index in collection)
            {
                result[index] = true;
            }
            return result;
        }
    }
}
