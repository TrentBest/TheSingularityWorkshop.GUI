<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# SvgGuiRenderer

**Purpose.** `SvgGuiRenderer` manifests a Core `GuiNode` tree as an SVG XML document. It produces a serialized visual artifact; it does not run an animation clock, execute FSM_API, host browser interaction, or render through a GPU.

## UML class card

![SvgGuiRenderer UML relationship diagram](../../uml/source-files/SVG/TheSingularityWorkshop-Workshop-Gui-Svg-SvgGuiRenderer-188b9d58e1.svg)

See the [focused UML and use cases](../../uml/SvgGuiRendererUML.md).

## Contract and behavior

- `Render(root, width, height)` returns a compact XML document containing an SVG root, intrinsic dimensions, a viewBox, accessibility metadata, and rendered children.
- The root must be non-null and dimensions must be positive integers.
- The renderer checks unique IDs and rejects nesting deeper than 256 levels before recursive serialization.
- Root `aria-label` falls back to root text and then root ID. A nonblank `description` property adds an SVG `desc` and associates it with the root through `aria-labelledby`.
- Child order is preserved. The renderer maps explicit node kinds to SVG elements, including basic geometry, text, paths, groups, images, links, gradients, clipping/masking, and additional scene constructs supported by the implementation.
- Coordinates are formatted invariantly, and XML is constructed with `XElement`/`XDocument`, which handles XML escaping.
- `coordinate-system` roots use the configured coordinate range to map coordinate points into the view box.

## Example

```csharp
var svg = new SvgGuiRenderer().Render(scene, width: 960, height: 540);
```

Here `scene` must be a Core `GuiNode` whose kinds/properties are supported by the SVG adapter. The returned string can be saved as an SVG artifact or supplied to a downstream viewer subject to that viewer's policy.

## Failure boundaries

| Condition | Behavior |
|---|---|
| Null root | `ArgumentNullException` |
| Width or height not positive | `ArgumentOutOfRangeException` |
| Duplicate ID | `ArgumentException` |
| Depth greater than 256 | `InvalidOperationException` |
| SVG link node with blank `href` | `ArgumentException` |

**Security boundary:** valid XML is not the same as safe-to-display SVG. This serializer does not establish a URL allow-list, sanitize external references, or promise that downstream viewers will treat embedded styles/links safely. Apply consumer-specific policy before displaying untrusted SVG.

## Verification

See the [SVG adapter contract](../../SVG_ADAPTER.md), [renderer tests](../../../tests/SVG.Tests/SvgGuiRendererTests.cs), and the [package workflow](../../../.github/workflows/package.yml).

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
