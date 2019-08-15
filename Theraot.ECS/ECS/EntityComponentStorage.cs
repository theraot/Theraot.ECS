using System;
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

        private readonly IndexedCollection<Component> _globalComponentStorage;

        public EntityComponentStorage(IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager, IndexedCollection<Component> globalComponentStorage)
        {
            _componentTypeManager = componentTypeManager;
            _globalComponentStorage = globalComponentStorage;
            _componentIndex = new CompactDictionary<TComponentType, ComponentId>(componentTypeManager, 16);
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

        public bool SetComponents(IDictionary<TComponentType, Component> components, out List<TComponentType> addedComponents)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            addedComponents = _componentIndex.SetAll
            (
                components.Keys,
                key => _globalComponentStorage.Add(components[key]),
                (key, id) => _globalComponentStorage.Set(id, components[key])
            );
            if (addedComponents.Count == 0)
            {
                return false;
            }

            _componentTypeManager.Add(ComponentTypes, addedComponents);
            return true;
        }

        public bool TryGetValue(TComponentType componentType, out Component component)
        {
            component = default;
            return _componentIndex.TryGetValue(componentType, out var componentId) && _globalComponentStorage.TryGetValue(componentId, out component);
        }

        public bool UnsetComponent(TComponentType componentType)
        {
            if (!_componentIndex.Remove(componentType, out var removedValue))
            {
                return false;
            }

            _globalComponentStorage.Remove(removedValue);
            _componentTypeManager.Remove(ComponentTypes, componentType);
            return true;
        }

        public bool UnsetComponents(IEnumerable<TComponentType> componentTypes, out List<TComponentType> removedComponentTypes)
        {
            if (_componentIndex.RemoveAll(componentTypes, out removedComponentTypes, out var removedComponentIds) == 0)
            {
                return false;
            }

            _globalComponentStorage.RemoveAll(removedComponentIds);
            _componentTypeManager.Remove(ComponentTypes, removedComponentTypes);
            return true;
        }
    }
}