using System.Collections.Generic;

namespace Theraot.ECS
{
    /// <summary>
    /// Represent a change in the component kinds associated with an entity
    /// </summary>
    /// <typeparam name="TEntityId"></typeparam>
    /// <typeparam name="TComponentKind"></typeparam>
    public class EntityComponentsChangeEventArgs<TEntityId, TComponentKind> : EntityCollectionChangeBaseEventArgs<TEntityId>
    {
        internal EntityComponentsChangeEventArgs(CollectionChangeActionEx action, TEntityId entityId, IList<TComponentKind> componentKinds)
            : base(action, entityId)
        {
            ComponentKinds = componentKinds;
        }

        /// <summary>
        /// Gets the collection of component kinds that changed.
        /// </summary>
        public IList<TComponentKind> ComponentKinds { get; }
    }
}