# GUI.SVG adapter

GUI.SVG is the SVG manifestation adapter for the platform-neutral GUI model. It accepts a `GuiNode` snapshot and produces a standalone SVG XML document. It is useful for inspectable 2D scenes, generated documentation diagrams, visual proofs, and CPU-side integrations such as the Visualize proof backend.

It is deliberately an adapter, not a second semantic model. Core owns the node tree; this package owns SVG element names, attributes, XML escaping, SVG coordinate conventions, and SVG animation instructions.

## Quick example

```csharp
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

var scene = GuiBuilder.Create("svg-scene", "scene")
    .Text("Workshop geometry")
    .Property("description", "A rounded panel with a highlighted marker.")
    .Child("svg-rect", "panel", panel => panel
        .Property("x", "5").Property("y", "5")
        .Property("width", "90").Property("height", "90")
        .Property("rx", "4")
        .Property("fill", "#0b1725")
        .Property("stroke", "#294055"))
    .Child("svg-circle", "marker", marker => marker
        .Property("cx", "50").Property("cy", "50")
        .Property("r", "5").Property("fill", "#7ee0bd")
        .Property("transform", "translate(2 0)"))
    .Build();

var svg = new SvgGuiRenderer().Render(scene, width: 960, height: 540);
File.WriteAllText("scene.svg", svg);
```

The output is a regular SVG document; a browser, Markdown viewer, image viewer, or downstream pipeline can inspect it without a GUI runtime.

## Current manifestation vocabulary

| GUI node kind | SVG element or behavior |
|---|---|
| `svg-scene`, `svg` | Root document and scene container |
| `coordinate-system`, `coordinate-point` | Normalized coordinate plane; Cartesian Y is inverted for SVG |
| `svg-circle`, `svg-ellipse` | Circle and ellipse |
| `svg-line`, `svg-rect` | Line and rectangle; rectangles also accept `rx` / `ry` |
| `svg-polyline`, `svg-polygon`, `svg-path` | Connected point lists, closed facets, and SVG path data |
| `svg-text`, `svg-tspan` | Text, multiline text spans, and text styling |
| `svg-image` | Image reference from `href` or the Core node's `Source` |
| `svg-defs`, `svg-linear-gradient`, `svg-radial-gradient`, `svg-stop` | Reusable gradient definitions |
| `svg-symbol`, `svg-use` | Define and reuse a semantic geometry fragment |
| Gradients and patterns with `href` | Inherit or reuse an existing definition rather than duplicating it |
| `svg-a` | Navigable links inside the SVG, with explicit destination and optional target/relationship metadata |
| `svg-desc`, `svg-metadata`, `svg-view`, `svg-switch` | Semantic descriptions, metadata, named views, and conditional-content containers |
| `svg-pattern`, `svg-marker` | Reusable fill patterns and path markers |
| `svg-clip-path`, `svg-mask` | Clipping and masking definitions |
| `svg-filter` and `svg-fe-*` | Filter pipelines: blur, offset, color matrix, blend, flood, composite, merge, turbulence, displacement, morphology, tiling, convolution, component transfer, image input, and diffuse/specular lighting with distant, point, or spot lights |
| `svg-animate`, `svg-animate-transform`, `svg-animate-motion`, `svg-set`, `svg-mpath` | Attribute/transform/motion animation, discrete timed changes, and reusable referenced motion paths |
| Other node kinds | A grouping element (`g`), retaining child structure and a title when text is present |

The filter vocabulary includes the channel transfer functions `svg-fe-func-r`, `svg-fe-func-g`, `svg-fe-func-b`, and `svg-fe-func-a`; these belong beneath `svg-fe-component-transfer`. Lighting source nodes belong beneath diffuse/specular lighting primitives. These nodes describe standard SVG filter graphs; actual appearance and support depend on the SVG viewer.

Presentation properties include transforms, opacity, fill/stroke settings, dash patterns, line caps and joins, font settings, visibility, clipping, masking, filters, class/style, and accessibility attributes. Properties are serialized as XML attributes; XML text and attribute escaping is handled by `System.Xml.Linq`.

## External references and URL policy

This adapter does **not** apply a URL-scheme allow-list or fetch/resolve references. It serializes reference values supplied by the scene; XML well-formedness is not a destination-safety guarantee.

