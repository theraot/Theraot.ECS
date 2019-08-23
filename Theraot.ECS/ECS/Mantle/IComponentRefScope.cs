namespace Theraot.ECS.Mantle
{
    internal interface IComponentRefScope<TEntity, in TComponentType>
    {
        void With<TComponent1>
        (
            TEntity entity,
            TComponentType componentType1,
            ActionRef<TEntity, TComponent1> callback
        );

        void With<TComponent1, TComponent2>
        (
            TEntity entity,
            TComponentType componentType1,
            TComponentType componentType2,
            ActionRef<TEntity, TComponent1, TComponent2> callback
        );

        void With<TComponent1, TComponent2, TComponent3>
        (
            TEntity entity,
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            ActionRef<TEntity, TComponent1, TComponent2, TComponent3> callback
        );

        void With<TComponent1, TComponent2, TComponent3, TComponent4>
        (
            TEntity entity,
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            TComponentType componentType4,
            ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4> callback
        );

        void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>
        (
            TEntity entity,
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            TComponentType componentType4,
            TComponentType componentType5,
            ActionRef<TEntity, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback
        );
    }
}