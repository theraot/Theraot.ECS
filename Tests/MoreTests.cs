using NUnit.Framework;
using System;
using System.Collections.Generic;
using Theraot.ECS;

namespace Tests
{
    public static class MoreTests
    {
        [Test]
        public static void CreateScopeWithNullComponentKindManagerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => Scope.CreateScope<int, int, int>(EqualityComparer<int>.Default, null));
        }

        [Test]
        public static void DestroyExistingEntity()
        {
            var scope = Scope.CreateScope(EqualityComparer<int>.Default, new SetManager());
            scope.RegisterEntity(0);
            Assert.AreEqual(true, scope.GetAllEntities().Contains(0));
            scope.DestroyEntity(0);
            Assert.AreEqual(false, scope.GetAllEntities().Contains(0));
        }

        [Test]
        public static void DestroyNonExistingEntity()
        {
            var scope = Scope.CreateScope(EqualityComparer<int>.Default, new SetManager());
            Assert.Throws<KeyNotFoundException>(() => scope.DestroyEntity(0));
        }
    }
}