#pragma warning disable CA1031 // Do not catch general exception types
#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;

#if LESSTHAN_NET35

using Action = System.Threading.ThreadStart;

#endif

namespace Theraot.ECS
{
    internal partial class ComponentStorage<TEntityId, TComponentKind>
    {
        private readonly IComparer<TComponentKind> _componentKindComparer;

        private readonly ComponentKindRegistry<TComponentKind> _componentKindRegistry;

        private readonly Dictionary<TEntityId, CompactDictionary<TComponentKind, ComponentId>> _componentsByEntity;

        private readonly EntityComponentEventDispatcher<TEntityId, TComponentKind> _entityComponentEventDispatcher;

        public ComponentStorage(IEqualityComparer<TComponentKind> componentKindEqualityComparer, IEqualityComparer<TEntityId> entityEqualityComparer, ComponentKindRegistry<TComponentKind> componentKindRegistry, EntityComponentEventDispatcher<TEntityId, TComponentKind> entityComponentEventDispatcher)
        {
            _componentKindComparer = new ProxyComparer<TComponentKind>(componentKindEqualityComparer);
            _componentsByEntity = new Dictionary<TEntityId, CompactDictionary<TComponentKind, ComponentId>>(entityEqualityComparer);
            _componentKindRegistry = componentKindRegistry;
            _entityComponentEventDispatcher = entityComponentEventDispatcher;
        }

        public bool DestroyEntity(TEntityId entityId)
        {
            UnsetAllComponents(entityId);
            return _componentsByEntity.Remove(entityId);
        }

        public ICollection<TComponentKind> GetComponentKinds(TEntityId entityId)
        {
            return _componentsByEntity[entityId].Keys;
        }

        public bool RegisterEntity(TEntityId entityId)
        {
            if (_componentsByEntity.ContainsKey(entityId))
            {
                return false;
            }

            var componentIndex = new CompactDictionary<TComponentKind, ComponentId>(_componentKindComparer, 16);
            try
            {
                _componentsByEntity.Add(entityId, componentIndex);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public void SetComponent<TComponentValue>(TEntityId entityId, TComponentKind componentKind, TComponentValue component)
        {
            if (BufferSetComponent(entityId, componentKind, component))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entityId];
            if
            (
                entityComponentStorage.Set
                (
                    componentKind,
                    key => _componentKindRegistry.GetOrCreateTypedContainer<TComponentValue>(key).Add(component),
                    pair => _componentKindRegistry.GetOrCreateTypedContainer<TComponentValue>(pair.Key).Update(pair.Value, component)
                )
            )
            {
                _entityComponentEventDispatcher.NotifyAddedComponents(entityId, new[] { componentKind });
            }
        }

        public bool TryGetComponent<TComponentValue>(TEntityId entityId, TComponentKind componentKind, out TComponentValue component)
        {
            component = default!;
            return _componentsByEntity.TryGetValue(entityId, out var entityComponentStorage)
                   && entityComponentStorage.TryGetValue(componentKind, out var componentId)
                   && _componentKindRegistry.TryGetTypedContainer<TComponentValue>(componentKind, out var typedComponentStorage)
                   && typedComponentStorage.TryGetValue(componentId, out component);
        }

        public void UnsetAllComponents(TEntityId entityId)
        {
            var entityComponentStorage = _componentsByEntity[entityId];
            var componentKinds = entityComponentStorage.Keys;
            var componentKindsArray = new TComponentKind[componentKinds.Count];
            componentKinds.CopyTo(componentKindsArray, 0);
            if (BufferUnsetComponent(entityId, componentKindsArray))
            {
                return;
            }

            var removedComponentKinds = new List<TComponentKind>();
            foreach (var componentKind in componentKindsArray)
            {
                if (!entityComponentStorage.Remove(componentKind, out var removedComponentId))
                {
                    continue;
                }

                if (_componentKindRegistry.TryGetContainer(componentKind, out var componentStorage))
                {
                    componentStorage.Remove(removedComponentId);
                }
                removedComponentKinds.Add(componentKind);
            }

            if (removedComponentKinds.Count > 0)
            {
                _entityComponentEventDispatcher.NotifyRemovedComponents(entityId, removedComponentKinds);
            }
        }

        public void UnsetComponent(TEntityId entityId, TComponentKind componentKind)
        {
            if (BufferUnsetComponent(entityId, componentKind))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entityId];
            if (!entityComponentStorage.Remove(componentKind, out var removedComponentId))
            {
                return;
            }

            if (_componentKindRegistry.TryGetContainer(componentKind, out var componentStorage))
            {
                componentStorage.Remove(removedComponentId);
            }
            _entityComponentEventDispatcher.NotifyRemovedComponents(entityId, new[] { componentKind });
        }

