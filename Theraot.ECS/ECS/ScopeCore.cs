using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

namespace Theraot.ECS
{
    internal class ScopeCore<TEntity, TComponentType, TComponentTypeSet> : IComponentRefScope<TEntity, TComponentType>
    {
        private readonly Dictionary<TEntity, EntityComponentStorage<TComponentType, TComponentTypeSet>> _componentsByEntity;

        private readonly IComparer<TComponentType> _componentTypeComparer;

        private readonly IComponentTypeManager<TComponentType, TComponentTypeSet> _componentTypeManager;

        private readonly GlobalComponentStorage<TComponentType> _globalComponentStorage;

        public ScopeCore(IEqualityComparer<TComponentType> componentTypeEqualityComparer, IComponentTypeManager<TComponentType, TComponentTypeSet> componentTypeManager)
        {
            _componentTypeManager = componentTypeManager;
            _componentTypeComparer = new ProxyComparer<TComponentType>(componentTypeEqualityComparer);
            _globalComponentStorage = new GlobalComponentStorage<TComponentType>(componentTypeEqualityComparer);
            _componentsByEntity = new Dictionary<TEntity, EntityComponentStorage<TComponentType, TComponentTypeSet>>();
        }

        public IEnumerable<TEntity> AllEntities => _componentsByEntity.Keys;

        public int EntityCount => _componentsByEntity.Count;

        public void CreateEntity(TEntity entity)
        {
            _componentsByEntity[entity] = new EntityComponentStorage<TComponentType, TComponentTypeSet>(_componentTypeManager, _globalComponentStorage, _componentTypeComparer);
        }

        public TComponentTypeSet GetComponentTypes(TEntity entity)
        {
            return _componentsByEntity[entity].ComponentTypes;
        }

        public Type GetRegisteredComponentType(TComponentType componentType)
        {
            return _globalComponentStorage.GetRegisteredComponentType(componentType);
        }

        public bool SetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            return _componentsByEntity[entity].SetComponent(componentType, component);
        }

        public bool TryGetComponent<TComponent>(TEntity entity, TComponentType componentType, out TComponent component)
        {
            if (_componentsByEntity.TryGetValue(entity, out var components) && components.TryGetComponent(componentType, out component))
            {
                return true;
            }

            component = default;
            return false;
        }

        public bool TryRegisterComponentType<TComponent>(TComponentType componentType)
        {
            return _globalComponentStorage.TryRegisterComponentType<TComponent>(componentType);
        }

        public bool UnsetComponent(TEntity entity, TComponentType componentType)
        {
            return _componentsByEntity[entity].UnsetComponent(componentType);
        }

        public void With<TComponent1>(TEntity entity, TComponentType componentType1, ActionRef<TEntity, TComponent1> callback)
        {
            if (!TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1)
            );
        }

        public void With<TComponent1, TComponent2>(TEntity entity, TComponentType componentType1, TComponentType componentType2, ActionRef<TEntity, TComponent1, TComponent2> callback)
        {
            if (!TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1),
                ref componentRefSource.GetComponentRef<TComponent2>(componentType2)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            if (!TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1),
                ref componentRefSource.GetComponentRef<TComponent2>(componentType2),
                ref componentRefSource.GetComponentRef<TComponent3>(componentType3)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback)
        {
            if (!TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1),
                ref componentRefSource.GetComponentRef<TComponent2>(componentType2),
                ref componentRefSource.GetComponentRef<TComponent3>(componentType3),
                ref componentRefSource.GetComponentRef<TComponent4>(componentType4)
            );
        }

        public void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(TEntity entity, TComponentType componentType1, TComponentType componentType2, TComponentType componentType3, TComponentType componentType4, TComponentType componentType5, ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback)
        {
            if (!TryGetComponentRefSource(entity, out var componentRefSource))
            {
                throw new KeyNotFoundException("Entity not found");
            }

            callback
            (
                entity,
                ref componentRefSource.GetComponentRef<TComponent1>(componentType1),
                ref componentRefSource.GetComponentRef<TComponent2>(componentType2),
                ref componentRefSource.GetComponentRef<TComponent3>(componentType3),
                ref componentRefSource.GetComponentRef<TComponent4>(componentType4),
                ref componentRefSource.GetComponentRef<TComponent5>(componentType5)
            );
        }

        private bool TryGetComponentRefSource(TEntity entity, out IComponentRef<TComponentType> componentRef)
        {
            if (_componentsByEntity.TryGetValue(entity, out var result))
            {
                componentRef = result;
                return true;
            }

            componentRef = default;
            return false;
        }
    }
}