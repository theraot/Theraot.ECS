using System;
using System.Collections.Generic;
using System.Linq;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed partial class ScopeInternal<TEntity, TComponentType, TComponentTypeSet> : IScope<TEntity, TComponentType>
    {
        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly ScopeCore<TEntity, TComponentType, TComponentTypeSet> _core;

        private readonly Dictionary<QueryId, EntityCollection<TEntity, TComponentType>> _entitiesByQueryId;

        private readonly Func<TEntity> _entityFactory;

        private readonly Dictionary<TComponentType, HashSet<QueryId>> _queryIdsByComponentType;

        private readonly QueryManager<TComponentType, TComponentTypeSet> _queryManager;

        internal ScopeInternal(Func<TEntity> entityFactory, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            _componentTypeManager = componentTypeManager ?? throw new ArgumentNullException(nameof(componentTypeManager));
            var componentTypEqualityComparer = componentTypeManager.ComponentTypEqualityComparer;
            _queryManager = new QueryManager<TComponentType, TComponentTypeSet>(componentTypeManager);
            _entitiesByQueryId = new Dictionary<QueryId, EntityCollection<TEntity, TComponentType>>();
            _queryIdsByComponentType = new Dictionary<TComponentType, HashSet<QueryId>>(componentTypEqualityComparer);
            _core = new ScopeCore<TEntity, TComponentType, TComponentTypeSet>(componentTypEqualityComparer);
        }

        public TEntity CreateEntity()
        {
            var entity = _entityFactory();
            _core.CreateEntity(entity, _componentTypeManager.Create());
            return entity;
        }

        public TComponent GetComponent<TComponent>(TEntity entity, TComponentType componentType)
        {
            if (_core.TryGetComponent<TComponent>(entity, componentType, out var component))
            {
                return component;
            }

            throw new KeyNotFoundException("Entity not found");
        }

        public EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            var allAsICollection = all is ICollection<TComponentType> allCollection ? allCollection : all.ToList();
            var anyAsICollection = any is ICollection<TComponentType> anyCollection ? anyCollection : any.ToList();
            var noneAsICollection = none is ICollection<TComponentType> noneCollection ? noneCollection : none.ToList();
            var queryId = _queryManager.CreateQuery(allAsICollection, anyAsICollection, noneAsICollection);
            if (_entitiesByQueryId.TryGetValue(queryId, out var entityCollection))
            {
                return entityCollection;
            }
            entityCollection = _entitiesByQueryId[queryId] = new EntityCollection<TEntity, TComponentType>(this);
            foreach (var componentType in allAsICollection.Concat(anyAsICollection).Concat(noneAsICollection))
            {
                if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
                {
                    queryIds = new HashSet<QueryId>();
                    _queryIdsByComponentType[componentType] = queryIds;
                }

                queryIds.Add(queryId);
            }

            if (_core.EntityCount == 0)
            {
                return entityCollection;
            }
            foreach (var entity in _core.AllEntities)
            {
                var allComponentTypes = _core.GetComponentTypes(entity);
                if (_queryManager.QueryCheck(allComponentTypes, queryId) == QueryCheckResult.Add)
                {
                    entityCollection.Add(entity);
                }
            }
            return entityCollection;
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _core.GetRegisteredComponentType(componentType);
        }

        public void SetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            if (!_core.SetComponent(entity, componentType, component))
            {
                return;
            }

            var allComponentTypes = _core.GetComponentTypes(entity);
            _componentTypeManager.Add(allComponentTypes, componentType);
            UpdateEntitiesByQueryOnAddedComponent(entity, allComponentTypes, componentType);
        }

        public void SetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            var addedComponents = new List<TComponentType>();
            foreach (var componentType in componentTypes)
            {
                if (_core.SetComponent(entity, componentType, componentSelector(componentType)))
                {
                    addedComponents.Add(componentType);
                }
            }

            if (addedComponents.Count == 0)
            {
                return;
            }

            var allComponentTypes = _core.GetComponentTypes(entity);
            _componentTypeManager.Add(allComponentTypes, addedComponents);
            UpdateEntitiesByQueryOnAddedComponents(entity, allComponentTypes, addedComponents);
        }

        public bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component)
        {
            return _core.TryGetComponent(entity, componentType, out component);
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType)
        {
            return _core.TryRegisterComponentType<TComponent>(componentType);
        }

        public void UnsetComponent(TEntity entity, TComponentType componentType)
        {
            if (!_core.UnsetComponent(entity, componentType))
            {
                return;
            }

            var allComponentTypes = _core.GetComponentTypes(entity);
            _componentTypeManager.Remove(allComponentTypes, componentType);
            UpdateEntitiesByQueryOnRemoveComponent(entity, allComponentTypes, componentType);
        }

        public void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes)
        {
            var removedComponents = new List<TComponentType>();
            foreach (var componentType in componentTypes)
            {
                if (!_core.UnsetComponent(entity, componentType))
                {
                    continue;
                }
                removedComponents.Add(componentType);
            }

            if (removedComponents.Count == 0)
            {
                return;
            }

            var allComponentTypes = _core.GetComponentTypes(entity);
            _componentTypeManager.Remove(allComponentTypes, removedComponents);
            UpdateEntitiesByQueryOnRemoveComponents(entity, allComponentTypes, removedComponents);
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
            var set = new HashSet<QueryId>();
            foreach (var componentType in componentTypes)
            {
                foreach (var queryId in GetQueriesByComponentType(componentType))
                {
                    set.Add(queryId);
                }
            }

            return set;
        }

        private void UpdateEntitiesByQueryOnAddedComponent(TEntity entity, TComponentTypeSet allComponentsTypes, TComponentType addedComponentType)
        {
            foreach (var queryId in GetQueriesByComponentType(addedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_queryManager.QueryCheckOnAddedComponent(addedComponentType, allComponentsTypes, queryId))
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

        private void UpdateEntitiesByQueryOnAddedComponents(TEntity entity, TComponentTypeSet allComponentsTypes, List<TComponentType> addedComponentTypes)
        {
            foreach (var queryId in GetQueriesByComponentTypes(addedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_queryManager.QueryCheckOnAddedComponents(addedComponentTypes, allComponentsTypes, queryId))
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

        private void UpdateEntitiesByQueryOnRemoveComponent(TEntity entity, TComponentTypeSet allComponentsTypes, TComponentType removedComponentType)
        {
            foreach (var queryId in GetQueriesByComponentType(removedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_queryManager.QueryCheckOnRemovedComponent(removedComponentType, allComponentsTypes, queryId))
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
                switch (_queryManager.QueryCheckOnRemovedComponents(removedComponentTypes, allComponentsTypes, queryId))
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

    internal sealed partial class ScopeInternal<TEntity, TComponentType, TComponentTypeSet>
    {
        public void With<TComponent1>(TEntity entity, TComponentType componentType1, ActionRef<TEntity, TComponent1> callback)
        {
            _core.With(entity, componentType1, callback);
        }

        public void With<TComponent1, TComponent2>(TEntity entity, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntity, TComponent1, TComponent2> callback)
        {
            _core.With(entity, componentType1, componentType2, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            _core.With(entity, componentType1, componentType2, componentType3, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            _core.With(entity, componentType1, componentType2, componentType3, componentType4, callback);
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            _core.With(entity, componentType1, componentType2, componentType3, componentType4, componentType5, callback);
        }
    }
}