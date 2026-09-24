# The 2D GUI Domain and Its 3D Extension

## Purpose

This document defines the spatial domain that the ideal GUI should be able to express.

The starting point is deliberately simple: **2D composition**. From there, the model extends into **3D spatial representation** without changing the fundamental distinction between semantic content, observation, and manifestation.

This is an ideal-model document. It describes the vocabulary the architecture should be capable of representing; it does not imply that every concept below is already implemented.

## 1. The 2D domain

A 2D GUI is a spatial composition of semantic elements on a presentation surface.

At minimum, the domain needs to describe:

- **surface** — the space in which a GUI manifestation is composed;
- **region** — a bounded area of that surface;
- **element** — something semantically represented within a region;
- **container** — an element that contains other elements;
- **viewport** — a region through which another semantic space is observed;
- **transform** — a mapping between coordinate spaces;
- **layer** — an ordering relationship affecting visibility and occlusion;
- **camera** — a controlled description of what is being observed;
- **interaction target** — a region or element that can receive interaction;
- **focus / selection** — semantic state describing what the observer is currently attending to.

The model should be able to express ordinary application layouts as easily as maps, diagrams, dashboards, schematics, editors, and specialized visualization surfaces.

## 2. Coordinate spaces

The GUI should not assume that one coordinate system is sufficient.

A useful conceptual chain is:

```
semantic space
      |
      v
local element space
      |
      v
container space
      |
      v
viewport space
      |
      v
surface space
      |
      v
platform manifestation space
```

A transformation changes where something appears without changing what it means.

This distinction becomes essential when a single GUI contains many independent views.

## 3. Many views of one reality

Consider a GUI containing twelve panels. Each panel displays a hero view of a selected unit.

```
                    ONE GUI
+------------------------------------------------+
| +----------+ +----------+ +----------+ +-----+ |
| | Unit A   | | Unit B   | | Unit C   | | ... | |
| | Camera A | | Camera B | | Camera C | |     | |
| +----------+ +----------+ +----------+ +-----+ |
| +----------+ +----------+ +----------+ +-----+ |
| | Unit E   | | Unit F   | | Unit G   | | ... | |
| | Camera E | | Camera F | | Camera G | |     | |
| +----------+ +----------+ +----------+ +-----+ |
+------------------------------------------------+
```

Each panel is a presentation surface for a view. Each camera defines the observation of its target. The GUI remains one composition.

That allows the model to express relationships such as:

```
Unit
  |
  +--> selected representation
  |
  +--> camera / observation state
  |
  +--> viewport
  |
  +--> panel
```

A camera may change while the unit remains the same. A panel may move while the camera remains the same. A representation may change while the underlying unit remains unchanged.

Those are semantic relationships, not rendering-library details.

## 4. The 3D extension

The 3D domain adds depth and spatial relationships to the same representational model.

A 3D subject may have:

- position;
- orientation;
- scale;
- geometry or volume;
- hierarchy;
- spatial relationships to other subjects;
- a visual representation;
- a camera-relative appearance.

The 3D observation pipeline can be described as:

```
3D semantic domain
        |
        v
   spatial scene
        |
        v
     camera
        |
        v
    projection
        |
        v
2D viewport / panel
        |
        v
 GUI manifestation
```

The projection step is important: a 3D GUI does not abandon the 2D domain. It uses the 2D domain as the presentation plane for a spatial subject.

## 5. Camera as a first-class concept

A camera should be understood as an **observer definition**.

Its semantic responsibility is to define what portion of a domain is being observed and how that observation is mapped into a viewport.

For 2D, this can mean pan, zoom, crop, and focus.

For 3D, it can additionally mean position, orientation, projection, field of view, clipping, and other observation parameters.

The architecture should avoid making the camera synonymous with a particular graphics-engine camera type.

## 6. Why this matters to the ideal GUI

Once these concepts exist, the GUI stops being constrained to the vocabulary of widgets.

The same semantic model can describe:

- a form;
- a dashboard;
- a floor plan;
- a map;
- a CAD view;
- a machine monitor;
- a multi-camera control surface;
- a 3D model viewer;
- a game or simulation view;
- a VR-oriented presentation;
- a diagnostic visualization.

The platform adapter remains responsible for turning that semantic description into a concrete manifestation.

## 7. What remains to be defined

This document intentionally establishes a domain vocabulary rather than pretending that all contracts are settled.

Future work must define, with tests:

- precise coordinate-space semantics;
- transformation composition;
- camera and viewport contracts;
- clipping and occlusion semantics;
- hit-testing and interaction routing;
- 2D-to-3D projection contracts;
- selection and focus semantics across multiple views;
- rendering hints that remain platform-neutral;
- lifecycle behavior for views and cameras.

The objective is not to build a miniature graphics engine inside Core.

The objective is to define enough semantic structure that different renderers can faithfully manifest the same intended GUI.
