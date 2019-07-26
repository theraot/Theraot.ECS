using System;
using System.Collections.Generic;
using Component = System.Object;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public interface IScope<TEntity, in TQuery>
    {
        void SetComponent<TComponent>(TEntity entity, TComponent component);
        void SetComponent<TComponent1, TComponent2>(TEntity entity, TComponent1 component1, TComponent2 component2);
        void SetComponent<TComponent1, TComponent2, TComponent3>(TEntity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3);
        void SetComponent<TComponent1, TComponent2, TComponent3, TComponent4>(TEntity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3, TComponent4 component4);
        void SetComponent(TEntity entity, params Component[] components);
        void Query<TComponent>(QueryId queryId, Action<TEntity, TComponent> callback);
        void Query<TComponent1, TComponent2>(QueryId queryId, Action<TEntity, TComponent1, TComponent2> callback);
        void Query<TComponent1, TComponent2, TComponent3>(QueryId queryId, Action<TEntity, TComponent1, TComponent2, TComponent3> callback);
        void Query<TComponent>(Action<TEntity, TComponent> callback);
        void Query<TComponent1, TComponent2>(Action<TEntity, TComponent1, TComponent2> callback);
        void Query<TComponent1, TComponent2, TComponent3>(Action<TEntity, TComponent1, TComponent2, TComponent3> callback);
        TEntity CreateEntity();
        TComponent GetComponent<TComponent>(TEntity entity);
        IEnumerable<TEntity> GetEntities(QueryId query);
        QueryId RegisterQuery(TQuery query);
        bool TryGetComponent<TComponent>(TEntity entity, out TComponent component);
    }
}