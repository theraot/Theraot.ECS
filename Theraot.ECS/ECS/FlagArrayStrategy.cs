﻿using System;
using System.Collections.Generic;
using System.Linq;
using Component = System.Object;
using ComponentType = System.Int32;
using ComponentTypeSet = Theraot.Collections.Specialized.FlagArray;

namespace Theraot.ECS
{
    public sealed class FlagArrayStrategy : IComponentQueryStrategy<ComponentType, ComponentTypeSet, FlagArrayQuery>
    {
        private readonly int _capacity;

        public FlagArrayStrategy(int capacity)
        {
            _capacity = capacity;
        }

        public ComponentTypeSet CreateComponentTypeSet(Dictionary<ComponentType, Component> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            var set = new ComponentTypeSet(_capacity);
            foreach (var pair in dictionary)
            {
                set[pair.Key] = true;
            }
            return set;
        }

        public FlagArrayQuery CreateQuery(IEnumerable<ComponentType> all, IEnumerable<ComponentType> any, IEnumerable<ComponentType> none)
        {
            return new FlagArrayQuery(_capacity, all, any, none);
        }

        public QueryCheckResult QueryCheck(ComponentTypeSet allComponentsTypes, FlagArrayQuery query)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (allComponentsTypes.Overlaps(query.None))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (!query.All.Contains(true) || allComponentsTypes.IsSupersetOf(query.All))
                && (!query.Any.Contains(true) || allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponent(ComponentType addedComponentType, ComponentTypeSet allComponentsTypes, FlagArrayQuery query)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (query.None[addedComponentType])
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (!query.All.Contains(true) || allComponentsTypes.IsSupersetOf(query.All))
                && (!query.Any.Contains(true) || allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<ComponentType> addedComponentTypes, ComponentTypeSet allComponentsTypes, FlagArrayQuery query)
        {
            if (addedComponentTypes == null)
            {
                throw new ArgumentNullException(nameof(addedComponentTypes));
            }
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (addedComponentTypes.Any(index => query.None[index]))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (!query.All.Contains(true) || allComponentsTypes.IsSupersetOf(query.All))
                && (!query.Any.Contains(true) || allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponent(int removedComponentType, ComponentTypeSet allComponentsTypes, FlagArrayQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }
            if (query.All[removedComponentType] || (query.Any.Contains(true) && !allComponentsTypes.Overlaps(query.Any)))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (!query.None.Contains(true) || !query.None.Overlaps(allComponentsTypes))
                && (!query.Any.Contains(true) || allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has none of the components it should not have for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponents(IEnumerable<int> removedComponentTypes, ComponentTypeSet allComponentsTypes, FlagArrayQuery query)
        {
            if (removedComponentTypes == null)
            {
                throw new ArgumentNullException(nameof(removedComponentTypes));
            }
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            if (removedComponentTypes.Any(index => query.All[index]) || (query.Any.Contains(true) && !allComponentsTypes.Overlaps(query.Any)))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (!query.None.Contains(true) || !query.None.Overlaps(allComponentsTypes))
                && (!query.Any.Contains(true) || allComponentsTypes.Overlaps(query.Any))
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
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            componentTypeSet[componentType] = true;
        }

        public void SetComponentTypes(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            foreach (var componentType in componentTypes)
            {
                componentTypeSet[componentType] = true;
            }
        }

        public void UnsetComponentType(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            componentTypeSet[componentType] = false;
        }

        public void UnsetComponentTypes(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            foreach (var componentType in componentTypes)
            {
                componentTypeSet[componentType] = false;
            }
        }
    }
}