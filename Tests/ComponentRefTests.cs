using NUnit.Framework;
using System.Collections.Generic;
using Theraot.ECS;

namespace Tests
{
    public static class ComponentRefTests
    {
        [Test]
        public static void RefToRemovedComponentIsSafe()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new SetManager());
            var entityA = scope.CreateEntity();
            scope.SetComponent(entityA, "narf", 123);
            scope.SetComponent(entityA, "puff", 456);
            scope.SetComponent(entityA, "zorg", 789);
            scope.With
            (
                entityA,
                "puff",
                (int _, ref int component) =>
                {
                    Assert.AreEqual(456, component);
                    component = 546;
                    Assert.AreEqual(546, scope.GetComponent<int>(entityA, "puff"));
                    scope.UnsetComponent(entityA, "puff");
                    Assert.AreEqual(546, scope.GetComponent<int>(entityA, "puff"));
                    Assert.AreEqual(546, component);
                }
            );
            Assert.Throws<KeyNotFoundException>(() => scope.GetComponent<int>(entityA, "puff"));
        }

        [Test]
        public static void UpdatingTheRefWorks()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new SetManager());
            var entityA = scope.CreateEntity();
            scope.SetComponent(entityA, "puff", 123);
            scope.With
            (
                entityA,
                "puff",
                (int _, ref int component) =>
                {
                    Assert.AreEqual(123, component);
                    component = 546;
                    Assert.AreEqual(546, scope.GetComponent<int>(entityA, "puff"));
                }
            );
        }

        [Test]
        public static void UpdatingTheRefWorksRegardlessOfAddedComponentsA()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new FlagArrayManager(16));
            var entityA = scope.CreateEntity();
            scope.SetComponent(entityA, 1, 123);
            scope.With
            (
                entityA,
                1,
                (int _, ref int component) =>
                {
                    Assert.AreEqual(123, component);
                    component = 546;
                    Assert.AreEqual(546, scope.GetComponent<int>(entityA, 1));
                    scope.SetComponent(entityA, 0, 100);
                    scope.SetComponent(entityA, 2, 200);
                    Assert.AreEqual(546, scope.GetComponent<int>(entityA, 1));
                    component = 741;
                    Assert.AreEqual(741, scope.GetComponent<int>(entityA, 1));
                }
            );
        }

        [Test]
        public static void UpdatingTheRefWorksRegardlessOfAddedComponentsB()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new SetManager());
            var entityA = scope.CreateEntity();
            scope.SetComponent(entityA, "puff", 123);
            scope.With
            (
                entityA,
                "puff",
                (int _, ref int component) =>
                {
                    Assert.AreEqual(123, component);
                    component = 546;
                    Assert.AreEqual(546, scope.GetComponent<int>(entityA, "puff"));
                    scope.SetComponent(entityA, "narf", 100);
                    scope.SetComponent(entityA, "zorg", 200);
                    component = 741;
                    Assert.AreEqual(741, scope.GetComponent<int>(entityA, "puff"));
                }
            );
        }
    }
}