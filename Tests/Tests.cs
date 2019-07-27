using NUnit.Framework;
using Theraot.ECS;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void RecoverValueTypeBitArray()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new BitArrayStrategy(1));
            var entity = scope.CreateEntity();
            scope.SetComponent(entity, 42);
            Assert.AreEqual(42, scope.GetComponent<int>(entity));
        }

        [Test]
        public void RecoverValueTypeTypeHashSet()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new TypeHashSetStrategy());
            var entity = scope.CreateEntity();
            scope.SetComponent(entity, 42);
            Assert.AreEqual(42, scope.GetComponent<int>(entity));
        }

        [Test]
        public void RecoverObjectBitArray()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new BitArrayStrategy(1));
            var entity = scope.CreateEntity();
            var obj = new object();
            scope.SetComponent(entity, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entity));
        }

        [Test]
        public void RecoverObjectTypeHashSet()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new TypeHashSetStrategy());
            var entity = scope.CreateEntity();
            var obj = new object();
            scope.SetComponent(entity, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entity));
        }
    }
}