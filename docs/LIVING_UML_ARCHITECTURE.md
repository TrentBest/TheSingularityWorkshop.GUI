# Living UML and Per-Type Documentation Architecture

> If a picture is worth a thousand words… then what is a living picture worth?

## Purpose

UML is a developer on-ramp to the Workshop stack. A developer should be able to point the tooling at a real codebase, generate accurate diagrams for an article or documentation page, embed those diagrams, and keep the published documentation synchronized as the code evolves.

This is not a general-purpose drawing editor and not a collection of manually maintained pictures. Source and verified behavior are evidence; the diagram model is a derived, inspectable representation; SVG and Markdown are generated manifestations.

## Authority and pipeline

Use a staged pipeline with explicit provenance:

1. **Source analysis (Roslyn).** Parse every included source file, preserve declaration identity and source locations, and report syntax errors. Use a project/solution compilation when available so partial declarations, aliases, generic types, nested types, references, inheritance, interface realization, and member types can be resolved semantically.
2. **Compiled-domain inspection (reflection).** After a successful build, inspect the compiled assemblies to verify the public/runtime type surface, attributes, generic constraints and members as exposed by the target assembly. Reflection supplements source analysis; it does not replace it because it loses source locations, comments, some syntax-level intent, and cannot represent uncompiled edits. Record assembly identity, target framework, build/configuration, and source revision.
3. **Canonical documentation model.** Normalize source and compiled facts into a versioned, serializable model. Although a folder may be named `docs/AST`, serialized files in it should be described precisely as *documentation model snapshots* or *source-analysis snapshots* unless they actually preserve Roslyn syntax trees. Do not conflate a Roslyn AST with reflection metadata.
4. **Diagram model.** Project the canonical model into renderer-neutral UML structures. Keep semantic identity, relationship kind, multiplicity, ownership evidence, provenance, and source links separate from SVG layout.
5. **Rendering.** GUI.SVG renders class cards and relationship connectors from the diagram model. Other renderers may consume the same model. SVG is generated output and must never be the only editable or authoritative representation.
6. **Documentation composition.** Generate one Markdown/API documentation page per maintained type, including its UML class view, summary, member inventory, source links, and links to applicable interaction/state/architecture diagrams. Articles can embed the same generated SVG or use a stable generated include/reference.
7. **Verification and publication.** Regenerate deterministically; validate accessibility, bounds, links, inventory coverage, model schema, and SVG structure; compare generated output with committed artifacts; fail CI when output is stale or evidence is missing.

## What is generated for every type

For each included maintained class, struct, interface, record, enum, and delegate, generate a stable inventory entry and a per-type documentation page. Each type gets a focused UML class view where the type kind supports one. A page may include nested types or link to their own pages, but identity must remain unambiguous across namespaces, containing types, generic arity, and overloads.

A per-type class view should show, where applicable:

- Type kind, visibility, modifiers, generic parameters and constraints.
- Fields, properties, events, constructors, methods, parameters, return types, and relevant static/abstract/virtual/override information.
- Inheritance and interface realization as correctly styled connectors.
- Type dependencies and associations derived from resolved member types, with relationship semantics clearly distinguished from mere textual references.
- Multiplicity, aggregation, or composition only when supported by evidence; do not infer ownership solely because a field references another type.
- Source file/line links and provenance, plus explicit unresolved/external symbols where applicable.

Coverage means every maintained declaration is accounted for, not that every possible UML diagram family is fabricated for every class.

## Other UML views are evidence-driven

Generate additional diagram families when their subjects exist:

- **Sequence/communication:** an actual scenario, call trace, or documented interaction with named participants and ordered messages.
- **State machine:** defined states, events/guards, and transitions grounded in the implementation or an explicitly authored state model.
- **Activity:** a real workflow or algorithm.
- **Component/package:** module boundaries and dependency relationships.
- **Deployment:** actual runtime nodes and deployment topology.
- **Use case:** identified actors and goals.

Where a view is authored from a scenario rather than mechanically inferred from code, label the authored assumptions and connect claims to source, tests, traces, or other evidence. Runtime traces can animate/highlight responsibility flow, but a decorative timeline must not be represented as proof of live execution.

