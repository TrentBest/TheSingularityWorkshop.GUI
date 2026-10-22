# GUI Next-Release Work Queue

This is the execution queue for the next release of `TrentBest/TheSingularityWorkshop.GUI`. The authoritative release gate remains [Release Readiness](RELEASE_READINESS.md); this document prioritizes work and records the evidence required to call the release substantial. It does not authorize publication.

**Review branch:** `development`. **Estimated readiness: 80%.** This is a qualitative estimate, not a test score. The increase reflects the committed 69-type inventory, verified WPF archive-only consumption, and closure of the SVG vocabulary-to-test audit: all 60 explicit renderer mappings now have representative executable coverage and a documented test map. Core, Blazor, SVG, and WPF remain in scope; SVG is a centerpiece, not a reason to skip package-family validation.

**Latest green implementation evidence (2026-10-22):** commit [`31f48f3`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/31f48f30625cd6d7a18b5e41169d7972fd5e2765) passed [Package GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646), [Source-derived diagrams](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597714), [Solution-derived UML inventory](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597699), and [Documentation quality](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597680). The four-project generated-doc promotion passed in [run 39863162020](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863162020), and WPF now has a successful archive-only Windows consumer in the package workflow. Publication remained skipped. The latest four-package implementation artifact was inspected; exact archive hashes and metadata are recorded in [Release Readiness](RELEASE_READINESS.md). The selected candidate/version decision and candidate-specific sign-off remain open.

**Documentation gap remains substantial but has advanced:** compiler-backed generation covers 69 types (Core 31, Blazor 6, SVG 9, WPF 23); all 69 Markdown pages and 69 SVG diagrams are now committed under `docs/uml/source-files`, with four project inventories and a solution-level index. The custom target-centered renderer is tested and its output has been visually inspected, including revision-pinned source links. The remaining documentation gap is the human-authored companion UML/use-case material and the consumer-facing canonical type-page pass.

**Remaining release blockers:** human-authored companion UML/use-case pages and consumer-facing canonical type docs; version/channel alignment; selected-candidate archive inspection; archive-only consumer verification; broader hostile-input tests and SVG vocabulary-to-test audit; candidate-version WebPage/Blazor integration; and final owner approval. Generated output is now automatically promoted and drift-checked when maintained source/project/generator inputs change. No release candidate has been selected, and publication remains disabled.

## Definition of a substantial next release

The broader current-surface audit is tracked in [GUI Surface Coverage Audit](GUI_SURFACE_COVERAGE_AUDIT.md). It distinguishes current adapters from future candidates and turns cross-renderer semantics, interactions/accessibility, SVG boundaries, package integrity, and potential additional adapters into explicit review items. This audit is a planning artifact; it does not mark those gaps as closed.

SVG is a valid release centerpiece, but the release should deliver a dependable package family, not merely a new SVG file. The intended value is a tested, documented, reproducible path from the neutral GUI model to useful SVG output, with coherent package metadata and a real consumer proving the published artifact works.

## P0 — Decisions and hard release blockers

- [x] **Inspect the latest four-package implementation artifact (not the selected release candidate):** [Package GUI run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646), outer ZIP SHA-256 `4940f032f59205043b413c31eb554234be9afe354e361db1a09e7b9a2680f865`. Core `0.1.0` SHA-256 `721ed30e0d6d864b6e6c98a47fe8bd9963481c867b3443123c16aec3f356c9ce`; Blazor `0.1.0` `254629d4f8d66b43cc25e458b3149827b3c8f3addd03a0d94b4541716b230ac0`; SVG `0.1.0-alpha.1` `e8ced6f9cfee2dec2c195841eabaee9705b031b723230ee9cf93419db73b2e9d`; WPF `0.1.0` `ec2caabada7a261b03404c256414f005d9c089d53dc585c870d0ed4d63687bbe`. All four include README, matching DLL/XML docs, MIT license expression, and repository URL/commit metadata; SVG also includes its adapter guide and gallery. Dependencies and framework assets were inspected. The selected-candidate archive inspection remains a separate gate because package versions/channel have not been chosen.

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
- [x] **Audit the documented SVG vocabulary against executable tests.** `docs/SVG_ADAPTER.md` now maps every explicit renderer switch case to representative tests. The prior gap of 11 unasserted node kinds was closed by focused tests for `svg-switch`, `svg-title`, radial gradients, color matrix/blend/flood/composite/tile filters, and the green/blue/alpha component-transfer functions. The full package workflow passed on PR head [`0f16e10`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/0f16e1089507275757e321e28cab22a6aee1f300), [run 39864841258](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39864841258). Viewer-dependent behavior and URL sanitization remain explicitly outside this coverage claim.
- [ ] **Complete regression coverage for malformed/hostile-but-parseable inputs** at the serializer boundary where the renderer claims to validate them. CI currently verifies XML escaping, dimensions/ranges, duplicate IDs (including generated overlays), depth limits, non-finite numeric properties, and blank required `href` properties; the full suite passed in [run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646). The renderer is not a sanitizer; consumer-side validation remains necessary for untrusted scene data.
- [x] **Document the external `href`/link policy:** `docs/SVG_ADAPTER.md` now lists link, image, filter-image, reusable-definition, and motion-path reference surfaces and states that values are serialized without a scheme/origin allow-list. Consumers must apply policy for their own threat model; this is not a security-sanitizer claim. Viewer-dependent resource loading/navigation remains outside the adapter's guarantee.
- [x] **Prove deterministic output** for the same input scene across repeated generation. `Escapes_xml_attribute_values_and_preserves_non_ascii_text` and `Rendering_the_same_scene_twice_produces_identical_bytes` pass in the current full test suite, [run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646). No intentional nondeterministic output is documented.

