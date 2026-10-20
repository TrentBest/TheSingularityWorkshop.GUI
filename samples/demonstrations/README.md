# GUI.SVG Demonstrations and Recipes

Start with the **[Markdown recipe gallery](GALLERY.md)**: short visual entries grouped by the actual package/repository boundary, each with a two-sentence summary and one source or walkthrough link.

## Progressive proof roadmap

### A. GUI Core + GUI.SVG only

| # | Recipe | Focus | Evidence status |
|---|---|---|---|
| 01 | [Primitive geometry](recipes/01-primitive-geometry.md) | Semantic nodes → basic SVG | Corrected geometry and stroke-aware bounds assertions passed in [run 39454324734](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39454324734); generated artifact visually reviewed and panel fits inside the viewBox. The package job passed; a separate WPF test-host crash occurred in a later run and is being tracked independently |
| 02 | [Coordinate systems, functions, and transforms](recipes/02-coordinate-functions.md) | Cartesian plane; base function plus vertical/horizontal translations and reflection | Generator and SVG/XML assertions passed in CI; artifact visually reviewed; legend layout corrected and reverified in run [39452760590](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39452760590) |
| 03 | Reusable symbols and repeated entities | Define once, reuse many times | Planned |
| 04 | [Animated scene](recipes/04-animated-scene.md) | SVG definitions and declarative motion | Existing generated gallery snapshot; regenerate and verify against source |
| 05 | Sequential visual explanation | One step at a time or a left-to-right sequence with the current step highlighted | Planned; use SVG animation only for presentation, not as a claim of live computation |

### B. GUI.SVG + domain/runtime packages

| # | Recipe | Package boundary | Evidence status |
|---|---|---|---|
| 06 | [Projectile trajectory from a recorded Physics run](recipes/03-projectile-trajectory.md) | GUI Core + GUI.SVG + Physics-owned JSON run record | Full workflow passed in [run 39453243758](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39453243758); SVG artifact visually reviewed; all nine plotted markers match the pinned Physics record |
| 07 | State-driven behavior trace | FSM_API + verified presentation path | Planned; requires deterministic runtime state and trace |
| 08 | FSM_API-driven fox and sheepdog | FSM_API + GUI.SVG; Renderer/Visualize separately | Planned; must be driven by real execution, not a parallel animation timeline |
| 09 | UML/data → textual scene → Renderer → Visualize | Actual consuming package set to be verified | Planned; no integration claim until a runnable consumer exists |

### C. Harvested repository galleries

These entries let this gallery collect useful SVGs without taking ownership of their implementation. Each harvested item must link to its source repository and retain the source's explanation; a harvested SVG is not evidence that GUI.SVG generated it or that the pictured domain model drove it.

- [Physics-owned SVGs and experiments](https://github.com/TrentBest/TheSingularityWorkshop.Physics/tree/development/docs)
- [Visualize primitive gallery and GUI.SVG proof output](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/tree/development/samples/PrimitiveGallery)

## Package organization

The gallery is a **curated view over repository-owned examples**, not a new source of truth for every image. GUI.SVG-only examples are created here. Domain examples are harvested from their owning repositories, then promoted to integrated GUI.SVG + domain recipes only when the actual data/API path is verified. Renderer, Visualize, FSM_API, and FSM_COS claims require their own source walkthrough, real package references, generated artifact, and reproducible verification.

## Sequence design

A mathematical or physical concept should be explorable as a sequence rather than a static textbook plate. A lesson may show one frame at a time, or a left-to-right progression with the current step highlighted; the displayed values, equations, and curve must remain tied to the same underlying data. For function transformations, explicitly distinguish vertical translation \(f(x)+C\) from horizontal translation \(f(x+C)\).

## Evidence policy

A visible SVG is not, by itself, proof that GUI.SVG generated it, Physics computed the displayed values, FSM_API drove its behavior, or a Renderer/Visualize pipeline produced it. For generation claims, include the source, exact command, successful run or test, and artifact comparison; for behavior claims, include a trace or assertion from actual runtime execution.

The gallery stays intentionally short. Detailed diagrams, implementation notes, commands, and acceptance criteria belong in each recipe's own Markdown file. The [HTML view](index.html) is a convenience view.
