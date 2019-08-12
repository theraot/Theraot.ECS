using System.Collections.Generic;
using Component = System.Object;

namespace Theraot.ECS
{
    internal class ComponentStorage<TComponentType, TComponentTypeSet>
    {
        private IComponentTypeManager<TComponentType, TComponentTypeSet> _manager;

        public ComponentStorage(IComponentTypeManager<TComponentType, TComponentTypeSet> manager)
        {
            _manager = manager;
            Dictionary = new Dictionary<TComponentType, Component>();
            ComponentTypes = _manager.CreateComponentTypeSet(Dictionary);
        }

        public TComponentTypeSet ComponentTypes { get; }
        public Dictionary<TComponentType, Component> Dictionary { get; }
    }
}