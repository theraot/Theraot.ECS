using System.Collections.Generic;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public interface IComponentTypeManager<TComponentType, TComponentTypeSet>
    {
        TComponentTypeSet CreateComponentTypeSet(IEnumerable<TComponentType> enumerable);

        TComponentTypeSet CreateComponentTypeSet(Dictionary<TComponentType, Component> dictionary);

        void SetComponentType(TComponentTypeSet componentTypeSet, TComponentType componentType);

        void SetComponentTypes(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        void UnsetComponentType(TComponentTypeSet componentTypeSet, TComponentType componentType);

        void UnsetComponentTypes(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);
    }
}