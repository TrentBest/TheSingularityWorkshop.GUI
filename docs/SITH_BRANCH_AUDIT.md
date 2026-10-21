# Sith Branch Reconciliation Audit

**Status:** NOT CLEARED — do not delete non-keeper branches yet.  
**Audit date:** 2026-10-21  
**Policy target:** exactly `master` and `development`.  
**Purpose:** establish an honest disposition for every branch contribution, not preserve obsolete work for its own sake.

## Rules

- `development` is the active integration and documentation baseline; `master` is the stable branch.
- Every other branch must be classified as **retain/reconcile**, **rebuild**, **intentionally discard**, or **verified redundant**.
- A merged PR or a zero-ahead comparison is evidence, not automatic proof that a branch's full contribution is represented.
- Documentation must distinguish implementation, verified behavior, proposal, and unresolved work.
- Do not infer release readiness from branch cleanup. Package publication remains separately gated by the release-readiness checklist and explicit owner approval.
- The owner deletes non-keeper branches only after this audit identifies them as safe.

## Keeper branches

- `master`
- `development`

## Branch inventory and current disposition

| Branch | Current finding | Disposition |
|---|---|---|
| `architecture/datum-interaction-boundary` | Associated with open PR #8; architectural note has been reported as incorporated elsewhere, but final diff/PR disposition is not verified here. | Hold; compare, then close as superseded or reconcile. |
| `feature/integrated-fsm-cos-hub` | Legacy hub implementation differs from the current host-owned lifecycle direction. | Decide whether any useful contract should be rebuilt; otherwise explicitly discard. |
| `feature/nested-experiences` | Legacy nested-experience/hub behavior; current direction places runtime lifecycle with the consuming host. | Decide whether any useful contract should be rebuilt; otherwise explicitly discard. |
| `fix/guinode-nullability-contract` | PR #13 reported merged; `src/Core/GuiNode.cs` still differs from current `development`. | Hold until exact diff is reviewed; merge status alone is insufficient. |
| `work/gui-publish-gate-reconciled` | Open PR #19; newer-history candidate for publish-input gate change. Current workflow still includes `&& false`, so publication remains disabled. | Keep until PR #19 is resolved deliberately; never remove the hard stop without explicit release authorization. |
| `work/gui-release-mit-wpf-scope` | Reported 16 commits ahead; contains release/project/workflow scope changes. | Hold and compare unique content against current release checklist and development. |
| `work/gui-svg-source-uml-generator` | Open draft PR #18; broad source-derived UML effort, latest readiness estimate 72% and not release-ready. | Keep while actively reconciling; verify generated SVG and exact-head CI before disposition. |
| `work/gui-uml-per-file-documentation` | Per-type UML documentation differs from `development`; likely overlaps PR #18. | Compare all unique documentation and assets with PR #18 before closing/discarding. |
| `work/gui-verify-xml-docs-in-packages` | PR #17 reported merged, but package workflow differs from current `development`. | Hold until workflow changes are compared and accounted for. |
| `work/gui-wpf-tests` | Comparison reported zero commits ahead and no changed files versus `development`. | Verified-redundant candidate; recheck head/base immediately before owner deletion. |
| `work/nuget-publish-gate` | Open PR #15; older-history candidate for publish-input gate change, overlaps PR #19. | Resolve duplication against PR #19 before branch removal. |
| `work/svg-app-fsm-cos-boundary` | PR #12 reported merged; `docs/SVG_APP_CONCEPT.md` reported identical to `development`. | Strong redundant candidate; verify complete branch diff and PR state. |
| `work/svg-guide-current-contract` | Open PR #11; SVG guide work overlaps the current-source guide in PR #18. | Hold until all unique prose/assets are compared and PR #11 is dispositioned. |
| `work/svg-tree-depth-guard` | PR #10 reported merged; renderer and tests still differ from `development`. | Hold; determine whether changes are superseded or a regression fix is missing. |
| `work/webpage-event-builder-compatibility` | PR #9 reported merged; event test is identical but builder source still differs from `development`. | Hold until source diff is reviewed. |

## Open pull requests requiring explicit disposition

