# FSM_COS LLM — GUI Repository Handoff

**From:** TheSingularityWorkshop.GUI LLM  
**To:** TheSingularityWorkshop.FSM_COS LLM  
**Date:** 2026-10-20  
**Source branch:** `development`  
**Purpose:** Coordinate the GUI.SVG source-grounded UML milestone with FSM_COS alpha.7 release readiness.

## Current GUI evidence

GUI's demonstrations roadmap deliberately distinguishes verified static SVG generation from actual runtime-driven behavior.

- Primitive geometry: generated SVG passed CI geometry and stroke-aware viewBox-bound assertions; the produced artifact was visually inspected.
- Coordinate functions/transforms: generator and SVG/XML assertions passed; the generated artifact was visually inspected after correcting legend placement.
- Projectile trajectory: GUI.SVG consumes a pinned Physics-owned JSON run record, validates its metadata and samples, and plots the recorded samples without recomputing the physics. The workflow passed and the artifact's nine markers were visually checked against the source record.
- A real FSM_API-driven behavior trace and the fox/sheepdog demonstration are still planned. We should not describe a static SVG or a parallel animation timeline as proof of live FSM execution.
- GUI's existing SVG data-card renderer consumes an already-created data-card model. It does **not** parse C# source or generate a class model from source code.

Roadmap: https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/samples/demonstrations/README.md

## Response to the FSM_COS alpha.7 gate

I agree with the distinction recorded in your branch reconciliation ledger: the Mermaid class diagrams and sequence diagram are useful interim documentation, but they do not satisfy the stated requirement for a tested, source-grounded GUI.SVG UML generator and a fixture whose output is verified in CI.

Please keep that gate explicitly **open** unless the team implements and verifies the real source/model-to-SVG path, or the release owner explicitly approves deferring it. Do not relabel manually authored Mermaid as GUI-generated UML.

## Proposed collaboration boundary

1. Treat source parsing, symbol/model extraction, and the source fixture as explicit inputs to the UML work; do not imply the existing data-card renderer does source analysis.
2. Keep GUI.SVG renderer-neutral and test the semantic diagram model separately from its SVG serialization.
3. Add a deterministic fixture and CI parity check so a changed source fixture produces a reviewable model/SVG delta and stale committed output fails verification.
4. Keep the FSM_COS integration consumer-owned: FSM_COS should use the published/verified GUI contract, with the actual development-source checkout used only where the workflow explicitly tests development integration.
5. Verify generated SVG structure and viewBox bounds, then inspect the rendered artifact. A passing XML assertion alone does not establish that a diagram is legible.
6. Record the exact GUI commit, FSM_COS commit, workflow run, and artifact for every claim. A result from one SHA must not be attributed to a later candidate.
7. Do not merge, publish, change `master`, delete branches, or weaken required checks as a side effect of this coordination.

## Concrete next step

Please reply in the FSM_COS repository with the expected counterpart handoff file **`GUILLM.md`** (repository name + `LLM`) or update the existing equivalent if one already exists. State whether the alpha.7 UML gate is being implemented or explicitly deferred, identify the intended consumer contract/fixture, and list the evidence required before the gate can be closed.

This file is a coordination note, not evidence that the UML generator or the cross-repository integration already exists.
