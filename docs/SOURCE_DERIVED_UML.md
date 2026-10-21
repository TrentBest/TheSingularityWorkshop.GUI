![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# Source-derived UML tooling

## Purpose and boundary

The source-derived UML tooling is a build-time documentation tool, not a GUI Core runtime feature. It analyzes C# source with Roslyn, optionally uses a project compilation for symbol-resolved relationships, normalizes the results into a renderer-neutral documentation model, and renders focused SVG manifestations through GUI.SVG.

The current generator is a staging tool under samples/SourceUmlGenerator. It is not an installable documentation package or a supported public interchange contract. Roslyn/MSBuild and reflection work must remain outside the runtime GUI Core dependency graph.

## Current pipeline

1. **Syntax-only analysis:** parses a selected C# source file and can produce syntax-derived cards/class views. Names appearing in a base list are not automatically treated as semantically resolved relationships.
2. **Project-semantic analysis:** loads the configured project through MSBuild/Roslyn and fails closed on compilation errors. Compiler symbols provide evidence for inheritance, interface realization, member associations, and signature dependencies.
3. **Normalized documentation model:** carries type/member identities, source spans, repository/revision/project provenance, typed relationships, and resolution/evidence details. It must not be mislabeled as a Roslyn AST when it is a normalized model snapshot.
4. **Focused UML rendering:** projects a selected type and its direct relationship neighborhood into SVG. It is not yet a full-project or solution-wide graph layout.
5. **Inventory and per-type pages:** can be generated as workflow artifacts. A safe promotion script and manually triggered promotion workflow exist, but the generated page set has not yet been promoted into the keeper branch.
6. **Validation:** exercises syntax mutations and malformed input, partial/generic declarations, project-semantic relationships, exact-build type-identity checks, SVG accessibility metadata, unique IDs, local references, and inventory coverage.

## Reproduce and inspect

- [Generator source](../samples/SourceUmlGenerator/Program.cs)
- [Source-derived workflow](../.github/workflows/source-derived-diagrams.yml)
- [Solution-derived inventory workflow](../.github/workflows/solution-derived-uml.yml)
- [SVG contract verifier](../scripts/verify_svg_contract.py)
- [Promotion safety tests](../scripts/test_promote_source_uml.py)
- [Manual promotion workflow](../.github/workflows/promote-source-uml-docs.yml)

Use the documented CLI options in the generator source/help output. Do not treat generated artifacts as authoritative source or edit generated pages as if they were hand-authored documents.

## Current evidence and limits

The three UML/package workflows passed on commit 5d003d864758 on 2026-10-21:

- [Source-derived diagrams run 39680522114](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39680522114)
- [Solution-derived UML inventory run 39680522333](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39680522333)
- [Package GUI run 39680522235](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39680522235)

Those results validate that specific commit only. Always use exact-head workflow runs when reporting later status.

Open gaps:

- Generated per-type Markdown/SVG pages are not yet committed to docs/uml/source-files; only its authored README is present.
- The manual promotion workflow has not been dispatched.
- Exact-build reflection comparison currently covers type identity, not complete member/API parity.
- The focused graph is not a full project/solution graph and does not yet provide comprehensive layout/routing.
- SVG structure checks do not prove that all painted text fits or that every browser renders every feature identically.
- Committed generated-artifact drift checks and visual inspection evidence remain incomplete.
- The generator is not yet an installable build-time package with a stable consumer contract.

## Safety rules

- Keep source-analysis, compiler/reflection, diagram semantics, and SVG layout as distinct stages.
- Preserve stable identities, source spans, provenance, relationship kinds, and explicit unresolved/external states.
- Do not infer composition, aggregation, or ownership from a field reference alone.
- Fail closed when semantic analysis requires a successful compilation but errors are present.
- Keep generated files separate from authored documents. A manifest may authorize pruning only files it previously generated and recognizes; never overwrite authored files or delete paths outside the manifest's safe scope.
- Do not execute arbitrary project targets or load arbitrary assemblies in a privileged process without a deliberate trust boundary.
- CI success does not authorize NuGet publication; release authorization is a separate owner-controlled gate.

See [Living UML architecture](LIVING_UML_ARCHITECTURE.md) for acceptance criteria and [GUI Gallery](GALLERY.md) for the current visual/evidence index.