## Keeping documentation fresh

SVG cannot update itself after its source changes. The build/documentation workflow must regenerate it.

- A local command should support generating and verifying the documentation model, per-type pages, and SVGs.
- A post-build step may use reflection against the exact built assemblies and merge its results with the source model.
- CI should regenerate all outputs from the checked-out revision and fail on stale committed artifacts.
- Generated artifacts should include a source revision/model version and deterministic provenance metadata where useful; avoid embedding volatile timestamps that break reproducibility.
- The tool should report missing, stale, orphaned, or hand-edited generated artifacts distinctly.
- Documentation embeds should reference stable relative paths or generated include directives rather than copying SVG markup into many pages. If inline SVG is required for a publishing target, produce it as a build artifact.
- Never overwrite hand-authored source documentation silently. Clearly separate generated regions or generate dedicated files and link them from authored pages.

## Suggested repository layout

```text
docs/
  AST/                         # optional, versioned source-analysis snapshots
  api/
    Namespace.Type.md           # generated per-type pages
  uml/
    Namespace.Type.svg          # generated class views
    scenarios/                  # evidence-based sequence/state/activity views
  articles/
    ...                         # authored articles embedding generated views
  manifest.json                 # schema/version, source revision, inventory and outputs
```

The folder name `AST` is a convention, not a requirement. If it contains normalized JSON rather than actual Roslyn syntax trees, document the format as a normalized source-analysis model. Keep generated data bounded and reviewable; do not commit opaque compiler caches or huge duplicated trees without a demonstrated need.

## Determinism, drift, and safety

- Stable type/member IDs must not depend only on line numbers or declaration order.
- Fully qualified metadata identity should include assembly/project identity, namespace, containing types, generic arity, and member signatures where relevant.
- Sort output by stable identity; use normalized line endings and invariant formatting.
- Fail closed on syntax/compilation errors when semantic correctness is required. Partial syntax-only output may be supported only when clearly labeled and not mistaken for semantically verified UML.
- Never draw a resolved inheritance/association connector from a name string alone when compilation symbols are available.
- Validate XML/SVG, unique IDs, title/description, keyboard/accessibility behavior where interactive, viewBox bounds, text overflow, and link targets. Inspect rendered artifacts; XML validity alone does not prove legibility.
- Test source mutations, additions/removals/renames, nested and partial types, overloaded/generic members, ambiguous simple names, unresolved external symbols, malformed source, failed builds, and stale-output detection.
- Keep runtime GUI Core renderer-neutral. Roslyn/MSBuild and reflection discovery belong in build-time tooling/adapters, not in the core GUI rendering package.
- Do not execute user project build targets or load arbitrary assemblies in a privileged process without an explicit trust boundary. Prefer isolated build processes and make the target framework/configuration explicit.

## Current implementation status

The current `samples/SourceUmlGenerator` staging implementation has two analysis paths. The source-file path parses one C# file with Roslyn syntax APIs, projects declarations into GUI.SVG cards, and emits a syntax-derived class-compartment SVG plus an optional syntax-only model snapshot. The `--project-model-json` path loads one `.csproj` through `MSBuildWorkspace`, obtains a compiler compilation, fails closed on compilation errors, and emits the renderer-neutral model with source spans and compiler-symbol-derived inheritance, interface realization, member associations, and signature dependencies. The optional `--diagram-svg` / `--diagram-type` path renders the selected type's one-hop relationship neighborhood through `SvgUmlDocumentationGraphRenderer`. CI verifies a partial generic type, a resolved generic interface, association/dependency edges, graph SVG accessibility, and local SVG references. Still outstanding are solution-wide inventory/project traversal, exact-build reflection comparison, per-type Markdown generation, full-project graph layout, and comprehensive painted-text/bounds inspection.

This document describes the intended architecture, not a claim that those future stages already exist. Keep the FSM_COS alpha.7 UML gate open until the relevant implementation and exact-commit evidence meet the acceptance criteria.

## Completion criteria

The living UML feature is ready for adoption when a clean checkout can:

