# GUI Package Release Readiness

**Prioritized execution queue:** [Next-Release Work Queue](NEXT_RELEASE_WORK_QUEUE.md). **Renderer/package coverage audit:** [GUI Surface Coverage Audit](GUI_SURFACE_COVERAGE_AUDIT.md). Use the queue to sequence implementation and the audit to track current adapter contracts and gaps; this checklist remains the authoritative release gate.

This is the release gate for the packages produced by `TrentBest/TheSingularityWorkshop.GUI`. It is a working checklist, not a claim that a release is currently approved. Check an item only when the evidence is available for the exact release candidate commit.

**Review branch:** `development`. **Estimated readiness: 80%** as of 2026-10-22. This is a qualitative release-gate estimate, not a test pass percentage. The increase reflects three verified deliverables: all 69 compiler-backed type pages and 69 UML SVGs are committed with automated regeneration/promotion; WPF has a Windows consumer test that restores and runs from packed Core/WPF archives only; and all 60 explicit SVG renderer node mappings now have representative executable coverage plus a vocabulary-to-test matrix. Human-authored use cases and final candidate validation remain open.

**Latest green implementation commit (2026-10-22):** [`31f48f3`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/31f48f30625cd6d7a18b5e41169d7972fd5e2765). [Package GUI run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646), [Source-derived diagrams run 39863597714](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597714), [Solution-derived UML inventory run 39863597699](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597699), and [Documentation quality run 39863597680](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597680) all passed on this exact development commit. The package run passed Core, Blazor, SVG, shared consumer, and Windows WPF tests, including the new archive-only WPF consumer; `publish_nuget` was skipped. The four-project UML promotion and corrected inventory links passed in [run 39863162020](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863162020), with generated pages committed at [`5492595`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/5492595d83b828c1ce7e874f98601028f03fec4b).

**Committed compiler-backed UML baseline:** 69 types across Core (31), Blazor (6), SVG (9), and WPF (23). The promotion run committed 69 Markdown type pages, 69 SVG diagrams, four project inventories, four type indexes, and the solution-level index under [`docs/uml/source-files`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/tree/development/docs/uml/source-files). The generation/promotion workflow builds all four projects, reconciles exact-build type metadata, runs promotion safety tests, promotes only manifest-owned output, and verifies generated-output drift. These pages are source-derived technical inventory; they are not a substitute for the required human-authored use-case pages.

**Verified diagram behavior:** consumers above the focal type, the focal card vertically centered, dependencies below, external/framework types to the right; compact cards show declaration kind/name, namespace, and a member-count hint; relationship labels are concise, type-conduit colors are identity-stable, receiving stubs are rotated 180 degrees, and the SVG canvas is opaque for readable labels. Source-backed cards link to a source file at an immutable commit SHA and line number. This is a one-hop neighborhood view, not a complete project graph or runtime trace.

**Current package declarations:** Core `0.1.0`; Blazor `0.1.0`; WPF `0.1.0`; SVG `0.1.0-alpha.1`. **Public NuGet versions last verified 2026-10-18:** Core `0.1.0-alpha.4`; Blazor `0.1.0-alpha.8`; WPF `0.1.0-alpha.6`. See [Core](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Core), [Blazor](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Blazor/), and [WPF](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.WPF/). SVG publication status remains unverified. The mismatch must be reconciled by an explicit release-version decision; do not infer the desired channel or version.

**Latest four-package archive inspection (2026-10-22; implementation artifact, not selected release candidate):** downloaded and inspected [`gui-nuget-packages` from run 39863597646](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646), outer ZIP SHA-256 `4940f032f59205043b413c31eb554234be9afe354e361db1a09e7b9a2680f865`. It contains exactly four `.nupkg` files: Core `0.1.0` (SHA-256 `721ed30e0d6d864b6e6c98a47fe8bd9963481c867b3443123c16aec3f356c9ce`), Blazor `0.1.0` (`254629d4f8d66b43cc25e458b3149827b3c8f3addd03a0d94b4541716b230ac0`), SVG `0.1.0-alpha.1` (`e8ced6f9cfee2dec2c195841eabaee9705b031b723230ee9cf93419db73b2e9d`), and WPF `0.1.0` (`ec2caabada7a261b03404c256414f005d9c089d53dc585c870d0ed4d63687bbe`). All four contain a README, matching DLL and XML documentation, MIT license expression, repository URL `https://github.com/TrentBest/TheSingularityWorkshop.GUI`, and repository commit `31f48f30625cd6d7a18b5e41169d7972fd5e2765`. Core targets `net8.0` and has no package dependencies; Blazor targets `net8.0` and depends on Core `0.1.0` plus ASP.NET Core Components/Web `8.0.31`; SVG targets `net8.0`, depends on Core `0.1.0`, and includes `docs/SVG_ADAPTER.md` plus the generated gallery; WPF targets `net8.0-windows7.0` and depends on Core `0.1.0`. No unexpected source/build files were present. These are verified implementation archives, not selected-candidate sign-off; published-version mismatch and the explicit release version/channel decision remain open.

