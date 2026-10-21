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
