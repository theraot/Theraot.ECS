﻿using System;
using System.Collections.Generic;
using System.Linq;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public partial class Scope<TEntity, TComponentType, TQuery>
    {
        private readonly Dictionary<TEntity, Dictionary<TComponentType, Component>> _componentsByEntity;
        private readonly Dictionary<TEntity, DictionaryKeySet<TComponentType>> _componentTypesByEntity;
        private readonly Dictionary<QueryId, HashSet<TEntity>> _entitiesByQueryId;
        private readonly Func<TEntity> _entityFactory;
        private readonly Dictionary<QueryId, TQuery> _queryByQueryId;
        private readonly Dictionary<TQuery, QueryId> _queryIdByQuery;
        private readonly Dictionary<TComponentType, HashSet<QueryId>> _queryIdsByComponentType;
        private readonly IComponentQueryStrategy<TComponentType, TQuery> _strategy;
        private int _queryId;

        public Scope(Func<TEntity> entityFactory, IComponentQueryStrategy<TComponentType, TQuery> strategy)
        {
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            _strategy = strategy ?? throw  new ArgumentNullException(nameof(strategy));
            _componentsByEntity = new Dictionary<TEntity, Dictionary<TComponentType, Component>>();
            _componentTypesByEntity = new Dictionary<TEntity, DictionaryKeySet<TComponentType>>();
            _entitiesByQueryId = new Dictionary<QueryId, HashSet<TEntity>>();
            _queryIdsByComponentType = new Dictionary<TComponentType, HashSet<QueryId>>();
            _queryByQueryId = new Dictionary<QueryId, TQuery>();
            _queryIdByQuery = new Dictionary<TQuery, QueryId>();
        }

        public TEntity CreateEntity()
        {
            var entity = _entityFactory();
            var dictionary = new Dictionary<TComponentType, Component>();
            _componentsByEntity[entity] = dictionary;
            _componentTypesByEntity[entity] = DictionaryKeySet<TComponentType>.CreateFrom(dictionary);
            return entity;
        }

        public TComponent GetComponent<TComponent>(TEntity entity)
        {
            if (_componentsByEntity.TryGetValue(entity, out var components) && components.TryGetValue(GetComponentType<TComponent>(), out var result))
            {
                return (TComponent) result;
            }
            return default;
        }

        public IEnumerable<TEntity> GetEntities(QueryId query)
        {
            if (_entitiesByQueryId.TryGetValue(query, out var result))
            {
                return result;
            }
            return Array.Empty<TEntity>();
        }

        public QueryId RegisterQuery(TQuery query)
        {
            var queryId = _queryId;
            if (!_queryIdByQuery.TryAdd(query, queryId))
            {
                return _queryIdByQuery[query];
            }
            _queryId++;
            _queryByQueryId[queryId] = query;
            var set = _entitiesByQueryId[queryId] = new HashSet<TEntity>();
            foreach (var componentType in _strategy.GetRelevantComponentTypes(query))
            {
                if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
                {
                    queryIds = new HashSet<QueryId>();
                    _queryIdsByComponentType[componentType] = queryIds;
                }

                queryIds.Add(queryId);
            }

            if (_componentTypesByEntity.Count == 0)
            {
                return queryId;
            }
            foreach (var entity in _componentTypesByEntity.Keys)
            {
                var componentTypes = _componentTypesByEntity[entity];
                if (_strategy.QueryCheck(componentTypes, query) == QueryCheckResult.Add)
                {
                    set.Add(entity);
                }
            }
            return queryId;
        }

        public bool TryGetComponent<TComponent>(TEntity entity, out TComponent component)
        {
            if (_componentsByEntity.TryGetValue(entity, out var components) && components.TryGetValue(GetComponentType<TComponent>(), out var result))
            {
                component = (TComponent) result;
                return true;
            }
            component = default;
            return false;
        }

        private TComponentType GetComponentType<TComponent>(TComponent component)
        {
            var _ = component;
            return _strategy.GetType(typeof(TComponent));
        }

        private TComponentType GetComponentType<TComponent>()
        {
            return _strategy.GetType(typeof(TComponent));
        }

        private IEnumerable<QueryId> GetQueriesByComponentType(TComponentType componentType)
        {
            if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
            {
                return Array.Empty<QueryId>();
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

        private void UpdateEntitiesByQueryOnAddedComponent(TEntity entity, ISet<TComponentType> allComponentsTypes, TComponentType addedComponentType)
        {
            foreach (var queryId in GetQueriesByComponentType(addedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_strategy.QueryCheckOnAddedComponent(addedComponentType, allComponentsTypes, _queryByQueryId[queryId]))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entity);
                        break;
                    case QueryCheckResult.Add:
                        set.Add(entity);
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateEntitiesByQueryOnAddedComponents(TEntity entity, ISet<TComponentType> allComponentsTypes, Component[] addedComponents)
        {
            var addedComponentTypes = addedComponents.Select(GetComponentType).ToArray();
            foreach (var queryId in GetQueriesByComponentTypes(addedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_strategy.QueryCheckOnAddedComponents(addedComponentTypes, allComponentsTypes, _queryByQueryId[queryId]))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entity);
                        break;
                    case QueryCheckResult.Add:
                        set.Add(entity);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
