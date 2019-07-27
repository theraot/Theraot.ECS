#pragma warning disable RECS0096 // Type parameter is never used
// ReSharper disable UnusedTypeParameter

using System;
using QueryId = System.Int32;

namespace Theraot.ECS
{
    public sealed partial class Scope<TEntity, TComponentType, TComponentTypeSet, TQuery>
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

            var queryId = CreateQuery
            (
                new[]
                {
                    GetComponentType<TComponent>()
                },
                Array.Empty<TComponentType>(),
                Array.Empty<TComponentType>()
            );
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

            var queryId = CreateQuery
            (
                new[]
                {
                    GetComponentType<TComponent1>(),
                    GetComponentType<TComponent2>()
                },
                Array.Empty<TComponentType>(),
                Array.Empty<TComponentType>()
            );
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

            var queryId = CreateQuery
            (
                new[]
                {
                    GetComponentType<TComponent1>(),
                    GetComponentType<TComponent2>(),
                    GetComponentType<TComponent3>()
                },
                Array.Empty<TComponentType>(),
                Array.Empty<TComponentType>()
            );
            foreach (var entity in GetEntities(queryId))
            {
                callback(entity, GetComponent<TComponent1>(entity), GetComponent<TComponent2>(entity), GetComponent<TComponent3>(entity));
            }
        }
    }
}