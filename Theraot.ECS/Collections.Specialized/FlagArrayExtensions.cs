using System;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections.Specialized
{
    public static class FlagArrayExtensions
    {
        public static bool IsEmpty(this FlagArray flagArray)
        {
            return !flagArray.Contains(true);
        }

        public static bool IsProperSubsetOf(this FlagArray flagArray, FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = flagArray;
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
             * a.And(b.Not()) // a.Minus(b)
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
             * b.And(a.Not()) // b.Minus(a)
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
            return a.IsSubsetOf(b) && !b.IsSubsetOf(a);
        }

        public static bool IsProperSupersetOf(this FlagArray flagArray, FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = flagArray;
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
             * b.And(a.Not()) // b.Minus(a)
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
             * a.And(b.Not()) // a.Minus(b)
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
            return b.IsSubsetOf(a) && !a.IsSubsetOf(b);
        }

        public static bool IsSupersetOf(this FlagArray flagArray, FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = flagArray;
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
             * b.And(a.Not()) // b.Minus(a)
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
            return b.IsSubsetOf(a);
        }

        public static bool Overlaps(this FlagArray flagArray, FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = flagArray;
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

        public static bool Overlaps(this FlagArray flagArray, IEnumerable<int> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return other.Any(index => flagArray[index]);
        }

        public static bool SetEquals(this FlagArray flagArray, FlagArray other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var a = flagArray;
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
             * a.Or(b).And(a.And(b).Not()) // a.Or(b).Minus(a.And(b))
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
            return a.Or(b).IsSubsetOf(a.And(b));
        }
    }
}