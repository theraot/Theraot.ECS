using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    internal sealed class TypeMapping<TComponentType>
    {
        private readonly Dictionary<TComponentType, Type> _actualTypeByComponentType;

        public TypeMapping(IEqualityComparer<TComponentType> componentTypeEqualityComparer)
        {
            _actualTypeByComponentType = new Dictionary<TComponentType, Type>(componentTypeEqualityComparer);
        }

        public Type Get(TComponentType componentType)
        {
            return _actualTypeByComponentType[componentType];
        }

        public bool TryRegister(TComponentType componentType, Type actualType)
        {
            if (_actualTypeByComponentType.TryGetValue(componentType, out var type))
            {
                return type == actualType;
            }

            _actualTypeByComponentType.Add(componentType, actualType);
            return true;
        }
    }
}