using System;
using System.Collections.Generic;
using System.Linq;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public static class Scope
    {
        public static IScope<TEntity, TComponentType> CreateScope<TEntity, TComponentType, TComponentTypeSet>(Func<TEntity> entityIdFactory, IComponentQueryStrategy<TComponentType, TComponentTypeSet> strategy)
        {
            return new Scope<TEntity, TComponentType, TComponentTypeSet>(entityIdFactory, strategy);
        }
    }

    public sealed partial class Scope<TEntity, TComponentType, TComponentTypeSet> : IScope<TEntity, TComponentType>
    {
        private readonly Dictionary<TEntity, Dictionary<TComponentType, Component>> _componentsByEntity;

        private readonly Dictionary<TEntity, TComponentTypeSet> _componentTypesByEntity;

        private readonly Dictionary<QueryId, HashSet<TEntity>> _entitiesByQueryId;

        private readonly Func<TEntity> _entityFactory;

        private readonly Dictionary<TComponentType, HashSet<QueryId>> _queryIdsByComponentType;

        private readonly IComponentQueryStrategy<TComponentType, TComponentTypeSet> _strategy;

        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _manager;

        internal Scope(Func<TEntity> entityFactory, IComponentQueryStrategy<TComponentType, TComponentTypeSet> strategy)
        {
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _manager = _strategy.ComponentTypeManager;
            _componentsByEntity = new Dictionary<TEntity, Dictionary<TComponentType, Component>>();
            _componentTypesByEntity = new Dictionary<TEntity, TComponentTypeSet>();
            _entitiesByQueryId = new Dictionary<QueryId, HashSet<TEntity>>();
            _queryIdsByComponentType = new Dictionary<TComponentType, HashSet<QueryId>>();
        }

        public TEntity CreateEntity()
        {
            var entity = _entityFactory();
            var dictionary = new Dictionary<TComponentType, Component>();
            _componentsByEntity[entity] = dictionary;
            _componentTypesByEntity[entity] = _manager.CreateComponentTypeSet(dictionary);
            return entity;
        }

        public QueryId CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            var allArray = all.ToArray();
            var anyArray = any.ToArray();
            var noneArray = none.ToArray();
            var queryId = _strategy.CreateQuery(allArray, anyArray, noneArray);
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

            if (_componentTypesByEntity.Count == 0)
            {
                return queryId;
            }
            foreach (var entity in _componentTypesByEntity.Keys)
            {
                var componentTypes = _componentTypesByEntity[entity];
                if (_strategy.QueryCheck(componentTypes, queryId) == QueryCheckResult.Add)
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

        public void UnsetComponent(TEntity entity, TComponentType componentType)
        {
            var allComponents = _componentsByEntity[entity];
            if (!allComponents.Remove(componentType))
            {
                return;
            }

            var allComponentsTypes = _componentTypesByEntity[entity];
            _manager.UnsetComponentType(allComponentsTypes, componentType);
            UpdateEntitiesByQueryOnRemoveComponent(entity, allComponentsTypes, componentType);
        }

        public void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes)
        {
            var allComponents = _componentsByEntity[entity];
            var removedComponents = allComponents.RemoveAll(componentTypes);
            if (removedComponents.Count == 0)
            {
                return;
            }

            var allComponentsTypes = _componentTypesByEntity[entity];
            _manager.UnsetComponentTypes(allComponentsTypes, removedComponents);
            UpdateEntitiesByQueryOnRemoveComponents(entity, allComponentsTypes, removedComponents);
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

        private void UpdateEntitiesByQueryOnRemoveComponent(TEntity entity, TComponentTypeSet allComponentsTypes, TComponentType removedComponentType)
        {
            foreach (var queryId in GetQueriesByComponentType(removedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_strategy.QueryCheckOnRemovedComponent(removedComponentType, allComponentsTypes, queryId))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entity);
                        break;

                    case QueryCheckResult.Add:
                        set.Add(entity);
                        break;

                    case QueryCheckResult.Noop:
                        break;

                    default:
                        break;
                }
            }
        }

        private void UpdateEntitiesByQueryOnRemoveComponents(TEntity entity, TComponentTypeSet allComponentsTypes, List<TComponentType> removedComponentTypes)
        {
            foreach (var queryId in GetQueriesByComponentTypes(removedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_strategy.QueryCheckOnRemovedComponents(removedComponentTypes, allComponentsTypes, queryId))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entity);
                        break;

                    case QueryCheckResult.Add:
                        set.Add(entity);
                        break;

                    case QueryCheckResult.Noop:
                        break;

                    default:
                        break;
                }
            }
        }
    }
}