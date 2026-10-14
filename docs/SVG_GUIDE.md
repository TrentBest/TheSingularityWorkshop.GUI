# GUI.SVG Guide

> **Status:** early implementation. The project, serializer, and tests are in the repository; a successful build/test run is still required before the adapter can be called verified.

![One semantic GUI, multiple manifestations](images/gui-svg-manifestation.svg)

GUI.SVG is a sibling manifestation beside GUI.Blazor and GUI.WPF. GUI.Core describes the interface; SVG turns a semantic tree into a standalone vector document. That makes SVG useful for portable artifacts, diagrams, generated documentation, previews, and visual experiments—without pretending an SVG file is an interactive application.

## First render

```csharp
using TheSingularityWorkshop.GUI.SVG;
using TheSingularityWorkshop.Workshop.Gui;

var view = GuiBuilders.Column("settings")
    .Child(GuiBuilders.Text("title", "Settings"))
    .Child(GuiBuilders.Button("save", "Save"))
    .Child(GuiBuilders.Warning("status", "Changes are not saved yet."))
    .Build();

var svg = new SvgGuiRenderer().Render(view, new SvgRenderOptions
{
    Title = "Settings interface",
    Description = "A title, a save button, and a status warning.",
    Width = 800,
    Height = 400,
    Background = "#101827"
});
```

The result is an SVG string. The semantic tree remains a Core object; rendering does not mutate it or create a GUI event loop.

## Current renderer coverage

The serializer has explicit basic representations for all 14 canonical kinds in `GuiKinds`:

| Core kind | Current SVG treatment |
|---|---|
| `Text` | Text label |
| `Button` | Button-like visual; not interactive |
| `Warning` | Warning panel |
| `Separator` | Horizontal rule |
| `Panel`, `Stack`, `Row`, `Column`, `Object` | Labeled container outline |
| `TextBox` | Input-like visual; not editable |
| `IntegerSlider`, `FloatSlider` | Slider-like visual; not interactive |
| `Toggle` | On/off visual based on the current property value |
| `Image` | Placeholder panel; source image is not yet embedded or loaded |
| Unknown kind | Neutral fallback representation |

These are visual representations, not a claim of semantic or behavioral parity with Blazor or WPF. In particular, a slider is not a slider control, a button does not dispatch events, and the image node does not yet render its source.

## What the renderer guarantees—and does not

Current implementation includes:

- standalone SVG markup returned as a string;
- configurable positive width and height;
- optional accessible document title and description;
- optional validated background color;
- XML text/attribute escaping;
- a nesting-depth guard;
- basic recursive vertical placement for most containers;
- horizontal placement for direct children of `Row` nodes, with row height estimated from the tallest child.

Still to be designed or proven:

- a shared layout contract for row, column, stack, sizing, spacing, and constraints;
- typed style and theme tokens;
- real image/media loading and safe resource policy;
- explicit semantic accessibility mappings for every node;
- event and state contracts owned by a host;
- animation/transition intent in the semantic model;
- cross-adapter conformance tests;
- successful CI build and test evidence.

The current renderer uses a deliberately small layout model: most containers place children vertically, while `Row` places its direct children side by side and sizes its outline from the tallest child. Container heights are structural estimates—not font-metric measurement or a shared Core layout contract. Row widths are evenly divided from the available width; this is not responsive sizing, and `Stack` orientation is not yet honored. The canonical kind names preserve semantic intent, but the current SVG manifestation is still an early visual mapping.

## Living motion example

![Animated energy field](examples/living-energy-vortex.svg)

[Open the animated energy-field SVG](examples/living-energy-vortex.svg). The study explores alternating gold/blue pulses, a slowly rotating orbital field, a breathing focal point, and a reduced-motion fallback.

This is hand-authored illustrative SVG—not output from `SvgGuiRenderer`, not a physical light simulation, and not yet driven by GUI node state. It demonstrates why SVG is worth developing beyond static screenshot generation.

## Next steps toward a serious manifestation

1. **Correctness:** parse generated output as XML in tests; cover every canonical kind, escaping, dimensions, accessibility metadata, malformed input, and nesting limits.
2. **Layout:** define a shared Core layout contract for row/column/stack orientation, sizing, spacing, and constraints; extend beyond the current estimated horizontal `Row` layout without accidentally inheriting browser/CSS semantics.
3. **Resources:** implement a deliberate image policy, including whether the output references a URI or embeds data.
4. **Style:** define safe, typed style tokens and consistent defaults instead of accepting arbitrary renderer-specific strings.
5. **Conformance:** share test cases across Blazor, WPF, and SVG where semantics overlap, while testing native capabilities separately.
6. **Living documentation:** use motion to explain state and structure, with reduced-motion behavior and clear separation between illustration and generated output.

Related: [Start Here](START_HERE.md) · [GUI Model](GUI_MODEL.md) · [Architecture](ARCHITECTURE.md) · [Implementation Status](IMPLEMENTATION_STATUS.md) · [Testing](TESTING.md) · [Living Documentation](LIVING_DOCUMENTATION.md)
