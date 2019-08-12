using System;
using System.Collections.Generic;
using Component = System.Object;
using ComponentType = System.String;
using ComponentTypeSet = System.Collections.Generic.ISet<string>;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public sealed partial class TypeHashSetStrategy : IComponentQueryStrategy<ComponentType, ComponentTypeSet>
    {
        private readonly QueryStorage<TypeHashSetQuery> _queryStorage;

        public TypeHashSetStrategy()
        {
            _queryStorage = new QueryStorage<TypeHashSetQuery>();
        }

        public IComponentTypeManager<ComponentType, ComponentTypeSet> ComponentTypeManager => this;

        public QueryId CreateQuery(IEnumerable<ComponentType> all, IEnumerable<ComponentType> any, IEnumerable<ComponentType> none)
        {
            return _queryStorage.AddQuery(new TypeHashSetQuery(all, any, none));
        }

        public QueryCheckResult QueryCheck(ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
            if
            (
                query.None.Count != 0
                && (query.None.Count > allComponentsTypes.Count ? query.None.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.None))
            )
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.All.Count == 0 || allComponentsTypes.IsSupersetOf(query.All))
                && (query.Any.Count == 0 || (query.Any.Count > allComponentsTypes.Count ? query.Any.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.Any)))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponent(ComponentType addedComponentType, ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
            if (query.None.Count != 0 && query.None.Contains(addedComponentType))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.All.Count == 0 || allComponentsTypes.IsSupersetOf(query.All))
                && (query.Any.Count == 0 || (query.Any.Count > allComponentsTypes.Count ? query.Any.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.Any)))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnAddedComponents(IEnumerable<ComponentType> addedComponentTypes, ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
            if (query.None.Count != 0 && query.None.Overlaps(addedComponentTypes))
            {
                // The entity has one of the components it should not have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.All.Count == 0 || allComponentsTypes.IsSupersetOf(query.All))
                && (query.Any.Count == 0 || (query.Any.Count > allComponentsTypes.Count ? query.Any.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.Any)))
            )
            {
                // The entity has all the required components for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponent(ComponentType removedComponentType, ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
            if (query.All.Contains(removedComponentType) || (query.Any.Count != 0 && !allComponentsTypes.Overlaps(query.Any)))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.None.Count == 0 || !query.None.Overlaps(allComponentsTypes))
                && (query.Any.Count == 0 || (query.Any.Count > allComponentsTypes.Count ? query.Any.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.Any)))
            )
            {
                // The entity has none of the components it should not have for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }

        public QueryCheckResult QueryCheckOnRemovedComponents(IEnumerable<ComponentType> removedComponentTypes, ComponentTypeSet allComponentsTypes, QueryId queryId)
        {
            if (allComponentsTypes == null)
            {
                throw new ArgumentNullException(nameof(allComponentsTypes));
            }

            var query = _queryStorage.GetQuery(queryId);
            if (query.All.Overlaps(removedComponentTypes) || (query.Any.Count != 0 && !allComponentsTypes.Overlaps(query.Any)))
            {
                // The entity no longer has one of the components it should have for this queryId
                return QueryCheckResult.Remove;
            }
            if
            (
                (query.None.Count == 0 || !query.None.Overlaps(allComponentsTypes))
                && (query.Any.Count == 0 || (query.Any.Count > allComponentsTypes.Count ? query.Any.Overlaps(allComponentsTypes) : allComponentsTypes.Overlaps(query.Any)))
            )
            {
                // The entity has none of the components it should not have for this queryId
                // and at least one of the optional components (if any) for this queryId
                return QueryCheckResult.Add;
            }
            return QueryCheckResult.Noop;
        }
    }

    public sealed partial class TypeHashSetStrategy : IComponentTypeManager<ComponentType, ComponentTypeSet>
    {
        public ComponentTypeSet CreateComponentTypeSet(Dictionary<ComponentType, Component> dictionary)
        {
            return DictionaryKeySet.CreateFrom(dictionary);
        }

        public ComponentTypeSet CreateComponentTypeSet(IEnumerable<string> enumerable)
        {
            return new HashSet<string>(enumerable);
        }

        public void SetComponentType(ComponentTypeSet componentTypeSet, ComponentType componentType)
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

        public void SetComponentTypes(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
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

        public void UnsetComponentType(ComponentTypeSet componentTypeSet, ComponentType componentType)
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

        public void UnsetComponentTypes(ComponentTypeSet componentTypeSet, IEnumerable<ComponentType> componentTypes)
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