using System;
using System.Collections.Generic;
using System.Linq;
using Component = System.Object;
using ComponentType = System.Int32;
using ComponentTypeSet = Theraot.Collections.Specialized.FlagArray;

using QueryId = System.Int32;

namespace Theraot.ECS
{
    public sealed class FlagArrayManager : IComponentTypeManager<ComponentType, ComponentTypeSet>
    {
        private readonly int _capacity;

        public FlagArrayManager(int capacity)
        {
            _capacity = capacity;
        }

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

        public ComponentTypeSet Create(Dictionary<ComponentType, Component> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return CreateComponentTypeSetExtracted(dictionary.Keys);
        }

        public ComponentTypeSet Create(IEnumerable<ComponentType> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            return CreateComponentTypeSetExtracted(enumerable);
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

        public void Remove(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
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
                componentTypeSet[componentType] = false;
            }
        }

        private ComponentTypeSet CreateComponentTypeSetExtracted(IEnumerable<ComponentType> enumerable)
        {
            var set = new ComponentTypeSet(_capacity);
            foreach (var key in enumerable)
            {
                set[key] = true;
            }
            return set;
        }
    }
}