- `svg-a` preserves its required `href` and optional navigation metadata.
- `svg-image` serializes its `href` or Core `Source` value. `svg-fe-image` can also carry an `href`.
- Reusable definitions and motion paths can carry references through `href`, including `svg-use`, gradient/pattern inheritance, and `svg-mpath`.
- Fragment references such as `#definition-id` and external URLs are emitted as supplied. The renderer does not currently guarantee a restricted set of schemes, origins, or local-only resources.

Consumers that handle untrusted or remotely supplied scenes must enforce their own scheme/origin policy before rendering or serving the SVG, and must consider the target viewer's external-resource and navigation behavior. Do not treat generated SVG as sanitized merely because it is well-formed XML. This package's contract is serialization, not URL authorization.

## Generated gallery

The checked-in `docs/assets/svg-adapter-gallery.svg` is a snapshot emitted by `samples/SvgGallery/Program.cs`. CI regenerates the SVG and compares it byte-for-byte with the snapshot before packing, so a scene change cannot silently leave the documentation image behind. When the sample changes intentionally, regenerate the image with the sample, review the resulting SVG, and update the snapshot in the same change.

## Coordinate systems

The coordinate-system builder supplies a shared minimum/maximum range and can opt into grid, axes, origin, and bounds. The renderer maps coordinates to a normalized 100-unit-wide viewBox and flips Y so Cartesian points remain intuitive (larger Y appears higher). A non-square output changes the viewBox height to preserve the requested aspect ratio.

## Animation is declarative

`svg-animate`, `svg-animate-transform`, and `svg-animate-motion` emit SVG animation elements from explicit input nodes. They do not start a clock, advance FSM_API state, or make this adapter a GPU renderer. An SVG viewer may execute supported SVG animation instructions; authoritative state and timing remain outside this package.

## Accessibility and output

The root document receives a `role` and `aria-label`, plus a `title`. Set `description` on the root node to emit an SVG `desc`. Individual images can provide descriptive text. Text content is XML-escaped. The renderer validates output dimensions, coordinate ranges, finite numeric properties, non-negative radii/dimensions, unique node IDs (including synthesized coordinate overlays), and a maximum tree depth of 256. Tree-depth validation is iterative and runs before recursive serialization, so an excessively deep scene is rejected with a clear exception instead of exhausting the call stack.

## Trust and security boundary

**XML escaping is not the same as sanitizing SVG behavior.** The renderer produces well-formed XML and constrains several structural properties, but it is not a security sanitizer for arbitrary or untrusted scene data.

- Treat `GuiNode` trees, property values, image sources, links, and embedded style values as trusted application input unless a separate validation policy says otherwise.
- SVG output can contain references to external resources and navigation destinations. A renderer that does not fetch those resources itself does not prevent a browser or other SVG viewer from resolving them later.
- Do not use this serializer alone as a safe-preview boundary for user-submitted SVG content. Validate allowed URLs, CSS/style, path data, and element-specific properties according to the consuming application's threat model before rendering or serving the result.
- When generating documentation or CI artifacts, prefer local assets and controlled scene definitions; inspect generated SVG when adding new node kinds or properties.

This is a statement of the current boundary, not a claim that GUI.SVG already implements a general-purpose sanitizer or URL allow-list.

## Feature gallery

The repository includes a runnable gallery that composes a gradient-backed panel, a clipping definition, vector paths, a polygon facet, accessible labels, and an animated marker using the same public builder surface.

Run it from the repository root:

```sh
dotnet run --project samples/SvgGallery/SvgGallery.csproj --configuration Release
```

By default it writes `docs/assets/svg-adapter-gallery.svg`. CI runs the same sample and uploads the generated document as the `gui-svg-feature-gallery` artifact, so the actual adapter output can be inspected independently of source code.

## Verification

The contract suite covers coordinate mapping, primitive geometry, transforms, XML escaping, gradient/clip/mask/pattern definitions, symbol reuse, markers, filter primitives, image and text accessibility, explicit animation nodes, invalid ranges, and invalid dimensions.

Run:

```sh
dotnet test tests/SVG.Tests/TheSingularityWorkshop.GUI.SVG.Tests.csproj --configuration Release
```

For the Visualize integration, see the [Visualize repository](https://github.com/TrentBest/TheSingularityWorkshop.Visualize) and its GUI.SVG proof output. Visualize consumes GUI.SVG through an explicit adapter; neither repository is a dependency of the other's Core model.
