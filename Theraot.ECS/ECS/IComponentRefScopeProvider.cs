namespace Theraot.ECS
{
    internal interface IComponentRefScopeProvider<TEntity, in TComponentType>
    {
        IComponentRefScope<TEntity, TComponentType> GetComponentRefScope();
    }
}