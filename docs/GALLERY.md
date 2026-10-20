# GUI Gallery

This is the repository's visual index. Each entry identifies what the artifact proves and where its reproducible recipe lives.

## Source-derived UML — staging proof

**Artifact:** [Source-derived UML architecture and evidence](LIVING_UML_ARCHITECTURE.md) and [source generator](../samples/SourceUmlGenerator/Program.cs).

**What it shows:** the source-file mode parses one C# file and renders syntax-derived GUI.SVG cards/class compartments. The project-semantic mode loads the GUI Core `.csproj`, emits the versioned model, resolves inheritance/interface realization/member associations/signature dependencies with Roslyn symbols, and renders a focused relationship neighborhood as SVG.

**Evidence boundary:** this is a CI-verified staging vertical slice, not complete living UML. The syntax-only class SVG still labels base-list references as unresolved text; compiler-backed facts are emitted separately in the project-semantic model. The focused graph renders one-hop neighbors, not a full project/solution graph. A revision-pinned Markdown inventory now includes a sorted type index and a per-type section for every analyzed type; the generator also emits a standalone Markdown page per type plus an index. CI checks inventory completeness/order and page-set coverage, then uploads the generated evidence. Solution-wide/multi-project inventory, committed generated-artifact drift checks, exact-build reflection comparison, large-graph layout, and comprehensive painted-text/bounds evidence remain open. Do not treat this as a complete semantic UML pipeline or supported published interchange contract.

**Reproduce and inspect:** follow [Source-derived UML tooling](SOURCE_DERIVED_UML.md), then read [Living UML architecture and acceptance gates](LIVING_UML_ARCHITECTURE.md).

## GUI.SVG renderer guide

**Artifact:** [GUI.SVG Guide](SVG_GUIDE.md) and the [Fox/Sheepdog scene model](../samples/FoxSheepdogPlane/Program.cs).

**What it explains:** the current renderer's element families, coordinate-system handling, validation rules, explicit animation serialization, and accessibility/evidence limits. It distinguishes scene-authored SVG from a live runtime or physics simulation.

**Evidence boundary:** the guide is source-grounded; browser-level accessibility, animation interoperability, and external-resource policy remain separate verification work.

## GUI.SVG demonstration recipes

Browse the [demonstration gallery](../samples/demonstrations/GALLERY.md). Each recipe describes its input, generated artifact, verification, and evidence limits. The [demonstration index](../samples/demonstrations/README.md) tracks the progression toward runtime-backed proof.
