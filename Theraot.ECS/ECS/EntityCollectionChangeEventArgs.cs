using System.ComponentModel;

namespace Theraot.ECS
{
    public static class EntityCollectionChangeEventArgs
    {
        public static EntityCollectionChangeEventArgs<TEntity> CreateAdd<TEntity>(TEntity entity)
        {
            return new EntityCollectionChangeEventArgs<TEntity>(CollectionChangeAction.Add, entity);
        }

        public static EntityCollectionChangeEventArgs<TEntity> CreateRemove<TEntity>(TEntity entity)
        {
            return new EntityCollectionChangeEventArgs<TEntity>(CollectionChangeAction.Remove, entity);
        }
    }

    public sealed class EntityCollectionChangeEventArgs<TEntity> : EntityCollectionChangeBaseEventArgs<TEntity>
    {
        internal EntityCollectionChangeEventArgs(CollectionChangeAction action, TEntity entity)
            : base(action, entity)
        {
            // Empty
        }
    }
}