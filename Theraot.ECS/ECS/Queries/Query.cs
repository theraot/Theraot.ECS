namespace Theraot.ECS.Queries
{
    internal sealed class Query<TComponentKindSet>
    {
        public Query(TComponentKindSet all, TComponentKindSet any, TComponentKindSet none)
        {
            All = all;
            Any = any;
            None = none;
        }

        public TComponentKindSet All { get; }

        public TComponentKindSet Any { get; }

        public TComponentKindSet None { get; }
    }
}