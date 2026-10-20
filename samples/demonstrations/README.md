# GUI.SVG Demonstrations and Recipes

Start with the **[Markdown recipe gallery](GALLERY.md)**: each entry has a preview, a two-sentence summary, and one link to its full walkthrough.

## Progressive proof roadmap

| # | Recipe | Package boundary | Evidence status |
|---|---|---|---|
| 01 | [Primitive geometry](recipes/01-primitive-geometry.md) | GUI Core + GUI.SVG | Generator and XML assertions exist; successful execution still needs to be recorded |
| 02 | Coordinate systems and transforms | GUI Core + GUI.SVG | Planned |
| 03 | Reusable symbols and repeated entities | GUI Core + GUI.SVG | Planned |
| 04 | [Animated scene](recipes/04-animated-scene.md) | GUI Core + GUI.SVG | Existing generated gallery snapshot; regenerate and verify against source |
| 05 | State-driven behavior trace | FSM_API + verified presentation path | Planned; requires deterministic runtime state and trace |
| 06 | FSM_API-driven fox and sheepdog | FSM_API + GUI.SVG, then Renderer/Visualize separately | Planned; must be driven by real execution, not a parallel animation timeline |
| 07 | Semantic tree to adapter output | GUI Core + relevant adapter | Planned; build and compare generated output |
| 08 | UML/data → textual scene → Renderer → Visualize | Actual consuming package set to be verified | Planned; no integration claim until a runnable consumer exists |
| 09 | Cross-adapter / host interoperability | Each participating package boundary | Planned; verify each boundary independently |

## Package organization

Recipes are organized by **actual package consumption**, not by what a picture resembles. The current examples use GUI Core and GUI.SVG; Renderer, Visualize, FSM_API, and FSM_COS claims require their own source walkthrough, real package references, generated artifact, and reproducible verification.

## Evidence policy

A visible SVG is not, by itself, proof that GUI.SVG generated it, FSM_API drove its behavior, or a Renderer/Visualize pipeline produced it. For generation claims, include the source, exact command, successful run or test, and artifact comparison; for behavior claims, include a trace or assertion from actual runtime execution.

The gallery stays intentionally short. Detailed diagrams, implementation notes, commands, and acceptance criteria belong in each recipe's own Markdown file. The [HTML view](index.html) is a convenience view.