        public void UnsetComponents(TEntityId entityId, IEnumerable<TComponentKind> componentKinds)
        {
            var componentKindList = EnumerableHelper.AsIList(componentKinds);
            if (BufferUnsetComponent(entityId, componentKindList))
            {
                return;
            }

            var removedComponentKinds = new List<TComponentKind>();
            var entityComponentStorage = _componentsByEntity[entityId];
            foreach (var componentKind in componentKindList)
            {
                if (!entityComponentStorage.Remove(componentKind, out var removedComponentId))
                {
                    continue;
                }

                if (_componentKindRegistry.TryGetContainer(componentKind, out var componentStorage))
                {
                    componentStorage.Remove(removedComponentId);
                }
                removedComponentKinds.Add(componentKind);
            }

            if (removedComponentKinds.Count > 0)
            {
                _entityComponentEventDispatcher.NotifyRemovedComponents(entityId, removedComponentKinds);
            }
        }
    }

    internal partial class ComponentStorage<TEntityId, TComponentKind> : IComponentReferenceAccess<TEntityId, TComponentKind>
    {
        public void With<TComponentValue1>(TEntityId entityId, TComponentKind componentKind1, ActionRef<TEntityId, TComponentValue1> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if (!entityComponentStorage.TryGetValue(componentKind1, out var componentId))
            {
                throw new KeyNotFoundException("Component kind not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentKindRegistry.GetTypedContainer<TComponentValue1>(componentKind1).GetRef(componentId)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponentValue1, TComponentValue2>(TEntityId entityId, TComponentKind componentKind1, TComponentKind componentKind2, ActionRef<TEntityId, TComponentValue1, TComponentValue2> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentKind1, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentKind2, out var componentId2)
            )
            {
                throw new KeyNotFoundException("Component kind not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentKindRegistry.GetTypedContainer<TComponentValue1>(componentKind1).GetRef(componentId1),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue2>(componentKind2).GetRef(componentId2)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponentValue1, TComponentValue2, TComponentValue3>(TEntityId entityId, TComponentKind componentKind1, TComponentKind componentKind2, TComponentKind componentKind3, ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentKind1, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentKind2, out var componentId2)
                || !entityComponentStorage.TryGetValue(componentKind3, out var componentId3)
            )
            {
                throw new KeyNotFoundException("Component kind not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentKindRegistry.GetTypedContainer<TComponentValue1>(componentKind1).GetRef(componentId1),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue2>(componentKind2).GetRef(componentId2),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue3>(componentKind3).GetRef(componentId3)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4>(TEntityId entityId, TComponentKind componentKind1, TComponentKind componentKind2, TComponentKind componentKind3, TComponentKind componentKind4, ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentKind1, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentKind2, out var componentId2)
                || !entityComponentStorage.TryGetValue(componentKind3, out var componentId3)
                || !entityComponentStorage.TryGetValue(componentKind4, out var componentId4)
            )
            {
                throw new KeyNotFoundException("Component kind not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentKindRegistry.GetTypedContainer<TComponentValue1>(componentKind1).GetRef(componentId1),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue2>(componentKind2).GetRef(componentId2),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue3>(componentKind3).GetRef(componentId3),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue4>(componentKind4).GetRef(componentId4)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4, TComponentValue5>(TEntityId entityId, TComponentKind componentKind1, TComponentKind componentKind2, TComponentKind componentKind3, TComponentKind componentKind4, TComponentKind componentKind5, ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4, TComponentValue5> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentKind1, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentKind2, out var componentId2)
                || !entityComponentStorage.TryGetValue(componentKind3, out var componentId3)
                || !entityComponentStorage.TryGetValue(componentKind4, out var componentId4)
                || !entityComponentStorage.TryGetValue(componentKind5, out var componentId5)
            )
            {
                throw new KeyNotFoundException("Component kind not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentKindRegistry.GetTypedContainer<TComponentValue1>(componentKind1).GetRef(componentId1),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue2>(componentKind2).GetRef(componentId2),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue3>(componentKind3).GetRef(componentId3),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue4>(componentKind4).GetRef(componentId4),
                ref _componentKindRegistry.GetTypedContainer<TComponentValue5>(componentKind5).GetRef(componentId5)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }
    }

    internal partial class ComponentStorage<TEntityId, TComponentKind>
    {
        private List<Action>? _log;

        public bool BufferSetComponent<TComponentValue>(TEntityId entityId, TComponentKind componentKind, TComponentValue component)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponent(entityId, componentKind, component));
            return true;
        }

        public bool BufferUnsetComponent(TEntityId entityId, IList<TComponentKind> componentKinds)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => UnsetComponents(entityId, componentKinds));
            return true;
        }

        public bool BufferUnsetComponent(TEntityId entityId, TComponentKind componentKind)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => UnsetComponent(entityId, componentKind));
            return true;
        }

        public bool CreateBuffer()
        {
            if (_log != null)
            {
                return false;
            }

            _log = new List<Action>();
            return true;
        }

        public void ExecuteBuffer()
        {
            var log = _log;
            _log = null;
            if (log == null)
            {
                return;
            }

            foreach (var action in log)
            {
                action();
            }
        }
    }
}