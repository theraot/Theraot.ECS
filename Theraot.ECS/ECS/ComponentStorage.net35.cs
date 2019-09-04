#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable RECS0096 // Type parameter is never used

using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
#if LESSTHAN_NET35

    internal partial class ComponentStorage<TEntityId, TComponentKind>
    {
        public bool BufferSetComponents<TComponentValue>(TEntityId entityId, IList<TComponentKind> componentKinds, Converter<TComponentKind, TComponentValue> componentSelector)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponents(entityId, componentKinds, componentSelector));
            return true;
        }

        public void SetComponents<TComponentValue>(TEntityId entityId, IEnumerable<TComponentKind> componentKinds, Converter<TComponentKind, TComponentValue> componentSelector)
        {
            var componentKindList = EnumerableHelper.AsIList(componentKinds);
            if (BufferSetComponents(entityId, componentKindList, componentSelector))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entityId];

            var addedComponentKinds = new List<TComponentKind>();
            foreach (var componentKind in componentKindList)
            {
                if
                (
                    entityComponentStorage.Set
                    (
                        componentKind,
                        key => _componentKindRegistry.GetOrCreateTypedContainer<TComponentValue>(key).Add(componentSelector(key)),
                        pair => _componentKindRegistry.GetOrCreateTypedContainer<TComponentValue>(pair.Key).Update(pair.Value, componentSelector(pair.Key))
                    )
                )
                {
                    addedComponentKinds.Add(componentKind);
                }
            }

            if (addedComponentKinds.Count > 0)
            {
                _entityComponentEventDispatcher.NotifyAddedComponents(entityId, addedComponentKinds);
            }
        }
    }

#else

    internal partial class ComponentStorage<TEntityId, TComponentKind>
    {
        public bool BufferSetComponents<TComponentValue>(TEntityId entityId, IList<TComponentKind> componentKinds, Func<TComponentKind, TComponentValue> componentSelector)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(() => SetComponents(entityId, componentKinds, componentSelector));
            return true;
        }

        public void SetComponents<TComponentValue>(TEntityId entityId, IEnumerable<TComponentKind> componentKinds, Func<TComponentKind, TComponentValue> componentSelector)
        {
            var componentKindList = EnumerableHelper.AsIList(componentKinds);
            if (BufferSetComponents(entityId, componentKindList, componentSelector))
            {
                return;
            }

            var entityComponentStorage = _componentsByEntity[entityId];

            var addedComponentKinds = new List<TComponentKind>();
            foreach (var componentKind in componentKindList)
            {
                if
                (
                    entityComponentStorage.Set
                    (
                        componentKind,
                        key => _componentKindRegistry.GetOrCreateTypedContainer<TComponentValue>(key).Add(componentSelector(key)),
                        pair => _componentKindRegistry.GetOrCreateTypedContainer<TComponentValue>(pair.Key).Update(pair.Value, componentSelector(pair.Key))
                    )
                )
                {
                    addedComponentKinds.Add(componentKind);
                }
            }

            if (addedComponentKinds.Count > 0)
            {
                _entityComponentEventDispatcher.NotifyAddedComponents(entityId, addedComponentKinds);
            }
        }
    }

#endif
}