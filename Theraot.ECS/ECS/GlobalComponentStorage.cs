using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class GlobalComponentStorage
    {
        private readonly Dictionary<Type, IHasIndexedRemove> _globalComponentIndex;

        public GlobalComponentStorage()
        {
            _globalComponentIndex = new Dictionary<Type, IHasIndexedRemove>();
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

        public ref TComponent GetRef<TComponent>(ComponentId componentId)
        {
            var storage = GetTypedStorage<TComponent>();
            return ref storage.GetRef(componentId);
        }

        public void Remove(ComponentId removedComponentId, Type actualType)
        {
            if (TryGetTypedStorage(out var storage, actualType))
            {
                storage.Remove(removedComponentId);
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
            return TryGetTypedStorage(out var storage, typeof(TComponent)) && storage is IndexedCollection<TComponent> typedStorage && typedStorage.TryGetValue(componentId, out component);
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

        private bool TryGetTypedStorage(out IHasIndexedRemove storage, Type actualType)
        {
            return _globalComponentIndex.TryGetValue(actualType, out storage);
        }
    }
}