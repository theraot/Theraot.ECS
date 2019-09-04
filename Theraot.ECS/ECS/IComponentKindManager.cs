using System.Collections.Generic;

namespace Theraot.ECS
{
    /// <summary>
    /// Represents an object that manages sets of component kinds.
    /// </summary>
    /// <typeparam name="TComponentKind">The type used to identify component kinds.</typeparam>
    /// <typeparam name="TComponentKindSet">The type used to store sets of component kinds.</typeparam>
#if LESSTHAN_NET40

    public interface IComponentKindManager<TComponentKind, TComponentKindSet>
#else

    public interface IComponentKindManager<in TComponentKind, TComponentKindSet>
#endif
    {
        /// <summary>
        /// Retrieves an <see cref="System.Collections.Generic.IEqualityComparer{TComponentKind}"/>, useful to compare component kinds.
        /// </summary>
        IEqualityComparer<TComponentKind> ComponentKindEqualityComparer { get; }

        /// <summary>
        /// Retrieves an <see cref="System.Collections.Generic.IEqualityComparer{TComponentKindSet}"/>, useful to compare sets of component kinds.
        /// </summary>
        IEqualityComparer<TComponentKindSet> ComponentKindSetEqualityComparer { get; }

        /// <summary>
        /// Adds component kinds to a set of component kinds.
        /// </summary>
        /// <param name="componentKindSet">The set of component kinds to add to.</param>
        /// <param name="componentKinds">The collection of component kinds to add.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentKindSet"/> or <paramref name="componentKinds"/> is null.</exception>
        void Add(TComponentKindSet componentKindSet, IEnumerable<TComponentKind> componentKinds);

        /// <summary>
        /// Verifies if a set of component kinds contains the specified component.
        /// </summary>
        /// <param name="componentKindSet">The set of component kinds where to search.</param>
        /// <param name="componentKind">The component kind to search.</param>
        /// <returns>True if the component kind was found in the set of component kinds; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentKindSet"/> is null.</exception>
        bool Contains(TComponentKindSet componentKindSet, TComponentKind componentKind);

        /// <summary>
        /// Verifies if a set of component kinds contains all the component kinds present in another set of component kinds.
        /// </summary>
        /// <param name="componentKindSet">The set of component kinds where to search.</param>
        /// <param name="other">The set of component kinds to search.</param>
        /// <returns>True if the set of component kinds contains all the component kinds of other set of component kinds.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentKindSet"/> is null.</exception>
        bool ContainsAll(TComponentKindSet componentKindSet, TComponentKindSet other);

        /// <summary>
        /// Create a new set of component kinds.
        /// </summary>
        /// <returns>A newly created empty set of component kinds.</returns>
        TComponentKindSet Create();

        /// <summary>
        /// Verifies if a set of component kinds is empty.
        /// </summary>
        /// <param name="componentKindSet">The set of component kinds to check.</param>
        /// <returns>True if the set of component kinds is empty; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentKindSet"/> is null.</exception>
        bool IsEmpty(TComponentKindSet componentKindSet);

        /// <summary>
        /// Verifies if the set of component kinds contains any of the component kinds provided.
        /// </summary>
        /// <param name="componentKindSet">The set of component kinds where to search.</param>
        /// <param name="componentKinds">The collection of component kinds to search.</param>
        /// <returns>True if the set of component kinds contains any of the component kinds; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentKindSet"/> or <paramref name="componentKinds"/> is null.</exception>
        /// <remark>Calling <see cref="Contains"></see> in a loop is a valid implementation.</remark>
        bool Overlaps(TComponentKindSet componentKindSet, IEnumerable<TComponentKind> componentKinds);

        /// <summary>
        /// Verifies if the sets of component kinds overlap.
        /// </summary>
        /// <param name="componentKindSetA">The first set of component kinds to compare.</param>
        /// <param name="componentKindSetB">The second set of component kinds to compare.</param>
        /// <returns>True if there is at least one component kind common to both sets of component kinds.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentKindSetA"/> or <paramref name="componentKindSetB"/> is null.</exception>
        bool Overlaps(TComponentKindSet componentKindSetA, TComponentKindSet componentKindSetB);

        /// <summary>
        /// Removes component kinds from a set of component kinds.
        /// </summary>
        /// <param name="componentKindSet">The set of component kinds to remove from.</param>
        /// <param name="componentKinds">The collection of component kinds to remove.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="componentKindSet"/> or <paramref name="componentKinds"/> is null.</exception>
        void Remove(TComponentKindSet componentKindSet, IEnumerable<TComponentKind> componentKinds);
    }
}