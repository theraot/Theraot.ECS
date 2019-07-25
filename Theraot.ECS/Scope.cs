using System;
using System.Collections.Generic;
using System.Linq;
using ComponentType = System.Type;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public partial class Scope<TEntity>
    {
        private readonly Dictionary<TEntity, Dictionary<ComponentType, Component>> _componentsByEntity;
        private readonly Dictionary<QueryId, HashSet<TEntity>> _entitiesByQueryId;
        private readonly Func<TEntity> _entityFactory;
        private readonly Dictionary<QueryId, Query> _queryByQueryId;
        private readonly Dictionary<Query, QueryId> _queryIdByQuery;
        private readonly Dictionary<ComponentType, HashSet<QueryId>> _queryIdsByComponentType;
        private int _queryId;

        public Scope(Func<TEntity> entityFactory)
        {
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            _componentsByEntity = new Dictionary<TEntity, Dictionary<ComponentType, Component>>();
            _entitiesByQueryId = new Dictionary<QueryId, HashSet<TEntity>>();
            _queryIdsByComponentType = new Dictionary<ComponentType, HashSet<QueryId>>();
            _queryByQueryId = new Dictionary<QueryId, Query>();
            _queryIdByQuery = new Dictionary<Query, QueryId>();
        }

        public TEntity CreateEntity()
        {
            var entity = _entityFactory();
            _componentsByEntity[entity] = new Dictionary<ComponentType, Component>();
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

        public QueryId RegisterQuery(Query query)
        {
            var queryId = _queryId;
            if (!_queryIdByQuery.TryAdd(query, queryId))
            {
                return _queryIdByQuery[query];
            }
            _queryId++;
            _queryByQueryId[queryId] = query;
            var set = _entitiesByQueryId[queryId] = new HashSet<TEntity>();
            foreach (var componentTypes in new[] { query.All, query.Any, query.None })
            {
                foreach (var componentType in componentTypes)
                {
                    if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
                    {
                        queryIds = new HashSet<QueryId>();
                        _queryIdsByComponentType[componentType] = queryIds;
                    }

                    queryIds.Add(queryId);
                }
            }

            if (_componentsByEntity.Count == 0)
            {
                return queryId;
            }
            foreach (var entity in _componentsByEntity.Keys)
            {
                if (QueryCheck(new HashSet<ComponentType>(_componentsByEntity[entity].Select(GetComponentType)), queryId) == QueryCheckResult.Add)
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

        private static ComponentType GetComponentType<TComponent>(TComponent component)
        {
            var _ = component;
            return typeof(TComponent);
        }

        private static ComponentType GetComponentType<TComponent>()
        {
            return typeof(TComponent);
        }

        private IEnumerable<QueryId> GetQueriesByComponentType(ComponentType componentType)
        {
            if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
            {
                return Array.Empty<QueryId>();
            }
            return queryIds;
        }

        private IEnumerable<QueryId> GetQueriesByComponentTypes(IEnumerable<ComponentType> componentTypes)
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

        private QueryCheckResult QueryCheck(HashSet<ComponentType> allComponentsTypes, QueryId queryId)
        {
            var query = _queryByQueryId[queryId];
            if
            (
                query.None.Count != 0
                && (query.None.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.None) : query.None.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        private QueryCheckResult QueryCheckOnAddedComponent(ComponentType addedComponentType, HashSet<ComponentType> allComponentsTypes, QueryId queryId)
        {
            var query = _queryByQueryId[queryId];
            if (query.None.Count != 0 && query.None.Contains(addedComponentType))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        private QueryCheckResult QueryCheckOnAddedComponents(ComponentType[] addedComponentTypes, HashSet<ComponentType> allComponentsTypes, QueryId queryId)
        {
            var query = _queryByQueryId[queryId];
            if (query.None.Count != 0 && query.None.ContainsAny(addedComponentTypes))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        private void UpdateEntitiesByQueryOnAddedComponent(TEntity entity, Dictionary<ComponentType, object> allComponents, ComponentType addedComponentType)
        {
            var allComponentsTypes = new HashSet<ComponentType>(allComponents.Select(GetComponentType));
            foreach (var queryId in GetQueriesByComponentType(addedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (QueryCheckOnAddedComponent(addedComponentType, allComponentsTypes, queryId))
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

        private void UpdateEntitiesByQueryOnAddedComponents(TEntity entity, Dictionary<ComponentType, object> allComponents, object[] addedComponents)
        {
            var addedComponentTypes = addedComponents.Select(GetComponentType).ToArray();
            var allComponentsTypes = new HashSet<ComponentType>(allComponents.Select(GetComponentType));
            foreach (var queryId in GetQueriesByComponentTypes(addedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (QueryCheckOnAddedComponents(addedComponentTypes, allComponentsTypes, queryId))
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
