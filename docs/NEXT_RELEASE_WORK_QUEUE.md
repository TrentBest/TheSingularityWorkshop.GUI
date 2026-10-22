# GUI Next-Release Work Queue

This is the execution queue for the next release of `TrentBest/TheSingularityWorkshop.GUI`. The authoritative release gate remains [Release Readiness](RELEASE_READINESS.md); this document prioritizes work and records the evidence required to call the release substantial. It does not authorize publication.

**Review branch:** `development`. **Estimated readiness: 70%.** This is a qualitative estimate, not a test score. Core, Blazor, SVG, and WPF are all in scope for the next package-family release; SVG is a centerpiece, not a reason to skip package-family validation.

**Latest green implementation evidence:** on source commit [`645573f`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/645573fce4c843a94953abbee11e46240b9f1378), [Package GUI run 39719153668](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39719153668) passed `test-and-package` and Windows `test-wpf`; `publish_nuget` was skipped. [Source-derived diagrams run 39719153678](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39719153678) and [solution-derived UML inventory run 39719153725](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39719153725) also passed on that exact source commit. The solution inventory generated 69 type pages and 69 SVGs across Core (31), Blazor (6), SVG (9), and WPF (23), but they remain workflow artifacts rather than a complete committed page set.

**Changes newer than the green baseline:** the package workflow now pins external actions to immutable commit SHAs and runs a pin-check script; the SVG UML neighborhood renderer now attempts the Workshop target-centered layout with stable identity-colored conduit ports; tests assert the consumer/target/dependency/external placement; and a manually authored GuiBuilder type page and neighborhood diagram were added. **These newer changes have not yet been verified by a fresh workflow run or rendered visual review.**

**Remaining release blockers:** full committed per-type documentation and companion UML/use-case pages; generated-output promotion and drift enforcement; fresh CI for the action pins and renderer change; visual legibility review; version/channel alignment; selected-candidate archive inspection; archive-only consumer verification; broader hostile-input tests and SVG vocabulary-to-test audit; candidate-version WebPage/Blazor integration; and final owner approval. No release candidate has been selected, and publication remains disabled.

## Definition of a substantial next release

The broader current-surface audit is tracked in [GUI Surface Coverage Audit](GUI_SURFACE_COVERAGE_AUDIT.md). It distinguishes current adapters from future candidates and turns cross-renderer semantics, interactions/accessibility, SVG boundaries, package integrity, and potential additional adapters into explicit review items. This audit is a planning artifact; it does not mark those gaps as closed.

SVG is a valid release centerpiece, but the release should deliver a dependable package family, not merely a new SVG file. The intended value is a tested, documented, reproducible path from the neutral GUI model to useful SVG output, with coherent package metadata and a real consumer proving the published artifact works.

## P0 — Decisions and hard release blockers

- [x] **Inspect the four-package artifact from run 39197481340:** [artifact](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39197481340) SHA-256 `2b538293e28c4137cb172289173d541c7ac8a83d54a421fb2174f234fb5afc42`. Core `0.1.0`, Blazor `0.1.0`, and SVG `0.1.0-alpha.1` contain `lib/net8.0/*.dll` and matching XML documentation; WPF `0.1.0` contains `lib/net8.0-windows7.0/*.dll` and matching XML documentation. All four contain `README.md`, declare MIT, and have coherent Core dependency versions. Package SHA-256: Core `45621c7e0e5a73376bce8bfb15f3420905e96a5a9240de1ece09280a815221e0`; Blazor `f130b6b753f17d0d828b98e969bd74b9446780b47ac8f235b59da9f9d16dd531`; SVG `6530819034a7102cbd7b31fc7499ff756e0b326878672cccc0941c3cc256f232`; WPF `03c1e32716d16cbda236517df7d7fe04cf3c9661fb56621db4c1494465136a07`. These are implementation artifacts, not the selected release candidate; published-version drift remains unresolved.

