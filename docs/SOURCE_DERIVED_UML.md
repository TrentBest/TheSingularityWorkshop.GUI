# Source-derived UML tooling

## Purpose and current boundary

The source-derived UML tool is a **build-time documentation tool**, not a GUI Core runtime feature. It has a single-source syntax mode for immediate class/data-card views and a project-semantic mode that loads a C# project with MSBuild/Roslyn, emits the renderer-neutral `UmlDocumentationModel`, and can render a focused relationship neighborhood through GUI.SVG.

The v1 model is a staging interchange contract, not yet a supported package promise. It records repository/revision/project provenance, metadata-based type/member identities, source spans, typed relationship kinds/resolution states, and compiler-symbol provenance. Syntax-only mode intentionally emits no asserted semantic relationships.

## Prerequisites

- .NET 8 SDK.
- A clean checkout of this repository.
- GUI Core and GUI.SVG restored through the generator project references.

The generator uses Roslyn `Microsoft.CodeAnalysis.CSharp` and `Microsoft.CodeAnalysis.CSharp.Workspaces` 4.12.0, `Microsoft.CodeAnalysis.Workspaces.MSBuild` 4.12.0, and `Microsoft.Build.Locator` 1.7.8. Roslyn/MSBuild are build-time dependencies of the sample generator, not dependencies of GUI Core.

## Commands

### Single-file syntax view

From the repository root:

```sh
dotnet run --project samples/SourceUmlGenerator/SourceUmlGenerator.csproj -- path/to/input.cs path/to/structure.svg --verify --uml path/to/type.svg --type Namespace.Type
```

The source file must exist. `--verify` checks that at least one top-level type is present and that the generated structure SVG has a title and description. `--uml` requests a focused class-compartment view; `--type` selects the target type and is valid only with `--uml`. Ambiguous simple-name selection fails closed.

### Project-semantic model and focused graph

```sh
dotnet run --project samples/SourceUmlGenerator/SourceUmlGenerator.csproj --configuration Release -- \
  --project-model-json artifacts/Core.uml-model.json \
  --project src/Core/TheSingularityWorkshop.GUI.Core.csproj \
  --repository TrentBest/TheSingularityWorkshop.GUI \
  --revision <git-sha> \
  --configuration Release \
  --diagram-svg artifacts/DataCard.semantic-uml.svg \
  --diagram-type TheSingularityWorkshop.Workshop.Gui.DataCard
```

The project mode loads one `.csproj` through `MSBuildWorkspace`, requests its compiler `Compilation`, and refuses to emit a model if compiler errors are present. The optional `--diagram-svg` and `--diagram-type` arguments must be supplied together. The type selector accepts a fully qualified namespace/type name (preferred) or a unique metadata name. The graph contains the selected type and its directly connected neighbors; it is not yet a full-project layout.

## Outputs and evidence

- **Structure SVG:** source declarations and members projected as GUI.SVG data cards.
- **Syntax class SVG:** a focused single-class view. Base-list names in this path remain explicitly unresolved text.
- **Syntax-only model:** `--model-json` with explicit repository/revision/project provenance; it emits no semantic relationships.
- **Project-semantic model:** `--project-model-json` emits types, partial declaration spans, members, and compiler-symbol relationships.
- **Relationship extraction:** inheritance and interface realization come from Roslyn type symbols; associations come from field/property/event types; dependencies come from method return/parameter and indexer parameter types. Generic arguments are inspected so application-owned types remain visible even when common `System.*` wrappers are omitted.
- **Focused graph SVG:** `SvgUmlDocumentationGraphRenderer` renders typed connectors for the focus type's direct neighborhood. Inheritance/realization use hollow-triangle markers; dependency/realization edges are dashed as appropriate; external/unresolved targets are labelled rather than assigned fake local IDs.
- **CI fixture:** GUI Core project analysis plus a synthetic partial generic project verifies type aggregation, a compiler-resolved generic interface realization, association/dependency edges, model-to-SVG graph output, accessible title/description, and SVG reference validity.
- **Syntax fixtures:** fully qualified type selection, ambiguous simple-name rejection, source mutation changing output, malformed C# failing closed, and SVG contract-validator failure cases.
- **SVG structural contract:** `scripts/verify_svg_contract.py` checks XML parseability, accessible title/description, finite positive `viewBox` dimensions, duplicate IDs, and broken local fragment/`url(#...)` references. It does not prove painted text fits or replace rendered visual inspection.

SVG is generated output, not the authoritative source. Do not hand-edit generated artifacts as if they were the model or source of truth.

## What remains open

- Load a solution and its in-scope project graph as a deterministic inventory, including explicit policies for project references and target frameworks.
- Cross-check source facts against reflection from the exact successful build output.
- Render and navigate the whole project/solution graph; the current renderer focuses on one type and one-hop neighbors.
- Improve connector routing and large-neighborhood layout; measure actual painted text bounds/overflow and record visual inspection.
- Generate a deterministic inventory and a Markdown/API page for every maintained type, with source links and generated-artifact drift checks.
- Add stronger model normalization/validation and explicit evidence for multiplicity, ownership, aggregation, or composition. A field reference alone is not ownership evidence.
- Complete authored use-case persistence, evidence review, and lifecycle.
- Detect all stale, orphaned, or hand-edited generated pages and artifacts.

## Verification

The staging workflow is `.github/workflows/source-derived-diagrams.yml`. It runs both syntax fixtures and project-semantic model/graph checks. Record the exact candidate SHA and workflow run when reporting results. The source-derived workflow and package workflow must both pass on the same candidate; a later commit needs its own evidence. Package publication remains separate and is not implied by successful CI.

## Intended pipeline

```text
C# source + project/solution
  -> Roslyn syntax + compiler symbols
  -> versioned documentation-model snapshot
  -> renderer-neutral UML relationship graph
  -> focused GUI.SVG manifestation
  -> per-type Markdown pages + inventory + manifest
  -> exact-build reflection cross-check
  -> deterministic regeneration and CI drift/accessibility/geometry checks
```

See [Living UML architecture](LIVING_UML_ARCHITECTURE.md) for the acceptance gates and [Gallery](GALLERY.md) for the current proof artifact. The FSM_COS alpha.7 UML adoption gate stays open until solution-level inventory, generated per-type documentation, reflection cross-check, and complete rendering/visual evidence are verified.
