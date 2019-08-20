namespace Theraot.ECS
{
    internal interface IComponentRef<in TComponentType>
    {
        ref TComponent GetComponentRef<TComponent>(TComponentType componentType);
    }
}