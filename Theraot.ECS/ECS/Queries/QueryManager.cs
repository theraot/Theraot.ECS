#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using QueryId = System.Int32;

namespace Theraot.ECS.Queries
{
    internal sealed class QueryManager<TComponentKind, TComponentKindSet> : IEqualityComparer<Query<TComponentKindSet>>
    {
        private readonly IComponentKindManager<TComponentKind, TComponentKindSet> _componentKindManager;

        private readonly QueryStorage<TComponentKindSet> _queryStorage;

        public QueryManager(IComponentKindManager<TComponentKind, TComponentKindSet> componentKindManager)
        {
            _componentKindManager = componentKindManager;
            _queryStorage = new QueryStorage<TComponentKindSet>(new ProxyComparer<Query<TComponentKindSet>>(this));
        }

        public QueryId CreateQuery(IEnumerable<TComponentKind> all, IEnumerable<TComponentKind> any, IEnumerable<TComponentKind> none)
        {
            var allSet = _componentKindManager.Create();
            _componentKindManager.Add(allSet, all);
            var anySet = _componentKindManager.Create();
            _componentKindManager.Add(anySet, any);
            var noneSet = _componentKindManager.Create();
            _componentKindManager.Add(noneSet, none);
            return _queryStorage.AddQuery(
                new Query<TComponentKindSet>
                (
                    allSet,
                    anySet,
                    noneSet
                )
            );
        }

        public bool Equals(Query<TComponentKindSet> x, Query<TComponentKindSet> y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }

            var componentKindSetEqualityComparer = _componentKindManager.ComponentKindSetEqualityComparer;

            return componentKindSetEqualityComparer.Equals(x.All, y.All)
                   && componentKindSetEqualityComparer.Equals(x.Any, y.Any)
                   && componentKindSetEqualityComparer.Equals(x.None, y.None);
        }

        public int GetHashCode(Query<TComponentKindSet> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var componentKindSetEqualityComparer = _componentKindManager.ComponentKindSetEqualityComparer;

            return componentKindSetEqualityComparer.GetHashCode(obj.All)
                   ^ componentKindSetEqualityComparer.GetHashCode(obj.Any)
                   ^ componentKindSetEqualityComparer.GetHashCode(obj.None);
        }

        public QueryCheckResult QueryCheck(TComponentKindSet allComponentsKinds, QueryId queryId)
        {
            if (allComponentsKinds == null)
            {
                throw new ArgumentNullException(nameof(allComponentsKinds));
            }

            var query = _queryStorage.GetQuery(queryId);
            if (CheckNotNone(allComponentsKinds, query.None))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckAll(allComponentsKinds, query.All) && CheckAny(allComponentsKinds, query.Any))
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponent(TComponentKind addedComponentKind, TComponentKindSet allComponentsKinds, QueryId queryId)
        {
            if (allComponentsKinds == null)
            {
                throw new ArgumentNullException(nameof(allComponentsKinds));
            }

            var query = _queryStorage.GetQuery(queryId);
            if (CheckNotNone(addedComponentKind, query.None))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckAll(allComponentsKinds, query.All) && CheckAny(allComponentsKinds, query.Any))
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<TComponentKind> addedComponentKinds, TComponentKindSet allComponentsKinds, QueryId queryId)
        {
            if (allComponentsKinds == null)
            {
                throw new ArgumentNullException(nameof(allComponentsKinds));
            }

            var query = _queryStorage.GetQuery(queryId);
            if (CheckNotNone(addedComponentKinds, query.None))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckAll(allComponentsKinds, query.All) && CheckAny(allComponentsKinds, query.Any))
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponent(TComponentKind removedComponentKind, TComponentKindSet allComponentsKinds, QueryId queryId)
        {
            if (allComponentsKinds == null)
            {
                throw new ArgumentNullException(nameof(allComponentsKinds));
            }

            var query = _queryStorage.GetQuery(queryId);
            if (CheckNotAll(removedComponentKind, query.All) || CheckNotAny(allComponentsKinds, query.Any))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckNone(allComponentsKinds, query.None) && CheckAny(allComponentsKinds, query.Any))
            {
                // The entity has none of the components it should not have for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponents(IEnumerable<TComponentKind> removedComponentKinds, TComponentKindSet allComponentsKinds, QueryId queryId)
        {
            if (allComponentsKinds == null)
            {
                throw new ArgumentNullException(nameof(allComponentsKinds));
            }

            var query = _queryStorage.GetQuery(queryId);
            if (CheckNotAll(removedComponentKinds, query.All) || CheckNotAny(allComponentsKinds, query.Any))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if (CheckNone(allComponentsKinds, query.None) && CheckAny(allComponentsKinds, query.Any))
            {
                // The entity has none of the components it should not have for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        private bool CheckAll(TComponentKindSet allComponentsKinds, TComponentKindSet all)
        {
            return _componentKindManager.IsEmpty(all) || _componentKindManager.ContainsAll(allComponentsKinds, all);
        }

        private bool CheckAny(TComponentKindSet allComponentsKinds, TComponentKindSet any)
        {
            return _componentKindManager.IsEmpty(any) || _componentKindManager.Overlaps(any, allComponentsKinds);
        }

        private bool CheckNone(TComponentKindSet allComponentsKinds, TComponentKindSet none)
        {
            return _componentKindManager.IsEmpty(none) || !_componentKindManager.Overlaps(none, allComponentsKinds);
        }

        private bool CheckNotAll(IEnumerable<TComponentKind> removedComponentKinds, TComponentKindSet all)
        {
            return _componentKindManager.Overlaps(all, removedComponentKinds);
        }

        private bool CheckNotAll(TComponentKind removedComponentKind, TComponentKindSet all)
        {
            return _componentKindManager.Contains(all, removedComponentKind);
        }

        private bool CheckNotAny(TComponentKindSet allComponentsKinds, TComponentKindSet any)
        {
            return !_componentKindManager.IsEmpty(any) && !_componentKindManager.Overlaps(any, allComponentsKinds);
        }

        private bool CheckNotNone(IEnumerable<TComponentKind> addedComponentKinds, TComponentKindSet none)
        {
            return _componentKindManager.Overlaps(none, addedComponentKinds);
        }

        private bool CheckNotNone(TComponentKind addedComponentKind, TComponentKindSet none)
        {
            return _componentKindManager.Contains(none, addedComponentKind);
        }

        private bool CheckNotNone(TComponentKindSet allComponentsKinds, TComponentKindSet none)
        {
            return _componentKindManager.Overlaps(none, allComponentsKinds);
        }
    }
}