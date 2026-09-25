# Nested Experiences

The GUI execution model must support recursive experience composition.

A runnable experience is not necessarily a terminal experience.

A city simulation may contain an arcade. The arcade may contain playable machines. A machine may contain a game. A game may contain a timeline, level, or simulation state of its own.

The important property is that each of these can remain an experience without requiring the outer experience to understand its implementation.

## Recursive execution

Conceptually:

GUI Execution Host
  -> City Experience
       -> Arcade Experience
            -> Arcade Machine Experience
                 -> Game Experience
                      -> Timeline / Level Experience

The same execution contract applies at every level.

The outer experience provides a context in which a child experience can be entered, executed, observed, interacted with, suspended, resumed, or terminated.

This means nesting is not a special GUI feature. It is a consequence of treating an experience as a runnable semantic unit.

## Experience versus Hub

The distinction should remain explicit:

- Experience — something that can execute.
- Hub — an execution boundary that can host and coordinate experiences.

A Hub may host an Experience, and an Experience may itself expose another Hub boundary.

Therefore the conceptual graph is recursive: Hub -> Experience -> Hub -> Experience.

AsHub() should therefore mean more than making a node named Hub. It should establish an executable composition boundary.

The GUI semantic tree remains the manifestation model. The experience graph is a separate recursive execution structure. They may correspond, but they are not the same structure.

## Context inheritance

Nested experiences need context without becoming coupled to the parent's implementation.

A child should receive a derived execution context containing inherited capabilities, services, identity/session, and relevant world context, plus child-local state, capabilities, and lifecycle.

A child may add capabilities required by its own experience.

A child should not silently mutate arbitrary parent state. Changes that cross the boundary should be explicit through the execution contract.

This is especially important for nested simulations. An arcade machine can have its own game state while still existing inside the arcade, which exists inside the city.

## Re-entry and suspension

Entering a child experience should not imply destroying the parent.

For example, when a player enters a game inside an arcade, the city can remain running or observing, the arcade can be suspended or observing, and the game can be running.

When the player exits, the game can terminate while the arcade and city resume according to their lifecycle policy.

The exact lifecycle semantics belong to the execution contract, but the model must permit this nesting.

## Recursive manifestation

A nested experience can have a manifestation of its own.

City viewport
  -> Arcade building
      -> Arcade interior
          -> Machines
              -> Game surface

The child manifestation does not have to replace the parent's entire manifestation. It may occupy a region, viewport, scene, panel, window, or other semantic surface supplied by its parent.

This allows the same recursive execution model to manifest differently on WPF, Blazor, Unity, or another adapter without changing the experience graph.

## Intellectual property as an experience boundary

This architecture also creates a natural place for licensed IP to enter the Workshop.

A rights holder could provide an experience bundle representing a product, character, game, world, historical artifact, machine, or other IP.

That bundle could be nested inside a larger experience without requiring the larger experience to absorb the IP implementation.

For example:

Time Travel Experience
  -> 1980s Timeline
       -> Arcade Experience
            -> Licensed Game Experience
  -> 1990s Timeline
       -> Console Experience
            -> Licensed Game Experience

The commercial boundary should remain explicit. A product being old or discontinued does not by itself establish that its copyright, trademark, licensing, or other rights have expired.

The infrastructure should therefore model rights and capabilities as a first-class bundle concern rather than assuming public-domain status.

That gives rights holders a path to contribute licensed experiences while preserving the recursive execution model.

## Progression across nested experiences

A parent experience may use child experiences as progression gates without owning their internal rules.

For example, a console-history experience could present successive timelines and require completion of a child game before exposing the next timeline.

The parent should observe semantic outcomes such as Completed, Failed, Abandoned, or Unlocked without knowing how the child achieved that result.

This permits the same game or IP experience to participate in many larger experiences.

## Architectural consequence

The GUI project should not evolve toward a collection of special-case containers such as AsHub(), AsExperience(), AsArcade(), AsGame(), AsCity(), and AsTimeline(). Those are domain concepts, not GUI primitives.

The reusable semantic contract should instead support the recursive relationship:

AsHub()
  -> Execute(Experience)
       -> Execute(Experience)
            -> Execute(Experience)

with the experience graph remaining recursively composable.

Domain-specific builders can then describe City, Arcade, Game, and Timeline experiences without requiring GUI Core to know what those domains mean.

## Design rule

> If an experience can run, it can be nested. If it can host another runnable experience, it can establish a child execution boundary.

The implementation should remain small until the semantic execution contract is proven by tests.