- [x] **Choose package scope:** Core, Blazor, SVG, and WPF are in scope for the next GUI package-family release. WPF remains an initial renderer/builder package; do not imply full parity with other adapters.
- [ ] **Choose release channel and versions** for every in-scope package. Reconcile project-file declarations with published NuGet versions; do not guess a version or infer a stable-release decision.
- [x] **Resolve license policy with the owner:** MIT is approved; the root `LICENSE` and four package license expressions are implemented in the working PR and passed the archive metadata checks in [run 39197481340](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39197481340).
- [ ] **Name the candidate commit and source branch** after the scope/version decisions; all final evidence must match that exact commit.
- [ ] **Keep publishing disabled** during readiness work; publication remains a separate explicit approval.

## P1 — Make SVG a release-quality feature

- [x] Generate the checked-in SVG feature gallery from the runnable sample and verify the artifact byte-for-byte in CI.
- [x] Run SVG Release tests and include SVG in the clean .NET 8 consumer smoke test; see the evidence linked from [Release Readiness](RELEASE_READINESS.md).
- [ ] **Inspect the actual candidate `.nupkg` and `.nuspec`:** verify the SVG README, adapter guide, generated gallery, framework assets, dependency groups, license metadata, and absence of unintended files.
- [ ] **Verify the sample using only the packed package** in a clean consumer—not repository project references or uncommitted files.
- [ ] **Audit the documented SVG vocabulary against executable tests.** Confirm each advertised element family has a representative assertion; explicitly list known viewer-dependent or unsupported behaviors rather than promising universal rendering parity.
- [ ] **Complete regression coverage for malformed/hostile-but-parseable inputs** at the serializer boundary where the renderer claims to validate them. CI verifies XML escaping, dimensions/ranges, duplicate IDs (including generated overlays), depth limits, non-finite numeric properties, and blank required `href` properties; see [run 39194940274](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39194940274). The renderer is not a sanitizer; consumer-side validation remains necessary for untrusted scene data.
- [x] **Document the external `href`/link policy:** `docs/SVG_ADAPTER.md` now lists link, image, filter-image, reusable-definition, and motion-path reference surfaces and states that values are serialized without a scheme/origin allow-list. Consumers must apply policy for their own threat model; this is not a security-sanitizer claim. Viewer-dependent resource loading/navigation remains outside the adapter's guarantee.
- [x] **Prove deterministic output** for the same input scene across repeated generation. `Escapes_xml_attribute_values_and_preserves_non_ascii_text` and `Rendering_the_same_scene_twice_produces_identical_bytes` pass in [workflow run 39185442472](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39185442472). No intentional nondeterministic output is documented.

- [x] **Make the living example traceable:** `docs/SVG_ADAPTER.md` maps visible gallery features to the sample source and representative renderer tests. The gallery remains generated from real adapter output and CI verifies the checked-in SVG byte-for-byte; the mapping is representative, not a claim that every SVG vocabulary item is exhaustively tested. Evidence: [gallery-map workflow run 39279222565](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39279222565) and [latest gallery/package verification run 39316832283](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39316832283).

## P2 — Package family integrity

