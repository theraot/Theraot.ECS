using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEqualityComparer<int> ComponentTypEqualityComparer => this;

        public IEqualityComparer<ComponentTypeSet> ComponentTypSetEqualityComparer => this;

        public void Add(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            componentTypeSet[componentType] = true;
        }

        public void Add(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
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

        public bool Contains(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet[componentType];
        }

        public bool ContainsAll(ComponentTypeSet componentTypeSet, ComponentTypeSet other)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.IsSupersetOf(other);
        }

        public ComponentTypeSet Create()
        {
            return new ComponentTypeSet(_capacity);
        }

        public bool Equals(ComponentType x, ComponentType y)
        {
            return x == y;
        }

        public bool Equals(ComponentTypeSet x, ComponentTypeSet y)
        {
            return x == y;
        }

        public int GetHashCode(ComponentTypeSet obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode();
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }

        public bool IsEmpty(ComponentTypeSet componentTypeSet)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return !componentTypeSet.Contains(true);
        }

        public bool Overlaps(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypes.Any(index => componentTypeSet[index]);
        }

        public bool Overlaps(ComponentTypeSet componentTypeSetA, ComponentTypeSet componentTypeSetB)
        {
            if (componentTypeSetA == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSetA));
            }
            return componentTypeSetA.Overlaps(componentTypeSetB);
        }

        public void Remove(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            componentTypeSet[componentType] = false;
        }

        public void Remove(ComponentTypeSet componentTypeSet, IEnumerable<int> componentTypes)
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