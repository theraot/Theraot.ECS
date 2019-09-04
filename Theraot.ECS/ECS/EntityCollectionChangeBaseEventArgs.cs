namespace Theraot.ECS
{
    /// <summary>
    /// Represents a change in a <see cref="EntityCollection{TEntity, TComponentType}"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityCollectionChangeBaseEventArgs<TEntity>
#if NET_20 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
        : System.EventArgs
#else
        : System.ComponentModel.CollectionChangeEventArgs
#endif
    {
        internal EntityCollectionChangeBaseEventArgs(CollectionChangeActionEx action, TEntity element)
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
            : base((System.ComponentModel.CollectionChangeAction)action, element)
#endif
        {
            Entity = element;
            IsAdd = action == CollectionChangeActionEx.Add;
            IsRemove = action == CollectionChangeActionEx.Remove;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public TEntity Entity { get; }

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