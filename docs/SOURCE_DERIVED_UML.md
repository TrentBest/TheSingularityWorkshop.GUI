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
  --diagram-type TheSingularityWorkshop.Workshop.Gui.DataCard \
  --inventory-markdown artifacts/Core.uml-inventory.md \
  --type-pages-dir artifacts/Core.uml-types
```

The project mode loads one `.csproj` through `MSBuildWorkspace`, requests its compiler `Compilation`, and refuses to emit a model if compiler errors are present. The optional `--diagram-svg` and `--diagram-type` arguments must be supplied together. The type selector accepts a fully qualified namespace/type name (preferred) or a unique metadata name. The graph contains the selected type and its directly connected neighbors; it is not yet a full-project layout.

## Outputs and evidence

- **Structure SVG:** source declarations and members projected as GUI.SVG data cards.
- **Syntax class SVG:** a focused single-class view. Base-list names in this path remain explicitly unresolved text.
- **Syntax-only model:** `--model-json` with explicit repository/revision/project provenance; it emits no semantic relationships.
- **Project-semantic model:** `--project-model-json` emits types, partial declaration spans, members, and compiler-symbol relationships. Optional `--reflection-assembly` reads the explicitly requested compiled PE file's assembly/type metadata without loading it or its runtime dependencies, checks assembly identity against the compiler model, and compares source-model type metadata identities, failing on a mismatched artifact or missing/unexpected non-generated types. This exact-build metadata gate validates type inventory only; member signatures, attributes, constraints, and target-framework-specific surface reconciliation remain open.
- **Generated type inventory:** `--inventory-markdown` emits deterministic Markdown for every model type, with a sorted index, type identity, members/signatures, source links pinned to the recorded revision, and incoming/outgoing relationship evidence.
- **Standalone type pages and embedded UML SVGs:** `--type-pages-dir` emits one deterministic Markdown page and one focused relationship-graph SVG per type, plus `index.md`. Each page embeds its sibling SVG and includes compiler-model identity, source declarations, member signatures, relationships, and revision-pinned evidence links. The `.source-uml-generated-files` manifest tracks generated Markdown and SVG filenames. Regeneration prunes only manifested filenames matching the generator's stable hashed naming scheme; path traversal and manifests that claim arbitrary authored filenames fail closed. Generated pages/diagrams are artifacts, not hand-edited source.
- **Relationship extraction:** inheritance and interface realization come from Roslyn type symbols; associations come from field/property/event types; dependencies come from method return/parameter and indexer parameter types. Generic arguments are inspected so application-owned types remain visible even when common `System.*` wrappers are omitted.
- **Focused graph SVG:** `SvgUmlDocumentationGraphRenderer` renders typed connectors for the focus type's direct neighborhood. Inheritance/realization use hollow-triangle markers; dependency/realization edges are dashed as appropriate; external/unresolved targets are labelled rather than assigned fake local IDs.
- **CI fixture:** GUI Core project analysis plus a synthetic partial generic project verifies type aggregation, a compiler-resolved generic interface realization, association/dependency edges, model-to-SVG graph output, accessible title/description, and SVG reference validity.
- **Syntax fixtures:** fully qualified type selection, ambiguous simple-name rejection, source mutation changing output, malformed C# failing closed, and SVG contract-validator failure cases.
- **SVG structural contract:** `scripts/verify_svg_contract.py` checks XML parseability, accessible title/description, finite positive `viewBox` dimensions, duplicate IDs, and broken local fragment/`url(#...)` references. It does not prove painted text fits or replace rendered visual inspection.

SVG is generated output, not the authoritative source. Do not hand-edit generated artifacts as if they were the model or source of truth.

## What remains open

- **Implemented and verified on PR #18 code candidate `b5eafdc`:** `.github/workflows/solution-derived-uml.yml` parses the solution manifest, explicitly scopes all four maintained product projects (Core, Blazor, WPF, SVG), rejects unclassified new `src/**/*.csproj` files, builds each in Release, and compares exact compiled assembly identity/type metadata with the Roslyn model. [Exact candidate run #39508725522](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39508725522) passed: Core 32 types, Blazor 6, WPF 22, SVG 7. This is project/type inventory coverage, not yet a merged cross-project relationship graph or per-target-framework matrix.
- **Extend exact-build assembly metadata reconciliation beyond type identity.** The all-project type-inventory gate now passes for Core, Blazor, WPF, and SVG. The source-derived workflow also includes a negative test that rejects the wrong assembly. Compare members/signatures, visibility/modifiers, attributes, generic constraints, and framework-specific API surfaces; add per-target-framework output partitioning before any project multi-targets.
- Render and navigate the whole project/solution graph; the current renderer focuses on one type and one-hop neighbors.
- Improve connector routing and large-neighborhood layout; measure actual painted text bounds/overflow and record visual inspection.
- Add committed generated-artifact drift checks for the inventory, type pages, and embedded SVGs; CI currently validates completeness/order/accessibility and uploads the generated outputs rather than checking them into source control.
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
  -> per-type Markdown pages with embedded UML SVGs + inventory + manifest
  -> exact-build assembly-metadata cross-check
  -> deterministic regeneration and CI drift/accessibility/geometry checks
```

See [Living UML architecture](LIVING_UML_ARCHITECTURE.md) for the acceptance gates and [Gallery](GALLERY.md) for the current proof artifact. The FSM_COS alpha.7 UML adoption gate stays open. All four maintained product projects build and pass exact assembly-metadata/type-inventory reconciliation, and every generated type page embeds an accessible UML SVG. [Solution-derived run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623) and [source-derived run #39509559621](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559621) passed on code candidate `1471b9a`. The follow-up commit `127fe99` adds `docs/uml/README.md` and an authored guide for each of the 37 maintained product `.cs` files. Remaining requirements include committing the generated per-type SVG/page set beside those guides, member-level metadata parity, full graph/navigation, rendered text-bound and visual evidence, and generated-artifact drift checks. This remains a staging tool, not a supported installable package.


## Installable build-time documentation package

The intended consumer experience is an **installable documentation package**, not a runtime library dependency and not a generator that every repository must copy into its own source tree. The current `samples/SourceUmlGenerator` project proves the analysis/rendering path; it is not yet that supported package.

### Consumer contract

A consuming .NET project should be able to add the documentation package and opt into generation with a small, explicit MSBuild property or target. The package should provide:

- A versioned command-line generator and its pinned build-time dependencies.
- An MSBuild `.targets` file, preferably under `buildTransitive/`, with an explicit opt-in switch and configurable output directory.
- A documented local command to regenerate and a verification mode for CI.
- Deterministic per-type Markdown, UML SVG, inventory, and a manifest identifying the source revision, assembly identity, target framework, configuration, and generator/model versions.
- A clear separation between generated output and authored documentation, with drift checks that never silently overwrite hand-written files.

The package must not add Roslyn, MSBuild workspace, reflection discovery, or GUI.SVG dependencies to the consumer's runtime dependency graph. The runtime application should pay **zero documentation-generation cost**: no startup scanning, no reflection walk, no background documentation task, and no extra application execution path. Generation occurs only in the developer/build/release pipeline.

### Build lifecycle

The preferred lifecycle has two coordinated stages:

1. **Build-time source analysis.** Use Roslyn and the project's actual compilation context to resolve declarations and source locations. This stage should not claim that source syntax alone proves the final assembly surface.
2. **Post-build verification and generation.** After a successful build, inspect the exact target assembly produced for the current target framework/configuration. Reconcile reflected types and members with the source model, then generate the documentation outputs. If the build failed, the target assembly is missing, or the reflection comparison finds an unexplained mismatch, fail the documentation step rather than silently publish a misleading result.

The post-build target must not invoke a second build of the same project or create a target recursion. Multi-targeted projects need one isolated generation pass per target framework, with outputs partitioned by framework or a deterministic merge policy. Referenced assemblies must be resolved from the actual build output/reference context rather than whichever same-named assembly happens to be loadable in the generator process.

Reflection is evidence about the compiled artifact, not a replacement for Roslyn: reflection cannot recover comments and source spans and cannot describe edits that were not compiled. Conversely, source analysis alone can describe code that never made it into the successful artifact. The comparison is valuable precisely because the two evidence sources have different blind spots.

### Package safety and adoption gates

- Keep the package opt-in; installing it must not unexpectedly change normal builds or release artifacts.
- Do not execute arbitrary project targets or load arbitrary assemblies in a privileged process without an explicit trust boundary. Prefer an isolated generator process and explicit assembly paths.
- Define behavior for trimming, single-file deployment, generated source, source-link paths, symbols, and multi-target frameworks before claiming broad support.
- Keep package versioning and schema compatibility explicit. Do not promise the current staging DTO as a stable public contract until normalization and compatibility tests are complete.
- CI should test both enabled and disabled modes, failed builds, missing assemblies, multiple target frameworks, deterministic output, stale generated files, and reflection/source mismatches.
- The release workflow should generate and verify documentation before packaging/publishing; publishing remains governed by the repository's separate explicit release authorization.

This package/lifecycle is the target architecture, not an implemented feature yet. Current proof remains the staging CLI and focused graph renderer. Project-semantic generation of deterministic per-type Markdown pages and an inventory is implemented and CI-checked for the analyzed Core project. The remaining gates above still apply: solution-wide inventory, repository-wide maintained-type coverage, member-level exact-build reflection reconciliation, committed generated-artifact drift checks, and visual/bounds evidence.


## Distinct SVG contracts

The generator emits multiple SVGs for different audiences; they are not interchangeable canonical files:

- `DataCard.source-structure.svg` is the source-declaration structure view.
- `DataCard.uml.svg` is the focused syntax-only class view for the selected source type.
- `DataCard.semantic-uml.svg` is the compiler-semantic neighborhood graph, with resolved type relationships.
- `docs/assets/uml/classes/Core/DataCard.svg` is the authored ecosystem diagram. It includes the class in its documented dependency/consumer context and is maintained as a separate living-documentation artifact.

CI validates each generated view's own contract and the authored ecosystem diagram's required identity/context. It must not require byte equality between the focused syntax class view and the authored ecosystem diagram. If a generator is intentionally promoted to regenerate the ecosystem diagram, that needs an explicit layout/contract migration and reviewed output update—not a silent overwrite.
