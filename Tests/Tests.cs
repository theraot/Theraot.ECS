using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.ECS;

namespace Tests
{
    public static class Tests
    {
        [Test]
        public static void GetMissingComponent()
        {
            GetMissingComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, Guid.NewGuid());
            GetMissingComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0);
        }

        [Test]
        public static void QueryAfterEntities()
        {
            QueryAfterEntities(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(1)), 0, Guid.NewGuid(), Guid.NewGuid());
            QueryAfterEntities(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0, 1);
        }

        [Test]
        public static void QueryAllUpdateOnAddedComponent()
        {
            QueryAllUpdateOnAddedComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(1)), 0, Guid.NewGuid());
            QueryAllUpdateOnAddedComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0);
        }

        [Test]
        public static void QueryAllUpdateOnAddedComponents()
        {
            QueryAllUpdateOnAddedComponents(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            QueryAllUpdateOnAddedComponents(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void QueryAllUpdateOnRemoveComponent()
        {
            QueryAllUpdateOnRemoveComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            QueryAllUpdateOnRemoveComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void QueryAllUpdateOnRemoveComponents()
        {
            QueryAllUpdateOnRemoveComponents(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            QueryAllUpdateOnRemoveComponents(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void QueryAnyUpdateOnAddedComponent()
        {
            QueryAnyUpdateOnAddedComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(1)), 0, Guid.NewGuid());
            QueryAnyUpdateOnAddedComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0);
        }

        [Test]
        public static void QueryAnyUpdateOnAddedComponents()
        {
            QueryAnyUpdateOnAddedComponents(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            QueryAnyUpdateOnAddedComponents(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void QueryAnyUpdateOnRemovedComponent()
        {
            QueryAnyUpdateOnRemovedComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            QueryAnyUpdateOnRemovedComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void QueryAnyUpdateOnRemovedComponents()
        {
            QueryAnyUpdateOnRemovedComponents(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            QueryAnyUpdateOnRemovedComponents(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void QueryBeforeEntities()
        {
            QueryBeforeEntities(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(1)), 0, Guid.NewGuid(), Guid.NewGuid());
            QueryBeforeEntities(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0, 1);
        }

        [Test]
        public static void QueryNoneUpdateOnAddedComponent()
        {
            QueryNoneUpdateOnAddedComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(1)), 0, Guid.NewGuid());
            QueryNoneUpdateOnAddedComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0);
        }

        [Test]
        public static void QueryNoneUpdateOnAddedComponents()
        {
            QueryNoneUpdateOnAddedComponents(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            QueryNoneUpdateOnAddedComponents(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void QueryNoneUpdateOnRemovedComponent()
        {
            QueryNoneUpdateOnRemovedComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            QueryNoneUpdateOnRemovedComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void QueryNoneUpdateOnRemovedComponents()
        {
            QueryNoneUpdateOnRemovedComponents(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            QueryNoneUpdateOnRemovedComponents(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void RecoverObject()
        {
            RecoverObject(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(1)), 0, Guid.NewGuid());
            RecoverObject(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0);
        }

        [Test]
        public static void RecoverValueType()
        {
            RecoverValueType(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(1)), 0, Guid.NewGuid());
            RecoverValueType(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0);
        }

        [Test]
        public static void SetMultipleComponents()
        {
            SetMultipleComponents(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid());
            SetMultipleComponents(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0);
        }

        [Test]
        public static void SimpleGetComponent()
        {
            SimpleGetComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(1)), 0, Guid.NewGuid());
            SimpleGetComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0);
        }

        [Test]
        public static void TryGetComponent()
        {
            TryGetComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid(), Guid.NewGuid());
            TryGetComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0, 1);
        }

        [Test]
        public static void UnsetComponent()
        {
            UnsetComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, 1, Guid.NewGuid(), Guid.NewGuid());
            UnsetComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", 0, 1);
        }

        [Test]
        public static void UnsetMissingComponent()
        {
            UnsetMissingComponent(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2)), 0, Guid.NewGuid());
            UnsetMissingComponent(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", 0);
        }

        [Test]
        public static void UnsetMultipleComponents()
        {
            UnsetMultipleComponents(Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(3)), 0, 1, 2, Guid.NewGuid());
            UnsetMultipleComponents(Scope.CreateScope(EqualityComparer<int>.Default, new SetManager()), "a", "b", "c", 0);
        }

        private static void GetMissingComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            Assert.Throws<KeyNotFoundException>(() => scope.GetComponent<object>(entityA, type));
        }

        private static void QueryAfterEntities<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA, TEntity entityB)
        {
            scope.RegisterEntity(entityA);
            scope.RegisterEntity(entityB);
            scope.SetComponent(entityA, type, 100);
            var entities = scope.GetEntityCollection(new[] { type }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>()).ToArray();
            Assert.AreEqual(1, entities.Length);
            Assert.AreEqual(entityA, entities[0]);
            Assert.AreNotEqual(entityB, entities[0]);
        }

        private static void QueryAllUpdateOnAddedComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(new[] { type }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entitiesA = entities.ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponent(entityA, type, 100);
            var entitiesB = entities.ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
        }

        private static void QueryAllUpdateOnAddedComponents<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(new[] { typeA }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entitiesA = entities.ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new[] { typeA, typeB },
                new object[] { 100, 100 }
            );
            var entitiesB = entities.ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
        }

        private static void QueryAllUpdateOnRemoveComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(new[] { typeA }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entitiesA = entities.ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new[] { typeA, typeB },
                new object[] { 100, 100 }
            );
            var entitiesB = entities.ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
            scope.UnsetComponent(entityA, typeA);
            var entitiesC = entities.ToArray();
            Assert.AreEqual(0, entitiesC.Length);
        }

        private static void QueryAllUpdateOnRemoveComponents<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(new[] { typeA }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            var entitiesA = entities.ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new[] { typeA, typeB },
                new object[] { 100, 100 }
            );
            var entitiesB = entities.ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
            scope.UnsetComponents(entityA, typeA, typeB);
            var entitiesC = entities.ToArray();
            Assert.AreEqual(0, entitiesC.Length);
        }

        private static void QueryAnyUpdateOnAddedComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(Array.Empty<TComponentType>(), new[] { type }, Array.Empty<TComponentType>());
            var entitiesA = entities.ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponent(entityA, type, 100);
            var entitiesB = entities.ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
        }

        private static void QueryAnyUpdateOnAddedComponents<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(Array.Empty<TComponentType>(), new[] { typeA }, Array.Empty<TComponentType>());
            var entitiesA = entities.ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new[] { typeA, typeB },
                new object[] { 100, 100 }
            );
            var entitiesB = entities.ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
        }

        private static void QueryAnyUpdateOnRemovedComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(Array.Empty<TComponentType>(), new[] { typeA }, Array.Empty<TComponentType>());
            var entitiesA = entities.ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new[] { typeA, typeB },
                new object[] { 100, 100 }
            );
            var entitiesB = entities.ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
            scope.UnsetComponent(entityA, typeA);
            var entitiesC = entities.ToArray();
            Assert.AreEqual(0, entitiesC.Length);
        }

        private static void QueryAnyUpdateOnRemovedComponents<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(Array.Empty<TComponentType>(), new[] { typeA }, Array.Empty<TComponentType>());
            var entitiesA = entities.ToArray();
            Assert.AreEqual(0, entitiesA.Length);
            scope.SetComponents
            (
                entityA,
                new[] { typeA, typeB },
                new object[] { 100, 100 }
            );
            var entitiesB = entities.ToArray();
            Assert.AreEqual(1, entitiesB.Length);
            Assert.AreEqual(entityA, entitiesB[0]);
            scope.UnsetComponents(entityA, typeA, typeB);
            var entitiesC = entities.ToArray();
            Assert.AreEqual(0, entitiesC.Length);
        }

        private static void QueryBeforeEntities<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA, TEntity entityB)
        {
            var entities = scope.GetEntityCollection(new[] { type }, Array.Empty<TComponentType>(), Array.Empty<TComponentType>());
            scope.RegisterEntity(entityA);
            scope.RegisterEntity(entityB);
            scope.SetComponent(entityA, type, 100);
            var entitiesA = entities.ToArray();
            Assert.AreEqual(1, entitiesA.Length);
            Assert.AreEqual(entityA, entitiesA[0]);
            Assert.AreNotEqual(entityB, entitiesA[0]);
        }

        private static void QueryNoneUpdateOnAddedComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(Array.Empty<TComponentType>(), Array.Empty<TComponentType>(), new[] { type });
            var entitiesA = entities.ToArray();
            Assert.AreEqual(1, entitiesA.Length);
            Assert.AreEqual(entityA, entitiesA[0]);
            scope.SetComponent(entityA, type, 100);
            var entitiesB = entities.ToArray();
            Assert.AreEqual(0, entitiesB.Length);
        }

        private static void QueryNoneUpdateOnAddedComponents<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(Array.Empty<TComponentType>(), Array.Empty<TComponentType>(), new[] { typeA });
            var entitiesA = entities.ToArray();
            Assert.AreEqual(1, entitiesA.Length);
            Assert.AreEqual(entityA, entitiesA[0]);
            scope.SetComponents
            (
                entityA,
                new[] { typeA, typeB },
                new object[] { 100, 100 }
            );
            var entitiesB = entities.ToArray();
            Assert.AreEqual(0, entitiesB.Length);
        }

        private static void QueryNoneUpdateOnRemovedComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(Array.Empty<TComponentType>(), Array.Empty<TComponentType>(), new[] { typeA });
            var entitiesA = entities.ToArray();
            Assert.AreEqual(1, entitiesA.Length);
            Assert.AreEqual(entityA, entitiesA[0]);
            scope.SetComponents
            (
                entityA,
                new[] { typeA, typeB },
                new object[] { 100, 100 }
            );
            var entitiesB = entities.ToArray();
            Assert.AreEqual(0, entitiesB.Length);
            scope.UnsetComponent(entityA, typeA);
            var entitiesC = entities.ToArray();
            Assert.AreEqual(1, entitiesC.Length);
            Assert.AreEqual(entityA, entitiesC[0]);
        }

        private static void QueryNoneUpdateOnRemovedComponents<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var entities = scope.GetEntityCollection(Array.Empty<TComponentType>(), Array.Empty<TComponentType>(), new[] { typeA });
            var entitiesA = entities.ToArray();
            Assert.AreEqual(1, entitiesA.Length);
            Assert.AreEqual(entityA, entitiesA[0]);
            scope.SetComponents
            (
                entityA,
                new[] { typeA, typeB },
                new object[] { 100, 100 }
            );
            var entitiesB = entities.ToArray();
            Assert.AreEqual(0, entitiesB.Length);
            scope.UnsetComponents(entityA, typeA, typeB);
            var entitiesC = entities.ToArray();
            Assert.AreEqual(1, entitiesC.Length);
            Assert.AreEqual(entityA, entitiesC[0]);
        }

        private static void RecoverObject<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var obj = new object();
            scope.SetComponent(entityA, type, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entityA, type));
        }

        private static void RecoverValueType<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            scope.SetComponent(entityA, type, 42);
            Assert.AreEqual(42, scope.GetComponent<int>(entityA, type));
        }

        private static void SetMultipleComponents<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType componentTypeA, TComponentType componentTypeB, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var objA = new object();
            var objB = new object();
            scope.SetComponents
                (
                    entityA,
                    new[] { componentTypeA, componentTypeB },
                    new[] { objA, objB }
                );
            Assert.IsTrue(scope.TryGetComponent<object>(entityA, componentTypeA, out var componentA));
            Assert.AreEqual(objA, componentA);
            Assert.IsTrue(scope.TryGetComponent<object>(entityA, componentTypeB, out var componentB));
            Assert.AreEqual(objB, componentB);
        }

        private static void SimpleGetComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var obj = new object();
            scope.SetComponent(entityA, type, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entityA, type));
        }

        private static void TryGetComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA, TEntity entityB)
        {
            scope.RegisterEntity(entityA);
            scope.RegisterEntity(entityB);
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

        private static void UnsetComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType typeA, TComponentType typeB, TEntity entityA, TEntity entityB)
        {
            scope.RegisterEntity(entityA);
            scope.RegisterEntity(entityB);
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

        private static void UnsetMissingComponent<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType type, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            scope.UnsetComponent(entityA, type);
        }

        private static void UnsetMultipleComponents<TEntity, TComponentType>(Scope<TEntity, TComponentType> scope, TComponentType componentTypeA, TComponentType componentTypeB, TComponentType componentTypeC, TEntity entityA)
        {
            scope.RegisterEntity(entityA);
            var objA = new object();
            var objB = new object();
            var objC = new object();
            scope.SetComponents
            (
                entityA,
                new[] { componentTypeA, componentTypeB, componentTypeC },
                new[] { objA, objB, objC }
            );
            Assert.IsTrue(scope.TryGetComponent<object>(entityA, componentTypeA, out var componentA));
            Assert.AreEqual(objA, componentA);
            Assert.IsTrue(scope.TryGetComponent<object>(entityA, componentTypeB, out var componentB));
            Assert.AreEqual(objB, componentB);
            scope.UnsetComponents(entityA, componentTypeA, componentTypeB);
            Assert.IsFalse(scope.TryGetComponent<object>(entityA, componentTypeA, out _));
            Assert.IsFalse(scope.TryGetComponent<object>(entityA, componentTypeB, out _));
            Assert.IsTrue(scope.TryGetComponent<object>(entityA, componentTypeC, out var componentC));
            Assert.AreEqual(objC, componentC);
            scope.UnsetComponents(entityA, componentTypeC, componentTypeB);
            Assert.IsFalse(scope.TryGetComponent<object>(entityA, componentTypeC, out _));
        }
    }
}