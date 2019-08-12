using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.ECS;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void CreateEntityAreDifferent()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new DummyManager());
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            Assert.AreNotEqual(entityA, entityB);
        }

        [Test]
        public void GetMissingComponent()
        {
            GetMissingComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0);
            GetMissingComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void Guid()
        {
            Guid(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(1)), 0);
            Guid(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void NotExistingQuery()
        {
            NotExistingQuery(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(1)));
            NotExistingQuery(Scope.CreateScope(IncrementingInt(), new SetManager()));
        }

        [Test]
        public void QueryAfterEntities()
        {
            QueryAfterEntities(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(1)), 0);
            QueryAfterEntities(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void QueryAllUpdateOnAddedComponent()
        {
            QueryAllUpdateOnAddedComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(1)), 0);
            QueryAllUpdateOnAddedComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void QueryAllUpdateOnAddedComponents()
        {
            QueryAllUpdateOnAddedComponents(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            QueryAllUpdateOnAddedComponents(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void QueryAllUpdateOnRemoveComponent()
        {
            QueryAllUpdateOnRemoveComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            QueryAllUpdateOnRemoveComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void QueryAllUpdateOnRemoveComponents()
        {
            QueryAllUpdateOnRemoveComponents(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            QueryAllUpdateOnRemoveComponents(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void QueryAnyUpdateOnAddedComponent()
        {
            QueryAnyUpdateOnAddedComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(1)), 0);
            QueryAnyUpdateOnAddedComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void QueryAnyUpdateOnAddedComponents()
        {
            QueryAnyUpdateOnAddedComponents(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            QueryAnyUpdateOnAddedComponents(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void QueryAnyUpdateOnRemovedComponent()
        {
            QueryAnyUpdateOnRemovedComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            QueryAnyUpdateOnRemovedComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void QueryAnyUpdateOnRemovedComponents()
        {
            QueryAnyUpdateOnRemovedComponents(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            QueryAnyUpdateOnRemovedComponents(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void QueryBeforeEntities()
        {
            QueryBeforeEntities(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(1)), 0);
            QueryBeforeEntities(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void QueryNoneUpdateOnAddedComponent()
        {
            QueryNoneUpdateOnAddedComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(1)), 0);
            QueryNoneUpdateOnAddedComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void QueryNoneUpdateOnAddedComponents()
        {
            QueryNoneUpdateOnAddedComponents(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            QueryNoneUpdateOnAddedComponents(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void QueryNoneUpdateOnRemovedComponent()
        {
            QueryNoneUpdateOnRemovedComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            QueryNoneUpdateOnRemovedComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void QueryNoneUpdateOnRemovedComponents()
        {
            QueryNoneUpdateOnRemovedComponents(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            QueryNoneUpdateOnRemovedComponents(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void RecoverObject()
        {
            RecoverObject(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(1)), 0);
            RecoverObject(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void RecoverValueType()
        {
            RecoverValueType(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(1)), 0);
            RecoverValueType(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void SetMultipleComponents()
        {
            SetMultipleComponents(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            SetMultipleComponents(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void TryGetComponent()
        {
            TryGetComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            TryGetComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void UnsetComponent()
        {
            UnsetComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0, 1);
            UnsetComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b");
        }

        [Test]
        public void UnsetMissingComponent()
        {
            UnsetMissingComponent(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(2)), 0);
            UnsetMissingComponent(Scope.CreateScope(IncrementingInt(), new SetManager()), "a");
        }

        [Test]
        public void UnsetMultipleComponents()
        {
            UnsetMultipleComponents(Scope.CreateScope(System.Guid.NewGuid, new FlagArrayManager(3)), 0, 1, 2);
            UnsetMultipleComponents(Scope.CreateScope(IncrementingInt(), new SetManager()), "a", "b", "c");
        }

        private static void GetMissingComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entityA = scope.CreateEntity();
            Assert.AreEqual(null, scope.GetComponent<object>(entityA, type));
        }

        private static void Guid<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entity = scope.CreateEntity();
            var obj = new object();
            scope.SetComponent(entity, type, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entity, type));
        }

        private static Func<int> IncrementingInt()
        {
            var value = 0;
            return () => value++;
        }

        private static void NotExistingQuery<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope)
        {
            Assert.AreEqual(0, scope.GetEntities(0).ToArray().Length);
        }

        private static void QueryAfterEntities<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
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

        private static void QueryAllUpdateOnAddedComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(new[] { type }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponent(entityA, type, 100);
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
        }

        private static void QueryAllUpdateOnAddedComponents<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(new[] { typeA }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new Dictionary<TComponentType, object>
                {
                    {typeA, 100 },
                    {typeB, 100 }
                }
            );
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
        }

        private static void QueryAllUpdateOnRemoveComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(new[] { typeA }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new Dictionary<TComponentType, object>
                {
                    {typeA, 100 },
                    {typeB, 100 }
                }
            );
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
            scope.UnsetComponent(entityA, typeA);
            var entitiesC = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesC.Length);
        }

        private static void QueryAllUpdateOnRemoveComponents<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(new[] { typeA }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new Dictionary<TComponentType, object>
                {
                    {typeA, 100 },
                    {typeB, 100 }
                }
            );
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
            scope.UnsetComponents(entityA, typeA, typeB);
            var entitiesC = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesC.Length);
        }

        private static void QueryAnyUpdateOnAddedComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(Array.Empty<TComponentType>(), new[] { type }, Array.Empty<TComponentType>());
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponent(entityA, type, 100);
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
        }

        private static void QueryAnyUpdateOnAddedComponents<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(Array.Empty<TComponentType>(), new[] { typeA }, Array.Empty<TComponentType>());
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new Dictionary<TComponentType, object>
                {
                    {typeA, 100 },
                    {typeB, 100 }
                }
            );
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
        }

        private static void QueryAnyUpdateOnRemovedComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(Array.Empty<TComponentType>(), new[] { typeA }, Array.Empty<TComponentType>());
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new Dictionary<TComponentType, object>
                {
                    {typeA, 100 },
                    {typeB, 100 }
                }
            );
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
            scope.UnsetComponent(entityA, typeA);
            var entitiesC = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesC.Length);
        }

        private static void QueryAnyUpdateOnRemovedComponents<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(Array.Empty<TComponentType>(), new[] { typeA }, Array.Empty<TComponentType>());
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new Dictionary<TComponentType, object>
                {
                    {typeA, 100 },
                    {typeB, 100 }
                }
            );
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
            scope.UnsetComponents(entityA, typeA, typeB);
            var entitiesC = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesC.Length);
        }

        private static void QueryBeforeEntities<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
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

        private static void QueryNoneUpdateOnAddedComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(Array.Empty<TComponentType>(), Array.Empty<TComponentType>(), new[] { type });
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesA.Length);
            Assert.AreEqual(entityA, entitiesA[0]);
            scope.SetComponent(entityA, type, 100);
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesB.Length);
        }

        private static void QueryNoneUpdateOnAddedComponents<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(Array.Empty<TComponentType>(), Array.Empty<TComponentType>(), new[] { typeA });
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesA.Length);
            Assert.AreEqual(entityA, entitiesA[0]);
            scope.SetComponents
            (
                entityA,
                new Dictionary<TComponentType, object>
                {
                    {typeA, 100 },
                    {typeB, 100 }
                }
            );
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesB.Length);
        }

        private static void QueryNoneUpdateOnRemovedComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(Array.Empty<TComponentType>(), Array.Empty<TComponentType>(), new[] { typeA });
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesA.Length);
            Assert.AreEqual(entityA, entitiesA[0]);
            scope.SetComponents
            (
                entityA,
                new Dictionary<TComponentType, object>
                {
                    {typeA, 100 },
                    {typeB, 100 }
                }
            );
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesB.Length);
            scope.UnsetComponent(entityA, typeA);
            var entitiesC = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesC.Length);
            Assert.AreEqual(entityA, entitiesC[0]);
        }

        private static void QueryNoneUpdateOnRemovedComponents<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var query = scope.CreateQuery(Array.Empty<TComponentType>(), Array.Empty<TComponentType>(), new[] { typeA });
            var entitiesA = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesA.Length);
            Assert.AreEqual(entityA, entitiesA[0]);
            scope.SetComponents
            (
                entityA,
                new Dictionary<TComponentType, object>
                {
                    {typeA, 100 },
                    {typeB, 100 }
                }
            );
            var entitiesB = scope.GetEntities(query).ToArray();
            Assert.AreEqual(0, entitiesB.Length);
            scope.UnsetComponents(entityA, typeA, typeB);
            var entitiesC = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entitiesC.Length);
            Assert.AreEqual(entityA, entitiesC[0]);
        }

        private static void RecoverObject<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entity = scope.CreateEntity();
            var obj = new object();
            scope.SetComponent(entity, type, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entity, type));
        }

        private static void RecoverValueType<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entity = scope.CreateEntity();
            scope.SetComponent(entity, type, 42);
            Assert.AreEqual(42, scope.GetComponent<int>(entity, type));
        }

        private static void SetMultipleComponents<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType componentTypeA, TComponentType componentTypeB)
        {
            var entity = scope.CreateEntity();
            var objA = new object();
            var objB = new object();
            scope.SetComponents
                (
                    entity,
                    new Dictionary<TComponentType, object>
                    {
                        {componentTypeA, objA },
                        {componentTypeB, objB }
                    }
                );
            Assert.IsTrue(scope.TryGetComponent<object>(entity, componentTypeA, out var componentA));
            Assert.AreEqual(objA, componentA);
            Assert.IsTrue(scope.TryGetComponent<object>(entity, componentTypeB, out var componentB));
            Assert.AreEqual(objB, componentB);
        }

        private static void TryGetComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            var objA = new object();
            var objB = new object();
            Assert.IsFalse(scope.TryGetComponent<object>(entityA, typeA, out _));
            Assert.IsFalse(scope.TryGetComponent<object>(entityA, typeB, out _));
            Assert.IsFalse(scope.TryGetComponent<object>(entityB, typeA, out _));
            Assert.IsFalse(scope.TryGetComponent<object>(entityB, typeB, out _));
            scope.SetComponent(entityA, typeA, objA);
            scope.SetComponent(entityB, typeB, objB);
            Assert.IsTrue(scope.TryGetComponent(entityA, typeA, out object foundA));
            Assert.AreEqual(objA, foundA);
            Assert.IsFalse(scope.TryGetComponent<object>(entityA, typeB, out _));
            Assert.IsFalse(scope.TryGetComponent<object>(entityB, typeA, out _));
            Assert.IsTrue(scope.TryGetComponent(entityB, typeB, out object foundB));
            Assert.AreEqual(objB, foundB);
        }

        private static void UnsetComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB)
        {
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            var objA = new object();
            var objB = new object();
            scope.SetComponent(entityA, typeA, objA);
            scope.SetComponent(entityB, typeB, objB);
            Assert.AreEqual(objA, scope.GetComponent<object>(entityA, typeA));
            Assert.AreEqual(objB, scope.GetComponent<object>(entityB, typeB));
            scope.UnsetComponent(entityA, typeA);
            Assert.IsFalse(scope.TryGetComponent(entityA, typeA, out object _));
            scope.UnsetComponent(entityB, typeB);
            Assert.IsFalse(scope.TryGetComponent(entityB, typeB, out object _));
        }

        private static void UnsetMissingComponent<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType type)
        {
            var entityA = scope.CreateEntity();
            scope.UnsetComponent(entityA, type);
        }

        private static void UnsetMultipleComponents<TEntity, TComponentType>(IScope<TEntity, TComponentType> scope, TComponentType componentTypeA, TComponentType componentTypeB, TComponentType componentTypeC)
        {
            var entity = scope.CreateEntity();
            var objA = new object();
            var objB = new object();
            var objC = new object();
            scope.SetComponents
            (
                entity,
                new Dictionary<TComponentType, object>
                {
                    {componentTypeA, objA },
                    {componentTypeB, objB },
                    {componentTypeC, objC }
                }
            );
            Assert.IsTrue(scope.TryGetComponent<object>(entity, componentTypeA, out var componentA));
            Assert.AreEqual(objA, componentA);
            Assert.IsTrue(scope.TryGetComponent<object>(entity, componentTypeB, out var componentB));
            Assert.AreEqual(objB, componentB);
            scope.UnsetComponents(entity, componentTypeA, componentTypeB);
            Assert.IsFalse(scope.TryGetComponent<object>(entity, componentTypeA, out _));
            Assert.IsFalse(scope.TryGetComponent<object>(entity, componentTypeB, out _));
            Assert.IsTrue(scope.TryGetComponent<object>(entity, componentTypeC, out var componentC));
            Assert.AreEqual(objC, componentC);
            scope.UnsetComponents(entity, componentTypeC, componentTypeB);
            Assert.IsFalse(scope.TryGetComponent<object>(entity, componentTypeC, out _));
        }
    }
}