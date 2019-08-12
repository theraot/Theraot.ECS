using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    public interface IComponentTypeManager<TComponentType, TComponentTypeSet>
    {
        void Add(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        void Add(TComponentTypeSet componentTypeSet, TComponentType componentType);

        bool Contains(TComponentTypeSet componentTypeSet, TComponentType componentType);

        bool ContainsAll(TComponentTypeSet componentTypeSet, TComponentTypeSet other);

        TComponentTypeSet Create(Dictionary<TComponentType, Component> dictionary);

        TComponentTypeSet Create(IEnumerable<TComponentType> enumerable);

        bool IsEmpty(TComponentTypeSet componentTypeSet);

        bool Overlaps(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        bool Overlaps(TComponentTypeSet componentTypeSetA, TComponentTypeSet componentTypeSetB);

        void Remove(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        void Remove(TComponentTypeSet componentTypeSet, TComponentType componentType);
    }
}