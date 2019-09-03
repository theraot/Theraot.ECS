namespace Theraot.Collections.Specialized
{
    public interface IIntKeyCollection<TValue> : IHasRemoveByIntKey
    {
        int Add(TValue value);

        ref TValue GetRef(int key);

        bool TryGetValue(int key, out TValue value);

        int Update(int key, TValue value);
    }
}