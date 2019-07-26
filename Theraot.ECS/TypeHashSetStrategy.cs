using System;
using System.Collections.Generic;
using Component = System.Object;
using ComponentType = System.Type;
using ComponentTypeSet = Theraot.ECS.DictionaryKeySet<System.Type>;

namespace Theraot.ECS
{
    public sealed class TypeHashSetStrategy : IComponentQueryStrategy<ComponentType, ComponentTypeSet, TypeHashSetQuery>
    {
        public ComponentTypeSet CreateComponentTypeSet(Dictionary<ComponentType, Component> dictionary)
        {
            return ComponentTypeSet.CreateFrom(dictionary);
        }

        public TypeHashSetQuery CreateQuery(IEnumerable<ComponentType> all, IEnumerable<ComponentType> any, IEnumerable<ComponentType> none)
        {
            return new TypeHashSetQuery(all, any, none);
        }

        public IEnumerable<ComponentType> GetRelevantComponentTypes(TypeHashSetQuery query)
        {
            foreach (var componentType in query.All)
            {
                yield return componentType;
            }
            foreach (var componentType in query.Any)
            {
                yield return componentType;
            }
            foreach (var componentType in query.None)
            {
                yield return componentType;
            }
        }

        public ComponentType GetType(Type type)
        {
            return type;
        }

        public QueryCheckResult QueryCheck(ComponentTypeSet allComponentsTypes, TypeHashSetQuery query)
        {
            if
            (
                query.None.Count != 0
                && (query.None.Count > allComponentsTypes.Count ? query.None.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.None))
            )
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.IsSupersetOf(query.All)
                && (query.Any.Count == 0 || query.Any.Count > allComponentsTypes.Count ? query.Any.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponent(ComponentType addedComponentType, ComponentTypeSet allComponentsTypes, TypeHashSetQuery query)
        {
            if (query.None.Count != 0 && query.None.Contains(addedComponentType))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.IsSupersetOf(query.All)
                && (query.Any.Count == 0 || query.Any.Count > allComponentsTypes.Count ? query.Any.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<ComponentType> addedComponentTypes, ComponentTypeSet allComponentsTypes, TypeHashSetQuery query)
        {
            if (query.None.Count != 0 && query.None.Overlaps(addedComponentTypes))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.IsSupersetOf(query.All)
                && (query.Any.Count == 0 || query.Any.Count > allComponentsTypes.Count ? query.Any.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.Any))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public void SetComponentType(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            var _ = componentTypeSet;
        }
    }
}
