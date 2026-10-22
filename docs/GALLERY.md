![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# GUI Gallery

This is the visual index for TheSingularityWorkshop.GUI. Each entry says what the real implementation demonstrates, where to reproduce it, and what the evidence does not prove.

## GUI.SVG adapter manifestation

**Picture:** [Generated SVG adapter gallery](assets/svg-adapter-gallery.svg)

The package workflow runs the actual SvgGallery sample through SvgGuiRenderer, checks required SVG constructs, and compares the generated output byte-for-byte with the checked-in gallery asset. This is a reproducible living artifact, not a hand-drawn mockup.

**Reproduce:** [SVG adapter guide](SVG_GUIDE.md) · [gallery source](../samples/SvgGallery/Program.cs) · [package workflow](../.github/workflows/package.yml)

**Boundary:** viewer-dependent animation and filters are declarative SVG output. This does not establish a GUI-owned animation clock, live FSM_API/FSM_COS execution, or GPU rendering.

## Source-derived UML — staging proof

**Artifact:** outputs from the [Source-derived diagrams workflow](../.github/workflows/source-derived-diagrams.yml).

The build-time generator has a syntax-only path and a compiler-backed project-semantic path, with a renderer-neutral documentation model and a focused relationship-neighborhood SVG. The solution-derived inventory workflow generated 69 type pages and 69 SVGs across Core (31), Blazor (6), SVG (9), and WPF (23) at its verified source revision. These remain workflow artifacts rather than a committed, regenerated documentation set.

**Reproduce:** [Source-derived UML tooling](SOURCE_DERIVED_UML.md) · [living UML architecture and acceptance gates](LIVING_UML_ARCHITECTURE.md) · [generator source](../samples/SourceUmlGenerator/Program.cs)

**Boundary:** this is a staging vertical slice, not complete solution-wide living UML. The committed tree currently contains the authored README under docs/uml/source-files, not the generated per-type page set. The promotion workflow is manual and has not been dispatched. Do not claim committed-artifact freshness or full project/solution graph coverage yet.

**CI evidence (2026-10-22):** [Package GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39843352488) · [source-derived diagrams](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39843352517) · [solution-derived UML inventory](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39843352626). All three passed on implementation commit [fdef14c](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/fdef14cb93430fa2678f5f117bf48d97554bedcd); NuGet publication was skipped.

## GUI.SVG demonstration recipes

**Picture collection:** [Markdown demonstration gallery](../samples/demonstrations/GALLERY.md) · [scrollable visual index](../samples/demonstrations/index.html)

The recipes demonstrate primitive geometry, sampled functions, recorded Physics trajectories, seeded point sets, declarative animation, and a parameterized Mandelbrot field. Physics-backed examples consume versioned run records and validate rendered markers/cells against recorded source data instead of recomputing the mathematics inside GUI.SVG.

**Reproduce:** follow each linked recipe from the [demonstration gallery](../samples/demonstrations/GALLERY.md).

**Boundary:** generated run records and SVGs are evidence for their specific finite inputs, not proof of a general physics engine, live application binding, GPU rendering, or an interactive browser explorer.

## Fox/sheepdog scene

**Picture:** [10×10 fox/sheepdog SVG](assets/fox-sheepdog-10x10.svg)

The FoxSheepdogPlane sample builds a semantic GUI scene and serializes it through the SVG adapter, including a grid and declarative motion/choreography. It demonstrates how a GUI model becomes inspectable vector output.

**Reproduce:** [sample source](../samples/FoxSheepdogPlane/Program.cs) · [SVG adapter contract](SVG_ADAPTER.md)

**Boundary:** the checked-in visual has been manually iterated and is not currently guaranteed to be byte-identical to fresh sample output. The animation is SVG/CSS presentation, not a live FSM trace, algorithmic animal behavior, or a physics simulation.

## WPF and Blazor adapters

The repository also contains a WPF renderer and Blazor render-fragment builders, with focused tests for their current contracts. These are implementation surfaces, not a claim of identical feature parity across adapters.

**Reproduce:** [WPF renderer source](../src/WPF/WpfGuiRenderer.cs) · [Blazor builder source](../src/Blazor/WorkshopGuiBuilder.cs) · [tests](../tests)

## Authored visual concepts

The [SVG visual concepts index](visual-concepts/README.md) preserves two hand-authored exploratory illustrations: the semantic GUI boundary and an energy-vortex motif. They are clearly labelled concept art and are not evidence that SvgGuiRenderer generated them or that the framework runs a live simulation.

## Audit and release boundary

- [Operation Sith branch audit](SITH_BRANCH_AUDIT.md) records reconciliation evidence and keeper-branch policy.
- [Release readiness](RELEASE_READINESS.md) records the separate package gate.
- Successful CI is not authorization to publish. NuGet publication remains disabled by the explicit workflow hard stop until the owner authorizes a change.

The gallery is an evidence index, not a claim that every adapter, diagram, viewer, or release gate is complete.
