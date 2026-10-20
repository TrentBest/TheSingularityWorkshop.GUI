# UML Documentation Visual Audit

**Scope:** source-derived UML tooling documentation and generated UML artifacts only. This is not the repository-wide visual audit.

The shared FSM_COS documentation standard requires every Markdown document to be considered for a useful source-verified visual, a tracked missing visual, or a reason no visual is needed. The UML feature scope is recorded here so the implementation and its documentation move together.

| Item | Classification | Status and evidence |
|---|---|---|
| `README.md` | Existing visual plus navigation to the Gallery | The opening architecture flowchart is authored Mermaid and is conceptual; it is not source-generated UML. The README links to `docs/GALLERY.md` and the focused UML guides. |
| `docs/GALLERY.md` | Visual/proof index | Describes the current syntax-derived staging proof and links to reproduction and architecture docs. Does not yet embed a canonical semantic graph artifact. |
| `docs/SOURCE_DERIVED_UML.md` | Useful visual tracked as needed | The text pipeline explains the stages; a real project-level generated diagram is still needed. |
| `docs/LIVING_UML_ARCHITECTURE.md` | Architecture visual needed and tracked | Pipeline is text-based today. Generate an evidence-backed view from the same canonical model once the semantic pipeline exists. |
| `docs/DATA_CARDS_AND_UML.md` | Domain explanation; source-grounded visual is useful and tracked | Existing data-card vocabulary must be distinguished from source parsing and semantic UML. Add a generated artifact when the end-to-end source model exists. |
| `samples/SourceUmlGenerator/Program.cs` output | Implemented syntax-derived SVG; not semantically authoritative | Fixture tests cover source mutation, type selection, ambiguous names, and malformed C#. Semantic relationships, comprehensive bounds checks, and rendered inspection remain open. |

## Required update discipline

- Reclassify each row when its source, visual, or evidence changes.
- Record exact source revision and workflow run for generated artifacts.
- Verify SVG/XML structure, accessible title/description, stable IDs, viewBox bounds, text overflow, and local links.
- Inspect the rendered SVG at ordinary GitHub width and in light/dark contexts; XML validity alone is insufficient.
- Keep syntax-only references explicitly unresolved; never style them as compiler-verified connectors.
- Add every new UML-specific Markdown guide or generated artifact to this audit.
- Do not mark the FSM_COS adoption gate closed until the semantic model, renderer, per-type inventory, drift checks, and visual evidence pass on the exact candidate.

