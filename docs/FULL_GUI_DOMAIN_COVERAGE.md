<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# Full GUI Domain Coverage Map

**Purpose:** maintain one evidence-oriented map from the Workshop's expansive GUI domain to implemented contracts, adapter manifestations, tests, and documentation. This is an engineering control document, not a claim that every domain capability is shipped.

**Current target:** `development`. This document began as a draft on `docs/full-gui-domain-coverage-2026-10-23` and has been reconciled as a starting inventory, not yet accepted as a complete domain definition.  
**Release status:** BLOCKED. The previously recorded 80% readiness estimate from 2026-10-22 is stale and is not a current release decision. See [Release Readiness](RELEASE_READINESS.md) for the explicit domain-definition gate.

## Governing boundary

The GUI is the extrinsic observation and interaction boundary around an experience. It does not own the experience manifest, domain state, or FSM lifecycle. A manifest describes the experience's MicroBundles/configuration; runtime composition belongs to FSM_COS; GUI observes, accepts semantic input, and manifests a view through an adapter.

The shared Workshop standard is [FSM_COS Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/DOCUMENTATION_STANDARD.md), supplemented by [FSM_COS Living UML and Data-Card Visual Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/docs/UML_VISUAL_STYLE.md) and this repository's [UML-Inspired Data Cards](DATA_CARDS_AND_UML.md). Each implementation claim must point to source and executable evidence. A concept being specified is not the same as a supported API.

## Domain-definition gate — OPEN

This map is an inventory and evidence ledger; its existence does **not** mean the GUI domain is fully defined. No release candidate or package version/channel may be selected until the owner accepts the domain definition and its documentation. The goal is a complete, testable statement of what GUI means and owns—not an assertion that every future capability must be implemented before release.

The domain definition must make these boundaries explicit:

1. **Semantic intent:** the platform-neutral GUI tree, declaration vocabulary, identity, containment/order, properties, styles, and extension points.
2. **Observation and input:** what it means to observe domain/runtime state, what semantic input/actions are represented, and where commands cross into the authoritative domain. The GUI is not the source of truth for the experience.
3. **Lifecycle and composition:** creation, update, replacement, multiple observers, suspension/disposal, and the boundary with FSM_API/FSM_COS and experience manifests. GUI must not duplicate the composition/runtime owner.
4. **Representation space:** conventional controls and information, 2D/spatial views, 3D/immersive intent, expressive media, charts, animation, and future extensions. For each, distinguish a semantic concept from a shipped API.
5. **Manifestation/adapter contract:** Core owns neutral semantics; Blazor/WPF/SVG each declare supported, unsupported, partial, and unverified behavior. GUI.SVG owns semantic GUI-tree-to-SVG interoperability; general visual-scene SVG ownership belongs to Visualize. Renderer owns observer-facing representation/visibility decisions, not GUI serialization or domain composition.
6. **Host and safety boundaries:** accessibility, keyboard/focus, event routing, native/browser/Markdown limitations, external references/resources, and what serialization does not guarantee. Host behavior must not be inferred from a static artifact.
7. **Evidence and learning path:** each capability has an owner, non-goals, status, public types/source, tests, a runnable example or consumer where applicable, failure/alternative cases, limitations, and next evidence needed. Theory chapters trace intuition → concepts → reasoning → implementation → evidence/limits → counterexample/open question → practice/further reading.
8. **Documentation conformance:** every Markdown page has one Workshop moniker at the top and the standard Workshop footer at the end, with no duplicate moniker; links and badges resolve; authored and generated pages retain accurate evidence boundaries; the README keeps the canonical five-section entry path.

Every capability must be classified as **implemented/tested**, **implemented/partial evidence**, **specified**, **proposed**, **unverified**, or **owned elsewhere/not in this package**. These categories are not interchangeable. A concept can be in the domain without being implemented; a generated type inventory does not infer user intent; a green build does not close a conceptual boundary; and a visual does not prove runtime behavior.

## Open domain decisions that block acceptance

- **UML member grouping and static semantics:** the standing GUI direction calls for the member display order Public → Static → Protected → Private. The current FSM_COS visual standard says public contract first with protected/private detail subordinate, while draft PRs #50/#57 implement Public → Protected → Internal → Private and treat `static` as a modifier. These are not yet a single normative rule. Decide how static members are grouped without erasing their actual language accessibility; then align the semantic model, renderer, tests, examples, and documentation. Until then, the nested data-card prototype is valuable work, but not the accepted standard.
- **Charting contract:** chart kinds, axes, units, missing values, accessibility summaries, and adapter support remain proposed. PR #43's renderer-neutral boundary analysis must be reconciled before a public chart API is claimed.
- **Cross-adapter parity:** define capability-by-capability behavior rather than promising pixel parity. Blazor candidate-version/WebPage integration and the Windows archive-only WPF consumer remain separate evidence gates.

