using System.Collections.Generic;
using Theraot.Collections.Specialized;
using QueryId = System.Int32;

namespace Theraot.ECS.Queries
{
    internal sealed class QueryStorage<TComponentKindSet>
        where TComponentKindSet : notnull
    {
        private readonly Dictionary<QueryId, Query<TComponentKindSet>> _queryByQueryId;

        private readonly CompactDictionary<Query<TComponentKindSet>, QueryId> _queryIdByQuery;

        private int _queryId;

        public QueryStorage(IComparer<Query<TComponentKindSet>> queryComparer)
        {
            _queryByQueryId = new Dictionary<QueryId, Query<TComponentKindSet>>();
            _queryIdByQuery = new CompactDictionary<Query<TComponentKindSet>, QueryId>(queryComparer, 16);
        }

        public QueryId AddQuery(Query<TComponentKindSet> query)
        {
            if (_queryIdByQuery.TryGetValue(query, out var found))
            {
                return found;
            }
            var queryId = _queryId;
            _queryIdByQuery.Add(query, queryId);
            _queryByQueryId[queryId] = query;
            _queryId++;
            return queryId;
        }

        public Query<TComponentKindSet> GetQuery(QueryId queryId)
        {
            return _queryByQueryId[queryId];
        }
    }
}