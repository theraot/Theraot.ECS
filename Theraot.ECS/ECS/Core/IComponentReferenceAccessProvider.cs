namespace Theraot.ECS.Mantle.Core
{
    internal interface IComponentReferenceAccessProvider<TEntity, in TComponentType>
    {
        IComponentReferenceAccess<TEntity, TComponentType> GetComponentReferenceAccess();
    }
}