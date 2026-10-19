# GUI Surface Coverage Audit

**Purpose:** make renderer/package boundaries and unverified surface area explicit so release scope is deliberate and future adapters do not silently expand the compatibility promise.

**Status:** audit plan, not a claim that every capability exists or has been tested. Classify findings as observed, needs verification, or future option. Related documents: [Next-Release Work Queue](NEXT_RELEASE_WORK_QUEUE.md) · [Release Readiness](RELEASE_READINESS.md).

## 1. Current renderer and package map

| Surface | Current role | Known evidence / caveat | Next-release treatment |
|---|---|---|---|
| GUI.Core | Neutral GUI model and shared contracts | Release tests and shared conformance tests passed in recorded baseline CI. Platform neutrality still needs an explicit dependency/API audit. | Candidate scope; verify archive and neutral boundary. |
| GUI.Blazor | Blazor-facing renderer/adapter | Release tests and a clean local-package consumer passed. Real consumer-host demonstration and candidate-version WebPage run remain open. | Candidate scope; prove the documented path in a host. |
| GUI.SVG | Portable vector output | Release tests, generated-gallery byte comparison, clean consumer using locally packed packages, XML-attribute escaping/Unicode round-trip, and repeat-render byte determinism passed in [workflow run 39185442472](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39185442472). Archive-only consumption, broader hostile-input coverage, and external-reference policy remain open. | Candidate scope; likely headline feature, limitations explicit. |
| GUI.WPF | Windows-native rendering surface | Windows tests and WPF package build/archive checks passed in [workflow run 39199107981](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39199107981); the workflow now includes WPF packaging and a dedicated Windows test lane. This does not establish full adapter parity. | Candidate scope; verify the selected candidate archive and document adapter-specific behavior. |
| WebPage consumer | Downstream integration / proof application | Existing NuGet-mode and repository-source CI passed for the configured dependency set, not a future GUI candidate. | Consumer proof, not another renderer package. Re-test candidate versions. |

Do not infer full parity from multiple adapters. A renderer can be useful and deliberately partial; document and test its actual behavior and limitations.

**SVG verification update (2026-10-19):** workflow run [39185442472](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39185442472), commit [`1ca6c07`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1ca6c07bbd96d837dc6884e238236f49ff06f7f0), passed Core/Blazor/SVG tests, shared adapter conformance, generated-gallery byte comparison, package documentation/metadata checks, and clean consumption of locally packed Core/Blazor/SVG packages; Windows WPF tests also passed. New SVG tests prove XML attribute escaping with Unicode round-trip and deterministic bytes when rendering the same scene twice. This does **not** prove archive-only package consumption or define a security policy for external `href` values; those remain open release checks. The publish job was skipped as intended.

## 2. Cross-renderer semantic contract

Build a conformance matrix naming each contract, test location, each adapter result, and intentional exceptions. Mark each item supported, unsupported, or unverified; never leave ambiguous blanks.

| Contract area | Questions requiring answers | Minimum evidence |
|---|---|---|
| Tree and order | Are parent/child relationships and sibling order preserved? What happens for null children or invalid nodes? | Shared nested-tree and order tests; deviations documented. |
| Identity | How are IDs assigned? What happens for missing, duplicate, or generated IDs? Can overlays collide with user IDs? | Duplicate-ID and generated-ID tests per adapter; defined policy. |
| Properties | Which names and value types are recognized? What happens to unknown, null, malformed, or out-of-range values? | Property vocabulary mapped to source and executable tests. |
| Dimensions/layout | Units, width/height bounds, margins, padding, alignment, overflow, clipping, negative values? | Boundary tests and per-adapter notes; no promise of pixel-identical layout. |
| Text/typography | Text source, whitespace, wrapping, font fallback, font-size ranges, alignment? | Representative tests and platform/viewer caveats. |
| Color/styling | Supported formats, invalid-color fallback, opacity, borders, fills, themes? | Positive and invalid-value tests for advertised formats. |
| Images/links | URI schemes, missing resources, external references, semantic text, eager/lazy loading? | Resource tests and explicit external-reference policy. |
| Depth/size | Maximum tree depth, node count/output limits, boundary behavior? | Exact boundary and over-limit tests. |
| Failure behavior | Which errors surface, are ignored, fall back, or logged? | Tests for chosen behavior and useful diagnostics. |
| Platform boundary | Which projects/frameworks may Core reference? Which APIs are adapter-specific? | Dependency audit and clean builds for supported targets. |

