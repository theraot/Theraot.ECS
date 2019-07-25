using System;
using System.Collections.Generic;
using ComponentType = System.Type;

namespace Theraot.ECS
{
    public class TypeHashSetStrategy : IComponentQueryStrategy<ComponentType, TypeHashSetQuery>
    {
        public TypeHashSetQuery CreateQuery(ComponentType[] all, ComponentType[] any, ComponentType[] none)
        {
            return new TypeHashSetQuery(all, any, none);
        }

        public IEnumerable<ComponentType> GetRelevantComponentTypes(TypeHashSetQuery typeHashSetQuery)
        {
            foreach (var componentType in typeHashSetQuery.All)
            {
                yield return componentType;
            }
            foreach (var componentType in typeHashSetQuery.Any)
            {
                yield return componentType;
            }
            foreach (var componentType in typeHashSetQuery.None)
            {
                yield return componentType;
            }
        }

        public ComponentType GetType(Type type)
        {
            return type;
        }

        public QueryCheckResult QueryCheck(ISet<ComponentType> allComponentsTypes, TypeHashSetQuery typeHashSetQuery)
        {
            if
            (
                typeHashSetQuery.None.Count != 0
                && (typeHashSetQuery.None.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(typeHashSetQuery.None) : typeHashSetQuery.None.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(typeHashSetQuery.All)
                && (typeHashSetQuery.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(typeHashSetQuery.Any) : typeHashSetQuery.Any.Count == 0 || typeHashSetQuery.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponent(ComponentType addedComponentType, ISet<ComponentType> allComponentsTypes, TypeHashSetQuery typeHashSetQuery)
        {
            if (typeHashSetQuery.None.Count != 0 && typeHashSetQuery.None.Contains(addedComponentType))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(typeHashSetQuery.All)
                && (typeHashSetQuery.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(typeHashSetQuery.Any) : typeHashSetQuery.Any.Count == 0 || typeHashSetQuery.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponents(ComponentType[] addedComponentTypes, ISet<ComponentType> allComponentsTypes, TypeHashSetQuery typeHashSetQuery)
        {
            if (typeHashSetQuery.None.Count != 0 && typeHashSetQuery.None.ContainsAny(addedComponentTypes))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(typeHashSetQuery.All)
                && (typeHashSetQuery.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(typeHashSetQuery.Any) : typeHashSetQuery.Any.Count == 0 || typeHashSetQuery.Any.ContainsAny(allComponentsTypes))
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
