namespace Theraot.ECS
{
    public class EntityCollectionChangeBaseEventArgs<TEntity>
#if NET_20 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
        : System.EventArgs
#else
        : System.ComponentModel.CollectionChangeEventArgs
#endif
    {
        protected EntityCollectionChangeBaseEventArgs(CollectionChangeActionEx action, TEntity element)
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
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