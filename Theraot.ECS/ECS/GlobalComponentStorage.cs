using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using ComponentId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class GlobalComponentStorage<TComponentType>
    {
        private readonly Dictionary<Type, IHasIndexedRemove> _indexByActualType;

        private readonly Dictionary<TComponentType, Type> _indexByComponentType;

        public GlobalComponentStorage(IEqualityComparer<TComponentType> componentTypeEqualityComparer)
        {
            _indexByComponentType = new Dictionary<TComponentType, Type>(componentTypeEqualityComparer);
            _indexByActualType = new Dictionary<Type, IHasIndexedRemove>();
        }

        public ComponentId AddComponent<TComponent>(TComponent component, TComponentType componentType)
        {
            var typedComponentStorage = GetOrCreateStorage<TComponent>(componentType);
            return ((IndexedCollection<TComponent>)typedComponentStorage).Add(component);
        }

        public TComponent GetComponent<TComponent>(ComponentId componentId, TComponentType componentType)
        {
            var typedComponentStorage = GetStorage<TComponent>(componentType);
            return ((IndexedCollection<TComponent>)typedComponentStorage)[componentId];
        }

        public ref TComponent GetComponentRef<TComponent>(ComponentId componentId, TComponentType componentType)
        {
            var typedComponentStorage = GetStorage<TComponent>(componentType);
            return ref ((IndexedCollection<TComponent>)typedComponentStorage).GetRef(componentId);
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _indexByComponentType[componentType];
        }

        public void RemoveComponent(ComponentId componentId, TComponentType componentType)
        {
            if (_indexByComponentType.TryGetValue(componentType, out var actualType))
            {
                _indexByActualType[actualType].Remove(componentId);
            }
        }

        public void RemoveComponents(List<ComponentId> componentIds, List<TComponentType> componentTypes)
        {
            for (var index = 0; index < componentIds.Count; index++)
            {
                if (_indexByComponentType.TryGetValue(componentTypes[index], out var actualType))
                {
                    _indexByActualType[actualType].Remove(componentIds[index]);
                }
            }
        }

        public bool TryGetComponent<TComponent>(ComponentId componentId, TComponentType componentType, out TComponent component)
        {
            if (TryGetStorage<TComponent>(componentType, out var typedComponentStorage))
            {
                return ((IndexedCollection<TComponent>)typedComponentStorage).TryGetValue(componentId, out component);
            }

            component = default;
            return false;
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType)
        {
            return TryAddStorage<TComponent>(componentType);
        }

        public ComponentId UpdateComponent<TComponent>(ComponentId componentId, TComponent component, TComponentType componentType)
        {
            var typedComponentStorage = GetOrCreateStorage<TComponent>(componentType);
            return ((IndexedCollection<TComponent>)typedComponentStorage).Update(componentId, component);
        }

        private IHasIndexedRemove GetOrCreateStorage<TComponent>(TComponentType componentType)
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

            return GetOrCreateStorage<TComponent>(actualType);
        }

        private IHasIndexedRemove GetOrCreateStorage<TComponent>(Type actualType)
        {
            if (_indexByActualType.TryGetValue(actualType, out var result))
            {
                return result;
            }

            result = new IndexedCollection<TComponent>(16);
            _indexByActualType[actualType] = result;
            return result;
        }

        private IHasIndexedRemove GetStorage<TComponent>(TComponentType componentType)
        {
            var actualType = typeof(TComponent);
            if (_indexByComponentType.TryGetValue(componentType, out var type))
            {
                if (type != actualType)
                {
                    throw new ArgumentException($"{actualType} does not match {componentType}");
                }
            }
            else
            {
                throw new KeyNotFoundException("Component not stored");
            }

            return GetStorage(actualType);
        }

        private IHasIndexedRemove GetStorage(Type actualType)
        {
            if (_indexByActualType.TryGetValue(actualType, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException("Component not stored");
        }

        private bool TryAddStorage<TComponent>(TComponentType componentType)
        {
            var actualType = typeof(TComponent);
            if (_indexByComponentType.TryGetValue(componentType, out var type))
            {
                return type == actualType;
            }

            _indexByComponentType.Add(componentType, actualType);

            return TryAddStorage<TComponent>(actualType);
        }

        private bool TryAddStorage<TComponent>(Type actualType)
        {
            if (!_indexByActualType.ContainsKey(actualType))
            {
                _indexByActualType[actualType] = new IndexedCollection<TComponent>(16);
            }

            return true;
        }

        private bool TryGetStorage<TComponent>(TComponentType componentType, out IHasIndexedRemove typedComponentStorage)
        {
            var actualType = typeof(TComponent);
            if (_indexByComponentType.TryGetValue(componentType, out var type))
            {
                if (type != actualType)
                {
                    throw new ArgumentException($"{actualType} does not match {componentType}");
                }
            }
            else
            {
                typedComponentStorage = default;
                return false;
            }

            return TryGetStorage(actualType, out typedComponentStorage);
        }

        private bool TryGetStorage(Type actualType, out IHasIndexedRemove typedComponentStorage)
        {
            return _indexByActualType.TryGetValue(actualType, out typedComponentStorage);
        }
    }
}