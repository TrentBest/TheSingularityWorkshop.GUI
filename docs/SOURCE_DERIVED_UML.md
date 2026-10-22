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

**Latest verified implementation baseline (2026-10-22):** [Package GUI run 39843352488](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39843352488), [Source-derived diagrams run 39843352517](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39843352517), and [Solution-derived UML inventory run 39843352626](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39843352626) passed on source commit [`fdef14c`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/fdef14cb93430fa2678f5f117bf48d97554bedcd). The package workflow passed Core, Blazor, SVG, and WPF tests; the publish job was skipped. The action-pin check, committed UML SVG safety/accessibility check, source-link validation, and Markdown link check passed.

The solution inventory reports 69 types and produced 69 Markdown pages plus 69 SVGs across Core (31), Blazor (6), SVG (9), and WPF (23). The generated neighborhood SVG was rendered and reviewed: cards are collapsed to kind/name plus namespace/member-count context; consumers are above the focus, dependencies below, external references to the right, relationship labels are concise, assembly provenance is shown for external nodes, and the focal card is vertically centered. Source-backed cards link to a revision-pinned source declaration and line. These generated pages remain workflow artifacts rather than a committed complete page set.

Open gaps:

- The complete generated per-type Markdown/SVG set is not yet committed in canonical `docs/api/generated/<Area>` and `docs/assets/uml/classes/<Area>` locations.
- The per-type pages still need companion `<TypeName>UML.md` pages with human-authored use cases; the generator cannot infer actor intent from source alone.
- The focused graph is not a full project/solution graph; it shows one-hop relationships only.
- Exact-build reflection comparison currently covers type identity, not complete member/API parity.
- Committed generated-artifact drift checks and promotion/review of the generated set remain incomplete.
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
