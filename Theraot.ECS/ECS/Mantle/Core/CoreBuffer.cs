using System;
using System.Collections.Generic;

namespace Theraot.ECS.Mantle.Core
{
    internal class CoreBuffer<TEntity, TComponentType>
    {
        private Dictionary<TComponentType, TypedCoreBuffer> _logDictionary;

        public bool BufferSetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            if (_logDictionary == null)
            {
                return false;
            }

            GetLog(componentType).Add(entity, component);
            return true;
        }

        public bool BufferSetComponents<TComponent>(TEntity entity, IList<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector)
        {
            if (_logDictionary == null)
            {
                return false;
            }

            foreach (var componentType in componentTypes)
            {
                GetLog(componentType).Add(entity, componentSelector(componentType));
            }

            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, IList<TComponentType> componentTypes)
        {
            if (_logDictionary == null)
            {
                return false;
            }

            foreach (var componentType in componentTypes)
            {
                GetLog(componentType).Remove(entity);
            }

            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, TComponentType componentType)
        {
            if (_logDictionary == null)
            {
                return false;
            }

            GetLog(componentType).Remove(entity);
            return true;
        }

        public bool CreateBuffer()
        {
            if (_logDictionary != null)
            {
                return false;
            }

            _logDictionary = new Dictionary<TComponentType, TypedCoreBuffer>();
            return true;
        }

        public void ExecuteBuffer<TComponentTypeSet>(ICore<TEntity, TComponentType, TComponentTypeSet> core)
        {
            var logDictionary = _logDictionary;
            _logDictionary = null;
            foreach (var typeLogPair in logDictionary)
            {
                typeLogPair.Value.ExecuteBuffer(core, typeLogPair.Key);
            }
        }

        private TypedCoreBuffer GetLog(TComponentType componentType)
        {
            if (_logDictionary.TryGetValue(componentType, out var log))
            {
                return log;
            }

            log = new TypedCoreBuffer();
            _logDictionary[componentType] = log;
            return log;
        }

        private class TypedCoreBuffer
        {
            private readonly HashSet<Operation> _added;

            private readonly HashSet<Operation> _removed;

            public TypedCoreBuffer()
            {
                _added = new HashSet<Operation>();
                _removed = new HashSet<Operation>();
            }

            public void Add(TEntity entity, object payload)
            {
                _added.Add(new Operation(entity, payload));
            }

            public void ExecuteBuffer<TComponentTypeSet>(ICore<TEntity, TComponentType, TComponentTypeSet> core, TComponentType componentType)
            {
                foreach (var operation in _added)
                {
                    core.SetComponent(operation.Entity, componentType, operation.Component);
                }
                foreach (var operation in _removed)
                {
                    core.UnsetComponent(operation.Entity, componentType);
                }
            }

            public void Remove(TEntity entity)
            {
                _removed.Add(new Operation(entity, null));
            }

            private sealed class Operation
            {
                public readonly object Component;

                public readonly TEntity Entity;

                public Operation(TEntity entity, object component)
                {
                    Entity = entity;
                    Component = component;
                }
            }
        }
    }
}