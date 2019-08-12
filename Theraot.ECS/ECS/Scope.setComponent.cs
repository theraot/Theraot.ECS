﻿#pragma warning disable RECS0096 // Type parameter is never used
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

        public void SetComponents(TEntity entity, IEnumerable<KeyValuePair<TComponentType, Component>> components)
        {
            var componentStorage = _componentsByEntity[entity];
            if (componentStorage.SetComponents(components, out var addedComponents))
            {
                UpdateEntitiesByQueryOnAddedComponents(entity, componentStorage.ComponentTypes, addedComponents);
            }
        }

        private void UpdateEntitiesByQueryOnAddedComponent(TEntity entity, TComponentTypeSet allComponentsTypes, TComponentType addedComponentType)
        {
            foreach (var queryId in GetQueriesByComponentType(addedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_strategy.QueryCheckOnAddedComponent(addedComponentType, allComponentsTypes, queryId))
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

        private void UpdateEntitiesByQueryOnAddedComponents(TEntity entity, TComponentTypeSet allComponentsTypes, Dictionary<TComponentType, Component> addedComponents)
        {
            var addedComponentTypes = addedComponents.Keys;
            foreach (var queryId in GetQueriesByComponentTypes(addedComponentTypes))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_strategy.QueryCheckOnAddedComponents(addedComponentTypes, allComponentsTypes, queryId))
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