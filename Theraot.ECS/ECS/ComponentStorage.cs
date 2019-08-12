using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    internal class ComponentStorage<TComponentType, TComponentTypeSet>
    {
        private readonly Dictionary<TComponentType, Component> _dictionary;

        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _manager;

        public ComponentStorage(IComponentTypeManager<TComponentType, TComponentTypeSet> manager)
        {
            _manager = manager;
            _dictionary = new Dictionary<TComponentType, Component>();
            ComponentTypes = _manager.CreateComponentTypeSet(_dictionary);
        }

        public TComponentTypeSet ComponentTypes { get; }

        public bool SetComponent<TComponent>(TComponentType componentType, TComponent component, out TComponentTypeSet allComponentsTypes)
        {
            var allComponents = _dictionary;
            allComponentsTypes = ComponentTypes;
            if (!allComponents.Set(componentType, component))
            {
                return false;
            }

            _manager.SetComponentType(allComponentsTypes, componentType);
            return true;
        }

        public bool SetComponents(IEnumerable<KeyValuePair<TComponentType, Component>> components, out TComponentTypeSet allComponentsTypes, out Dictionary<TComponentType, Component> addedComponents)
        {
            var allComponents = _dictionary;
            allComponentsTypes = ComponentTypes;
            addedComponents = allComponents.SetAll(components);
            if (addedComponents.Count == 0)
            {
                return false;
            }

            _manager.SetComponentTypes(allComponentsTypes, addedComponents.Keys);
            return true;
        }

        public bool TryGetValue(TComponentType componentType, out Component component)
        {
            return _dictionary.TryGetValue(componentType, out component);
        }

        public bool UnsetComponent(TComponentType componentType, out TComponentTypeSet allComponentsTypes)
        {
            var allComponents = _dictionary;
            allComponentsTypes = ComponentTypes;
            if (!allComponents.Remove(componentType))
            {
                return false;
            }

            _manager.UnsetComponentType(allComponentsTypes, componentType);
            return true;
        }

        public bool UnsetComponents(IEnumerable<TComponentType> componentTypes, out TComponentTypeSet allComponentsTypes, out List<TComponentType> removedComponents)
        {
            var allComponents = _dictionary;
            allComponentsTypes = ComponentTypes;
            removedComponents = allComponents.RemoveAll(componentTypes);
            if (removedComponents.Count == 0)
            {
                return false;
            }

            _manager.UnsetComponentTypes(allComponentsTypes, removedComponents);
            return true;
        }
    }
}