- [x] Core, Blazor, and SVG Release tests plus shared conformance tests passed in the recorded baseline workflow.
- [x] Windows-native WPF tests passed in the recorded baseline workflow; this does not itself mean WPF is packaged for the candidate.
- [x] Add reusable native WPF numeric and enum value-control builders with STA tests for initial state, change callbacks, and invalid inputs; verified by [workflow run 39277269099](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39277269099). These controls expand the current WPF surface but do not imply complete adapter parity.
- [x] Add STA regression tests for native-control system defaults, fluent style/layout overrides, builder identity, and repeatable stack-panel builds; [Windows workflow run 39195324527](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39195324527) passed. See [WPF Builder Pattern Audit](WPF_BUILDER_PATTERN_AUDIT.md) for patterns extracted from other Workshop WPF builders and boundaries on what should remain optional or host-specific.
- [x] Verify Grid splitter/track mapping, Canvas positioning, and nested-component parent propagation; [Windows workflow run 39195444173](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39195444173) passed after a compile-time assertion correction. Reusable window-host builder tests remain in a later run.
- [ ] Audit public APIs and XML documentation for every in-scope package; build Release with no unexplained warnings/errors.
- [ ] **Inventory every production type across Core, Blazor, SVG, and WPF** and map each fully qualified type/source file to its own documentation page and UML SVG. Use [Per-Type Documentation and UML Standard](CLASS_DOCUMENTATION_STANDARD.md); XML API comments alone do not satisfy this requirement.
- [ ] **Create one dedicated Markdown page per declared type**, including internal and nested maintained types, with source link, responsibility/boundaries, member contracts, verified relationships, behavior/failure modes, and relevant tests.
- [ ] **Embed each type's UML class-diagram SVG on its page**; include inheritance, implemented interfaces, and source-grounded relationships, plus accessible title/description and a textual fallback. Do not use decorative or unverified diagrams.
- [ ] **Add a central type inventory/index and CI drift checks** so every new or existing type must have a page and diagram; validate links, SVG accessibility metadata, and a deliberately missing-page/diagram failure case.
- [ ] **Review generated diagram accuracy against source and tests** and record exact commit/workflow evidence before marking this requirement complete.
- [ ] Inspect all candidate package archives and dependency graphs; ensure package versions and Blazor-to-Core dependencies agree.
- [x] Wire WPF into the package workflow and verify its package README, tags, repository metadata, MIT license expression, and archive README presence in [workflow run 39197481340](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39197481340). The same run passed the clean Core/Blazor/SVG consumer smoke test and Windows WPF tests; `publish_nuget` was skipped.
- [ ] Confirm Core remains platform-neutral and has no accidental UI-framework dependency.
- [ ] Record test counts, exact workflow URL, commit SHA, package filenames, and clean-consumer result.

## P3 — Consumer proof and documentation alignment

- [x] WebPage repository-source integration passed on commit [`6f1d710`](https://github.com/TrentBest/WebPage/commit/6f1d710bf61285ab2536b2a5406c6de78914c234) in [run 39274438190](https://github.com/TrentBest/WebPage/actions/runs/39274438190); its source integration job built WebPage and passed the live Renderer smoke test. The separate WebPage test job failed 6 of 829 `SingularityHub.Tests` tests because experience-manifest responses supplied `text/plain` instead of the required JSON content type (the run log reports `InvalidDataException`). This is a WebPage test blocker, not evidence that GUI packages caused the failure; candidate-version NuGet integration remains unverified.
- [ ] Re-run WebPage against the **selected candidate package versions**; baseline integration is not candidate-version proof.
- [ ] Demonstrate the documented Blazor renderer path in a real consumer host and retain representative evidence.
- [ ] Align README, Start Here, architecture/model/adapter/theory guides, roadmap, and package READMEs with the behavior actually implemented and tested.
- [ ] Ensure every advertised living image is generated from real source output or clearly labeled as a conceptual reference. Verify local links and package-included documentation paths.
- [ ] Publish release notes that separate shipped behavior, known limitations, compatibility changes, and future theory/roadmap items.

## P4 — Candidate review and release decision

- [ ] Full CI passes on the exact candidate commit, including Windows WPF checks if WPF is in scope.
- [ ] Candidate archives are inspected and clean-consumer-tested; evidence points to those exact artifacts.
- [ ] All P0 decisions and hard gates in [Release Readiness](RELEASE_READINESS.md) are resolved.
- [ ] Complete the release decision record in [Release Readiness](RELEASE_READINESS.md).
- [ ] Obtain explicit approval before changing the publish guard or publishing anything.
- [ ] After an authorized publication, verify the exact package IDs and versions on NuGet.org and record the result.

## Current blockers / owner decisions

1. Release channel and per-package versions, including dependency version alignment.
2. Select the exact candidate commit and source branch.
3. Inspect the candidate four-package archive set and run clean-consumer verification against those exact versions.
4. Candidate-version WebPage integration.
5. Complete the remaining SVG hostile-input regression coverage and candidate sign-off; the external-reference serialization policy is documented, but the renderer intentionally does not sanitize or allow-list URLs.\n6. Complete per-type documentation and UML SVG coverage across all four in-scope packages, with a source-derived inventory and CI enforcement.

These are intentionally explicit blockers rather than decisions an implementation agent should silently make. Update the readiness percentage only when evidence changes; checked boxes alone do not equal release readiness.
