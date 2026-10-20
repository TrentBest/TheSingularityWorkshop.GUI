# GUI.SVG Demonstrations

A progressive, inspectable gallery of SVG artifacts with a write-up for each demonstration. Each card answers four questions:

- **What does it show?**
- **What does it prove today?**
- **What does it not prove?**
- **What is the next verification step?**

Open the [scrollable demonstration gallery](index.html) in a browser for the card-based view, or read the entries directly here.

## Demonstration sequence

| # | Demonstration | Artifact | Evidence status |
|---|---|---|---|
| 01 | [Primitive geometry](01-primitive-geometry.md) | [Reference SVG](01-primitive-geometry.svg) · [Runnable generator](PrimitiveGeometry/Program.cs) | Generator and XML assertions added; successful execution still needs verification |
| 02 | Coordinate systems and transforms | Planned | Not started |
| 03 | Reusable symbols and repeated entities | Planned | Not started |
| 04 | Declarative SVG animation | Existing gallery reference: [svg-adapter-gallery.svg](../../docs/assets/svg-adapter-gallery.svg) | Existing SVG includes declarative animation; connect to adapter generation and test viewer behavior |
| 05 | State-driven behavior trace | Planned | Requires deterministic state/trace producer |
| 06 | FSM_API-driven fox and sheepdog | Planned | Must be driven by real FSM_API execution, not a parallel animation timeline |
| 07 | GUI.SVG adapter generated from a semantic tree | Planned | Must build and compare generated artifact in a reproducible test |
| 08 | Cross-adapter / host interoperability | Planned | Add only after each participating boundary is independently verified |

## Evidence policy

A visually correct SVG proves only that the SVG can be rendered. It does not by itself prove GUI.SVG generated it, that FSM_API drove its behavior, or that FSM_COS composed the pipeline.

For a demonstration to claim adapter generation, include the C# source, the command used to generate the artifact, a successful test or reproducible output comparison, and the resulting SVG. Demonstration 01 now includes a runnable generator and structural assertions; its successful execution has not yet been independently verified. For behavior claims, include a trace or assertions from the actual state machine execution.

This catalogue is intentionally incremental: a small verified proof is more useful than a spectacular but ungrounded mock-up.
