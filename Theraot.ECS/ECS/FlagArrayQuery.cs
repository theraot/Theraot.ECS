using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.ECS
{
    public sealed class FlagArrayQuery
    {
        public FlagArrayQuery(int length, IEnumerable<int> all, IEnumerable<int> any, IEnumerable<int> none)
        {
            if (all == null)
            {
                throw new ArgumentNullException(nameof(all));
            }
            if (any == null)
            {
                throw new ArgumentNullException(nameof(any));
            }
            if (none == null)
            {
                throw new ArgumentNullException(nameof(none));
            }
            All = ToFlagArray(all, length);
            Any = ToFlagArray(any, length);
            None = ToFlagArray(none, length);
        }

        public FlagArray All { get; }

        public FlagArray Any { get; }

        public FlagArray None { get; }

        private static FlagArray ToFlagArray(IEnumerable<int> collection, int length)
        {
            var result = new FlagArray(length);
            foreach (var index in collection)
            {
                result[index] = true;
            }
            return result;
        }
    }
}