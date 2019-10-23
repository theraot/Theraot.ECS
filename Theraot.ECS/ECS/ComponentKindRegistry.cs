using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.ECS
{
    internal sealed class ComponentKindRegistry<TComponentKind>
    {
        private readonly Dictionary<Type, IHasRemoveByIntKey> _containerByType;

        private readonly Dictionary<TComponentKind, Type> _typeByComponentKind;

        public ComponentKindRegistry(IEqualityComparer<TComponentKind> componentKindEqualityComparer)
        {
            _typeByComponentKind = new Dictionary<TComponentKind, Type>(componentKindEqualityComparer);
            _containerByType = new Dictionary<Type, IHasRemoveByIntKey>();
        }

        public IIntKeyCollection<TComponentValue> GetOrCreateTypedContainer<TComponentValue>(TComponentKind componentKind)
        {
            var requestedType = typeof(TComponentValue);
            if (_typeByComponentKind.TryGetValue(componentKind, out var registeredType))
            {
                if (registeredType != requestedType)
                {
                    throw new ArgumentException($"{requestedType} does not match the component kind {componentKind}");
                }
            }
            else
            {
                _typeByComponentKind.Add(componentKind, requestedType);
            }

            if (_containerByType.TryGetValue(requestedType, out var result))
            {
                return (IIntKeyCollection<TComponentValue>)result;
            }

            result = new IntKeyCollection<TComponentValue>(16);
            _containerByType[requestedType] = result;
            return (IIntKeyCollection<TComponentValue>)result;
        }

        public Type GetRegisteredType(TComponentKind componentKind)
        {
            return _typeByComponentKind[componentKind];
        }

        public IIntKeyCollection<TComponentValue> GetTypedContainer<TComponentValue>(TComponentKind componentKind)
        {
            var requestedType = typeof(TComponentValue);
            if (!_typeByComponentKind.TryGetValue(componentKind, out var registeredType))
            {
                throw new KeyNotFoundException("Component not stored");
            }

            if (registeredType != requestedType)
            {
                throw new ArgumentException($"{requestedType} does not match {componentKind}");
            }

            if (_containerByType.TryGetValue(requestedType, out var result))
            {
                return (IIntKeyCollection<TComponentValue>)result;
            }

            throw new KeyNotFoundException("Component not stored");
        }

        public bool TryGetContainer(TComponentKind componentKind, out IHasRemoveByIntKey typedComponentContainer)
        {
            typedComponentContainer = default!;
            return _typeByComponentKind.TryGetValue(componentKind, out var type)
                   && _containerByType.TryGetValue(type, out typedComponentContainer);
        }

        public bool TryGetTypedContainer<TComponentValue>(TComponentKind componentKind, out IIntKeyCollection<TComponentValue> typedComponentContainer)
        {
            var requestedType = typeof(TComponentValue);
            if (!_typeByComponentKind.TryGetValue(componentKind, out var registeredType))
            {
                typedComponentContainer = default!;
                return false;
            }

            if (registeredType != requestedType)
            {
                throw new ArgumentException($"{requestedType} does not match {componentKind}");
            }

            if (!_containerByType.TryGetValue(requestedType, out var result))
            {
                typedComponentContainer = default!;
                return false;
            }

            typedComponentContainer = (IIntKeyCollection<TComponentValue>)result;
            return true;
        }

        public bool TryRegisterType<TComponentValue>(TComponentKind componentKind, IIntKeyCollection<TComponentValue> container)
        {
            var requestedType = typeof(TComponentValue);
            if (_typeByComponentKind.TryGetValue(componentKind, out _))
            {
                return false;
            }

            _typeByComponentKind.Add(componentKind, requestedType);

            if (_containerByType.ContainsKey(requestedType))
            {
                return false;
            }

            _containerByType[requestedType] = container;

            return true;
        }
    }
}