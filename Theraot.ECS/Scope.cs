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
        private readonly Dictionary<TEntity, HashSet<Component>> _componentsByEntity;
        private readonly Dictionary<QueryId, HashSet<TEntity>> _entitiesByQuery;
        private readonly Func<TEntity> _entityFactory;
        private readonly Dictionary<QueryId, Query> _queries;
        private readonly Dictionary<ComponentType, HashSet<QueryId>> _queriesByComponentType;
        private int _queryId;

        public Scope(Func<TEntity> entityFactory)
        {
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            _queries = new Dictionary<int, Query>();
            _queriesByComponentType = new Dictionary<Type, HashSet<int>>();
            _entitiesByQuery = new Dictionary<int, HashSet<TEntity>>();
            _componentsByEntity = new Dictionary<TEntity, HashSet<object>>();
        }

        public TEntity CreateEntity()
        {
            var entity = _entityFactory();
            _componentsByEntity[entity] = new HashSet<object>();
            return entity;
        }

        public void RegisterQuery(Query query)
        {
            var queryId = _queryId++; // set and increment afterwards
            _queries.Add(queryId, query);
            if (!_entitiesByQuery.TryAdd(queryId, new HashSet<TEntity>()))
            {
                return;
            }

            foreach (var componentTypes in new[] { query.All, query.Any, query.None })
            {
                foreach (var componentType in componentTypes)
                {
                    if (!_queriesByComponentType.TryGetValue(componentType, out var set))
                    {
                        set = new HashSet<QueryId>();
                        _queriesByComponentType.TryAdd(componentType, set);
                    }

                    set.Add(queryId);
                }
            }
        }

        public void SetComponent<TComponent>(TEntity entity, TComponent component)
        {
            var allComponents = _componentsByEntity[entity];
            if (!allComponents.Add(entity))
            {
                return;
            }
            var addedComponentType = GetComponentType(component);
            var allComponentsTypes = new HashSet<ComponentType>(allComponents.Select(GetComponentType));
            foreach (var queryId in GetQueriesByComponentType(addedComponentType))
            {
                var set = _entitiesByQuery[queryId];
                switch (QueryCheck(addedComponentType, allComponentsTypes, queryId))
                {
                    case -1:
                        set.Remove(entity);
                        break;
                    case 1:
                        set.Add(entity);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetComponent<TComponent>(TEntity entity, params TComponent[] components)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.AddAll(components).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }
            var addedComponentTypes = addedComponents.Select(GetComponentType).ToArray();
            var allComponentsTypes = new HashSet<ComponentType>(allComponents.Select(GetComponentType));
            foreach (var queryId in GetQueriesByComponentTypes(addedComponentTypes))
            {
                var set = _entitiesByQuery[queryId];
                switch (QueryCheck(addedComponentTypes, allComponentsTypes, queryId))
                {
                    case -1:
                        set.Remove(entity);
                        break;
                    case 1:
                        set.Add(entity);
                        break;
                    default:
                        break;
                }
            }
        }

        private static ComponentType GetComponentType<TComponent>(TComponent component)
        {
            return component.GetType();
        }

        private IEnumerable<QueryId> GetQueriesByComponentType(ComponentType componentType)
        {
            if (!_queriesByComponentType.TryGetValue(componentType, out var queryIds))
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

        private int QueryCheck(ComponentType[] addedComponentTypes, HashSet<ComponentType> allComponentsTypes, int queryId)
        {
            var query = _queries[queryId];
            if (query.None.Count != 0 && query.None.ContainsAny(addedComponentTypes))
            {
                // The entity has one of the components it should not have for this query
                return -1;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this query
                // and at least one of the optional components (if any) for this query
                return 1;
            }
            return 0;
        }

        private int QueryCheck(ComponentType addedComponentType, HashSet<ComponentType> allComponentsTypes, int queryId)
        {
            var query = _queries[queryId];
            if (query.None.Count != 0 && query.None.Contains(addedComponentType))
            {
                // The entity has one of the components it should not have for this query
                return -1;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this query
                // and at least one of the optional components (if any) for this query
                return 1;
            }
            return 0;
        }
    }
}
