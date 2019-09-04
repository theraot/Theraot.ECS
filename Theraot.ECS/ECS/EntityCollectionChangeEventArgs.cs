namespace Theraot.ECS
{
    internal static class EntityCollectionChangeEventArgs
    {
        public static EntityCollectionChangeEventArgs<TEntityId> CreateAdd<TEntityId>(TEntityId entity)
        {
            return new EntityCollectionChangeEventArgs<TEntityId>(CollectionChangeActionEx.Add, entity);
        }

        public static EntityCollectionChangeEventArgs<TEntityId> CreateRemove<TEntityId>(TEntityId entity)
        {
            return new EntityCollectionChangeEventArgs<TEntityId>(CollectionChangeActionEx.Remove, entity);
        }
    }

    /// <inheritdoc />
    public sealed class EntityCollectionChangeEventArgs<TEntityId> : EntityCollectionChangeBaseEventArgs<TEntityId>
    {
        internal EntityCollectionChangeEventArgs(CollectionChangeActionEx action, TEntityId entity)
            : base(action, entity)
        {
            // Empty
        }
    }
}