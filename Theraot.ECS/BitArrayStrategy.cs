﻿using System.Collections.Generic;
using Component = System.Object;
using ComponentType = System.Int32;
using ComponentTypeSet = System.Collections.BitArray;

namespace Theraot.ECS
{
    public sealed class BitArrayStrategy : IComponentQueryStrategy<ComponentType, ComponentTypeSet, BitArrayQuery>
    {
        private readonly int _capacity;

        public BitArrayStrategy(int capacity)
        {
            _capacity = capacity;
        }

        public ComponentTypeSet CreateComponentTypeSet(Dictionary<ComponentType, Component> dictionary)
        {
            var set = new ComponentTypeSet(_capacity);
            foreach (var pair in dictionary)
            {
                set[pair.Key] = true;
            }
            return set;
        }

        public BitArrayQuery CreateQuery(IEnumerable<ComponentType> all, IEnumerable<ComponentType> any, IEnumerable<ComponentType> none)
        {
            return new BitArrayQuery(_capacity, all, any, none);
        }

        public QueryCheckResult QueryCheck(ComponentTypeSet allComponentsTypes, BitArrayQuery query)
        {
            if (allComponentsTypes.Overlaps(query.None))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.All.IsEmpty() || allComponentsTypes.IsSupersetOf(query.All))
                && (query.Any.IsEmpty() || allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponent(ComponentType addedComponentType, ComponentTypeSet allComponentsTypes, BitArrayQuery query)
        {
            if (query.None[addedComponentType])
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.All.IsEmpty() || allComponentsTypes.IsSupersetOf(query.All))
                && (query.Any.IsEmpty() || allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<ComponentType> addedComponentTypes, ComponentTypeSet allComponentsTypes, BitArrayQuery query)
        {
            if (query.None.Overlaps(addedComponentTypes))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.All.IsEmpty() || allComponentsTypes.IsSupersetOf(query.All))
                && (query.Any.IsEmpty() || allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponent(int removedComponentType, ComponentTypeSet allComponentsTypes, BitArrayQuery query)
        {
            if (query.All[removedComponentType] || (!query.Any.IsEmpty() && !allComponentsTypes.Overlaps(query.Any)))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.None.IsEmpty() || !query.None.Overlaps(allComponentsTypes))
                && (query.Any.IsEmpty() || allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has none of the components it should not have for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponents(IEnumerable<int> removedComponentTypes, ComponentTypeSet allComponentsTypes, BitArrayQuery query)
        {
            if (query.All.Overlaps(removedComponentTypes) || (!query.Any.IsEmpty() && !allComponentsTypes.Overlaps(query.Any)))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.None.IsEmpty() || !query.None.Overlaps(allComponentsTypes))
                && (query.Any.IsEmpty() || allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has none of the components it should not have for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public void SetComponentType(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            componentTypeSet[componentType] = true;
        }

        public void SetComponentTypes(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
        {
            foreach (var componentType in componentTypes)
            {
                componentTypeSet[componentType] = true;
            }
        }

        public void UnsetComponentType(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            componentTypeSet[componentType] = false;
        }

        public void UnsetComponentTypes(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
        {
            foreach (var componentType in componentTypes)
            {
                componentTypeSet[componentType] = false;
            }
        }
    }
}