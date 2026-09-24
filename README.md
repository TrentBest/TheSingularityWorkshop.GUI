# TheSingularityWorkshop.GUI

A platform-neutral GUI engineering layer with recursive builders and platform adapters.

The repository exists to separate GUI intent from GUI manifestation.

A consuming application should be able to describe an interface once and allow a platform adapter to materialize it for WPF, Blazor, Unity UI Toolkit, or another supported technology.

## Repository structure

- src/Core — the neutral recursive GUI model and platform-independent primitives.
- src/Blazor — Blazor builders and the first Core-to-web manifestation.
- src/WPF — reusable WPF builders and desktop manifestation infrastructure.
- tests/Core.Tests — platform-independent contract tests.
- docs — architecture, model, adapter, and testing contracts.
- ROADMAP.md — engineering work required to make the neutral model durable.

## The engineering boundary

The important abstraction is not a shared collection of buttons and panels.

It is a semantic GUI model:

application intent -> recursive GUI model -> platform adapter -> native GUI

Core therefore must not depend on HTML, DOM, CSS, JavaScript, Blazor RenderFragment, WPF DependencyObject, XAML, Unity UI Toolkit types, or platform event delegates.

Platform adapters own those details.

## Recursive builders

The model is intentionally recursive. A builder can contain children, and each child can itself contain children.

For example:

Experience -> Panel -> Panel -> Button -> Text

This permits small builders to compose larger interfaces without creating a single application-specific GUI builder.

## Neutrality is a contract

Neutral does not mean copying the lowest common denominator between platforms.

A Core concept belongs in the model only when it has a defined, platform-independent meaning. Platform-specific details are translated by adapters.

This distinction is the central engineering problem for this repository.

See:

- docs/ARCHITECTURE.md
- docs/GUI_MODEL.md
- docs/PLATFORM_ADAPTERS.md
- docs/TESTING.md
- ROADMAP.md

## Current integrations

The WebPage repository is the first migration target for the Blazor layer.

RevitFamilyManagerBuilders remains unchanged. Reusable WPF infrastructure is being migrated here for forward-moving applications rather than modifying that existing repository.

## Status

The repository is in active contract-design phase. The current recursive tree is intentionally small. The next work is to replace ambiguous stringly-typed behavior with explicit semantic contracts, capability negotiation, shared adapter conformance tests, and deterministic GUI artifacts.


## The Land of Idealism

The long-term design begins with a stronger premise than platform abstraction.

The GUI is an **extrinsic interface layer**: an ephemeral observer and interaction boundary around a digital experience. It is not an intrinsic member of the experience's domain code.

An experience manifest identifies micro-bundles and configuration. It does not contain GUI implementation. The GUI observes the resulting runtime and manifests an appropriate representation.

Conceptually:

```
experience manifest
    -> micro-bundles + configuration
    -> runtime experience
    -> GUI observation / interaction
    -> platform manifestation
```

The intended relationship with FSM_API is similarly separated:

- **FSM_API** provides behavior and state-transition machinery.
- **GUI** provides visualization and interaction representation.
- **Platform adapters** turn semantic GUI intent into platform-native manifestation.
- **Input devices** provide physical or external input to the interaction boundary.

## GUI is larger than widgets

GUI purpose is independent of GUI platform.

The model must eventually support diagnostic, operational, informational, spatial, immersive, and expressive visualization. A GUI may represent a button, a floor plan, a state inspector, a 3D scene, an image, a mesh, animation, or even deliberately unstructured visual output such as static/noise.

The goal is not to build a larger widget library. The goal is to build a semantic representation space expressive enough to describe digital visualization without making a particular device or rendering technology intrinsic to the domain.

This is also the foundation for VR: semantic interaction such as Activate, Select, Point, Navigate, or Rotate must not intrinsically mean mouse click, keyboard input, or any other particular physical device.

## Visual references

Concrete images are useful as **reference manifestations** of the semantic model.

The first reference is the Workshop map: a large spatial/operational interface combining navigation, a map/scene, destination indexing, textual information, and controls. It is intentionally treated as evidence of expressiveness rather than as a pixel-level Core contract.

When committed, the canonical asset path is:

`docs/assets/workshop-map-reference.png`

See [docs/GUI_VISUAL_REFERENCE.md](docs/GUI_VISUAL_REFERENCE.md) for the reference-artifact convention and the path from visual reference to semantic model to adapter manifestation.

See:

- docs/IDEAL_GUI.md — the philosophical and architectural boundary.
- docs/GUI_TAXONOMY.md — GUI purposes independent of platform.
- docs/MANIFEST_BOUNDARY.md — why GUI implementation stays outside experience manifests.
- docs/GUI_EXPRESSIVENESS.md — the intended visualization and interaction domain.
- docs/GUI_LIFECYCLE.md — ephemeral and multi-observer lifecycle.
