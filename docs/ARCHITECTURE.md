# GUI Architecture

![The engineering boundary](assets/gui-engineering-boundary.jpg)

*The semantic model is the protected boundary; native platform details remain in manifestations and adapters.*

## Purpose

The GUI repository owns the engineering boundary between GUI intent and platform manifestation.

A consuming application should be able to describe a user interface without choosing WPF, Blazor, Unity UI Toolkit, or another presentation technology at the point where the domain experience is authored.

## Ownership

Each layer has a deliberately narrow owner:

| Layer | Owns | Does not own |
|---|---|---|
| **GUI.Core** | semantic GUI tree, builder primitives, Hub contract | rendering, WPF, Blazor, HTML, XAML, runtime composition |
| **GUI.Blazor** | browser manifestation | semantic ownership, runtime composition |
| **GUI.WPF** | desktop manifestation and WPF-native builders | Core semantics, FSM_COS composition |
| **FSM_COS** | runtime composition and assembly | rendering implementation |
| **WebPage / WebForge** | browser host and domain experience | generic GUI semantics |

This ownership table is a key architectural constraint.

## 1. Core model

GUI is the human-facing semantic bridge between datum and platform manifestation. See [Datum, Presentation, and Interaction Boundary](DATUM_AND_INTERACTION_BOUNDARY.md) for the ownership model: GUI describes presentation and interaction semantics around datum, while domain/application code retains datum ownership and platform adapters retain native manifestation.

src/Core contains platform-neutral primitives.

The current model is a recursive tree:

- a node has a kind and stable identity;
- a node may have text or a source;
- a node may have semantic properties;
- a node may contain zero or more child nodes;
- builders construct trees recursively;
- a built tree is a snapshot and does not retain mutable builder state.

Core also defines the intrinsic GUI Hub boundary. IGuiHub exposes identity plus a semantic GuiNode root. The Hub is therefore a platform-neutral surface, not a renderer and not a Blazor component.

Core must not reference WPF, Blazor, HTML, CSS, JavaScript, XAML, Unity, or another presentation framework.

## 2. Platform manifestations

Platform projects translate the neutral model into native constructs.

### Blazor

GUI.Blazor currently manifests the canonical Core kinds through a Blazor renderer. Its implementation is intentionally conservative: it maps the supported semantic vocabulary and selected common properties rather than pretending all future GUI capabilities already exist.

### WPF

GUI.WPF currently provides two related surfaces:

1. **Semantic manifestation** — WpfGuiRenderer and WpfGuiHubRenderer consume Core trees/Hub roots and create WPF controls.
2. **Native builder system** — WPF-specific builders expose richer desktop composition primitives where Core intentionally remains neutral.

The native builder system includes layouts, controls, dialogs, reflection, diagnostics, and window-oriented composition.

These two surfaces are complementary. A WPF application can either consume the semantic Core model or intentionally use the WPF-native builder APIs.

## 3. Platform adapters

A platform adapter owns:

- native element creation;
- platform-specific layout;
- event wiring;
- accessibility APIs;
- rendering lifecycle;
- resource management;
- platform capability differences.

A platform adapter must not force platform-native concepts into Core merely because the target platform exposes them.

## 4. Domain builders

A consuming application may provide builders such as FsmForgeGuiBuilder or SpatialWorldGuiBuilder.

These builders express domain intent. They belong in the consuming repository because the GUI repository should not know what a Forge, workshop, laboratory, or AEC office means.

## Hub and composition

The Hub is the default GUI surface, but GUI does not own runtime composition.

```text
RuntimeManifest
      |
      v
   FSM_COS
      |
      v
 RuntimeAssembly
      |
      +---- Hub MicroBundle
                |
                v
          GUI Core semantic Hub
                |
                v
       platform-specific adapter
        /          |          \
     Blazor       WPF        Unity
```

The Hub MicroBundle is an ordinary FSM_COS composition participant. FSM_COS assembles it; GUI.Core defines what its platform-neutral surface means; a platform adapter decides how that surface is manifested.

## Current boundary

The architecture is intentionally ahead of some implementation.

Implemented today:

- recursive Core model and builder vocabulary;
- Core Hub surface;
- Blazor manifestation;
- WPF manifestation;
- WPF-native builder family;
- package/build lanes for Core, Blazor, and WPF.

Not implemented as stable runtime contracts yet:

- semantic input;
- capability negotiation;
- accessibility contract;
- deterministic GUI serialization;
- GUI execution host;
- stable GUI MicroBundle contract;
- Unity manifestation.

See [Implementation Status](IMPLEMENTATION_STATUS.md) and [Roadmap](../ROADMAP.md) before treating an architectural document as an API guarantee.

## Design principle

**Core should be boring.**

The sophistication belongs in the contracts and in the adapters, not in a hidden dependency on one rendering technology.
