# Spatial Representation and Controlled Observation

## The problem

Graphical interfaces are often described in terms of widgets: buttons, panels, lists, fields, and windows.

That vocabulary is useful, but it becomes incomplete as soon as the interface must represent a map, floor plan, CAD model, simulation, game scene, machine, or several simultaneous views of a changing digital reality.

The deeper problem is spatial:

> How can one semantic interface describe what is being observed, where it is presented, and how that observation is controlled without becoming dependent on a rendering platform?

## Observation is separate from presentation

A useful distinction is:

```
subject
  |
  v
observation
  |
  v
viewport
  |
  v
presentation surface
```

The subject is what the interface represents.

The observation describes what portion or aspect of the subject is currently being viewed.

The viewport is the region through which that observation is presented.

The presentation surface is where the resulting representation is composed with the rest of the GUI.

This separation permits one subject to have many simultaneous views.

## Many views of one reality

Consider twelve panels, each showing a hero view of a different selected unit.

Each panel can have its own camera and viewport while remaining part of one GUI.

```
Unit A -> Camera A -> Viewport A -> Panel A
Unit B -> Camera B -> Viewport B -> Panel B
Unit C -> Camera C -> Viewport C -> Panel C
...
Unit L -> Camera L -> Viewport L -> Panel L
```

The panels do not constitute twelve independent applications.

They are twelve observations composed into one interface.

This distinction becomes valuable when selection, synchronization, interaction, resizing, and lifecycle are considered. A camera can change without changing the identity of its subject. A panel can move without changing the observation. A representation can be replaced without replacing the underlying subject.

## 2D as the foundational presentation domain

The 2D domain provides the surface on which GUI composition occurs.

It supplies concepts such as:

- regions;
- containers;
- layers;
- viewports;
- transformations;
- selection;
- focus;
- interaction targets.

2D should therefore be treated as a semantic spatial domain, not merely as a collection of pixel coordinates.

## 3D as an extension

Three-dimensional representation adds depth, position, orientation, scale, geometry, and spatial hierarchy.

It does not require a new theory of GUI.

Instead:

```
3D semantic space
      |
      v
   camera
      |
      v
  projection
      |
      v
2D viewport
      |
      v
GUI presentation
```

The 3D domain becomes another subject of controlled observation whose result is presented through the 2D composition model.

This gives the architecture a continuous path from ordinary panels to maps, CAD, simulation, games, 3D model viewers, and immersive interfaces.

## Camera as a semantic observer

The word *camera* should not imply a particular graphics API.

A camera is a semantic description of controlled observation.

In 2D, that may mean pan, zoom, crop, and focus.

In 3D, it may additionally include position, orientation, projection, field of view, and clipping.

The common concept is:

> A camera determines how a semantic domain is observed through a viewport.

## Consequences

If observation is represented independently from presentation:

1. one subject can be observed in multiple ways;
2. multiple subjects can be composed into one GUI;
3. a presentation surface can be replaced without redefining the subject;
4. a renderer can change without changing the semantic observation;
5. 2D and 3D can share a common interface model;
6. platform-specific camera and rendering mechanisms remain boundary concerns.

## Counterargument

A developer could reasonably implement all of this directly in a graphics framework.

That may be appropriate when the application is intentionally coupled to that framework.

The architectural question is different:

> Is the application better served when the meaning of the observation is independent from the mechanism that renders it?

If the answer is yes, the semantic boundary has earned a place.

## Boundary

This theory does not claim that a semantic GUI should reproduce every capability of every graphics engine.

The boundary is deliberate.

Core should describe meaning.

Platform adapters should describe manifestation.

Graphics engines should remain free to provide implementation-specific capabilities where the semantic model cannot or should not standardize them.

## Further work

The theory implies concrete contracts that must eventually be tested:

- coordinate-space transformations;
- camera and viewport semantics;
- projection;
- clipping and occlusion;
- hit testing;
- selection across multiple views;
- synchronization between views;
- view and camera lifecycle.

The theory therefore produces engineering questions rather than replacing engineering with abstraction.
