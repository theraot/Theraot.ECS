namespace Theraot.ECS
{
    public delegate void ActionRef<in T, TArg>(T obj, ref TArg component1);

    public delegate void ActionRef<in T, TArg1, TArg2>(T obj, ref TArg1 component1, ref TArg2 component2);

    public delegate void ActionRef<in T, TArg1, TArg2, TArg3>(T obj, ref TArg1 component1, ref TArg2 component2, ref TArg3 component3);

    public delegate void ActionRef<in T, TArg1, TArg2, TArg3, TArg4>(T obj, ref TArg1 component1, ref TArg2 component2, ref TArg3 component3, ref TArg4 component4);

    public delegate void ActionRef<in T, TArg1, TArg2, TArg3, TArg4, TArg5>(T obj, ref TArg1 component1, ref TArg2 component2, ref TArg3 component3, ref TArg4 component4, ref TArg5 component5);
}