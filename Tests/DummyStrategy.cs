using System.Collections.Generic;
using Theraot.ECS;

namespace Tests
{
    internal class DummyStrategy : IComponentQueryStrategy<int, int>, IComponentTypeManager<int, int>
    {
        public IComponentTypeManager<int, int> ComponentTypeManager => this;

        public int Create(Dictionary<int, object> dictionary)
        {
            return 0;
        }

        public int Create(IEnumerable<int> enumerable)
        {
            return 0;
        }

        public int CreateQuery(IEnumerable<int> all, IEnumerable<int> any, IEnumerable<int> none)
        {
            return 0;
        }

        public QueryCheckResult QueryCheck(int allComponentsTypes, int query)
        {
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponent(int addedComponentType, int allComponentsTypes, int query)
        {
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<int> addedComponentTypes, int allComponentsTypes, int query)
        {
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponent(int removedComponentType, int allComponentsTypes, int query)
        {
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponents(IEnumerable<int> removedComponentTypes, int allComponentsTypes, int query)
        {
            return QueryCheckResult.Noop;
        }

        public void Add(int componentTypeSet, int componentType)
        {
        }

        public void Add(int componentTypeSet, IEnumerable<int> componentTypes)
        {
        }

        public void Remove(int componentTypeSet, int componentType)
        {
        }

        public void Remove(int componentTypeSet, IEnumerable<int> componentTypes)
        {
        }
    }
}