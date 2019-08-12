using System;
using System.Collections.Generic;
using System.Linq;
using Component = System.Object;
using ComponentType = System.Int32;
using ComponentTypeSet = Theraot.Collections.Specialized.FlagArray;

using QueryId = System.Int32;

namespace Theraot.ECS
{
    public sealed partial class FlagArrayStrategy : IComponentQueryStrategy<ComponentType, ComponentTypeSet>
    {
        private readonly int _capacity;

        private readonly QueryStorage<Query<ComponentTypeSet>> _queryStorage;

        public FlagArrayStrategy(int capacity)
        {
            _capacity = capacity;
            _queryStorage = new QueryStorage<Query<ComponentTypeSet>>();
        }

        public IComponentTypeManager<ComponentType, ComponentTypeSet> ComponentTypeManager => this;

        public QueryId CreateQuery(IEnumerable<ComponentType> all, IEnumerable<ComponentType> any, IEnumerable<ComponentType> none)
        {
            return _queryStorage.AddQuery(
                new Query<ComponentTypeSet>
                (
                    CreateComponentTypeSet(all),
                    CreateComponentTypeSet(any),
                    CreateComponentTypeSet(none)
                )
            );
        }

        public QueryCheckResult QueryCheck(ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
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

        public QueryCheckResult QueryCheckOnAddedComponent(ComponentType addedComponentType, ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
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

        public QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<ComponentType> addedComponentTypes, ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (addedComponentTypes == null)
            {
                throw new ArgumentNullException(nameof(addedComponentTypes));
            }
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
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

        public QueryCheckResult QueryCheckOnRemovedComponent(ComponentType removedComponentType, ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
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

        public QueryCheckResult QueryCheckOnRemovedComponents(IEnumerable<ComponentType> removedComponentTypes, ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (removedComponentTypes == null)
            {
                throw new ArgumentNullException(nameof(removedComponentTypes));
            }
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
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
    }

    public sealed partial class FlagArrayStrategy : IComponentTypeManager<ComponentType, ComponentTypeSet>
    {
        public ComponentTypeSet CreateComponentTypeSet(Dictionary<ComponentType, Component> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return CreateComponentTypeSetExtracted(dictionary.Keys);
        }

        public ComponentTypeSet CreateComponentTypeSet(IEnumerable<ComponentType> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            return CreateComponentTypeSetExtracted(enumerable);
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

        private ComponentTypeSet CreateComponentTypeSetExtracted(IEnumerable<ComponentType> enumerable)
        {
            var set = new ComponentTypeSet(_capacity);
            foreach (var key in enumerable)
            {
                set[key] = true;
            }
            return set;
        }
    }
}