### Contract completion checklist
- [ ] Inventory actual public node/property vocabulary from source, not memory.
- [ ] Map each documented property and node family to executable tests.
- [ ] Identify shared tests for shared invariants, or explain why a test is adapter-specific.
- [ ] Record Core, Blazor, SVG, and WPF results separately; distinguish not applicable from not tested.
- [ ] Document intentional differences rather than forcing artificial pixel-level parity.
- [ ] Link the matrix and evidence from the release checklist.

## 3. Interaction, lifecycle, and semantics

A rendered visual tree is not automatically an interactive application. Separate static representation from runtime behavior.
- [ ] Inventory whether the model represents actions, events, input values, checked/selected state, disabled state, focus, keyboard intent, and lifecycle/disposal.
- [ ] For each represented interaction, record whether each adapter implements, ignores, or does not model it.
- [ ] Test callbacks and state changes in a real host where required; snapshots/string-output tests alone are insufficient.
- [ ] Define keyboard and focus expectations before claiming keyboard accessibility.
- [ ] Define how semantic names, roles, descriptions, and disabled state map per adapter.
- [ ] Do not claim accessibility conformance without a named standard, scope, and appropriate automated/manual evidence.
- [ ] Define lifecycle/disposal only where resources or subscriptions exist; do not invent a fake lifecycle for symmetry.

## 4. Layout, styling, and localization
- [ ] Document the supported layout model and distinguish model intent from native layout-engine behavior.
- [ ] Define whether dimensions are unitless, CSS units, device-independent units, or adapter-specific; do not silently interpret the same string differently.
- [ ] Cover overflow, clipping, wrapping, font fallback, and unavailable fonts.
- [ ] Decide whether shared design tokens/themes belong in the neutral model.
- [ ] Identify localization requirements: resource ownership, culture-sensitive formatting, text direction, and right-to-left layout.
- [ ] Use visual regression evidence only in controlled environments; use semantic tests for cross-platform contracts.

## 5. SVG output and input boundaries
- [ ] Inspect the actual packed .nupkg and .nuspec: README, adapter guide, gallery, target assets, dependencies, license metadata, and unintended files.
- [ ] Demonstrate the sample from a clean consumer using only packed package artifacts.
- [ ] Prove repeated generation from the same model produces identical bytes, or document intentional nondeterminism.
- [ ] Map each gallery example to its source builder and a test proving the relevant behavior.
- [ ] Test XML escaping for text and attributes, including quotes, ampersands, angle brackets, and non-ASCII text.
- [ ] Test dimensions and numeric values at normal, boundary, invalid, non-finite, and extreme inputs where applicable.
- [ ] Define duplicate-ID behavior, including generated overlay/auxiliary IDs.
- [ ] Test maximum-depth behavior at and beyond the documented limit.
- [ ] Decide and test external href/link policy. XML escaping alone is not URL validation or sanitization.
- **Observed implementation detail (2026-10-19):** `SvgGuiRenderer` currently copies `svg-a` `href` values directly into the SVG anchor, and writes image `href` from the node's `href` property or `Source`. It requires a non-empty anchor destination but does not validate URL schemes. SVG resource-reference attributes are also copied for features such as gradients and motion paths. This is a documented open policy decision, not evidence that output is sanitized; define per-element rules before claiming hostile-input safety.
- [ ] State whether image/resource references are preserved, restricted, rewritten, or omitted. Do not claim sanitizer behavior without a separate tested allow-list contract.
- [ ] Document viewer-dependent features and unsupported SVG vocabulary instead of promising universal viewer parity.

## 6. Package and consumer integrity
- [ ] Resolve package scope, release channel, versions, license policy, and candidate commit before calling the package set ready.
- [ ] Inspect actual archives, not only project files; compare nuspec, README inclusion, framework assets, dependencies, and intended file lists.
- [ ] Confirm package dependency versions align, especially Blazor to Core.
- [ ] Prove Core has no accidental UI-framework dependencies.
- [ ] Prove each included package restores and works in a clean consumer without sibling project references.
- [ ] Demonstrate Blazor in a real host and test WebPage against selected candidate versions.
- [ ] If WPF ships, package and inspect it in the same candidate; if not, record the deferral.
- [ ] Record workflow URL, commit SHA, test counts, warnings, package filenames, and consumer result for the exact candidate.
- [ ] Keep publication guarded until review and explicit owner approval.

## 7. Serialization, compatibility, extensibility

