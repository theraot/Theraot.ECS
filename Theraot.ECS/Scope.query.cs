// ReSharper disable UnusedTypeParameter

using System;
using System.Collections.Generic;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public partial class Scope<TEntity, TComponentType, TQuery>
    {
        public void Query<TComponent>(QueryId queryId, Action<TEntity, TComponent> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entity in GetEntities(queryId))
            {
                callback(entity, GetComponent<TComponent>(entity));
            }
        }

        public void Query<TComponent1, TComponent2>(QueryId queryId, Action<TEntity, TComponent1, TComponent2> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entity in GetEntities(queryId))
            {
                callback(entity, GetComponent<TComponent1>(entity), GetComponent<TComponent2>(entity));
            }
        }

        public void Query<TComponent1, TComponent2, TComponent3>(QueryId queryId, Action<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            foreach (var entity in GetEntities(queryId))
            {
                callback(entity, GetComponent<TComponent1>(entity), GetComponent<TComponent2>(entity), GetComponent<TComponent3>(entity));
            }
        }

        public void Query<TComponent>(Action<TEntity, TComponent> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var query = _strategy.CreateQuery
            (
                new HashSet<TComponentType>
                (
                    new[]
                    {
                        GetComponentType<TComponent>()
                    }
                ),
                new HashSet<TComponentType>(),
                new HashSet<TComponentType>()
            );
            var queryId = RegisterQuery(query);
            foreach (var entity in GetEntities(queryId))
            {
                callback(entity, GetComponent<TComponent>(entity));
            }
        }

        public void Query<TComponent1, TComponent2>(Action<TEntity, TComponent1, TComponent2> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var query = _strategy.CreateQuery
            (
                new HashSet<TComponentType>
                (
                    new[]
                    {
                        GetComponentType<TComponent1>(),
                        GetComponentType<TComponent2>()
                    }
                ),
                new HashSet<TComponentType>(),
                new HashSet<TComponentType>()
            );
            var queryId = RegisterQuery(query);
            foreach (var entity in GetEntities(queryId))
            {
                callback(entity, GetComponent<TComponent1>(entity), GetComponent<TComponent2>(entity));
            }
        }

        public void Query<TComponent1, TComponent2, TComponent3>(Action<TEntity, TComponent1, TComponent2, TComponent3> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var query = _strategy.CreateQuery
            (
                new HashSet<TComponentType>
                (
                    new[]
                    {
                        GetComponentType<TComponent1>(),
                        GetComponentType<TComponent2>(),
                        GetComponentType<TComponent3>()
                    }
                ),
                new HashSet<TComponentType>(),
                new HashSet<TComponentType>()
            );
            var queryId = RegisterQuery(query);
            foreach (var entity in GetEntities(queryId))
            {
                callback(entity, GetComponent<TComponent1>(entity), GetComponent<TComponent2>(entity), GetComponent<TComponent3>(entity));
            }
        }
    }
}