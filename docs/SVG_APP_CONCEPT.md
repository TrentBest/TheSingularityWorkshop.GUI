# SVG_App — Proposed Host Boundary

> **Status: concept only.** `SVG_App` has not been created. This document records the intended architectural direction so implementation can proceed without accidentally assigning host responsibilities to FSM_COS or GUI.SVG.

## The idea

The goal is to let **FSM_COS handle composition complexity** while a small SVG-focused application entrypoint supplies the SVG experience's specific inputs and presents the result.

The working distinction is:

- **FSM_COS** composes requested MicroBundles and dependencies, applies configuration supplied through its contracts, arbitrates toward convergence, and returns a `RuntimeAssembly`.
- **SVG_App** would be the host/entrypoint for a particular SVG application: provide the composition request and relevant data, receive the assembled result, and coordinate the application-specific handoff.
- **GUI.SVG** is the SVG manifestation adapter. It translates a platform-neutral `GuiNode` tree into SVG markup; it is not the application host or a general execution loop.

This is a direction to validate against the actual contracts, not a claim that the integration exists today.

## Conceptual flow

```text
SVG_App (future host / entrypoint)
    ├── supplies RuntimeManifest + host-provided configuration/data
    ▼
IFsmCos.Execute(...)
    ▼
FSM_COS
    ├── resolves requested MicroBundles and dependencies
    ├── loads the composition
    └── arbitrates toward convergence
    ▼
RuntimeAssembly (loaded MicroBundles + runtime metadata)
    │
    ▼
SVG_App (host interprets/coordinates the result)
    │ builds or obtains a GuiNode scene
    ▼
SvgGuiRenderer.Render(scene, width, height)
    ▼
SVG document / artifact
```

The middle and final steps are separate contracts today; the host-side bridge between them is not implemented.

## What the current contracts actually expose

This distinction is the key design constraint—not a missing feature to paper over.

### FSM_COS result

The current `RuntimeAssembly` exposes:

- `RuntimeId`
- `Bundles` / `LoadedBundles` — the loaded `IMicroBundle` instances
- `ArbitrationRounds`
- optional `Intent`
- `TryGetBundle(...)` helpers for retrieving loaded capabilities

It does **not** expose a `GuiNode`, SVG markup, a scene graph, or a renderer. A bundle may offer useful behavior or data through its own contract, but the host must inspect those actual bundle contracts before assuming what is available.

### GUI.SVG input and output

GUI.SVG currently offers `SvgGuiRenderer.Render(GuiNode root, int width = 960, int height = 540)`, returning a string containing the SVG document. GUI Core's `GuiBuilder.Create(kind, id)` builds a `GuiNode` snapshot via `Build()`.

So the host-side conversion is a real responsibility: select useful data/capabilities from the assembled runtime, create or obtain a GUI scene, then pass that scene to GUI.SVG. The renderer should remain unaware of FSM_COS.

## Why keep these boundaries?

### FSM_COS composes; the host owns the application

FSM_COS is a composition kernel, not a GUI, renderer, artifact repository, transport layer, or application loop. Its `RuntimeAssembly` is a handoff object. SVG_App should not expect FSM_COS to decide how an SVG document is rendered, saved, served, or displayed.

### GUI.SVG manifests; it should not become the composition kernel

GUI.SVG owns SVG serialization, SVG-specific element and attribute behavior, and SVG output constraints. It should not need to know how MicroBundles are discovered, configured, or composed.

### The manifest describes composition, not the picture

The experience manifest identifies capabilities and versions needed for composition. SVG layout, scene content, rendering options, and artifact destinations should not be smuggled into the manifest as renderer implementation details. The host can supply appropriate configuration/data through the relevant contracts.

## A deliberately small first milestone

Before creating a broad application framework, prove one vertical slice with real repository contracts:

1. Identify the smallest useful manifest and catalog for an SVG-oriented composition.
2. Use a real, existing MicroBundle contract where possible; avoid inventing bundle vocabulary before the example requires it.
3. Supply any per-bundle configuration through the existing configuration boundary.
4. Receive a successfully converged `RuntimeAssembly` in a minimal host.
5. Demonstrate, from a concrete bundle contract, what runtime data the host can actually consume.
6. Build a small `GuiNode` scene from that data and call `SvgGuiRenderer.Render`.
7. Write the resulting SVG artifact and verify meaningful output in a test or CI step.
8. Test failure paths as well as the happy path: missing bundles, dependency/configuration failures, non-convergence, invalid SVG input, and output-generation failure.

This sequence is a proposed investigation plan, not a claim that every failure path is currently represented by a single API.

## Questions to settle with evidence

These are implementation questions—not assumptions to bake into the first version:

- Which existing MicroBundle provides useful data or behavior for the first SVG proof?
- Does its current public contract expose that data, or is a small host-side adapter appropriate?
- What should the first scene represent: a composition/loaded-capability map, a documentation artifact, or a real experience?
- Should the host write a standalone `.svg` file, return markup to another host, or support both as separate output policies?
- Which rendering options are application configuration, and which belong to GUI.SVG's explicit API?

Answer these from source and a runnable proof before adding abstractions.

## Non-goals for the first slice

- Do not add SVG or GUI dependencies to FSM_COS.
- Do not turn FSM_COS into a renderer or application loop.
- Do not make GUI.SVG responsible for MicroBundle discovery or arbitration.
- Do not introduce a large framework, generic plugin system, or speculative bundle vocabulary just to prove the path.
- Do not describe SVG_App as implemented until its code, tests, and generated artifact demonstrate it.

## Related contracts

- [GUI Manifest Boundary](MANIFEST_BOUNDARY.md) — experience composition and GUI implementation are separate.
- [GUI.SVG source contract](../src/SVG/README.md) — SVG manifestation role and current usage.
- [FSM_COS RuntimeAssembly source](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/master/src/FSM_COS/RuntimeAssembly.cs) — exact composition result surface.
- [FSM_COS Architecture](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/master/docs/ARCHITECTURE.md) — the composition kernel and runtime handoff.
- [FSM_COS Runtime Boundary](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/master/docs/RUNTIME_BOUNDARY.md) — responsibilities that remain with the host.
