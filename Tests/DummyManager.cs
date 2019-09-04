using System.Collections.Generic;
using Theraot.ECS;

namespace Tests
{
    internal class DummyManager : IComponentKindManager<int, int>, IEqualityComparer<int>
    {
        public IEqualityComparer<int> ComponentKindEqualityComparer => this;

        public IEqualityComparer<int> ComponentKindSetEqualityComparer => this;

        public void Add(int componentTypeSet, IEnumerable<int> componentTypes)
        {
        }

        public bool Contains(int componentTypeSet, int other)
        {
            return false;
        }

        public bool ContainsAll(int componentTypeSet, int componentType)
        {
            return false;
        }

        public int Create()
        {
            return 0;
        }

        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }

        public bool IsEmpty(int componentTypeSet)
        {
            return false;
        }

        public bool Overlaps(int componentTypeSet, IEnumerable<int> componentTypes)
        {
            return false;
        }

        public bool Overlaps(int componentTypeSetA, int componentTypeSetB)
        {
            return false;
        }

        public void Remove(int componentTypeSet, IEnumerable<int> componentTypes)
        {
        }
    }
}