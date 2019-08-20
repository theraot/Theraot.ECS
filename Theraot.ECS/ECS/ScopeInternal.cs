using System;
using System.Collections.Generic;
using System.Linq;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed partial class ScopeInternal<TEntity, TComponentType, TComponentTypeSet> : IScope<TEntity, TComponentType>
    {
        private readonly Dictionary<QueryId, EntityCollection<TEntity, TComponentType>> _entitiesByQueryId;

        private readonly Func<TEntity> _entityFactory;

        private readonly ScopeCore<TEntity, TComponentType, TComponentTypeSet> _core;

        private readonly Dictionary<TComponentType, HashSet<QueryId>> _queryIdsByComponentType;

        private readonly QueryManager<TComponentType, TComponentTypeSet> _queryManager;

        internal ScopeInternal(Func<TEntity> entityFactory, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeEqualityComparer)
        {
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            var componentTypeManager = componentTypeEqualityComparer ?? throw new ArgumentNullException(nameof(componentTypeEqualityComparer));
            var componentTypEqualityComparer = componentTypeEqualityComparer.ComponentTypEqualityComparer;
            _queryManager = new QueryManager<TComponentType, TComponentTypeSet>(componentTypeEqualityComparer);
            _entitiesByQueryId = new Dictionary<QueryId, EntityCollection<TEntity, TComponentType>>();
            _queryIdsByComponentType = new Dictionary<TComponentType, HashSet<QueryId>>(componentTypEqualityComparer);
            _core = new ScopeCore<TEntity, TComponentType, TComponentTypeSet>(componentTypEqualityComparer, componentTypeManager);
        }

        public TEntity CreateEntity()
        {
            var entity = _entityFactory();
            _core.CreateEntity(entity);
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
                var componentTypes = _core.GetComponentTypes(entity);
                if (_queryManager.QueryCheck(componentTypes, queryId) == QueryCheckResult.Add)
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
            if (_core.SetComponent(entity, componentType, component))
            {
                UpdateEntitiesByQueryOnAddedComponent(entity, _core.GetComponentTypes(entity), componentType);
            }
        }

        public void SetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector)
        {
            if (_core.SetComponents(entity, componentTypes, componentSelector, out var addedComponents))
            {
                UpdateEntitiesByQueryOnAddedComponents(entity, _core.GetComponentTypes(entity), addedComponents);
            }
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
            if (_core.UnsetComponent(entity, componentType))
            {
                UpdateEntitiesByQueryOnRemoveComponent(entity, _core.GetComponentTypes(entity), componentType);
            }
        }

        public void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes)
        {
            if (_core.UnsetComponents(entity, componentTypes, out var removedComponents))
            {
                UpdateEntitiesByQueryOnRemoveComponents(entity, _core.GetComponentTypes(entity), removedComponents);
            }
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
            if (!_core.TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1)
            );
        }

        public void With<TComponent1, TComponent2>(TEntity entity, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntity, TComponent1, TComponent2> callback)
        {
            if (!_core.TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1),
                ref componentRefSource.GetComponentRef<TComponent2>(componentType2)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            if (!_core.TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1),
                ref componentRefSource.GetComponentRef<TComponent2>(componentType2),
                ref componentRefSource.GetComponentRef<TComponent3>(componentType3)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            if (!_core.TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1),
                ref componentRefSource.GetComponentRef<TComponent2>(componentType2),
                ref componentRefSource.GetComponentRef<TComponent3>(componentType3),
                ref componentRefSource.GetComponentRef<TComponent4>(componentType4)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            if (!_core.TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1),
                ref componentRefSource.GetComponentRef<TComponent2>(componentType2),
                ref componentRefSource.GetComponentRef<TComponent3>(componentType3),
                ref componentRefSource.GetComponentRef<TComponent4>(componentType4),
                ref componentRefSource.GetComponentRef<TComponent5>(componentType5)
            );
        }
    }
}