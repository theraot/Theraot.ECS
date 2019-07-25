using System;
using System.Linq;

namespace Theraot.ECS
{
    public partial class Scope<TEntity>
    {
        public void SetComponent<TComponent>(TEntity entity, TComponent component)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponentType = GetComponentType(component);
            if (!allComponents.Set(addedComponentType, component))
            {
                return;
            }

            UpdateEntitiesByQueryOnAddedComponent(entity, allComponents, addedComponentType);
        }

        public void SetComponent<TComponent1, TComponent2>(TEntity entity, TComponent1 component1, TComponent2 component2)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll
            (
                new Object[] {component1, component2},
                new[] {GetComponentType(component1), GetComponentType(component2)}
            ).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }

            UpdateEntitiesByQueryOnAddedComponents(entity, allComponents, addedComponents);
        }

        public void SetComponent<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll
            (
                new Object[] {component1, component2, component3},
                new[] {GetComponentType(component1), GetComponentType(component2), GetComponentType(component3)}
            ).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }

            UpdateEntitiesByQueryOnAddedComponents(entity, allComponents, addedComponents);
        }

        public void SetComponent<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3, TComponent4 component4)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll
            (
                new Object[] {component1, component2, component3, component4},
                new[] {GetComponentType(component1), GetComponentType(component2), GetComponentType(component3), GetComponentType(component4)}
            ).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }

            UpdateEntitiesByQueryOnAddedComponents(entity, allComponents, addedComponents);
        }

        public void SetComponent(TEntity entity, params Object[] components)
        {
            var allComponents = _componentsByEntity[entity];
            var addedComponents = allComponents.SetAll(components, GetComponentType).ToArray();
            if (addedComponents.Length == 0)
            {
                return;
            }

            UpdateEntitiesByQueryOnAddedComponents(entity, allComponents, addedComponents);
        }
    }
}