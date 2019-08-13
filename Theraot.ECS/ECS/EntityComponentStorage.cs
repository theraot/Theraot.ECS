using System.Collections.Generic;
using Theraot.Collections.Specialized;
using Component = System.Object;

namespace Theraot.ECS
{
    internal sealed class EntityComponentStorage<TComponentType, TComponentTypeSet>
    {
        private readonly CacheFriendlyDictionary<TComponentType, Component> _dictionary;

        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        public EntityComponentStorage(IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            _componentTypeManager = componentTypeManager;
            _dictionary = new CacheFriendlyDictionary<TComponentType, Component>(null, 16);
            ComponentTypes = _componentTypeManager.Create(_dictionary.Keys);
        }

        public TComponentTypeSet ComponentTypes { get; }

        public bool SetComponent<TComponent>(TComponentType componentType, TComponent component)
        {
            var allComponents = _dictionary;
            var allComponentsTypes = ComponentTypes;
            if (!allComponents.Set(componentType, component))
            {
                return false;
            }

            _componentTypeManager.Add(allComponentsTypes, componentType);
            return true;
        }

        public bool SetComponents(IEnumerable<KeyValuePair<TComponentType, Component>> components, out List<TComponentType> addedComponents)
        {
            var allComponents = _dictionary;
            var allComponentsTypes = ComponentTypes;
            addedComponents = allComponents.SetAll(components);
            if (addedComponents.Count == 0)
            {
                return false;
            }

            _componentTypeManager.Add(allComponentsTypes, addedComponents);
            return true;
        }

        public bool TryGetValue(TComponentType componentType, out Component component)
        {
            return _dictionary.TryGetValue(componentType, out component);
        }

        public bool UnsetComponent(TComponentType componentType)
        {
            var allComponents = _dictionary;
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
            var allComponents = _dictionary;
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