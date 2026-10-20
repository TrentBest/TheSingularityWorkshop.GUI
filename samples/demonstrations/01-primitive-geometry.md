# 01 — Primitive geometry

**Level:** 1 · **Topic:** Basic SVG manifestation · **Status:** Runnable adapter-generation proof added; execution still needs verification

[Open the SVG artifact](01-primitive-geometry.svg) · [View the gallery](index.html)

## What it shows

A standalone SVG containing a background, a rounded rectangle, a circle, a line, and labeled text. These are intentionally elementary primitives: they let us isolate the output format before introducing coordinate transforms, animation, or simulation state.

## What it proves

- The artifact is valid standalone SVG markup that a browser can render.
- The visual elements have explicit geometry and styling.
- The file can serve as a stable expected-output reference for an adapter test.

## What it does not prove yet

- **It does not prove that GUI.SVG generated this exact file.** The artifact was authored as a reference.
- It does not prove FSM_API is driving anything.
- It does not prove FSM_COS composition, GPU rendering, or interoperability with another host.

We must preserve these distinctions instead of treating a picture as evidence for a runtime claim.

## Runnable adapter generation proof

The actual source is now in [PrimitiveGeometry/Program.cs](PrimitiveGeometry/Program.cs), with project references to the repository's Core and SVG implementations. Follow [its run instructions](PrimitiveGeometry/README.md). It parses the renderer output and asserts that the expected rectangle and circle are present. This is stronger than the reference picture, but the project has not been executed in this session, so a passing run is not yet claimed.

## Adapter generation contract

The current adapter documentation shows the intended API shape:

```csharp
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

var scene = GuiBuilder.Create("svg-scene", "scene")
    .Text("Primitive geometry")
    .Child("svg-rect", "panel", panel => panel
        .Property("x", "10").Property("y", "10")
        .Property("width", "80").Property("height", "60")
        .Property("rx", "5")
        .Property("fill", "#173b4c")
        .Property("stroke", "#7ee0bd"))
    .Child("svg-circle", "marker", marker => marker
        .Property("cx", "50").Property("cy", "40")
        .Property("r", "8").Property("fill", "#ffb86c"))
    .Build();

var svg = new SvgGuiRenderer().Render(scene, width: 960, height: 540);
File.WriteAllText("01-primitive-geometry.generated.svg", svg);
```

This snippet follows the documented builder/renderer pattern. The next verification task is to compile and run the checked-in proof project, then compare the generated output with the reference (normalizing irrelevant serializer differences if needed). If the current API differs, update the example to match the implementation rather than changing the implementation to fit an unverified snippet.

## Acceptance criteria

1. The sample compiles against the actual GUI.SVG project.
2. Running it produces a standalone SVG file.
3. Automated validation parses the SVG as XML and checks for the expected rectangle, circle, and text nodes.
4. The generated output is linked from this card.
5. The test fails if a required primitive disappears or is serialized incorrectly.

## Next step

Verify this sample against the real project and its test conventions. Only after that should we progress to coordinate systems and transforms.
