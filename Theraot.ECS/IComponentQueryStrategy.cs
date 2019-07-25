using System;
using System.Collections.Generic;

namespace Theraot.ECS
{
    public interface IComponentQueryStrategy<TComponentType, TQuery>
    {
        TQuery CreateQuery(TComponentType[] all, TComponentType[] any, TComponentType[] none);

        TComponentType GetType(Type type);

        QueryCheckResult QueryCheck(ISet<TComponentType> allComponentsTypes, TQuery query);

        QueryCheckResult QueryCheckOnAddedComponent(TComponentType addedComponentType, ISet<TComponentType> allComponentsTypes, TQuery query);

        QueryCheckResult QueryCheckOnAddedComponents(TComponentType[] addedComponentTypes, ISet<TComponentType> allComponentsTypes, TQuery query);

        IEnumerable<TComponentType> GetRelevantComponentTypes(TQuery query);
    }
}
