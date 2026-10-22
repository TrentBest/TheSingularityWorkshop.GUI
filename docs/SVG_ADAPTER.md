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


## Capability boundaries: interaction, sound, and the fox/sheepdog scene

The adapter contract is intentionally narrower than a browser application. Use the following distinctions when describing what works.

| Capability | Current status | Evidence and boundary |
|---|---|---|
| Produce SVG from the GUI model | Implemented | `SvgGuiRenderer.Render(GuiNode, width, height)` serializes the model into an SVG XML document. The fox/sheepdog sample constructs its scene with `GuiBuilder` and calls this renderer. |
| Draw the fox/sheepdog plane | Implemented in the sample | `samples/FoxSheepdogPlane/Program.cs` builds the grid, animal shapes, scene labels, gradients, and groups using GUI builders and SVG node kinds. Its grid lines are calculated from the documented isometric projection of a regular 10×10 grid. |
| Declarative movement and timed visual changes | Implemented as output generation | The renderer emits `svg-animate`, `svg-animate-transform`, `svg-animate-motion`, and `svg-set` elements. The fox sample also places CSS keyframes in an SVG style node. A compatible SVG viewer runs those instructions; GUI.SVG itself does not own a clock or execute the animation. |
| Browser input that changes application behavior | Not provided by this adapter contract | The adapter can serialize attributes such as `tabindex`, `focusable`, `pointer-events`, and SVG links (`svg-a`). Those do not provide GUI-model event callbacks, application commands, data binding, or a route back into FSM_API/FSM_COS. CSS hover/focus presentation may be authored in style text, but it is not equivalent to application input handling. Use a host/application interaction layer for behavior-changing input. |
| Sound playback authored through the current SVG node vocabulary | Not implemented | There is no audio node, audio playback controller, or audio lifecycle in this adapter. The fox/sheepdog sample contains no sound. Do not claim that it makes sound. Sound requires a separate supported host/media capability; it should not be implied by SVG animation. |
| Execute arbitrary application logic or run FSM_API/FSM_COS | Not implemented | The SVG renderer serializes a model snapshot. It does not load assemblies, invoke FSM transitions, poll runtime state, or bind animation progress to live process groups. |
| Identical behavior in every SVG viewer | Not guaranteed | Output is SVG markup. Animation, CSS, filters, external references, and accessibility behavior depend on the consuming viewer and its support/security policies. The current contract tests validate serialization and selected structures, not every browser/viewer or full animation playback. |

### What the fox/sheepdog proof does—and does not—prove

The source of the scene is real GUI.SVG technology: `samples/FoxSheepdogPlane/Program.cs` creates a recursive GUI model with `GuiBuilder`, then calls `new SvgGuiRenderer().Render(scene.Build(), 1200, 760)` and writes the resulting SVG. This is not merely a hand-drawn SVG file being described as generated by GUI.SVG.

However, distinguish **the builder's output** from **the checked-in visual asset**. The sample writes to `docs/assets/fox-sheepdog-10x10.svg` by default, while that asset has also been edited directly during visual iteration. Unlike `docs/assets/svg-adapter-gallery.svg`, the fox asset does not currently have a documented byte-for-byte regeneration check. Until the sample is run and its output is compared with the committed asset, do not claim that the checked-in asset is an exact, current rendering of the builder source.

The choreography is declarative presentation authored in the sample: the fox route uses SVG motion animation, while wake/reach/cloud/limp-fox/RIP/angel phases use CSS keyframes and opacity/transform changes. It illustrates that GUI.SVG can build and serialize an animated SVG scene. It is **not** a live FSM_API/FSM_COS trace, a physics simulation, or proof that the dog algorithmically decides to attack. Those would require a separate runtime-to-scene integration and tests.

### Reproducible verification

From the repository root:

```sh
dotnet run --project samples/FoxSheepdogPlane/FoxSheepdogPlane.csproj --configuration Release
dotnet test tests/SVG.Tests/TheSingularityWorkshop.GUI.SVG.Tests.csproj --configuration Release
```

The first command exercises the sample's actual builder-to-renderer path and rewrites the default SVG asset. Review the diff after running it: a large diff is evidence that the hand-maintained asset and builder output had drifted, not a reason to hide the difference. The second command verifies the adapter's existing serialization contract; it does not substitute for visually checking the animated scene in a target browser.


## Vocabulary-to-test audit

The renderer has explicit mappings for the node kinds listed below. The named tests assert serialized SVG structure and selected attributes; they do **not** prove identical visual or runtime behavior across browsers and SVG viewers.

