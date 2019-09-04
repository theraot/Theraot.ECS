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

        public bool RegisterEntity(TEntityId entity)
        {
            if (_componentsByEntity.ContainsKey(entity))
            {
                return false;
            }

            var componentIndex = new CompactDictionary<TComponentType, ComponentId>(_componentTypeComparer, 16);
            try
            {
                _componentsByEntity.Add(entity, componentIndex);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public void SetComponent<TComponent>(TEntityId entity, TComponentType componentType, TComponent component)
        {
            if (BufferSetComponent(entity, componentType, component))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entity];
            if
            (
                entityComponentStorage.Set
                (
                    componentType,
                    key => _componentTypeRegistry.GetOrCreateTypedStorage<TComponent>(key).Add(component),
                    pair => _componentTypeRegistry.GetOrCreateTypedStorage<TComponent>(pair.Key).Update(pair.Value, component)
                )
            )
            {
                _entityComponentEventDispatcher.NotifyAddedComponents(entity, new[] { componentType });
            }
        }

        public bool TryGetComponent<TComponent>(TEntityId entity, TComponentType componentType, out TComponent component)
        {
            component = default;
            return _componentsByEntity.TryGetValue(entity, out var entityComponentStorage)
                   && entityComponentStorage.TryGetValue(componentType, out var componentId)
                   && _componentTypeRegistry.TryGetTypedStorage<TComponent>(componentType, out var typedComponentStorage)
                   && typedComponentStorage.TryGetValue(componentId, out component);
        }

        public void UnsetComponent(TEntityId entity, TComponentType componentType)
        {
            if (BufferUnsetComponent(entity, componentType))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entity];
            if (!entityComponentStorage.Remove(componentType, out var removedComponentId))
            {
                return;
            }

            if (_componentTypeRegistry.TryGetStorage(componentType, out var componentStorage))
            {
                componentStorage.Remove(removedComponentId);
            }
            _entityComponentEventDispatcher.NotifyRemovedComponents(entity, new[] { componentType });
        }

        public void UnsetComponents(TEntityId entity, IEnumerable<TComponentType> componentTypes)
        {
            var componentTypeList = EnumerableHelper.AsIList(componentTypes);
            if (BufferUnsetComponent(entity, componentTypeList))
            {
                return;
            }

            var removedComponentTypes = new List<TComponentType>();
            foreach (var componentType in componentTypeList)
            {
                var entityComponentStorage = _componentsByEntity[entity];
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
                _entityComponentEventDispatcher.NotifyRemovedComponents(entity, removedComponentTypes);
            }
        }
    }

    internal partial class ComponentStorage<TEntityId, TComponentType> : IComponentReferenceAccess<TEntityId, TComponentType>
    {
        public void With<TComponent1>(TEntityId entity, TComponentType componentType1, ActionRef<TEntityId, TComponent1> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if (!entityComponentStorage.TryGetValue(componentType1, out var componentId))
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entity,
                ref _componentTypeRegistry.GetTypedStorage<TComponent1>(componentType1).GetRef(componentId)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponent1, TComponent2>(TEntityId entity, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntityId, TComponent1, TComponent2> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentType1, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentType2, out var componentId2)
            )
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entity,
                ref _componentTypeRegistry.GetTypedStorage<TComponent1>(componentType1).GetRef(componentId1),
                ref _componentTypeRegistry.GetTypedStorage<TComponent2>(componentType2).GetRef(componentId2)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntityId entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntityId, TComponent1, TComponent2, TComponent3> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
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
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entity,
                ref _componentTypeRegistry.GetTypedStorage<TComponent1>(componentType1).GetRef(componentId1),
                ref _componentTypeRegistry.GetTypedStorage<TComponent2>(componentType2).GetRef(componentId2),
                ref _componentTypeRegistry.GetTypedStorage<TComponent3>(componentType3).GetRef(componentId3)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntityId entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
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
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entity,
                ref _componentTypeRegistry.GetTypedStorage<TComponent1>(componentType1).GetRef(componentId1),
                ref _componentTypeRegistry.GetTypedStorage<TComponent2>(componentType2).GetRef(componentId2),
                ref _componentTypeRegistry.GetTypedStorage<TComponent3>(componentType3).GetRef(componentId3),
                ref _componentTypeRegistry.GetTypedStorage<TComponent4>(componentType4).GetRef(componentId4)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntityId entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
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
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entity,
                ref _componentTypeRegistry.GetTypedStorage<TComponent1>(componentType1).GetRef(componentId1),
                ref _componentTypeRegistry.GetTypedStorage<TComponent2>(componentType2).GetRef(componentId2),
                ref _componentTypeRegistry.GetTypedStorage<TComponent3>(componentType3).GetRef(componentId3),
                ref _componentTypeRegistry.GetTypedStorage<TComponent4>(componentType4).GetRef(componentId4),
                ref _componentTypeRegistry.GetTypedStorage<TComponent5>(componentType5).GetRef(componentId5)
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

        public bool BufferSetComponent<TComponent>(TEntityId entity, TComponentType componentType, TComponent component)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponent(entity, componentType, component));
            return true;
        }

        public bool BufferUnsetComponent(TEntityId entity, IList<TComponentType> componentTypes)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => UnsetComponents(entity, componentTypes));
            return true;
        }

        public bool BufferUnsetComponent(TEntityId entity, TComponentType componentType)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => UnsetComponent(entity, componentType));
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