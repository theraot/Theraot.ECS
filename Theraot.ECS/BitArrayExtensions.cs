using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.ECS
{
    public static class BitArrayExtensions
    {
        public static bool IsEmpty(this BitArray bitArray)
        {
            var integers = new int[(bitArray.Count >> 5) + 1];
            bitArray.CopyTo(integers, 0);
            return integers.All(integer => integer == 0);
        }

        public static bool IsProperSubsetOf(this BitArray bitArray, BitArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = bitArray;
            var b = other;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * b.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/###/\   \##|
             * |#|###|  |   |#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * a.And(b.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\   \  |
             * | |###|  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, it means a is a subset of b
             *
             * a.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/   /\###\##|
             * |#|   |  |###|#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * b.And(a.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\###\  |
             * | |   |  |###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If also this is not empty, it means a is a proper subset of b
             */
            return a.And(b.Not()).IsEmpty() && !b.And(a.Not()).IsEmpty();
        }

        public static bool IsProperSupersetOf(this BitArray bitArray, BitArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = bitArray;
            var b = other;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/   /\###\##|
             * |#|   |  |###|#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * b.And(a.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\###\  |
             * | |   |  |###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, it means a is superset of b
             *
             * b.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/###/\   \##|
             * |#|###|  |   |#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * a.And(b.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\   \  |
             * | |###|  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If also this is not empty, it means a is a proper superset of b
             */
            return b.And(a.Not()).IsEmpty() && !a.And(b.Not()).IsEmpty();
        }

        public static bool IsSubsetOf(this BitArray bitArray, BitArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = bitArray;
            var b = other;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * b.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/###/\   \##|
             * |#|###|  |   |#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * a.And(b.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\   \  |
             * | |###|  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, it means a is a subset of b
             */
            return a.And(b.Not()).IsEmpty();
        }

        public static bool IsSupersetOf(this BitArray bitArray, BitArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = bitArray;
            var b = other;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/   /\###\##|
             * |#|   |  |###|#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * b.And(a.Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\###\  |
             * | |   |  |###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, it means a is superset of b
             */
            return b.And(a.Not()).IsEmpty();
        }

        public static bool Overlaps(this BitArray bitArray, BitArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = bitArray;
            var b = other;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.And(b)
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\   \  |
             * | |   |##|   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is not empty, the sets overlap
             */
            return !a.And(b).IsEmpty();
        }

        public static bool Overlaps(this BitArray bitArray, IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return other.Any(index => bitArray[index]);
        }

        public static bool SetEquals(this BitArray bitArray, BitArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            var a = bitArray;
            var b = other;
            /*
             * +--------------+
             * |              |
             * | a ___  ___ b |
             * |  /   /\   \  |
             * | |   |  |   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.Or(b)
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\###\  |
             * | |###|##|###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.And(b)
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /   /\   \  |
             * | |   |##|   | |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             *
             * a.And(b).Not()
             * +--------------+
             * |##############|
             * |###___##___###|
             * |##/###/\###\##|
             * |#|###|  |###|#|
             * |##\___\/___/##|
             * |##############|
             * +--------------+
             *
             * a.Or(b).And(a.And(b).Not())
             * +--------------+
             * |              |
             * |   ___  ___   |
             * |  /###/\###\  |
             * | |###|  |###| |
             * |  \___\/___/  |
             * |              |
             * +--------------+
             * If this is empty, the sets are equal
             */
            return a.Or(b).And(a.And(b).Not()).IsEmpty();
        }
    }
}
