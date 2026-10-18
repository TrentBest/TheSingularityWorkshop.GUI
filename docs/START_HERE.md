# Start Here: Explore the GUI

If you arrived here thinking, "I already know how to build a GUI. What would another abstraction add?", that is a useful question.

This repository is designed to answer it gradually. Start with a real manifestation, inspect the code and tests behind it, then decide whether the semantic boundary earns its place.

![The Land of Idealism](assets/ideal-gui-separation-of-concerns.jpg)

*Figure 1 — A reference manifestation of the separation this repository is trying to make tangible: runtime behavior, semantic GUI meaning, and platform presentation can cooperate without becoming the same thing.*

## See a real manifestation move

![Animated GUI.SVG gallery](assets/svg-adapter-gallery.svg)

[Open the animated SVG directly](assets/svg-adapter-gallery.svg) · [Inspect how the adapter produces it](SVG_ADAPTER.md)

This is a generated SVG artifact, not a mockup: the marker pulses and a point travels along a path using declarative SVG animation. It demonstrates a real adapter output without claiming a GPU renderer or a GUI-owned animation loop. [GUI Visual Reference](GUI_VISUAL_REFERENCE.md) distinguishes conceptual references from generated, inspectable output.

## The two-prong documentation model

This project documents two different things because they answer different questions.

### Prong 1 — The technology

**Question:** *How does it work?*

- [GUI Model](GUI_MODEL.md) — neutral semantic tree and current primitives.
- **Architecture & manifestation:** [Architecture](ARCHITECTURE.md) · [Platform Adapters](PLATFORM_ADAPTERS.md) · [GUI Execution Model](GUI_EXECUTION_MODEL.md)
- **Spatial model:** [Ideal GUI](IDEAL_GUI.md) · [GUI Spatial Domain](GUI_SPATIAL_DOMAIN.md) · [GUI Spatial Contracts](GUI_SPATIAL_CONTRACTS.md)
- **Verification & measurement:** [Testing](TESTING.md) · [GUI Benchmarking](GUI_BENCHMARKING.md) · [Roadmap](../ROADMAP.md)
- **Release path:** [Release Readiness Checklist](RELEASE_READINESS.md) — package-specific hard gates; no publication is implied.
- **Next contract surface:** [Next Contract Surface](NEXT_CONTRACT_SURFACE.md) · [Subcomposition and Digital Art](SUBCOMPOSITION_AND_DIGITAL_ART.md)

![Repository structure](assets/repository-structure-overview.jpg)

*Figure 2 — A conceptual map of how the semantic model, adapters, tests, and documentation fit together. It is not intended to be an exhaustive file listing.*

The technology track should let another developer inspect the implementation and answer: "What does this thing actually do?"

### Prong 2 — The theory

**Question:** *Why is it designed this way, and what larger engineering problem is it trying to solve?*

The theory track treats the project as an engineering thesis rather than a widget library. It develops the GUI as an extrinsic interface layer, the breakwater principle, the digital shadow, execution-boundary inversion, representational compatibility, and spatial representation.

Start with the [Theory Guide](THEORY_GUIDE.md), then use the [Textbook Map](THEORY_TEXTBOOK_MAP.md) to follow the chapters.

The theory track is deliberately written so a textbook author can lift a chapter, section, definition, or example with minimal rewriting.

---

# A practical evaluation path

You do not need to agree with the architecture. Ask five concrete questions.

## 1. Can Core remain platform-neutral?

Inspect `src/Core`. The claim is that platform mechanisms belong at the boundary rather than in the semantic model. Look for dependencies on HTML, DOM, CSS, JavaScript, Blazor rendering types, WPF controls, XAML, or Unity UI Toolkit types. If Core cannot maintain that boundary, the architecture has failed its own test.

## 2. Can one intent have different manifestations?

Conceptually:

```text
intent
  -> semantic GUI model
      -> platform adapter
          -> manifestation
```

