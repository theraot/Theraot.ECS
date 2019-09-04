namespace Theraot.ECS
{
    internal interface IComponentReferenceAccess<TEntityId, in TComponentKind>
    {
        void With<TComponent1>
        (
            TEntityId entityId,
            TComponentKind componentKind1,
            ActionRef<TEntityId, TComponent1> callback
        );

        void With<TComponent1, TComponent2>
        (
            TEntityId entityId,
            TComponentKind componentKind1,
            TComponentKind componentKind2,
            ActionRef<TEntityId, TComponent1, TComponent2> callback
        );

        void With<TComponent1, TComponent2, TComponent3>
        (
            TEntityId entityId,
            TComponentKind componentKind1,
            TComponentKind componentKind2,
            TComponentKind componentKind3,
            ActionRef<TEntityId, TComponent1, TComponent2, TComponent3> callback
        );

        void With<TComponent1, TComponent2, TComponent3, TComponent4>
        (
            TEntityId entityId,
            TComponentKind componentKind1,
            TComponentKind componentKind2,
            TComponentKind componentKind3,
            TComponentKind componentKind4,
            ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4> callback
        );

        void With<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>
        (
            TEntityId entityId,
            TComponentKind componentKind1,
            TComponentKind componentKind2,
            TComponentKind componentKind3,
            TComponentKind componentKind4,
            TComponentKind componentKind5,
            ActionRef<TEntityId, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> callback
        );
    }
}