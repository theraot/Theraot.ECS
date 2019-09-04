#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
#if LESSTHAN_NET35

    internal partial class ComponentStorage<TEntityId, TComponentType>
    {
        public bool BufferSetComponents<TComponent>(TEntityId entity, IList<TComponentType> componentTypes, Converter<TComponentType, TComponent> componentSelector)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponents(entity, componentTypes, componentSelector));
            return true;
        }

        public void SetComponents<TComponent>(TEntityId entity, IEnumerable<TComponentType> componentTypes, Converter<TComponentType, TComponent> componentSelector)
        {
            var componentTypeList = EnumerableHelper.AsIList(componentTypes);
            if (BufferSetComponents(entity, componentTypeList, componentSelector))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entity];

            var addedComponentTypes = new List<TComponentType>();
            foreach (var componentType in componentTypeList)
            {
                if
                (
                    entityComponentStorage.Set
                    (
                        componentType,
                        key => _componentTypeRegistry.GetOrCreateTypedStorage<TComponent>(key).Add(componentSelector(key)),
                        pair => _componentTypeRegistry.GetOrCreateTypedStorage<TComponent>(pair.Key).Update(pair.Value, componentSelector(pair.Key))
                    )
                )
                {
                    addedComponentTypes.Add(componentType);
                }
            }

            if (addedComponentTypes.Count > 0)
            {
                _entityComponentEventDispatcher.NotifyAddedComponents(entity, addedComponentTypes);
            }
        }
    }

#else

    internal partial class ComponentStorage<TEntityId, TComponentType>
    {
        public bool BufferSetComponents<TComponent>(TEntityId entity, IList<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponents(entity, componentTypes, componentSelector));
            return true;
        }

        public void SetComponents<TComponent>(TEntityId entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector)
        {
            var componentTypeList = EnumerableHelper.AsIList(componentTypes);
            if (BufferSetComponents(entity, componentTypeList, componentSelector))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entity];

            var addedComponentTypes = new List<TComponentType>();
            foreach (var componentType in componentTypeList)
            {
                if
                (
                    entityComponentStorage.Set
                    (
                        componentType,
                        key => _componentTypeRegistry.GetOrCreateTypedStorage<TComponent>(key).Add(componentSelector(key)),
                        pair => _componentTypeRegistry.GetOrCreateTypedStorage<TComponent>(pair.Key).Update(pair.Value, componentSelector(pair.Key))
                    )
                )
                {
                    addedComponentTypes.Add(componentType);
                }
            }

            if (addedComponentTypes.Count > 0)
            {
                _entityComponentEventDispatcher.NotifyAddedComponents(entity, addedComponentTypes);
            }
        }
    }

#endif
}