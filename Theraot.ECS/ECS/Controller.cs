#pragma warning disable RECS0096 // Type parameter is never used

using System.Collections.Generic;
using Theraot.ECS.Core;
using Theraot.ECS.Mantle.Queries;
using QueryId = System.Int32;

namespace Theraot.ECS.Mantle
{
    internal sealed class Controller<TEntity, TComponentType, TComponentTypeSet> : IController<TEntity, TComponentType>
    {
        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly Dictionary<TEntity, TComponentTypeSet> _componentTypesByEntity;

        private readonly Dictionary<QueryId, EntityCollection<TEntity, TComponentType>> _entitiesByQueryId;

        private readonly IEqualityComparer<TEntity> _entityEqualityComparer;

        private readonly Dictionary<TComponentType, HashSet<QueryId>> _queryIdsByComponentType;

        private readonly QueryManager<TComponentType, TComponentTypeSet> _queryManager;

        internal Controller(IEqualityComparer<TEntity> entityEqualityComparer, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            _entityEqualityComparer = entityEqualityComparer;
            _componentTypeManager = componentTypeManager;
            var componentTypEqualityComparer = componentTypeManager.ComponentTypEqualityComparer;
            _queryManager = new QueryManager<TComponentType, TComponentTypeSet>(componentTypeManager);
            _entitiesByQueryId = new Dictionary<QueryId, EntityCollection<TEntity, TComponentType>>();
            _queryIdsByComponentType = new Dictionary<TComponentType, HashSet<QueryId>>(componentTypEqualityComparer);
            _componentTypesByEntity = new Dictionary<TEntity, TComponentTypeSet>(_entityEqualityComparer);
        }

        public EntityCollection<TEntity, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none, IComponentReferenceAccess<TEntity, TComponentType> componentReferenceAccess)
        {
            var allAsICollection = EnumerableHelper.AsICollection(all);
            var anyAsICollection = EnumerableHelper.AsICollection(any);
            var noneAsICollection = EnumerableHelper.AsICollection(none);
            var queryId = _queryManager.CreateQuery(allAsICollection, anyAsICollection, noneAsICollection);
            return _entitiesByQueryId.TryGetValue(queryId, out var entityCollection)
                ? entityCollection
                : CreateEntityCollection(EnumerableHelper.Concat(allAsICollection, anyAsICollection, noneAsICollection), queryId, componentReferenceAccess);
        }

        public void RegisterEntity(TEntity entity)
        {
            _componentTypesByEntity[entity] = _componentTypeManager.Create();
        }

        public void SubscribeTo(EntityComponentEventDispatcher<TEntity, TComponentType> entityComponentEventDispatcher)
        {
            entityComponentEventDispatcher.AddedComponents += OnAddedComponents;
            entityComponentEventDispatcher.RemovedComponents += OnRemovedComponents;
        }

        private EntityCollection<TEntity, TComponentType> CreateEntityCollection(IEnumerable<TComponentType> componentTypes, int queryId, IComponentReferenceAccess<TEntity, TComponentType> componentReferenceAccess)
        {
            var entityCollection = _entitiesByQueryId[queryId] = new EntityCollection<TEntity, TComponentType>(componentReferenceAccess, _entityEqualityComparer);
            foreach (var componentType in componentTypes)
            {
                if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
                {
                    queryIds = new HashSet<QueryId>();
                    _queryIdsByComponentType[componentType] = queryIds;
                }

                queryIds.Add(queryId);
            }

            foreach (var entity in _componentTypesByEntity.Keys)
            {
                var allComponentTypes = _componentTypesByEntity[entity];
                if (_queryManager.QueryCheck(allComponentTypes, queryId) == QueryCheckResult.Add)
                {
                    entityCollection.Add(entity);
                }
            }
            return entityCollection;
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

        private void OnAddedComponents(object sender, EntityComponentsChangeEventArgs<TEntity, TComponentType> args)
        {
            var _ = sender;
            var allComponentTypes = _componentTypesByEntity[args.Entity];
            _componentTypeManager.Add(allComponentTypes, args.ComponentTypes);
            UpdateEntitiesByQueryOnAddedComponents(args.Entity, allComponentTypes, args.ComponentTypes);
        }

        private void OnRemovedComponents(object sender, EntityComponentsChangeEventArgs<TEntity, TComponentType> args)
        {
            var _ = sender;
            var allComponentTypes = _componentTypesByEntity[args.Entity];
            _componentTypeManager.Remove(allComponentTypes, args.ComponentTypes);
            UpdateEntitiesByQueryOnRemoveComponents(args.Entity, allComponentTypes, args.ComponentTypes);
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