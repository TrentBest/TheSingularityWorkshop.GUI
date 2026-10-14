# SVG_App — Proposed Host Boundary

> **Status: concept only.** `SVG_App` has not been created. This document records the intended architectural direction so implementation can proceed without accidentally assigning host responsibilities to FSM_COS or GUI.SVG.

## The idea

The goal is to let **FSM_COS handle composition complexity** while a small SVG-focused application entrypoint supplies the SVG experience's specific inputs and presents the result.

The working distinction is:

- **FSM_COS** composes requested MicroBundles and dependencies, applies the configuration supplied through its contracts, arbitrates toward convergence, and returns a `RuntimeAssembly`.
- **SVG_App** would be the host/entrypoint for a particular SVG application: provide the composition request and relevant data, receive the assembled result, and coordinate the application-specific handoff.
- **GUI.SVG** is the SVG manifestation adapter. It translates supported GUI/scene data into SVG markup; it is not, by itself, the application host or a general execution loop.

This is a direction to validate against the actual contracts, not a claim that the integration exists today.

## Conceptual flow

```text
SVG_App (future host / entrypoint)
    ├── supplies manifest + host-provided configuration/data
    ▼
FSM_COS
    ├── resolves requested MicroBundles and dependencies
    ├── loads the composition
    └── arbitrates toward convergence
    ▼
RuntimeAssembly
    │
    ▼
SVG_App (host interprets/coordinates the result)
    │ SVG-specific scene / GUI intent and data
    ▼
GUI.SVG adapter
    │
    ▼
SVG document / artifact
```

The arrows represent the intended responsibility flow, not existing APIs. Exact data types, bundle contracts, and the conversion from a `RuntimeAssembly` to SVG input remain to be designed and verified.

## Why keep these boundaries?

### FSM_COS composes; the host owns the application

FSM_COS is a composition kernel, not a GUI, renderer, artifact repository, transport layer, or application loop. Its `RuntimeAssembly` is a handoff object. SVG_App should not expect FSM_COS to decide how an SVG document is rendered, saved, served, or displayed.

### GUI.SVG manifests; it should not become the composition kernel

GUI.SVG owns SVG serialization, SVG-specific element and attribute behavior, and SVG output constraints. It should not need to know how MicroBundles are discovered, configured, or composed.

### The manifest describes composition, not the picture

The experience manifest should identify the capabilities and versions needed for composition. SVG layout, scene content, rendering options, and artifact destinations should not be smuggled into the manifest as renderer implementation details. The host can supply appropriate configuration/data through the relevant contracts.

## A deliberately small first milestone

Before creating SVG_App, prove one vertical slice with real repository contracts:

1. Identify the smallest useful manifest and catalog for an SVG-oriented composition.
2. Supply any per-bundle configuration through the existing configuration boundary; do not invent a new FSM_COS responsibility.
3. Receive a successfully converged `RuntimeAssembly` in a minimal host.
4. Determine what data that assembly actually exposes and what additional host-supplied data is needed to build an SVG scene.
5. Pass the resulting supported scene/model to GUI.SVG and produce a deterministic SVG artifact.
6. Test the failure paths as well as the happy path: missing bundles, dependency/configuration failures, non-convergence, invalid SVG input, and output-generation failure.
7. Keep the sample runnable and make the produced artifact inspectable in documentation and CI.

This sequence is a proposed investigation plan, not a promise that every listed failure is currently represented by a single API.

## Questions to settle with evidence

These are implementation questions—not assumptions to bake into the first version:

- What is the first real SVG experience to compose: a gallery, a documentation artifact, a live visualization, or something else?
- Which MicroBundles already provide useful behavior/data, and which would need to be created?
- Does the resulting `RuntimeAssembly` expose the required data directly, or should a host-side provider/adapter translate it?
- Should the first host write a standalone `.svg` file, return markup to another host, or support both as separate output policies?
- Which rendering options are application configuration, and which belong to the SVG adapter's explicit API?

Answer these from existing code and a runnable proof before adding abstractions.

## Non-goals for the first slice

- Do not add SVG or GUI dependencies to FSM_COS.
- Do not turn FSM_COS into a renderer or application loop.
- Do not make GUI.SVG responsible for MicroBundle discovery or arbitration.
- Do not introduce a large framework, generic plugin system, or speculative bundle vocabulary just to prove the path.
- Do not describe SVG_App as implemented until its code, tests, and generated artifact demonstrate it.

## Related contracts

- [GUI.SVG Guide](SVG_GUIDE.md) — renderer capabilities and current limitations, when available on the working documentation branch.
- [GUI Manifest Boundary](MANIFEST_BOUNDARY.md) — experience composition and GUI implementation are separate.
- [FSM_COS Architecture](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/master/docs/ARCHITECTURE.md) — the composition kernel and `RuntimeAssembly` handoff.
- [FSM_COS Runtime Boundary](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/master/docs/RUNTIME_BOUNDARY.md) — responsibilities that remain with the host.
- [GUI.SVG adapter](SVG_ADAPTER.md) — SVG-specific serialization contract, when available on the working documentation branch.
