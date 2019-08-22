using System;
using System.Collections.Generic;
using System.ComponentModel;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;

namespace Theraot.ECS
{
    internal partial class ScopeCore<TEntity, TComponentType, TComponentTypeSet>
    {
        private readonly Dictionary<TEntity, EntityComponentStorage> _componentsByEntity;

        private readonly IComparer<TComponentType> _componentTypeComparer;

        private readonly GlobalComponentStorage<TComponentType> _globalComponentStorage;

        public ScopeCore(IEqualityComparer<TComponentType> componentTypeEqualityComparer)
        {
            _componentTypeComparer = new ProxyComparer<TComponentType>(componentTypeEqualityComparer);
            _globalComponentStorage = new GlobalComponentStorage<TComponentType>(componentTypeEqualityComparer);
            _componentsByEntity = new Dictionary<TEntity, EntityComponentStorage>();
            _addedComponent = new HashSet<EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>>>();
            _removedComponent = new HashSet<EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>>>();
        }

        public IEnumerable<TEntity> AllEntities => _componentsByEntity.Keys;

        public int EntityCount => _componentsByEntity.Count;

        public void CreateEntity(TEntity entity, TComponentTypeSet componentTypes)
        {
            _componentsByEntity[entity] = new EntityComponentStorage
            (
                componentTypes,
                new CompactDictionary<TComponentType, ComponentId>(_componentTypeComparer, 16)
            );
        }

        public TComponentTypeSet GetComponentTypes(TEntity entity)
        {
            return _componentsByEntity[entity].ComponentTypes;
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _globalComponentStorage.GetRegisteredComponentType(componentType);
        }

        public bool SetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            var entityComponentStorage = _componentsByEntity[entity];
            return entityComponentStorage.ComponentIndex.Set
            (
                componentType,
                key => _globalComponentStorage.AddComponent(component, key),
                (key, id) => _globalComponentStorage.UpdateComponent(id, component, key)
            );
        }

        public bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component)
        {
            if
            (
                _componentsByEntity.TryGetValue(entity, out var entityComponentStorage)
                && entityComponentStorage.ComponentIndex.TryGetValue(componentType, out var componentId)
                && _globalComponentStorage.TryGetComponent(componentId, componentType, out component)
            )
            {
                return true;
            }

            component = default;
            return false;
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType)
        {
            return _globalComponentStorage.TryRegisterComponentType<TComponent>(componentType);
        }

        public bool UnsetComponent(TEntity entity, TComponentType componentType)
        {
            var entityComponentStorage = _componentsByEntity[entity];
            if (!entityComponentStorage.ComponentIndex.Remove(componentType, out var removedComponentId))
            {
                return false;
            }

            _globalComponentStorage.RemoveComponent(removedComponentId, componentType);
            return true;
        }

        private sealed class EntityComponentStorage
        {
            public readonly CompactDictionary<TComponentType, ComponentId> ComponentIndex;

            public readonly TComponentTypeSet ComponentTypes;

            public EntityComponentStorage(TComponentTypeSet componentTypes, CompactDictionary<TComponentType, ComponentId> componentIndex)
            {
                ComponentIndex = componentIndex;
                ComponentTypes = componentTypes;
            }
        }
    }

    internal partial class ScopeCore<TEntity, TComponentType, TComponentTypeSet> : IComponentRefScope<TEntity, TComponentType>
    {
        public void With<TComponent1>(TEntity entity, TComponentType componentType1, ActionRef<TEntity, TComponent1> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if (!entityComponentStorage.ComponentIndex.TryGetValue(componentType1, out var componentId))
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            callback
            (
                entity,
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1)
            );
        }

        public void With<TComponent1, TComponent2>(TEntity entity, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntity, TComponent1, TComponent2> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.ComponentIndex.TryGetValue(componentType1, out var componentId)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType2, out var componentId1)
            )
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            callback
            (
                entity,
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1),
                ref _globalComponentStorage.GetComponentRef<TComponent2>(componentId1, componentType2)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.ComponentIndex.TryGetValue(componentType1, out var componentId)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType2, out var componentId1)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType3, out var componentId2)
            )
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            callback
            (
                entity,
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1),
                ref _globalComponentStorage.GetComponentRef<TComponent2>(componentId1, componentType2),
                ref _globalComponentStorage.GetComponentRef<TComponent3>(componentId2, componentType3)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.ComponentIndex.TryGetValue(componentType1, out var componentId)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType2, out var componentId1)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType3, out var componentId2)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType4, out var componentId3)
            )
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            callback
            (
                entity,
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1),
                ref _globalComponentStorage.GetComponentRef<TComponent2>(componentId1, componentType2),
                ref _globalComponentStorage.GetComponentRef<TComponent3>(componentId2, componentType3),
                ref _globalComponentStorage.GetComponentRef<TComponent4>(componentId3, componentType4)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.ComponentIndex.TryGetValue(componentType1, out var componentId)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType2, out var componentId1)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType3, out var componentId2)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType4, out var componentId3)
                || !entityComponentStorage.ComponentIndex.TryGetValue(componentType5, out var componentId4)
            )
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            callback
            (
                entity,
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1),
                ref _globalComponentStorage.GetComponentRef<TComponent2>(componentId1, componentType2),
                ref _globalComponentStorage.GetComponentRef<TComponent3>(componentId2, componentType3),
                ref _globalComponentStorage.GetComponentRef<TComponent4>(componentId3, componentType4),
                ref _globalComponentStorage.GetComponentRef<TComponent5>(componentId4, componentType5)
            );
        }
    }

    internal partial class ScopeCore<TEntity, TComponentType, TComponentTypeSet>
    {
        private readonly HashSet<EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>>> _addedComponent;
        private readonly HashSet<EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>>> _removedComponent;

        public event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> AddedComponents
        {
            add => _addedComponent.Add(value);
            remove => _addedComponent.Remove(value);
        }

        public event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> RemovedComponents
        {
            add => _removedComponent.Add(value);
            remove => _removedComponent.Remove(value);
        }

        private void OnAddedComponents(TEntity entity, ICollection<TComponentType> componentTypes)
        {
            foreach (var handler in _addedComponent)
            {
                handler.Invoke(this, new EntityComponentsChangeEventArgs<TEntity, TComponentType>(CollectionChangeAction.Add, entity, componentTypes));
            }
        }

        private void OnRemovedComponents(TEntity entity, ICollection<TComponentType> componentTypes)
        {
            foreach (var handler in _removedComponent)
            {
                handler.Invoke(this, new EntityComponentsChangeEventArgs<TEntity, TComponentType>(CollectionChangeAction.Remove, entity, componentTypes));
            }
        }
    }
}