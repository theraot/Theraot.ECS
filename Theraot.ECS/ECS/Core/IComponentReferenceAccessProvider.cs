using Theraot.ECS.Mantle;

namespace Theraot.ECS.Core
{
    internal interface IComponentReferenceAccessProvider<TEntity, in TComponentType>
    {
        IComponentReferenceAccess<TEntity, TComponentType> GetComponentReferenceAccess();
    }
}