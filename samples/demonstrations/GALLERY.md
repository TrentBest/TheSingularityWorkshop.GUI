# GUI.SVG Recipes — Gallery

> **Recipes, not just screenshots.** Each entry starts with a visible result, tells you what it demonstrates, identifies the packages actually consumed, and links to the implementation trail so you can reproduce and adapt it.

This Markdown file is the canonical reading gallery. The companion [scrollable HTML view](index.html) is a convenience; it must not become a second source of truth.

## How to read a recipe

1. **See it:** the SVG manifestation is embedded in the entry.
2. **Understand it:** the short explainer states the purpose and evidence boundary.
3. **Rebuild it:** follow the recipe document into the model, source code, renderer, generation command, and checks.
4. **Know the dependencies:** package labels distinguish what the sample really references from future integration targets.

## Recipe index by package boundary

| Package / layer | Recipes | What this gallery can demonstrate |
|---|---|---|
| **GUI Core** | [01 · Primitive geometry](#recipe-01--primitive-geometry) | A platform-neutral `GuiNode` tree and recursive builder input |
| **GUI.SVG** | [01 · Primitive geometry](#recipe-01--primitive-geometry), [04 · Animated scene](#recipe-04--animated-scene) | Turning a GUI model into inspectable SVG markup |
| **Renderer** | Planned integration recipe | A textual/semantic scene consumed by the Renderer package; do not infer this dependency from the GUI.SVG sample |
| **Visualize** | Planned integration recipe | A visualization manifestation produced through the Visualize boundary; add only when a real runnable consumer exists |
| **FSM_API / FSM_COS** | Planned behavior-to-visualization recipe | Runtime state and composition driving a scene, with a trace proving the causal path |

**Important:** package groupings are dependency statements, not aspirational labels. Recipe 01 currently references this repository's Core and SVG source projects. It does not claim to consume Renderer, Visualize, FSM_API, or FSM_COS.

---

## Recipe 01 — Primitive geometry

![Recipe 01: primitive geometry — a rounded panel and marker](01-primitive-geometry.svg)

**Purpose:** start with the smallest end-to-end example: define semantic scene nodes, pass the resulting tree to the SVG adapter, and inspect the standalone SVG.

**Packages actually consumed:** GUI Core + GUI.SVG, through source project references.

**What the picture demonstrates**
- A rounded rectangular panel with an explicit identity.
- A circle marker with its own identity.
- Explicit geometry and presentation properties.
- A browser-viewable SVG manifestation that can be inspected as XML.

**What it proves—and does not prove**

The SVG above is a **reference illustration**, not the output of the checked-in generator. The runnable source constructs a `GuiNode` tree and calls `SvgGuiRenderer.Render`, then parses its output and asserts that the expected panel and marker exist. That generator has been added, but its successful execution is not claimed here until a build/run is recorded. A rendered picture alone does not prove FSM_API behavior, Renderer integration, or GPU visualization.

**Follow the recipe**
- [Deep dive: everything behind Recipe 01](recipes/01-primitive-geometry.md) — model, source, renderer, output contract, commands, and verification.
- [Generator source](PrimitiveGeometry/Program.cs)
- [Project and run instructions](PrimitiveGeometry/README.md)
- [Open the reference SVG source](01-primitive-geometry.svg)

---

## Recipe 04 — Animated scene

![Animated GUI.SVG adapter gallery with a filled star following a motion path](../../docs/assets/svg-adapter-gallery.svg)

**Purpose:** explore reusable SVG definitions and declarative animation in a richer scene. The moving entity is a filled star, so the controlled object reads as a distinct symbol rather than a tiny dot.

**Packages actually consumed:** GUI Core + GUI.SVG; the repository's SVG gallery generator and snapshot workflow.

**What it demonstrates:** reusable symbols, a referenced motion path, and declarative SVG animation. The animation is interpreted by a compatible SVG viewer; it is not an FSM_API execution clock and is not evidence of GPU rendering.

**Follow the recipe**
- [SVG adapter contract](../../docs/SVG_ADAPTER.md)
- [Source gallery generator](../../samples/SvgGallery/Program.cs)
- [SVG adapter tests](../../tests/SVG.Tests/)

---

## The intended full-stack recipe

The longer-term showcase should make each boundary visible, rather than skip directly from a diagram to a picture:

```text
UML / data contract
    ↓
Typed domain data + semantic scene description
    ↓
Renderer-facing textual scene / render description
    ↓
Renderer package
    ↓
Visualize package / visualization backend
    ↓
Visible manifestation + inspectable trace
```

The exact API and package order must be taken from the consuming repositories and runnable integration—not assumed from names. GUI.SVG remains a useful inspectable proof surface, but it is a separate adapter and should be shown alongside the Renderer → Visualize path, not silently substituted for it.

A future full-stack recipe should include:
- a UML/data diagram and the concrete data types it describes;
- a human-readable textual scene that can be reviewed before rendering;
- the actual Renderer and Visualize package references;
- a runnable producer and the generated artifact;
- a trace linking input data and state to the visible result;
- explicit statements about which steps are tested and which remain planned.

## Evidence rule

Every recipe must distinguish **reference artwork**, **generated output**, and **runtime-driven behavior**. Link the exact source and test for each claim. Never promote a planned package integration into a demonstrated dependency merely to make the gallery look complete.
