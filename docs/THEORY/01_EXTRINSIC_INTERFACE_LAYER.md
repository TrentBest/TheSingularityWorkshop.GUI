# The Extrinsic Interface Layer

## The problem

Software systems often allow the interface to become entangled with the identity of the system it represents.

A domain object begins to know about controls.

A state machine begins to know about rendering.

An application begins to assume a particular windowing system.

Input events arrive already shaped like the framework that produced them.

Over time, replacing the interface becomes a domain migration rather than an interface migration.

The alternative proposed here is to treat the GUI as an **extrinsic interface layer**.

## Definition

An extrinsic interface layer is an interface system that exists outside the intrinsic identity of the experience it represents.

It observes the experience.

It provides an interaction boundary.

It translates external interaction into semantic operations.

It manifests the resulting state through an appropriate representation.

The experience therefore does not need to become a WPF experience, a Blazor experience, a Unity experience, or a VR experience merely because one of those systems is currently observing it.

## The boundary

The relationship can be expressed as:

```
                 external world
                       |
                       v
               +---------------+
               | GUI boundary  |
               | observe       |
               | translate     |
               | manifest      |
               +---------------+
                       |
                       v
               +---------------+
               |   experience  |
               | state/behavior|
               +---------------+
```

The GUI is therefore not the experience's definition.

It is one possible way to observe and interact with it.

## Why "extrinsic" matters

The word is architectural, not cosmetic.

If an interface is intrinsic, changing the interface can require changing the thing being represented.

If an interface is extrinsic, the interface can be:

- replaced
- recreated
- suspended
- resumed
- reconfigured
- observed by multiple manifestations
- removed entirely

without requiring the represented experience to acquire a new identity.

This is the same reason an external diagnostic observer can inspect a system without becoming part of the system's domain model.

## Interaction is still part of the boundary

Extrinsic does not mean passive.

The interface is where external interaction enters.

A mouse click, touch gesture, controller action, voice command, accessibility action, or spatial gesture is a physical or platform-specific event.

The experience should receive the semantic meaning that matters.

For example:

```
mouse click
touch
controller activation
voice command
spatial gesture
        |
        v
   GUI boundary
        |
        v
     Activate
```

The physical mechanism is external.

The semantic operation is what crosses the boundary.

## Consequences

If this principle is accepted, several design consequences follow.

### Experience identity must not depend on GUI implementation

A manifest should not need to contain WPF controls, HTML, CSS, Unity UI Toolkit objects, or similar implementation artifacts merely to describe an experience.

### Platform-specific input belongs at the boundary

A domain contract should not need to know whether an action originated from a mouse, touchscreen, controller, keyboard, voice system, or spatial input device unless that physical distinction is itself part of the domain.

### Manifestation is replaceable

A representation can be destroyed and recreated without redefining the experience.

### Multiple observers become possible

Different manifestations can represent the same underlying experience for different purposes.

A diagnostic observer and an immersive observer do not need to share the same visual form.

## The skeptical objection

> "Isn't this just another abstraction layer?"

It can be.

An abstraction is only justified when it creates a stable semantic boundary around volatility that would otherwise leak into the rest of the system.

That is why the theory must be tested against actual platform changes, not defended as a matter of taste.

The repository's Core/adapter split is one such test.

## What this principle does not claim

It does not claim that all GUIs should look alike.

It does not claim that platform-specific capabilities should disappear.

It does not claim that one semantic model can express every possible future interface without extension.

It claims something narrower:

> The meaning of an experience should not be forced to become identical to the technology used to display or manipulate it.

That is the boundary this project is attempting to make explicit.
