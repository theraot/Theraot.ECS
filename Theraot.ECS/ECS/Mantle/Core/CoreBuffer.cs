using System;
using System.Collections.Generic;

namespace Theraot.ECS.Mantle.Core
{
    internal class CoreBuffer<TEntity, TComponentType, TComponentTypeSet>
    {
        private Dictionary<TComponentType, TypedCoreBuffer> _logDictionary;

        public bool BufferSetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            if (_logDictionary == null)
            {
                return false;
            }

            GetLog(componentType).Add(entity, core => core.SetComponent(entity, componentType, component));
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
                GetLog(componentType).Add(entity, core => core.SetComponent(entity, componentType, componentSelector(componentType)));
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
                GetLog(componentType).Add(entity, core => core.UnsetComponent(entity, componentType));
            }

            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, TComponentType componentType)
        {
            if (_logDictionary == null)
            {
                return false;
            }

            GetLog(componentType).Add(entity, core => core.UnsetComponent(entity, componentType));
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

        public void ExecuteBuffer(ICore<TEntity, TComponentType, TComponentTypeSet> core)
        {
            var logDictionary = _logDictionary;
            _logDictionary = null;
            foreach (var typeLogPair in logDictionary)
            {
                typeLogPair.Value.ExecuteBuffer(core);
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
            private readonly Dictionary<TEntity, Action<ICore<TEntity, TComponentType, TComponentTypeSet>>> _log;

            public TypedCoreBuffer()
            {
                _log = new Dictionary<TEntity, Action<ICore<TEntity, TComponentType, TComponentTypeSet>>>();
            }

            public void Add(TEntity entity, Action<ICore<TEntity, TComponentType, TComponentTypeSet>> action)
            {
                _log[entity] = action;
            }

            public void ExecuteBuffer(ICore<TEntity, TComponentType, TComponentTypeSet> core)
            {
                foreach (var operation in _log)
                {
                    operation.Value(core);
                }
            }
        }
    }
}