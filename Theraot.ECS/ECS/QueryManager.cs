using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class QueryManager<TComponentType, TComponentTypeSet> : IEqualityComparer<Query<TComponentTypeSet>>
    {
        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly QueryStorage<TComponentTypeSet> _queryStorage;

        public QueryManager(IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            _componentTypeManager = componentTypeManager;
            _queryStorage = new QueryStorage<TComponentTypeSet>(new ProxyComparer<Query<TComponentTypeSet>>(this));
        }

        public QueryId CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            var allSet = _componentTypeManager.Create();
            _componentTypeManager.Add(allSet, all);
            var anySet = _componentTypeManager.Create();
            _componentTypeManager.Add(anySet, any);
            var noneSet = _componentTypeManager.Create();
            _componentTypeManager.Add(noneSet, none);
            return _queryStorage.AddQuery(
                new Query<TComponentTypeSet>
                (
                    allSet,
                    anySet,
                    noneSet
                )
            );
        }

        public bool Equals(Query<TComponentTypeSet> x, Query<TComponentTypeSet> y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }

            return _componentTypeManager.ContainsAll(x.All, y.All)
                   && _componentTypeManager.ContainsAll(y.All, x.All)
                   && _componentTypeManager.ContainsAll(x.Any, y.Any)
                   && _componentTypeManager.ContainsAll(y.Any, x.Any)
                   && _componentTypeManager.ContainsAll(x.None, y.None)
                   && _componentTypeManager.ContainsAll(y.None, x.None);
        }

        public int GetHashCode(Query<TComponentTypeSet> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.All.GetHashCode() ^ obj.Any.GetHashCode() ^ obj.None.GetHashCode();
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
            return _componentTypeManager.IsEmpty(all) || _componentTypeManager.ContainsAll(allComponentsTypes, all); //
        }

        private bool CheckAny(TComponentTypeSet allComponentsTypes, TComponentTypeSet any)
        {
            return _componentTypeManager.IsEmpty(any) || _componentTypeManager.Overlaps(any, allComponentsTypes); //
        }

        private bool CheckNone(TComponentTypeSet allComponentsTypes, TComponentTypeSet none)
        {
            return _componentTypeManager.IsEmpty(none) || !_componentTypeManager.Overlaps(none, allComponentsTypes); //
        }

        private bool CheckNotAll(IEnumerable<TComponentType> removedComponentTypes, TComponentTypeSet all)
        {
            return _componentTypeManager.Overlaps(all, removedComponentTypes); //
        }

        private bool CheckNotAll(TComponentType removedComponentType, TComponentTypeSet all)
        {
            return _componentTypeManager.Contains(all, removedComponentType); //
        }

        private bool CheckNotAny(TComponentTypeSet allComponentsTypes, TComponentTypeSet any)
        {
            return !_componentTypeManager.IsEmpty(any) && !_componentTypeManager.Overlaps(any, allComponentsTypes); //
        }

        private bool CheckNotNone(IEnumerable<TComponentType> addedComponentTypes, TComponentTypeSet none)
        {
            return _componentTypeManager.Overlaps(none, addedComponentTypes); //
        }

        private bool CheckNotNone(TComponentType addedComponentType, TComponentTypeSet none)
        {
            return _componentTypeManager.Contains(none, addedComponentType); //
        }

        private bool CheckNotNone(TComponentTypeSet allComponentsTypes, TComponentTypeSet none)
        {
            return _componentTypeManager.Overlaps(none, allComponentsTypes); //
        }
    }
}