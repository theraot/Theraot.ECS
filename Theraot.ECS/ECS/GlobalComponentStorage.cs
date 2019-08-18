using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class GlobalComponentStorage
    {
        private readonly Dictionary<Type, object> _globalComponentIndex;

        public GlobalComponentStorage()
        {
            _globalComponentIndex = new Dictionary<Type, object>();
        }

        public ComponentId Add<TComponent>(TComponent component)
        {
            var storage = GetOrCreateTypedStorage<TComponent>();
            return storage.Add(component);
        }

        public TComponent Get<TComponent>(ComponentId componentId)
        {
            var storage = GetTypedStorage<TComponent>();
            return storage[componentId];
        }

        public void Remove<TComponent>(ComponentId removedComponentId)
        {
            if (TryGetTypedStorage<TComponent>(out var storage))
            {
                storage.Remove(removedComponentId);
            }
        }

        public void RemoveAll<TComponent>(List<ComponentId> removedComponentIds)
        {
            if (TryGetTypedStorage<TComponent>(out var storage))
            {
                storage.RemoveAll(removedComponentIds);
            }
        }

        public ComponentId Set<TComponent>(ComponentId id, TComponent component)
        {
            var storage = GetOrCreateTypedStorage<TComponent>();
            return storage.Set(id, component);
        }

        public bool TryGetComponent<TComponent>(ComponentId componentId, out TComponent component)
        {
            component = default;
            return TryGetTypedStorage<TComponent>(out var storage) && storage.TryGetValue(componentId, out component);
        }

        private IndexedCollection<TComponent> GetOrCreateTypedStorage<TComponent>()
        {
            if (_globalComponentIndex.TryGetValue(typeof(TComponent), out var storageObj))
            {
                return (IndexedCollection<TComponent>)storageObj;
            }

            var storage = new IndexedCollection<TComponent>(1024);
            _globalComponentIndex[typeof(TComponent)] = storage;
            return storage;
        }

        private IndexedCollection<TComponent> GetTypedStorage<TComponent>()
        {
            if (_globalComponentIndex.TryGetValue(typeof(TComponent), out var storageObj))
            {
                return (IndexedCollection<TComponent>)storageObj;
            }

            throw new KeyNotFoundException("Component not stored");
        }

        private bool TryGetTypedStorage<TComponent>(out IndexedCollection<TComponent> storage)
        {
            if (_globalComponentIndex.TryGetValue(typeof(TComponent), out var storageObj))
            {
                storage = (IndexedCollection<TComponent>)storageObj;
                return true;
            }

            storage = default;
            return false;
        }
    }
}