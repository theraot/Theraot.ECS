using System;
using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    public interface IComponentQueryStrategy<TComponentType, TComponentTypeSet, TQuery>
    {
        TComponentTypeSet CreateComponentTypeSet(Dictionary<TComponentType, Component> dictionary);

        TQuery CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);

        IEnumerable<TComponentType> GetRelevantComponentTypes(TQuery query);

        TComponentType GetType(Type type);

        QueryCheckResult QueryCheck(TComponentTypeSet allComponentsTypes, TQuery query);

        QueryCheckResult QueryCheckOnAddedComponent(TComponentType addedComponentType, TComponentTypeSet allComponentsTypes, TQuery query);

        QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<TComponentType> addedComponentTypes, TComponentTypeSet allComponentsTypes, TQuery query);

        void SetComponentType(TComponentTypeSet componentTypeSet, TComponentType componentType);
    }
}
