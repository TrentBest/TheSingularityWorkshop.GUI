
# Spatial GUI Contracts

This document is the engineering companion to the ideal spatial model. It identifies the semantic questions that must eventually have precise, testable answers before spatial representation becomes a durable Core contract.

Status terms:
- Implemented — present in source and/or tests.
- Specified — vocabulary and intended behavior are documented.
- Proposed — design direction requires a precise contract.
- Unresolved — an architectural question must remain visible rather than being hidden in implementation.

## 1. Spatial vocabulary

The model should distinguish surface, region, element, container, subject, representation, view, viewport, camera, transform, layer, and interaction target.

A panel is a presentation surface. A view is an observation. A viewport is the region through which the observation is presented. A camera is the semantic state describing controlled observation.

Status: Specified.

## 2. Identity must survive movement

Spatial position must never become semantic identity.

A subject can move. A panel can move. A camera can move. A representation can be replaced.

Required tests:
- moving an element does not change its identity;
- replacing a panel does not change its subject;
- replacing a representation does not change the represented subject;
- multiple views can reference one subject without merging their identities.

Status: Proposed.

## 3. Coordinate spaces and transforms

The semantic model should distinguish, where needed:

    semantic/world space
        -> subject/local space
        -> view space
        -> viewport space
        -> surface space
        -> platform manifestation space

A transform maps one space to another without changing semantic identity.

The eventual contract must define translation, rotation, scale, composition, inversion, precision, invalid transforms, and deterministic behavior.

Status: Coordinate primitives implemented; full semantic transform contract proposed.

## 4. The 2D domain

2D is a complete spatial domain, not merely a reduced form of 3D.

It must be capable of describing documents, dashboards, maps, floor plans, schematics, diagrams, image editors, timelines, node graphs, and dense diagnostic surfaces.

A viewport may observe a finite or effectively unbounded semantic space while appearing inside a finite presentation surface.

Status: Specified.

## 5. Views and viewports

A view is not a panel.

One subject may have several observations:

    subject
       -> view A -> viewport A -> panel A
       -> view B -> viewport B -> panel B
       -> view C -> viewport C -> panel C

A panel may also contain multiple viewports.

Status: Specified.

## 6. Camera semantics

A camera is a semantic observer, not a graphics-engine type.

For 2D it may include center, zoom, crop, rotation, and focus.

For 3D it may additionally include position, orientation, projection, field of view, and clipping parameters.

Status: Vocabulary specified; contract proposed.

## 7. Projection

Projection maps an observed spatial domain into a presentation space.

The model should eventually distinguish orthographic, perspective, and 2D affine observation, along with projection parameters, clipping behavior, and viewport mapping.

Tests should compare semantic projection intent rather than native renderer classes.

Status: Proposed.

## 8. Visibility, depth, and occlusion

The model must eventually define semantic visibility relationships:

- 2D layer order;
- 3D depth order;
- clipping;
- occlusion;
- hidden versus collapsed versus present-but-not-visible;
- transparency intent where meaningful.

The goal is not to reproduce a GPU depth buffer. It is to define what an adapter must preserve.

Status: Proposed.

## 9. Hit testing and interaction routing

Spatial interaction should follow a semantic path:

    physical input
        -> platform coordinates
        -> semantic coordinate space
        -> hit test
        -> interaction target
        -> semantic action

The contract must address overlapping targets, ordering, non-interactive regions, pointer capture, drag continuity, keyboard focus, gaze, controller input, and voice equivalents.

Status: Proposed.

## 10. Selection and multi-view synchronization

Selection is semantic state, not merely a highlight.

The model should distinguish subject selection, view focus, input focus, hover/pointing state, active interaction target, and camera focus.

Multiple views may observe the same subject, different subjects, or intentionally remain independent. Synchronization must be explicit.

Examples include synchronized time, selection propagation, linked cameras, and one view controlling another.

Status: Proposed.

## 11. Rendering intent

The semantic model may need to express meaning such as emphasis, selection, focus, annotation, schematic mode, preferred projection, visual priority, and level-of-detail intent.

It should not become a bag of renderer-specific properties.

Principle: meaning belongs in Core; mechanisms belong in adapters.

Status: Proposed.

## 12. Time and animation

Spatial interfaces are temporal.

The subject may move, the camera may move, data may change, or a presentation transition may run.

The model should distinguish semantic state change from platform-specific interpolation so that animation does not become part of domain identity.

Status: Proposed.

## 13. Accessibility and alternate presentation

Spatial content must remain meaningful when pixels are not the only presentation.

Maps may need landmarks and ordered regions. 3D scenes may need object descriptions and nonvisual summaries.

The same spatial semantics should be usable by visual rendering, accessibility, automation, testing, and other observers.

Status: Proposed.

## 14. Localization and spatial layout

Localization changes geometry.

Text expansion, writing systems, directionality, fonts, and locale-sensitive presentation can change layout.

Semantic position must therefore remain distinct from localized content and platform typography. Coordinates must never secretly encode the expected width of an English label.

Status: Specified.

## 15. Performance and level of detail

A semantic spatial model can describe a large reality without requiring every adapter to materialize everything at full fidelity.

Future platform-neutral hints may include visibility-based loading, level of detail, culling, progressive representation, virtualization, and update frequency.

These are adapter concerns, but the semantic contract may need a stable vocabulary for requesting them.

Status: Proposed.

## 16. Persistence and deterministic reconstruction

If GUI is a semantic artifact, spatial state should eventually be serializable.

Questions include coordinate-space versioning, stable IDs, transform precision, camera persistence, viewport persistence, selection persistence, compatibility, and deterministic reconstruction.

Status: Proposed.

## 17. 2D-to-3D continuity

The intended path is:

    2D semantic space
        -> 3D semantic space
        -> controlled observation
        -> projection
        -> 2D viewport
        -> presentation surface

A richer subject does not require a second GUI theory. Depth enriches the subject; observation and presentation remain recognizable.

Status: Specified.

## 18. Platform capability boundaries

A platform may not support every semantic capability.

Adapters should manifest, negotiate, degrade predictably, or reject with a diagnostic. Silently changing meaning is the failure mode to avoid.

Status: Proposed.

## 19. Spatial conformance test

A future adapter-conformance suite should exercise the same semantic scenarios:

1. place subjects in 2D;
2. transform a subject into nested spaces;
3. create multiple views of one subject;
4. change a camera without changing subject identity;
5. select a subject in one view;
6. route interaction through a viewport;
7. extend the subject into 3D;
8. project it into a 2D viewport;
9. exercise unsupported capabilities;
10. serialize and reconstruct the semantic artifact.

The test compares semantic outcomes, not native control classes.

Status: Proposed.

## 20. Boundary

This work does not put a full graphics engine, GPU, physics engine, or renderer inside Core.

It defines the semantic questions that need a stable home before those implementation concerns cross the boundary.

The desired end state is:

    digital reality
        -> semantic observation
        -> semantic GUI representation
        -> platform adapter
        -> native manifestation

The adapter may be sophisticated. Core should remain semantically precise.

## Relationship to the documentation

Ideal GUI defines the destination.
GUI Spatial Domain introduces the vocabulary.
Spatial Representation explains the theory.
Architecture defines the engineering boundary.
Platform Adapters define manifestation responsibilities.
Roadmap tracks implementation work.

This document is the bridge between the ideal model and future executable contracts.
