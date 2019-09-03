using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.ECS
{
    internal sealed class ComponentTypeRegistry<TComponentType>
    {
        private readonly Dictionary<Type, IHasRemoveByIntKey> _indexByActualType;

        private readonly Dictionary<TComponentType, Type> _indexByComponentType;

        public ComponentTypeRegistry(IEqualityComparer<TComponentType> componentTypeEqualityComparer)
        {
            _indexByComponentType = new Dictionary<TComponentType, Type>(componentTypeEqualityComparer);
            _indexByActualType = new Dictionary<Type, IHasRemoveByIntKey>();
        }

        public IIntKeyCollection<TComponent> GetOrCreateTypedStorage<TComponent>(TComponentType componentType)
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
                return (IIntKeyCollection<TComponent>)result;
            }

            result = new IntKeyCollection<TComponent>(16);
            _indexByActualType[actualType] = result;
            return (IIntKeyCollection<TComponent>)result;
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _indexByComponentType[componentType];
        }

        public IIntKeyCollection<TComponent> GetTypedStorage<TComponent>(TComponentType componentType)
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
                return (IIntKeyCollection<TComponent>)result;
            }

            throw new KeyNotFoundException("Component not stored");
        }

        public bool TryGetStorage(TComponentType componentType, out IHasRemoveByIntKey typedComponentStorage)
        {
            typedComponentStorage = default;
            return _indexByComponentType.TryGetValue(componentType, out var type)
                   && _indexByActualType.TryGetValue(type, out typedComponentStorage);
        }

        public bool TryGetTypedStorage<TComponent>(TComponentType componentType, out IIntKeyCollection<TComponent> typedComponentStorage)
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

            typedComponentStorage = (IIntKeyCollection<TComponent>)result;
            return true;
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType, IIntKeyCollection<TComponent> storage)
        {
            var actualType = typeof(TComponent);
            if (_indexByComponentType.TryGetValue(componentType, out _))
            {
                return false;
            }

            _indexByComponentType.Add(componentType, actualType);

            if (_indexByActualType.ContainsKey(actualType))
            {
                return false;
            }

            _indexByActualType[actualType] = storage;

            return true;
        }
    }
}