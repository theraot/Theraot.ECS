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

        private readonly Dictionary<QueryId, EntityCollection<TEntityId, TComponentKind>> _entityIdsByQueryId;

        private readonly IEqualityComparer<TEntityId> _entityEqualityComparer;

        private readonly Dictionary<TComponentKind, HashSet<QueryId>> _queryIdsByComponentKind;

        private readonly QueryManager<TComponentKind, TComponentKindSet> _queryManager;

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
            _componentKindsByEntity[entityId] = _componentKindManager.Create();
        }

        public void SubscribeTo(EntityComponentEventDispatcher<TEntityId, TComponentKind> entityComponentEventDispatcher)
        {
            entityComponentEventDispatcher.AddedComponents += OnAddedComponents;
            entityComponentEventDispatcher.RemovedComponents += OnRemovedComponents;
        }

        private EntityCollection<TEntityId, TComponentKind> CreateEntityCollection(IEnumerable<TComponentKind> componentKinds, int queryId, IComponentReferenceAccess<TEntityId, TComponentKind> componentReferenceAccess)
        {
            var entityCollection = _entityIdsByQueryId[queryId] = new EntityCollection<TEntityId, TComponentKind>(componentReferenceAccess, _entityEqualityComparer);
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