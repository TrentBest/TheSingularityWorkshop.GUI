# GUI Release Closeout

**Purpose:** turn the current development state into one reviewable release candidate without expanding scope or confusing implementation evidence with release approval.

**Current readiness:** 78% (qualitative gate estimate; see [Release Readiness](RELEASE_READINESS.md) for the evidence and remaining gates). **Publication remains disabled.** This document does not authorize publishing.

## Release scope

The next package family contains:

- `TheSingularityWorkshop.GUI.Core` — platform-neutral semantic model and builders.
- `TheSingularityWorkshop.GUI.Blazor` — Blazor manifestation.
- `TheSingularityWorkshop.GUI.SVG` — SVG artifact manifestation.
- `TheSingularityWorkshop.GUI.WPF` — initial Windows-native renderer/builders, with no claim of parity.

The intended target framework currently shown by package artifacts is .NET 8; WPF is Windows-targeted. The owner must decide the release channel and exact versions because project declarations and previously published versions do not currently align.

## Evidence already available

On development commit [`31f48f3`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/31f48f30625cd6d7a18b5e41169d7972fd5e2765), these workflows passed:

- [Package GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597646)
- [Source-derived diagrams](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597714)
- [Solution-derived UML inventory](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597699)
- [Documentation quality](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39863597680)

The package workflow exercised Core, Blazor, SVG, shared consumer, and Windows WPF tests. It produced four packages whose metadata and contents were inspected. The WPF workflow also restored and ran a consumer using packed Core/WPF archives. These are valuable implementation baselines, **not** validation of the release candidate that has yet to be selected.

## Remaining hard gates, in execution order

1. **Owner decision — release channel and versions.** Reconcile declared versions with public NuGet versions. Do not silently publish stable versions or invent a version alignment.
2. **Freeze candidate.** Record one commit and branch after the version decision. All evidence below must correspond to that exact commit.
3. **Complete user-facing documentation.** Keep the README concise and package-oriented. Provide a runnable first example, link to the SVG use case, state per-adapter limitations, and keep theory in its own track. Source-derived type inventory is useful reference material but is not a substitute for human-authored use cases.
4. **Close the behavioral audit.** Map the actual public node/property vocabulary to tests. Prioritize malformed/hostile input, depth and size limits, duplicate identities, SVG attribute escaping, external references, and unsupported-node behavior. Record supported/unsupported/unverified per adapter; do not infer parity.
5. **Candidate package verification.** Pack all four packages from the frozen candidate. Inspect IDs, versions, target frameworks, dependency groups, readmes, XML docs, license and repository metadata, and unintended files. Record filenames and hashes.
6. **Archive-only consumers.** Restore and run minimal consumers from the candidate archives—not project references—for Core/Blazor/SVG and Windows Core/WPF. Verify the documented SVG use case from the packed package.
7. **Downstream proof.** Test WebPage against the candidate Blazor package version. A prior green run using existing published versions is not candidate integration proof.
8. **Documentation consistency and CI.** Run documentation link checks, generated SVG gallery drift checks, source-derived inventory promotion/drift checks, Linux package tests, and Windows WPF tests. Resolve release-relevant failures.
9. **Review and approval.** Record known limitations and any explicitly accepted exceptions. Obtain owner approval for the exact package IDs and versions.
10. **Publish only after approval.** Keep the workflow's publish gate disabled until the explicit decision. After publishing, verify each exact package/version on NuGet.org.

## Explicit non-goals for this release

Do not block this package release on GPU/Vulkan/WebGPU implementation, full adapter parity, a GUI-owned animation runtime, a universal interaction/lifecycle system, or interactive AST editing. These are future architecture directions unless present in code and covered by tests. Do not describe reference images or static SVGs as working interactive features.

## Theory update rule

The theory is an engineering argument, not a feature checklist. Update it only when implementation or evidence changes the argument:

- **Supported now:** a platform-neutral semantic tree is separated from distinct Blazor, SVG, and WPF manifestation code paths.
- **Supported narrowly:** current tests demonstrate selected rendering and serialization behavior for the covered vocabulary.
- **Not yet established:** universal representational compatibility, complete semantic parity, safe authorization of external resources, a GUI-owned execution/lifecycle system, and GPU rendering.
- **Validation method:** each stronger claim must link to an implementation, a test that asserts the claim, and the adapter/target scope. Otherwise label it as a hypothesis or future direction.

## Release decision record

- Candidate branch: pending
- Candidate commit: pending
- Release channel and versions: owner decision required
- Package archive hashes: pending for selected candidate
- CI run URLs: pending for selected candidate
- WebPage candidate-version result: pending
- Known limitations accepted: pending review
- Publication approval: **not granted**
