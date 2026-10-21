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
