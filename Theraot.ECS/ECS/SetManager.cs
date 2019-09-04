using System;
using System.Collections.Generic;
using ComponentKind = System.String;

#if LESSTHAN_NET35

using ComponentKindSet = Theraot.HashSet<string>;

#else

using ComponentKindSet = System.Collections.Generic.HashSet<string>;

#endif

namespace Theraot.ECS
{
    /// <summary>
    /// Represents a manager of set of types stored as a hash based set.
    /// </summary>
    public sealed partial class SetManager : IComponentKindManager<ComponentKind, ComponentKindSet>, IEqualityComparer<ComponentKind>, IEqualityComparer<ComponentKindSet>
    {
        IEqualityComparer<string> IComponentKindManager<string, ComponentKindSet>.ComponentKindEqualityComparer => this;

        IEqualityComparer<ComponentKindSet> IComponentKindManager<string, ComponentKindSet>.ComponentKindSetEqualityComparer => this;

        void IComponentKindManager<string, ComponentKindSet>.Add(ComponentKindSet componentKindSet, IEnumerable<ComponentKind> componentKinds)
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
                componentKindSet.Add(componentKind);
            }
        }

        bool IComponentKindManager<string, ComponentKindSet>.Contains(ComponentKindSet componentKindSet, ComponentKind componentKind)
        {
            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }

            return componentKindSet.Count != 0 && componentKindSet.Contains(componentKind);
        }

        bool IComponentKindManager<string, ComponentKindSet>.ContainsAll(ComponentKindSet componentKindSet, ComponentKindSet other)
        {
            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }

            return componentKindSet.IsSupersetOf(other);
        }

        ComponentKindSet IComponentKindManager<string, ComponentKindSet>.Create()
        {
            return new HashSet<ComponentKind>();
        }

        public bool Equals(string x, string y)
        {
            return EqualityComparer<string>.Default.Equals(x, y);
        }

        bool IEqualityComparer<ComponentKindSet>.Equals(ComponentKindSet x, ComponentKindSet y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.SetEquals(y);
        }

        int IEqualityComparer<ComponentKindSet>.GetHashCode(ComponentKindSet obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode();
        }

        bool IComponentKindManager<string, ComponentKindSet>.IsEmpty(ComponentKindSet componentKindSet)
        {
            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }

            return componentKindSet.Count == 0;
        }

        bool IComponentKindManager<string, ComponentKindSet>.Overlaps(ComponentKindSet componentKindSet, IEnumerable<string> componentKinds)
        {
            if (componentKindSet == null)
            {
                throw new ArgumentNullException(nameof(componentKindSet));
            }
            if (componentKinds == null)
            {
                throw new ArgumentNullException(nameof(componentKinds));
            }

            return componentKindSet.Count != 0 && componentKindSet.Overlaps(componentKinds);
        }

        bool IComponentKindManager<string, ComponentKindSet>.Overlaps(ComponentKindSet componentKindSetA, ComponentKindSet componentKindSetB)
        {
            if (componentKindSetA == null)
            {
                throw new ArgumentNullException(nameof(componentKindSetA));
            }

            if (componentKindSetB == null)
            {
                throw new ArgumentNullException(nameof(componentKindSetA));
            }

            return componentKindSetA.Count != 0 && (componentKindSetA.Count > componentKindSetB.Count ? componentKindSetA.Overlaps(componentKindSetB) : componentKindSetB.Overlaps(componentKindSetA));
        }

        void IComponentKindManager<string, ComponentKindSet>.Remove(ComponentKindSet componentKindSet, IEnumerable<string> componentKinds)
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
                componentKindSet.Remove(componentKind);
            }
        }
    }

#if TARGETS_NET || LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

    public sealed partial class SetManager
    {
        public int GetHashCode(string obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode();
        }
    }

#else

    public sealed partial class SetManager
    {
        public int GetHashCode(string obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }
    }

#endif
}