**Still blocking release:** human-authored per-type UML/use-case pages and a consumer-facing canonical type-documentation pass; selected-candidate four-package archive inspection and archive-only consumer verification against that exact candidate; release version/channel alignment; remaining hostile-input regression review; candidate-version WebPage/Blazor integration; and final owner approval. WPF archive-only consumption and SVG vocabulary-to-test coverage are verified for the current implementation, but not yet all release-candidate gates. The generated inventory is committed, but the generated pages are not claimed to complete the separate use-case documentation requirement. No release candidate has been selected, and publication remains disabled.

**Owner-approved scope and policy:** Core, Blazor, SVG, and WPF are included in the next package family; MIT is approved. Publication remains disabled until the exact candidate passes the hard gates and receives explicit approval. Older implementation snapshots and audit logs are intentionally not duplicated here; the current evidence and unresolved gates are recorded above and in the checklist below.

## 1. Decide what this release contains

- [ ] Decide the release version and channel for each package; record whether this is an alpha, preview, or stable release.
- [x] Scope decision: Core, Blazor, SVG, and WPF are all included in the next GUI package-family release.
- [x] WPF is included as the current initial renderer/builder package; do not imply adapter parity beyond the implementation and tests.
- [ ] Record the exact candidate commit and intended source branch. Release from the reviewed candidate only; do not infer release readiness from `development` alone.
- [ ] Confirm the intended public package IDs and version relationships:
  - `TheSingularityWorkshop.GUI.Core`
  - `TheSingularityWorkshop.GUI.Blazor`
  - `TheSingularityWorkshop.GUI.SVG`
  - `TheSingularityWorkshop.GUI.WPF` (only if included)

## 2. Implementation and contract audit

- [ ] **Per-type documentation and UML coverage:** every declared production type in Core, Blazor, SVG, and WPF has a dedicated Markdown page and an embedded, source-grounded UML class-diagram SVG. XML API comments and packaged XML files do not satisfy this gate by themselves.
- [ ] A complete type/source/page/diagram inventory is checked against actual source declarations; internal and nested maintained types are included or explicitly dispositioned.
- [ ] Every type page documents purpose and architectural boundaries, relevant members and invariants, verified relationships, failure behavior, and representative tests.
- [ ] Every SVG has accessible title/description and a textual fallback; links and assets validate, and CI fails on missing pages/diagrams or a deliberate negative fixture.
- [ ] The diagram generation/maintenance path and source-to-diagram review are documented; no conceptual or invented relationship is presented as implementation fact.
- [ ] Record the exact commit and workflow run proving coverage before checking these items.



- [ ] Review Core for accidental platform dependencies and confirm the neutral-model boundary remains intact.
- [ ] Verify public API and XML documentation for all APIs intended to be consumed in this release.
- [ ] Review breaking changes and compatibility implications against the last published version of each package.
- [ ] Confirm adapter behavior is documented as implemented, tested, specified, or unresolved; do not present proposed roadmap items as shipped functionality.
- [ ] Confirm the shared contract tests cover the promised semantic invariants, and list remaining adapter-specific differences.
- [ ] Confirm duplicate-ID and depth-limit behavior is consistent with the documented adapter-boundary rules.
- [ ] Review property mapping, source/media handling, text, child order, identity, unsupported-property behavior, accessibility, and interaction/event behavior. Explicitly mark unsupported or unverified behavior rather than implying parity.
- [x] Reconcile the top-level README's Core-to-WPF status with current code. The README and Start Here guide now describe the tested initial WPF renderer accurately; the broader documentation consistency audit remains open below.
- [x] Audit cross-repository integration claims against workflow evidence. WebPage's NuGet-mode and repository-source jobs both passed on commit [`217cc4d`](https://github.com/TrentBest/WebPage/commit/217cc4dbc0312bc0ec005293cb70d29b961a5e2c), [run 39180999142](https://github.com/TrentBest/WebPage/actions/runs/39180999142). This proves the current published dependency set and source-integration path pass; it does not verify a future GUI candidate package version.

## 3. Tests and verification evidence

- [x] Run Core tests in Release configuration. Verified in [workflow run 39180369067](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39180369067) on commit `0f237f2`.
- [x] Run Blazor tests in Release configuration. Verified in [workflow run 39180369067](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39180369067) on commit `0f237f2`.
- [x] Run SVG tests in Release configuration. Verified in [workflow run 39180369067](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39180369067) on commit `0f237f2`.
- [x] Run shared adapter conformance tests in Release configuration. Verified in [workflow run 39180369067](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39180369067) on commit `0f237f2`.
- [x] Run WPF renderer tests on Windows; confirm the native test job actually executes, not just that the project compiles. The Windows `test-wpf` job passed in [workflow run 39180369067](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39180369067) on commit `0f237f2`.
- [x] Run the repository documentation-link checker. The workflow's `Check repository-local documentation links` step passed in [run 39180369067](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39180369067).
- [x] Regenerate the SVG feature gallery and verify it matches the committed artifact byte-for-byte. The generator and `cmp` verification passed in [run 39180369067](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39180369067).
- [ ] Build all in-scope projects in Release configuration with no unexpected warnings or errors.
- [ ] Record test counts, warnings/errors, workflow URL, and exact commit SHA. A local test result is not CI evidence; a green workflow only verifies the checks it actually ran.
- [ ] Review the full workflow result for the candidate commit and resolve every failure relevant to release.

