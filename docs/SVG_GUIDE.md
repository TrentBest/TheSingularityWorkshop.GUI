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

**Verification note:** confirm this builder example against the current Core API by building the project before treating it as a verified sample.

## Current contract

- Input: a `GuiNode` semantic tree from GUI.Core.
- Output: standalone SVG markup as a string.
- Options: positive width/height, optional accessible title and description, optional background.
- Basic node rendering: text, button, warning, separator, and a fallback for other kinds.
- Text content and accessibility labels are encoded before insertion.
- Nesting beyond 64 levels is rejected.

## Boundaries

This first pass is a deterministic serializer, not a browser application or interaction runtime. SVG buttons are pictures of buttons: they do not dispatch GUI events. Layout is basic and does not yet provide a full constraint, flex, grid, or responsive layout engine. Image sources, typed styling, state updates, keyboard interaction, animation contracts, and adapter conformance remain future work unless verified in code.

## Next capabilities

1. Define typed style and layout mappings shared with Core.
2. Render all canonical node kinds and supported properties.
3. Add stable IDs and a safe attribute policy.
4. Define interactivity as a separate host-controlled contract.
5. Test escaping, dimensions, accessibility metadata, nesting limits, and each node kind.
6. Add living examples where motion clarifies behavior.

Related: [Start here](START_HERE.md) · [Architecture](ARCHITECTURE.md) · [Implementation status](IMPLEMENTATION_STATUS.md) · [Testing](TESTING.md) · [Living documentation](LIVING_DOCUMENTATION.md)
