﻿using System;
using System.Collections.Generic;
using ComponentType = System.String;

#if LESSTHAN_NET35

using ComponentTypeSet = Theraot.HashSet<string>;

#else

using ComponentTypeSet = System.Collections.Generic.HashSet<string>;

#endif

namespace Theraot.ECS
{
    public sealed class SetManager : IComponentTypeManager<ComponentType, ComponentTypeSet>, IEqualityComparer<ComponentType>, IEqualityComparer<ComponentTypeSet>
    {
        public IEqualityComparer<string> ComponentTypEqualityComparer => this;

        IEqualityComparer<ComponentTypeSet> IComponentTypeManager<string, ComponentTypeSet>.ComponentTypSetEqualityComparer => this;

        void IComponentTypeManager<string, ComponentTypeSet>.Add(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }

            componentTypeSet.Add(componentType);
        }

        void IComponentTypeManager<string, ComponentTypeSet>.Add(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
        {
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }

            foreach (var componentType in componentTypes)
            {
                componentTypeSet.Add(componentType);
            }
        }

        bool IComponentTypeManager<string, ComponentTypeSet>.Contains(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.Count != 0 && componentTypeSet.Contains(componentType);
        }

        bool IComponentTypeManager<string, ComponentTypeSet>.ContainsAll(ComponentTypeSet componentTypeSet, ComponentTypeSet other)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.IsSupersetOf(other);
        }

        ComponentTypeSet IComponentTypeManager<string, ComponentTypeSet>.Create()
        {
            return new HashSet<ComponentType>();
        }

        bool IEqualityComparer<ComponentTypeSet>.Equals(ComponentTypeSet x, ComponentTypeSet y)
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

        public bool Equals(string x, string y)
        {
            return EqualityComparer<string>.Default.Equals(x, y);
        }

        int IEqualityComparer<ComponentTypeSet>.GetHashCode(ComponentTypeSet obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode();
        }

        public int GetHashCode(string obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

#if TARGETS_NET || LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD
            return obj.GetHashCode();
#else
            return obj.GetHashCode(StringComparison.OrdinalIgnoreCase);
#endif
        }

        bool IComponentTypeManager<string, ComponentTypeSet>.IsEmpty(ComponentTypeSet componentTypeSet)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.Count == 0;
        }

        bool IComponentTypeManager<string, ComponentTypeSet>.Overlaps(ComponentTypeSet componentTypeSet, IEnumerable<string> componentTypes)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.Count != 0 && componentTypeSet.Overlaps(componentTypes);
        }

        bool IComponentTypeManager<string, ComponentTypeSet>.Overlaps(ComponentTypeSet componentTypeSetA, ComponentTypeSet componentTypeSetB)
        {
            if (componentTypeSetA == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSetA));
            }
            if (componentTypeSetB == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSetA));
            }
            return componentTypeSetA.Count != 0 && (componentTypeSetA.Count > componentTypeSetB.Count ? componentTypeSetA.Overlaps(componentTypeSetB) : componentTypeSetB.Overlaps(componentTypeSetA));
        }

        void IComponentTypeManager<string, ComponentTypeSet>.Remove(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }

            componentTypeSet.Remove(componentType);
        }

        void IComponentTypeManager<string, ComponentTypeSet>.Remove(ComponentTypeSet componentTypeSet, IEnumerable<string> componentTypes)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }

            foreach (var componentType in componentTypes)
            {
                componentTypeSet.Remove(componentType);
            }
        }
    }
}