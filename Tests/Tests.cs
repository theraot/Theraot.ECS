using System;
using System.Linq;
using NUnit.Framework;
using Theraot.ECS;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void CreateEntityAreDifferent()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new DummyStrategy());
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            Assert.AreNotEqual(entityA, entityB);
        }

        [Test]
        public void Guid()
        {
            Guid(Scope.CreateScope(System.Guid.NewGuid, new BitArrayStrategy(1)), 0);
            Guid(Scope.CreateScope(IncrementingInt(), new TypeHashSetStrategy()), "a");
        }

        [Test]
        public void QueryAfterEntities()
        {
            QueryAfterEntities(Scope.CreateScope(System.Guid.NewGuid, new BitArrayStrategy(1)), 0);
            QueryAfterEntities(Scope.CreateScope(IncrementingInt(), new TypeHashSetStrategy()), "a");
        }

        [Test]
        public void QueryBeforeEntities()
        {
            QueryBeforeEntities(Scope.CreateScope(System.Guid.NewGuid, new BitArrayStrategy(1)), 0);
            QueryBeforeEntities(Scope.CreateScope(IncrementingInt(), new TypeHashSetStrategy()), "a");
        }

        [Test]
        public void RecoverObject()
        {
            RecoverObject(Scope.CreateScope(System.Guid.NewGuid, new BitArrayStrategy(1)), 0);
            RecoverObject(Scope.CreateScope(IncrementingInt(), new TypeHashSetStrategy()), "a");
        }

        [Test]
        public void RecoverValueType()
        {
            RecoverValueType(Scope.CreateScope(System.Guid.NewGuid, new BitArrayStrategy(1)), 0);
            RecoverValueType(Scope.CreateScope(IncrementingInt(), new TypeHashSetStrategy()), "a");
        }

        private static Func<int> IncrementingInt()
        {
            var value = 0;
            return () => value++;
        }

        private static void Guid<TEntity, TComponentType> (IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entity = scope.CreateEntity();
            var obj = new object();
            scope.SetComponent(entity, type, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entity, type));
        }

        private static void QueryAfterEntities<TEntity, TComponentType> (IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            scope.SetComponent(entityA, type, 100);
            var query = scope.CreateQuery(new[] { type }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entities = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entities.Length);
            Assert.AreEqual(entityA, entities[0]);
            Assert.AreNotEqual(entityB, entities[0]);
        }

        private static void QueryBeforeEntities<TEntity, TComponentType> (IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var query = scope.CreateQuery(new[] { type }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            scope.SetComponent(entityA, type, 100);
            var entities = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entities.Length);
            Assert.AreEqual(entityA, entities[0]);
            Assert.AreNotEqual(entityB, entities[0]);
        }

        private static void RecoverObject<TEntity, TComponentType> (IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entity = scope.CreateEntity();
            var obj = new object();
            scope.SetComponent(entity, type, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entity, type));
        }

        private static void RecoverValueType<TEntity, TComponentType> (IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entity = scope.CreateEntity();
            scope.SetComponent(entity, type, 42);
            Assert.AreEqual(42, scope.GetComponent<int>(entity, type));
        }
    }
}