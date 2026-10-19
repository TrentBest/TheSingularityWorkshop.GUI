# GUI Next-Release Work Queue

This is the execution queue for the next release of `TrentBest/TheSingularityWorkshop.GUI`. The authoritative release gate remains [Release Readiness](RELEASE_READINESS.md); this document prioritizes work and records the evidence required to call the release substantial. It does not authorize publication.

**Baseline:** `development`  
**Estimated readiness:** **65%** — substantial CI and SVG artifact work is already verified, but the candidate package set is not yet release-ready.  
**Publication:** Keep the workflow's explicit `&& false` publish guard unchanged until the release candidate is reviewed and explicitly approved.

## Definition of a substantial next release

The broader current-surface audit is tracked in [GUI Surface Coverage Audit](GUI_SURFACE_COVERAGE_AUDIT.md). It distinguishes current adapters from future candidates and turns cross-renderer semantics, interactions/accessibility, SVG boundaries, package integrity, and potential additional adapters into explicit review items. This audit is a planning artifact; it does not mark those gaps as closed.

SVG is a valid release centerpiece, but the release should deliver a dependable package family, not merely a new SVG file. The intended value is a tested, documented, reproducible path from the neutral GUI model to useful SVG output, with coherent package metadata and a real consumer proving the published artifact works.

## P0 — Decisions and hard release blockers

- [ ] **Choose package scope:** Core, Blazor, SVG, and whether WPF ships in this release. If WPF is deferred, state that explicitly and do not imply it is part of the candidate package set.
- [ ] **Choose release channel and versions** for every in-scope package. Reconcile project-file declarations with published NuGet versions; do not guess a version or infer a stable-release decision.
- [ ] **Resolve license policy with the owner.** Add the authoritative repository license and align package license metadata only after the intended license is confirmed.
- [ ] **Name the candidate commit and source branch** after the scope/version decisions; all final evidence must match that exact commit.
- [ ] **Keep publishing disabled** during readiness work; publication remains a separate explicit approval.

## P1 — Make SVG a release-quality feature

- [x] Generate the checked-in SVG feature gallery from the runnable sample and verify the artifact byte-for-byte in CI.
- [x] Run SVG Release tests and include SVG in the clean .NET 8 consumer smoke test; see the evidence linked from [Release Readiness](RELEASE_READINESS.md).
- [ ] **Inspect the actual candidate `.nupkg` and `.nuspec`:** verify the SVG README, adapter guide, generated gallery, framework assets, dependency groups, license metadata, and absence of unintended files.
- [ ] **Verify the sample using only the packed package** in a clean consumer—not repository project references or uncommitted files.
- [ ] **Audit the documented SVG vocabulary against executable tests.** Confirm each advertised element family has a representative assertion; explicitly list known viewer-dependent or unsupported behaviors rather than promising universal rendering parity.
- [ ] **Complete regression coverage for malformed/hostile-but-parseable inputs** at the serializer boundary where the renderer claims to validate them. CI now verifies XML escaping, dimensions/ranges, duplicate IDs (including generated overlays), depth limits, non-finite numeric properties, and blank required `href` properties; see [run 39194940274](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39194940274). The remaining review is to make the external `href`/link policy explicit for supported schemes and document viewer-dependent behavior. Do not claim a security sanitizer unless a dedicated allow-list/sanitization contract is implemented.
- [x] **Prove deterministic output** for the same input scene across repeated generation. `Escapes_xml_attribute_values_and_preserves_non_ascii_text` and `Rendering_the_same_scene_twice_produces_identical_bytes` pass in [workflow run 39185442472](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39185442472). No intentional nondeterministic output is documented.

- [ ] **Make the living example useful:** the gallery should explain which model features are exercised and let reviewers map each visible example to the source builder and relevant test. Keep it generated from real adapter output, not a hand-drawn substitute.

## P2 — Package family integrity

- [x] Core, Blazor, and SVG Release tests plus shared conformance tests passed in the recorded baseline workflow.
- [x] Windows-native WPF tests passed in the recorded baseline workflow; this does not itself mean WPF is packaged for the candidate.
- [x] Add STA regression tests for native-control system defaults, fluent style/layout overrides, builder identity, and repeatable stack-panel builds; [Windows workflow run 39195324527](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39195324527) passed. See [WPF Builder Pattern Audit](WPF_BUILDER_PATTERN_AUDIT.md) for patterns extracted from other Workshop WPF builders and boundaries on what should remain optional or host-specific.
- [ ] Verify Grid splitter/track mapping, Canvas positioning, and nested-component parent propagation in the newest Windows run; these tests are added on `development` but are not yet verified.
- [ ] Audit public APIs and XML documentation for every in-scope package; build Release with no unexplained warnings/errors.
- [ ] Inspect all candidate package archives and dependency graphs; ensure package versions and Blazor-to-Core dependencies agree.
- [ ] Decide whether WPF is in scope. If included, wire it into the package workflow and verify package README, tags, repository metadata, license, dependencies, and archive contents. If excluded, record the deferral.
- [ ] Confirm Core remains platform-neutral and has no accidental UI-framework dependency.
- [ ] Record test counts, exact workflow URL, commit SHA, package filenames, and clean-consumer result.

## P3 — Consumer proof and documentation alignment

- [x] Baseline WebPage NuGet-mode and repository-source CI passed for the currently configured dependency set.
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

1. Package scope and WPF disposition.
2. Release channel and per-package versions, including dependency version alignment.
3. Repository/package license policy.
4. Candidate-version WebPage integration.
5. Candidate artifact inspection and final release sign-off.

These are intentionally explicit blockers rather than decisions an implementation agent should silently make. Update the readiness percentage only when evidence changes; checked boxes alone do not equal release readiness.
