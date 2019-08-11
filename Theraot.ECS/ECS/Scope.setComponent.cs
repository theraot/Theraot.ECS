#pragma warning disable RECS0096 // Type parameter is never used
// ReSharper disable UnusedTypeParameter

using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    public sealed partial class Scope<TEntity, TComponentType, TComponentTypeSet, TQuery>
    {
        public void SetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            var allComponents = _componentsByEntity[entity];
            if (!allComponents.Set(componentType, component))
            {
                return;
            }

            var allComponentsTypes = _componentTypesByEntity[entity];
            _strategy.SetComponentType(allComponentsTypes, componentType);
            UpdateEntitiesByQueryOnAddedComponent(entity, allComponentsTypes, componentType);
        }

        public void SetComponents(TEntity entity, IEnumerable<KeyValuePair<TComponentType, Component>> components)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll(components);
            if (addedComponents.Count == 0)
            {
                return;
            }

            var allComponentsTypes = _componentTypesByEntity[entity];
            _strategy.SetComponentTypes(allComponentsTypes, addedComponents.Keys);
            UpdateEntitiesByQueryOnAddedComponents(entity, allComponentsTypes, addedComponents);
        }

        private void UpdateEntitiesByQueryOnAddedComponent(TEntity entity, TComponentTypeSet allComponentsTypes, TComponentType addedComponentType)
        {
            foreach (var queryId in GetQueriesByComponentType(addedComponentType))
            {
                var set = _entitiesByQueryId[queryId];
                switch (_strategy.QueryCheckOnAddedComponent(addedComponentType, allComponentsTypes, _queryStorage.GetQuery(queryId)))
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
                switch (_strategy.QueryCheckOnAddedComponents(addedComponentTypes, allComponentsTypes, _queryStorage.GetQuery(queryId)))
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