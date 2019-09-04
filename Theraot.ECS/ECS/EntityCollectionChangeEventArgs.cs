namespace Theraot.ECS
{
    internal static class EntityCollectionChangeEventArgs
    {
        public static EntityCollectionChangeEventArgs<TEntityId> CreateAdd<TEntityId>(TEntityId entityId)
        {
            return new EntityCollectionChangeEventArgs<TEntityId>(CollectionChangeActionEx.Add, entityId);
        }

        public static EntityCollectionChangeEventArgs<TEntityId> CreateRemove<TEntityId>(TEntityId entityId)
        {
            return new EntityCollectionChangeEventArgs<TEntityId>(CollectionChangeActionEx.Remove, entityId);
        }
    }

    /// <inheritdoc />
    public sealed class EntityCollectionChangeEventArgs<TEntityId> : EntityCollectionChangeBaseEventArgs<TEntityId>
    {
        internal EntityCollectionChangeEventArgs(CollectionChangeActionEx action, TEntityId entityId)
            : base(action, entityId)
        {
            // Empty
        }
    }
}