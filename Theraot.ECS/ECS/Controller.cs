#pragma warning disable RECS0096 // Type parameter is never used

using System.Collections.Generic;
using Theraot.ECS.Queries;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class Controller<TEntityId, TComponentKind, TComponentKindSet> : IController<TEntityId, TComponentKind>
    {
        private readonly IComponentKindManager<TComponentKind, TComponentKindSet> _componentKindManager;

        private readonly Dictionary<TEntityId, TComponentKindSet> _componentKindsByEntity;

        private readonly IEqualityComparer<TEntityId> _entityEqualityComparer;

        private readonly Dictionary<QueryId, EntityCollection<TEntityId, TComponentKind>> _entityIdsByQueryId;

        private readonly Dictionary<TComponentKind, HashSet<QueryId>> _queryIdsByComponentKind;

        private readonly QueryManager<TComponentKind, TComponentKindSet> _queryManager;

        private EntityCollection<TEntityId, TComponentKind>? _allEntities;

        internal Controller(IEqualityComparer<TEntityId> entityEqualityComparer, IComponentKindManager<TComponentKind, TComponentKindSet> componentKindManager)
        {
            _entityEqualityComparer = entityEqualityComparer;
            _componentKindManager = componentKindManager;
            var componentKindEqualityComparer = componentKindManager.ComponentKindEqualityComparer;
            _queryManager = new QueryManager<TComponentKind, TComponentKindSet>(componentKindManager);
            _entityIdsByQueryId = new Dictionary<QueryId, EntityCollection<TEntityId, TComponentKind>>();
            _queryIdsByComponentKind = new Dictionary<TComponentKind, HashSet<QueryId>>(componentKindEqualityComparer);
            _componentKindsByEntity = new Dictionary<TEntityId, TComponentKindSet>(_entityEqualityComparer);
        }

        public void DestroyEntity(TEntityId entityId)
        {
            if (_componentKindsByEntity.Remove(entityId))
            {
                _allEntities?.Remove(entityId);
            }
        }

        public EntityCollection<TEntityId, TComponentKind> GetAllEntities(IComponentReferenceAccess<TEntityId, TComponentKind> componentReferenceAccess)
        {
            if (_allEntities != null)
            {
                return _allEntities;
            }

            _allEntities = new EntityCollection<TEntityId, TComponentKind>(componentReferenceAccess, _componentKindsByEntity.Keys, _ => true, _ => true);
            return _allEntities;
        }

        public EntityCollection<TEntityId, TComponentKind> GetEntityCollection(IEnumerable<TComponentKind> all, IEnumerable<TComponentKind> any, IEnumerable<TComponentKind> none, IComponentReferenceAccess<TEntityId, TComponentKind> componentReferenceAccess)
        {
            var allAsICollection = EnumerableHelper.AsICollection(all);
            var anyAsICollection = EnumerableHelper.AsICollection(any);
            var noneAsICollection = EnumerableHelper.AsICollection(none);
            var queryId = _queryManager.CreateQuery(allAsICollection, anyAsICollection, noneAsICollection);
            return _entityIdsByQueryId.TryGetValue(queryId, out var entityCollection)
                ? entityCollection
                : CreateEntityCollection(EnumerableHelper.Concat(allAsICollection, anyAsICollection, noneAsICollection), queryId, componentReferenceAccess);
        }

        public void RegisterEntity(TEntityId entityId)
        {
            if (_componentKindsByEntity.ContainsKey(entityId))
            {
                return;
            }
            _componentKindsByEntity[entityId] = _componentKindManager.Create();
            _allEntities?.Add(entityId);
        }

        public void SubscribeTo(EntityComponentEventDispatcher<TEntityId, TComponentKind> entityComponentEventDispatcher)
        {
            entityComponentEventDispatcher.AddedComponents += OnAddedComponents;
            entityComponentEventDispatcher.RemovedComponents += OnRemovedComponents;
        }

        private EntityCollection<TEntityId, TComponentKind> CreateEntityCollection(IEnumerable<TComponentKind> componentKinds, int queryId, IComponentReferenceAccess<TEntityId, TComponentKind> componentReferenceAccess)
        {
            var set = new HashSet<TEntityId>(_entityEqualityComparer);
            var entityCollection = _entityIdsByQueryId[queryId] = new EntityCollection<TEntityId, TComponentKind>(componentReferenceAccess, set, set.Add, set.Remove);
            foreach (var componentKind in componentKinds)
            {
                if (!_queryIdsByComponentKind.TryGetValue(componentKind, out var queryIds))
                {
                    queryIds = new HashSet<QueryId>();
                    _queryIdsByComponentKind[componentKind] = queryIds;
                }

                queryIds.Add(queryId);
            }

            foreach (var entityId in _componentKindsByEntity.Keys)
            {
                var allComponentKinds = _componentKindsByEntity[entityId];
                if (_queryManager.QueryCheck(allComponentKinds, queryId) == QueryCheckResult.Add)
                {
                    entityCollection.Add(entityId);
                }
            }
            return entityCollection;
        }

        private IEnumerable<QueryId> GetQueriesByComponentKind(TComponentKind componentKind)
        {
            if (!_queryIdsByComponentKind.TryGetValue(componentKind, out var queryIds))
            {
                return EmptyArray<QueryId>.Instance;
            }
            return queryIds;
        }

        private IEnumerable<QueryId> GetQueriesByComponentKinds(IEnumerable<TComponentKind> componentKinds)
        {
            var set = new HashSet<QueryId>();
            foreach (var componentKind in componentKinds)
            {
                foreach (var queryId in GetQueriesByComponentKind(componentKind))
                {
                    set.Add(queryId);
                }
            }

            return set;
        }

        private void OnAddedComponents(object sender, EntityComponentsChangeEventArgs<TEntityId, TComponentKind> args)
        {
            var _ = sender;
            var allComponentKinds = _componentKindsByEntity[args.EntityId];
            _componentKindManager.Add(allComponentKinds, args.ComponentKinds);
            UpdateEntitiesByQueryOnAddedComponents(args.EntityId, allComponentKinds, args.ComponentKinds);
        }

        private void OnRemovedComponents(object sender, EntityComponentsChangeEventArgs<TEntityId, TComponentKind> args)
        {
            var _ = sender;
            var allComponentKinds = _componentKindsByEntity[args.EntityId];
            _componentKindManager.Remove(allComponentKinds, args.ComponentKinds);
            UpdateEntitiesByQueryOnRemoveComponents(args.EntityId, allComponentKinds, args.ComponentKinds);
        }

        private void UpdateEntitiesByQueryOnAddedComponents(TEntityId entityId, TComponentKindSet allComponentsKinds, IList<TComponentKind> addedComponentKinds)
        {
            foreach (var queryId in GetQueriesByComponentKinds(addedComponentKinds))
            {
                var set = _entityIdsByQueryId[queryId];
                switch (_queryManager.QueryCheckOnAddedComponents(addedComponentKinds, allComponentsKinds, queryId))
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

        private void UpdateEntitiesByQueryOnRemoveComponents(TEntityId entityId, TComponentKindSet allComponentsKinds, IList<TComponentKind> removedComponentKinds)
        {
            foreach (var queryId in GetQueriesByComponentKinds(removedComponentKinds))
            {
                var set = _entityIdsByQueryId[queryId];
                switch (_queryManager.QueryCheckOnRemovedComponents(removedComponentKinds, allComponentsKinds, queryId))
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