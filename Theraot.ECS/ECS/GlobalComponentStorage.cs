using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class GlobalComponentStorage<TComponentType>
    {
        private readonly Dictionary<TComponentType, Type> _actualTypeByComponentType;

        private readonly Dictionary<Type, IHasIndexedRemove> _globalComponentIndex;

        public GlobalComponentStorage(IEqualityComparer<TComponentType> componentTypeEqualityComparer)
        {
            _globalComponentIndex = new Dictionary<Type, IHasIndexedRemove>();
            _actualTypeByComponentType = new Dictionary<TComponentType, Type>(componentTypeEqualityComparer);
        }

        public ComponentId Add<TComponent>(TComponent component, TComponentType componentType)
        {
            ThrowIfInvalidType(componentType, component.GetType());
            var storage = GetOrCreateTypedStorage<TComponent>();
            return storage.Add(component);
        }

        public TComponent Get<TComponent>(ComponentId componentId, TComponentType componentType)
        {
            ThrowIfInvalidType(componentType, typeof(TComponent));
            var storage = GetTypedStorage<TComponent>();
            return storage[componentId];
        }

        public ref TComponent GetRef<TComponent>(ComponentId componentId, TComponentType componentType)
        {
            ThrowIfInvalidType(componentType, typeof(TComponent));
            var storage = GetTypedStorage<TComponent>();
            return ref storage.GetRef(componentId);
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _actualTypeByComponentType[componentType];
        }

        public void Remove(ComponentId removedComponentId, TComponentType componentType)
        {
            if (TryGetTypedStorage(out var storage, _actualTypeByComponentType[componentType]))
            {
                storage.Remove(removedComponentId);
            }
        }

        public ComponentId Set<TComponent>(ComponentId id, TComponent component, TComponentType componentType)
        {
            ThrowIfInvalidType(componentType, component.GetType());
            var storage = GetOrCreateTypedStorage<TComponent>();
            return storage.Set(id, component);
        }

        public bool TryGetComponent<TComponent>(ComponentId componentId, TComponentType componentType, out TComponent component)
        {
            ThrowIfInvalidType(componentType, typeof(TComponent));
            component = default;
            return TryGetTypedStorage(out var storage, typeof(TComponent)) && storage is IndexedCollection<TComponent> typedStorage && typedStorage.TryGetValue(componentId, out component);
        }

        public bool TryRegisterComponentType(TComponentType componentType, Type actualType)
        {
            if (_actualTypeByComponentType.TryGetValue(componentType, out var type))
            {
                return type == actualType;
            }

            _actualTypeByComponentType.Add(componentType, actualType);
            return true;
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

        private void ThrowIfInvalidType(TComponentType componentType, Type actualType)
        {
            if (!TryRegisterComponentType(componentType, actualType))
            {
                throw new ArgumentException($"{actualType} does not match {componentType}");
            }
        }

        private bool TryGetTypedStorage(out IHasIndexedRemove storage, Type actualType)
        {
            return _globalComponentIndex.TryGetValue(actualType, out storage);
        }
    }
}