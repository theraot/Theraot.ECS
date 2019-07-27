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
            var scope = Scope.CreateScope(() => entityId++, new TypeHashSetStrategy());
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            Assert.AreNotEqual(entityA, entityB);
        }

        [Test]
        public void GuidBitArray()
        {
            var scope = Scope.CreateScope(System.Guid.NewGuid, new BitArrayStrategy(1));
            var entity = scope.CreateEntity();
            var obj = new object();
            scope.SetComponent(entity, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entity));
        }

        [Test]
        public void GuidTypeHashSet()
        {
            var scope = Scope.CreateScope(System.Guid.NewGuid, new TypeHashSetStrategy());
            var entity = scope.CreateEntity();
            var obj = new object();
            scope.SetComponent(entity, obj);
            Assert.AreEqual(obj, scope.GetComponent<object>(entity));
        }

        [Test]
        public void QueryAfterEntities()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new TypeHashSetStrategy());
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            scope.SetComponent(entityA, 100);
            var query = scope.CreateQuery(new[] { typeof(int) }, System.Type.EmptyTypes, System.Type.EmptyTypes);
            var entities = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entities.Length);
            Assert.AreEqual(entityA, entities[0]);
            Assert.AreNotEqual(entityB, entities[0]);
        }

        [Test]
        public void QueryBeforeEntities()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new TypeHashSetStrategy());
            var query = scope.CreateQuery(new[] { typeof(int) }, System.Type.EmptyTypes, System.Type.EmptyTypes);
            var entityA = scope.CreateEntity();
            var entityB = scope.CreateEntity();
            scope.SetComponent(entityA, 100);
            var entities = scope.GetEntities(query).ToArray();
            Assert.AreEqual(1, entities.Length);
            Assert.AreEqual(entityA, entities[0]);
            Assert.AreNotEqual(entityB, entities[0]);
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
    }
}