# GUI Architecture

![The engineering boundary](assets/gui-engineering-boundary.jpg)

*The semantic model is the protected boundary; native platform details remain in manifestations and adapters.*

## Purpose

The GUI repository owns the engineering boundary between GUI intent and platform manifestation.

A consuming application should be able to describe a user interface without choosing WPF, Blazor, Unity UI Toolkit, or another presentation technology at the point where the domain experience is authored.

## Layers

### 1. Core model

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

### 2. Platform adapters

Platform projects translate the neutral model into native constructs.

A platform adapter owns:

- native element creation;
- platform-specific layout;
- event wiring;
- accessibility APIs;
- rendering lifecycle;
- resource management;
- platform capability differences.

For example, GUI.Blazor manifests the Core Hub through BlazorGuiHubRenderer. A future WPF or Unity adapter can consume the same Hub root without changing Core.

A platform adapter must not force platform-native concepts into Core merely because the target platform exposes them.

### 3. Domain builders

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

This prevents the composition kernel from acquiring a GUI or browser dependency while still giving every host a common default presentation boundary.

## Design principle

Core should be boring.

The sophistication belongs in the contracts and in the adapters, not in a hidden dependency on one rendering technology.
