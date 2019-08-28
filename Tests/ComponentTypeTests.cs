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
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, EqualityComparer<int>.Default, new SetManager());
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            scope.SetComponent(entityA, "test", 1);
            Assert.AreEqual(typeof(int), scope.GetRegisteredComponentType("test"));
            Assert.Throws<ArgumentException>(() => scope.SetComponent(entityB, "test", "hello"));
        }

        [Test]
        public static void RegisterComponentType()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, EqualityComparer<int>.Default, new SetManager());
            scope.TryRegisterComponentType<int>("test");
            var entityA = scope.CreateEntity();
            Assert.AreEqual(typeof(int), scope.GetRegisteredComponentType("test"));
            Assert.Throws<ArgumentException>(() => scope.SetComponent(entityA, "test", "hello"));
        }

        [Test]
        public static void UnableToChangeComponentType()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, EqualityComparer<int>.Default, new SetManager());
            var entityA = scope.CreateEntity();
            scope.SetComponent(entityA, "test", 1);
            Assert.Throws<ArgumentException>(() => scope.SetComponent(entityA, "test", "hello"));
        }
    }
}