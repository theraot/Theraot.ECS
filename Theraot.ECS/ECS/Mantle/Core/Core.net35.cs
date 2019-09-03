#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections.Generic;

namespace Theraot.ECS.Mantle.Core
{
#if LESSTHAN_NET35

    internal partial class Core<TEntity, TComponentType>
    {
        public bool BufferSetComponents<TComponent>(TEntity entity, IList<TComponentType> componentTypes, Converter<TComponentType, TComponent> componentSelector)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponents(entity, componentTypes, componentSelector));
            return true;
        }

        public void SetComponents<TComponent>(TEntity entity, IEnumerable<TComponentType> componentTypes, Converter<TComponentType, TComponent> componentSelector)
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
                        key => _componentTypeRegistry.AddComponent(componentSelector(key), key),
                        pair => _componentTypeRegistry.UpdateComponent(pair.Value, componentSelector(pair.Key), pair.Key)
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

    internal partial class Core<TEntity, TComponentType>
    {
        public bool BufferSetComponents<TComponent>(TEntity entity, IList<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponents(entity, componentTypes, componentSelector));
            return true;
        }

        public void SetComponents<TComponent>(TEntity entity, IEnumerable<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector)
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
                        key => _componentTypeRegistry.AddComponent(componentSelector(key), key),
                        pair => _componentTypeRegistry.UpdateComponent(pair.Value, componentSelector(pair.Key), pair.Key)
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