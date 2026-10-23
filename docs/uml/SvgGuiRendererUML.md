# SvgGuiRenderer — UML and use cases

**Purpose:** describe the renderer that converts Core semantic GUI data into an SVG XML artifact.

## UML class diagram

![Compiler-derived class relationship diagram for SvgGuiRenderer](source-files/SVG/TheSingularityWorkshop-Workshop-Gui-Svg-SvgGuiRenderer-188b9d58e1.svg)

The focal class consumes Core `GuiNode` values and uses XML document types to construct the SVG result. Its private coordinate-range helper supports coordinate-system mapping. The linked diagram describes source structure, not runtime execution or complete SVG specification coverage.

## Use cases

### Produce a visual artifact from a semantic scene

1. Build a Core tree with a supported SVG scene root or supported SVG node kinds.
2. Call `new SvgGuiRenderer().Render(scene, width, height)`.
3. Save the returned XML or pass it to a viewer that accepts SVG.
4. Apply the viewer application's own policy to references and embedded content.

**Expected result:** XML serialization of the represented scene, with child order retained and supported presentation attributes mapped to SVG.

### Represent a coordinate system

1. Build a `coordinate-system` root with its supported range metadata.
2. Add `coordinate-point` children.
3. Render the scene with positive dimensions.

**Expected result:** point coordinates are mapped into SVG view space according to the configured range.

## Explicit limits

- This is a serializer/manifestation adapter, not an SVG sanitizer, renderer engine, animation scheduler, GPU backend, or interactive host.
- It enforces ID uniqueness and a maximum depth of 256, but those checks do not make arbitrary untrusted SVG safe.
- Unsupported node kinds and properties must be evaluated against implementation and tests; do not infer complete SVG-spec coverage.
- See the [authored API contract](../api/SVG/SvgGuiRenderer.md), [implementation](../../src/SVG/SvgGuiRenderer.cs), and [renderer tests](../../tests/SVG.Tests/SvgGuiRendererTests.cs).
