using System;
using System.Collections.Generic;
using Component = System.Object;
using ComponentType = System.String;
using ComponentTypeSet = System.Collections.Generic.ISet<string>;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public sealed partial class TypeHashSetStrategy : IComponentQueryStrategy<ComponentType, ComponentTypeSet>
    {
        private readonly QueryStorage<Query<ComponentTypeSet>> _queryStorage;

        public TypeHashSetStrategy()
        {
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
            return Overlaps(none, allComponentsTypes); //
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
            return componentTypeSet.Count != 0 && componentTypeSet.Contains(componentType);
        }

        private static bool Contains(ComponentTypeSet componentTypeSet, ComponentTypeSet other)
        {
            return componentTypeSet.IsSupersetOf(other);
        }

        private static bool Overlaps(ComponentTypeSet componentTypeSet, IEnumerable<string> componentTypes)
        {
            return componentTypeSet.Count != 0 && componentTypeSet.Overlaps(componentTypes);
        }

        private static bool Overlaps(ComponentTypeSet componentTypeSetA, ComponentTypeSet componentTypeSetB)
        {
            return componentTypeSetA.Count != 0 && (componentTypeSetA.Count > componentTypeSetB.Count ? componentTypeSetA.Overlaps(componentTypeSetB) : componentTypeSetB.Overlaps(componentTypeSetA));
        }

        private static bool IsEmpty(ComponentTypeSet componentTypeSet)
        {
            return componentTypeSet.Count == 0;
        }
    }

    public sealed partial class TypeHashSetStrategy : IComponentTypeManager<ComponentType, ComponentTypeSet>
    {
        public ComponentTypeSet Create(Dictionary<ComponentType, Component> dictionary)
        {
            return DictionaryKeySet.CreateFrom(dictionary);
        }

        public ComponentTypeSet Create(IEnumerable<ComponentType> enumerable)
        {
            return new HashSet<ComponentType>(enumerable);
        }

        public void Add(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            if (componentTypeSet.IsReadOnly)
            {
                return;
            }
            componentTypeSet.Add(componentType);
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
            if (componentTypeSet.IsReadOnly)
            {
                return;
            }

            foreach (var componentType in componentTypes)
            {
                componentTypeSet.Add(componentType);
            }
        }

        public void Remove(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            if (componentTypeSet.IsReadOnly)
            {
                return;
            }

            componentTypeSet.Remove(componentType);
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
            if (componentTypeSet.IsReadOnly)
            {
                return;
            }

            foreach (var componentType in componentTypes)
            {
                componentTypeSet.Remove(componentType);
            }
        }
    }
}