# Source-derived UML tooling

## Purpose and current boundary

The source-derived UML tool is a **build-time documentation tool**, not a GUI Core runtime feature. It currently accepts one C# source file, parses it with Roslyn syntax APIs, projects declarations to GUI Core data-card nodes, and renders SVG through GUI.SVG. Its class view is syntax-derived: base-list names are displayed as unresolved text, not asserted as semantically resolved inheritance or interface connectors.

The canonical interchange model in `src/Core/UmlDocumentationModel.cs` is a provisional, renderer-neutral DTO contract. The current generator does **not** emit that model yet. A schema DTO and serialization tests alone do not establish an end-to-end canonical pipeline.

## Prerequisites

- .NET 8 SDK.
- A clean checkout of this repository.
- The source file being analyzed.
- GUI Core and GUI.SVG projects restored through the generator project references.

The current project references Roslyn `Microsoft.CodeAnalysis.CSharp` 4.12.0. It does not yet load an MSBuild project/solution or compiler-resolve symbols.

## Current command shape

From the repository root:

```sh
dotnet run --project samples/SourceUmlGenerator/SourceUmlGenerator.csproj -- path/to/input.cs path/to/structure.svg --verify --uml path/to/type.svg --type Namespace.Type
```

The source file must exist. `--verify` checks that at least one top-level type is present and that the generated structure SVG has a title and description. `--uml` requests a focused class-compartment view; `--type` selects the target type and is valid only with `--uml`. A simple name that matches more than one declaration fails closed; use the declared namespace-qualified name where necessary.

This is a **source-shaped command reference**, not a claim that every arbitrary consumer project has been tested. Verify the command against the current checkout before relying on its output.

## Outputs and evidence

- **Structure SVG:** declarations and members projected as data cards.
- **Focused class SVG:** fields, properties, operations, and explicitly unresolved declared type references.
- **Workflow fixtures:** selection in a multi-type source file, ambiguous selection rejection, source mutation changing generated output, and malformed C# failing closed without emitting diagram output.
- **SVG structural contract:** `scripts/verify_svg_contract.py` checks XML parseability, non-empty accessible title/description, finite positive `viewBox` dimensions, duplicate IDs, and broken local fragment/`url(#...)` references for generated workflow artifacts. It deliberately does not claim to measure rendered text overflow or replace visual inspection.
- **Provisional documentation model:** tested JSON contract types, but not wired into generator output.

SVG output is a generated manifestation. The source and future canonical model remain the evidence-bearing inputs; do not hand-edit generated files as if they were authoritative.

## What the tool does not do yet

- Load a `.csproj` or `.sln` into a compiler-backed semantic compilation.
- Resolve symbols across partial/nested/generic declarations and project references.
- Inspect the exact built assembly with reflection and compare its type surface.
- Emit a canonical model snapshot from source and compilation.
- Render a multi-type graph with semantically resolved, typed connectors.
- Generate deterministic inventory coverage and a Markdown page for every maintained type.
- Provide complete authored use-case persistence, evidence review, and lifecycle.
- Measure all painted text bounds/overflows or provide recorded visual inspection for every diagram. The current SVG contract script is a structural check, not a visual-layout proof.
- Detect all stale, orphaned, or hand-edited generated pages and artifacts.

## Verification

The staging workflow is `.github/workflows/source-derived-diagrams.yml`. It contains syntax-tool fixtures, not proof of semantic relationship correctness. Record the exact candidate SHA and workflow run whenever reporting a result. The source-derived workflow and package workflow must both pass on the same candidate; a later commit needs its own evidence. Package publication remains separate and is not implied by these commands.

## Intended pipeline

```text
C# source + project/solution
  -> Roslyn syntax + compiler symbols
  -> optional exact-build reflection cross-check
  -> versioned documentation-model snapshot
  -> renderer-neutral UML diagram graph
  -> GUI.SVG manifestation
  -> per-type Markdown pages + inventory + manifest
  -> deterministic regeneration and CI drift/geometry/accessibility checks
```

See [Living UML architecture](LIVING_UML_ARCHITECTURE.md) for the contract and acceptance gates, and [Gallery](GALLERY.md) for proof artifacts. The FSM_COS alpha.7 UML adoption gate stays open until the semantic and documentation-composition stages are implemented and verified.
