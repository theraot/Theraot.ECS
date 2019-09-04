namespace Theraot.Collections.Specialized
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a collection operated by int keys.
    /// </summary>
    /// <typeparam name="TValue">The type of values stored in the collection.</typeparam>
    public interface IIntKeyCollection<TValue> : IHasRemoveByIntKey
    {
        /// <summary>
        /// Adds a value to the collection.
        /// </summary>
        /// <param name="value">The value to add to the collection.</param>
        /// <returns>The key associated to the added value.</returns>
        int Add(TValue value);

        /// <summary>
        /// Gets a reference to a value by its key.
        /// </summary>
        /// <param name="key">The key of the value to get a reference of.</param>
        /// <returns>A reference to the value associated with the key.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The specified key has not been associated with any value.</exception>
        ref TValue GetRef(int key);

        /// <summary>
        /// Attempts to get a value by its key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">The retrieved value</param>
        /// <returns>True if the value was retrieved; otherwise, false.</returns>
        bool TryGetValue(int key, out TValue value);

        /// <summary>
        /// Replaces an already stored value with a new one.
        /// </summary>
        /// <param name="key">The key of the value to update.</param>
        /// <param name="value">The new value to store.</param>
        /// <returns>The key where the new value was stored.</returns>
        /// <remarks>If the collection requires to use a new key to store the new value, it must return the new key.
        /// Otherwise, it can return the same key that was provided.
        /// It is valid to implement Update by calling Add to store the new value and then Remove on the old one.</remarks>
        int Update(int key, TValue value);
    }
}