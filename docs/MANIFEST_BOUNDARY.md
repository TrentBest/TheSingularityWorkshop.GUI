# Manifest Boundary

## Principle

An experience manifest identifies **what the experience uses**.

It does not encode the implementation of its GUI.

The GUI is an extrinsic consumer of the resulting experience.

## Conceptual manifest

A manifest may contain concepts such as:

```text
Experience
  MicroBundle IDs
  Configuration
  Dependency information
  Capability requirements
  Version information
```

It should not need to contain:

```text
Button
  Width = 120
  Background = ...
  OnClick = ...
```

That information belongs to the GUI layer or to configuration consumed by the GUI layer.

## Why this matters

If GUI implementation enters the experience manifest, the manifest becomes coupled to:

- a presentation technology
- a device
- a rendering environment
- an input model
- potentially an operating system

That defeats the purpose of a portable experience definition.

Instead:

```
Manifest
   |
   v
Experience composition
   |
   +----> behavior
   |
   +----> data/state
   |
   +----> GUI observation
             |
             v
        GUI configuration
             |
             v
       GUI manifestation
```

## GUI configuration

The GUI may consume configuration that describes presentation policy.

This allows the same underlying experience to be observed differently without changing the experience's identity.

For example, one configuration could request:

- compact diagnostic presentation

while another could request:

- immersive spatial presentation

The experience itself remains the same.

## Micro-bundles

A GUI capability can itself be delivered as a micro-bundle.

The manifest can identify the bundle.

The GUI system can then use the bundle's providers and configuration to construct the appropriate presentation.

This permits GUI capabilities to become composable rather than hard-coded into an experience.

## Separation of concerns

The intended separation is:

| Concern | Owns |
|---|---|
| Experience manifest | composition and configuration |
| Micro-bundles | reusable capabilities |
| Runtime/domain | digital reality |
| FSM_API | behavior/state transitions |
| GUI model | visual and interaction intent |
| Platform adapter | manifestation |
| Input device | physical/external input |

The boundaries are contracts, not walls preventing communication.

They exist to prevent one layer from becoming the implementation detail of another.
