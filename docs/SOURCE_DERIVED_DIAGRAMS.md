# Source-Derived Diagrams

> If a picture is worth a thousand words… then what is a living picture worth?

The source-derived diagram path begins with C# syntax, projects declarations into GUI's renderer-neutral models, and manifests those models through GUI.SVG. GUI Core remains language-agnostic; C# parsing belongs to the separate source-analysis tool.

The intended end state is **living UML documentation**: inventory every maintained type, generate per-type pages and class views, embed generated diagrams in articles, and regenerate/verify them when source changes. The architecture, provenance rules, reflection complement, documentation model, drift checks, and acceptance criteria are defined in [Living UML Architecture](LIVING_UML_ARCHITECTURE.md).

## First vertical slice

`samples/SourceUmlGenerator` currently projects top-level type declarations (class, struct, interface, enum, and record), nested types, delegates, and declared members into inspectable cards. Namespace identity is shown in type subtitles; this first slice does not yet build a nested namespace containment tree. It validates C# syntax before emitting SVG and verifies generated XML, accessible title/description, and unique card IDs.

```sh
dotnet run --project samples/SourceUmlGenerator/SourceUmlGenerator.csproj -- src/Core/DataCard.cs artifacts/DataCard.source-structure.svg --verify --uml artifacts/DataCard.uml.svg --type DataCard
```

The tool emits a general GUI.SVG structure view and a UML class-compartment view for the selected declaration. The UML view includes fields, properties, operations, and declared base-list references, with accessible title/description. This is still not a complete semantic UML generator: base-list names are unresolved, relationships are not yet drawn as UML connectors, and the output is one class per invocation. `--type DataCard` selects a declaration by its simple name or fully qualified namespace/type path; ambiguous simple names fail closed. It does not yet generate a project-wide type inventory, per-type documentation pages, compiled reflection snapshots, or round-trip editing.

## Next milestones

1. Build a project/solution-level Roslyn compilation and use symbol identity to resolve base types, interfaces, generic types, and member types across project boundaries.
2. Render semantically grounded inheritance, interface realization, dependency/association, and justified multiplicity as UML connectors between class nodes.
3. Generate a deterministic project-wide inventory and one per-type Markdown page plus UML SVG from the same versioned documentation model.
4. Cross-check the source model against reflection from the exact successfully built target assemblies; keep source locations and documentation comments from Roslyn.
5. Add fixtures that change declarations and prove the model/page/SVG changes, plus malformed-source/build-failure fixtures that prove fail-closed behavior.
6. Verify stale/missing/orphaned artifact detection, accessibility, viewBox bounds, text legibility, and deterministic output in CI.
7. Add validated domain commands before offering source-edit affordances. SVG controls alone must never mutate source.

## Architectural boundary

- C# syntax and semantic analysis live in build-time source-analysis tooling, not GUI Core.
- Compiled reflection supplements source analysis; it does not replace it or constitute an AST.
- The canonical documentation model is authoritative for generated pages/diagrams; `DataCard` is a renderer-neutral presentation projection, not the source model.
- GUI.SVG owns SVG manifestation; the generator owns source parsing, semantic analysis, projection, and regeneration.
- Generated SVG is output. Change source/model/rendering inputs, then regenerate—do not hand-edit generated output as if it were authoritative.

See [Data Cards and UML](DATA_CARDS_AND_UML.md), [Per-Type Documentation and UML Standard](CLASS_DOCUMENTATION_STANDARD.md), and [Living UML Architecture](LIVING_UML_ARCHITECTURE.md).
