# GUI.SVG Demonstrations and Recipes

Start with the **[Markdown-first recipe gallery](GALLERY.md)**. Each entry embeds its SVG preview, explains what it demonstrates, names the packages actually consumed, and links to a deeper implementation recipe.

The [scrollable HTML view](index.html) is a convenience view; the Markdown gallery is the canonical guide.

## Progressive proof roadmap

| # | Recipe | Package boundary | Evidence status |
|---|---|---|---|
| 01 | [Primitive geometry](GALLERY.md#recipe-01--primitive-geometry) · [implementation deep dive](recipes/01-primitive-geometry.md) | GUI Core + GUI.SVG | Generator and XML assertions exist; successful execution still needs to be recorded |
| 02 | Coordinate systems and transforms | GUI Core + GUI.SVG | Planned |
| 03 | Reusable symbols and repeated entities | GUI Core + GUI.SVG | Planned |
| 04 | [Animated scene](GALLERY.md#recipe-04--animated-scene) | GUI Core + GUI.SVG | Existing generated gallery snapshot; declarative SVG animation is not an FSM_API clock |
| 05 | State-driven behavior trace | FSM_API + a verified presentation path | Planned; requires a deterministic state/trace producer |
| 06 | FSM_API-driven fox and sheepdog | FSM_API + GUI.SVG, then Renderer/Visualize as separate integration recipes | Planned; must be driven by real execution, not a parallel animation timeline |
| 07 | Semantic tree to adapter output | GUI Core + relevant adapter | Planned; build and compare generated output |
| 08 | UML/data → textual scene → Renderer → Visualize | Exact consuming package set to be verified from source | Planned; do not claim integration before a runnable consumer exists |
| 09 | Cross-adapter / host interoperability | Each participating package boundary | Planned; add only after each boundary is independently verified |

## Package organization

The gallery groups recipes by **actual package consumption**, not by the package a picture resembles. Recipe 01 currently references the repository's Core and SVG source projects. Renderer, Visualize, FSM_API, and FSM_COS integration claims require their own real project references, source walkthrough, generated artifact, and reproducible verification.

## Evidence policy

A visually correct SVG proves only that the SVG can be rendered. It does not by itself prove GUI.SVG generated it, that FSM_API drove its behavior, or that a Renderer/Visualize pipeline produced it.

For adapter-generation claims, include the C# source, exact command, successful test or reproducible output comparison, and generated SVG. Keep reference artwork distinct from generated output. For behavior claims, include a trace or assertions from actual runtime execution.

A small verified recipe is more valuable than a spectacular but ungrounded mock-up.
