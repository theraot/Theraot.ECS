#pragma warning disable CC0091

using System;
using System.Collections.Generic;
using ComponentKind = System.Int32;
using ComponentKindSet = Theraot.Collections.Specialized.FlagArray;

namespace Theraot.ECS
{
    /// <summary>
    /// Represents a manager of set of component kinds stored as a <see cref="Collections.Specialized.FlagArray"/>
    /// </summary>
    public sealed class FlagArrayManager : IComponentKindManager<ComponentKind, ComponentKindSet>, IEqualityComparer<ComponentKind>, IEqualityComparer<ComponentKindSet>
    {
        private readonly int _capacity;

        public FlagArrayManager(int capacity)
        {
            _capacity = capacity;
        }

        IEqualityComparer<int> IComponentKindManager<int, ComponentKindSet>.ComponentKindEqualityComparer => this;

        IEqualityComparer<ComponentKindSet> IComponentKindManager<int, ComponentKindSet>.ComponentKindSetEqualityComparer => this;

        void IComponentKindManager<int, ComponentKindSet>.Add(ComponentKindSet componentKindSet, IEnumerable<ComponentKind> componentKinds)
        {
            if (componentKinds == null)
            {
                throw new ArgumentNullException(nameof(componentKinds));
            }

            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }

            foreach (var componentKind in componentKinds)
            {
                componentKindSet[componentKind] = true;
            }
        }

        bool IComponentKindManager<int, ComponentKindSet>.Contains(ComponentKindSet componentKindSet, ComponentKind componentKind)
        {
            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }

            return componentKindSet[componentKind];
        }

        bool IComponentKindManager<int, ComponentKindSet>.ContainsAll(ComponentKindSet componentKindSet, ComponentKindSet other)
        {
            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }

            return componentKindSet.IsSupersetOf(other);
        }

        ComponentKindSet IComponentKindManager<int, ComponentKindSet>.Create()
        {
            return new ComponentKindSet(_capacity);
        }

        public bool Equals(ComponentKind x, ComponentKind y)
        {
            return x == y;
        }

        bool IEqualityComparer<ComponentKindSet>.Equals(ComponentKindSet x, ComponentKindSet y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }

        int IEqualityComparer<ComponentKindSet>.GetHashCode(ComponentKindSet obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode();
        }

        bool IComponentKindManager<int, ComponentKindSet>.IsEmpty(ComponentKindSet componentKindSet)
        {
            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }

            return !componentKindSet.Contains(true);
        }

        bool IComponentKindManager<int, ComponentKindSet>.Overlaps(ComponentKindSet componentKindSet, IEnumerable<ComponentKind> componentKinds)
        {
            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }

            if (componentKinds == null)
            {
                throw new ArgumentNullException(nameof(componentKinds));
            }

            return EnumerableHelper.Any(componentKinds, index => componentKindSet[index]);
        }

        bool IComponentKindManager<int, ComponentKindSet>.Overlaps(ComponentKindSet componentKindSetA, ComponentKindSet componentKindSetB)
        {
            if (componentKindSetA == null)
            {
                throw new ArgumentNullException(nameof(componentKindSetA));
            }

            return componentKindSetA.Overlaps(componentKindSetB);
        }

        void IComponentKindManager<int, ComponentKindSet>.Remove(ComponentKindSet componentKindSet, IEnumerable<int> componentKinds)
        {
            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }

            if (componentKinds == null)
            {
                throw new ArgumentNullException(nameof(componentKinds));
            }

            foreach (var componentKind in componentKinds)
            {
                componentKindSet[componentKind] = false;
            }
        }
    }
}