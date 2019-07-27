#pragma warning disable RECS0096 // Type parameter is never used
// ReSharper disable UnusedTypeParameter

using System.Linq;
using Component = System.Object;

namespace Theraot.ECS
{
    public sealed partial class Scope<TEntity, TComponentType, TComponentTypeSet, TQuery>
    {
        public void SetComponent<TComponent>(TEntity entity, TComponent component)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponentType = GetComponentType(component);
            if (!allComponents.Set(addedComponentType, component))
            {
                return;
            }

            var allComponentsTypes = _componentTypesByEntity[entity];
            _strategy.SetComponentType(allComponentsTypes, addedComponentType);
            UpdateEntitiesByQueryOnAddedComponent(entity, allComponentsTypes, addedComponentType);
        }

        public void SetComponent<TComponent1, TComponent2>(TEntity entity, TComponent1 component1, TComponent2 component2)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll
            (
                new Component[] {component1, component2},
                new[] {GetComponentType(component1), GetComponentType(component2)}
            ).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }

            var allComponentsTypes = _componentTypesByEntity[entity];
            UpdateEntitiesByQueryOnAddedComponents(entity, allComponentsTypes, addedComponents);
        }

        public void SetComponent<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll
            (
                new Component[] {component1, component2, component3},
                new[] {GetComponentType(component1), GetComponentType(component2), GetComponentType(component3)}
            ).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }

            var allComponentsTypes = _componentTypesByEntity[entity];
            UpdateEntitiesByQueryOnAddedComponents(entity, allComponentsTypes, addedComponents);
        }

        public void SetComponent(TEntity entity, params Component[] components)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll(components, GetComponentType).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }

            var allComponentsTypes = _componentTypesByEntity[entity];
            UpdateEntitiesByQueryOnAddedComponents(entity, allComponentsTypes, addedComponents);
        }
    }
}