These are design questions, not assumed features of the current release.
- [ ] Decide whether the neutral model needs a stable serialized form. If yes, specify schema/versioning, round-trip guarantees, unknown-field handling, and compatibility tests.
- [ ] Decide whether adapters need capability reporting. If yes, let consumers discover support without guessing from package names.
- [ ] Define extension points for adapter-specific properties without leaking framework types into Core.
- [ ] Specify compatibility expectations for model nodes, property names, and serialized data if they become public contracts.
- [ ] Avoid a general plugin/registry mechanism unless a real consumer or test justifies it.

## 8. Performance and resource boundaries
- [ ] Identify realistic large-tree workloads and measure render/serialization time and allocations before making performance claims.
- [ ] Decide whether node-count, output-byte, depth, image, or external-resource limits are necessary.
- [ ] Ensure rendering does not repeat whole-tree validation for every descendant; add regression/performance evidence where relevant.
- [ ] Keep benchmarks separate from correctness tests and document runtime/hardware context.
- [ ] Define cancellation or streaming only if real consumers need it; do not make speculative APIs release blockers.

## 9. Potential future adapters and adjacent surfaces

These are future options, not current package commitments or release blockers unless the owner selects them.

| Candidate | Why it might matter | Gate before adding it |
|---|---|---|
| Avalonia | Cross-platform desktop UI. | Concrete consumer need, target-framework policy, mapping, conformance suite. |
| .NET MAUI | Mobile and cross-platform application UI. | Define supported platforms/native behavior and prove a real host. |
| WinUI | Windows-native surface distinct from WPF. | Show a meaningful consumer need beyond existing WPF coverage. |
| Unity | Game/interactive spatial host; relevant as a consumer, but keep Unity types out of Core. | Separate adapter package and real Unity-host tests. |
| WinForms | Existing Windows desktop ecosystem and simpler control model. | Identify useful coverage and limitations before committing. |
| Console/terminal | Diagnostics, lightweight tools, text-oriented workflows. | Define a text/interaction projection rather than pretending it has GUI layout parity. |
| Native mobile adapters | Platform-specific features beyond MAUI abstractions. | Demonstrated platform requirement. |
| Spatial/XR presentation | Scene-oriented or spatial presentation. | Keep spatial semantics distinct from ordinary 2D layout and prove a real consumer. |
| Design-time/editor surface | More inspectable model construction and documentation. | Reuse the neutral model; avoid a parallel undocumented model. |

Stabilize and test the neutral contract first. The existence of a plausible framework is not a reason to add dependencies.

## 10. Priorities

### P0 — Decisions that block a candidate
Package scope (especially WPF), channel and versions, license policy, exact candidate commit, and continued publication guard.

### P1 — Evidence for a credible SVG-centered release
Actual archive inspection and packed-package-only consumer proof; SVG boundary tests and deterministic-output proof; source-to-test links for the generated gallery; public API/XML documentation and explicit limitations.

### P2 — Renderer family and consumer confidence
Shared conformance matrix across current adapters; Blazor real-host proof; candidate-version WebPage integration; package metadata/dependency audit and consistent documentation.

### P3 — Roadmap after contract clarity
Serialization/versioning, capability negotiation, themes/design tokens, localization, expanded accessibility/interaction, performance budgets, and additional adapters, prioritized by actual consumer needs.

## 11. Evidence log

Update with exact links and candidate commit identifiers as work proceeds; do not replace evidence with an unqualified 'passes.'

| Evidence | Status | Link / note |
|---|---|---|
| Baseline GUI package workflow | Passed for recorded baseline | [Run 39180369067](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39180369067): Core/Blazor/SVG Release tests, shared conformance, link check, gallery comparison, clean .NET 8 consumer, Windows WPF tests; publish skipped. |
| Recent WPF margin/padding regression | Passed | [Run 39184142092](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39184142092) on [53560b6](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/53560b6e9ba96857f307f2358a0076f9d7038252). |
| WPF negative-margin implementation | Passed | [Run 39184130479](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39184130479) on [83e1e79](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/83e1e7908e43babbe5072a714e79ad48adf22680). |
| WebPage current-dependency integration | Passed; candidate-version proof open | [Run 39180999142](https://github.com/TrentBest/WebPage/actions/runs/39180999142) on [217cc4d](https://github.com/TrentBest/WebPage/commit/217cc4dbc0312bc0ec005293cb70d29b961a5e2c). |
| Candidate package archive inspection | Open | Must be performed against selected release artifacts. |
| Candidate-version WebPage integration | Open | Must wait for package/version decision. |

**Readiness estimate remains 65%.** This audit clarifies missing coverage but does not itself close release gates or authorize publication.
