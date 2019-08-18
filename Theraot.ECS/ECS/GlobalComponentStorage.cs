using System.Collections.Generic;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;
using Component = System.Object;

namespace Theraot.ECS
{
    internal sealed class GlobalComponentStorage
    {
        private readonly IndexedCollection<Component> _globalComponentStorage;

        public GlobalComponentStorage()
        {
            _globalComponentStorage = new IndexedCollection<Component>(1024);
        }

        public ComponentId Add<TComponent>(TComponent component)
        {
            return _globalComponentStorage.Add(component);
        }

        public ComponentId Set<TComponent>(ComponentId id, TComponent component)
        {
            return _globalComponentStorage.Set(id, component);
        }

        public Component Get(ComponentId componentId)
        {
            return _globalComponentStorage[componentId];
        }

        public bool TryGetComponent(ComponentId componentId, out Component component)
        {
            return _globalComponentStorage.TryGetValue(componentId, out component);
        }

        public void Remove(ComponentId removedComponentId)
        {
            _globalComponentStorage.Remove(removedComponentId);
        }

        public void RemoveAll(List<ComponentId> removedComponentIds)
        {
            _globalComponentStorage.RemoveAll(removedComponentIds);
        }
    }
}