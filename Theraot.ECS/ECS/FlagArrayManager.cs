using System;
using System.Collections.Generic;
using ComponentType = System.Int32;
using ComponentTypeSet = Theraot.Collections.Specialized.FlagArray;

namespace Theraot.ECS
{
    public sealed class FlagArrayManager : IComponentTypeManager<ComponentType, ComponentTypeSet>, IEqualityComparer<ComponentType>, IEqualityComparer<ComponentTypeSet>
    {
        private readonly int _capacity;

        public FlagArrayManager(int capacity)
        {
            _capacity = capacity;
        }

        IEqualityComparer<int> IComponentTypeManager<int, ComponentTypeSet>.ComponentTypEqualityComparer => this;

        IEqualityComparer<ComponentTypeSet> IComponentTypeManager<int, ComponentTypeSet>.ComponentTypSetEqualityComparer => this;

        void IComponentTypeManager<int, ComponentTypeSet>.Add(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            componentTypeSet[componentType] = true;
        }

        void IComponentTypeManager<int, ComponentTypeSet>.Add(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
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
                componentTypeSet[componentType] = true;
            }
        }

        bool IComponentTypeManager<int, ComponentTypeSet>.Contains(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet[componentType];
        }

        bool IComponentTypeManager<int, ComponentTypeSet>.ContainsAll(ComponentTypeSet componentTypeSet, ComponentTypeSet other)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.IsSupersetOf(other);
        }

        ComponentTypeSet IComponentTypeManager<int, ComponentTypeSet>.Create()
        {
            return new ComponentTypeSet(_capacity);
        }

        public bool Equals(ComponentType x, ComponentType y)
        {
            return x == y;
        }

        bool IEqualityComparer<ComponentTypeSet>.Equals(ComponentTypeSet x, ComponentTypeSet y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }

        int IEqualityComparer<ComponentTypeSet>.GetHashCode(ComponentTypeSet obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode();
        }

        bool IComponentTypeManager<int, ComponentTypeSet>.IsEmpty(ComponentTypeSet componentTypeSet)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return !componentTypeSet.Contains(true);
        }

        bool IComponentTypeManager<int, ComponentTypeSet>.Overlaps(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            if (componentTypes == null)
            {
                throw new ArgumentNullException(nameof(componentTypes));
            }
            return EnumerableHelper.Any(componentTypes, index => componentTypeSet[index]);
        }

        bool IComponentTypeManager<int, ComponentTypeSet>.Overlaps(ComponentTypeSet componentTypeSetA, ComponentTypeSet componentTypeSetB)
        {
            if (componentTypeSetA == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSetA));
            }
            return componentTypeSetA.Overlaps(componentTypeSetB);
        }

        void IComponentTypeManager<int, ComponentTypeSet>.Remove(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            componentTypeSet[componentType] = false;
        }

        void IComponentTypeManager<int, ComponentTypeSet>.Remove(ComponentTypeSet componentTypeSet, IEnumerable<int> componentTypes)
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
                componentTypeSet[componentType] = false;
            }
        }
    }
}