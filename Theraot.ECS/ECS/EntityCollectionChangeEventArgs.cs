namespace Theraot.ECS
{
    internal static class EntityCollectionChangeEventArgs
    {
        public static EntityCollectionChangeEventArgs<TEntity> CreateAdd<TEntity>(TEntity entity)
        {
            return new EntityCollectionChangeEventArgs<TEntity>(CollectionChangeActionEx.Add, entity);
        }

        public static EntityCollectionChangeEventArgs<TEntity> CreateRemove<TEntity>(TEntity entity)
        {
            return new EntityCollectionChangeEventArgs<TEntity>(CollectionChangeActionEx.Remove, entity);
        }
    }

    /// <inheritdoc />
    public sealed class EntityCollectionChangeEventArgs<TEntity> : EntityCollectionChangeBaseEventArgs<TEntity>
    {
        internal EntityCollectionChangeEventArgs(CollectionChangeActionEx action, TEntity entity)
            : base(action, entity)
        {
            // Empty
        }
    }
}