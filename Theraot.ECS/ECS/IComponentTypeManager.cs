using System.Collections.Generic;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public interface IComponentTypeManager<TComponentType, TComponentTypeSet>
    {
        TComponentTypeSet Create(IEnumerable<TComponentType> enumerable);

        TComponentTypeSet Create(Dictionary<TComponentType, Component> dictionary);

        void Add(TComponentTypeSet componentTypeSet, TComponentType componentType);

        void Add(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        void Remove(TComponentTypeSet componentTypeSet, TComponentType componentType);

        void Remove(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);
    }
}