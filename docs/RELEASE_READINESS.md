# GUI Package Release Readiness

This is the release gate for the packages produced by `TrentBest/TheSingularityWorkshop.GUI`. It is a working checklist, not a claim that a release is currently approved. Check an item only when the evidence is available for the exact release candidate commit.

**Current review baseline:** `development`  
**Current project-file declarations (reviewed 2026-10-18):** Core `0.1.0`; Blazor `0.1.0`; WPF `0.1.0`; SVG `0.1.0-alpha.1`. **Official NuGet Gallery versions observed 2026-10-18:** Core `0.1.0-alpha.4`; Blazor `0.1.0-alpha.8`; WPF `0.1.0-alpha.6`. See [Core](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Core), [Blazor](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Blazor/), and [WPF](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.WPF/). SVG publication status remains unverified. The project declarations and public package versions therefore diverge for all three listed packages; this is a release blocker, not a reason to infer a desired version.  
**Publication:** disabled by the workflow's explicit `&& false` guard. Do not remove that guard or publish until the release candidate has passed this checklist and publication is explicitly approved.

## 1. Decide what this release contains

- [ ] Decide the release version and channel for each package; record whether this is an alpha, preview, or stable release.
- [ ] Decide whether this release includes Core, Blazor, SVG, and WPF, or explicitly defer any package.
- [ ] Decide whether WPF is a supported `GuiNode` adapter package or only reusable WPF builder infrastructure for this release. Do not imply adapter parity beyond the implementation and tests.
- [ ] Record the exact candidate commit and intended source branch. Release from the reviewed candidate only; do not infer release readiness from `development` alone.
- [ ] Confirm the intended public package IDs and version relationships:
  - `TheSingularityWorkshop.GUI.Core`
  - `TheSingularityWorkshop.GUI.Blazor`
  - `TheSingularityWorkshop.GUI.SVG`
  - `TheSingularityWorkshop.GUI.WPF` (only if included)

## 2. Implementation and contract audit

- [ ] Review Core for accidental platform dependencies and confirm the neutral-model boundary remains intact.
- [ ] Verify public API and XML documentation for all APIs intended to be consumed in this release.
- [ ] Review breaking changes and compatibility implications against the last published version of each package.
- [ ] Confirm adapter behavior is documented as implemented, tested, specified, or unresolved; do not present proposed roadmap items as shipped functionality.
- [ ] Confirm the shared contract tests cover the promised semantic invariants, and list remaining adapter-specific differences.
- [ ] Confirm duplicate-ID and depth-limit behavior is consistent with the documented adapter-boundary rules.
- [ ] Review property mapping, source/media handling, text, child order, identity, unsupported-property behavior, accessibility, and interaction/event behavior. Explicitly mark unsupported or unverified behavior rather than implying parity.
- [x] Reconcile the top-level README's Core-to-WPF status with current code. The README and Start Here guide now describe the tested initial WPF renderer accurately; the broader documentation consistency audit remains open below.
- [ ] Audit cross-repository integration claims. A dependency reference or closed migration issue is not by itself proof of a working end-to-end WebPage integration.

## 3. Tests and verification evidence

- [ ] Run Core tests in Release configuration.
- [ ] Run Blazor tests in Release configuration.
- [ ] Run SVG tests in Release configuration.
- [ ] Run shared adapter conformance tests in Release configuration.
- [ ] Run WPF renderer tests on Windows; confirm the native test job actually executes, not just that the project compiles.
- [ ] Run the repository documentation-link checker.
- [ ] Regenerate the SVG feature gallery and verify it matches the committed artifact byte-for-byte.
- [ ] Build all in-scope projects in Release configuration with no unexpected warnings or errors.
- [ ] Record test counts, warnings/errors, workflow URL, and exact commit SHA. A local test result is not CI evidence; a green workflow only verifies the checks it actually ran.
- [ ] Review the full workflow result for the candidate commit and resolve every failure relevant to release.

## 4. Package metadata and artifact inspection