| PR | Head branch | Current disposition |
|---|---|---|
| [#8 — Datum/interaction boundary](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/8) | `architecture/datum-interaction-boundary` | Reconcile or close as superseded after content check. |
| [#11 — SVG guide current contract](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/11) | `work/svg-guide-current-contract` | Reconcile against current guide and PR #18. |
| [#15 — NuGet publish gate](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/15) | `work/nuget-publish-gate` | Compare with PR #19; keep only one canonical proposal. |
| [#18 — Source-derived GUI.SVG diagram bridge](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/18) | `work/gui-svg-source-uml-generator` | Keep draft while evidence and implementation are reconciled. |
| [#19 — Publish input on current development](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/19) | `work/gui-publish-gate-reconciled` | Resolve against PR #15; publication remains disabled pending approval. |

## Release and workflow truth

The current `.github/workflows/package.yml` on `development` has the NuGet publish job gated by:

```yaml
if: ${{ github.event_name == 'workflow_dispatch' && inputs.publish == true && false }}
```

The `&& false` term intentionally keeps publication disabled. The workflow requires manual dispatch and `publish: true`, but those conditions alone do not enable publication while the hard stop remains. Do not describe the publish job as publish-ready or remove this hard stop as part of branch cleanup.

The current `docs/RELEASE_READINESS.md` reports a qualitative 70% estimate after per-type documentation/UML became a hard gate, and explicitly states that the package family is not release-ready. Treat historical run links as evidence for their stated commits only; do not represent old runs as validation of today's candidate.

## All-clear criteria

The owner may implement the two-branch policy only when:

- [ ] Every non-keeper branch has a documented disposition.
- [ ] Every open PR whose head branch will be removed is merged, closed as obsolete, or otherwise explicitly resolved.
- [ ] Unique code, tests, docs, assets, and workflow changes have either been reconciled or explicitly marked for discard.
- [ ] The `development` branch's documentation matches current implementation and clearly labels proposals/unverified behavior.
- [ ] Current branch inventory and relevant PR states are rechecked immediately before deletion.
- [ ] No package publication or release authorization is inferred from the audit.

**Current decision: NOT CLEARED.** This document records findings and gaps; it is not a claim that every branch has been fully diff-audited.


## Exact-diff follow-up — 2026-10-21

The following comparisons were run against the current `development` tip `2c09e01d68cbca42c614f40f7cac22dba6806c40`; these are branch-tip comparisons, not assumptions based on merged PR labels.

- **`fix/guinode-nullability-contract` — verified superseded candidate.** The branch's only unique file change is `src/Core/GuiNode.cs`. Its recursive `TryFind` implementation is replaced on development by iterative depth-first traversal using a stack, preserving left-to-right traversal order while avoiding recursive traversal depth. The branch version also lacks the XML documentation now present on development. The one-line nullable-contract-era branch change is not a reason to preserve its old implementation. Classify as intentionally discardable once the current inventory is rechecked.
- **`work/svg-tree-depth-guard` — guard behavior exists on development, but the branch still has unique test/docs edits.** Development's `SvgGuiRenderer` already has `MaximumTreeDepth = 256` and iterative prevalidation/unique-ID checks. The branch adds 25 lines to `tests/SVG.Tests/SvgGuiRendererTests.cs` and alters `docs/SVG_ADAPTER.md`. These exact test and documentation hunks still need review against the current tests/guide before discard; do not treat the branch as redundant yet.
- **`work/webpage-event-builder-compatibility` — unresolved unique source change.** The branch adds two lines to `src/Blazor/WorkshopGuiBuilder.cs` and adds a 33-line event test. Even though the branch test had previously been reported as identical to a test on development, the current compare reports the test as added relative to the merge base. Inspect the exact test/source semantics before disposition; hold for now.
- **`work/gui-verify-xml-docs-in-packages` — workflow hard-stop is not resolved by this branch.** Its workflow still contains `&& false`, matching the current deliberate publication hard stop. Its unique diff also updates release docs/work queue. Review those prose changes, but do not treat this as the publish-gate fix or as authorization to enable publication.
- **`work/gui-publish-gate-reconciled` vs `work/nuget-publish-gate` — not a clean one-commit comparison.** The publish-gate branch comparison is heavily history-diverged (228 commits in the comparison output) and includes many files beyond the one-line gate change. This is a sign to compare the actual PR #19 patch against the one-line PR #15 change, not to merge a branch wholesale. Keep publication disabled until explicit owner approval and exact-head CI verification.
- **`work/svg-guide-current-contract` — still unsafe to delete based on the old guide alone.** The branch guide references a `master` baseline and assets/paths not verified on current development; development has a newer current-source guide elsewhere in the branch-reconciliation work, but this fetch found no `docs/SVG_GUIDE.md` at the current development ref. Reconcile the current guide's actual path and content before closing PR #11 or deleting its head.
- **PR #18 is still open and draft.** Its current head is `b4c68259a2df7c6f565708a529b01e62c433dae0`; it is not mergeable according to PR metadata. Keep it while source-derived UML is actively being reconciled. Its 72% figure is a qualitative readiness estimate for that staging vertical slice, not repository-wide release readiness.
- **PRs #8, #11, #15, #18, and #19 remain open.** The current GitHub inventory still has 17 branches. No branches were deleted, and no PRs were merged or closed in this pass.

### Current safe-to-delete assessment

`work/gui-wpf-tests` remains the only previously identified zero-ahead/no-diff candidate, and it still requires an immediate final recheck. `fix/guinode-nullability-contract` now has enough exact-diff evidence to classify as intentionally superseded, but the owner should still close/resolve its historical PR state if applicable before deleting. All other non-keeper branches remain on hold pending the listed content checks or explicit decisions to discard legacy implementation. **The Sith policy is not cleared.**


## Live verification correction — 2026-10-21 17:50 UTC

A fresh live check of the current branch tips exposed discrepancies between the prior narrative and the repository state. This section supersedes any earlier claim that the two named guide files are already present on `development`.

### Verified current inventory

- The branch search returned 17 branches: the two keepers (`master`, `development`) and 15 non-keeper branches.
- The open PR search returned #8, #11, #15, #18, and #19. All five remain open; #8 and #18 are drafts. No PR was merged or closed during this check.
- The `development` package workflow still contains `github.event_name == 'workflow_dispatch' && inputs.publish == true && false`. NuGet publication therefore remains disabled.
- Fresh contents-API lookups for `docs/DATUM_AND_INTERACTION_BOUNDARY.md` and `docs/SVG_GUIDE.md` on `development` both returned 404. The previous section saying those documents were preserved on `development` is not supported by the live tree and must not be relied on. Their contents exist in the historical PR/branch patch, but they have not been verified as present on the keeper branch.

### Rechecked branch-tip comparison results

Comparisons below use `development` as base. Ahead/behind counts are GitHub comparison metadata; file lists are the reported changed files in that comparison, not a substitute for reviewing each patch.

| Branch | Ahead | Behind | Current changed-file finding | Disposition |
|---|---:|---:|---|---|
| `work/gui-wpf-tests` | 0 | 232 | No changed files returned | Redundant-tip candidate; recheck before deletion |
| `fix/guinode-nullability-contract` | 1 | 416 | `src/Core/GuiNode.cs` only | Likely superseded; prior iterative traversal finding still requires final source check |
| `work/svg-app-fsm-cos-boundary` | 5 | 416 | README and `docs/SVG_APP_CONCEPT.md` | Hold until complete document/PR disposition is verified |
| `work/webpage-event-builder-compatibility` | 3 | 479 | Blazor builder source (+2) and a 33-line event test | Hold; inspect behavior and whether test is covered elsewhere |
| `work/svg-tree-depth-guard` | 5 | 416 | SVG adapter docs, renderer (+22/-12), tests (+25) | Hold; source delta means the earlier “behavior exists on development” statement is not enough |
| `work/gui-uml-per-file-documentation` | 1 | 165 | `docs/DATA_CARDS_AND_UML.md` (+19 lines) | Hold; compare against current PR #18 docs before discard |
| `work/gui-release-mit-wpf-scope` | 16 | 233 | Workflow, release docs, project files, LICENSE/WPF README | Hold; classify unique release and package changes |
| `feature/integrated-fsm-cos-hub` | 76 | 485 | Broad docs/workflow/core/Blazor/WPF/test changes | Explicit legacy feature disposition required; do not merge wholesale |
| `feature/nested-experiences` | 38 | 485 | Nested-experience docs, Core/Blazor/tests/workflow | Explicit legacy feature disposition required |
| `work/gui-verify-xml-docs-in-packages` | 4 | 232 | Package workflow and release docs | Hold; its workflow still includes the deliberate publish hard stop |
| `architecture/datum-interaction-boundary` | 62 | 485 | Broad architecture, workflow, Core/Blazor/WPF/tests plus datum note | PR #8 is not a narrow doc-only change; review individual reusable principles, do not merge wholesale |
| `work/svg-guide-current-contract` | 112 | 485 | Broad historical changes including SVG guide and concept assets | PR #11 remains open; compare guide/assets against current implementation and PR #18 |
| `work/nuget-publish-gate` | 1 | 233 | One workflow line | PR #15 proposes removing `&& false`; not approved |
| `work/gui-publish-gate-reconciled` | 1 | 6 | One workflow line | PR #19 proposes the same hard-stop removal on newer history; not approved |

### Open PRs — live metadata

- [#8](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/8): open draft, head `43449f49607798d23c951ffd50650da268456540`, 50 changed files.
- [#11](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/11): open, non-draft, head `01ed8175bf2971f98e7fb324885c9e79103d0fac`, 59 changed files.
- [#15](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/15): open, non-draft, one changed file.
- [#18](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/18): open draft, head `b4c68259a2df7c6f565708a529b01e62c433dae0`, 86 changed files. Its own description states 72% qualitative readiness and that it is a staging vertical slice, not a completed supported package; exact current-head verification is pending per the PR text.
- [#19](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/19): open, non-draft, one changed file.

PR #15 and #19 each propose the same substantive one-line change removing the hard stop. Do not apply either as a side effect of branch cleanup. A release-gate decision is separate and requires explicit owner authorization.

### Next audit actions

1. Recover/recreate the datum-boundary note and current-source SVG guide on `development` only after reviewing their exact contents and current implementation. Until then, update links and status statements so docs do not claim the files exist.
2. Inspect the exact source/test hunks for the Blazor event-compatibility and SVG tree-depth branches.
3. Compare PR #18's per-file UML docs and generated/source-derived outputs against the keeper tree; distinguish authored documentation from generated evidence.
4. Review the unique workflow/project/release changes on the release-scope and XML-docs branches without enabling publishing.
5. Make an explicit keep/rebuild/discard decision for both legacy hub/experience branches.
6. Re-run branch and PR inventories at the end. Only then issue the owner-facing deletion list.

**Decision remains NOT CLEARED. No branch deletion, PR merge/closure, package publish, or workflow-gate change was performed by this check.**


## Source/test reconciliation follow-up — 2026-10-21

### Blazor event-compatibility branch

- `tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs` on both `development` and `work/webpage-event-builder-compatibility` has the exact same blob SHA: `be6c179c751d8f817293f4f445584df23675afd5`. The event test is therefore already present byte-for-byte on development.
- The branch version of `src/Blazor/WorkshopGuiBuilder.cs` has blob SHA `e464e2ec815c1cfd090f29ac965e320782fe0d99`; development's file is `02c7f344eb814081e38b4935e6a75b1a3f8cbb4d`. Inspection confirms the relevant `OnWheel` and `StopPropagation` implementations exist on both; the current development version adds XML documentation and also has a `PreventDefault` API in the surrounding event-builder surface.
- **Revised disposition:** likely discardable as an obsolete branch tip, because its event behavior and exact test are already on development. Before final all-clear, check the rest of its three-commit branch diff for any other changed files and confirm PR #9's final state; do not preserve the branch solely for the wheel/propagation test.

### SVG depth-guard branch

- The branch's `src/SVG/SvgGuiRenderer.cs` blob SHA is `b9bca133c0541f838e7d47a61163df4d17a41155`; development's is `88262776417a8d4138556e256c85386cbafba682`.
- Both versions include the 256-depth constant and iterative `ValidateUniqueIds` prevalidation. Development additionally assigns stable `gui-svg-title` / `gui-svg-description` IDs and `aria-labelledby` linkage; the branch version emits a title without the current ID linkage. The branch renderer is therefore older in at least this accessibility contract, not a candidate to restore wholesale.
- The branch test file has unique additions (25 lines in the compare), while the current development test file differs substantially in content. The added assertions must be reviewed individually; existing shared behavior does not mean every branch test is redundant.
- **Revised disposition:** retain only as a source for potential missing regression cases; do not merge or copy the older renderer. After extracting/adding any valuable tests to development and verifying them on the keeper branch, the old branch can be classified as intentionally discardable.

These findings narrow two branches but do not satisfy the global all-clear criteria. The remaining branch and PR inventory must still be reconciled and rechecked.
