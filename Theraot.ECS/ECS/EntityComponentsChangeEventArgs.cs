using System.Collections.Generic;

namespace Theraot.ECS
{
    /// <summary>
    /// Represent a change in the component types associated with an entity
    /// </summary>
    /// <typeparam name="TEntityId"></typeparam>
    /// <typeparam name="TComponentType"></typeparam>
    public class EntityComponentsChangeEventArgs<TEntityId, TComponentType> : EntityCollectionChangeBaseEventArgs<TEntityId>
    {
        internal EntityComponentsChangeEventArgs(CollectionChangeActionEx action, TEntityId entity, IList<TComponentType> componentTypes)
            : base(action, entity)
        {
            ComponentTypes = componentTypes;
        }

        /// <summary>
        /// Gets the collection of component types that changed.
        /// </summary>
        public IList<TComponentType> ComponentTypes { get; }
    }
}