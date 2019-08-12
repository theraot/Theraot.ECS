using System;
using System.Collections.Generic;
using ComponentType = System.Int32;
using ComponentTypeSet = Theraot.Collections.Specialized.FlagArray;

using QueryId = System.Int32;

namespace Theraot.ECS
{
    public sealed class FlagArrayStrategy : IComponentQueryStrategy<ComponentType, ComponentTypeSet>
    {
        private readonly QueryStorage<Query<ComponentTypeSet>> _queryStorage;

        public FlagArrayStrategy(int capacity)
        {
            ComponentTypeSetManager = new FlagArrayManager(capacity);
            _queryStorage = new QueryStorage<Query<ComponentTypeSet>>();
        }

        public IComponentTypeManager<ComponentType, ComponentTypeSet> ComponentTypeSetManager { get; }

        public QueryId CreateQuery(IEnumerable<ComponentType> all, IEnumerable<ComponentType> any, IEnumerable<ComponentType> none)
        {
            return _queryStorage.AddQuery(
                new Query<ComponentTypeSet>
                (
                    ComponentTypeSetManager.Create(all),
                    ComponentTypeSetManager.Create(any),
                    ComponentTypeSetManager.Create(none)
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
            return ComponentTypeSetManager.IsEmpty(all) || ComponentTypeSetManager.Contains(allComponentsTypes, all); //
        }

        private bool CheckAny(ComponentTypeSet allComponentsTypes, ComponentTypeSet any)
        {
            return ComponentTypeSetManager.IsEmpty(any) || ComponentTypeSetManager.Overlaps(any, allComponentsTypes); //
        }

        private bool CheckNone(ComponentTypeSet allComponentsTypes, ComponentTypeSet none)
        {
            return ComponentTypeSetManager.IsEmpty(none) || !ComponentTypeSetManager.Overlaps(none, allComponentsTypes); //
        }

        private bool CheckNotAll(ComponentType removedComponentType, ComponentTypeSet all)
        {
            return ComponentTypeSetManager.Contains(all, removedComponentType); //
        }

        private bool CheckNotAll(IEnumerable<ComponentType> removedComponentTypes, ComponentTypeSet all)
        {
            return ComponentTypeSetManager.Overlaps(all, removedComponentTypes); //
        }

        private bool CheckNotAny(ComponentTypeSet allComponentsTypes, ComponentTypeSet any)
        {
            return !ComponentTypeSetManager.IsEmpty(any) && !ComponentTypeSetManager.Overlaps(any, allComponentsTypes); //
        }

        private bool CheckNotNone(ComponentType addedComponentType, ComponentTypeSet none)
        {
            return ComponentTypeSetManager.Contains(none, addedComponentType); //
        }

        private bool CheckNotNone(ComponentTypeSet allComponentsTypes, ComponentTypeSet none)
        {
            return ComponentTypeSetManager.Overlaps(allComponentsTypes, none); //
        }

        private bool CheckNotNone(IEnumerable<ComponentType> addedComponentTypes, ComponentTypeSet none)
        {
            return ComponentTypeSetManager.Overlaps(none, addedComponentTypes); //
        }
    }
}