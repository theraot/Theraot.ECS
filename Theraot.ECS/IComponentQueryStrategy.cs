using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    public interface IComponentQueryStrategy<TComponentType, TQuery>
    {
        TQuery CreateQuery(HashSet<TComponentType> all, HashSet<TComponentType> any, HashSet<TComponentType> none);

        TComponentType GetType(Type type);

        QueryCheckResult QueryCheck(HashSet<TComponentType> allComponentsTypes, TQuery query);

        QueryCheckResult QueryCheckOnAddedComponent(TComponentType addedComponentType, HashSet<TComponentType> allComponentsTypes, TQuery queryId);

        QueryCheckResult QueryCheckOnAddedComponents(TComponentType[] addedComponentTypes, HashSet<TComponentType> allComponentsTypes, TQuery query);

        IEnumerable<TComponentType> GetRelevantComponentTypes(TQuery query);
    }
}
