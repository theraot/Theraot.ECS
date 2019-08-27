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
            private readonly Dictionary<TEntity, Option> _log;

            public TypedCoreBuffer()
            {
                _log = new Dictionary<TEntity, Option>();
            }

            public void Add(TEntity entity, object payload)
            {
                _log[entity] = Option.CreateValue(payload);
            }

            public void ExecuteBuffer<TComponentTypeSet>(ICore<TEntity, TComponentType, TComponentTypeSet> core, TComponentType componentType)
            {
                foreach (var operation in _log)
                {
                    var option = operation.Value;
                    if (option.Set)
                    {
                        core.SetComponent(operation.Key, componentType, option.Value);
                    }
                    else
                    {
                        core.UnsetComponent(operation.Key, componentType);
                    }
                }
            }

            public void Remove(TEntity entity)
            {
                _log[entity] = Option.CreateNotSet();
            }

            private sealed class Option
            {
                public readonly bool Set;

                public readonly object Value;

                private Option(object value, bool set)
                {
                    Value = value;
                    Set = set;
                }

                public static Option CreateNotSet()
                {
                    return new Option(null, false);
                }

                public static Option CreateValue(object value)
                {
                    return new Option(value, true);
                }
            }
        }
    }
}