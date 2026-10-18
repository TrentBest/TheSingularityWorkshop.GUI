# TheSingularityWorkshop.GUI.Blazor

A Blazor manifestation adapter and builder helpers for The Singularity Workshop GUI model.

**Package:** `TheSingularityWorkshop.GUI.Blazor`  
**Current project target:** `.NET 8.0`

## Two related surfaces

- `BlazorGuiRenderer.Render(GuiNode)` recursively maps a neutral `GuiNode` tree to a Blazor `RenderFragment`.
- The fluent Blazor builders provide platform-specific composition helpers, including element attributes and event callbacks.

These are related but not interchangeable contracts. The recursive `GuiNode` renderer is a prototype adapter, not a complete cross-platform widget renderer.

## Minimal model-rendering example

```csharp
using TheSingularityWorkshop.Workshop.Gui;

var tree = GuiBuilder.Create("Panel", "settings")
    .Child("Button", "save", button => button.Text("Save"))
    .Build();

var fragment = BlazorGuiRenderer.Render(tree);
// Render the fragment from a Blazor component/host.
```

The renderer maps the known kinds `Panel`, `Button`, `Image`, `Text`, and `Warning` to HTML elements. Unknown kinds fall back to a `div`. Node identity is emitted as the HTML `id`; a property named `id` is ignored so it cannot override that identity. Other string properties are forwarded as render attributes by the current prototype.

## Important limits

- Arbitrary properties are not a portable semantic contract shared with SVG or WPF.
- The recursive model renderer does not automatically convert every property into a typed HTML, accessibility, or interaction contract.
- Duplicate IDs within a rendered tree and trees deeper than 256 edges are rejected.
- Do not assume that the recursive renderer wires events declared through separate builder helpers.
- Full lifecycle/resource management and broad cross-adapter parity remain unresolved.

## Documentation

- [Platform adapter guide](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/PLATFORM_ADAPTERS.md)
- [GUI model contract](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/GUI_MODEL.md)
- [Testing strategy](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/TESTING.md)
- [Release readiness checklist](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/RELEASE_READINESS.md)
