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
| `svg-clip-path`, `svg-mask` | Clipping and masking definitions |
| `svg-animate`, `svg-animate-transform`, `svg-animate-motion` | Explicit SVG animation instructions |
| Other node kinds | A grouping element (`g`), retaining child structure and a title when text is present |

Presentation properties include transforms, opacity, fill/stroke settings, dash patterns, line caps and joins, font settings, visibility, clipping, masking, filters, class/style, and accessibility attributes. Properties are serialized as XML attributes; XML text and attribute escaping is handled by `System.Xml.Linq`.

## Coordinate systems

The coordinate-system builder supplies a shared minimum/maximum range and can opt into grid, axes, origin, and bounds. The renderer maps coordinates to a normalized 100-unit-wide viewBox and flips Y so Cartesian points remain intuitive (larger Y appears higher). A non-square output changes the viewBox height to preserve the requested aspect ratio.

## Animation is declarative

`svg-animate`, `svg-animate-transform`, and `svg-animate-motion` emit SVG animation elements from explicit input nodes. They do not start a clock, advance FSM_API state, or make this adapter a GPU renderer. An SVG viewer may execute supported SVG animation instructions; authoritative state and timing remain outside this package.

## Accessibility and output

The root document receives a `role` and `aria-label`, plus a `title`. Set `description` on the root node to emit an SVG `desc`. Individual images can provide descriptive text. Text content is XML-escaped. The renderer validates output dimensions, coordinate ranges, finite numeric properties, and non-negative radii/dimensions.

The adapter does not fetch remote images, validate arbitrary SVG path syntax, implement an interaction runtime, or promise identical rendering across SVG viewers. Use local or trusted image sources in documentation and demos.

## Verification

The contract suite covers coordinate mapping, primitive geometry, transforms, XML escaping, gradient/clip/mask definitions, image and text accessibility, explicit animation nodes, invalid ranges, and invalid dimensions.

Run:

```sh
dotnet test tests/SVG.Tests/TheSingularityWorkshop.GUI.SVG.Tests.csproj --configuration Release
```

For the Visualize integration, see the [Visualize repository](https://github.com/TrentBest/TheSingularityWorkshop.Visualize) and its GUI.SVG proof output. Visualize consumes GUI.SVG through an explicit adapter; neither repository is a dependency of the other's Core model.
