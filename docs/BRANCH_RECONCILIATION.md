## 0.3 Live reconciliation refresh — 2026-11-21 09:07 UTC

**Snapshot base:** [development `0e7cd26d3c163ce841429ad2d07cad6718a2378d`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/0e7cd26d3c163ce841429ad2d07cad6718a2378d).  
**Master:** [`31bad8d8fec333a4c940b189f99e75deffb94473`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/31bad8d8fec333a4c940b189f99e75deffb94473).  
**Live inventory:** **26 branches** = canonical `master` + `development` and **24 noncanonical branches**; **24 open PRs** map one-to-one to those noncanonical branches. The Sith two-branch gate is still failing. No noncanonical branch is currently safe to delete because every one still has an open PR requiring explicit disposition.

This refresh supersedes the 2026-11-20 05:16 UTC snapshot and earlier inventory tables. The branch count did not change, but `development` advanced from the prior recorded head. Findings below are based on live branch/PR inventory and source-level inspection of the current `development` tree. A changed filename or diverged ancestry is not, by itself, evidence of missing work.

### Current PR/branch disposition assessment

| PR | Branch | Current finding against development | Disposition |
|---|---|---|---|
| [#83](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/83) | `work/integrate-ciai-pr-evidence` | PR-triggered exact CiAi evidence lane is present on this branch and has exact-head successful evidence. Required-check enforcement remains unverified because the integration could not read classic branch protection. | **Keep for owner review.** Successful check production is proven; merge blocking is not. |
| [#82](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/82) | `work/integrate-autonomous-design-presentation` | Ports the Autonomous Design badge assets, contract, gallery and Pages artifact checks onto the current .NET 10 line. Exact-head workflows passed; deploy and publish were skipped. | **Keep for owner review.** Consolidated presentation port of #77. |
| [#81](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/81) | `work/integrate-nested-uml-data-cards` | Exact two-file renderer/test patch matches the unique change set from #50, ported onto the newer development baseline. Exact-head package, WPF, diagram and showcase-build evidence passed. | **Keep for owner review.** This is the current-baseline port; do not separately port #50 again. |
| [#80](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/80) | `work/ciai-evidence-lane-reconciled` | Its `.github/workflows/ciai-evidence.yml` blob is byte-identical to the same path on #83 (`d28966726cbce237444e3304015f86cfc8145965`). #83 carries that implementation on the newer base and validates the PR event path. | **Duplicate implementation;** retain until owner disposes the older PR/branch. |
| [#77](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/77) | `work/autonomous-design-badge` | Its unique visual/contract work is consolidated by #82; no live runtime-status schema is introduced by either presentation tranche. | **Superseded by #82 for integration;** owner disposition still required. |
| [#65](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/65) | `work/adapter-conformance-three-way-2026-10-30` | Development contains the shared Blazor/SVG fixture, WPF nested identity/order/text fixture, WPF shared-adapter cases, and three-adapter evidence documentation. | **Implementation represented.** Historical exact-head status is not a substitute for fresh current-head checks; PR disposition remains owner-controlled. |
| [#64](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/64) | `work/sith-branch-reconciliation-2026-10-29` | This branch carries the reconciliation ledger. This update refreshes its stale snapshot using current live inventory and current source-level dispositions. | **Keep as the review vehicle for this ledger update.** Not deletion-ready until owner accepts the refreshed record and disposes the PR. |
| [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60) | `work/uml-neighborhood-self-reference-clearance-2026-10-28` | Its self-reference connector-clearance change is represented in the current renderer/test cluster and reviewed together with #55. | **Implementation represented;** preserve the paired review history until owner disposition. |
| [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57) | `work/uml-visibility-tier-data-cards-2026-10-28` | The visibility/member ordering proposal remains unresolved: current docs disagree on whether Static is a visibility tier or a modifier. | **Keep.** Requires owner decision and consistent semantic model, renderer, tests and docs before adoption. |
| [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55) | `work/fix-uml-self-reference-clearance-2026-10-27` | Self-reference routing, label/anchor clearance and related regression behavior are represented in development; #55/#60 were reviewed as one renderer cluster. | **Implementation represented;** keep paired review history until owner disposition. |
| [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50) | `work/uml-visual-data-cards-2026-10-27` | Its two-file nested-card renderer/test patch is identical to the patch ported by #81. | **Superseded by #81 for integration;** no duplicate port. |
| [#48](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/48) | `feature/architect-renderer-integration` | WPF host resource keys, dynamic theme resources, Row/orientation/isEnabled support, conformance tests and source-UML generator filtering are represented in the more complete development implementation. | **Implementation represented.** Retain until owner disposes the historical PR. |
| [#47](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/47) | `docs/fsm-driven-oscillator` | The FSM-driven bounded oscillator is present on development, migrated to .NET 10, and marked CI-verified/artifact-reviewed (Package GUI run [40961583029](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/40961583029)). The old branch project targets .NET 8. | **Superseded by the current .NET 10 implementation.** Preserve the old branch only until owner PR disposition. |
| [#45](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/45) | `docs/ci-test-coverage-matrix-2026-10-23` | Current `docs/TESTING.md` contains the adapter scopes, Windows-only WPF lane, semantic-ID fixture and current evidence in a more evolved form. | **Documentation represented/superseded.** |
| [#43](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/43) | `docs/charting-contract-adr-2026-10-23` | The charting semantic-contract ADR exists in an evolved form. Charting remains a proposed surface; no API/package is implied until consumer requirements and Core-versus-optional boundary are accepted. | **Proposal represented.** Keep the unresolved design boundary explicit. |
| [#41](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/41) | `tests/core-builder-cycle-regression-2026-10-23` | Current `GuiNode` rejects null child entries; builder tests cover direct/indirect cycles and null children; API/UML companion docs describe the behavior. | **Implementation, tests and documentation represented.** |
| [#40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40) | `docs/full-gui-domain-coverage-2026-10-23` | Current full-domain coverage map is evolved and remains a hard owner-acceptance/release gate. The release work queue does not convert this into a readiness percentage. | **Current domain map represented.** Owner acceptance remains open. |
| [#39](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/39) | `docs/uml-three-view-standard-2026-10-23` | The proposed canonical `docs/UML_VIEWS_AND_DATA_CARD_STANDARD.md` remains absent from development. Its taxonomy is useful, but member-order/visibility details conflict with unresolved #57. | **Keep as a design proposal.** Do not promote its normative ordering until #57 is resolved. |
| [#38](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/38) | `docs/gui-wpf-authored-contracts-2026-10-23` | WPF API/UML companion pages are present in development in longer, evolved versions. | **Documentation represented/superseded.** |
| [#37](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/37) | `docs/gui-blazor-svg-authored-contracts-2026-10-23` | Blazor/SVG API/UML companion pages and source companions are present in development in longer, evolved versions. | **Documentation represented/superseded.** |
| [#36](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/36) | `docs/gui-core-authored-contracts-2026-10-23` | Core API/UML companion pages and source companions are present in development in longer, evolved versions. | **Documentation represented/superseded.** |
| [#35](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/35) | `fix/svg-node-count-limit-2026-10-23` | Development contains the iterative 100,000-node pre-serialization guard and regression coverage for scenes above the limit. | **Implementation and regression coverage represented.** |
| [#34](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/34) | `release/gui-root-id-contract-2026-10-22` | Development preserves the root `GuiNode.Id` as the SVG document ID and tests both root identity and collisions with generated accessibility IDs; companion docs/gallery are current. | **Implementation, tests and documentation represented.** |
| [#32](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/32) | `release/gui-closeout-2026-10-22` | The semantic-tree-to-SVG use case, release-closeout/evidence-boundary theory chapter and Theory Guide additions are present. The camera/viewport/independent-view theory is incorporated into current theory. Exact-candidate archive/consumer proof and owner release acceptance remain separate gates. | **Documentation represented.** No release authorization implied. |

### What is safe to conclude now

- The branch-count policy is **not compliant**: 24 noncanonical branches remain, each mapped to an open PR.
- **No branch is ready for deletion today.** Even branches whose implementation or documentation is represented still have open PRs. Owner PR disposition must come first; recheck the exact branch head and references before any deletion.
- Consolidation pairs are clear: #81 carries #50's exact renderer/test patch; #82 consolidates #77's presentation work; #83 carries the exact CiAi evidence workflow blob also present on #80.
- The current .NET 10 oscillator proof (#47), Core tree-validation work (#41), SVG root-ID contract (#34), and SVG node-count guard (#35) are represented in development with current source/tests/docs.
- #39 and #57 remain a genuine design decision, not a cleanup-only task. Do not canonize conflicting UML visibility/member ordering.
- All exact-head CI claims remain commit-scoped. Historical branch green checks do not certify the current development head. NuGet publication and Pages deployment remain disabled unless separately authorized.
- This ledger update is committed only to the existing #64 review branch. It does not modify `development` or `master`, close/merge PRs, delete branches, publish packages, or deploy Pages.

<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# Operation Sith — GUI Branch Reconciliation

**Audit timestamp:** 2026-11-05 01:21 UTC  
**Repository:** `TrentBest/TheSingularityWorkshop.GUI`  
**Canonical branches:** `master`, `development` only  
**Development integration head for this update:** [`11777f576f3348ca34b082ddf431dd80944f857f`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/11777f576f3348ca34b082ddf431dd80944f857f)  
**Comparison base for the branch-diff snapshot:** [`c48d32c652cf00493da8c6a66625f8406961a7aa`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/c48d32c652cf00493da8c6a66625f8406961a7aa) (the development head immediately before this ledger was added)  
**Master head:** [`36342df0654d6960f0ea36d3410d135366c09d1c`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/36342df0654d6960f0ea36d3410d135366c09d1c)

This is a point-in-time reconciliation ledger for [Operation Sith issue #62](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/62). It is intended to make branch work findable and support owner-directed cleanup. It does **not** authorize branch deletion, PR closure/merge, force-push, release, deployment, or package publication.

## 0. Latest reconciliation update — 2026-11-05 01:21 UTC

- PR [#74](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/74), `work/ciai-evidence-lane`, adds only `.github/workflows/ciai-evidence.yml` (194 lines).
- The workflow was reviewed and copied unchanged to `development` in [commit `574d66a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/574d66a5fc6bd13e5adfc428a3375d3ce605d56a).
- GitHub reports the development and PR branch file blobs are identical: `944c0b4f8e47bfad115557c574d1e666b0b6a02b`.
- The workflow is read-only (`contents: read`, `actions: read`), runs on pushes to `development`, resolves the authoritative `package.yml` run by exact source SHA, run ID and attempt, and retains evidence artifacts. Existing package/publication workflows are not modified.
- The branch remains one commit ahead by ancestry and ten commits behind current development, but its only changed file is now content-identical on `development`. This is **content integrated, not ancestry merged**.
- The PR remains open pending a live development-push validation and explicit PR disposition. No merge, close, or branch deletion was performed.
- The development push's combined commit-status endpoint has not yet reported checks; this is **pending/unverified**, not a pass.

## 0.1. Adapter-conformance integration update

PR [#65](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/65) was reviewed against the live `development` files. Its four changed-file contributions have now been ported to `development` as four small commits ending at [`11777f5`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/11777f576f3348ca34b082ddf431dd80944f857f):

- `tests/Adapter.Conformance.Tests/SharedAdapterConformanceTests.cs`: added the unknown-kind fallback/descendant-preservation shared fixture.
- `tests/WPF.Tests/WpfGuiRendererConformanceTests.cs`: added the shared nested WPF identity/order/text fixture.
- `tests/WPF.Tests/WpfSharedAdapterConformanceTests.cs`: added the WPF-side shared semantic cases.
- `docs/TESTING.md`: updated the documented three-adapter evidence and explicit fallback behavior.

**Compile defect corrected during port:** PR #65's new `WpfSharedAdapterConformanceTests.cs` referenced `Border`, `StackPanel`, and `TextBlock` without importing `System.Windows.Controls`. The port adds that using. The original branch's exact-head WPF compile failure is therefore addressed in the development copy, but only a fresh Windows CI run can establish the fix.

PR #65 remains open; no merge or close action was taken. Current compare ancestry is 7 commits ahead / 69 behind development, and the four-file compare still reflects branch history rather than a clean ancestry integration. The relevant implementation has been ported, not cherry-picked. The new development commit's combined-status endpoint currently reports no statuses, so validation remains **pending/unverified**.

## 1. Current inventory

| Measure | Current result |
|---|---:|
| Total branches | **22** |
| Canonical branches | **2** |
| Noncanonical branches | **20** |
| Open pull requests | **20** |
| Open draft pull requests | **20** |
| Noncanonical branches without an open PR | **0** |
| Branches currently recommended for owner deletion | **0** |

Every noncanonical branch currently maps to an open draft PR. Therefore none is presently a deletion candidate, even if its unique changes may already be represented in `development`. First disposition the corresponding PR, then recheck the branch head and references before deletion.

GitHub's branch-name listing is not a useful modified-time view. The table below derives **last changed** from each branch head commit's commit timestamp and sorts newest first. This is the practical way to find recent branch work without relying on branch-name ordering.

## 2. Noncanonical branches — newest first

All comparisons below use `development` at `c48d32c652cf00493da8c6a66625f8406961a7aa` as the base. “Ahead” is the number of commits unique to the branch according to GitHub compare; “behind” is the number of commits unique to current development. Every comparison reports `diverged`. File lists are the current compare endpoint's changed-file list, not a full review of every historical commit.

| Last changed (UTC) | Branch | Ahead / behind | Open PR | Current unique file diff / work area |
|---|---|---:|---:|---|
| 2026-11-04 06:57 | `work/ciai-evidence-lane` | 1 / 10 | [#74](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/74) | `.github/workflows/ciai-evidence.yml` — **content integrated on development** (identical blob); ancestry/PR disposition still pending |
| 2026-10-30 20:22 | `work/adapter-conformance-three-way-2026-10-30` | 7 / 59 | [#65](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/65) | `docs/TESTING.md`; shared adapter conformance tests; WPF shared conformance tests |
| 2026-10-29 22:21 | `work/sith-branch-reconciliation-2026-10-29` | 22 / 99 | [#64](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/64) | `docs/BRANCH_RECONCILIATION.md` — prior rescue ledger; **this current ledger supersedes its stale snapshot** |
| 2026-10-28 20:28 | `work/uml-neighborhood-self-reference-clearance-2026-10-28` | 4 / 103 | [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60) | UML graph renderer and regression tests — self-reference routing |
| 2026-10-28 20:01 | `work/uml-visibility-tier-data-cards-2026-10-28` | 3 / 103 | [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57) | `docs/DATA_CARDS_AND_UML.md` — visibility-tier/data-card proposal |
| 2026-10-28 20:00 | `work/uml-visual-data-cards-2026-10-27` | 19 / 105 | [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50) | `src/SVG/SvgUmlClassDiagramRenderer.cs`; `tests/SVG.Tests/SvgUmlClassDiagramRendererTests.cs` — nested UML cards |
| 2026-10-28 05:12 | `work/fix-uml-self-reference-clearance-2026-10-27` | 7 / 105 | [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55) | Package workflow, UML graph renderer, regression tests — self-reference clearance |
| 2026-10-27 00:03 | `docs/uml-three-view-standard-2026-10-23` | 2 / 111 | [#39](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/39) | UML/class documentation standards — proposed hierarchy and view model |
| 2026-10-26 16:41 | `feature/architect-renderer-integration` | 16 / 107 | [#48](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/48) | WPF renderer/tests, Core docs, source UML model builder — host theme/layout integration |
| 2026-10-24 21:47 | `docs/fsm-driven-oscillator` | 8 / 107 | [#47](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/47) | Package workflow, oscillator sample, recipe docs — executable FSM-driven demonstration |
| 2026-10-23 23:35 | `docs/ci-test-coverage-matrix-2026-10-23` | 1 / 107 | [#45](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/45) | `docs/TESTING.md` — CI test execution matrix |
| 2026-10-23 23:27 | `docs/charting-contract-adr-2026-10-23` | 4 / 107 | [#43](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/43) | `docs/adr/CHARTING_SEMANTIC_CONTRACT.md` — proposed charting boundary |
| 2026-10-23 23:11 | `docs/full-gui-domain-coverage-2026-10-23` | 4 / 107 | [#40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40) | README, full GUI domain coverage, next-release queue |
| 2026-10-23 23:03 | `tests/core-builder-cycle-regression-2026-10-23` | 5 / 107 | [#41](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/41) | GuiNode docs/source and Core builder tests — null-child/cycle regression |
| 2026-10-23 03:11 | `docs/gui-blazor-svg-authored-contracts-2026-10-23` | 1 / 111 | [#37](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/37) | Blazor/SVG API contracts and UML/use-case docs |
| 2026-10-23 02:10 | `docs/gui-core-authored-contracts-2026-10-23` | 7 / 111 | [#36](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/36) | Core API contracts, UML/use cases, DataCard source companion |
| 2026-10-23 01:45 | `fix/svg-node-count-limit-2026-10-23` | 2 / 111 | [#35](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/35) | SVG renderer and tests — node-count resource guard |
| 2026-10-23 01:19 | `release/gui-closeout-2026-10-22` | 13 / 112 | [#32](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/32) | README, release queue/readiness, theory, use-case documentation |
| 2026-10-22 22:52 | `release/gui-root-id-contract-2026-10-22` | 9 / 111 | [#34](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/34) | SVG renderer, adapter conformance, SVG fallback/identity tests |

**Timestamp note:** these are commit timestamps on branch heads, not PR update timestamps or a claim that every file in a branch was last edited at that time. Rows are sorted by the timestamp; the October 28 entries are ordered by exact timestamp.

## 3. What the comparison tells us

- All 20 noncanonical branches have unique commits relative to the current `development` ancestry. This does **not** prove that their code/content is absent from development: several changes were integrated by cherry-pick or equivalent content reconciliation, so Git ancestry can still report divergence.
- The compare endpoint reports the following current changed-file sets:
  - #74: one workflow file.
  - #65: four files.
  - #64: one ledger file.
  - #60: two UML renderer/test files.
  - #57: one documentation file.
  - #55: three workflow/renderer/test files.
  - #50: two UML renderer/test files.
  - #48: six WPF/source-model/doc/test files.
  - #47: five sample/workflow/documentation files.
  - #45: one test documentation file.
  - #43: one chart ADR.
  - #40: three README/domain/queue files.
  - #41: four Core source/test/API/UML files.
  - #39: three UML standard files.
  - #38: six WPF API/UML/source-companion files.
  - #37: nine Blazor/SVG API/UML/source-companion files.
  - #36: eight Core API/UML/source-companion files.
  - #35: two SVG renderer/test files.
  - #34: six SVG renderer/conformance/test/gallery files.
  - #32: seven release/theory/use-case/README files.
- The old audit branch's own documentation describes many earlier PRs as already integrated. That is useful historical context, but it is not sufficient proof for current deletion: the comparison snapshot has advanced substantially, and each remaining PR still needs a deliberate disposition.
- **Exact-head GitHub Actions checks have now been queried for all 20 branch-head SHAs.** Results are recorded below. These are historical results for the branch-head commit, not proof that the same patch will pass against today's development base. Do not infer integration readiness from a green historical run.


### Exact branch-head CI snapshot

The following results come from GitHub Actions check-runs queried against each exact branch-head SHA on 2026-11-05. “Checks passed” means all reported non-publication checks completed successfully. In every run that exposed a `publish_nuget` job, that job was **skipped**. No NuGet package was published by these runs.

| PR | Branch head CI result |
|---|---|
| [#74](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/74) | **Passed** — `test-and-package`, `test-wpf`, `solution-uml`, `generate-and-verify`; publication skipped |
| [#65](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/65) | **Failed** — `test-wpf` failed; `test-and-package`, `solution-uml`, source diagrams, and documentation checks passed; publication skipped. [Package GUI run](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/41122606918) |
| [#64](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/64) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60) | **Passed** — package, WPF, source diagrams, solution UML; publication skipped |
| [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55) | **Passed** — package, WPF, source diagrams, solution UML; publication skipped |
| [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50) | **Passed** — package, WPF, source diagrams, solution UML; publication skipped |
| [#39](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/39) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#48](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/48) | **Passed** — package, WPF, source diagrams, solution UML; publication skipped |
| [#47](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/47) | **Passed** — package, WPF, source diagrams, solution UML; publication skipped |
| [#45](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/45) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#43](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/43) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#41](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/41) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#37](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/37) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#36](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/36) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#35](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/35) | **Passed** — package, WPF, source diagrams, solution UML; publication skipped |
| [#32](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/32) | **No check runs found** for the exact branch-head SHA; this is not a pass |
| [#34](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/34) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |
| [#38](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/38) | **Passed** — package, WPF, source diagrams, solution UML, and documentation checks; publication skipped |

#### Known failure detail — PR #65

The WPF test job failed to compile `tests/WPF.Tests/WpfSharedAdapterConformanceTests.cs`: references to `Border`, `StackPanel`, and `TextBlock` were unresolved (`CS0246`). The package workflow's cross-targeted WPF build succeeded, but the dedicated Windows WPF test job failed. This is a concrete branch fix required before #65 can be considered green. Keep its PR/branch open; do not delete it.

## 4. PR disposition queue

Every row below is an open draft PR, not an approved merge. Review in this order to reduce overlap and avoid losing newer work:

1. **Newest CI evidence lane:** [#74](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/74). Its workflow file is now content-integrated on development; verify the live development-push evidence and then disposition the still-open PR/branch explicitly.
2. **Adapter conformance:** [#65](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/65). Compare with the more recent shared Experience work now in development; do not assume identical purpose merely because both touch WPF conformance.
3. **UML implementation cluster:** [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50), [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55), [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60), and [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57). Review renderer behavior, regression tests, and generated artifacts together; preserve the owner's desired nested data-card visual language.
4. **WPF/Architect integration:** [#48](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/48). Verify exact current consumer behavior before disposition.
5. **Executable example:** [#47](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/47). Verify current sample/workflow behavior and generated artifact.
6. **Earlier domain, contract, and release documentation:** [#32](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/32), [#34](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/34), [#35](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/35), [#36](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/36), [#37](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/37), [#38](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/38), [#39](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/39), [#40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40), [#41](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/41), [#43](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/43), [#45](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/45).
7. **This reconciliation ledger:** [#64](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/64). This new `development` document supersedes the old branch snapshot; after verifying this file and its commit, the old PR/branch can be explicitly dispositioned by the owner.

## 5. Safe operating rules

- Work only on `development`; preserve `master` as the released baseline.
- Do not create more branches to perform this cleanup.
- Do not merge or close PRs automatically. A branch being behind, old, or apparently duplicated is not sufficient evidence.
- Do not delete branches automatically. The owner performs deletion after receiving exact `owner/repository:branch` recommendations.
- Before recommending deletion, verify all of the following against fresh GitHub state:
  1. No open PR or other active reference depends on the branch.
  2. No unique commits or file changes contain unpreserved work.
  3. Useful work is already integrated or explicitly rejected.
  4. Required CI/consumer evidence exists for the integrated result.
  5. The branch head has not moved since the audit.
- Keep NuGet publication disabled. Build, test, and pack may run independently; no publication without exact Sith compliance and explicit owner authorization.
- The publication gate must fail closed unless the remote branch inventory is exactly `master` and `development`, policy evidence is current, and the owner has explicitly authorized publication.

## 6. Next execution steps

1. **Exact-head CI audit complete:** all 20 noncanonical branch-head SHAs were queried; see the matrix above. PR #65's WPF compile failure and PR #32's missing check-run evidence remain explicit blockers.
2. Review the actual unique diffs in priority order; classify each as **already integrated**, **needs integration**, **preserve as open decision**, or **superseded/rejected**.
3. Integrate only reviewed, useful changes into `development`, with tests and consumer evidence where behavior changes.
4. Update this ledger after each reconciliation pass. Keep its table sorted by branch-head commit time, newest first.
5. Only after PR disposition and fresh comparison should any branch be labeled **READY FOR OWNER DELETION**.
6. Recheck the remote inventory after the owner performs cleanup; the target is exactly `master` and `development`.

**Current decision:** branch cleanup is not yet ready for owner deletion. The first actionable cleanup pass is PR disposition plus exact-head CI/diff review—not further feature implementation.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>


## 7. Live reconciliation refresh — 2026-11-16 03:55 UTC

This section supersedes the inventory, branch-count, and compare-base figures in the earlier sections above. The historical findings remain useful context; the table below is the current live snapshot.

**Live refs**
- `development`: `1a4967c3589798a922ec8688d5463d807f4f67bd` — “Document package-aware release branch semantics”.
- `master`: `bf4ed9e78a1c7683becadb53f9192419861aee4a` — “chore: refresh Sith policy evidence”.
- Current remote inventory: **23 branches = 2 canonical + 21 noncanonical**.
- Current open PR inventory: **21 open PRs; all 21 are drafts and each maps to one of the 21 noncanonical branches**.
- Current noncanonical deletion recommendations: **0**. Every noncanonical branch still has an open PR; all have commits not reachable from current `development` by ancestry.

### 7.1. Current branch-by-branch comparison

All comparisons below use current `development` SHA `1a4967c3589798a922ec8688d5463d807f4f67bd`. Every branch is diverged and has one or more commits ahead. Ahead/behind is ancestry evidence, not a verdict on whether equivalent content was separately integrated.

| Noncanonical branch | Open PR | Ahead / behind | Current branch-side file changes reported by compare |
|---|---:|---:|---|
| `docs/charting-contract-adr-2026-10-23` | [#43](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/43) | 4 / 155 | `docs/adr/CHARTING_SEMANTIC_CONTRACT.md` |
| `docs/ci-test-coverage-matrix-2026-10-23` | [#45](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/45) | 1 / 155 | `docs/TESTING.md` |
| `docs/fsm-driven-oscillator` | [#47](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/47) | 8 / 155 | Package workflow; oscillator project/program; demonstrations README and recipe |
| `docs/full-gui-domain-coverage-2026-10-23` | [#40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40) | 4 / 155 | README; `docs/FULL_GUI_DOMAIN_COVERAGE.md`; release work queue |
| `docs/gui-blazor-svg-authored-contracts-2026-10-23` | [#37](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/37) | 1 / 159 | Blazor/SVG API contracts, UML pages, and source companions (9 files) |
| `docs/gui-core-authored-contracts-2026-10-23` | [#36](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/36) | 7 / 159 | Core API contracts, UML pages, and DataCard source companion (8 files) |
| `docs/gui-wpf-authored-contracts-2026-10-23` | [#38](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/38) | 1 / 159 | WPF API contracts, UML pages, and source companions (6 files) |
| `docs/uml-three-view-standard-2026-10-23` | [#39](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/39) | 2 / 159 | Class documentation standard; UML views/data-card standard; UML index |
| `feature/architect-renderer-integration` | [#48](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/48) | 16 / 155 | Source-UML generator/model builder; Core/WPF docs; WPF renderer and conformance tests (6 files) |
| `fix/svg-node-count-limit-2026-10-23` | [#35](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/35) | 2 / 159 | SVG renderer and tests (2 files) |
| `release/gui-closeout-2026-10-22` | [#32](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/32) | 13 / 160 | README; release queue/readiness; theory guide/chapter; SVG use case (7 files) |
| `release/gui-root-id-contract-2026-10-22` | [#34](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/34) | 9 / 159 | SVG adapter docs/gallery; renderer; shared conformance and SVG tests (6 files) |
| `tests/core-builder-cycle-regression-2026-10-23` | [#41](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/41) | 5 / 155 | GuiNode source/API/UML and Core builder tests (4 files) |
| `work/adapter-conformance-three-way-2026-10-30` | [#65](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/65) | 8 / 107 | Testing docs; shared adapter and WPF conformance tests (4 files) |
| `work/autonomous-design-badge` | [#77](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/77) | 14 / 34 | Pages/Sith workflows; eight AD lifecycle/dependency SVGs; badge documentation and showcase pages (12 files) |
| `work/ciai-evidence-lane-reconciled` | [#80](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/80) | 3 / 2 | `.github/workflows/ciai-evidence.yml` (177 additions, 44 deletions) |
| `work/fix-uml-self-reference-clearance-2026-10-27` | [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55) | 7 / 153 | Package workflow; UML graph renderer and tests (3 files) |
| `work/sith-branch-reconciliation-2026-10-29` | [#64](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/64) | 22 / 147 | `docs/BRANCH_RECONCILIATION.md` — older ledger snapshot |
| `work/uml-neighborhood-self-reference-clearance-2026-10-28` | [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60) | 4 / 151 | UML graph renderer and tests (2 files) |
| `work/uml-visibility-tier-data-cards-2026-10-28` | [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57) | 3 / 151 | `docs/DATA_CARDS_AND_UML.md` |
| `work/uml-visual-data-cards-2026-10-27` | [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50) | 19 / 153 | UML class diagram renderer and tests (2 files) |

### 7.2. Current disposition classes

**Preserve for explicit review; do not delete**
- `work/ciai-evidence-lane-reconciled` / PR #80: current evidence-observer implementation is a substantial workflow diff, not the old content-identical PR #74 branch. PR #80 reports successful exact CiAi evidence and Package GUI evidence in its description, but remains draft/unmerged. Its own latest Sith run is not a substitute for branch reconciliation.
- `work/autonomous-design-badge` / PR #77: preserve the AD visual state assets and presentation contract. Foundry remains lifecycle authority; CiAi remains evidence/dependency authority; GUI owns presentation. This is not yet a live runtime integration.
- `work/adapter-conformance-three-way-2026-10-30` / PR #65: preserve for exact-head Windows WPF conformance review. Do not assume the historical compile failure is fixed without current exact-head Windows checks.
- `work/uml-visual-data-cards-2026-10-27` / PR #50 and `work/uml-visibility-tier-data-cards-2026-10-28` / PR #57: review together as one design/implementation cluster. Preserve the desired nested, colorful semantic data-card concept, but do not make an unresolved ordering proposal normative.
- `work/fix-uml-self-reference-clearance-2026-10-27` / PR #55 and `work/uml-neighborhood-self-reference-clearance-2026-10-28` / PR #60: review as one implementation/test cluster against the current generated SVG artifacts and development tests.

**Previously integrated or substantially reconciled; still require PR disposition**
- PRs #32, #34, #35, #36, #37, #38, #40, #41, #43, #45, #47, and #48 have historical integration notes above. The current compare still reports branch-side diffs and unique ancestry, so this ledger does not certify those branches for deletion. Verify current file equivalence and exact-head CI, then have the owner disposition the stale open PRs before any deletion recommendation.
- PR #39 remains an open design decision because the proposed UML member/visibility ordering has not been accepted as normative.
- PR #64's branch contains an older version of this ledger. This refresh supersedes its inventory snapshot, but the PR remains open and needs explicit owner disposition.

### 7.3. Sith workflow state

The latest live Sith Policy run is [43799113783](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/43799113783), on `development` SHA `1a4967c3589798a922ec8688d5463d807f4f67bd`, completed **failure** at 2026-11-16 03:35:41 UTC. Its failure is expected under the current 23-branch inventory: the repository has 21 extras.

The policy workflow itself is already strict: it queries all remote branches, requires exactly the sorted pair `development` and `master`, emits the live JSON/SVG badge evidence, and fails the job for every other state. The latest failure is not evidence that the policy is malfunctioning; it is the correct result for a non-compliant repository.

The separate benchmark GUI repository remains the clean reference: its live inventory is exactly `master` and `development`, and its Sith workflow has passed.

### 7.4. Updated execution plan

1. Keep this repository on `development`; do not create cleanup branches.
2. Treat #80, #77, #65, #50/#57, and #55/#60 as the first preservation/review clusters.
3. For older PRs whose useful work is already in development, verify current file equivalence and exact-head validation, then prepare an owner disposition list. Do not merge stale branches merely to close PRs.
4. Do not call any branch **READY FOR OWNER DELETION** while its PR remains open or its unique work has not been explicitly integrated/rejected.
5. Keep `master` unchanged. Keep package publication hard-disabled; this audit does not authorize publication.
6. Once the owner has dispositioned PRs and deleted a batch, rerun the full inventory. The only successful terminal state is exactly `master` + `development`.

**Current conclusion:** Operation Sith is being enforced correctly, but this repository is not yet reconciled. The next useful work is to resolve the preservation clusters and produce a smaller, evidence-backed PR/branch disposition batch—not to delete blindly.
