using System.ComponentModel;

namespace Theraot.ECS
{
    public class CollectionChangeEventArgs<T> : CollectionChangeEventArgs
    {
        public new T Element { get; }

        public CollectionChangeEventArgs(CollectionChangeAction action, T element)
            : base(action, element)
        {
            Element = element;
        }
    }
}