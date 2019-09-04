namespace Theraot.ECS
{
    /// <summary>
    /// Represents a change in a <see cref="EntityCollection{TEntityId, TComponentType}"/>
    /// </summary>
    /// <typeparam name="TEntityId"></typeparam>
    public class EntityCollectionChangeBaseEventArgs<TEntityId>
#if NET_20 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
        : System.EventArgs
#else
        : System.ComponentModel.CollectionChangeEventArgs
#endif
    {
        internal EntityCollectionChangeBaseEventArgs(CollectionChangeActionEx action, TEntityId element)
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
            : base((System.ComponentModel.CollectionChangeAction)action, element)
#endif
        {
            EntityId = element;
            IsAdd = action == CollectionChangeActionEx.Add;
            IsRemove = action == CollectionChangeActionEx.Remove;
        }

        /// <summary>
        /// Gets the entity id.
        /// </summary>
        public TEntityId EntityId { get; }

        /// <summary>
        /// Gets whatever or not the change was an addition.
        /// </summary>
        public bool IsAdd { get; }

        /// <summary>
        /// Gets whatever or not the change was a removal.
        /// </summary>
        public bool IsRemove { get; }
    }
}