using System;
using System.Collections.Generic;

namespace Theraot.ECS.Mantle.Core
{
    internal class CoreBuffer<TEntity, TComponentType>
    {
        private Dictionary<TComponentType, HashSet<Operation>> _logDictionary;

        public bool BufferSetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            if (_logDictionary == null)
            {
                return false;
            }

            AddToLog(CollectionChangeActionEx.Add, entity, componentType, component);
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
                AddToLog(CollectionChangeActionEx.Add, entity, componentType, componentSelector(componentType));
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
                AddToLog(CollectionChangeActionEx.Remove, entity, componentType, null);
            }
            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, TComponentType componentType)
        {
            if (_logDictionary == null)
            {
                return false;
            }

            AddToLog(CollectionChangeActionEx.Remove, entity, componentType, null);
            return true;
        }

        public bool CreateBuffer()
        {
            if (_logDictionary != null)
            {
                return false;
            }
            _logDictionary = new Dictionary<TComponentType, HashSet<Operation>>();
            return true;
        }

        public void ExecuteBuffer<TComponentTypeSet>(ICore<TEntity, TComponentType, TComponentTypeSet> core)
        {
            var logDictionary = _logDictionary;
            _logDictionary = null;
            foreach (var typeLogPair in logDictionary)
            {
                foreach (var operation in typeLogPair.Value)
                {
                    var componentType = typeLogPair.Key;
                    if (operation.Action == CollectionChangeActionEx.Add)
                    {
                        core.SetComponent(operation.Entity, componentType, operation.Component);
                    }
                    else
                    {
                        core.UnsetComponent(operation.Entity, componentType);
                    }
                }
            }
        }

        private void AddToLog(CollectionChangeActionEx action, TEntity entity, TComponentType componentType, object payload)
        {
            if (!_logDictionary.TryGetValue(componentType, out var log))
            {
                log = new HashSet<Operation>();
                _logDictionary[componentType] = log;
            }
            log.Add(new Operation(entity, payload, action));
        }

        private sealed class Operation
        {
            public readonly CollectionChangeActionEx Action;

            public readonly object Component;

            public readonly TEntity Entity;

            public Operation(TEntity entity, object component, CollectionChangeActionEx action)
            {
                Entity = entity;
                Component = component;
                Action = action;
            }
        }
    }
}