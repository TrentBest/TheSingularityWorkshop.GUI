![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# UML Documentation Visual Audit

**Scope:** source-derived UML documentation and generated UML artifacts. This is not the repository-wide visual audit.

| Item | Classification | Current status and evidence |
|---|---|---|
| README | Existing conceptual flowchart and navigation | The opening Mermaid flowchart explains architecture; it is not source-generated UML. README links to the visual gallery and current UML guides. |
| docs/GALLERY.md | Visual/evidence index | Documents the real SVG gallery and current UML staging output, including the gap between workflow artifacts and committed generated pages. |
| docs/SOURCE_DERIVED_UML.md | Practical guide | Documents syntax-only and project-semantic paths, exact-head evidence, safety rules, and current gaps. |
| docs/LIVING_UML_ARCHITECTURE.md | Architecture and acceptance criteria | Describes the source-to-model-to-diagram pipeline and the gates for adoption. A current generated architecture picture remains future work. |
| docs/uml/source-files/README.md | Authored generated-directory boundary | Explains that generated pages must be kept separate from authored documentation. The generated per-type page set is not yet committed. |
| Source-derived SVG artifacts | Generated staging outputs | CI checks XML structure, accessibility metadata, IDs, local references, selected semantic relationships, and fixture coverage. |
| Per-type Markdown/SVG set | Missing from committed keeper tree | CI artifacts are not equivalent to committed living documentation. Promotion must be deliberate and followed by drift verification. |
| Rendered text bounds and visual review | Open | XML validity does not prove legibility. Record visual inspection and geometry/bounds evidence for representative and edge-case diagrams. |
| Generated-artifact drift gate | Open | Regeneration must fail CI when committed generated pages/diagrams are stale, while never overwriting authored files. |

## Acceptance checks

- Every maintained production type has a stable inventory entry and a dedicated page where required by the documentation standard.
- Each generated page embeds a focused source-grounded class view and links to source/provenance.
- Semantic relationships come from compiler symbols when available; unresolved/external references remain explicit.
- SVG has accessible title/description, valid IDs/references, finite geometry, and inspected text bounds.
- Source mutation/addition/removal changes the appropriate inventory/pages; malformed or semantically invalid input fails closed.
- Generated output is deterministic and compared with the committed file set.
- The exact source commit and workflow runs are recorded; a successful earlier run does not validate a later commit.

See [Source-derived UML tooling](SOURCE_DERIVED_UML.md) and [Living UML architecture](LIVING_UML_ARCHITECTURE.md). These are current acceptance criteria, not a claim that every item is already complete.
