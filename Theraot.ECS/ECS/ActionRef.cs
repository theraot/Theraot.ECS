namespace Theraot.ECS
{
    public delegate void ActionRef<in T, TComponent1>(T obj, ref TComponent1 component1);

    public delegate void ActionRef<in T, TComponent1, TComponent2>(T obj, ref TComponent1 component1, ref TComponent2 component2);

    public delegate void ActionRef<in T, TComponent1, TComponent2, TComponent3>(T obj, ref TComponent1 component1, ref TComponent2 component2, ref TComponent3 component3);

    public delegate void ActionRef<in T, TComponent1, TComponent2, TComponent3, TComponent4>(T obj, ref TComponent1 component1, ref TComponent2 component2, ref TComponent3 component3, ref TComponent4 component4);

    public delegate void ActionRef<in T, TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(T obj, ref TComponent1 component1, ref TComponent2 component2, ref TComponent3 component3, ref TComponent4 component4, ref TComponent5 component5);
}