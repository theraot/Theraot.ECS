#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;

#if LESSTHAN_NET35

using Action = System.Threading.ThreadStart;

#endif

namespace Theraot.ECS.Mantle.Core
{
    internal partial class Core<TEntity, TComponentType> : ICore<TEntity, TComponentType>
    {
        private readonly Dictionary<TEntity, CompactDictionary<TComponentType, ComponentId>> _componentsByEntity;

        private readonly IComparer<TComponentType> _componentTypeComparer;

        private readonly GlobalComponentStorage<TComponentType> _globalComponentStorage;

        public Core(IEqualityComparer<TComponentType> componentTypeEqualityComparer, IEqualityComparer<TEntity> entityEqualityComparer, GlobalComponentStorage<TComponentType> globalComponentStorage)
        {
            _componentTypeComparer = new ProxyComparer<TComponentType>(componentTypeEqualityComparer);
            _componentsByEntity = new Dictionary<TEntity, CompactDictionary<TComponentType, ComponentId>>(entityEqualityComparer);
            _globalComponentStorage = globalComponentStorage;
        }

        public IComponentReferenceAccess<TEntity, TComponentType> GetComponentRef()
        {
            return this;
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _globalComponentStorage.GetRegisteredComponentType(componentType);
        }

        public bool RegisterEntity(TEntity entity)
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

        public void SetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
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
                    key => _globalComponentStorage.AddComponent(component, key),
                    pair => _globalComponentStorage.UpdateComponent(pair.Value, component, pair.Key)
                )
            )
            {
                OnAddedComponents(entity, new[] { componentType });
            }
        }

        public bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component)
        {
            if
            (
                _componentsByEntity.TryGetValue(entity, out var entityComponentStorage)
                && entityComponentStorage.TryGetValue(componentType, out var componentId)
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

        public void UnsetComponent(TEntity entity, TComponentType componentType)
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

            _globalComponentStorage.RemoveComponent(removedComponentId, componentType);
            OnRemovedComponents(entity, new[] { componentType });
        }

        public void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes)
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

                _globalComponentStorage.RemoveComponent(removedComponentId, componentType);
                removedComponentTypes.Add(componentType);
            }

            if (removedComponentTypes.Count > 0)
            {
                OnRemovedComponents(entity, removedComponentTypes);
            }
        }
    }

    internal partial class Core<TEntity, TComponentType> : IComponentReferenceAccess<TEntity, TComponentType>
    {
        public void With<TComponent1>(TEntity entity, TComponentType componentType1, ActionRef<TEntity, TComponent1> callback)
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
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponent1, TComponent2>(TEntity entity, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntity, TComponent1, TComponent2> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentType1, out var componentId)
                || !entityComponentStorage.TryGetValue(componentType2, out var componentId1)
            )
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entity,
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1),
                ref _globalComponentStorage.GetComponentRef<TComponent2>(componentId1, componentType2)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentType1, out var componentId)
                || !entityComponentStorage.TryGetValue(componentType2, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentType3, out var componentId2)
            )
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entity,
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1),
                ref _globalComponentStorage.GetComponentRef<TComponent2>(componentId1, componentType2),
                ref _globalComponentStorage.GetComponentRef<TComponent3>(componentId2, componentType3)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentType1, out var componentId)
                || !entityComponentStorage.TryGetValue(componentType2, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentType3, out var componentId2)
                || !entityComponentStorage.TryGetValue(componentType4, out var componentId3)
            )
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entity,
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1),
                ref _globalComponentStorage.GetComponentRef<TComponent2>(componentId1, componentType2),
                ref _globalComponentStorage.GetComponentRef<TComponent3>(componentId2, componentType3),
                ref _globalComponentStorage.GetComponentRef<TComponent4>(componentId3, componentType4)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            if (!_componentsByEntity.TryGetValue(entity, out var entityComponentStorage))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            if
            (
                !entityComponentStorage.TryGetValue(componentType1, out var componentId)
                || !entityComponentStorage.TryGetValue(componentType2, out var componentId1)
                || !entityComponentStorage.TryGetValue(componentType3, out var componentId2)
                || !entityComponentStorage.TryGetValue(componentType4, out var componentId3)
                || !entityComponentStorage.TryGetValue(componentType5, out var componentId4)
            )
            {
                throw new KeyNotFoundException("ComponentType not found on the entity");
            }

            var created = CreateBuffer();

            callback
            (
                entity,
                ref _globalComponentStorage.GetComponentRef<TComponent1>(componentId, componentType1),
                ref _globalComponentStorage.GetComponentRef<TComponent2>(componentId1, componentType2),
                ref _globalComponentStorage.GetComponentRef<TComponent3>(componentId2, componentType3),
                ref _globalComponentStorage.GetComponentRef<TComponent4>(componentId3, componentType4),
                ref _globalComponentStorage.GetComponentRef<TComponent5>(componentId4, componentType5)
            );

            if (created)
            {
                ExecuteBuffer();
            }
        }
    }

    internal partial class Core<TEntity, TComponentType>
    {
        public event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> AddedComponents;

        public event EventHandler<EntityComponentsChangeEventArgs<TEntity, TComponentType>> RemovedComponents;

        private void OnAddedComponents(TEntity entity, IList<TComponentType> componentTypes)
        {
            AddedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntity, TComponentType>(CollectionChangeActionEx.Add, entity, componentTypes));
        }

        private void OnRemovedComponents(TEntity entity, IList<TComponentType> componentTypes)
        {
            RemovedComponents?.Invoke(this, new EntityComponentsChangeEventArgs<TEntity, TComponentType>(CollectionChangeActionEx.Remove, entity, componentTypes));
        }
    }

    internal partial class Core<TEntity, TComponentType>
    {
        private List<Action> _log;

        public bool BufferSetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponent(entity, componentType, component));
            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, IList<TComponentType> componentTypes)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => UnsetComponents(entity, componentTypes));
            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, TComponentType componentType)
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