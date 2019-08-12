using System.Collections.Generic;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class QueryStorage<TQuery>
    {
        private readonly Dictionary<QueryId, TQuery> _queryByQueryId;

        private readonly Dictionary<TQuery, QueryId> _queryIdByQuery;

        private int _queryId;

        public QueryStorage()
        {
            _queryByQueryId = new Dictionary<QueryId, TQuery>();
            _queryIdByQuery = new Dictionary<TQuery, QueryId>();
        }

        public QueryId AddQuery(TQuery query)
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

        public TQuery GetQuery(QueryId queryId)
        {
            return _queryByQueryId[queryId];
        }
    }
}