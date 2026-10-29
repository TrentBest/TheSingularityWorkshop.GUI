<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# Operation Sith — GUI Branch Reconciliation

**Audit date:** 2026-10-29  
**Audited repository:** `TrentBest/TheSingularityWorkshop.GUI`  
**Audited development head:** [`f5ad0f2d5020b13852b4fd43ee23b3be00c86eaf`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/f5ad0f2d5020b13852b4fd43ee23b3be00c86eaf)  
**Canonical branch policy:** exactly `master` and `development`, as defined by [SITH_POLICY.md](https://github.com/TrentBest/TheSingularityWorkshop/blob/development/SITH_POLICY.md).

This is a point-in-time rescue ledger, not authorization to merge, close, delete, force-push, release, deploy, or publish. The repository owner performs branch deletion manually after reviewing the exact `owner/repository:branch` recommendations below.

**Snapshot timing:** the 30-branch / 16-PR inventory below was captured before creating this audit's temporary branch and PR. The audit itself therefore temporarily adds one branch and one open draft PR (#64); the live count immediately after creation became 31 branches and 17 open PRs. Subsequent documentation commits on `development` established the explicit domain-definition release gate, added the evidence-oriented coverage map, linked it from the README, and integrated the authored Core/Blazor/SVG/WPF documentation from PRs #36–#38 in commit [`8d11c5f`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/8d11c5f449b73b519cfb1819c9c3942307f717c6). This PR's base is therefore behind the current documentation-only updates. Re-audit the live inventory and rerun CI before treating this ledger as current. The temporary audit branch is not intended to remain after the work is integrated and the owner manually cleans up.

## 01 — What the audit found

At the audit snapshot, GitHub reported **30 branches: the two canonical branches plus 28 noncanonical branches**. There were **16 open pull requests, all drafts**. This is a material Sith-policy violation and a work-management problem: useful implementation, tests, domain documentation, release evidence, and generated UML work are split across branches at different points in the history.

The current branch heads are:

- `development`: `f5ad0f2d5020b13852b4fd43ee23b3be00c86eaf`
- `master`: `36342df0654d6960f0ea36d3410d135366c09d1c`

A GitHub compare of `master...development` reports **811 commits ahead and 124 behind**, with divergent history. This is not a routine fast-forward situation. Do not merge, reset, or force-update `master` as part of branch cleanup. Preserve the released baseline until the owner explicitly chooses a history/release reconciliation strategy.

### Evidence and limits

- Branch names and head SHAs were read from GitHub's branch inventory.
- Each noncanonical branch was compared with `development` using GitHub's commit-compare endpoint. “Ahead” means commits not reachable from the current `development` head according to that comparison; it does not by itself prove that every changed line is useful or should be merged.
- Open PRs were enumerated separately and matched to their head branches.
- This is not yet a full source-level review of every unique commit or a CI re-run of every branch. Branches with unique commits remain **not approved for deletion** until the work is reviewed, integrated, or explicitly dispositioned.

## 02 — Branch inventory and preliminary disposition

Counts below are relative to `development` at the audit head above. “Ahead/behind” is a comparison result, not a quality score.

| Branch | Ahead / behind | PR | Preliminary disposition |
|---|---:|---:|---|
| `docs/charting-contract-adr-2026-10-23` | 4 / 8 | [#43](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/43) | Preserve for review; proposed charting boundary/ADR. No chart API is claimed. |
| `docs/ci-test-coverage-matrix-2026-10-23` | 1 / 8 | [#45](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/45) | Preserve for review; CI evidence and test-scope clarification. |
| `docs/fsm-driven-oscillator` | 8 / 8 | [#47](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/47) | Preserve for review; runnable FSM-driven oscillator sample, recipe, and workflow changes. Validate workflow impact before integration. |
| `docs/full-gui-domain-coverage-2026-10-23` | 4 / 16 | [#40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40) | Its useful domain map is now reconciled into current `development` as `docs/FULL_GUI_DOMAIN_COVERAGE.md`; README and release-gate links are also integrated. PR #40 is still open and its branch is an older snapshot. Do not merge blindly; review its remaining diff and disposition the stale PR/branch. |
| `docs/gui-blazor-svg-authored-contracts-2026-10-23` | 1 / 12 | [#37](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/37) | Its authored Blazor/SVG contracts and source companions have since been integrated into `development` at `8d11c5f`; the old PR/branch remains open and must be dispositioned rather than merged blindly. |
| `docs/gui-core-authored-contracts-2026-10-23` | 7 / 12 | [#36](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/36) | Its authored Core contracts/use cases and source companions have since been integrated into `development` at `8d11c5f`; the old PR/branch remains open and must be dispositioned rather than merged blindly. |
| `docs/gui-wpf-authored-contracts-2026-10-23` | 1 / 12 | [#38](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/38) | Its authored WPF renderer/window-builder contracts and source companions have since been integrated into `development` at `8d11c5f`; the old PR/branch remains open and must be dispositioned rather than merged blindly. |
| `docs/uml-three-view-standard-2026-10-23` | 2 / 12 | [#39](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/39) | Preserve; UML view/data-card standard. Reconcile with the current FSM_COS standard before adopting. |
| `feature/architect-renderer-integration` | 16 / 8 | [#48](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/48) | Preserve for source-level review; WPF/Architect host-theme integration and related generator/docs changes. |
| `fix/svg-node-count-limit-2026-10-23` | 2 / 12 | [#35](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/35) | Preserve; SVG scene node-count resource guard and regression test. |
| `release/gui-closeout-2026-10-22` | 13 / 13 | [#32](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/32) | Preserve for reconciliation; contains release-closeout, evidence-boundary theory, and readiness changes. Do not treat the old readiness snapshot as current. |
| `release/gui-root-id-contract-2026-10-22` | 9 / 12 | [#34](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/34) | Preserve; SVG root identity/accessibility-ID contract and tests. |
| `tests/core-builder-cycle-regression-2026-10-23` | 5 / 8 | [#41](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/41) | Preserve; malformed-child and direct/indirect builder-cycle regression coverage. |
| `work/automatic-uml-documentation-promotion` | 1 / 23 | — | Review unique workflow change against the current generated-doc promotion path; likely superseded, but not deletion-ready yet. |
| `work/fix-promoted-uml-index-links` | 3 / 20 | — | Review generator/index-link changes and tests against current committed inventories before disposition. |
| `work/fix-uml-promotion-generator-manifest` | 2 / 22 | — | Review manifest safety fix against the current promotion workflow and tests. |
| `work/fix-uml-self-reference-clearance-2026-10-27` | 7 / 6 | [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55) | Preserve for reconciliation; overlaps the newer neighborhood connector fix in #60. Compare actual rendering/tests and retain one coherent fix. |
| `work/four-package-uml-promotion` | 19 / 24 | — | Review promotion generator/tests; likely contains useful intermediate work, but current promotion workflow and generated inventory already exist. |
| `work/record-committed-uml-inventory` | 7 / 20 | — | Review documentation/evidence edits against the current 69-type committed inventory; avoid duplicating stale history. |
| `work/record-current-package-archive-evidence` | 2 / 16 | — | Review readiness/queue edits; candidate-specific archive evidence is still a separate release gate. |
| `work/record-svg-vocabulary-coverage` | 0 / 13 | — | **READY FOR OWNER DELETION** based on this snapshot: no unique commits relative to `development`, no open PR, and all branch commits are reachable from `development`. Owner should recheck the head before deleting. |
| `work/record-svg-vocabulary-coverage-status` | 2 / 13 | — | Review status/queue changes against current SVG vocabulary-to-test coverage; may be stale evidence history. |
| `work/svg-vocabulary-test-coverage` | 2 / 14 | — | Review source/test changes against current explicit-mapping coverage before disposition. |
| `work/trim-release-evidence-history` | 2 / 15 | — | Review release-history trimming against the current evidence standard; do not discard unique facts without checking. |
| `work/uml-neighborhood-self-reference-clearance-2026-10-28` | 4 / 4 | [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60) | Preserve; newer focal-card connector clearance fix and regression test. Requires fresh SVG visual review and CI. |
| `work/uml-visibility-tier-data-cards-2026-10-28` | 3 / 4 | [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57) | Preserve; clarify visibility tiers and colorful data cards against the canonical standard. |
| `work/uml-visual-data-cards-2026-10-27` | 19 / 6 | [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50) | Preserve; substantive nested data-card renderer/tests. Review alongside #57 and both self-reference fixes before integration. |
| `work/wpf-package-only-consumer` | 5 / 22 | — | Review workflow change against the current Windows archive-only consumer step; avoid retaining duplicate or obsolete workflow logic. |

### Canonical branches

- `TrentBest/TheSingularityWorkshop.GUI:master` — **KEEP**. Released baseline; do not rewrite it during rescue.
- `TrentBest/TheSingularityWorkshop.GUI:development` — **KEEP**. Active work line and target for reviewed integration.

### READY FOR OWNER DELETION

Only this branch meets the evidence threshold from the current snapshot:

- `TrentBest/TheSingularityWorkshop.GUI:work/record-svg-vocabulary-coverage`

No other noncanonical branch is currently certified ready for deletion in this audit. This is intentionally conservative: the remaining branches have unique commits, open PRs, or both. That does **not** mean every branch deserves to survive; it means the unique work needs a disposition first.

## 03 — Open pull requests

All 16 open PRs were drafts at the audit snapshot. Their unique commits must not be discarded merely because they are old or behind `development`.

- Domain/documentation candidates: [#40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40), [#36](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/36), [#37](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/37), [#38](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/38), [#39](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/39), [#43](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/43), [#45](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/45), [#47](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/47).
- Implementation/renderer candidates: [#34](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/34), [#35](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/35), [#41](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/41), [#48](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/48).
- UML/data-card candidates: [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50), [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55), [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57), [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60).
- Release closeout candidate: [#32](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/32).

These are not 16 independent pieces of work. Several overlap or represent earlier stages of a newer branch. In particular, #55 and #60 overlap on UML self-reference routing; #50 and #57 overlap on data-card presentation; #32 and the readiness/evidence branches overlap on release closeout. Reconcile by behavior, tests, documentation value, and current source—not by PR age or branch name.

## 04 — Sith publication gate

The current `.github/workflows/package.yml` contains a `publish_nuget` job with the condition:

```yaml
if: ${{ github.event_name == 'workflow_dispatch' && inputs.publish == true && false }}
```

That explicit `&& false` keeps publication disabled. **Do not remove it as part of this audit.** A new `Enforce Sith branch inventory before publication` step has been added to `development` (commit [`e12d793`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/e12d793e035c72eb4b84e701c23f10857ff603ca)). It queries the remote branch inventory and requires exactly `master` and `development`; an API failure or any extra/missing branch fails the step closed. The normal Package GUI and UML workflows passed on that commit, but the inventory step itself was **not executed** because `publish_nuget` remains hard-disabled.

Because the job remains hard-disabled, the branch check does not run as part of ordinary build/test/package runs. Keep the hard-disabled condition in place. Before anyone ever re-enables publication, verify this inventory step in CI and also ensure the reviewed candidate is the intended source, policy evidence is current, and explicit owner authorization has been supplied. Build, test, pack, archive inspection, and consumer verification may continue independently. No NuGet publication is authorized here.

## 05 — Rescue sequence

1. **Freeze the release path.** Keep `publish_nuget` hard-disabled; do not select package versions or a release candidate yet.
2. **Define the domain first.** Reconcile the GUI taxonomy, ideal model, semantic Core contract, interaction/input boundaries, lifecycle, spatial representation, renderer/adapter ownership, SVG/Visualize boundary, host constraints, and explicit non-goals. Use the FSM_COS [Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/DOCUMENTATION_STANDARD.md) as the living authority. Track every domain claim as implemented, tested, specified, proposed, externally owned, or unresolved.
3. **Integrate unique value into one line.** Review PRs and orphaned branches in small coherent groups. Keep exact source/test/doc evidence, avoid blindly merging stale histories, and rerun CI on the resulting `development` head.
4. **Complete user-facing documentation.** Build the modular theory path, per-type contracts, companion UML/use cases, runnable recipes, gallery and test-evidence map. Generated type inventory is valuable, but it does not replace human-authored purpose and use cases.
5. **Re-audit branches after integration.** Compare every surviving branch to the new `development` head. Name exact branches **READY FOR OWNER DELETION** only after their unique work is integrated or explicitly rejected and no open PR/reference remains.
6. **Keep the release decision separate.** Only after the domain and documentation are complete should the team define package versions/channel, select a candidate, run candidate-specific archive-only consumer checks, verify downstream WebPage integration, and seek owner approval.

## Next action

Continue the source-level branch review and consolidate the highest-value domain-definition/documentation work before considering a release. Do not merge or close PRs, delete branches, alter `master`, publish packages, or deploy as part of this ledger.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
