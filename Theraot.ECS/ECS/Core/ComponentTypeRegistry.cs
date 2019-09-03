using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.ECS.Core
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

        public IIndexedCollection<TComponent> GetOrCreateTypedStorage<TComponent>(TComponentType componentType)
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
                return (IIndexedCollection<TComponent>)result;
            }

            result = new IndexedCollection<TComponent>(16);
            _indexByActualType[actualType] = result;
            return (IIndexedCollection<TComponent>)result;
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _indexByComponentType[componentType];
        }

        public IIndexedCollection<TComponent> GetTypedStorage<TComponent>(TComponentType componentType)
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
                return (IIndexedCollection<TComponent>)result;
            }

            throw new KeyNotFoundException("Component not stored");
        }

        public bool TryGetStorage(TComponentType componentType, out IHasIndexedRemove typedComponentStorage)
        {
            typedComponentStorage = default;
            return _indexByComponentType.TryGetValue(componentType, out var type)
                   && _indexByActualType.TryGetValue(type, out typedComponentStorage);
        }

        public bool TryGetTypedStorage<TComponent>(TComponentType componentType, out IIndexedCollection<TComponent> typedComponentStorage)
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

            typedComponentStorage = (IIndexedCollection<TComponent>)result;
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