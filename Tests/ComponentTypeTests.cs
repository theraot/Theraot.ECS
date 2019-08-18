using NUnit.Framework;
using System;
using Theraot.ECS;

namespace Tests
{
    public class ComponentTypeTests
    {
        [Test]
        public void ComponentTypeIsSetForAllEntities()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new SetManager());
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            scope.SetComponent(entityA, "test", 1);
            Assert.Throws<ArgumentException>(() => scope.SetComponent(entityB, "test", "hello"));
        }

        [Test]
        public void UnableToChangeComponentType()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new SetManager());
            var entityA = scope.CreateEntity();
            scope.SetComponent(entityA, "test", 1);
            Assert.Throws<ArgumentException>(() => scope.SetComponent(entityA, "test", "hello"));
        }
    }
}