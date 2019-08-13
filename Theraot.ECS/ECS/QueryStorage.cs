using System.Collections.Generic;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class QueryStorage<TComponentTypeSet>
    {
        private readonly Dictionary<QueryId, Query<TComponentTypeSet>> _queryByQueryId;

        private readonly Dictionary<Query<TComponentTypeSet>, QueryId> _queryIdByQuery;

        private int _queryId;

        public QueryStorage()
        {
            _queryByQueryId = new Dictionary<QueryId, Query<TComponentTypeSet>>();
            _queryIdByQuery = new Dictionary<Query<TComponentTypeSet>, QueryId>();
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