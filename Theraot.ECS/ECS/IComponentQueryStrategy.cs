using System.Collections.Generic;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public interface IComponentQueryStrategy<TComponentType, TComponentTypeSet>
    {
        QueryId CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);

        QueryCheckResult QueryCheck(TComponentTypeSet allComponentsTypes, QueryId queryId);

        QueryCheckResult QueryCheckOnAddedComponent(TComponentType addedComponentType, TComponentTypeSet allComponentsTypes, QueryId queryId);

        QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<TComponentType> addedComponentTypes, TComponentTypeSet allComponentsTypes, QueryId queryId);

        QueryCheckResult QueryCheckOnRemovedComponent(TComponentType removedComponentType, TComponentTypeSet allComponentsTypes, QueryId queryId);

        QueryCheckResult QueryCheckOnRemovedComponents(IEnumerable<TComponentType> removedComponentTypes, TComponentTypeSet allComponentsTypes, QueryId queryId);

        IComponentTypeManager<TComponentType, TComponentTypeSet> ComponentTypeManager { get; }
    }
}