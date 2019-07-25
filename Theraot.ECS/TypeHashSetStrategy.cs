using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    public class TypeHashSetStrategy : IComponentQueryStrategy<Type, TypeHashSetQuery>
    {
        public TypeHashSetQuery CreateQuery(Type[] all, Type[] any, Type[] none)
        {
            return new TypeHashSetQuery(all, any, none);
        }

        public IEnumerable<Type> GetRelevantComponentTypes(TypeHashSetQuery typeHashSetQuery)
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

        public Type GetType(Type type)
        {
            return type;
        }

        public QueryCheckResult QueryCheck(ISet<Type> allComponentsTypes, TypeHashSetQuery typeHashSetQuery)
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

        public QueryCheckResult QueryCheckOnAddedComponent(Type addedComponentType, ISet<Type> allComponentsTypes, TypeHashSetQuery typeHashSetQuery)
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

        public QueryCheckResult QueryCheckOnAddedComponents(Type[] addedComponentTypes, ISet<Type> allComponentsTypes, TypeHashSetQuery typeHashSetQuery)
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
