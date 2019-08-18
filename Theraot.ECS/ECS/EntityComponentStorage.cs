﻿using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;
using Component = System.Object;

namespace Theraot.ECS
{
    internal sealed class EntityComponentStorage<TComponentType, TComponentTypeSet>
    {
        private readonly CompactDictionary<TComponentType, ComponentId> _componentIndex;

        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly GlobalComponentStorage _globalComponentStorage;

        public EntityComponentStorage(IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager, GlobalComponentStorage globalComponentStorage, IComparer<TComponentType> componentTypeComparer)
        {
            _componentTypeManager = componentTypeManager;
            _globalComponentStorage = globalComponentStorage;
            _componentIndex = new CompactDictionary<TComponentType, ComponentId>(componentTypeComparer, 16);
            ComponentTypes = _componentTypeManager.Create();
        }

        public TComponentTypeSet ComponentTypes { get; }

        public bool SetComponent<TComponent>(TComponentType componentType, TComponent component)
        {
            if
            (
                !_componentIndex.Set
                (
                    componentType,
                    _ => _globalComponentStorage.Add(component),
                    (_, id) => _globalComponentStorage.Set(id, component)
                )
            )
            {
                return false;
            }

            _componentTypeManager.Add(ComponentTypes, componentType);
            return true;
        }

        public bool SetComponents(IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector, out List<TComponentType> addedComponents)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }
            if (componentSelector == null)
            {
                throw new ArgumentNullException(nameof(componentSelector));
            }

            addedComponents = _componentIndex.SetAll
            (
                componentTypes,
                key => _globalComponentStorage.Add(componentSelector(key)),
                (key, id) => _globalComponentStorage.Set(id, componentSelector(key))
            );
            if (addedComponents.Count == 0)
            {
                return false;
            }

            _componentTypeManager.Add(ComponentTypes, addedComponents);
            return true;
        }

        public TComponent GetComponent<TComponent>(TComponentType componentType)
        {
            if (_componentIndex.TryGetValue(componentType, out var componentId))
            {
                return _globalComponentStorage.Get<TComponent>(componentId);
            }

            throw new KeyNotFoundException("ComponentType not found on the entity");
        }

        public bool TryGetComponent(TComponentType componentType, out Component component)
        {
            component = default;
            return _componentIndex.TryGetValue(componentType, out var componentId) && _globalComponentStorage.TryGetComponent(componentId, out component);
        }

        public bool UnsetComponent<TComponent>(TComponentType componentType)
        {
            if (!_componentIndex.Remove(componentType, out var removedComponentId))
            {
                return false;
            }

            _globalComponentStorage.Remove<TComponent>(removedComponentId);
            _componentTypeManager.Remove(ComponentTypes, componentType);
            return true;
        }

        public bool UnsetComponents<TComponent>(IEnumerable<TComponentType> componentTypes, out List<TComponentType> removedComponentTypes)
        {
            if (_componentIndex.RemoveAll(componentTypes, out removedComponentTypes, out var removedComponentIds) == 0)
            {
                return false;
            }

            _globalComponentStorage.RemoveAll<TComponent>(removedComponentIds);
            _componentTypeManager.Remove(ComponentTypes, removedComponentTypes);
            return true;
        }
    }
}