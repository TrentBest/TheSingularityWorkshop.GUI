# GUI Package Release Readiness

This is the release gate for the packages produced by `TrentBest/TheSingularityWorkshop.GUI`. It is a working checklist, not a claim that a release is currently approved. Check an item only when the evidence is available for the exact release candidate commit.

**Current review baseline:** `development`  
**Current package declarations observed during the initial audit:** Core `0.1.0`; Blazor `0.1.0`; WPF `0.1.0`; SVG `0.1.0-alpha.1`. These are project-file declarations, not confirmation of the versions currently published on NuGet.org.  
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
- [ ] Reconcile README and architecture claims with current code. The repository README currently contains older statements that the Core-to-WPF bridge is not established, while WPF renderer tests and the adapter guide describe a WPF `GuiNode` renderer; resolve that contradiction before release.
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

## Initial audit findings to resolve

These are evidence-based starting points, not an exhaustive audit:

- **Version declarations need reconciliation.** Core, Blazor, and WPF currently declare `0.1.0`; SVG declares `0.1.0-alpha.1`. The repository README describes the WebPage NuGet dependency as Blazor `0.1.0-alpha.8`. Confirm actual published versions and choose an intentional, consistent release plan.
- **Package scope is not yet aligned with workflow scope.** The workflow packs Core, Blazor, and SVG; it builds WPF on Linux and tests WPF on Windows but does not pack a WPF package.
- **README claims need updating.** The top-level README still describes WPF as reusable builders without an established Core bridge, which conflicts with the current WPF renderer, tests, and adapter guide.
- **The public release path is intentionally blocked.** The publish job has an explicit `&& false` guard. This is a safety control, not a failed readiness check to remove prematurely.
- **End-to-end integration remains a separate gate.** The roadmap says the WebPage is the proving ground but leaves its end-to-end evidence checklist open. Do not call integration verified until the consuming application demonstrates it.

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
