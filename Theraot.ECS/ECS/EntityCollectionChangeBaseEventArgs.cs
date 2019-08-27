namespace Theraot.ECS
{
    public enum CollectionChangeActionEx
    {
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
        Add = System.ComponentModel.CollectionChangeAction.Add,
        Remove = System.ComponentModel.CollectionChangeAction.Remove
#else
        Add = 1,
        Remove = 2
#endif
    }

    public class EntityCollectionChangeBaseEventArgs<TEntity>
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
        : System.ComponentModel.CollectionChangeEventArgs
#else
        : System.EventArgs
#endif
    {
        public EntityCollectionChangeBaseEventArgs(CollectionChangeActionEx action, TEntity element)
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
            : base((System.ComponentModel.CollectionChangeAction)action, element)
#endif
        {
            Entity = element;
            IsAdd = action == CollectionChangeActionEx.Add;
            IsRemove = action == CollectionChangeActionEx.Remove;
        }

        public TEntity Entity { get; }

        public bool IsAdd { get; }

        public bool IsRemove { get; }
    }
}