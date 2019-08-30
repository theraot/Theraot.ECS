using NUnit.Framework;
using System.Collections.Generic;
using Theraot.ECS;

namespace Tests
{
    public static class QueryTests
    {
        [Test]
        public static void CreatingTheSameQueryMultipleTimes()
        {
            var scope = Scope.CreateScope(EqualityComparer<int>.Default, new FlagArrayManager(8));
            var entitiesA = scope.GetEntityCollection(new[] { 1, 5 }, new[] { 6 }, new[] { 7 });
            var entitiesB = scope.GetEntityCollection(new[] { 1, 5 }, new[] { 6 }, new[] { 7 });
            var entitiesC = scope.GetEntityCollection(new[] { 0, 5 }, new[] { 6 }, new[] { 7 });
            var entitiesE = scope.GetEntityCollection(new[] { 2, 5 }, new[] { 6 }, new[] { 7 });
            var entitiesD = scope.GetEntityCollection(new[] { 0, 5 }, new[] { 6 }, new[] { 7 });
            var entitiesF = scope.GetEntityCollection(new[] { 2, 5 }, new[] { 6 }, new[] { 7 });
            Assert.AreSame(entitiesA, entitiesB);
            Assert.AreSame(entitiesC, entitiesD);
            Assert.AreSame(entitiesE, entitiesF);
            Assert.AreNotSame(entitiesA, entitiesC);
            Assert.AreNotSame(entitiesA, entitiesE);
            Assert.AreNotSame(entitiesC, entitiesE);
        }
    }
}