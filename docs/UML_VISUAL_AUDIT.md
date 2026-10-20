# UML Documentation Visual Audit

**Scope:** source-derived UML tooling documentation and generated UML artifacts only. This is not the repository-wide visual audit.

The shared FSM_COS documentation standard requires every Markdown document to be considered for a useful source-verified visual, a tracked missing visual, or a reason no visual is needed. The UML feature scope is recorded here so the implementation and its documentation move together.

| Item | Classification | Status and evidence |
|---|---|---|
| `README.md` | Existing visual plus navigation to the Gallery | The opening architecture flowchart is authored Mermaid and is conceptual; it is not source-generated UML. The README links to `docs/GALLERY.md` and the focused UML guides. |
| `docs/GALLERY.md` | Visual/proof index | Describes the syntax-only view and the CI-verified project-semantic model/focused graph slice; a committed semantic graph preview is still outstanding. |
| `docs/SOURCE_DERIVED_UML.md` | Useful visual tracked as needed | Documents syntax-only and project-semantic commands plus focused graph SVG output. The guide still needs a committed, inspected graph preview and evidence of text-bound/visual-layout checks. |
| `docs/LIVING_UML_ARCHITECTURE.md` | Architecture visual needed and tracked | Pipeline is text-based today. The model-to-focused-SVG vertical slice exists; generate and inspect a source-derived architecture-level view from the canonical model once the graph layout supports that scope. |
| `docs/DATA_CARDS_AND_UML.md` | Domain explanation; source-grounded visual is useful and tracked | Existing data-card vocabulary must be distinguished from source parsing and semantic UML. Add a generated artifact when the end-to-end semantic source model exists. |
| `samples/SourceUmlGenerator/Program.cs` output | Syntax-derived SVG, project-semantic model, and focused semantic graph SVG | CI covers source mutation, type selection, ambiguous names, malformed C#, partial generic type aggregation, compiler-resolved interface realization, member associations/signature dependencies, focused graph output, accessibility metadata, and SVG local-reference integrity. Whole-project graph layout, per-type inventory/pages, painted text bounds, and recorded rendered inspection remain open. |

## Required update discipline

- Reclassify each row when its source, visual, or evidence changes.
- Record exact source revision and workflow run for generated artifacts.
- Run `python3 scripts/verify_svg_contract.py <svg...>` for XML structure, accessible title/description, finite positive viewBox dimensions, duplicate IDs, and local-reference integrity. This is a structural guard, not a proof that painted text fits within the viewBox.
- Verify text overflow and visual composition by inspecting rendered SVGs at ordinary GitHub width and in light/dark contexts; retain evidence for the exact candidate SHA.
- Inspect the rendered SVG at ordinary GitHub width and in light/dark contexts; XML validity alone is insufficient.
- Keep syntax-only references explicitly unresolved; never style them as compiler-verified connectors.
- Add every new UML-specific Markdown guide or generated artifact to this audit.
- Do not mark the FSM_COS adoption gate closed until the semantic model, renderer, per-type inventory, drift checks, and visual evidence pass on the exact candidate.

