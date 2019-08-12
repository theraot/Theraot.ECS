using System;
using System.Collections.Generic;
using Component = System.Object;
using ComponentType = System.String;
using ComponentTypeSet = System.Collections.Generic.ISet<string>;

namespace Theraot.ECS
{
    public sealed class SetManager : IComponentTypeManager<ComponentType, ComponentTypeSet>
    {
        public void Add(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            if (componentTypeSet.IsReadOnly)
            {
                return;
            }
            componentTypeSet.Add(componentType);
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
            if (componentTypeSet.IsReadOnly)
            {
                return;
            }

            foreach (var componentType in componentTypes)
            {
                componentTypeSet.Add(componentType);
            }
        }

        public bool Contains(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.Count != 0 && componentTypeSet.Contains(componentType);
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
            return DictionaryKeySet.CreateFrom(dictionary);
        }

        public ComponentTypeSet Create(IEnumerable<ComponentType> enumerable)
        {
            return new HashSet<ComponentType>(enumerable);
        }

        public bool IsEmpty(ComponentTypeSet componentTypeSet)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.Count == 0;
        }

        public bool Overlaps(ComponentTypeSet componentTypeSet, IEnumerable<string> componentTypes)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            return componentTypeSet.Count != 0 && componentTypeSet.Overlaps(componentTypes);
        }

        public bool Overlaps(ComponentTypeSet componentTypeSetA, ComponentTypeSet componentTypeSetB)
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

        public void Remove(ComponentTypeSet componentTypeSet, ComponentType componentType)
        {
            if (componentTypeSet == null)
            {
                throw new ArgumentNullException(nameof(componentTypeSet));
            }
            if (componentTypeSet.IsReadOnly)
            {
                return;
            }

            componentTypeSet.Remove(componentType);
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
            if (componentTypeSet.IsReadOnly)
            {
                return;
            }

            foreach (var componentType in componentTypes)
            {
                componentTypeSet.Remove(componentType);
            }
        }
    }
}