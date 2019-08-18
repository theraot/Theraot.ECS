using NUnit.Framework;
using Theraot.ECS;

namespace Tests
{
    public class ComponentRefTests
    {
        [Test]
        public void UpdatingTheRefWorks()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new SetManager());
            var entityA = scope.CreateEntity();
            scope.SetComponent(entityA, "puff", 123);
            ref var component = ref scope.GetComponentRef<int>(entityA, "puff");
            Assert.AreEqual(123, component);
            component = 546;
            Assert.AreEqual(546, scope.GetComponent<int>(entityA, "puff"));
        }
    }
}