| Implemented node kinds | Executable coverage |
|---|---|
| Root and coordinates: `svg-scene`, `svg`, `coordinate-system`, `coordinate-point` | `Renders_coordinate_system_and_maps_cartesian_y_up_to_svg_y_down`, `Rejects_invalid_coordinate_range`, `Rejects_coordinate_ranges_whose_finite_endpoints_overflow_the_span`, `Rejects_collisions_with_synthesized_coordinate_overlay_ids` |
| Primitive geometry and text: `svg-circle`, `svg-ellipse`, `svg-line`, `svg-rect`, `svg-polyline`, `svg-polygon`, `svg-path`, `svg-text`, `text`, `svg-tspan` | `Renders_svg_primitives_and_escapes_text`, `Renders_geometry_and_transform_properties_for_composed_scenes`, `Emits_image_text_spans_and_accessible_description`, `Rejects_negative_geometry_dimensions` |
| Containers, styling, navigation, and metadata: `svg-g`, `svg-switch`, `svg-title`, `svg-desc`, `svg-metadata`, `svg-view`, `svg-style`, `svg-a`, `svg-image` | `Renders_embedded_stylesheet_text_for_scene_choreography`, `Emits_navigable_svg_links_and_semantic_metadata_nodes`, `Emits_svg_title_switch_and_radial_gradient_nodes`, `Emits_image_text_spans_and_accessible_description`, `Rejects_svg_links_without_a_destination` |
| Definitions and reuse: `svg-defs`, `svg-linear-gradient`, `svg-radial-gradient`, `svg-stop`, `svg-clip-path`, `svg-mask`, `svg-symbol`, `svg-use`, `svg-pattern`, `svg-marker` | `Emits_definitions_gradients_clips_and_masks`, `Renders_reusable_symbols_patterns_markers_and_filter_primitives`, `Emits_svg_title_switch_and_radial_gradient_nodes` |
| Filter primitives: `svg-filter`, `svg-fe-gaussian-blur`, `svg-fe-offset`, `svg-fe-color-matrix`, `svg-fe-blend`, `svg-fe-flood`, `svg-fe-composite`, `svg-fe-turbulence`, `svg-fe-displacement-map`, `svg-fe-morphology`, `svg-fe-tile`, `svg-fe-convolve-matrix`, `svg-fe-component-transfer`, `svg-fe-func-r`, `svg-fe-func-g`, `svg-fe-func-b`, `svg-fe-func-a`, `svg-fe-diffuse-lighting`, `svg-fe-specular-lighting`, `svg-fe-distant-light`, `svg-fe-point-light`, `svg-fe-spot-light`, `svg-fe-image`, `svg-fe-merge`, `svg-fe-merge-node` | `Renders_reusable_symbols_patterns_markers_and_filter_primitives`, `Emits_extended_filter_primitives_and_lighting_sources`, `Emits_color_matrix_blend_flood_composite_tile_and_all_channel_functions` |
| Declarative animation and motion references: `svg-animate`, `svg-animate-transform`, `svg-animate-motion`, `svg-set`, `svg-mpath` | `Emits_explicit_svg_animation_and_transform_nodes_from_gui_model` |
| Serialization and safety boundaries | `Escapes_xml_attribute_values_and_preserves_non_ascii_text`, `Rendering_the_same_scene_twice_produces_identical_bytes`, `Rejects_non_finite_or_malformed_numeric_geometry_properties`, `Rejects_blank_required_reference_properties`, `Rejects_duplicate_node_ids_before_emitting_ambiguous_svg_references`, `Rejects_duplicate_ids_for_any_enabled_coordinate_overlay`, `Rejects_excessive_tree_depth_before_recursive_svg_serialization`, `Rejects_non_positive_surface_dimensions` |

The two focused vocabulary tests were added because the prior suite did not explicitly mention radial gradients, switch/title nodes, color-matrix/blend/flood/composite/tile filters, or the green/blue/alpha component-transfer functions. Future changes to the switch mapping or the documented vocabulary must update this table and the corresponding assertions. The external-reference policy remains intentionally **not** an allow-list: serialization tests are not security-sanitizer tests.

## Gallery-to-test map

The generated gallery is a runnable integration example, not evidence that every SVG viewer supports every feature. Use this map to trace visible constructions to the sample source and representative serializer tests.

| Gallery construction | Sample source node / feature | Representative test |
|---|---|---|
| Background, panel, curve, facet | `background`, `panel`, `curve`, `facet`; rect/path/polygon and presentation attributes | `Renders_geometry_and_transform_properties_for_composed_scenes` |
| Gradient-backed panel | `workshop-gradient`, `gradient-start`, `gradient-end`; `svg-defs` / `svg-linear-gradient` / `svg-stop` | `Emits_definitions_gradients_clips_and_masks` |
| Clipped panel | `panel-clip`; `svg-clip-path` | `Emits_definitions_gradients_clips_and_masks` |
| Circuit pattern swatch | `circuit-pattern`, `pattern-swatch`; `svg-pattern` and pattern URL | `Renders_reusable_symbols_patterns_markers_and_filter_primitives` |
| Arrow on the curve | `arrowhead`; `svg-marker` and `marker-end` | `Renders_reusable_symbols_patterns_markers_and_filter_primitives` |
| Glow, displacement, and lighting swatches | `soft-glow`, `surface-relief`, `surface-lighting`; filter primitives | `Emits_extended_filter_primitives_and_lighting_sources` |
| Reused star | `star-symbol`, `reused-star`; `svg-symbol` / `svg-use` | `Renders_reusable_symbols_patterns_markers_and_filter_primitives` |
| Pulsing marker | `animated-marker`; `svg-animate` and `svg-set` | `Emits_explicit_svg_animation_and_transform_nodes_from_gui_model` |
| Moving scout | `orbit-path`, `motion-scout`, `scout-motion`, `scout-route-reference`; `svg-animate-motion` / `svg-mpath` | `Emits_explicit_svg_animation_and_transform_nodes_from_gui_model` |
| Linked heading and caption | `heading-link`, `heading`, `caption`; `svg-a` and `svg-text` | `Emits_navigable_svg_links_and_semantic_metadata_nodes` and `Escapes_xml_attribute_values_and_preserves_non_ascii_text` |

The sample is maintained in `samples/SvgGallery/Program.cs`; the checked-in image is `docs/assets/svg-adapter-gallery.svg`. CI regenerates the image and checks it byte-for-byte, so the image should not be edited independently. The named tests are representative coverage, not a claim that the gallery exercises the entire vocabulary table above.
