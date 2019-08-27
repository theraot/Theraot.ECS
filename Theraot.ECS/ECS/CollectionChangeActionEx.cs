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
}