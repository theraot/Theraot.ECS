#pragma warning disable RECS0096 // Type parameter is never used

using System.Collections.Generic;
using Theraot.ECS.Queries;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class Controller<TEntityId, TComponentType, TComponentTypeSet> : IController<TEntityId, TComponentType>
    {
        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly Dictionary<TEntityId, TComponentTypeSet> _componentTypesByEntity;

        private readonly Dictionary<QueryId, EntityCollection<TEntityId, TComponentType>> _entitiesByQueryId;

        private readonly IEqualityComparer<TEntityId> _entityEqualityComparer;

        private readonly Dictionary<TComponentType, HashSet<QueryId>> _queryIdsByComponentType;

        private readonly QueryManager<TComponentType, TComponentTypeSet> _queryManager;

        internal Controller(IEqualityComparer<TEntityId> entityEqualityComparer, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            _entityEqualityComparer = entityEqualityComparer;
            _componentTypeManager = componentTypeManager;
            var componentTypEqualityComparer = componentTypeManager.ComponentTypEqualityComparer;
            _queryManager = new QueryManager<TComponentType, TComponentTypeSet>(componentTypeManager);
            _entitiesByQueryId = new Dictionary<QueryId, EntityCollection<TEntityId, TComponentType>>();
            _queryIdsByComponentType = new Dictionary<TComponentType, HashSet<QueryId>>(componentTypEqualityComparer);
            _componentTypesByEntity = new Dictionary<TEntityId, TComponentTypeSet>(_entityEqualityComparer);
        }

        public EntityCollection<TEntityId, TComponentType> GetEntityCollection(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none, IComponentReferenceAccess<TEntityId, TComponentType> componentReferenceAccess)
        {
            var allAsICollection = EnumerableHelper.AsICollection(all);
            var anyAsICollection = EnumerableHelper.AsICollection(any);
            var noneAsICollection = EnumerableHelper.AsICollection(none);
            var queryId = _queryManager.CreateQuery(allAsICollection, anyAsICollection, noneAsICollection);
            return _entitiesByQueryId.TryGetValue(queryId, out var entityCollection)
                ? entityCollection
                : CreateEntityCollection(EnumerableHelper.Concat(allAsICollection, anyAsICollection, noneAsICollection), queryId, componentReferenceAccess);
        }

        public void RegisterEntity(TEntityId entityId)
        {
            _componentTypesByEntity[entityId] = _componentTypeManager.Create();
        }

        public void SubscribeTo(EntityComponentEventDispatcher<TEntityId, TComponentType> entityComponentEventDispatcher)
        {
            entityComponentEventDispatcher.AddedComponents += OnAddedComponents;
            entityComponentEventDispatcher.RemovedComponents += OnRemovedComponents;
        }

        private EntityCollection<TEntityId, TComponentType> CreateEntityCollection(IEnumerable<TComponentType> componentTypes, int queryId, IComponentReferenceAccess<TEntityId, TComponentType> componentReferenceAccess)
        {
            var entityCollection = _entitiesByQueryId[queryId] = new EntityCollection<TEntityId, TComponentType>(componentReferenceAccess, _entityEqualityComparer);
            foreach (var componentType in componentTypes)
            {
                if (!_queryIdsByComponentType.TryGetValue(componentType, out var queryIds))
                {
                    queryIds = new HashSet<QueryId>();
                    _queryIdsByComponentType[componentType] = queryIds;
                }

                queryIds.Add(queryId);
            }

            foreach (var entityId in _componentTypesByEntity.Keys)
            {
                var allComponentTypes = _componentTypesByEntity[entityId];
                if (_queryManager.QueryCheck(allComponentTypes, queryId) == QueryCheckResult.Add)
                {
                    entityCollection.Add(entityId);
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

        private void OnAddedComponents(object sender, EntityComponentsChangeEventArgs<TEntityId, TComponentType> args)
        {
            var _ = sender;
            var allComponentTypes = _componentTypesByEntity[args.EntityId];
            _componentTypeManager.Add(allComponentTypes, args.ComponentTypes);
            UpdateEntitiesByQueryOnAddedComponents(args.EntityId, allComponentTypes, args.ComponentTypes);
        }

        private void OnRemovedComponents(object sender, EntityComponentsChangeEventArgs<TEntityId, TComponentType> args)
        {
            var _ = sender;
            var allComponentTypes = _componentTypesByEntity[args.EntityId];
            _componentTypeManager.Remove(allComponentTypes, args.ComponentTypes);
            UpdateEntitiesByQueryOnRemoveComponents(args.EntityId, allComponentTypes, args.ComponentTypes);
        }

        private void UpdateEntitiesByQueryOnAddedComponents(TEntityId entityId, TComponentTypeSet allComponentsTypes, IList<TComponentType> addedComponentTypes)
        {
            foreach (var queryId in GetQueriesByComponentTypes(addedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_queryManager.QueryCheckOnAddedComponents(addedComponentTypes, allComponentsTypes, queryId))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entityId);
                        break;

                    case QueryCheckResult.Add:
                        set.Add(entityId);
                        break;

                    case QueryCheckResult.Noop:
                        break;

                    default:
                        break;
                }
            }
        }

        private void UpdateEntitiesByQueryOnRemoveComponents(TEntityId entityId, TComponentTypeSet allComponentsTypes, IList<TComponentType> removedComponentTypes)
        {
            foreach (var queryId in GetQueriesByComponentTypes(removedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_queryManager.QueryCheckOnRemovedComponents(removedComponentTypes, allComponentsTypes, queryId))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entityId);
                        break;

                    case QueryCheckResult.Add:
                        set.Add(entityId);
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