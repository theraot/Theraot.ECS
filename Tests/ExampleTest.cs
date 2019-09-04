using System;
using System.Collections.Generic;
using NUnit.Framework;
using Theraot.ECS;

namespace Tests
{
    public static class ExampleTest
    {
        [Test]
        public static void Test()
        {
            // Creates a new scope, the entity id type will be Guid, the component kind will be int, the component kind set is FlagArray
            // The default containers of component values will be used
            // Only 2 component kinds are allowed
            var scope = Scope.CreateScope(EqualityComparer<Guid>.Default, new FlagArrayManager(2));

            // These will be our component kinds
            const int first = 0;
            const int second = 1;

            // Create an entity id
            var entityId = Guid.NewGuid();
            // Register the entity in the scope
            scope.RegisterEntity(entityId);
            // Store an string "World" in the component kind 0 of the entity
            scope.SetComponent(entityId, first, "World");

            // Create an EntityCollection for all the entities that have component kind 1
            var entities = scope.GetEntityCollection(all: new[] { second }, any: Array.Empty<int>(), none: Array.Empty<int>());
            // Register a notification when new components satisfy the query
            entities.AddedEntity += (sender, args) =>
            {
                // Execute a callback on the entity
                scope.With
                (
                    args.EntityId,
                    second,
                    (Guid _, ref string componentValue) => Console.WriteLine(componentValue)
                );
            };

            // Add a string "Hello" in the component kind 1 of the entity
            scope.SetComponent(entityId, second, "Hello");

            // Recover the component we set earlier
            Console.WriteLine(scope.GetComponent<string>(entityId, first));
        }
    }
}