using System.Collections.Generic;
using Theraot.Collections.Specialized;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class QueryStorage<TComponentTypeSet>
    {
        private readonly CacheFriendlyDictionary<QueryId, Query<TComponentTypeSet>> _queryByQueryId;

        private readonly Dictionary<Query<TComponentTypeSet>, QueryId> _queryIdByQuery;

        private int _queryId;

        public QueryStorage(IEqualityComparer<Query<TComponentTypeSet>> queryEqualityComparer)
        {
            _queryByQueryId = new CacheFriendlyDictionary<QueryId, Query<TComponentTypeSet>>(Comparer<QueryId>.Default, 16);
            _queryIdByQuery = new Dictionary<Query<TComponentTypeSet>, QueryId>(queryEqualityComparer);
        }

        public QueryId AddQuery(Query<TComponentTypeSet> query)
        {
            var queryId = _queryId;
            if (!_queryIdByQuery.TryAdd(query, queryId))
            {
                return _queryIdByQuery[query];
            }
            _queryId++;
            _queryByQueryId[queryId] = query;
            return queryId;
        }

        public Query<TComponentTypeSet> GetQuery(QueryId queryId)
        {
            return _queryByQueryId[queryId];
        }
    }
}