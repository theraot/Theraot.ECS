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
            if (CheckNotNone(allComponentsTypes, query.None))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckAll(allComponentsTypes, query.All) && CheckAny(allComponentsTypes, query.Any))
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
            if (CheckNotNone(addedComponentType, query.None))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckAll(allComponentsTypes, query.All) && CheckAny(allComponentsTypes, query.Any))
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
            if (CheckNotNone(addedComponentTypes, query.None))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckAll(allComponentsTypes, query.All) && CheckAny(allComponentsTypes, query.Any))
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
            if (CheckNotAll(removedComponentType, query.All) || CheckNotAny(allComponentsTypes, query.Any))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckNone(allComponentsTypes, query.None) && CheckAny(allComponentsTypes, query.Any))
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
            if (CheckNotAll(removedComponentTypes, query.All) || CheckNotAny(allComponentsTypes, query.Any))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckNone(allComponentsTypes, query.None) && CheckAny(allComponentsTypes, query.Any))
            {
                // The entity has none of the components it should not have for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        private static bool CheckAll(ComponentTypeSet allComponentsTypes, ComponentTypeSet all)
        {
            return IsEmpty(all) || Contains(allComponentsTypes, all); //
        }

        private static bool CheckAny(ComponentTypeSet allComponentsTypes, ComponentTypeSet any)
        {
            return IsEmpty(any) || Overlaps(any, allComponentsTypes); //
        }

        private static bool CheckNone(ComponentTypeSet allComponentsTypes, ComponentTypeSet none)
        {
            return IsEmpty(none) || !Overlaps(none, allComponentsTypes); //
        }

        private static bool CheckNotAll(IEnumerable<ComponentType> removedComponentTypes, ComponentTypeSet all)
        {
            return Overlaps(all, removedComponentTypes); //
        }

        private static bool CheckNotAll(ComponentType removedComponentType, ComponentTypeSet all)
        {
            return Contains(all, removedComponentType); //
        }

        private static bool CheckNotAny(ComponentTypeSet allComponentsTypes, ComponentTypeSet any)
        {
            return !IsEmpty(any) && !Overlaps(any, allComponentsTypes); //
        }

        private static bool CheckNotNone(ComponentTypeSet allComponentsTypes, ComponentTypeSet none)
        {
            return Overlaps(allComponentsTypes, none); //
        }

        private static bool CheckNotNone(IEnumerable<ComponentType> addedComponentTypes, ComponentTypeSet none)
        {
            return Overlaps(none, addedComponentTypes); //
        }

        private static bool CheckNotNone(ComponentType addedComponentType, ComponentTypeSet none)
        {
            return Contains(none, addedComponentType); //
        }

        private static bool Contains(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            return componentTypeSet[componentType];
        }

        private static bool Contains(ComponentTypeSet componentTypeSet, ComponentTypeSet other)
        {
            return componentTypeSet.IsSupersetOf(other);
        }

        private static bool Overlaps(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
        {
            return componentTypes.Any(index => componentTypeSet[index]);
        }

        private static bool Overlaps(ComponentTypeSet componentTypeSetA, ComponentTypeSet componentTypeSetB)
        {
            return componentTypeSetA.Overlaps(componentTypeSetB);
        }

        private static bool IsEmpty(ComponentTypeSet componentTypeSet)
        {
            return !componentTypeSet.Contains(true);
        }
    }

    public sealed partial class FlagArrayStrategy : IComponentTypeManager<ComponentType, ComponentTypeSet>
    {
        private readonly int _capacity;

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