# GUI Gallery

This is the repository's visual index. Each entry identifies what the artifact proves and where its reproducible recipe lives.

## Source-derived UML — staging proof

**Artifact:** [Source-derived UML architecture and evidence](LIVING_UML_ARCHITECTURE.md) and [source generator](../samples/SourceUmlGenerator/Program.cs).

**What it shows:** the current tool parses a single C# source file with Roslyn syntax APIs, projects declarations into GUI Core data-card models, and renders a syntax-derived class-compartment SVG through GUI.SVG.

**Evidence boundary:** this is an implemented staging vertical slice with source-mutation, target-selection, ambiguity-rejection, and malformed-source CI fixtures. It is **not** yet project/solution semantic analysis; base-list references are not resolved compiler relationships. The versioned documentation-model DTO is not yet emitted by the generator. Do not use this artifact as proof of a complete semantic UML pipeline or as a supported published interchange contract.

**Reproduce and inspect:** follow [Source-derived UML tooling](SOURCE_DERIVED_UML.md), then read [Living UML architecture and acceptance gates](LIVING_UML_ARCHITECTURE.md).

## GUI.SVG demonstration recipes

Browse the [demonstration gallery](../samples/demonstrations/GALLERY.md). Each recipe describes its input, generated artifact, verification, and evidence limits. The [demonstration index](../samples/demonstrations/README.md) tracks the progression toward runtime-backed proof.
