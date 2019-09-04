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
    internal partial class ComponentStorage<TEntityId, TComponentType>
    {
        private readonly Dictionary<TEntityId, CompactDictionary<TComponentType, ComponentId>> _componentsByEntity;

        private readonly IComparer<TComponentType> _componentTypeComparer;

        private readonly ComponentTypeRegistry<TComponentType> _componentTypeRegistry;

        private readonly EntityComponentEventDispatcher<TEntityId, TComponentType> _entityComponentEventDispatcher;

        public ComponentStorage(IEqualityComparer<TComponentType> componentTypeEqualityComparer, IEqualityComparer<TEntityId> entityEqualityComparer, ComponentTypeRegistry<TComponentType> componentTypeRegistry, EntityComponentEventDispatcher<TEntityId, TComponentType> entityComponentEventDispatcher)
        {
            _componentTypeComparer = new ProxyComparer<TComponentType>(componentTypeEqualityComparer);
            _componentsByEntity = new Dictionary<TEntityId, CompactDictionary<TComponentType, ComponentId>>(entityEqualityComparer);
            _componentTypeRegistry = componentTypeRegistry;
            _entityComponentEventDispatcher = entityComponentEventDispatcher;
        }

        public bool RegisterEntity(TEntityId entityId)
        {
            if (_componentsByEntity.ContainsKey(entityId))
            {
                return false;
            }

            var componentIndex = new CompactDictionary<TComponentType, ComponentId>(_componentTypeComparer, 16);
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

        public void SetComponent<TComponentValue>(TEntityId entityId, TComponentType componentType, TComponentValue component)
        {
            if (BufferSetComponent(entityId, componentType, component))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entityId];
            if
            (
                entityComponentStorage.Set
                (
                    componentType,
                    key => _componentTypeRegistry.GetOrCreateTypedStorage<TComponentValue>(key).Add(component),
                    pair => _componentTypeRegistry.GetOrCreateTypedStorage<TComponentValue>(pair.Key).Update(pair.Value, component)
                )
            )
            {
                _entityComponentEventDispatcher.NotifyAddedComponents(entityId, new[] { componentType });
            }
        }

        public bool TryGetComponent<TComponentValue>(TEntityId entityId, TComponentType componentType, out TComponentValue component)
        {
            component = default;
            return _componentsByEntity.TryGetValue(entityId, out var entityComponentStorage)
                   && entityComponentStorage.TryGetValue(componentType, out var componentId)
                   && _componentTypeRegistry.TryGetTypedStorage<TComponentValue>(componentType, out var typedComponentStorage)
                   && typedComponentStorage.TryGetValue(componentId, out component);
        }

        public void UnsetComponent(TEntityId entityId, TComponentType componentType)
        {
            if (BufferUnsetComponent(entityId, componentType))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entityId];
            if (!entityComponentStorage.Remove(componentType, out var removedComponentId))
            {
                return;
            }

            if (_componentTypeRegistry.TryGetStorage(componentType, out var componentStorage))
            {
                componentStorage.Remove(removedComponentId);
            }
            _entityComponentEventDispatcher.NotifyRemovedComponents(entityId, new[] { componentType });
        }

        public void UnsetComponents(TEntityId entityId, IEnumerable<TComponentType> componentTypes)
        {
            var componentTypeList = EnumerableHelper.AsIList(componentTypes);
            if (BufferUnsetComponent(entityId, componentTypeList))
            {
                return;
            }

            var removedComponentTypes = new List<TComponentType>();
            foreach (var componentType in componentTypeList)
            {
                var entityComponentStorage = _componentsByEntity[entityId];
                if (!entityComponentStorage.Remove(componentType, out var removedComponentId))
                {
                    continue;
                }

                if (_componentTypeRegistry.TryGetStorage(componentType, out var componentStorage))
                {
                    componentStorage.Remove(removedComponentId);
                }
                removedComponentTypes.Add(componentType);
            }

            if (removedComponentTypes.Count > 0)
            {
                _entityComponentEventDispatcher.NotifyRemovedComponents(entityId, removedComponentTypes);
            }
        }
    }

    internal partial class ComponentStorage<TEntityId, TComponentType> : IComponentReferenceAccess<TEntityId, TComponentType>
    {
        public void With<TComponentValue1>(TEntityId entityId, TComponentType componentType1, ActionRef<TEntityId, TComponentValue1> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if (!entityComponentStorage.TryGetValue(componentType1, out var componentId))
            {
                throw new KeyNotFoundException("Component type not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue1>(componentType1).GetRef(componentId)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponentValue1, TComponentValue2>(TEntityId entityId, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntityId, TComponentValue1, TComponentValue2> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentType1, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentType2, out var componentId2)
            )
            {
                throw new KeyNotFoundException("Component type not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue1>(componentType1).GetRef(componentId1),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue2>(componentType2).GetRef(componentId2)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponentValue1, TComponentValue2, TComponentValue3>(TEntityId entityId, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentType1, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentType2, out var componentId2)
                || !entityComponentStorage.TryGetValue(componentType3, out var componentId3)
            )
            {
                throw new KeyNotFoundException("Component type not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue1>(componentType1).GetRef(componentId1),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue2>(componentType2).GetRef(componentId2),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue3>(componentType3).GetRef(componentId3)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4>(TEntityId entityId, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentType1, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentType2, out var componentId2)
                || !entityComponentStorage.TryGetValue(componentType3, out var componentId3)
                || !entityComponentStorage.TryGetValue(componentType4, out var componentId4)
            )
            {
                throw new KeyNotFoundException("Component type not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue1>(componentType1).GetRef(componentId1),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue2>(componentType2).GetRef(componentId2),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue3>(componentType3).GetRef(componentId3),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue4>(componentType4).GetRef(componentId4)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4, TComponentValue5>(TEntityId entityId, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntityId, TComponentValue1, TComponentValue2, TComponentValue3, TComponentValue4, TComponentValue5> callback)
        {
            if (!_componentsByEntity.TryGetValue(entityId, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentType1, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentType2, out var componentId2)
                || !entityComponentStorage.TryGetValue(componentType3, out var componentId3)
                || !entityComponentStorage.TryGetValue(componentType4, out var componentId4)
                || !entityComponentStorage.TryGetValue(componentType5, out var componentId5)
            )
            {
                throw new KeyNotFoundException("Component type not found for the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entityId,
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue1>(componentType1).GetRef(componentId1),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue2>(componentType2).GetRef(componentId2),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue3>(componentType3).GetRef(componentId3),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue4>(componentType4).GetRef(componentId4),
                ref _componentTypeRegistry.GetTypedStorage<TComponentValue5>(componentType5).GetRef(componentId5)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }
    }

    internal partial class ComponentStorage<TEntityId, TComponentType>
    {
        private List<Action> _log;

        public bool BufferSetComponent<TComponentValue>(TEntityId entityId, TComponentType componentType, TComponentValue component)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponent(entityId, componentType, component));
            return true;
        }

        public bool BufferUnsetComponent(TEntityId entityId, IList<TComponentType> componentTypes)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => UnsetComponents(entityId, componentTypes));
            return true;
        }

        public bool BufferUnsetComponent(TEntityId entityId, TComponentType componentType)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => UnsetComponent(entityId, componentType));
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
            foreach (var action in log)
            {
                action();
            }
        }
    }
}