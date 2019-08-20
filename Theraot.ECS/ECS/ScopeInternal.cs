using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections.Specialized;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class ScopeInternal<TEntity, TComponentType, TComponentTypeSet> : IScope<TEntity, TComponentType>, IComponentRefSource<TEntity, TComponentType>
    {
        private readonly Dictionary<TEntity, EntityComponentStorage<TComponentType, TComponentTypeSet>> _componentsByEntity;

        private readonly IComparer<TComponentType> _componentTypeComparer;

        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly Dictionary<QueryId, EntityCollection<TEntity, TComponentType>> _entitiesByQueryId;

        private readonly Func<TEntity> _entityFactory;

        private readonly GlobalComponentStorage<TComponentType> _globalComponentStorage;

        private readonly Dictionary<TComponentType, HashSet<QueryId>> _queryIdsByComponentType;

        private readonly QueryManager<TComponentType, TComponentTypeSet> _queryManager;

        internal ScopeInternal(Func<TEntity> entityFactory, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
            _componentTypeManager = componentTypeManager ?? throw new ArgumentNullException(nameof(componentTypeManager));
            var componentTypEqualityComparer = componentTypeManager.ComponentTypEqualityComparer;
            _componentTypeComparer = new ProxyComparer<TComponentType>(componentTypEqualityComparer);
            _queryManager = new QueryManager<TComponentType, TComponentTypeSet>(componentTypeManager);
            _componentsByEntity = new Dictionary<TEntity, EntityComponentStorage<TComponentType, TComponentTypeSet>>();
            _entitiesByQueryId = new Dictionary<QueryId, EntityCollection<TEntity, TComponentType>>();
            _queryIdsByComponentType = new Dictionary<TComponentType, HashSet<QueryId>>(componentTypEqualityComparer);
            _globalComponentStorage = new GlobalComponentStorage<TComponentType>(componentTypEqualityComparer);
        }

        public TEntity CreateEntity()
        {
            var entity = _entityFactory();
            _componentsByEntity[entity] = new EntityComponentStorage<TComponentType, TComponentTypeSet>(_componentTypeManager, _globalComponentStorage, _componentTypeComparer);
            return entity;
        }

        public TComponent GetComponent<TComponent>(TEntity entity, TComponentType componentType)
        {
            if (_componentsByEntity.TryGetValue(entity, out var components))
            {
                return components.GetComponent<TComponent>(componentType);
            }

            throw new KeyNotFoundException("Entity not found");
        }

        public ref TComponent GetComponentRef<TComponent>(TEntity entity, TComponentType componentType)
        {
            if (_componentsByEntity.TryGetValue(entity, out var components))
            {
                return ref components.GetComponentRef<TComponent>(componentType);
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

            if (_componentsByEntity.Count == 0)
            {
                return entityCollection;
            }
            foreach (var entity in _componentsByEntity.Keys)
            {
                var componentTypes = _componentsByEntity[entity].ComponentTypes;
                if (_queryManager.QueryCheck(componentTypes, queryId) == QueryCheckResult.Add)
                {
                    entityCollection.Add(entity);
                }
            }
            return entityCollection;
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _globalComponentStorage.GetRegisteredComponentType(componentType);
        }

        public void SetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            var componentStorage = _componentsByEntity[entity];
            if (componentStorage.SetComponent(componentType, component))
            {
                UpdateEntitiesByQueryOnAddedComponent(entity, componentStorage.ComponentTypes, componentType);
            }
        }

        public void SetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector)
        {
            var componentStorage = _componentsByEntity[entity];
            if (componentStorage.SetComponents(componentTypes, componentSelector, out var addedComponents))
            {
                UpdateEntitiesByQueryOnAddedComponents(entity, componentStorage.ComponentTypes, addedComponents);
            }
        }

        public bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component)
        {
            if (_componentsByEntity.TryGetValue(entity, out var components) && components.TryGetComponent<TComponent>(componentType, out var result))
            {
                component = result;
                return true;
            }
            component = default;
            return false;
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType)
        {
            return _globalComponentStorage.TryRegisterComponentType<TComponent>(componentType);
        }

        public void UnsetComponent(TEntity entity, TComponentType componentType)
        {
            var componentStorage = _componentsByEntity[entity];
            if (componentStorage.UnsetComponent(componentType))
            {
                UpdateEntitiesByQueryOnRemoveComponent(entity, componentStorage.ComponentTypes, componentType);
            }
        }

        public void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes)
        {
            var componentStorage = _componentsByEntity[entity];
            if (componentStorage.UnsetComponents(componentTypes, out var removedComponents))
            {
                UpdateEntitiesByQueryOnRemoveComponents(entity, componentStorage.ComponentTypes, removedComponents);
            }
        }

        public void With<TComponent1>(TEntity entity, TComponentType componentType1, ActionRef<TEntity, TComponent1> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (!_componentsByEntity.TryGetValue(entity, out var components))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref components.GetComponentRef<TComponent1>(componentType1)
            );
        }

        public void With<TComponent1, TComponent2>(TEntity entity, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntity, TComponent1, TComponent2> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (!_componentsByEntity.TryGetValue(entity, out var components))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref components.GetComponentRef<TComponent1>(componentType1),
                ref components.GetComponentRef<TComponent2>(componentType2)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (!_componentsByEntity.TryGetValue(entity, out var components))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref components.GetComponentRef<TComponent1>(componentType1),
                ref components.GetComponentRef<TComponent2>(componentType2),
                ref components.GetComponentRef<TComponent3>(componentType3)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (!_componentsByEntity.TryGetValue(entity, out var components))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref components.GetComponentRef<TComponent1>(componentType1),
                ref components.GetComponentRef<TComponent2>(componentType2),
                ref components.GetComponentRef<TComponent3>(componentType3),
                ref components.GetComponentRef<TComponent4>(componentType4)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (!_componentsByEntity.TryGetValue(entity, out var components))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref components.GetComponentRef<TComponent1>(componentType1),
                ref components.GetComponentRef<TComponent2>(componentType2),
                ref components.GetComponentRef<TComponent3>(componentType3),
                ref components.GetComponentRef<TComponent4>(componentType4),
                ref components.GetComponentRef<TComponent5>(componentType5)
            );
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
}