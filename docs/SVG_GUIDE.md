# GUI.SVG Guide

## Purpose and boundary

`TheSingularityWorkshop.GUI.SVG` turns a renderer-neutral `GuiNode` scene tree into a standalone SVG document. The current implementation lives in `src/SVG/SvgGuiRenderer.cs`, in namespace `TheSingularityWorkshop.Workshop.Gui.Svg`. Core nodes keep their string-valued properties and ordered children; SVG-specific elements use `svg-` kind identifiers.

The renderer **serializes a scene**. It does not run FSM_API, own an application event loop, perform GPU rendering, or supply a live scene clock. SVG animation can be described explicitly in the model and emitted into the document; a compatible browser/viewer must interpret the resulting SVG/CSS animation.

## Minimal scene

```csharp
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

var scene = GuiBuilder.Create("svg-scene", "example-scene")
    .Text("A small SVG scene")
    .Property("viewBox", "0 0 960 540")
    .Property("aria-label", "A green dot on a dark field")
    .Property("description", "A demonstration scene with a background rectangle and a circle.")
    .Child("svg-rect", "background", node => node
        .Property("x", "0").Property("y", "0")
        .Property("width", "960").Property("height", "540")
        .Property("fill", "#101827"))
    .Child("svg-circle", "dot", node => node
        .Property("cx", "480").Property("cy", "270")
        .Property("r", "28").Property("fill", "#7ee0bd"))
    .Build();

var svg = new SvgGuiRenderer().Render(scene, width: 960, height: 540);
File.WriteAllText("scene.svg", svg);
```

The root's `viewBox` sets the scene coordinate space. The renderer also writes intrinsic width/height, a title, an accessible name, and a description when the root's `description` property is nonblank.

## Implemented element families

The current switch in `SvgGuiRenderer.RenderNode` has explicit cases for:

- Geometry and text: circle, ellipse, line, rectangle, polyline, polygon, path, text, tspan.
- Scene composition and references: group, view, switch, image, definitions, symbol, use, pattern, marker.
- Paint and clipping: linear/radial gradients and stops, clip paths, masks.
- Filters: filter containers and supported filter primitives, including blur, turbulence, displacement map, compositing, blending, morphology, merge, and lighting-related elements.
- Navigation and semantics: links, title, description, metadata, and presentation/accessibility attributes.
- Explicit motion: `svg-set`, `svg-mpath`, `svg-animate`, `svg-animate-transform`, and `svg-animate-motion`.

This list describes serializer cases, not a promise of uniform browser support or complete conformance to every SVG specification edge case. Check the current renderer and tests when relying on a specific attribute or element.

## Coordinate-system scenes

A root of kind `coordinate-system` enables the coordinate overlay and maps Cartesian Y-up coordinates to SVG's Y-down space. Its numeric range must be valid and finite. `coordinate-point` children are mapped through that range. The overlay synthesizes IDs, so user-provided IDs must not collide with them.

For examples, see [the Fox/Sheepdog scene model](../samples/FoxSheepdogPlane/Program.cs) and [the checked-in scene SVG](assets/fox-sheepdog-10x10.svg). The sample constructs a 10×10 isometric plane, reusable actor symbols, CSS choreography, and explicit SVG motion-path nodes. It is a scene-authored manifestation—not evidence of a physics engine, live dog-detection service, or application runtime controlling those actors.

## Validation and guarantees tested

The renderer currently:

- rejects a null root and non-positive surface dimensions;
- validates unique node IDs before serialization, including collisions with generated coordinate-overlay IDs;
- rejects excessive nesting before recursive serialization (the tested maximum depth is 256);
- validates coordinate ranges and numeric geometry, including malformed/non-finite values and negative dimensions where prohibited;
- rejects an SVG link without a destination;
- serializes through XML APIs, escaping XML text and attribute content;
- produces deterministic bytes when the same scene is rendered twice.

These behaviors have corresponding tests in [`SvgGuiRendererTests.cs`](../tests/SVG.Tests/SvgGuiRendererTests.cs). The suite also covers geometry, transforms, styles, definitions, gradients, clips, masks, image/text elements, navigation/metadata, filters, and explicit animation elements. Passing unit tests is not a substitute for checking the final SVG in target browsers.

## Accessibility, motion, and external resources

Provide meaningful root `aria-label` text and a `description` for complex scenes. The renderer emits a title and, when present, a description and corresponding `aria-labelledby` references. This is useful markup evidence, **not proof** of keyboard behavior, screen-reader output, or complete accessibility for every descendant.

For CSS animation, include a `prefers-reduced-motion: reduce` path when motion is nonessential. The Fox/Sheepdog sample includes one. The renderer does not run or enforce that CSS policy; inspect the authored stylesheet and test the rendered artifact in browsers.

The renderer supports SVG links, image nodes, references, and filter resources, but these features do not establish a repository-wide safe external-resource policy. Treat external links, referenced media, embedded styles, and resource loading as a separate review area. Do not infer that an SVG is safe or self-contained merely because it serialized successfully.

## Visual concepts versus framework evidence

The files in [SVG visual concepts](visual-concepts/README.md) are hand-authored illustrations. They are not generated by `SvgGuiRenderer` and must not be presented as proof of framework behavior. For a behavior claim, point to the canonical scene model, the generated output, the relevant test, and—where browser behavior matters—hosted/browser evidence.

## Next verification steps

- Keep the checked-in Fox/Sheepdog SVG reconciled with its canonical `Program.cs` scene model; do not overwrite the authored asset when generator parity is unproven.
- Expand browser-level checks for SVG accessibility, reduced-motion behavior, filters, references, and SMIL/CSS animation differences.
- Continue source-derived UML and per-`.cs` documentation from the current source; do not confuse authored diagrams with generated source evidence.
- Add cross-adapter conformance tests only when a shared semantic/layout contract exists; SVG rendering behavior alone does not define that contract.
