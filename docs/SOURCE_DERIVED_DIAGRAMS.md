# Source-Derived Diagrams

> If a picture is worth a thousand words… then what is a living picture worth?

The source-derived diagram path begins with C# syntax, projects declarations into GUI's renderer-neutral models, and manifests those models through GUI.SVG. GUI Core remains language-agnostic; C# parsing belongs to the separate source-analysis tool.

The intended end state is **living UML documentation**: inventory every maintained type, generate per-type pages and class views, embed generated diagrams in articles, and regenerate/verify them when source changes. The architecture, provenance rules, reflection complement, documentation model, drift checks, and acceptance criteria are defined in [Living UML Architecture](LIVING_UML_ARCHITECTURE.md).

A complementary direction is **observable diagram data**: a console app or test driver can construct and mutate a model, while a view observes consistent revisions and re-renders. That does not make SVG itself reactive; a host must refresh the displayed SVG when the model changes. See [Observable Diagram Models and Console-Driven Views](OBSERVABLE_DIAGRAM_MODELS.md).

## First vertical slice

`samples/SourceUmlGenerator` currently projects top-level type declarations (class, struct, interface, enum, and record), nested types, delegates, and declared members into inspectable cards. Namespace identity is shown in type subtitles; this first slice does not yet build a nested namespace containment tree. It validates C# syntax before emitting SVG and verifies generated XML, accessible title/description, and unique card IDs.

```sh
dotnet run --project samples/SourceUmlGenerator/SourceUmlGenerator.csproj -- src/Core/DataCard.cs artifacts/DataCard.source-structure.svg --verify --uml artifacts/DataCard.uml.svg --type DataCard
```

The tool emits a provisional syntax-only documentation-model JSON snapshot with explicit repository, revision, and project provenance via `--model-json`, `--repository`, `--revision`, and `--project`. A separate `--project-model-json` mode loads a `.csproj` with `MSBuildWorkspace` and emits compiler-symbol-based type/member identities and relationships into the versioned model. For the focused type, the UML renderer draws typed semantic connectors for the selected type and its one-hop neighborhood; it does not yet lay out and navigate the complete project graph. The deterministic `--inventory-markdown` and `--type-pages-dir` options produce an inventory and one standalone Markdown page per analyzed type, with revision-pinned source evidence. CI verifies model contracts, page coverage/index links, source provenance, malformed-source rejection, type-selection behavior, SVG accessibility and geometry, and safe generated-page pruning. The focused syntax-only mode deliberately labels unresolved base-list references as unresolved rather than presenting them as compiler-confirmed relationships.

This is still a staging vertical slice, not a complete living UML system or a supported installable package. Exact-build reflection reconciliation is started: CI now compares compiler-model type identities against the built Core assembly, while member signatures/attributes/constraints and target-framework-specific surface reconciliation remain open. Solution/multi-project inventory, whole-project graph layout/navigation, comprehensive rendered text/bounds inspection, authored use-case persistence, and broad generated-artifact drift/orphan lifecycle checks also remain open. Round-trip editing, live model subscriptions, and host-driven SVG refresh are also not implemented.

## Next milestones

1. Extend project-semantic analysis to deterministic solution/multi-project inventory, including project references, target frameworks, source roots, diagnostics, and inclusion/exclusion decisions.
2. Reconcile compiler-symbol relationships against reflection from the exact successful build outputs for each target framework.
3. Extend the current focused-type/one-hop connector view into normalized whole-project graph layout and navigation.
4. Add complete rendered-text and connector-bounds checks, not only XML validity, accessibility metadata, and basic geometry assertions.
5. Persist human-authored use-case/scenario records with explicit provenance and separation from generated model content.
6. Expand generated-artifact drift and stale/orphan checks without overwriting authored SVGs or documentation.
7. Define observable snapshot/revision/mutation-notification semantics and prove host-level SVG refresh before claiming live behavior.
8. Add validated domain commands before offering source-edit affordances. SVG controls alone must never mutate source.

## Architectural boundary

- C# syntax and semantic analysis live in build-time source-analysis tooling, not GUI Core.
- Compiled reflection supplements source analysis; it does not replace it or constitute an AST.
- The versioned documentation model is the intended source for generated pages/diagrams; `DataCard` is a renderer-neutral presentation projection, not the source model. Current project-semantic output is JSON and is not yet rendered as a multi-node semantic graph.
- GUI.SVG owns SVG manifestation; the generator owns source parsing, semantic analysis, projection, and regeneration.
- An observable model owns state and revision semantics; a renderer observes snapshots and must not become the authoritative store.
- Generated SVG is output. Change source/model/rendering inputs, then regenerate—do not hand-edit generated output as if it were authoritative.

See [Data Cards and UML](DATA_CARDS_AND_UML.md), [Per-Type Documentation and UML Standard](CLASS_DOCUMENTATION_STANDARD.md), [Living UML Architecture](LIVING_UML_ARCHITECTURE.md), and [Observable Diagram Models](OBSERVABLE_DIAGRAM_MODELS.md).
