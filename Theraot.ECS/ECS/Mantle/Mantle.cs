using System;
using System.Collections.Generic;
using Theraot.ECS.Mantle.Core;
using Theraot.ECS.Mantle.Queries;
using QueryId = System.Int32;

namespace Theraot.ECS.Mantle
{
    internal sealed class Mantle<TEntity, TComponentType, TComponentTypeSet> : IMantle<TEntity, TComponentType>
    {
        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly ICore<TEntity, TComponentType, TComponentTypeSet> _core;

        private readonly Dictionary<QueryId, EntityCollection<TEntity, TComponentType>> _entitiesByQueryId;

        private readonly IEqualityComparer<TEntity> _entityEqualityComparer;

        private readonly Dictionary<TComponentType, HashSet<QueryId>> _queryIdsByComponentType;

        private readonly QueryManager<TComponentType, TComponentTypeSet> _queryManager;

        internal Mantle(IEqualityComparer<TEntity> entityEqualityComparer, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            _entityEqualityComparer = entityEqualityComparer;
            _componentTypeManager = componentTypeManager ?? throw new ArgumentNullException(nameof(componentTypeManager));
            var componentTypEqualityComparer = componentTypeManager.ComponentTypEqualityComparer;
            _queryManager = new QueryManager<TComponentType, TComponentTypeSet>(componentTypeManager);
            _entitiesByQueryId = new Dictionary<QueryId, EntityCollection<TEntity, TComponentType>>();
            _queryIdsByComponentType = new Dictionary<TComponentType, HashSet<QueryId>>(componentTypEqualityComparer);
            _core = new Core<TEntity, TComponentType, TComponentTypeSet>(componentTypEqualityComparer, _entityEqualityComparer);
            _core.AddedComponents += Core_AddedComponents;
            _core.RemovedComponents += Core_RemovedComponents;
        }

        public IComponentReferenceAccess<TEntity, TComponentType> GetComponentRef()
        {
            return _core.GetComponentRef();
        }

        public EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none)
        {
            var allAsICollection = EnumerableHelper.AsICollection(all);
            var anyAsICollection = EnumerableHelper.AsICollection(any);
            var noneAsICollection = EnumerableHelper.AsICollection(none);
            var queryId = _queryManager.CreateQuery(allAsICollection, anyAsICollection, noneAsICollection);
            if (_entitiesByQueryId.TryGetValue(queryId, out var entityCollection))
            {
                return entityCollection;
            }
            entityCollection = _entitiesByQueryId[queryId] = new EntityCollection<TEntity, TComponentType>(GetComponentRef(), _entityEqualityComparer);
            foreach (var componentType in EnumerableHelper.Concat(allAsICollection, anyAsICollection, noneAsICollection))
            {
                if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
                {
                    queryIds = new HashSet<QueryId>();
                    _queryIdsByComponentType[componentType] = queryIds;
                }

                queryIds.Add(queryId);
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

        public bool RegisterEntity(TEntity entity)
        {
            return _core.RegisterEntity(entity, _componentTypeManager.Create());
        }

        public void SetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            _core.SetComponent(entity, componentType, component);
        }

        public void SetComponents<TComponent>(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            _core.SetComponents(entity, componentTypes, componentSelector);
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
            _core.UnsetComponent(entity, componentType);
        }

        public void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes)
        {
            _core.UnsetComponents(entity, componentTypes);
        }

        private void Core_AddedComponents(object sender, EntityComponentsChangeEventArgs<TEntity, TComponentType> args)
        {
            var allComponentTypes = _core.GetComponentTypes(args.Entity);
            _componentTypeManager.Add(allComponentTypes, args.ComponentTypes);
            UpdateEntitiesByQueryOnAddedComponents(args.Entity, allComponentTypes, args.ComponentTypes);
        }

        private void Core_RemovedComponents(object sender, EntityComponentsChangeEventArgs<TEntity, TComponentType> args)
        {
            var allComponentTypes = _core.GetComponentTypes(args.Entity);
            _componentTypeManager.Remove(allComponentTypes, args.ComponentTypes);
            UpdateEntitiesByQueryOnRemoveComponents(args.Entity, allComponentTypes, args.ComponentTypes);
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

        private void UpdateEntitiesByQueryOnAddedComponents(TEntity entity, TComponentTypeSet allComponentsTypes, IList<TComponentType> addedComponentTypes)
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

        private void UpdateEntitiesByQueryOnRemoveComponents(TEntity entity, TComponentTypeSet allComponentsTypes, IList<TComponentType> removedComponentTypes)
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