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
            if (_componentsByEntity[entity].SetComponent(componentType, component, out var allComponentsTypes))
            {
                UpdateEntitiesByQueryOnAddedComponent(entity, allComponentsTypes, componentType);
            }
        }

        public void SetComponents(TEntity entity, IEnumerable<KeyValuePair<TComponentType, Component>> components)
        {
            if (_componentsByEntity[entity].SetComponents(components, out var allComponentsTypes, out var addedComponents))
            {
                UpdateEntitiesByQueryOnAddedComponents(entity, allComponentsTypes, addedComponents);
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