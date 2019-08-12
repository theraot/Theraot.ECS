using System;
using System.Collections.Generic;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class Strategy<TComponentType, TComponentTypeSet>
    {
        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeSetManager;

        private readonly QueryStorage<Query<TComponentTypeSet>> _queryStorage;

        public Strategy(IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeSetManager)
        {
            _componentTypeSetManager = componentTypeSetManager;
            _queryStorage = new QueryStorage<Query<TComponentTypeSet>>();
        }

        public QueryId CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            return _queryStorage.AddQuery(
                new Query<TComponentTypeSet>
                (
                    _componentTypeSetManager.Create(all),
                    _componentTypeSetManager.Create(any),
                    _componentTypeSetManager.Create(none)
                )
            );
        }

        public QueryCheckResult QueryCheck(TComponentTypeSet allComponentsTypes, QueryId queryId)
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

        public QueryCheckResult QueryCheckOnAddedComponent(TComponentType addedComponentType, TComponentTypeSet allComponentsTypes, QueryId queryId)
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

        public QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<TComponentType> addedComponentTypes, TComponentTypeSet allComponentsTypes, QueryId queryId)
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

        public QueryCheckResult QueryCheckOnRemovedComponent(TComponentType removedComponentType, TComponentTypeSet allComponentsTypes, QueryId queryId)
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

        public QueryCheckResult QueryCheckOnRemovedComponents(IEnumerable<TComponentType> removedComponentTypes, TComponentTypeSet allComponentsTypes, QueryId queryId)
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

        private bool CheckAll(TComponentTypeSet allComponentsTypes, TComponentTypeSet all)
        {
            return _componentTypeSetManager.IsEmpty(all) || _componentTypeSetManager.ContainsAll(allComponentsTypes, all); //
        }

        private bool CheckAny(TComponentTypeSet allComponentsTypes, TComponentTypeSet any)
        {
            return _componentTypeSetManager.IsEmpty(any) || _componentTypeSetManager.Overlaps(any, allComponentsTypes); //
        }

        private bool CheckNone(TComponentTypeSet allComponentsTypes, TComponentTypeSet none)
        {
            return _componentTypeSetManager.IsEmpty(none) || !_componentTypeSetManager.Overlaps(none, allComponentsTypes); //
        }

        private bool CheckNotAll(TComponentType removedComponentType, TComponentTypeSet all)
        {
            return _componentTypeSetManager.Contains(all, removedComponentType); //
        }

        private bool CheckNotAll(IEnumerable<TComponentType> removedComponentTypes, TComponentTypeSet all)
        {
            return _componentTypeSetManager.Overlaps(all, removedComponentTypes); //
        }

        private bool CheckNotAny(TComponentTypeSet allComponentsTypes, TComponentTypeSet any)
        {
            return !_componentTypeSetManager.IsEmpty(any) && !_componentTypeSetManager.Overlaps(any, allComponentsTypes); //
        }

        private bool CheckNotNone(TComponentType addedComponentType, TComponentTypeSet none)
        {
            return _componentTypeSetManager.Contains(none, addedComponentType); //
        }

        private bool CheckNotNone(TComponentTypeSet allComponentsTypes, TComponentTypeSet none)
        {
            return _componentTypeSetManager.Overlaps(none, allComponentsTypes); //
        }

        private bool CheckNotNone(IEnumerable<TComponentType> addedComponentTypes, TComponentTypeSet none)
        {
            return _componentTypeSetManager.Overlaps(none, addedComponentTypes); //
        }
    }
}