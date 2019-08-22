namespace Theraot.ECS
{
    internal interface IScopeCore<TEntity, TComponentType, in TComponentTypeSet> : IScopeCommon<TEntity, TComponentType>
    {
        void RegisterEntity(TEntity entity, TComponentTypeSet componentTypeSet);
    }
}