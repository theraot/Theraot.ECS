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
                    Create(all),
                    Create(any),
                    Create(none)
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

        private bool CheckAll(ComponentTypeSet allComponentsTypes, ComponentTypeSet all)
        {
            return IsEmpty(all) || Contains(allComponentsTypes, all); //
        }

        private bool CheckAny(ComponentTypeSet allComponentsTypes, ComponentTypeSet any)
        {
            return IsEmpty(any) || Overlaps(any, allComponentsTypes); //
        }

        private bool CheckNone(ComponentTypeSet allComponentsTypes, ComponentTypeSet none)
        {
            return IsEmpty(none) || !Overlaps(none, allComponentsTypes); //
        }

        private bool CheckNotAll(ComponentType removedComponentType, ComponentTypeSet all)
        {
            return Contains(all, removedComponentType); //
        }

        private bool CheckNotAll(IEnumerable<ComponentType> removedComponentTypes, ComponentTypeSet all)
        {
            return Overlaps(all, removedComponentTypes); //
        }

        private bool CheckNotAny(ComponentTypeSet allComponentsTypes, ComponentTypeSet any)
        {
            return !IsEmpty(any) && !Overlaps(any, allComponentsTypes); //
        }

        private bool CheckNotNone(ComponentType addedComponentType, ComponentTypeSet none)
        {
            return Contains(none, addedComponentType); //
        }

        private bool CheckNotNone(ComponentTypeSet allComponentsTypes, ComponentTypeSet none)
        {
            return Overlaps(allComponentsTypes, none); //
        }

        private bool CheckNotNone(IEnumerable<ComponentType> addedComponentTypes, ComponentTypeSet none)
        {
            return Overlaps(none, addedComponentTypes); //
        }
    }

    public sealed partial class FlagArrayStrategy : IComponentTypeManager<ComponentType, ComponentTypeSet>
    {
        private readonly int _capacity;

        public void Add(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            componentTypeSet[componentType] = true;
        }

        public void Add(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
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

        public bool Contains(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet[componentType];
        }

        public bool Contains(ComponentTypeSet componentTypeSet, ComponentTypeSet other)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.IsSupersetOf(other);
        }

        public ComponentTypeSet Create(Dictionary<ComponentType, Component> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return CreateComponentTypeSetExtracted(dictionary.Keys);
        }

        public ComponentTypeSet Create(IEnumerable<ComponentType> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            return CreateComponentTypeSetExtracted(enumerable);
        }

        public bool IsEmpty(ComponentTypeSet componentTypeSet)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return !componentTypeSet.Contains(true);
        }

        public bool Overlaps(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypes.Any(index => componentTypeSet[index]);
        }

        public bool Overlaps(ComponentTypeSet componentTypeSetA, ComponentTypeSet componentTypeSetB)
        {
            if (componentTypeSetA == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSetA));
            }
            return componentTypeSetA.Overlaps(componentTypeSetB);
        }

        public void Remove(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            componentTypeSet[componentType] = false;
        }

        public void Remove(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
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