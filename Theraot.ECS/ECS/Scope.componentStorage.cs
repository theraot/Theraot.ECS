#pragma warning disable RECS0096 // Type parameter is never used
// ReSharper disable UnusedTypeParameter

using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    public sealed partial class Scope<TEntity, TComponentType, TComponentTypeSet>
    {
        public void SetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            var componentStorage = _componentsByEntity[entity];
            if (componentStorage.SetComponent(componentType, component))
            {
                UpdateEntitiesByQueryOnAddedComponent(entity, componentStorage.ComponentTypes, componentType);
            }
        }

        public void SetComponents(TEntity entity, IDictionary<TComponentType, Component> components)
        {
            var componentStorage = _componentsByEntity[entity];
            if (componentStorage.SetComponents(components, out var addedComponents))
            {
                UpdateEntitiesByQueryOnAddedComponents(entity, componentStorage.ComponentTypes, addedComponents);
            }
        }

        public void UnsetComponent(TEntity entity, TComponentType componentType)
        {
            var componentStorage = _componentsByEntity[entity];
            if (componentStorage.UnsetComponent(componentType))
            {
                UpdateEntitiesByQueryOnRemoveComponent(entity, componentStorage.ComponentTypes, componentType);
            }
        }

        public void UnsetComponents(TEntity entity, IEnumerable<TComponentType> componentTypes)
        {
            var componentStorage = _componentsByEntity[entity];
            if (componentStorage.UnsetComponents(componentTypes, out var removedComponents))
            {
                UpdateEntitiesByQueryOnRemoveComponents(entity, componentStorage.ComponentTypes, removedComponents);
            }
        }

        private void UpdateEntitiesByQueryOnAddedComponent(TEntity entity, TComponentTypeSet allComponentsTypes, TComponentType addedComponentType)
        {
            foreach (var queryId in GetQueriesByComponentType(addedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_queryManager.QueryCheckOnAddedComponent(addedComponentType, allComponentsTypes, queryId))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entity);
                        break;

                    case QueryCheckResult.Add:
                        set.Add(entity);
                        break;

                    case QueryCheckResult.Noop:
                        break;

                    default:
                        break;
                }
            }
        }

        private void UpdateEntitiesByQueryOnAddedComponents(TEntity entity, TComponentTypeSet allComponentsTypes, List<TComponentType> addedComponentTypes)
        {
            foreach (var queryId in GetQueriesByComponentTypes(addedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_queryManager.QueryCheckOnAddedComponents(addedComponentTypes, allComponentsTypes, queryId))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entity);
                        break;

                    case QueryCheckResult.Add:
                        set.Add(entity);
                        break;

                    case QueryCheckResult.Noop:
                        break;

                    default:
                        break;
                }
            }
        }

        private void UpdateEntitiesByQueryOnRemoveComponent(TEntity entity, TComponentTypeSet allComponentsTypes, TComponentType removedComponentType)
        {
            foreach (var queryId in GetQueriesByComponentType(removedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_queryManager.QueryCheckOnRemovedComponent(removedComponentType, allComponentsTypes, queryId))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entity);
                        break;

                    case QueryCheckResult.Add:
                        set.Add(entity);
                        break;

                    case QueryCheckResult.Noop:
                        break;

                    default:
                        break;
                }
            }
        }

        private void UpdateEntitiesByQueryOnRemoveComponents(TEntity entity, TComponentTypeSet allComponentsTypes, List<TComponentType> removedComponentTypes)
        {
            foreach (var queryId in GetQueriesByComponentTypes(removedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_queryManager.QueryCheckOnRemovedComponents(removedComponentTypes, allComponentsTypes, queryId))
                {
                    case QueryCheckResult.Remove:
                        set.Remove(entity);
                        break;

                    case QueryCheckResult.Add:
                        set.Add(entity);
                        break;

                    case QueryCheckResult.Noop:
                        break;

                    default:
                        break;
                }
            }
        }
    }
}