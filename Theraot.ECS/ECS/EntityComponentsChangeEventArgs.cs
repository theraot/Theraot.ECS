using System.Collections.Generic;

namespace Theraot.ECS
{
    public class EntityComponentsChangeEventArgs<TEntity, TComponentType> : EntityCollectionChangeBaseEventArgs<TEntity>
    {
        internal EntityComponentsChangeEventArgs(CollectionChangeActionEx action, TEntity entity, IList<TComponentType> componentTypes)
            : base(action, entity)
        {
            ComponentTypes = componentTypes;
        }

        public IList<TComponentType> ComponentTypes { get; }
    }
}