# The Ideal GUI

## The Land of Idealism

This repository is not intended to begin with the question:

> How do we make a button in WPF, Blazor, Unity, or some other toolkit?

It begins with a more fundamental question:

> What is a GUI supposed to be?

In the Land of Idealism, the GUI is an **extrinsic interface layer**. It is not an intrinsic member of the domain code it represents.

The domain exists.

Its micro-bundles exist.

Its configuration exists.

Its runtime behavior exists.

The GUI is the ephemeral layer that observes, represents, and interacts with that digital reality.

The GUI therefore belongs beside the experience, not inside the experience's definition.

## The central boundary

An experience manifest should not contain GUI implementation.

Instead, an experience manifest identifies the micro-bundles required by the experience and the configuration to apply to those bundles.

Conceptually:

```
Experience Manifest
    |
    +-- Micro-bundle IDs
    |
    +-- Configuration
    |
    +-- Dependency / capability information
    |
    v
Runtime Experience
    |
    +--------------------+
    |                    |
    v                    v
FSM / behavior       GUI / visualization
    |                    |
    |                    |
    +---------+----------+
              |
              v
       ephemeral interface
              |
              v
   human / device / reality
```

The GUI does not become part of the manifest merely because the experience can have a GUI.

The manifest describes **what the experience is composed of**.

The GUI describes **how that experience is observed and interacted with at a particular moment**.

## GUI as an extrinsic voyeur

"Voyeur" is intentional terminology for the design metaphor.

The GUI does not need to own the thing it is observing.

It looks at a digital reality from the outside and constructs a representation of that reality.

That representation can be:

- diagnostic
- operational
- informational
- navigational
- spatial
- artistic
- schematic
- textual
- symbolic
- interactive
- immersive
- deliberately abstract
- deliberately literal
- or a combination of these

The GUI may also allow the observer to act upon the observed system.

That action crosses the interface boundary and becomes input to the digital system.

The GUI therefore sits at a boundary:

```
REALITY
  |
  | keyboard / mouse / joystick / controller / touch / gaze / gesture / voice
  v
GUI INPUT
  |
  v
GUI / INTERACTION LAYER
  |
  +---- observes ----> digital state
  |
  +---- presents ----> digital state
  |
  +---- requests ----> digital behavior
  |
  v
DIGITAL REALITY
```

The GUI is therefore neither the reality nor the domain.

It is the interface between them.

## FSM_API relationship

The intended relationship with FSM_API is deliberately loose.

FSM_API supplies a model for **behavior and state transition**.

The GUI supplies a model for **visualization and interaction representation**.

Conceptually:

```
FSM_API = behavior
GUI     = visualization + interaction boundary
```

Neither should require the other as an intrinsic implementation dependency.

A GUI adapter may use FSM_API state to determine what should currently be visible.

An experience may use FSM_API transitions to determine when the GUI changes.

But the GUI model itself must remain useful without embedding FSM_API types into every GUI node.

This keeps the two systems composable.

## The 2D domain

Ideal GUI land begins with a two-dimensional space.

Not because a GUI must be flat, but because 2D is the smallest useful spatial model in which we can describe composition, containment, adjacency, alignment, overlap, visibility, focus, and interaction without tying those ideas to a particular renderer.

A 2D GUI therefore needs more than a collection of rectangles. It needs a vocabulary for a **viewable surface**:

- regions and bounds
- panels and sub-panels
- anchors and alignment
- layering and occlusion
- clipping and visibility
- selection and focus
- text and symbolic content
- interaction targets
- spatial relationships
- viewports
- cameras
- transformations between coordinate spaces

The important distinction is between **the thing being represented** and **the place from which it is being viewed**.

A panel can be a region of the GUI. A viewport can be a window onto a larger semantic space. A camera can describe how that space is observed. None of those concepts requires WPF, CSS, a browser, or a particular graphics API.

### Multiple panels as multiple views

Imagine a dozen panels, each displaying the **hero view of a targeted unit**:

```
Digital Reality
      |
      +---- Unit A ----> Camera A ----> Panel A
      +---- Unit B ----> Camera B ----> Panel B
      +---- Unit C ----> Camera C ----> Panel C
      |          ...
      +---- Unit L ----> Camera L ----> Panel L
```

This is not twelve unrelated GUIs. It is one GUI containing twelve **views** over a digital reality.

A camera can change without changing the identity of the represented unit. A panel can be replaced without changing the experience it observes. A user can select a unit in one part of the interface while its representation changes elsewhere.

The semantic relationship is:

> **subject → view → viewport → presentation surface**

The GUI model should be able to describe that relationship without prescribing how the final pixels are produced.

## The 3D extension

Three-dimensional visualization should extend the 2D model rather than become a separate conceptual universe.

```
3D semantic space
        |
        v
     Camera
        |
        v
   Projection
        |
        v
2D viewport / panel
        |
        v
   GUI manifestation
```

A 3D scene adds depth, position, orientation, scale, volume, spatial hierarchy, and projection. But its observation still terminates in a presentation surface.

**2D is not the limit of the GUI model. It is the presentation plane onto which richer spatial domains can be observed.**

The word *camera* is therefore a semantic concept: controlled observation. For 2D it may mean pan, zoom, crop, and focus. For 3D it may additionally describe position, orientation, projection, field of view, and clipping.

The important abstraction is not the graphics-engine camera. It is **the relationship between a subject, an observation, and the surface on which that observation is presented**.

## What the ideal model must eventually define

This spatial domain leaves several contracts to be specified and tested:

- coordinate spaces and transformations
- viewport and camera semantics
- clipping and occlusion
- hit testing and interaction routing
- 2D-to-3D projection
- selection and focus across multiple views
- platform-neutral rendering hints
- lifecycle of views and cameras

The objective is not to build a graphics engine inside Core. It is to define enough semantic structure that different renderers can faithfully manifest the same intended GUI.

## The GUI must be alive

A GUI is not merely a static picture.

A living GUI can:

- observe state
- change representation
- respond to input
- expose available operations
- animate
- transition
- appear
- disappear
- compose and decompose
- present diagnostic information
- present operational controls
- enter and leave modes
- represent spatial relationships
- react to time
- react to external devices

FSM_API provides one possible behavioral substrate for this life.

The GUI remains the visual and interactive expression of that behavior.

## The ideal boundary

The ideal boundary is:

```
        DOMAIN / EXPERIENCE
                |
        micro-bundles + config
                |
                v
        +---------------+
        | runtime state |
        +---------------+
             /     \
            /       \
           v         v
       behavior   visualization
          |            |
       FSM_API      GUI model
                       |
                       v
                 platform adapter
                       |
          +------------+------------+
          |            |            |
        Web          WPF         Unity/VR
```

The same digital reality should not need to become a different domain merely because the observer changes.

## The design objective

The long-term objective of this repository is therefore:

> Build a GUI abstraction expressive enough to represent the full intended domain of digital visualization without making any particular platform, rendering technology, operating system, or input device intrinsic to the model.

That is a substantially stronger requirement than "support WPF and Blazor."

It is the reason this repository exists.
