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

        public QueryCheckResult QueryCheck(ISet<ComponentType> allComponentsTypes, TypeHashSetQuery query)
        {
            if
            (
                query.None.Count != 0
                && (query.None.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.None) : query.None.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponent(ComponentType addedComponentType, ISet<ComponentType> allComponentsTypes, TypeHashSetQuery query)
        {
            if (query.None.Count != 0 && query.None.Contains(addedComponentType))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponents(ComponentType[] addedComponentTypes, ISet<ComponentType> allComponentsTypes, TypeHashSetQuery query)
        {
            if (query.None.Count != 0 && query.None.ContainsAny(addedComponentTypes))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                allComponentsTypes.ContainsAll(query.All)
                && (query.Any.Count > allComponentsTypes.Count ? allComponentsTypes.ContainsAny(query.Any) : query.Any.Count == 0 || query.Any.ContainsAny(allComponentsTypes))
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
