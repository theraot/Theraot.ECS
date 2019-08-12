using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    internal class ComponentStorage<TComponentType, TComponentTypeSet>
    {
        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _manager;

        public ComponentStorage(IComponentTypeManager<TComponentType, TComponentTypeSet> manager)
        {
            _manager = manager;
            Dictionary = new Dictionary<TComponentType, Component>();
            ComponentTypes = _manager.CreateComponentTypeSet(Dictionary);
        }

        public TComponentTypeSet ComponentTypes { get; }
        public Dictionary<TComponentType, Component> Dictionary { get; }

        public bool UnsetComponent(TComponentType componentType, out TComponentTypeSet allComponentsTypes)
        {
            var allComponents = Dictionary;
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
            var allComponents = Dictionary;
            allComponentsTypes = ComponentTypes;
            removedComponents = allComponents.RemoveAll(componentTypes);
            if (removedComponents.Count == 0)
            {
                return false;
            }

            _manager.UnsetComponentTypes(allComponentsTypes, removedComponents);
            return true;
        }

        public bool SetComponent<TComponent>(TComponentType componentType, TComponent component, out TComponentTypeSet allComponentsTypes)
        {
            var allComponents = Dictionary;
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
            var allComponents = Dictionary;
            allComponentsTypes = ComponentTypes;
            addedComponents = allComponents.SetAll(components);
            if (addedComponents.Count == 0)
            {
                return false;
            }

            _manager.SetComponentTypes(allComponentsTypes, addedComponents.Keys);
            return true;
        }
    }
}