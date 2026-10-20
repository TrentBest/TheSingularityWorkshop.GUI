# Source-Derived Diagrams

> If a picture is worth a thousand words… then what is a living picture worth?

The source-derived diagram path begins with C# syntax, projects declarations into GUI's renderer-neutral `DataCard` model, and manifests that model through GUI.SVG. GUI Core remains language-agnostic; C# parsing belongs to the separate source-analysis tool.

## First vertical slice

`samples/SourceUmlGenerator` currently projects source-file namespaces, types (class, struct, interface, enum, and record), nested types, delegates, and declared members into stable, inspectable cards. It validates C# syntax before emitting SVG and verifies generated XML, accessible title/description, and unique card IDs.

```sh
dotnet run --project samples/SourceUmlGenerator/SourceUmlGenerator.csproj --   src/Core/DataCard.cs artifacts/DataCard.source-structure.svg --verify
```

This first slice is deliberately **source-derived structure**, not yet a complete UML class-diagram generator. It does not resolve symbols across projects, infer associations from semantic types, guarantee stable IDs across arbitrary source edits, generate type pages, or support round-trip editing. Those are explicit next bridges, not implied capabilities.

## Next milestones

1. Use Roslyn compilations and symbol identity to resolve base types, interfaces, field/property types, and associations across project boundaries.
2. Add a dedicated UML class-diagram model and UML-specific GUI.SVG renderer (inheritance, realization, dependency/association, multiplicity, member compartments).
3. Generate per-type Markdown pages and diagrams from one model; verify inventory coverage and deterministic output in CI.
4. Add fixtures that deliberately change a declaration and prove the diagram changes, and fixtures that break the source to prove generation fails closed.
5. Add validated domain commands before offering source-edit affordances. SVG controls alone must never mutate source.

## Architectural boundary

- C# syntax and semantic analysis live in the source-analysis tool, not GUI Core.
- `DataCard` is a renderer-neutral presentation projection, not the authoritative source model.
- GUI.SVG owns the SVG manifestation; the generator owns source parsing and projection.
- Generated SVG is output. Change source or generator, then regenerate—do not hand-edit generated output as if it were authoritative.

See [Data Cards and UML](DATA_CARDS_AND_UML.md) and [Per-Type Documentation and UML Standard](CLASS_DOCUMENTATION_STANDARD.md).