- [x] **Make the living example traceable:** `docs/SVG_ADAPTER.md` maps visible gallery features to sample source and representative renderer tests. The gallery is generated from real adapter output and byte-for-byte verified by the current full package workflow, [run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646). This mapping is representative, not a claim that every SVG vocabulary item is exhaustively tested.

## P2 — Package family integrity

- [x] Core, Blazor, SVG, and shared conformance tests pass in the current four-package CI baseline: [Package GUI run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646).
- [x] Windows-native WPF tests and the WPF archive-only consumer passed in [Package GUI run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646); candidate-specific packaging remains open.
- [x] Add reusable native WPF numeric and enum value-control builders with STA tests for initial state, change callbacks, and invalid inputs; verified by [workflow run 39277269099](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39277269099). These controls expand the current WPF surface but do not imply complete adapter parity.
- [x] Add STA regression tests for native-control system defaults, fluent style/layout overrides, builder identity, and repeatable stack-panel builds; [Windows workflow run 39195324527](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39195324527) passed. See [WPF Builder Pattern Audit](WPF_BUILDER_PATTERN_AUDIT.md) for patterns extracted from other Workshop WPF builders and boundaries on what should remain optional or host-specific.
- [x] Verify Grid splitter/track mapping, Canvas positioning, and nested-component parent propagation; [Windows workflow run 39195444173](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39195444173) passed after a compile-time assertion correction. Reusable window-host builder tests remain in a later run.
- [ ] Audit public APIs and XML documentation for every in-scope package; build Release with no unexplained warnings/errors.
- [ ] **Inventory every production type across Core, Blazor, SVG, and WPF** and map each fully qualified type/source file to its own documentation page and UML SVG. Use [Per-Type Documentation and UML Standard](CLASS_DOCUMENTATION_STANDARD.md); XML API comments alone do not satisfy this requirement.
- [ ] **Create one dedicated Markdown page per declared type**, including internal and nested maintained types, with source link, responsibility/boundaries, member contracts, verified relationships, behavior/failure modes, and relevant tests.
- [x] **Commit the compiler-backed source inventory and automate safe promotion:** all four product projects now produce 69 type pages and 69 accessible UML SVGs; the promotion safety/drift workflow passed in [run 39862352900](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39862352900), and the generated set is committed at [commit `14b442f`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/14b442f36b35465464c6ecd0a9589bffa277a5f0). This closes source inventory/promotion, not human-authored use cases.
- [ ] **Embed each type's UML class-diagram SVG on its page**; include inheritance, implemented interfaces, and source-grounded relationships, plus accessible title/description and a textual fallback. Do not use decorative or unverified diagrams.
- [ ] **Add a central type inventory/index and CI drift checks** so every new or existing type must have a page and diagram; validate links, SVG accessibility metadata, and a deliberately missing-page/diagram failure case.
- [ ] **Review generated diagram accuracy against source and tests** and record exact commit/workflow evidence before marking this requirement complete.
- [ ] Inspect all candidate package archives and dependency graphs; ensure package versions and Blazor-to-Core dependencies agree.
- [x] Wire WPF into the package workflow and verify its package README, tags, repository metadata, MIT license expression, and archive README presence in [workflow run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646). The same run passed the clean Core/Blazor/SVG consumer smoke test and Windows WPF tests; `publish_nuget` was skipped.
- [x] **Verify WPF from its packed archive, not a project reference:** the Windows lane packs Core/WPF, restores a fresh `net8.0-windows` consumer using only the local `.nupkg` folder, calls `WpfGuiRenderer.Render`, and passes in [run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646) on implementation commit [`31f48f3`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/31f48f30625cd6d7a18b5e41169d7972fd5e2765). Repeat this against the selected release candidate before final approval.
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
