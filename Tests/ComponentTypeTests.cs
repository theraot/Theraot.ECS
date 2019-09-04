using NUnit.Framework;
using System;
using System.Collections.Generic;
using Theraot.ECS;

namespace Tests
{
    public static class ComponentTypeTests
    {
        [Test]
        public static void ComponentTypeIsSetForAllEntities()
        {
            var scope = Scope.CreateScope(EqualityComparer<int>.Default, new SetManager());
            const int entityA = 0;
            const int entityB = 0;
            scope.RegisterEntity(entityA);
            scope.RegisterEntity(entityB);
            scope.SetComponent(entityA, "test", 1);
            Assert.AreEqual(typeof(int), scope.GetRegisteredType("test"));
            Assert.Throws<ArgumentException>(() => scope.SetComponent(entityB, "test", "hello"));
        }

        [Test]
        public static void RegisterComponentType()
        {
            var scope = Scope.CreateScope(EqualityComparer<int>.Default, new SetManager());
            scope.TryRegisterType<int>("test");
            const int entityA = 0;
            scope.RegisterEntity(entityA);
            Assert.AreEqual(typeof(int), scope.GetRegisteredType("test"));
            Assert.Throws<ArgumentException>(() => scope.SetComponent(entityA, "test", "hello"));
        }

        [Test]
        public static void UnableToChangeComponentType()
        {
            var scope = Scope.CreateScope(EqualityComparer<int>.Default, new SetManager());
            const int entityA = 0;
            scope.RegisterEntity(entityA);
            scope.SetComponent(entityA, "test", 1);
            Assert.Throws<ArgumentException>(() => scope.SetComponent(entityA, "test", "hello"));
        }
    }
}