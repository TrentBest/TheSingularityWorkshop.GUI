# Visualize and Renderer: hosted viewport boundary

This document defines the first executable seam between GUI's semantic presentation model and an experience-owned visual scene.

## Ownership

- **GUI.Core** describes a semantic `Viewport` region in the interface tree. It does not own the scene, observer policy, GPU resources, or simulation state.
- **GUI.Blazor** can delegate a `Viewport` node to a host-supplied Blazor `RenderFragment`.
- **Renderer** selects observer-aware representation and computation policy.
- **Visualize** executes the selected visual representation, including browser WebGPU work when the browser and device support it.
- **The host** composes those systems, supplies the surface fragment, routes permitted input, and owns lifecycle and authorization.

The direction is intentionally one-way: GUI offers a place to present an experience-owned surface; it does not take a dependency on Visualize or Renderer.

## Implemented contract

`BlazorGuiRenderer.RenderWithHostedSurface(GuiNode, Func<GuiNode, RenderFragment?>)` walks the normal immutable GUI tree. When it encounters a node whose kind is `Viewport`, it:

1. asks the host callback for a presentation fragment;
2. requires a non-null fragment rather than silently rendering an empty viewport;
3. wraps the fragment in a semantic `section` with the stable node ID;
4. exposes `role="region"`, an accessible label, and `data-gui-viewport="true"`;
5. forwards ordinary presentation properties while protecting identity, role, accessible label, and host-command attributes.

The callback receives the original `GuiNode`, so the host can use its stable ID and authored properties to select the appropriate scene or renderer. Other nodes continue through the existing Blazor renderer. Nested viewports are supported.

Example:

```csharp
var page = GuiBuilder.Create("Panel", "experience")
    .Child(GuiBuilders.Viewport("battlefield", "Battlefield view")
        .Property("class", "battlefield-viewport"))
    .Build();

var content = BlazorGuiRenderer.RenderWithHostedSurface(
    page,
    viewport => builder =>\n    {\n        builder.OpenComponent<BattlefieldSurface>(0);\n        builder.CloseComponent();\n    });
```

`BattlefieldSurface` is host-owned. It may compose Visualize, consume Renderer policy, or provide another presentation implementation. The GUI package does not inspect or control that component.

## What this does not claim

This seam is not yet a complete spatial GUI contract. It does not establish:

- typed camera, projection, transform, or coordinate-space semantics;
- automatic Renderer policy negotiation or level-of-detail selection;
- WebGPU availability, GPU timing, or a particular graphics backend;
- pointer, gaze, controller, keyboard, or spatial hit-testing integration;
- resize observation, focus transfer, suspension, disposal, or GPU resource lifecycle;
- cross-adapter parity with SVG or WPF.

The host must still validate commands and authorize all domain changes. A viewport's stable GUI identity is not a scene identity, and a host fragment is not permission to mutate the represented domain.

## Verification

The Blazor renderer tests verify nested viewport delegation, stable wrapper identity, accessible region metadata, presentation-property forwarding, null callback rejection, and the requirement that every delegated viewport receive a host fragment.

Core also provides `GuiBuilders.Viewport(id, accessibleLabel)` so callers can create a labelled viewport without repeating the raw kind string. The next vertical slice is a real Blazor host that supplies the current Visualize WebGPU runner as its viewport implementation, with explicit resize, focus, input, capability, and disposal behavior. Browser execution must be tested on an actual supported browser/device; a .NET unit test cannot prove GPU execution.

## References

- [GUI spatial domain](../GUI_SPATIAL_DOMAIN.md)
- [GUI spatial contracts](../GUI_SPATIAL_CONTRACTS.md)
- [GUI execution model](../GUI_EXECUTION_MODEL.md)
- [Visualize](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/tree/development)
- [Renderer](https://github.com/TrentBest/TheSingularityWorkshop.Renderer/tree/development)