- [ ] Align the version in each in-scope project file with the intended release version. Investigate any mismatch between package declarations and documentation before packing.
- [ ] Inspect package IDs, target frameworks, authors, descriptions, repository metadata, license, tags, readme configuration, and dependency versions.
- [ ] Ensure every package has an appropriate NuGet README and useful package-facing documentation. SVG explicitly packs a README, adapter guide, and generated gallery; verify this in the actual `.nupkg`. Review equivalent package documentation for Core and Blazor, and decide the appropriate WPF package documentation if WPF ships.
- [ ] Pack each in-scope package from the candidate commit into a clean output directory.
- [ ] Inspect every `.nupkg` archive and its `.nuspec`: verify the intended files, framework assets, dependency groups/versions, and absence of accidental source, build output, secrets, or unrelated files.
- [ ] Verify package-to-package dependency versions are coherent and the dependency graph does not rely on uncommitted project references.
- [ ] Confirm the workflow actually packs every package declared in release scope. The current workflow packs Core, Blazor, and SVG; WPF is built and tested but is not currently packed by that workflow.
- [ ] Install each package from the produced artifacts into a clean consumer project targeting the supported framework(s), then compile and run a minimal representative use case.
- [ ] For Blazor, verify the documented renderer path in a real consumer host; for SVG, verify the package can render the documented sample without relying on repository-only files.
- [ ] Record package filenames, versions, hashes if used, and the artifact/workflow run from which they came.

## 5. Dependency and consumer compatibility

- [ ] Confirm the supported .NET target framework policy for the release and ensure it is stated consistently in package docs.
- [ ] Review Blazor's ASP.NET Core component dependency versions and verify they are appropriate for the intended consumer runtime.
- [ ] Test a clean restore with only the published/in-scope package dependencies available; no accidental dependence on sibling repository source should remain.
- [ ] Check downstream consumers that matter to this release, especially WebPage, against the candidate package versions.
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
- [ ] Keep `publish_nuget` disabled while readiness is being established. The current workflow condition includes `&& false`; do not remove it as part of routine readiness work.
- [ ] Review the exact packages and versions to be published and obtain explicit release approval.
- [ ] Merge/promote the reviewed candidate according to the repository's release policy, then verify the target branch and commit before publishing.
- [ ] Publish only after approval and only the inspected artifacts; verify each package/version on NuGet.org after publication.
- [ ] If publication is not authorized or fails, leave the publish gate disabled and record the outcome without claiming release completion.

## Initial audit findings and current disposition

These findings are evidence-based and are not an exhaustive audit. Revisit them against the exact candidate commit before a release decision.

- **Project/package version drift — CONFIRMED RELEASE BLOCKER.** Project files declare Core `0.1.0`, Blazor `0.1.0`, and WPF `0.1.0`, while the official NuGet Gallery shows Core `0.1.0-alpha.4`, Blazor `0.1.0-alpha.8`, and WPF `0.1.0-alpha.6` as the latest versions observed on 2026-10-18. SVG's project declares `0.1.0-alpha.1`, but its publication status has not been established. The checked-in Blazor project depends on Core via a project reference, so the next package set must explicitly align package versions and dependency edges. Do not silently change project versions to match the latest public versions: first decide whether this release advances those package lines, what compatibility guarantees apply, and whether SVG is in scope. WebPage's `0.1.0-alpha.8` reference is confirmed as a published Blazor version, not proof of current end-to-end integration.
- **Package scope is unresolved — OPEN.** The workflow packs Core, Blazor, and SVG; it builds WPF on Linux and tests WPF on Windows but does not pack a WPF package. However, a WPF package is already published on NuGet (`0.1.0-alpha.6` observed 2026-10-18), and the checked-in WPF project declares `0.1.0`. Decide whether WPF is maintained in the release family, and if so align its version, package documentation, and publication workflow. The fact that WPF is absent from this workflow does not mean it has never been published.
- **README/WPF status contradiction — SOURCE UPDATED AND CI-VERIFIED.** Commits [`40e2aca`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/40e2aca844eda72bc2f50c2766f3abfe690daa4d) and [`dd9697d`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/dd9697d54ccb5f3c4ee5b68dc702644310a1468d) align the README with the initial `GuiNode`-to-WPF renderer evidence without claiming adapter parity. The repository-local documentation-link check passed in [workflow run 39149777954](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39149777954).
- **WPF CI compile blocker — FIXED AND VERIFIED.** The Windows test job had failed because `WpfGuiRendererConformanceTests` declared the same test method twice. The duplicate was removed in [`8f82493`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/8f824934013e13c68414bbe67ae98766a168a906). [Workflow run 39149777954](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39149777954) passed the Linux test/package job and Windows WPF tests; publication was skipped.
- **Core and Blazor package-facing READMEs — ADDED AND PACKAGE-VERIFIED.** `src/Core/README.md` and `src/Blazor/README.md` document the implemented contracts and limits. The same workflow verified both README entries in the packed `.nupkg` archives and their `.nuspec` readme metadata.
- **Clean package consumer — VERIFIED.** [Workflow run 39149777954](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39149777954) restored, built, and ran a clean .NET 8 consumer against the locally packed Core, Blazor, and SVG artifacts. This proves package dependency closure for that representative case, not a full WebPage integration.
- **License metadata — OWNER DECISION REQUIRED.** No root license file is present on `development`. SVG currently declares an MIT package license expression, while Core and Blazor do not declare a license. Confirm the intended repository/package license and add the appropriate authoritative license file before asserting parity; do not infer legal intent from a single project file.
- **Package metadata parity — OPEN.** Added `src/WPF/README.md` as project-level documentation, but the WPF project file still does not declare `PackageReadmeFile`, `PackageTags`, or `RepositoryType`, unlike Core and Blazor; Core and Blazor do not declare `PackageLicenseExpression`, while SVG declares MIT. The new WPF README is not claimed to be included in the NuGet package until the project configures it and the resulting `.nupkg` is inspected. Before packaging WPF or calling the package family consistent, decide the license policy and provide WPF package-facing README/tags/repository metadata as appropriate. This is a metadata completeness finding, not authorization to invent a license or expand the package set.
- **The public release path is intentionally blocked.** The publish job has an explicit `&& false` guard. This is a safety control, not a failed readiness check to remove prematurely.
- **End-to-end integration remains a separate gate.** WebPage is the proving ground. Its NuGet-mode reference to Blazor `0.1.0-alpha.8` matches the latest published Blazor version observed on 2026-10-18, but the current repository project declares `0.1.0` and Core's project declaration also diverges from its latest published version. Compile/run the actual consuming application in NuGet mode against the intentionally selected package set before calling integration verified.

