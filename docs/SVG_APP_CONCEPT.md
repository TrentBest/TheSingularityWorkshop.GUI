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
FsmCos.Execute(...)
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

The domain-owned `IMicroBundle` contract exposes a `MicroBundleDescriptor`, identity, dependency requests, and load/arbitration behavior. The descriptor provides bundle ID, version, declared dependency IDs, and provider IDs.

The assembly does **not** expose a `GuiNode`, SVG markup, a scene graph, or a renderer. It does, however, contain enough metadata to make a truthful *composition inspection* graphic without pretending that arbitrary application data is automatically available.

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

## Recommended first proof: visualize the assembly itself

The safest first vertical slice is a **composition inspection graphic**, not a claim that the composed bundles already provide a complete GUI experience.

Why this is the strongest starting point:

- The actual `RuntimeAssembly` provides loaded bundles.
- Each bundle's actual descriptor provides identity, version, declared dependency IDs, and provider IDs.
- GUI.SVG already has the primitives needed to draw nodes, connectors, labels, and accessible descriptions.
- This proves composition → host interpretation → `GuiNode` → SVG without inventing an application-specific bundle contract or adding dependencies to FSM_COS.

A first proof could show the runtime ID and arbitration result, each loaded bundle in assembly order, its version, declared dependencies, and provider IDs. It must distinguish **declared dependency metadata** from **verified graph edges**: the host should not imply a relationship that it has not resolved from the actual assembly/catalog contracts.

The FSM_COS repository has a minimal in-memory catalog / dependency example on its `development` branch at [Minimal Consumer](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/samples/FSM_COS.MinimalConsumer/Program.cs). That is useful as a composition reference; it is not an SVG integration and should not be described as available on every branch.

This is a recommendation for the first proof, not an implemented feature or a requirement to make every future SVG experience an assembly diagram.

## A deliberately small implementation milestone

1. Confirm the package/project reference strategy for the host. FSM_COS alpha.6 is still a source candidate; do not assume it is already available from NuGet.
2. Use the existing minimal-consumer composition pattern: an in-memory catalog, a tiny bundle set, and a versioned `RuntimeManifest`.
3. Receive a successfully converged `RuntimeAssembly` in the host.
4. Build a small composition-inspection `GuiNode` scene from the real assembly and descriptor fields.
5. Call `SvgGuiRenderer.Render`, write a standalone SVG artifact, and verify its semantic content in a test or CI step.
6. Test meaningful failure paths: missing bundles, dependency/configuration failures, non-convergence, invalid SVG input, and output-generation failure.

This sequence is a proposed plan. It does not claim that every failure path is represented by one API or that the end-to-end host currently exists.

## Questions to settle with evidence

- Where should the first host live? A future dedicated `SVG_App` repository is a clean boundary, but should be created only when the project/package references can be made reproducible.
- What is the narrowest reproducible dependency strategy while FSM_COS alpha.6 and the GUI.SVG project/package are still in development?
- Should the first host write a standalone `.svg` file, return markup to another host, or support both as separate output policies?
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
