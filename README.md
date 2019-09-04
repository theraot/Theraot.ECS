# Theraot.ECS

[![Build status](https://ci.appveyor.com/api/projects/status/yny5hsrbjc7asfud?svg=true)](https://ci.appveyor.com/project/theraot/theraot-ecs)

A flexible pure C# Entity-Component-System solution.

You can specify the types used for:

- Entity Ids (e.g. `int`, `GUID`, etc...).
- Component Values (whatever you need).
- Containers of Component Values (intended to allow special containers, such as quadtrees).
- Component Kinds (e.g. `int`, `string`, etc...).
- Sets of Component Kinds (e.g. to use bit arrays for quick comparison of sets).

---
Design decisions
---

- The ECS allows to specify the type used to represent component ids. As a useful side effect, it does not generate entity ids, which is useful if you want to synchronize entities across the network.

- The ECS uses component kinds. There can be multiple component kinds with the same type. *For example, a speed component can be `Vector2` and a position component can also be `Vector2`.* A type is registered for each component type, either explicitly with `Scope.TryRegisterType` (which fails if it is already registered) or implicitly with the first call of `Scope.SetComponent` that uses the component type. Type safety is kept for the registered type of the component kind. This also allows optimizations when handling sets of component kinds.

- The ECS allows to specify a class that handle component kinds and sets of component kinds. This is done by passing a type that implements `IcomponentKindManager` when creating a `Scope`. Two implementations of that interface are provided:

   1. `FlagArrayManager` which represents component types as integers, and stores them as binary flags. Initially I wanted to use `BitArray`, however it was sub optimal for comparing sets. Instead I wrote a more versatile `FlagArray` class. This is intended to be fast by taking advantage of bit-wise operations. Comparison complexity is `O(n)` where `n` is the number of all existing component kinds in the `Scope`.
   2. `SetManager` which represents component types as strings, and stores them in hash based sets (`HashSet<string>` except in .NET 2.0 and .NET 3.0 where it uses a custom internal set type). This is intended to have flat performance. Complexity on comparison is `O(n)` where `n` is the number of component kinds of the entity.

- The ECS keeps tracks of the created queries, this means that after the first time, getting the entities that match a query (`Scope.GetEntityCollection`) is an `O(1)` operation. The first time it is `O(n)` when `n` is the number of matched entities. Therefore it is recommended to make the needed calls to `Scope.GetEntityCollection` before registering entities.

- The ECS, by default, keeps components nearby in memory by their type, instead of by their entity. Because it allows to specify custom containers for particular types via `Scope.TryRegisterType` (which will fail is components of that type has already been added), which is useful for example, to handle components in a space aware data structure (e.g. a quadtree). This is done by passing an implementation of `IIntKeyCollection` to `Scope.TryRegisterType`, the only implementation provided is `IntKeyCollection` which is used by default.

- The ECS provides temporary ref access to components, this allows fast read and write of them. To get them use `Scope.With` or `EntityCollection.ForEach` which take callbacks with ref parameters. While the flow of execution is inside the callbacks, all addition or removals of components are buffered, because they could move the referenced components. *There are no guarantees about taking pointers from these references.*

- The ECS is not thread-safe. Since the components are arranged by their type and not by entity, it is not feasible to lock an entity. This only leaves the options of locking by component or locking the whole. Locking by component would have meant a noticeable performance penalty, and locking the whole is better handled outside of the library.

- The ECS provides events that notify when a component was added or removed. This was actually necessary to be able to buffer changes… since the changes can happen at a later moment from the call, a notification was necessary to update the component kind sets internally… it was minimal extra effort to make these events public.