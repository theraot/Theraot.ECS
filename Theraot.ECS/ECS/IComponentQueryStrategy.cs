﻿using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    public interface IComponentQueryStrategy<TComponentType, TComponentTypeSet, TQuery>
    {
        TComponentTypeSet CreateComponentTypeSet(Dictionary<TComponentType, Component> dictionary);

        TQuery CreateQuery(IEnumerable<TComponentType> all, IEnumerable<TComponentType> any, IEnumerable<TComponentType> none);

        QueryCheckResult QueryCheck(TComponentTypeSet allComponentsTypes, TQuery query);

        QueryCheckResult QueryCheckOnAddedComponent(TComponentType addedComponentType, TComponentTypeSet allComponentsTypes, TQuery query);

        QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<TComponentType> addedComponentTypes, TComponentTypeSet allComponentsTypes, TQuery query);

        QueryCheckResult QueryCheckOnRemovedComponent(TComponentType removedComponentType, TComponentTypeSet allComponentsTypes, TQuery query);

        QueryCheckResult QueryCheckOnRemovedComponents(IEnumerable<TComponentType> removedComponentTypes, TComponentTypeSet allComponentsTypes, TQuery query);

        void SetComponentType(TComponentTypeSet componentTypeSet, TComponentType componentType);

        void SetComponentTypes(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        void UnsetComponentType(TComponentTypeSet componentTypeSet, TComponentType componentType);

        void UnsetComponentTypes(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);
    }
}