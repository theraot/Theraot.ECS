using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using Component = System.Object;
using ComponentId = System.Int32;

namespace Theraot.ECS
{
    internal sealed class EntityComponentStorage<TComponentType, TComponentTypeSet>
    {
        private readonly CompactDictionary<TComponentType, ComponentId> _componentIndex;

        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly GlobalComponentStorage _globalComponentStorage;

        private readonly TypeMapping<TComponentType> _typeMapping;

        public EntityComponentStorage(IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager, GlobalComponentStorage globalComponentStorage, IComparer<TComponentType> componentTypeComparer, TypeMapping<TComponentType> typeMapping)
        {
            _componentTypeManager = componentTypeManager;
            _globalComponentStorage = globalComponentStorage;
            _typeMapping = typeMapping;
            _componentIndex = new CompactDictionary<TComponentType, ComponentId>(componentTypeComparer, 16);
            ComponentTypes = _componentTypeManager.Create();
        }

        public TComponentTypeSet ComponentTypes { get; }

        public TComponent GetComponent<TComponent>(TComponentType componentType)
        {
            ThrowIfInvalidType(componentType, typeof(TComponent));
            if (_componentIndex.TryGetValue(componentType, out var componentId))
            {
                return _globalComponentStorage.Get<TComponent>(componentId);
            }

            throw new KeyNotFoundException("ComponentType not found on the entity");
        }

        public ref TComponent GetComponentRef<TComponent>(TComponentType componentType)
        {
            ThrowIfInvalidType(componentType, typeof(TComponent));
            if (_componentIndex.TryGetValue(componentType, out var componentId))
            {
                return ref _globalComponentStorage.GetRef<TComponent>(componentId);
            }

            throw new KeyNotFoundException("ComponentType not found on the entity");
        }

        public bool SetComponent<TComponent>(TComponentType componentType, TComponent component)
        {
            ThrowIfInvalidType(componentType, typeof(TComponent));
            if
            (
                !_componentIndex.Set
                (
                    componentType,
                    _ => _globalComponentStorage.Add(component),
                    (_, id) => _globalComponentStorage.Set(id, component)
                )
            )
            {
                return false;
            }

            _componentTypeManager.Add(ComponentTypes, componentType);
            return true;
        }

        public bool SetComponents(IEnumerable<TComponentType> componentTypes, Func<TComponentType, Component> componentSelector, out List<TComponentType> addedComponents)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }
            if (componentSelector == null)
            {
                throw new ArgumentNullException(nameof(componentSelector));
            }

            addedComponents = _componentIndex.SetAll
            (
                componentTypes,
                key => Add(key, componentSelector(key)),
                (key, id) => Set(key, id, componentSelector(key))
            );
            if (addedComponents.Count == 0)
            {
                return false;
            }

            _componentTypeManager.Add(ComponentTypes, addedComponents);
            return true;
        }

        public bool TryGetComponent<TComponent>(TComponentType componentType, out TComponent component)
        {
            ThrowIfInvalidType(componentType, typeof(TComponent));
            component = default;
            return _componentIndex.TryGetValue(componentType, out var componentId) && _globalComponentStorage.TryGetComponent(componentId, out component);
        }

        public bool UnsetComponent(TComponentType componentType)
        {
            if (!_componentIndex.Remove(componentType, out var removedComponentId))
            {
                return false;
            }

            _globalComponentStorage.Remove(removedComponentId, _typeMapping.Get(componentType));
            _componentTypeManager.Remove(ComponentTypes, componentType);
            return true;
        }

        public bool UnsetComponents(IEnumerable<TComponentType> componentTypes, out List<TComponentType> removedComponentTypes)
        {
            if (_componentIndex.RemoveAll(componentTypes, out removedComponentTypes, out var removedComponentIds) == 0)
            {
                return false;
            }

            for (var index = 0; index < removedComponentIds.Count; index++)
            {
                _globalComponentStorage.Remove(removedComponentIds[index], _typeMapping.Get(removedComponentTypes[index]));
            }
            _componentTypeManager.Remove(ComponentTypes, removedComponentTypes);
            return true;
        }

        private int Add(TComponentType componentType, Component component)
        {
            ThrowIfInvalidType(componentType, component.GetType());
            return _globalComponentStorage.Add(component);
        }

        private int Set(TComponentType componentType, int id, Component component)
        {
            ThrowIfInvalidType(componentType, component.GetType());
            return _globalComponentStorage.Set(id, component);
        }

        private void ThrowIfInvalidType(TComponentType componentType, Type actualType)
        {
            if (!_typeMapping.TryRegister(componentType, actualType))
            {
                throw new ArgumentException($"{actualType} does not match {componentType}");
            }
        }
    }
}