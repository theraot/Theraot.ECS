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
            var allComponents = _componentIndex;
            var allComponentsTypes = ComponentTypes;
            var componentId = _globalComponentStorage.Add(component);
            if (!allComponents.Set(componentType, componentId))
            {
                return false;
            }

            _componentTypeManager.Add(allComponentsTypes, componentType);
            return true;
        }

        public bool SetComponents(IList<TComponentType> componentTypes, IList<Component> components, out List<TComponentType> addedComponents)
        {
            var allComponents = _componentIndex;
            var allComponentsTypes = ComponentTypes;
            var componentIds = _globalComponentStorage.AddRange(components);
            addedComponents = allComponents.SetAll(componentTypes, componentIds);
            if (addedComponents.Count == 0)
            {
                return false;
            }

            _componentTypeManager.Add(allComponentsTypes, addedComponents);
            return true;
        }

        public bool TryGetValue(TComponentType componentType, out Component component)
        {
            component = default;
            return _componentIndex.TryGetValue(componentType, out var componentId) && _globalComponentStorage.TryGetValue(componentId, out component);
        }

        public bool UnsetComponent(TComponentType componentType)
        {
            var allComponents = _componentIndex;
            var allComponentsTypes = ComponentTypes;
            if (!allComponents.Remove(componentType))
            {
                return false;
            }

            _componentTypeManager.Remove(allComponentsTypes, componentType);
            return true;
        }

        public bool UnsetComponents(IEnumerable<TComponentType> componentTypes, out List<TComponentType> removedComponents)
        {
            var allComponents = _componentIndex;
            var allComponentsTypes = ComponentTypes;
            removedComponents = allComponents.RemoveAll(componentTypes);
            if (removedComponents.Count == 0)
            {
                return false;
            }

            _componentTypeManager.Remove(allComponentsTypes, removedComponents);
            return true;
        }
    }
}