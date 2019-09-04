namespace Theraot.Collections.Specialized
{
    /// <summary>
    /// Represents a collection that allows to remove items by an int key.
    /// </summary>
    public interface IHasRemoveByIntKey
    {
        /// <summary>
        /// Removes an element by an int key.
        /// </summary>
        /// <param name="key">The key associated with the element to remove.</param>
        /// <returns>True if the element was removed; otherwise, false.</returns>
        /// <remarks>Removing the element should not alter the key for other elements. That is, it is a key, not an index.</remarks>
        bool Remove(int key);
    }
}