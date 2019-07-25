using System;
using System.Collections.Generic;
using System.Linq;
using ComponentType = System.Type;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public class Scope<TEntity>
    {
        private const int Add = 1;
        private const int Noop = 0;
        private const int Remove = -1;

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

        public IEnumerable<TEntity> Query(QueryId query)
        {
            return _entitiesByQueryId[query];
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
            _entitiesByQueryId[queryId] = new HashSet<TEntity>();
            foreach (var componentTypes in new[] { query.All, query.Any, query.None })
            {
                foreach (var componentType in componentTypes)
                {
                    if (!_queryIdsByComponentType.TryGetValue(componentType, out var set))
                    {
                        set = new HashSet<QueryId>();
                        _queryIdsByComponentType.TryAdd(componentType, set);
                    }

                    set.Add(queryId);
                }
            }
            return queryId;
        }

        public void SetComponent<TComponent>(TEntity entity, TComponent component)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponentType = GetComponentType(component);
            if (!allComponents.Set(addedComponentType, component))
            {
                return;
            }
            UpdateEntitiesByQueryOnComponentAdded(entity, allComponents, addedComponentType);
        }

        public void SetComponent<TComponent1, TComponent2>(TEntity entity, TComponent1 component1, TComponent2 component2)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll
            (
                new Component[] { component1, component2 },
                new[] { GetComponentType(component1), GetComponentType(component2) }
            ).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }
            UpdateEntitiesByQueryOnComponentsAdded(entity, allComponents, addedComponents);
        }

        public void SetComponent<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll
            (
                new Component[] { component1, component2, component3 },
                new[] { GetComponentType(component1), GetComponentType(component2), GetComponentType(component3) }
            ).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }
            UpdateEntitiesByQueryOnComponentsAdded(entity, allComponents, addedComponents);
        }

        public void SetComponent<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3, TComponent4 component4)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll
            (
                new Component[] { component1, component2, component3, component4 },
                new[] { GetComponentType(component1), GetComponentType(component2), GetComponentType(component3), GetComponentType(component4) }
            ).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }
            UpdateEntitiesByQueryOnComponentsAdded(entity, allComponents, addedComponents);
        }

        public void SetComponent(TEntity entity, params Component[] components)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll(components, GetComponentType).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }
            UpdateEntitiesByQueryOnComponentsAdded(entity, allComponents, addedComponents);
        }

        private static ComponentType GetComponentType<TComponent>(TComponent component)
        {
            var _ = component;
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
            return Enumerable().SelectMany(query => query).Distinct();

            IEnumerable<IEnumerable<QueryId>> Enumerable()
            {
                foreach (var componentType in componentTypes)
                {
                    yield return GetQueriesByComponentType(componentType);
                }
            }
        }

        private int QueryCheck(ComponentType[] addedComponentTypes, HashSet<ComponentType> allComponentsTypes, QueryId queryId)
        {
            var query = _queryByQueryId[queryId];
            if (query.None.Count != 0 && query.None.ContainsAny(addedComponentTypes))
            {
                // The entity has one of the components it should not have for this query
                return Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this query
                // and at least one of the optional components (if any) for this query
                return Add;
            }
            return Noop;
        }

        private int QueryCheck(ComponentType addedComponentType, HashSet<ComponentType> allComponentsTypes, QueryId queryId)
        {
            var query = _queryByQueryId[queryId];
            if (query.None.Count != 0 && query.None.Contains(addedComponentType))
            {
                // The entity has one of the components it should not have for this query
                return Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this query
                // and at least one of the optional components (if any) for this query
                return Add;
            }
            return Noop;
        }

        private void UpdateEntitiesByQueryOnComponentAdded(TEntity entity, Dictionary<ComponentType, object> allComponents, ComponentType addedComponentType)
        {
            var allComponentsTypes = new HashSet<ComponentType>(allComponents.Select(GetComponentType));
            foreach (var queryId in GetQueriesByComponentType(addedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (QueryCheck(addedComponentType, allComponentsTypes, queryId))
                {
                    case Remove:
                        set.Remove(entity);
                        break;
                    case Add:
                        set.Add(entity);
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateEntitiesByQueryOnComponentsAdded(TEntity entity, Dictionary<ComponentType, object> allComponents, object[] addedComponents)
        {
            var addedComponentTypes = addedComponents.Select(GetComponentType).ToArray();
            var allComponentsTypes = new HashSet<ComponentType>(allComponents.Select(GetComponentType));
            foreach (var queryId in GetQueriesByComponentTypes(addedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (QueryCheck(addedComponentTypes, allComponentsTypes, queryId))
                {
                    case Remove:
                        set.Remove(entity);
                        break;
                    case Add:
                        set.Add(entity);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
