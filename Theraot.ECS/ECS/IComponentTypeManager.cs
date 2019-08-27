using System.Collections.Generic;

namespace Theraot.ECS
{
#if LESSTHAN_NET40

    public interface IComponentTypeManager<TComponentType, TComponentTypeSet>
#else

    public interface IComponentTypeManager<in TComponentType, TComponentTypeSet>
#endif
    {
        IEqualityComparer<TComponentType> ComponentTypEqualityComparer { get; }

        IEqualityComparer<TComponentTypeSet> ComponentTypSetEqualityComparer { get; }

        void Add(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        void Add(TComponentTypeSet componentTypeSet, TComponentType componentType);

        bool Contains(TComponentTypeSet componentTypeSet, TComponentType componentType);

        bool ContainsAll(TComponentTypeSet componentTypeSet, TComponentTypeSet other);

        TComponentTypeSet Create();

        bool IsEmpty(TComponentTypeSet componentTypeSet);

        bool Overlaps(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        bool Overlaps(TComponentTypeSet componentTypeSetA, TComponentTypeSet componentTypeSetB);

        void Remove(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        void Remove(TComponentTypeSet componentTypeSet, TComponentType componentType);
    }
}