# The Breakwater Principle

## The problem

Every software boundary is exposed to environmental change.

Operating systems change.

Rendering technologies change.

Input devices change.

Browsers change.

Interaction models change.

New platforms appear that did not exist when the original domain model was written.

A weak abstraction allows those changes to propagate inward.

A strong boundary absorbs them.

## Definition

The **breakwater principle** states:

> A useful boundary should absorb environmental volatility so that the semantic core does not have to understand every form that volatility can take.

The analogy is physical.

A breakwater does not stop the sea from existing.

It changes where the force of the sea is absorbed.

## Applied to GUI architecture

Consider a semantic operation:

```
Activate
```

The external mechanism could be:

- a mouse click
- a touchscreen gesture
- a controller button
- a keyboard command
- a voice interaction
- an accessibility action
- a spatial interaction

The semantic core should not need a separate domain concept for every physical mechanism when the distinction is irrelevant to the experience.

The boundary translates:

```
physical/platform event
        |
        v
   boundary adapter
        |
        v
 semantic operation
```

The volatility remains at the edge.

## Why this is stronger than "cross-platform"

"Cross-platform" often means that the same application can be compiled or rendered for multiple platforms.

The breakwater principle asks a different question:

> Where does knowledge of platform difference live?

An application can technically support five platforms while still having platform details scattered throughout its domain code.

That is compatibility without containment.

The stronger architectural goal is containment.

## The skeptical objection

> "Adapters just move complexity around."

Correct.

They do move complexity.

The question is whether the complexity has been moved to the place where it can be reasoned about independently.

A WPF adapter can know about WPF.

A Blazor adapter can know about Blazor.

A future Unity adapter can know about Unity.

The semantic Core does not need to know all three simply to define what an actionable interface element means.

## The test

A boundary is earning its existence when adding or replacing an external mechanism primarily changes the boundary implementation rather than the semantic contract.

That is a testable property.

It can be examined in source code.

It can be exercised by adapter conformance tests.

It can be challenged whenever a supposedly neutral concept begins accumulating platform-specific conditions.

## Consequence

The goal is therefore not to minimize code.

The goal is to minimize **uncontained environmental knowledge**.

That distinction matters.

Sometimes a larger adapter is preferable to a smaller core that has become contaminated by platform assumptions.

## Boundary of the principle

The breakwater principle does not imply that every difference should be hidden.

Some differences are semantically meaningful.

If an experience genuinely needs a capability that exists only in one environment, the architecture must be able to represent that fact rather than pretending every platform is equivalent.

The principle therefore favors explicit capability boundaries over false uniformity.
