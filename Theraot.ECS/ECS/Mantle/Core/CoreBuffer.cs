using System;
using System.Collections.Generic;

namespace Theraot.ECS.Mantle.Core
{
    internal class CoreBuffer<TEntity, TComponentType, TComponentTypeSet>
    {
        private List<Action<ICore<TEntity, TComponentType, TComponentTypeSet>>> _log;

        public bool BufferSetComponent<TComponent>(TEntity entity, TComponentType componentType, TComponent component)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(core => core.SetComponent(entity, componentType, component));
            return true;
        }

        public bool BufferSetComponents<TComponent>(TEntity entity, IList<TComponentType> componentTypes, Func<TComponentType, TComponent> componentSelector)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(core => core.SetComponents(entity, componentTypes, componentSelector));
            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, IList<TComponentType> componentTypes)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(core => core.UnsetComponents(entity, componentTypes));
            return true;
        }

        public bool BufferUnsetComponent(TEntity entity, TComponentType componentType)
        {
            if (_log == null)
            {
                return false;
            }

            _log.Add(core => core.UnsetComponent(entity, componentType));
            return true;
        }

        public bool CreateBuffer()
        {
            if (_log != null)
            {
                return false;
            }

            _log = new List<Action<ICore<TEntity, TComponentType, TComponentTypeSet>>>();
            return true;
        }

        public void ExecuteBuffer(ICore<TEntity, TComponentType, TComponentTypeSet> core)
        {
            var log = _log;
            _log = null;
            foreach (var action in log)
            {
                action(core);
            }
        }
    }
}