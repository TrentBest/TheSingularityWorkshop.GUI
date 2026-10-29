<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ADR: Renderer-neutral charting semantics and package boundary

- **Status:** Proposed; owner decision required before public API implementation
- **Date:** 2026-10-23
- **Scope:** GUI.Core, GUI.Blazor, GUI.SVG, GUI.WPF and shared adapter-conformance tests
- **Related work:** [Full GUI Domain Coverage](../FULL_GUI_DOMAIN_COVERAGE.md), [GUI Surface Coverage Audit](../GUI_SURFACE_COVERAGE_AUDIT.md), [Issue #42](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/42)

## Context

The current Core model is a renderer-neutral immutable tree. `GuiNode` carries a kind and identity, optional text/source, string-valued properties, and ordered children. `GuiBuilder` builds that tree and can attach stable command identities. This is useful as a general-purpose representation, but string properties alone do not define a reliable quantitative chart contract.

The adapters deliberately own their platform manifestations: Blazor maps nodes to web elements, SVG serializes a visual tree, and WPF maps nodes to native controls. Existing documentation warns against assuming full adapter parity. Charts are named in the expressiveness vision as a future capability; that is not evidence that a chart API or chart renderer currently exists.

The design must preserve GUI's role as the semantic boundary without making Core depend on a particular renderer, UI framework, charting library, or platform.

## Source inspection: current capability evidence

Evidence refreshed on `development` 2026-10-29 (the proposal was first drafted 2026-10-23): `src/Core/GuiNode.cs`, `src/Core/GuiBuilder.cs`, `src/Blazor/BlazorGuiRenderer.cs`, `src/SVG/SvgGuiRenderer.cs`, `src/WPF/WpfGuiRenderer.cs`, and `tests/Adapter.Conformance.Tests/SharedAdapterConformanceTests.cs`.

The evidence confirms a general-purpose tree, not a typed chart contract:

- `GuiNode` stores `Kind`, `Id`, optional `Text`/`Source`, string-to-string `Properties`, and ordered `Children`. It has no numeric series, category, axis, range, unit, or missing-value model.
- `GuiBuilder.Property(name, value)` accepts strings; `WithOnClick` records a host-neutral command identity. Neither provides chart-specific data semantics.
- Blazor's observed built-in kind mapping is `Panel` → `div`, `Button` → `button`, `Image` → `img`, `Text` → `span`, `Warning` → `aside`; unknown kinds become `div`. This is not a chart renderer.
- WPF's observed built-in mapping is `Button`, `Text`, `Column`, `Row`, `Panel`, `Warning`, and `Image`; `orientation` and `isEnabled` map to native WPF state, while `background`/`foreground` support literal brushes and host-owned `resource:` keys. Unknown kinds become a content panel. This is not a chart renderer.
- SVG has explicit primitives such as `svg-line`, `svg-rect`, `svg-circle`, `svg-polyline`, and `svg-polygon`, plus coordinate-oriented vocabulary. Those primitives can draw marks, but the inspected code does not define quantitative chart semantics, series/category association, axis policy, or a chart-level accessible data alternative.
- Existing shared adapter-conformance tests cover tree identity, order, text, duplicate IDs, depth limits, and some property differences. The inspected shared fixture is a button/status tree, not a chart fixture.

### Evidence-based chart capability matrix

| Surface | What source inspection establishes | Chart semantic contract status |
|---|---|---|
| Core | Renderer-neutral tree; properties are strings; children ordered | No typed chart model observed |
| Blazor | Generic node-to-HTML-element mapping | No chart kind or chart-specific mapping observed |
| SVG | SVG drawing primitives and coordinate vocabulary | Drawing primitives exist; chart semantics not established |
| WPF | Generic nodes mapped to native WPF controls | No chart kind or chart-specific mapping observed |
| Shared conformance tests | Common tree invariants tested across Blazor and SVG | No shared chart fixture or chart conformance observed |

This is a bounded source inspection, not a claim that no chart-like experiment exists anywhere in every branch or external consumer. Capability labels are **not implemented in inspected paths** or **not established by inspected evidence**; adapter parity must not be inferred from drawing primitives.

## Project-reference and solution evidence

Inspected the project files and solution manifest on `development`:

- `src/Core/TheSingularityWorkshop.GUI.Core.csproj` has no explicit `ProjectReference` or `PackageReference`; this supports the current Core package's dependency-light boundary at the project-file level (it does not prove the absence of every transitive SDK/runtime dependency).
- `src/Blazor/TheSingularityWorkshop.GUI.Blazor.csproj` references Core and the `Microsoft.AspNetCore.Components` and `Microsoft.AspNetCore.Components.Web` packages (both version `8.0.31`).
- `src/SVG/TheSingularityWorkshop.GUI.SVG.csproj` references Core and has no explicit package references.
- `src/WPF/TheSingularityWorkshop.GUI.WPF.csproj` references Core and targets `net8.0-windows` with `UseWPF=true`; it has no explicit package references in the inspected project file.
- `tests/Adapter.Conformance.Tests/TheSingularityWorkshop.GUI.Adapter.Conformance.Tests.csproj` references Core, Blazor, and SVG. It does **not** reference WPF in the inspected file, so this project alone cannot substantiate shared WPF conformance.
- `TheSingularityWorkshop.GUI.slnx` lists Core, Blazor, WPF, SVG, documentation, one sample, and Core/Blazor/SVG test projects. It does **not** list the adapter-conformance test project or the WPF test project in the inspected solution manifest. The package workflow invokes adapter conformance explicitly on Ubuntu and runs WPF tests plus an archive-only consumer on Windows; this is CI orchestration evidence, not solution-membership evidence.

The project-reference shape is therefore a star around Core for the three adapters, not a chain between adapters. This is useful evidence for either future chart option: any optional chart semantic package should depend on Core only when it genuinely needs tree integration, and adapters should depend on the chart contract only if they implement that contract. This is a project-file audit, not a full transitive package-lock or workflow-execution audit.

## Decision drivers

1. Chart meaning and data interpretation must be independent of how a renderer draws them.
2. Existing non-chart GUI consumers should not acquire an unnecessary charting dependency.
3. Data should be typed and validated; it must not rely on undocumented string-property conventions.
4. Adapters must state what they support. Unsupported chart semantics must not silently turn into misleading output.
5. A single chart fixture should have shared semantic tests and adapter-specific evidence.
6. Package count, versioning, and dependency edges must follow demonstrated needs rather than naming symmetry.
7. Accessibility, text/non-visual fallback, labels, units, and data ordering are part of the contract—not late renderer decoration.
8. This work must not block or silently alter the existing four-package release candidate or authorize publication.

## Options considered

### A. Put typed chart semantics directly in GUI.Core

Core would own chart-domain types and validation alongside the renderer-neutral tree.

**Advantages**
- One obvious semantic authority.
- No extra package for consumers already using Core.
- Easier shared vocabulary for every adapter and future GUI surfaces.

**Costs and risks**
- Every Core consumer inherits chart API surface even when it never charts data.
- Chart-specific types could make the general-purpose model harder to evolve if the first contract is too narrow.
- Core release cadence becomes coupled to chart API compatibility.

**Fit:** best if charts are a foundational GUI semantic, expected to be understood by all adapters, and the public contract can remain small and renderer-independent.

### B. Create a separate chart-contract package that depends on GUI.Core

A small optional package would define typed chart semantics and validation; adapters would add explicit support for that contract.

**Advantages**
- Optional dependency for consumers that do not chart.
- Independent maturity and versioning while the chart contract evolves.
- A clean place to hold chart-specific domain types without importing a chart-rendering library into Core.

**Costs and risks**
- Another package and compatibility surface to build, document, test, and release.
- Potential fragmentation if the contract is actually foundational.
- Adapter and consumer dependency graphs become more complex.

**Fit:** best if charting is a genuinely optional capability with an independent evolution/release need, not merely because the name sounds tidy.

### C. Encode charts only as generic GuiNode kinds and string properties

**Advantages**
- No immediate package or public type expansion.
- A quick visual spike is possible.

**Costs and risks**
- Numeric types, missing values, ranges, category identity, units, and series semantics become conventions rather than enforceable contracts.
- Each adapter can interpret the same properties differently.
- Invalid data and accessibility behavior are difficult to validate consistently.

**Fit:** acceptable only as a throwaway visual experiment. Not recommended as the durable public chart contract.

## Provisional direction

**Do not choose A or B by naming convention alone. Do not ship C as the public contract.**

First implement and test a small design spike against the existing Core tree and shared adapter-conformance architecture. Keep the spike isolated from the current release branch and do not introduce a charting-library dependency. Then decide whether the typed semantic model belongs in Core or an optional package using these evidence gates:

- At least one concrete consumer needs charts and can state whether charting is optional.
- The proposed public types can express the required data semantics without renderer-specific concepts.
- The dependency graph is documented for Core, chart contract (if any), and each adapter.
- A real bar-chart fixture can be rendered by at least one adapter without embedding data interpretation in the adapter.
- Shared validation and adapter-capability tests demonstrate the contract is enforceable.
- Package size, versioning, compatibility, and documentation costs are compared explicitly.

If these gates show that all GUI consumers should understand chart meaning as part of the base semantic vocabulary, choose A. If chart semantics are independently optional and can evolve without burdening ordinary Core users, choose B. Record the final choice here before public API implementation.

## Minimum semantic vertical slice

Start with a **bar chart**, not a broad charting framework.

The contract should make explicit:

- chart identity and descriptive title;
- category identity and display label;
- one or more named series;
- numeric values and category/series association;
- optional units and meaningful formatting metadata;
- stable category and series ordering;
- missing-value policy and valid numeric range policy;
- accessible title/summary and a text or tabular alternative;
- adapter capability declaration and documented unsupported cases.

Validation must define behavior for empty data, duplicate identities, mismatched category/value lengths, missing values, non-finite numbers, invalid ranges, and ambiguous ordering. The contract must not silently fabricate data or coerce invalid values into plausible-looking bars.

The initial implementation should avoid committing prematurely to a full chart taxonomy, arbitrary expression language, chart-specific interaction system, or renderer-specific styling model. Grouped/stacked bars, lines, areas, scatter plots, distributions, pie/donut charts, ranges, and gauges remain follow-up design questions.

## Adapter and package responsibilities

- **GUI.Core:** owns the general renderer-neutral tree and base contracts it already defines. It must not reference Blazor, WPF, SVG implementation types, or a charting vendor.
- **Chart semantic contract (only if option B is selected):** owns typed chart meaning and validation; depends on Core only if tree integration requires it. It must not depend on renderer implementations.
- **GUI.Blazor / GUI.SVG / GUI.WPF:** each explicitly declares supported chart capabilities and owns manifestation details. One adapter's implementation does not imply support in the others.
- **Shared conformance tests:** validate common semantic invariants and fixture interpretation. Adapter tests verify actual output and intentional limitations.
- **Gallery and docs:** include a living chart example backed by the same fixture used by tests; include companion type docs, UML, and use cases under the repository documentation standard.

Do not create `Core.Charts` or `WPF.Charts` simply to mirror names. A WPF-specific package is justified only if a genuinely optional WPF implementation or dependency needs isolation. If chart support can live in the existing adapter package without compromising dependency boundaries, that is the default simpler option.

## Release boundary

Charting is future-surface work, not a hidden prerequisite for the current four-package release. Do not change package versions, promise chart support in release notes, implement this proposal as accepted public API, or publish packages based on this ADR alone. Current release readiness remains tracked separately in [Release Readiness](../RELEASE_READINESS.md) and the [Next-Release Work Queue](../NEXT_RELEASE_WORK_QUEUE.md).

## Acceptance criteria for the next implementation proposal

- [x] Inspect Core tree/builder public shape, Blazor/SVG/WPF renderer mappings, and existing shared conformance tests (bounded inspection documented above).
- [x] Record an initial chart capability matrix from inspected source.
- [x] Inspect direct project references, adapter package references, and the solution manifest; record the observed graph and its limits.
- [ ] Confirm the actual consumer and its minimum chart requirements.
- [ ] Choose option A or B with evidence and record the decision above.
- [ ] Define typed bar-chart data semantics and invalid-data behavior.
- [ ] Render the same deterministic fixture in at least one adapter.
- [ ] Add shared tests, adapter tests, accessibility/text fallback, companion docs, UML/use cases, and a living gallery example.
- [ ] Demonstrate package consumption from the built artifact if a new package is introduced.
- [ ] Keep release/version/publishing decisions separate and owner-approved.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
