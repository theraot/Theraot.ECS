namespace Theraot.ECS.Mantle.Queries
{
    internal sealed class Query<TComponentTypeSet>
    {
        public Query(TComponentTypeSet all, TComponentTypeSet any, TComponentTypeSet none)
        {
            All = all;
            Any = any;
            None = none;
        }

        public TComponentTypeSet All { get; }

        public TComponentTypeSet Any { get; }

        public TComponentTypeSet None { get; }
    }
}