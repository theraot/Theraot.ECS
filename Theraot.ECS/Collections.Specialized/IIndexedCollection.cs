namespace Theraot.Collections.Specialized
{
    public interface IIndexedCollection<TValue> : IHasIndexedRemove
    {
        int Add(TValue value);

        ref TValue GetRef(int index);

        bool TryGetValue(int index, out TValue value);

        int Update(int index, TValue value);
    }
}