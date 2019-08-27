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
            private readonly Dictionary<TEntity, object> _added;

            private readonly Dictionary<TEntity, object> _removed;

            public TypedCoreBuffer()
            {
                _added = new Dictionary<TEntity, object>();
                _removed = new Dictionary<TEntity, object>();
            }

            public void Add(TEntity entity, object payload)
            {
                _added.Add(entity, payload);
            }

            public void ExecuteBuffer<TComponentTypeSet>(ICore<TEntity, TComponentType, TComponentTypeSet> core, TComponentType componentType)
            {
                foreach (var operation in _added)
                {
                    core.SetComponent(operation.Key, componentType, operation.Value);
                }
                foreach (var operation in _removed)
                {
                    core.UnsetComponent(operation.Key, componentType);
                }
            }

            public void Remove(TEntity entity)
            {
                _removed.Add(entity, null);
            }
        }
    }
}