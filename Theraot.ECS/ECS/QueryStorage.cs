﻿using System.Collections.Generic;
using Theraot.Collections.Specialized;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class QueryStorage<TComponentTypeSet>
    {
        private readonly Dictionary<QueryId, Query<TComponentTypeSet>> _queryByQueryId;

        private readonly CompactDictionary<Query<TComponentTypeSet>, QueryId> _queryIdByQuery;

        private int _queryId;

        public QueryStorage(IComparer<Query<TComponentTypeSet>> queryComparer)
        {
            _queryByQueryId = new Dictionary<QueryId, Query<TComponentTypeSet>>();
            _queryIdByQuery = new CompactDictionary<Query<TComponentTypeSet>, QueryId>(queryComparer, 16);
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