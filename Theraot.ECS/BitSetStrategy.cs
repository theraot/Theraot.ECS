using System;
using System.Collections.Generic;
using ComponentType = System.Int32;

namespace Theraot.ECS
{
    public class BitSetStrategy : IComponentQueryStrategy<ComponentType, BitSetQuery>
    {
        private readonly int _capacity;
        private readonly Dictionary<Type, ComponentType> _componentTypeByType;
        private ComponentType _componentType;

        public BitSetStrategy(int capacity)
        {
            _capacity = capacity;
            _componentTypeByType = new Dictionary<Type, ComponentType>();
        }

        public BitSetQuery CreateQuery(ComponentType[] all, ComponentType[] any, ComponentType[] none)
        {
            return new BitSetQuery(_capacity, all, any, none);
        }

        public IEnumerable<ComponentType> GetRelevantComponentTypes(BitSetQuery query)
        {
            for (var index = 0; index < _capacity; index++)
            {
                if (query.All.Contains(index) || query.Any.Contains(index) || query.None.Contains(index))
                {
                    yield return index;
                }
            }
        }

        public ComponentType GetType(Type type)
        {
            if (_componentTypeByType.TryGetValue(type, out var result))
            {
                return result;
            }

            var componentType = _componentType;
            _componentTypeByType[type] = componentType;
            _componentType++;
            return componentType;
        }

        public QueryCheckResult QueryCheck(ISet<ComponentType> allComponentsTypes, BitSetQuery query)
        {
            if
            (
                query.None.Count != 0
                && (query.None.Count > allComponentsTypes.Count ? query.None.ContainsAny(allComponentsTypes) : allComponentsTypes.ContainsAny(query.None))
            )
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count == 0 || query.Any.Count > allComponentsTypes.Count ? query.Any.ContainsAny(allComponentsTypes) : allComponentsTypes.ContainsAny(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponent(ComponentType addedComponentType, ISet<ComponentType> allComponentsTypes, BitSetQuery query)
        {
            if (query.None.Count != 0 && query.None.Contains(addedComponentType))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count == 0 || query.Any.Count > allComponentsTypes.Count ? query.Any.ContainsAny(allComponentsTypes) : allComponentsTypes.ContainsAny(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponents(ComponentType[] addedComponentTypes, ISet<ComponentType> allComponentsTypes, BitSetQuery query)
        {
            if (query.None.Count != 0 && query.None.ContainsAny(addedComponentTypes))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count == 0 || query.Any.Count > allComponentsTypes.Count ? query.Any.ContainsAny(allComponentsTypes) : allComponentsTypes.ContainsAny(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }
    }
}
