using System.ComponentModel;

namespace Theraot.ECS
{
    public class EntityCollectionChangeEventArgs<TEntity> : CollectionChangeEventArgs
    {
        public TEntity Entity { get; }

        public EntityCollectionChangeEventArgs(CollectionChangeAction action, TEntity entity)
            : base(action, entity)
        {
            Entity = entity;
        }
    }
}