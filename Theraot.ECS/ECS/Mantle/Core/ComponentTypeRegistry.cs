using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.ECS.Mantle.Core
{
    internal sealed class ComponentTypeRegistry<TComponentType>
    {
        private readonly Dictionary<Type, IHasIndexedRemove> _indexByActualType;

        private readonly Dictionary<TComponentType, Type> _indexByComponentType;

        public ComponentTypeRegistry(IEqualityComparer<TComponentType> componentTypeEqualityComparer)
        {
            _indexByComponentType = new Dictionary<TComponentType, Type>(componentTypeEqualityComparer);
            _indexByActualType = new Dictionary<Type, IHasIndexedRemove>();
        }

        public IndexedCollection<TComponent> GetOrCreateTypedStorage<TComponent>(TComponentType componentType)
        {
            var actualType = typeof(TComponent);
            if (_indexByComponentType.TryGetValue(componentType, out var type))
            {
                if (type != actualType)
                {
                    throw new ArgumentException($"{actualType} does not match the component type {componentType}");
                }
            }
            else
            {
                _indexByComponentType.Add(componentType, actualType);
            }

            if (_indexByActualType.TryGetValue(actualType, out var result))
            {
                return (IndexedCollection<TComponent>)result;
            }

            result = new IndexedCollection<TComponent>(16);
            _indexByActualType[actualType] = result;
            return (IndexedCollection<TComponent>)result;
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _indexByComponentType[componentType];
        }

        public IndexedCollection<TComponent> GetTypedStorage<TComponent>(TComponentType componentType)
        {
            var actualType = typeof(TComponent);
            if (!_indexByComponentType.TryGetValue(componentType, out var type))
            {
                throw new KeyNotFoundException("Component not stored");
            }

            if (type != actualType)
            {
                throw new ArgumentException($"{actualType} does not match {componentType}");
            }

            if (_indexByActualType.TryGetValue(actualType, out var result))
            {
                return (IndexedCollection<TComponent>)result;
            }

            throw new KeyNotFoundException("Component not stored");
        }

        public bool TryGetStorage(TComponentType componentType, out IHasIndexedRemove typedComponentStorage)
        {
            typedComponentStorage = default;
            return _indexByComponentType.TryGetValue(componentType, out var type)
                   && _indexByActualType.TryGetValue(type, out typedComponentStorage);
        }

        public bool TryGetTypedStorage<TComponent>(TComponentType componentType, out IndexedCollection<TComponent> typedComponentStorage)
        {
            var actualType = typeof(TComponent);
            if (!_indexByComponentType.TryGetValue(componentType, out var type))
            {
                typedComponentStorage = default;
                return false;
            }

            if (type != actualType)
            {
                throw new ArgumentException($"{actualType} does not match {componentType}");
            }

            if (!_indexByActualType.TryGetValue(actualType, out var result))
            {
                typedComponentStorage = default;
                return false;
            }

            typedComponentStorage = (IndexedCollection<TComponent>)result;
            return true;
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType)
        {
            var actualType = typeof(TComponent);
            if (_indexByComponentType.TryGetValue(componentType, out var type))
            {
                return type == actualType;
            }

            _indexByComponentType.Add(componentType, actualType);

            if (!_indexByActualType.ContainsKey(actualType))
            {
                _indexByActualType[actualType] = new IndexedCollection<TComponent>(16);
            }

            return true;
        }
    }
}