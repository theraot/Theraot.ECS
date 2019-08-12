using System.Collections.Generic;
using Theraot.ECS;

namespace Tests
{
    internal class DummyManager : IComponentTypeManager<int, int>
    {
        public void Add(int componentTypeSet, IEnumerable<int> componentTypes)
        {
        }

        public void Add(int componentTypeSet, int componentType)
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

        public int Create(Dictionary<int, object> dictionary)
        {
            return 0;
        }

        public int Create(IEnumerable<int> enumerable)
        {
            return 0;
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

        public void Remove(int componentTypeSet, int componentType)
        {
        }
    }
}