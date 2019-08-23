namespace Theraot.ECS.Mantle.Core
{
    internal interface IComponentRefScopeProvider<TEntity, in TComponentType>
    {
        IComponentRefScope<TEntity, TComponentType> GetComponentRefScope();
    }
}