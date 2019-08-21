﻿using System.Collections.Generic;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class EntityComponentStorage<TComponentType, TComponentTypeSet> : IComponentRef<TComponentType>
    {
        private readonly CompactDictionary<TComponentType, ComponentId> _componentIndex;

        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly GlobalComponentStorage<TComponentType> _globalComponentStorage;

        public EntityComponentStorage(IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager, GlobalComponentStorage<TComponentType> globalComponentStorage, IComparer<TComponentType> componentTypeComparer)
        {
            _componentTypeManager = componentTypeManager;
            _globalComponentStorage = globalComponentStorage;
            _componentIndex = new CompactDictionary<TComponentType, ComponentId>(componentTypeComparer, 16);
            ComponentTypes = _componentTypeManager.Create();
        }

        public TComponentTypeSet ComponentTypes { get; }

        public ref TComponent GetComponentRef<TComponent>(TComponentType componentType)
        {
            if (_componentIndex.TryGetValue(componentType, out var componentId))
            {
                return ref _globalComponentStorage.GetComponentRef<TComponent>(componentId, componentType);
            }

            throw new KeyNotFoundException("ComponentType not found on the entity");
        }

        public bool SetComponent<TComponent>(TComponentType componentType, TComponent component)
        {
            if
            (
                !_componentIndex.Set
                (
                    componentType,
                    key => _globalComponentStorage.AddComponent(component, key),
                    (key, id) => _globalComponentStorage.UpdateComponent(id, component, key)
                )
            )
            {
                return false;
            }

            _componentTypeManager.Add(ComponentTypes, componentType);
            return true;
        }

        public bool TryGetComponent<TComponent>(TComponentType componentType, out TComponent component)
        {
            component = default;
            return _componentIndex.TryGetValue(componentType, out var componentId) && _globalComponentStorage.TryGetComponent(componentId, componentType, out component);
        }

        public bool UnsetComponent(TComponentType componentType)
        {
            if (!_componentIndex.Remove(componentType, out var removedComponentId))
            {
                return false;
            }

            _globalComponentStorage.RemoveComponent(removedComponentId, componentType);
            _componentTypeManager.Remove(ComponentTypes, componentType);
            return true;
        }
    }
}