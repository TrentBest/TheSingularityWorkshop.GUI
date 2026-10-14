# GUI.SVG Guide

> **Status:** initial implementation; build and test verification still required.

![One semantic GUI, multiple manifestations](images/gui-svg-manifestation.svg)

GUI.SVG is intended to be the SVG manifestation beside GUI.Blazor and GUI.WPF. GUI.Core describes the interface; each manifestation decides how to present it.

## First render

```csharp
using TheSingularityWorkshop.GUI.SVG;
using TheSingularityWorkshop.Workshop.Gui;

var view = GuiBuilders.Column("settings")
    .Child(GuiBuilders.Text("title", "Settings"))
    .Child(GuiBuilders.Button("save", "Save"))
    .Build();

var svg = new SvgGuiRenderer().Render(view, new SvgRenderOptions
{
    Title = "Settings interface",
    Description = "A title and a save button.",
    Width = 800,
    Height = 400,
    Background = "#101827"
});
```

The builder chain shown here is the same public composition style documented in the repository README. The SVG project and tests still require an actual build/test run before release readiness can be claimed.

## Current contract

- Input: a `GuiNode` semantic tree from GUI.Core.
- Output: standalone SVG markup as a string.
- Options: positive width/height, optional accessible title and description, optional background.
- Basic node rendering: text, button, warning, separator, and a fallback for other kinds.
- Text content and accessibility labels are encoded before insertion.
- Nesting beyond 64 levels is rejected.

## Boundaries

This first pass is a deterministic serializer, not a browser application or interaction runtime. SVG buttons are pictures of buttons: they do not dispatch GUI events. Layout is basic and does not yet provide a full constraint, flex, grid, or responsive layout engine. Image sources, typed styling, state updates, keyboard interaction, animation contracts, and adapter conformance remain future work unless verified in code.

## Living motion example

[Open the animated energy-field SVG](examples/living-energy-vortex.svg). It explores alternating gold/blue pulses, a slowly rotating orbital field, a breathing focal point, and a reduced-motion fallback. It is an illustration of SVG's expressive potential—not output from `SvgGuiRenderer`, not a physical light simulation, and not yet connected to GUI node state.

![Living energy-field motion study](examples/living-energy-vortex.svg)

## Next capabilities

1. Define typed style and layout mappings shared with Core.
2. Render all canonical node kinds and supported properties.
3. Add stable IDs and a safe attribute policy.
4. Define interactivity as a separate host-controlled contract.
5. Test escaping, dimensions, accessibility metadata, nesting limits, and each node kind.
6. Add living examples where motion clarifies behavior.

Related: [Start here](START_HERE.md) · [Architecture](ARCHITECTURE.md) · [Implementation status](IMPLEMENTATION_STATUS.md) · [Testing](TESTING.md) · [Living documentation](LIVING_DOCUMENTATION.md)
