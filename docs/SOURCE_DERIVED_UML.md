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
5. **Inventory and per-type pages:** the four-project inventory is generated and safely promoted into `docs/uml/source-files`. Source/project/generator changes on `development` trigger regeneration, exact-build validation, manifest-owned promotion, a drift check, and a commit of the generated pages. Manual dispatch remains available for explicit refreshes.
6. **Validation:** exercises syntax mutations and malformed input, partial/generic declarations, project-semantic relationships, exact-build type-identity checks, SVG accessibility metadata, unique IDs, local references, and inventory coverage.

## Reproduce and inspect

- [Generator source](../samples/SourceUmlGenerator/Program.cs)
- [Source-derived workflow](../.github/workflows/source-derived-diagrams.yml)
- [Solution-derived inventory workflow](../.github/workflows/solution-derived-uml.yml)
- [SVG contract verifier](../scripts/verify_svg_contract.py)
- [Promotion safety tests](../scripts/test_promote_source_uml.py)
- [Automatic/manual promotion workflow](../.github/workflows/promote-source-uml-docs.yml)

Use the documented CLI options in the generator source/help output. Do not treat generated artifacts as authoritative source or edit generated pages as if they were hand-authored documents.

## Current evidence and limits

**Latest verified implementation and promotion evidence (2026-10-22):** [Package GUI run 39862353055](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39862353055), [Source-derived diagrams run 39862352908](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39862352908), and [Solution-derived UML inventory run 39862352980](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39862352980) passed on implementation commit [`a25acf3`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/a25acf387b1e24b6dba0a927e763c0b55c8b9b3e). The promotion workflow also passed in [run 39862352900](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39862352900) and committed the generated set at [`14b442f`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/14b442f36b35465464c6ecd0a9589bffa277a5f0). NuGet publication remained skipped.

The committed solution inventory reports 69 types across Core (31), Blazor (6), SVG (9), and WPF (23), with 69 Markdown pages and 69 SVGs now present in `docs/uml/source-files`. The generated neighborhood SVG was rendered and reviewed: cards are collapsed to kind/name plus namespace/member-count context; consumers are above the focus, dependencies below, external references to the right, relationship labels are concise, assembly provenance is shown for external nodes, and the focal card is vertically centered. Source-backed cards link to a revision-pinned source declaration and line. The pages remain source-derived technical inventory and do not replace human-authored use-case pages.

Open gaps:

- The generated set is committed under `docs/uml/source-files`; the final consumer-facing canonical page layout (`docs/api/...` with the associated `docs/assets/uml/classes/...` assets) remains to be reconciled with the generated inventory path.
- The per-type pages still need companion `<TypeName>UML.md` pages with human-authored use cases; the generator cannot infer actor intent from source alone.
- The focused graph is not a full project/solution graph; it shows one-hop relationships only.
- Exact-build reflection comparison currently covers type identity, not complete member/API parity.
- Automatic regeneration, manifest-owned promotion, and drift checking are implemented and passed in the promotion run; a source change that affects the model triggers a refreshed generated commit. Human review of semantic relationships and the final canonical layout remain separate.
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
