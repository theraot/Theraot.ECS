using System.Collections.Generic;

namespace Theraot.ECS
{
    /// <summary>
    /// Represents an object that manages sets of component types.
    /// </summary>
    /// <typeparam name="TComponentType">The type used to identify component types.</typeparam>
    /// <typeparam name="TComponentTypeSet">The type used to store sets of component types.</typeparam>
#if LESSTHAN_NET40

    public interface IComponentTypeManager<TComponentType, TComponentTypeSet>
#else

    public interface IComponentTypeManager<in TComponentType, TComponentTypeSet>
#endif
    {
        /// <summary>
        /// Retrieves an <see cref="System.Collections.Generic.IEqualityComparer{TComponentType}"/>, useful to compare component types.
        /// </summary>
        IEqualityComparer<TComponentType> ComponentTypEqualityComparer { get; }

        /// <summary>
        /// Retrieves an <see cref="System.Collections.Generic.IEqualityComparer{TComponentTypeSet}"/>, useful to compare sets of component types.
        /// </summary>
        IEqualityComparer<TComponentTypeSet> ComponentTypSetEqualityComparer { get; }

        /// <summary>
        /// Adds component types to a set of component types.
        /// </summary>
        /// <param name="componentTypeSet">The set of component types to add to.</param>
        /// <param name="componentTypes">The collection of component types to add.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentTypeSet"/> or <paramref name="componentTypes"/> is null.</exception>
        void Add(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        /// <summary>
        /// Verifies if a set of component types contains the specified component.
        /// </summary>
        /// <param name="componentTypeSet">The set of component types where to search.</param>
        /// <param name="componentType">The component type to search.</param>
        /// <returns>True if the component type was found in the set of component types; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentTypeSet"/> is null.</exception>
        bool Contains(TComponentTypeSet componentTypeSet, TComponentType componentType);

        /// <summary>
        /// Verifies if a set of component types contains all the component types present in another set of component types.
        /// </summary>
        /// <param name="componentTypeSet">The set of component types where to search.</param>
        /// <param name="other">The set of component types to search.</param>
        /// <returns>True if the set of component types contains all the component types of other set of component types.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentTypeSet"/> is null.</exception>
        bool ContainsAll(TComponentTypeSet componentTypeSet, TComponentTypeSet other);

        /// <summary>
        /// Create a new set of component types.
        /// </summary>
        /// <returns>A newly created empty set of component types.</returns>
        TComponentTypeSet Create();

        /// <summary>
        /// Verifies if a set of component types is empty.
        /// </summary>
        /// <param name="componentTypeSet">The set of component types to check.</param>
        /// <returns>True if the set of component types is empty; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentTypeSet"/> is null.</exception>
        bool IsEmpty(TComponentTypeSet componentTypeSet);

        /// <summary>
        /// Verifies if the set of component types contains any of the component types provided.
        /// </summary>
        /// <param name="componentTypeSet">The set of component types where to search.</param>
        /// <param name="componentTypes">The collection of component types to search.</param>
        /// <returns>True if the set of component types contains any of the component types; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentTypeSet"/> or <paramref name="componentTypes"/> is null.</exception>
        /// <remark>Calling <see cref="Contains"></see> in a loop is a valid implementation.</remark>
        bool Overlaps(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);

        /// <summary>
        /// Verifies if the sets of component types overlap.
        /// </summary>
        /// <param name="componentTypeSetA">The first set of component types to compare.</param>
        /// <param name="componentTypeSetB">The second set of component types to compare.</param>
        /// <returns>True if there is at least one component type common to both sets of component types.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentTypeSetA"/> or <paramref name="componentTypeSetB"/> is null.</exception>
        bool Overlaps(TComponentTypeSet componentTypeSetA, TComponentTypeSet componentTypeSetB);

        /// <summary>
        /// Removes component types from a set of component types.
        /// </summary>
        /// <param name="componentTypeSet">The set of component types to remove from.</param>
        /// <param name="componentTypes">The collection of component types to remove.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentTypeSet"/> or <paramref name="componentTypes"/> is null.</exception>
        void Remove(TComponentTypeSet componentTypeSet, IEnumerable<TComponentType> componentTypes);
    }
}