## Follow-up implementation audit — WPF

- [x] Removed the duplicate `WpfGuiIdentity.SetNodeId(element, node.Id)` call in `src/WPF/WpfGuiRenderer.cs`. The remaining single assignment was verified in the current development file.
- [ ] Run the Windows WPF test job against the resulting commit and record its outcome. The available commit-status query currently returns 0 status entries; the connector's commit-workflow query is PR-trigger-only and returned 0 runs, so these results do not establish that a push-triggered run passed.
- [ ] Continue checking renderer behavior against explicit tests. Current WPF tests cover the documented basic tree, identity, dimensions/spacing, colors, typography, alignment, fallback, duplicate IDs, and depth guard; they do not establish full parity, accessibility, or event behavior.
- [ ] Resolve package version, license, and WPF package-scope decisions before changing package metadata or the publication workflow.
- [ ] Keep the publish job's `&& false` guard intact until explicit release approval.

This is an audit record, not a release approval. The duplicate-call cleanup is not a claim of changed runtime behavior or successful CI.

## Follow-up implementation audit — WPF tree validation

- [x] Removed the duplicate semantic-ID assignment in `WpfGuiRenderer.ApplyProperties`; the single assignment was verified in the current source.
- [x] Removed repeated whole-subtree validation during recursive rendering. Public `Render` now validates the complete tree once, then delegates descendants to a private validated render path. Source inspection confirms recursive calls no longer re-enter public `Render`.
- [ ] Run and record the Windows WPF tests for the exact follow-up commits. Available status queries have not provided evidence of a fresh passing workflow for these changes.
- [ ] Continue the property/source/media, accessibility, and interaction behavior audit; existing tests do not establish full adapter parity.
- [x] Made invalid WPF `fontFamily` values non-fatal, matching the renderer's existing ignore-invalid-property behavior for brushes and numeric values. The first NUL-containing and subsequent empty family test values were both accepted by WPF, so neither exercised the exception fallback. The test now passes a null value to exercise the caught argument exception and asserts the system-font fallback. The Windows WPF test job passed on commit [`f113482`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/f1134828b6585961f9c5c54e51826b24d7844fba) in [workflow run 39179513063](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39179513063); the overall package workflow is still running.
- [x] Made WPF image loading eager (`BitmapCacheOption.OnLoad`) so unreadable image sources can be handled during rendering; added a placeholder-fallback regression test for a missing file URI. The missing-file placeholder test passed in Windows run [39179255926](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39179255926); the run still failed on the font-family test.
- [ ] Run the new `Renderer_preserves_button_label_and_renders_nested_button_children_in_order` WPF regression test on Windows; it covers the existing button-child rendering path, including the original label and child order.
- [ ] Resolve release version, license policy, and WPF package scope before altering package metadata or publication workflow.

These are implementation cleanups, not release approval. The single-validation change is intended to avoid repeatedly traversing each descendant subtree (which would cause avoidable quadratic validation work on nested trees); runtime performance has not been benchmarked in this environment. Keep the publish job's `&& false` guard intact until explicit release approval.

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
