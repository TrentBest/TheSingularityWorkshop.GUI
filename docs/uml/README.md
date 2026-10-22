# UML Documentation Index

This directory separates generated source-file/type documentation from authored guides and use cases.

## Current state

- [Source-files directory contract](source-files/README.md) explains the generated-file boundary.
- [Source-derived UML tooling](../SOURCE_DERIVED_UML.md) documents the build-time generator and its verified staging behavior.
- [Living UML architecture](../LIVING_UML_ARCHITECTURE.md) defines the normalized model, provenance, renderer-neutral diagrams, and adoption gates.
- [UML visual audit](../UML_VISUAL_AUDIT.md) tracks the visual-evidence gaps.

The keeper tree contains a small, manually authored first vertical slice under `docs/api/` and focused authored diagrams such as `docs/uml/GuiBuilderUML.md`. The complete source-generated per-type Markdown/SVG set is still produced as workflow artifacts rather than committed and freshness-checked in the keeper tree. Workflow artifacts prove generation only for their exact source commit; they do not satisfy the full per-type coverage gate.

## Ownership rules

- Keep generated pages and SVGs in the generated area, with a manifest that limits pruning to known generated paths.
- Keep human-authored use cases, architecture, rationale, and acceptance decisions outside generated files.
- Never copy generated pages from a stale branch tip when they can be regenerated from the current source.
- Preserve stable type identity, source spans, repository revision, relationship provenance, and explicit unresolved/external states.
- Do not treat a folder named AST as containing Roslyn syntax trees unless it actually preserves those trees; normalized model snapshots must be named accurately.