## 4. Package metadata and artifact inspection

- [ ] Align the version in each in-scope project file with the intended release version. Investigate any mismatch between package declarations and documentation before packing.
- [ ] Inspect package IDs, target frameworks, authors, descriptions, repository metadata, MIT license, tags, readme configuration, and dependency versions.
- [ ] Ensure every package has an appropriate NuGet README and useful package-facing documentation. SVG explicitly packs a README, adapter guide, and generated gallery; verify this in the actual `.nupkg`. Review equivalent package documentation for Core and Blazor, and decide the appropriate WPF package documentation if WPF ships.
- [ ] Pack each in-scope package from the candidate commit into a clean output directory.
- [ ] Inspect every `.nupkg` archive and its `.nuspec`: verify the intended files, framework assets, dependency groups/versions, and absence of accidental source, build output, secrets, or unrelated files.
- [ ] Verify package-to-package dependency versions are coherent and the dependency graph does not rely on uncommitted project references.
- [ ] Confirm the workflow packs Core, Blazor, SVG, and WPF; inspect all four package archives and ensure WPF remains Windows-targeted.
- [x] Install each package from the produced artifacts into a clean consumer project targeting the supported framework(s), then compile and run a minimal representative use case. Core, Blazor, and SVG passed the workflow's clean .NET 8 consumer in [run 39180369067](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39180369067); this is not full WebPage integration.
- [ ] For Blazor, verify the documented renderer path in a real consumer host; for SVG, verify the package can render the documented sample without relying on repository-only files.
- [ ] Record package filenames, versions, hashes if used, and the artifact/workflow run from which they came.

## 5. Dependency and consumer compatibility

- [ ] Confirm the supported .NET target framework policy for the release and ensure it is stated consistently in package docs.
- [ ] Review Blazor's ASP.NET Core component dependency versions and verify they are appropriate for the intended consumer runtime.
- [ ] Test a clean restore with only the published/in-scope package dependencies available; no accidental dependence on sibling repository source should remain.
- [ ] Check downstream consumers that matter to this release, especially WebPage, against the explicitly selected candidate package versions. Baseline WebPage NuGet-mode and repository-source CI both passed on commit [`217cc4d`](https://github.com/TrentBest/WebPage/commit/217cc4dbc0312bc0ec005293cb70d29b961a5e2c), [run 39180999142](https://github.com/TrentBest/WebPage/actions/runs/39180999142), but that run uses WebPage's current published package versions rather than a not-yet-selected GUI candidate.
- [ ] Verify repository-source development mode and NuGet/package mode are clearly distinguished where applicable.
- [ ] Document known limitations and compatibility constraints instead of silently expanding the support promise.

## 6. Documentation and release communication

- [ ] Ensure README, `docs/START_HERE.md`, architecture guide, model guide, adapter guide, testing guide, roadmap, and package READMEs tell one consistent story.
- [ ] Update version examples and package installation instructions to the actual release versions.
- [ ] Ensure the SVG gallery and any advertised “living” examples are generated from the checked-in generator and validated in CI.
- [ ] Verify all local documentation links and package-included documentation paths.
- [ ] Distinguish current implementation from proposed Core contracts, including typed properties, lifecycle/execution, capability negotiation, semantic input, localization, accessibility, and broader spatial contracts.
- [ ] Add release notes describing shipped behavior, compatibility changes, known gaps, and migration steps where needed.
- [ ] Update the roadmap only for work whose implementation and evidence now justify a status change.

## 7. CI, publication safety, and release approval

- [ ] Confirm Linux checks and Windows WPF checks pass for the exact candidate commit.
- [ ] Confirm package artifacts are retained and correspond to that same commit.
- [ ] Confirm NuGet Trusted Publishing permissions/configuration are valid without enabling an unreviewed publish path.
- [ ] Keep `publish_nuget` disabled while readiness is being established. Do not remove the safety gate as part of routine readiness work.
- [ ] Review the exact packages and versions to be published and obtain explicit release approval.
- [ ] Merge/promote the reviewed candidate according to the repository's release policy, then verify the target branch and commit before publishing.
- [ ] Publish only after approval and only the inspected artifacts; verify each package/version on NuGet.org after publication.
- [ ] If publication is not authorized or fails, leave the publish gate disabled and record the outcome without claiming release completion.

## Release decision record

Complete this section for each candidate.

- Candidate commit:
- Intended release channel:
- Package IDs and versions:
- CI workflow run:
- Package inspection / clean-consumer evidence:
- Known limitations accepted for this release:
- Remaining unchecked hard gates:
- Release decision: **Not reviewed / Ready for approval / Approved / Published**
- Approval and publication evidence:

A package is ready only when all hard gates for the selected package scope are checked, exceptions are explicitly accepted, and the exact candidate has been reviewed. An unchecked item is not implicitly waived.