A button is not fundamentally a WPF Button, an HTML element, or a Unity control. Those are possible manifestations; the semantic object is the intent the adapter represents.

## 3. Does the abstraction earn its place?

A useful abstraction should reduce friction without forcing every target platform into the same shape. A Core concept belongs in the model only when its meaning can be defined independently of a particular platform.

## 4. Can the model describe more than conventional widgets?

Consider a floor plan, map, state inspector, CAD view, 3D scene, diagnostic visualization, image, mesh, animation, spatial interaction, accessibility interface, or deliberately unstructured visual output. The current implementation is not yet a complete model for every example; these are questions against the intended representation space. See [GUI Expressiveness](GUI_EXPRESSIVENESS.md) and [GUI Coverage Survey](GUI_COVERAGE_SURVEY.md).

## 5. Does the theory clarify engineering boundaries?

The theory makes architectural predictions that can be challenged: experience identity should not depend on a particular GUI; input translation belongs at the boundary; multiple manifestations can observe the same experience; and execution-host behavior should become a contract rather than a rendering convenience.

---

# A ten-minute evaluation

## 1. See the Workshop

Open the live [The Singularity Workshop WebPage](https://lemon-ground-09f542010.1.azurestaticapps.net/) and interact with it. The live site is an experience to explore, not proof that the current development branch has completed the GUI integration.

## 2. Inspect the intended integration path

The [WebPage repository](https://github.com/TrentBest/WebPage/tree/development) is the intended first consuming application. The original migration issue [WebPage #52](https://github.com/TrentBest/WebPage/issues/52) was closed, but issue closure and dependency references are not substitutes for end-to-end build/test evidence. See [ROADMAP.md](../ROADMAP.md); keep integration claims conditional until the current development build and tests prove the actual path.

```text
WebPage experience
      |
      v
GUI semantic model
      |
      v
Blazor adapter
      |
      v
browser manifestation
```

## 3. Inspect the boundary and tests

1. Read [GUI Model](GUI_MODEL.md).
2. Inspect `src/Core`.
3. Inspect `src/Blazor` and `src/SVG`.
4. Inspect `src/WPF` and its [Windows-native tests](../tests/WPF.Tests/TheSingularityWorkshop.GUI.WPF.Tests.csproj).
5. Read [Testing](TESTING.md) and [Platform Adapters](PLATFORM_ADAPTERS.md).

The WPF renderer has an initial tested Core-to-native path. That is evidence of an adapter, not a claim of full parity, complete lifecycle handling, or complete interaction/accessibility support.

## 4. Explore spatial representation and measurement

Continue into [Ideal GUI](IDEAL_GUI.md), [GUI Spatial Domain](GUI_SPATIAL_DOMAIN.md), and [GUI Spatial Contracts](GUI_SPATIAL_CONTRACTS.md). Benchmark work is tracked by [GUI issue #2](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/2); see [GUI Benchmarking](GUI_BENCHMARKING.md) for the intended methodology.

---

# What this repository is not claiming yet

The execution-host model, capability negotiation, broad cross-adapter conformance, and some richer semantic contracts remain design work. The roadmap is part of the documentation, not an afterthought.

- **Implemented:** visible in current source.
- **Tested:** covered by a named test or repeatable command.
- **CI-verified:** the relevant workflow passed for the stated commit.
- **Specified:** documented, with no implementation implied.
- **Proposed / unresolved:** work remains.

See [ROADMAP.md](../ROADMAP.md), [Next Contract Surface](NEXT_CONTRACT_SURFACE.md), and [Release Readiness](RELEASE_READINESS.md). A package is not release-ready merely because one test suite passes; use the checklist to record scope, artifacts, consumer evidence, and the exact candidate commit.

## The ultimate test

Do not ask whether you believe in this GUI abstraction. Ask whether it gives you a cleaner place to put a problem that already exists. The code, tests, and documentation should make that judgment possible without overstating what is implemented.