1. Analyze a real project/solution and enumerate every in-scope maintained type deterministically.
2. Cross-check source declarations with the exact compiled assembly when a successful build is available.
3. Generate a versioned model, per-type documentation pages, and source-grounded SVG class views.
4. Resolve and render semantic relationships, with unresolved/external references honestly marked.
5. Generate additional diagram families only for evidence-backed scenarios/state/workflows/architecture.
6. Embed diagrams in authored articles without duplicating their source of truth.
7. Prove that source changes change the right model/page/SVG outputs, malformed input fails closed, and stale artifacts fail CI.
8. Pass semantic tests, accessibility and geometry checks, visual inspection, deterministic regeneration, and exact-commit CI verification.
9. Offer a usable developer command and concise documentation that lets a new user try it on their own codebase quickly.

Until those conditions are met, report the implemented slice and remaining gaps precisely; do not describe the feature as complete.


## Contract v1 implementation status — staging only

The source-analysis branch now contains an initial renderer-neutral interchange DTO in `src/Core/UmlDocumentationModel.cs`, including project/build/revision provenance, metadata-based type/member identity slots, source spans, typed relationship kinds and resolution states, relationship evidence, and a separate human-authored use-case record. `UmlDocumentationModelJson` writes camel-case JSON and string-valued kebab-case enums, and rejects unsupported schema versions and unknown JSON properties. Core tests cover round-trip serialization, source provenance/spans, unresolved-edge preservation, and schema rejection.

This is a **staging model foundation**, not a complete living UML pipeline or a supported package promise. Syntax-only mode intentionally asserts no relationships. Project-semantic mode now extracts compiler-symbol-based inheritance/interface-realization, member associations, and signature dependencies; the GUI.SVG adapter renders a focused one-hop neighborhood with typed connectors and accessible labels. The DTOs still need stronger normalization/validation and should not yet be treated as a stable published interchange promise. Solution-wide inventory, exact-build reflection comparison, per-type pages, large-graph layout/routing, and comprehensive visual/bounds evidence remain outstanding. Keep FSM_COS on its interim Mermaid and keep the adoption gate open.

The source-derived workflow exercises syntax-only output, fail-closed fixtures, project-semantic model generation against GUI Core, and a synthetic partial-generic project. It verifies compiler-resolved generic interface realization, association/dependency edges, model-to-SVG graph rendering, SVG accessibility metadata, and local references. This is evidence for the focused graph vertical slice, not proof of complete solution coverage, large-graph layout, exact-build reflection parity, or rendered text bounds.

## Alignment with the shared Workshop documentation standard

This feature follows the shared standard in FSM_COS `DOCUMENTATION_STANDARD.md`: the repository README is the short reader journey; `docs/GALLERY.md` is the visual index; `docs/SOURCE_DERIVED_UML.md` is the practical tooling guide; this document owns the pipeline architecture and acceptance gates. These documents link to one another rather than duplicating the same contract.

The gallery entry is explicitly labelled as staging proof. It distinguishes the single-file syntax view from the compiler-backed project model and focused graph output. No static SVG or green CI run is described as proof of complete project/solution coverage or finished living UML.

### Visual coverage for UML documentation

| Document or artifact | Classification | Evidence / remaining work |
|---|---|---|
| `docs/GALLERY.md` | Useful visual/proof index embedded and source-linked | Identifies the current staging artifact and its limits; add generated artifact previews when canonical outputs exist. |
| `docs/SOURCE_DERIVED_UML.md` | Practical guide to the verified staging vertical slice | Documents both syntax-only and project-semantic modes, the focused graph renderer, reproducible CI fixtures, and remaining adoption gaps. |
| `docs/LIVING_UML_ARCHITECTURE.md` | Visual architecture is useful and tracked | Pipeline is currently expressed as text; replace or accompany it with a source-generated, evidence-matched architecture visual once the canonical model-to-SVG path exists. |
| Generated class-compartment and focused graph SVGs | Implemented staging outputs; project-semantic relationships have CI-backed symbol provenance | CI validates graph connectors, accessibility labels, generic/partial fixtures, and local references; comprehensive bounds/overflow checks and recorded rendered inspection remain open. |

This is a focused audit of the UML feature documents, not a claim that every Markdown document in the repository has completed the shared visual-coverage review. The repository-wide documentation visual audit remains a separate task.
