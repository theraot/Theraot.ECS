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

            var a1 = (BitArray)bitArray.Clone();
            var b1 = (BitArray)other.Clone();
            var a2 = (BitArray)bitArray.Clone();
            var b2 = (BitArray)other.Clone();
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
            return a1.And(b1.Not()).IsEmpty() && !b2.And(a2.Not()).IsEmpty();
        }

        public static bool IsProperSupersetOf(this BitArray bitArray, BitArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a1 = (BitArray)bitArray.Clone();
            var b1 = (BitArray)other.Clone();
            var a2 = (BitArray)bitArray.Clone();
            var b2 = (BitArray)other.Clone();
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
            return b1.And(a1.Not()).IsEmpty() && !a2.And(b2.Not()).IsEmpty();
        }

        public static bool IsSubsetOf(this BitArray bitArray, BitArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = (BitArray)bitArray.Clone();
            var b = (BitArray)other.Clone();
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

            var a = (BitArray)bitArray.Clone();
            var b = (BitArray)other.Clone();
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

            var a = (BitArray)bitArray.Clone();
            var b = (BitArray)other.Clone();
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

            var a1 = (BitArray)bitArray.Clone();
            var b1 = (BitArray)other.Clone();
            var a2 = (BitArray)bitArray.Clone();
            var b2 = (BitArray)other.Clone();
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
            return a1.Or(b1).And(a2.And(b2).Not()).IsEmpty();
        }
    }
}
