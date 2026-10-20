# Source-Derived Diagrams

> If a picture is worth a thousand words… then what is a living picture worth?

The source-derived diagram path begins with C# syntax, projects declarations into GUI's renderer-neutral `DataCard` model, and manifests that model through GUI.SVG. GUI Core remains language-agnostic; C# parsing belongs to the separate source-analysis tool.

## First vertical slice

`samples/SourceUmlGenerator` currently projects top-level type declarations (class, struct, interface, enum, and record), nested types, delegates, and declared members into inspectable cards. Namespace identity is shown in type subtitles; this first slice does not yet build a nested namespace containment tree. It validates C# syntax before emitting SVG and verifies generated XML, accessible title/description, and unique card IDs.

```sh
dotnet run --project samples/SourceUmlGenerator/SourceUmlGenerator.csproj -- src/Core/DataCard.cs artifacts/DataCard.source-structure.svg --verify --uml artifacts/DataCard.uml.svg
```

The tool now emits both the general GUI.SVG structure view and a first UML class-compartment view for the first top-level type in the source file. The UML view includes fields, properties, operations, and declared base-list references, with accessible title/description. This is still not a complete semantic UML generator: base-list names are explicitly unresolved, relationships are not yet drawn as UML connectors, output is one class per invocation, and it does not yet generate type pages or support round-trip editing.

## Next milestones

1. Use Roslyn compilations and symbol identity to resolve base types, interfaces, field/property types, and associations across project boundaries.
2. Resolve declarations with Roslyn compilations and render verified inheritance, interface realization, dependency/association, and multiplicity as UML connectors between class nodes.
3. Generate one per-type Markdown page and UML SVG from the same model; verify inventory coverage and deterministic output in CI.
4. Add fixtures that deliberately change a declaration and prove the diagram changes, and fixtures that break the source to prove generation fails closed.
5. Add validated domain commands before offering source-edit affordances. SVG controls alone must never mutate source.

## Architectural boundary

- C# syntax and semantic analysis live in the source-analysis tool, not GUI Core.
- `DataCard` is a renderer-neutral presentation projection, not the authoritative source model.
- GUI.SVG owns the SVG manifestation; the generator owns source parsing and projection.
- Generated SVG is output. Change source or generator, then regenerate—do not hand-edit generated output as if it were authoritative.

See [Data Cards and UML](DATA_CARDS_AND_UML.md) and [Per-Type Documentation and UML Standard](CLASS_DOCUMENTATION_STANDARD.md).
