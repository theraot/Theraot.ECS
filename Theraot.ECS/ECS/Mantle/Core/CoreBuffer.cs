using System;
using System.Collections.Generic;

namespace Theraot.ECS.Mantle.Core
{
    internal class CoreBuffer<TEntity, TComponentType>
    {
        private List<KeyValuePair<EntityComponentsChangeEventArgs<TEntity, TComponentType>, object>> _log;

        public bool BufferSetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add
            (
                new KeyValuePair<EntityComponentsChangeEventArgs<TEntity, TComponentType>, object>
                (
                    new EntityComponentsChangeEventArgs<TEntity, TComponentType>
                    (
                        CollectionChangeActionEx.Add,
                        entity,
                        new[] { componentType }
                    ),
                    new Func<TComponentType, TComponent>(_ => component)
                )
            );
            return true;
        }

        public bool BufferSetComponents<TComponent>(TEntity entity, IList<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add
            (
                new KeyValuePair<EntityComponentsChangeEventArgs<TEntity, TComponentType>, object>
                (
                    new EntityComponentsChangeEventArgs<TEntity, TComponentType>
                    (
                        CollectionChangeActionEx.Add,
                        entity,
                        componentTypes
                    ),
                    componentSelector
                )
            );
            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, IList<TComponentType> componentTypes)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add
            (
                new KeyValuePair<EntityComponentsChangeEventArgs<TEntity, TComponentType>, object>
                (
                    new EntityComponentsChangeEventArgs<TEntity, TComponentType>
                    (
                        CollectionChangeActionEx.Remove,
                        entity,
                        componentTypes
                    ),
                    null
                )
            );
            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, TComponentType componentType)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add
            (
                new KeyValuePair<EntityComponentsChangeEventArgs<TEntity, TComponentType>, object>
                (
                    new EntityComponentsChangeEventArgs<TEntity, TComponentType>
                    (
                        CollectionChangeActionEx.Remove,
                        entity,
                        new[] { componentType }
                    ),
                    null
                )
            );
            return true;
        }

        public bool CreateBuffer()
        {
            if (_log != null)
            {
                return false;
            }
            _log = new List<KeyValuePair<EntityComponentsChangeEventArgs<TEntity, TComponentType>, object>>();
            return true;
        }

        public void ExecuteBuffer<TComponentTypeSet>(ICore<TEntity, TComponentType, TComponentTypeSet> core)
        {
            var log = _log;
            _log = null;
            foreach (var pair in log)
            {
                var componentTypes = pair.Key.ComponentTypes;
                var entity = pair.Key.Entity;
                if (pair.Key.IsAdd)
                {
                    core.SetComponents(entity, componentTypes, (Func<TComponentType, object>)pair.Value);
                }
                else
                {
                    core.UnsetComponents(entity, componentTypes);
                }
            }
        }
    }
}