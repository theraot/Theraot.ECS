using System;
using System.Collections.Generic;
using System.Linq;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public static class Scope
    {
        public static IScope<TEntity, TComponentType> CreateScope<TEntity, TComponentType, TComponentTypeSet>(Func<TEntity> entityIdFactory, IComponentTypeManager<TComponentType, TComponentTypeSet> manager)
        {
            return new Scope<TEntity, TComponentType, TComponentTypeSet>(entityIdFactory, manager);
        }
    }

    public sealed partial class Scope<TEntity, TComponentType, TComponentTypeSet> : IScope<TEntity, TComponentType>
    {
        private readonly Dictionary<TEntity, ComponentStorage<TComponentType, TComponentTypeSet>> _componentsByEntity;

        private readonly Dictionary<QueryId, HashSet<TEntity>> _entitiesByQueryId;

        private readonly Func<TEntity> _entityFactory;

        private readonly Dictionary<TComponentType, HashSet<QueryId>> _queryIdsByComponentType;

        private readonly QueryManager<TComponentType, TComponentTypeSet> _queryManager;

        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _manager;

        internal Scope(Func<TEntity> entityFactory, IComponentTypeManager<TComponentType, TComponentTypeSet> manager)
        {
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _queryManager = new QueryManager<TComponentType, TComponentTypeSet>(manager);
            _componentsByEntity = new Dictionary<TEntity, ComponentStorage<TComponentType, TComponentTypeSet>>();
            _entitiesByQueryId = new Dictionary<QueryId, HashSet<TEntity>>();
            _queryIdsByComponentType = new Dictionary<TComponentType, HashSet<QueryId>>();
        }

        public TEntity CreateEntity()
        {
            var entity = _entityFactory();
            _componentsByEntity[entity] = new ComponentStorage<TComponentType, TComponentTypeSet>(_manager);
            return entity;
        }

        public QueryId CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            var allArray = all.ToArray();
            var anyArray = any.ToArray();
            var noneArray = none.ToArray();
            var queryId = _queryManager.CreateQuery(allArray, anyArray, noneArray);
            var set = _entitiesByQueryId[queryId] = new HashSet<TEntity>();
            foreach (var componentType in allArray.Concat(anyArray).Concat(noneArray))
            {
                if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
                {
                    queryIds = new HashSet<QueryId>();
                    _queryIdsByComponentType[componentType] = queryIds;
                }

                queryIds.Add(queryId);
            }

            if (_componentsByEntity.Count == 0)
            {
                return queryId;
            }
            foreach (var entity in _componentsByEntity.Keys)
            {
                var componentTypes = _componentsByEntity[entity].ComponentTypes;
                if (_queryManager.QueryCheck(componentTypes, queryId) == QueryCheckResult.Add)
                {
                    set.Add(entity);
                }
            }
            return queryId;
        }

        public TComponent GetComponent<TComponent>(TEntity entity, TComponentType componentType)
        {
            if (_componentsByEntity.TryGetValue(entity, out var components) && components.TryGetValue(componentType, out var result))
            {
                return (TComponent)result;
            }
            return default;
        }

        public IEnumerable<TEntity> GetEntities(QueryId query)
        {
            if (_entitiesByQueryId.TryGetValue(query, out var result))
            {
                return result;
            }
            return EmptyArray<TEntity>.Instance;
        }

        public bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component)
        {
            if (_componentsByEntity.TryGetValue(entity, out var components) && components.TryGetValue(componentType, out var result))
            {
                component = (TComponent)result;
                return true;
            }
            component = default;
            return false;
        }

        private IEnumerable<QueryId> GetQueriesByComponentType(TComponentType componentType)
        {
            if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
            {
                return EmptyArray<QueryId>.Instance;
            }
            return queryIds;
        }

        private IEnumerable<QueryId> GetQueriesByComponentTypes(IEnumerable<TComponentType> componentTypes)
        {
            var set = new HashSet<int>();
            foreach (var componentType in componentTypes)
            {
                foreach (var queryId in GetQueriesByComponentType(componentType))
                {
                    set.Add(queryId);
                }
            }

            return set;
        }
    }
}