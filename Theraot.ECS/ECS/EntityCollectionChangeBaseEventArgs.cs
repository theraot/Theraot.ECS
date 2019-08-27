using System.ComponentModel;

namespace Theraot.ECS
{
    public class EntityCollectionChangeBaseEventArgs<TEntity> : CollectionChangeEventArgs
    {
        public EntityCollectionChangeBaseEventArgs(CollectionChangeAction action, TEntity element)
            : base(action, element)
        {
            Entity = element;
            IsAdd = action == CollectionChangeAction.Add;
            IsRemove = action == CollectionChangeAction.Remove;
        }

        public TEntity Entity { get; }

        public bool IsAdd { get; }

        public bool IsRemove { get; }
    }
}