**Documentation conformance is not yet complete repo-wide.** The README, release gate, coverage map, newly integrated adapter contracts, and refreshed generated UML pages now follow the moniker/footer standard, but legacy pages such as `docs/TESTING.md`, `docs/THEORY_GUIDE.md`, `docs/CLASS_DOCUMENTATION_STANDARD.md`, and `docs/GUI_TAXONOMY.md` still need audit and correction. Do not claim the repository fully conforms until the complete Markdown inventory is checked and the remaining pages are corrected.

The review path is tracked by [issue #63](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/63). The existing [GUI domain-map draft PR #40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40) supplied the initial matrix, but its branch statuses and evidence references must be reconciled against current source/tests. Owner acceptance is required to close this gate.

## Status vocabulary

- **Implemented / tested** — the contract exists in source and the named behavior has executable coverage.
- **Implemented / partial evidence** — code exists, but scope or hostile/boundary behavior is not fully verified.
- **Specified** — the intended semantic distinction is documented; implementation is not implied.
- **Proposed** — contract, API, or runtime behavior still needs a decision and tests.
- **Unverified** — source or integration must be inspected before a reliable status can be assigned.
- **Not in this package** — the responsibility belongs to a host, FSM_COS, a domain adapter, or another package.

## Domain-to-evidence matrix

| Domain | Intended responsibility | Current evidence/status | Required closure evidence |
|---|---|---|---|
| Neutral recursive model | Represent GUI intent without binding Core to a platform | Core `GuiNode` / builders exist; Core test project and generated type inventory exist. Full platform-neutral dependency/API audit remains open. | Public API inventory, dependency audit, invariants and invalid-input tests, consumer example from packed Core package. |
| Composition and identity | Parent/child containment, stable node identity, sibling order | Recursive model and builder examples exist. SVG root identity fix is in open PR #34; don't treat it as part of `development` until merged. | Cross-adapter tree/order/identity matrix; duplicate and generated-ID collision tests; explicit null/invalid-node policy. |
| Properties and styling | Typed or documented semantic properties, text, color, size, layout | Current string/property vocabulary is represented in adapters; no claim of universal typed property parity. | Source-derived property inventory; supported/unsupported/unverified mapping by adapter; valid, malformed, missing, and out-of-range tests. |
| Input intent and interaction | Activate, select, cancel, navigate, point, drag, zoom, rotate, text input, gesture, gaze, voice | Device-independent input intent is specified in [GUI Expressiveness](GUI_EXPRESSIVENESS.md); do not assume a common event/action runtime is implemented. | Define semantic event/action contracts; distinguish command request from domain mutation; test keyboard/pointer and host-mediated actions in real hosts. |
| State observation | Show domain/runtime state without making the renderer authoritative | Extrinsic observation boundary is documented in [Ideal GUI](IDEAL_GUI.md) and [GUI Lifecycle](GUI_LIFECYCLE.md). | Host-backed example with a real source of state; prove update propagation and that GUI state does not become domain state. |
| 2D spatial representation | Surface, region, element, viewport, transforms, layers, focus and selection | Domain is specified in [GUI Spatial Domain](GUI_SPATIAL_DOMAIN.md); coordinate primitives exist, but the full semantic transform/identity contract remains proposed. | Contract for coordinate spaces, transform composition/inversion/invalid values, identity across movement, and deterministic adapter mappings. |
| 3D and immersive representation | Volume, camera, projection, depth, occlusion, world-space controls | Vocabulary and desired domain are documented; not a claim of a shipped 3D renderer or GPU pipeline. | Separate semantic model from graphics API; define camera/projection/visibility contracts; executable reference scene and adapter-specific capability tests. |
| Expressive media | Images, geometry, paths, symbols, charts, animation, video, audio-linked visualization, particles, noise/static and empty space | [GUI Expressiveness](GUI_EXPRESSIVENESS.md) defines the open representation space. SVG has explicit serialization vocabulary and generated gallery; SVG is not a general media engine. | For each media family, record Core representation, adapter support, output limitations, resource policy, test fixture, and runnable recipe. |
| Data visualization and charting | Express quantitative data as bars, lines, areas, points, distributions, proportions, ranges, and time series without embedding renderer-specific drawing instructions in the data contract | **Proposed domain expansion.** No chart API or chart package is asserted by this map. Existing SVG/Blazor/WPF support must be inspected before claiming native chart support. | Define renderer-neutral chart semantics and data validation first; then implement one vertical slice and prove each adapter's support/limits with shared fixtures, accessibility text, and runnable examples. |
| SVG manifestation | Portable vector output for supported nodes/scenes | SVG adapter, generated gallery and vocabulary-to-test map exist. Host/viewer behavior and destination authorization are not proven by XML serialization tests. | Candidate archive-only consumer; malformed/hostile input regression tests; clear external-reference policy; deterministic generated-gallery check. |
| Blazor manifestation | Render semantic model in a browser-hosted .NET component | Adapter and component tests exist. Candidate-version real consumer/WebPage integration remains open. | Minimal host renders reusable semantic builders from the selected packed package; assert actual output/interaction rather than merely restore/compile. |
| WPF manifestation | Translate supported semantic model into Windows-native controls | Windows tests now cover nested identity/order, Row/Column composition and orientation, dimensions/spacing/colors, host-owned `resource:` theme keys, `isEnabled`, and explicit command dispatch. This remains a capability-specific surface, not full adapter parity. | Keep a per-capability matrix for unsupported properties, lifecycle, accessibility/keyboard/focus, image/resource failures, and native-only behavior; run the clean archive-only consumer on Windows. |
| Shared adapter conformance | Identify shared invariants without forcing pixel parity | Initial shared conformance project exists; broader matrix remains open in [Surface Coverage Audit](GUI_SURFACE_COVERAGE_AUDIT.md). | Every contract row names shared test, adapter-specific result, and explicit N/A/unsupported/unverified status. |
| Data cards and source-derived UML | Explain source structure and relationships with recursive, accessible cards | Compiler-backed inventory and SVGs exist for 69 types. Authored purpose/use-case documentation remains incomplete; static SVGs are not interactive. | Per-type companion, focused `[TypeName]UML.md`, source-grounded use cases, accessible text fallback, stale/missing-output CI failure fixtures. |
| Interactive diagram editing | Edit an authoritative domain/AST through validated commands, not by mutating an SVG projection | Specified in [Data Cards and UML](DATA_CARDS_AND_UML.md); not a completed interactive AST editor. | Authoritative model, command authorization/validation, failure diagnostics, stable selection, regeneration and round-trip tests. |
| Execution host and lifecycle | Resolve/execute an experience, route input/data, suspend/resume/dispose | [GUI Execution Model](GUI_EXECUTION_MODEL.md) is explicitly proposed. This is not currently a GUI package runtime contract. | Define ownership across WebApp/AnyApp/FSM_COS; do not duplicate FSM_COS lifecycle; prove with an actual host integration before claiming implementation. |
| Manifest integration | Keep experience composition/configuration separate from GUI implementation | Boundary is specified in [Manifest Boundary](MANIFEST_BOUNDARY.md); manifest loading is not implied by a GUI renderer package. | End-to-end fixture from manifest to FSM_COS composition to observable GUI output, with invalid manifest/dependency failure tests in the host integration repo. |
| Package/consumer integrity | Ensure each package works without repository-only source and includes accurate docs | Core, Blazor, SVG and WPF are in release scope; recorded package workflow evidence exists, but exact candidate/version decision is open. | Select versions/channel, inspect all candidate archives, run clean archive-only consumers and candidate WebPage integration, retain exact commit/run evidence. |

## Required implementation order

This sequence is designed to make progress without confusing architectural ambition with shipped support.

1. **Close the package contract.** Complete the per-adapter behavior matrix and source/API audit. Fix correctness and hostile-input gaps that affect the currently advertised API.
2. **Close consumer documentation.** Finish authored per-type contracts and companion UML/use cases; keep generated source structure distinct from human-authored use cases and runtime evidence.
3. **Prove the package boundary.** Test the actual packed archives in clean consumers; prove Blazor in a real host and WPF on Windows. Keep NuGet publication gated.
4. **Turn specified semantics into small executable contracts.** Prioritize identity, coordinate transforms, semantic input, accessibility/focus, and lifecycle only when a clear owner and consumer exist.
5. **Expand manifestations by capability, not by label.** SVG, Blazor, WPF, and any future GPU/WebGPU/Vulkan adapter each get a support matrix and representative runnable proof. A new adapter must not silently promise parity.
6. **Integrate with FSM_COS/WebApp.** FSM_COS owns experience composition; WebApp owns web hosting/inputs; GUI owns semantic visualization/interaction representation. Verify the end-to-end route in those repositories without creating a second runtime meaning inside GUI.

## Documentation acceptance contract

For each maintained production type, the Workshop standard requires:

- A companion Markdown page identifying the user moniker, purpose, source, responsibility/non-goals, relevant contract and failure behavior.
- A focused `docs/uml/[TypeName]UML.md` neighborhood diagram page with a **Use cases** section, grounded consumers above, target centered, dependencies below, and verified package references to the right.
- A source-derived UML SVG with provenance and accessible title/description, plus text explanation. Generated output is regenerated, not hand-edited.
- Tests and usage links that support the claims; unknown behavior is explicitly marked rather than inferred.
- CI coverage inventory and deliberate negative tests proving missing documentation/diagrams fail.

Use the exact shared palette and nested data-card rules in the FSM_COS standard. Keep source structure, use-case intent, and captured runtime behavior as separate evidence classes.

## Release evidence rule

The readiness percentage changes only when implementation or documentation work is backed by evidence on a named commit. The old 80% estimate is stale; do not use it as a release decision. This map itself does not close the domain-definition gate. Update [Release Readiness](RELEASE_READINESS.md), [Next-Release Work Queue](NEXT_RELEASE_WORK_QUEUE.md), and [Surface Coverage Audit](GUI_SURFACE_COVERAGE_AUDIT.md) when a gate actually closes, and include exact workflow links. Publication, merge, deployment, and branch deletion remain separate authorized actions.


## Charting and quantitative visualization: proposed contract

Charts are a first-class GUI domain, not merely a collection of renderer-specific widgets. The shared model should describe **what the data means and how it should be interpreted**; each renderer decides how to manifest that meaning within its documented capabilities.

### Initial chart families

- **Categorical comparison:** vertical/horizontal bar and grouped/stacked bar charts.
- **Trends:** line and area charts, including time-series axes and missing samples.
- **Proportions:** pie and donut charts, with explicit rules for negative values, zero totals, and categories too small to label.
- **Relationships and distributions:** scatter plots, histograms, and related point/bin representations.
- **Ranges and status:** range/band charts and gauges only after their semantics are specified.

This is a scope vocabulary, **not a claim that these chart types already exist in Core or any adapter**. Start with a small, testable subset—bar, line, and pie/donut—rather than trying to implement every family at once.

### Architectural recommendation

1. **Core semantic contract first.** Model chart kind, series, data points, category/time/numeric axes, labels, units, legends, value formatting, missing values, and accessibility descriptions. Validate malformed data and unsupported combinations deterministically. Keep Core independent of WPF, Blazor, SVG, JavaScript chart libraries, and graphics APIs.
2. **Separate data from appearance.** Keep values and series identity distinct from colors, typography, gridlines, labels, and layout hints. Define defaults and precedence; avoid turning arbitrary renderer property strings into the only chart contract.
3. **Adapter capability is explicit.** An adapter can support a chart natively, translate a limited subset, or report it unsupported. Do not silently flatten semantic distinctions or claim parity because the same model compiles.
4. **Package split follows dependency evidence.** Do not create `Core.Charts` or `WPF.Charts` solely because the names sound tidy. First determine whether the chart model is a coherent extension of Core or a separately versioned optional package. If a WPF-specific implementation adds value, keep it in a WPF adapter package and reuse the neutral model. SVG and Blazor should consume the same contract where feasible.
5. **One vertical slice proves the architecture.** Add a representative bar chart with deterministic data; test valid and invalid inputs, labels/units, empty data, missing values, ordering, and accessible textual summary. Render it through at least one existing adapter, then add SVG/Blazor/WPF support based on actual adapter boundaries.
6. **Avoid accidental data semantics.** Specify aggregation, sorting, normalization, stacking, and time-zone treatment only when the chart contract requires them. A renderer should not invent domain calculations or mutate the source data.

### Charting acceptance evidence

A chart capability is considered implemented only when its source contract, tests, adapter mapping, and runnable example agree. Each chart family should record: supported data shapes; axis and value rules; empty/invalid behavior; accessibility fallback; interactive behavior (hover, selection, zoom, keyboard navigation) if implemented; renderer limitations; and package/consumer evidence. Static gallery imagery alone is not sufficient.

## Cross-repository integration as a CiAi opportunity

The same contract-first approach can make repository integration more systematic. A future CiAi integration workflow could consume explicit, versioned capability metadata from each repository: package identity and version, public contract, dependency direction, supported adapters, compatibility constraints, sample/consumer entry points, test evidence, and known limitations. It could then propose an integration plan and generate a verification matrix across repositories.

This should be an **evidence-producing assistant, not an authority that guesses compatibility**. It must distinguish declared capability from observed test results, link claims to source/CI evidence, flag version conflicts, and ask for human approval before changing dependencies, merging, publishing, or deploying. Begin with a small machine-readable capability manifest and a CI check that verifies its package names and links against the repository; do not make every repository depend on a new central service to get started.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
