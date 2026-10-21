![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# Living UML and per-type documentation architecture

> If a picture is worth a thousand words, what is a living picture worth?

## Purpose

UML is a developer on-ramp to the Workshop stack. The goal is to point the tooling at a real codebase, generate accurate diagrams for articles and documentation pages, and keep those manifestations synchronized as code evolves. This is not a general-purpose drawing editor or a collection of manually maintained pictures: source and verified behavior are evidence, the normalized diagram model is an inspectable representation, and SVG/Markdown are generated manifestations.

## Authority and pipeline

1. **Source analysis (Roslyn):** parse included files, preserve declaration identity and source spans, and report syntax errors. Prefer project/solution compilation so partial declarations, generic types, inheritance, interface realization, and member types can be resolved semantically.
2. **Compiled-domain inspection (reflection):** after a successful build, inspect the exact target assembly. Record assembly identity, target framework, build configuration, and source revision. Reflection supplements source analysis; it does not replace source spans/comments or describe uncompiled edits.
3. **Canonical documentation model:** normalize source and compiled facts into a versioned model. A directory named AST must not imply the files are actual Roslyn syntax trees if they are normalized snapshots.
4. **Renderer-neutral diagram model:** project canonical facts into UML structures with stable identities, relationship kinds, multiplicity where evidenced, ownership evidence, provenance, and source links separate from SVG layout.
5. **Rendering:** GUI.SVG renders cards and relationship connectors. Other renderers may consume the same model. SVG is generated output, never the only editable or authoritative representation.
6. **Documentation composition:** generate a stable inventory and one Markdown/API page per maintained type, including its class view, summary, member inventory, source links, and relevant interaction/state/architecture views.
7. **Verification and publication:** regenerate deterministically; validate inventory coverage, schema, SVG structure/accessibility, geometry, links, and drift against committed artifacts. Do not conflate a green build with completed living documentation.

## What a type page should contain

Where applicable, each maintained class, struct, interface, record, enum, and delegate should have an unambiguous inventory entry and focused class view. Pages should identify type kind, visibility/modifiers, generic parameters/constraints, fields/properties/events/constructors/methods, parameters/return types, inheritance and interface realization, and evidence-backed associations/dependencies. Include source links and provenance; explicitly mark unresolved/external symbols. Multiplicity, aggregation, and composition must only be shown when supported by evidence.

Coverage means every maintained declaration is accounted for, not that every possible UML diagram family is fabricated for every type.

## Additional diagram families are evidence-driven

- **Sequence/communication:** an actual scenario, trace, or documented interaction with named participants and ordered messages.
- **State machine:** defined states, events/guards, and transitions grounded in implementation or an explicitly authored state model.
- **Activity:** a real workflow or algorithm.
- **Component/package:** actual module boundaries and dependency relationships.
- **Deployment:** real runtime nodes and deployment topology.
- **Use case:** identified actors and goals.

When a view is authored from a scenario rather than mechanically inferred from code, label its assumptions and link claims to source, tests, traces, or other evidence. Runtime traces can highlight responsibility flow; a decorative timeline is not proof of live execution.

## Determinism, drift, and safety

- Stable type/member IDs must not depend only on line numbers or declaration order. Include project/assembly identity, namespace, containing types, generic arity, and member signatures where applicable.
- Sort output by stable identity; normalize line endings and invariant formatting; avoid volatile timestamps.
- Fail closed on syntax/compilation errors when semantic correctness is required. Syntax-only output may exist only when clearly labeled.
- Do not draw resolved semantic connectors from name strings when compiler symbols are available.
- Validate SVG XML, unique IDs, title/description, bounds, text overflow, and local links. XML validity does not prove legibility; rendered visual inspection is a separate gate.
- Test source additions/removals/renames, nested and partial types, overloaded/generic members, ambiguous names, unresolved symbols, malformed source, failed builds, and stale-output detection.
- Keep Roslyn/MSBuild and reflection discovery in build-time tooling, not GUI Core runtime.
- Do not execute arbitrary project build targets or load arbitrary assemblies in a privileged process without an explicit trust boundary.
- Generated-file manifests must distinguish generated files from authored documentation and must fail closed for unsafe or unknown paths.

## Current implementation status — 2026-10-21

The staging implementation has syntax-only and project-semantic paths, a renderer-neutral model, compiler-symbol relationship extraction, a focused one-hop SVG graph, a source/type inventory, per-type page generation, and CI fixtures for partial/generic declarations and resolved relationships. The solution-derived workflow builds and reconciles the maintained Core, Blazor, WPF, and SVG project inventories. The exact-build reflection check currently validates type identity rather than full member-level API parity.

**Important limitation:** generated per-type Markdown/SVG outputs are uploaded as workflow artifacts; they have not yet been promoted into the keeper branch. The committed docs/uml/source-files directory currently contains its authored README only. The safe promotion script/tests and manually triggered workflow exist, but that workflow has not been dispatched. CI therefore proves generation and artifact checks, not committed generated-document freshness.

The latest fully green UML/package/inventory evidence at checkpoint time is for commit 5d003d864758:
- [Source-derived diagrams](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39680522114)
- [Package GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39680522235)
- [Solution-derived UML inventory](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39680522333)

Use exact-head results for later commits. The promotion workflow still needs an explicit dispatch, a review of generated output, and exact-head CI after the promoted pages land.

## Completion criteria

The living UML feature is ready for adoption when a clean checkout can:

1. Analyze a real project/solution and enumerate every in-scope maintained type deterministically.
2. Cross-check source declarations with the exact compiled assembly where a successful build is available.
3. Generate a versioned model, per-type documentation pages, and source-grounded SVG class views.
4. Resolve and render semantic relationships, with unresolved/external references honestly marked.
5. Generate additional diagram families only for evidence-backed scenarios, state models, workflows, and architecture.
6. Embed diagrams in authored articles without duplicating their source of truth.
7. Prove source changes affect the right model/page/SVG outputs; malformed input fails closed and stale artifacts fail CI.
8. Pass semantic tests, accessibility/geometry checks, deterministic regeneration, rendered visual inspection, and exact-commit CI verification.
9. Offer a usable developer command and concise guide for trying the tooling on another codebase.

Until those conditions are met, report the implemented slice and remaining gaps precisely; do not describe the feature as complete.

## Target consumer experience

The longer-term target is an opt-in installable build-time documentation package, not a runtime library dependency and not a generator that every repository must copy into its source tree. It should provide a versioned CLI, an opt-in MSBuild target, deterministic per-type Markdown/SVG/inventory outputs, a manifest with source/build provenance, and safe drift checking. Normal application execution must pay zero documentation-generation cost.

This package lifecycle is a target architecture, not an implemented feature. The current proof remains the staging CLI and focused graph renderer. Package publication remains separately gated by explicit owner authorization.
