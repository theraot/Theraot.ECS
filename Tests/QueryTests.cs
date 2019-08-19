using NUnit.Framework;
using Theraot.ECS;

namespace Tests
{
    public class QueryTests
    {
        [Test]
        public void CreatingTheSameQueryMultipleTimes()
        {
            var entityId = 0;
            var scope = Scope.CreateScope(() => entityId++, new FlagArrayManager(8));
            var queryA = scope.CreateQuery(new[] { 1, 5 }, new[] { 6 }, new[] { 7 });
            var queryB = scope.CreateQuery(new[] { 1, 5 }, new[] { 6 }, new[] { 7 });
            var queryC = scope.CreateQuery(new[] { 0, 5 }, new[] { 6 }, new[] { 7 });
            var queryE = scope.CreateQuery(new[] { 2, 5 }, new[] { 6 }, new[] { 7 });
            var queryD = scope.CreateQuery(new[] { 0, 5 }, new[] { 6 }, new[] { 7 });
            var queryF = scope.CreateQuery(new[] { 2, 5 }, new[] { 6 }, new[] { 7 });
            Assert.AreEqual(queryA, queryB);
            Assert.AreEqual(queryC, queryD);
            Assert.AreEqual(queryE, queryF);
            Assert.AreNotEqual(queryA, queryC);
            Assert.AreNotEqual(queryA, queryE);
            Assert.AreNotEqual(queryC, queryE);
        }
    }
}