# GUI Gallery

This is the repository's visual index. Each entry identifies what the artifact proves and where its reproducible recipe lives.

## Source-derived UML — staging proof

**Artifact:** [Source-derived UML architecture and evidence](LIVING_UML_ARCHITECTURE.md) and [source generator](../samples/SourceUmlGenerator/Program.cs).

**What it shows:** the source-file mode parses one C# file and renders syntax-derived GUI.SVG cards/class compartments. A separate project-semantic mode loads the GUI Core `.csproj` and emits a versioned JSON model with compiler-symbol-based inheritance and interface-realization relationships.

**Evidence boundary:** the project-semantic JSON model is an implemented staging slice, not complete living UML. The syntax-only class SVG still labels base-list references as unresolved text; semantic edges are not yet rendered as a multi-node SVG graph. The tool does not yet extract member-type associations, generate a solution-wide inventory/per-type pages, cross-check against the exact built assembly through reflection, or prove text bounds/rendered visual correctness. Do not treat this as a complete semantic UML pipeline or supported published interchange contract.

**Reproduce and inspect:** follow [Source-derived UML tooling](SOURCE_DERIVED_UML.md), then read [Living UML architecture and acceptance gates](LIVING_UML_ARCHITECTURE.md).

## GUI.SVG demonstration recipes

Browse the [demonstration gallery](../samples/demonstrations/GALLERY.md). Each recipe describes its input, generated artifact, verification, and evidence limits. The [demonstration index](../samples/demonstrations/README.md) tracks the progression toward runtime-backed proof.
