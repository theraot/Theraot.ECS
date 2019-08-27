using System.Collections.Generic;
using System.ComponentModel;

namespace Theraot.ECS
{
    public static class EntityComponentsChangeEventArgs
    {
        public static EntityComponentsChangeEventArgs<TEntity, TComponentType> CreateAdd<TEntity, TComponentType>(TEntity entity, IList<TComponentType> componentTypes)
        {
            return new EntityComponentsChangeEventArgs<TEntity, TComponentType>(CollectionChangeAction.Add, entity, componentTypes);
        }

        public static EntityComponentsChangeEventArgs<TEntity, TComponentType> CreateRemove<TEntity, TComponentType>(TEntity entity, IList<TComponentType> componentTypes)
        {
            return new EntityComponentsChangeEventArgs<TEntity, TComponentType>(CollectionChangeAction.Remove, entity, componentTypes);
        }
    }

    public class EntityComponentsChangeEventArgs<TEntity, TComponentType> : EntityCollectionChangeBaseEventArgs<TEntity>
    {
        internal EntityComponentsChangeEventArgs(CollectionChangeAction action, TEntity entity, IList<TComponentType> componentTypes)
            : base(action, entity)
        {
            ComponentTypes = componentTypes;
        }

        public IList<TComponentType> ComponentTypes { get; }
    }
}