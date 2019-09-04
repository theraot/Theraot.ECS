namespace Theraot.ECS
{
    internal interface IComponentReferenceAccess<TEntityId, in TComponentType>
    {
        void With<TComponent1>
        (
            TEntityId entityId,
            TComponentType componentType1,
            ActionRef<TEntityId, TComponent1> callback
        );

        void With<TComponent1, TComponent2>
        (
            TEntityId entityId,
            TComponentType componentType1,
            TComponentType componentType2,
            ActionRef<TEntityId, TComponent1, TComponent2> callback
        );

        void With<TComponent1, TComponent2, TComponent3>
        (
            TEntityId entityId,
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            ActionRef<TEntityId, TComponent1, TComponent2, TComponent3> callback
        );

        void With<TComponent1, TComponent2, TComponent3, TComponent4>
        (
            TEntityId entityId,
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            TComponentType componentType4,
            ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4> callback
        );

        void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>
        (
            TEntityId entityId,
            TComponentType componentType1,
            TComponentType componentType2,
            TComponentType componentType3,
            TComponentType componentType4,
            TComponentType componentType5,
            ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback
        );
    }
}