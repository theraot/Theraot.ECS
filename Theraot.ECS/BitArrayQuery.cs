using System.Collections;
using System.Collections.Generic;

namespace Theraot.ECS
{
    public sealed class BitArrayQuery
    {
        public BitArrayQuery(int length, IEnumerable<int> all, IEnumerable<int> any, IEnumerable<int> none)
        {
            All = ToBitArray(all, length);
            Any = ToBitArray(any, length);
            None = ToBitArray(none, length);
        }

        public BitArray All { get; }

        public BitArray Any { get; }

        public BitArray None { get; }

        private static BitArray ToBitArray(IEnumerable<int> collection, int length)
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
