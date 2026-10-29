<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# Operation Sith — GUI Branch Reconciliation

**Audit date:** 2026-10-29  
**Audited repository:** `TrentBest/TheSingularityWorkshop.GUI`  
**Audited development head:** [`f5ad0f2d5020b13852b4fd43ee23b3be00c86eaf`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/f5ad0f2d5020b13852b4fd43ee23b3be00c86eaf)  
**Canonical branch policy:** exactly `master` and `development`, as defined by [SITH_POLICY.md](https://github.com/TrentBest/TheSingularityWorkshop/blob/development/SITH_POLICY.md).

This is a point-in-time rescue ledger, not authorization to merge, close, delete, force-push, release, deploy, or publish. The repository owner performs branch deletion manually after reviewing the exact `owner/repository:branch` recommendations below.

**Snapshot timing:** the 30-branch / 16-PR inventory below was captured before creating this audit's temporary branch and PR. The audit itself therefore temporarily adds one branch and one open draft PR (#64); the live count immediately after creation became 31 branches and 17 open PRs. Subsequent documentation commits on `development` established the explicit domain-definition release gate, added the evidence-oriented coverage map, linked it from the README, integrated the authored Core/Blazor/SVG/WPF documentation from PRs #36–#38 in commit [`8d11c5f`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/8d11c5f449b73b519cfb1819c9c3942307f717c6), and integrated the SVG root-ID/node-count and Core null-child/cycle-regression work from PRs #34, #35, and #41 in commit [`3844b48`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/3844b48b1a30f678cb8aa80c2a21aaaceed9348b), and added the executable FSM-driven oscillator recipe/workflow from PR #47 in commit [`f5c9a28`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/f5c9a2817cdcb9c9ab1ec235e7b67502bcd3b732), plus WPF host-resource/layout and source-inventory filtering from PR #48 in commit [`d015970`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/d01597026d83f798e62a2a144bfd9400a3cd44a8), and added the proposed charting boundary ADR from PR #43 at [`3bcc004`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/3bcc0043a8c4440ffcfa73c6e10b5e810c02abe1), then integrated the complete UML self-reference routing fix and focused regression/artifact path from PRs #55/#60 at [`0f256ac`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/0f256ace8cf4a63c92d5edefcf667f2ee0238ee4), and added the semantic-tree-to-SVG use case and evidence-boundary theory chapter from PR #32 at [`802fdce`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/802fdce0c9f0d3a0dbd1bda69116d72d974517fe). This PR's base is therefore behind the current documentation and code updates. Re-audit the live inventory and rerun CI before treating this ledger as current. The temporary audit branch is not intended to remain after the work is integrated and the owner manually cleans up.

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
| `docs/charting-contract-adr-2026-10-23` | 4 / 8 | [#43](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/43) | Its charting boundary analysis is integrated into `docs/adr/CHARTING_SEMANTIC_CONTRACT.md` as a still-Proposed ADR at `3bcc004`; source evidence and WPF mapping were refreshed. No chart API added. Old PR/branch remains open and must be dispositioned. |
| `docs/ci-test-coverage-matrix-2026-10-23` | 1 / 8 | [#45](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/45) | Its explicit CI execution matrix and solution-manifest caveat have since been integrated into `docs/TESTING.md` at `d3daa28`; the old PR/branch remains open and must be dispositioned, not merged blindly. |
| `docs/fsm-driven-oscillator` | 8 / 8 | [#47](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/47) | The executable FSM_API → GUI Core/SVG oscillator, recipe, and artifact-verification workflow have since been integrated into `development` at `f5c9a28`; the scene root was corrected so its grid/axes properties are effective. Package GUI run `40961583029` passed the executable/trace/SVG assertions and its generated SVG was visually reviewed; the recipe now records the evidence and static-snapshot limitation. Old PR/branch must be dispositioned, not merged blindly. |
| `docs/full-gui-domain-coverage-2026-10-23` | 4 / 16 | [#40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40) | Its useful domain map is now reconciled into current `development` as `docs/FULL_GUI_DOMAIN_COVERAGE.md`; README and release-gate links are also integrated. PR #40 is still open and its branch is an older snapshot. Do not merge blindly; review its remaining diff and disposition the stale PR/branch. |
| `docs/gui-blazor-svg-authored-contracts-2026-10-23` | 1 / 12 | [#37](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/37) | Its authored Blazor/SVG contracts and source companions have since been integrated into `development` at `8d11c5f`; the old PR/branch remains open and must be dispositioned rather than merged blindly. |
| `docs/gui-core-authored-contracts-2026-10-23` | 7 / 12 | [#36](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/36) | Its authored Core contracts/use cases and source companions have since been integrated into `development` at `8d11c5f`; the old PR/branch remains open and must be dispositioned rather than merged blindly. |
| `docs/gui-wpf-authored-contracts-2026-10-23` | 1 / 12 | [#38](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/38) | Its authored WPF renderer/window-builder contracts and source companions have since been integrated into `development` at `8d11c5f`; the old PR/branch remains open and must be dispositioned rather than merged blindly. |
| `docs/uml-three-view-standard-2026-10-23` | 2 / 12 | [#39](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/39) | Preserve; UML view/data-card standard. Reconcile with the current FSM_COS standard before adopting. |
| `feature/architect-renderer-integration` | 16 / 8 | [#48](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/48) | Preserve for source-level review; WPF/Architect host-theme integration and related generator/docs changes. |
| `fix/svg-node-count-limit-2026-10-23` | 2 / 12 | [#35](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/35) | Its 100,000-node guard and regression test have since been integrated with the root-ID fix into `development` at `3844b48`; old PR/branch remains open and must be dispositioned, not merged blindly. |
| `release/gui-closeout-2026-10-22` | 13 / 13 | [#32](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/32) | Preserve for reconciliation; contains release-closeout, evidence-boundary theory, and readiness changes. Do not treat the old readiness snapshot as current. |
| `release/gui-root-id-contract-2026-10-22` | 9 / 12 | [#34](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/34) | Root SVG identity/accessibility-ID collision fix, gallery artifact, and tests have since been integrated with the node-count guard into `development` at `3844b48`; old PR/branch remains open and must be dispositioned. |
| `tests/core-builder-cycle-regression-2026-10-23` | 5 / 8 | [#41](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/41) | Null-child rejection, direct/indirect builder-cycle tests, and matching API/UML docs have since been integrated into `development` at `3844b48`; old PR/branch remains open and must be dispositioned. |
| `work/automatic-uml-documentation-promotion` | 1 / 23 | — | Its only unique change is the older `SOURCE_REVISION: github.sha`/direct-push implementation. Current `promote-source-uml-docs.yml` captures the checked-out source revision, checks source drift, serializes runs, and safely rebases/retries. **Ready for owner deletion**; no open PR. |
| `work/fix-promoted-uml-index-links` | 3 / 20 | — | Its promotion script and tests match current `development`; only the generated index differs, and the current branded index has corrected package links. **Ready for owner deletion**; no open PR. |
| `work/fix-uml-promotion-generator-manifest` | 2 / 22 | — | Its unique `index.md` copy/test is superseded by the current manifest-owned four-project promotion, which rewrites links and validates safe destinations. **Ready for owner deletion**; no open PR. |
| `work/fix-uml-self-reference-clearance-2026-10-27` | 7 / 6 | [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55) | Its complete self-reference route/label/canvas fix and focused review-artifact workflow have since been integrated into `development` at `0f256ac`, combined with the focused regression test from #60. Follow-up commit `466167f` keeps the loop above the outgoing dependency edge; Package GUI passed and the artifact was visually reviewed. Old PR/branch remains open and must be dispositioned. |
| `work/four-package-uml-promotion` | 19 / 24 | — | The three changed files are older versions of the current promotion workflow/script/tests; current development contains four-project manifest safety, drift checks, branded pages, and race-safe retry. Its branch-only index copy is obsolete. **Ready for owner deletion**; no open PR. |
| `work/record-committed-uml-inventory` | 7 / 20 | — | `docs/SOURCE_DERIVED_UML.md` matches current development; its README/queue/readiness content is older than the current branded README, domain gate, and newer artifact evidence. **Ready for owner deletion**; no open PR. |
| `work/record-current-package-archive-evidence` | 2 / 16 | — | Its readiness/queue archive evidence is superseded by the current four-package artifact record and explicit selected-candidate gate. **Ready for owner deletion**; no open PR. |
| `work/record-svg-vocabulary-coverage` | 0 / 13 | — | **READY FOR OWNER DELETION** based on this snapshot: no unique commits relative to `development`, no open PR, and all branch commits are reachable from `development`. Owner should recheck the head before deleting. |
| `work/record-svg-vocabulary-coverage-status` | 2 / 13 | — | The current queue marks the 60-mapping vocabulary-to-test audit complete, and `docs/SVG_ADAPTER.md` contains the matrix. Its old readiness estimate is stale. **Ready for owner deletion**; no open PR. |
| `work/svg-vocabulary-test-coverage` | 2 / 14 | — | Current `docs/SVG_ADAPTER.md` and `SvgGuiRendererTests.cs` contain the newer, expanded mapping/test matrix and regression coverage; no branch-only test lines remain. **Ready for owner deletion**; no open PR. |
| `work/trim-release-evidence-history` | 2 / 15 | — | Current `RELEASE_READINESS.md` has the concise current gate/checklist; this branch's unique history and readiness estimate are older than the current record. **Ready for owner deletion**; no open PR. |
| `work/uml-neighborhood-self-reference-clearance-2026-10-28` | 4 / 4 | [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60) | Its focused boundary regression test is integrated into `development` at `0f256ac` alongside #55's broader layout fix. Follow-up commit `466167f` adds a no-crossing assertion; Package GUI passed and the new review artifact was visually inspected. Old PR/branch remains open and must be dispositioned. |
| `work/uml-visibility-tier-data-cards-2026-10-28` | 3 / 4 | [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57) | Preserve; clarify visibility tiers and colorful data cards against the canonical standard. |
| `work/uml-visual-data-cards-2026-10-27` | 19 / 6 | [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50) | Preserve; substantive nested data-card renderer/tests. Review alongside #57 and both self-reference fixes before integration. |
| `work/wpf-package-only-consumer` | 5 / 22 | — | Current `package.yml` explicitly packs Core/WPF and restores a clean Windows consumer only from the local `.nupkg` folder; its queue/readiness notes are older. **Ready for owner deletion**; no open PR. |

### Canonical branches

- `TrentBest/TheSingularityWorkshop.GUI:master` — **KEEP**. Released baseline; do not rewrite it during rescue.
- `TrentBest/TheSingularityWorkshop.GUI:development` — **KEEP**. Active work line and target for reviewed integration.

### READY FOR OWNER DELETION

The following exact branches have no open PR and no remaining unique content after comparison with current `development`. The listed ahead/behind counts are the original audit snapshot; the deletion decision is based on current file-content comparison and supersession evidence. Recheck the live head once more immediately before deletion.

- `TrentBest/TheSingularityWorkshop.GUI:work/automatic-uml-documentation-promotion`
- `TrentBest/TheSingularityWorkshop.GUI:work/fix-promoted-uml-index-links`
- `TrentBest/TheSingularityWorkshop.GUI:work/fix-uml-promotion-generator-manifest`
- `TrentBest/TheSingularityWorkshop.GUI:work/four-package-uml-promotion`
- `TrentBest/TheSingularityWorkshop.GUI:work/record-committed-uml-inventory`
- `TrentBest/TheSingularityWorkshop.GUI:work/record-current-package-archive-evidence`
- `TrentBest/TheSingularityWorkshop.GUI:work/record-svg-vocabulary-coverage`
- `TrentBest/TheSingularityWorkshop.GUI:work/record-svg-vocabulary-coverage-status`
- `TrentBest/TheSingularityWorkshop.GUI:work/svg-vocabulary-test-coverage`
- `TrentBest/TheSingularityWorkshop.GUI:work/trim-release-evidence-history`
- `TrentBest/TheSingularityWorkshop.GUI:work/wpf-package-only-consumer`

These are recommendations for owner-performed deletion only. No branch was deleted by this audit.

## 03 — Open pull requests

The audit snapshot had 17 open draft PRs; the audit itself adds one. Live inventory on 2026-10-29 is **18 open PRs, all drafts**. No PR has been merged or closed by this work.

- **Content already integrated; old PR needs owner disposition:** [#32](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/32), [#34](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/34), [#35](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/35), [#36](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/36), [#37](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/37), [#38](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/38), [#40](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/40), [#41](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/41), [#43](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/43), [#45](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/45), [#47](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/47), [#48](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/48), [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55), [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60). Their useful content has been integrated directly into `development`; their branches/PRs are not safe to delete/close until the owner disposes of the open PRs.
- **Open domain decisions/prototypes:** [#39](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/39) defines the UML view/data-card standard but conflicts with the unresolved static/member-order requirement; [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50) and [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57) overlap on data-card visibility tiers and must not be adopted as normative until the order is resolved.
- **This audit PR:** [#64](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/64) carries this ledger. Its content is being integrated into `development` separately; it remains open until the owner decides how to dispose of it.

These are not 18 independent pieces of work. Several overlap or represent earlier stages of a newer branch. Reconcile by behavior, tests, documentation value, and current source—not by PR age or branch name.

## 04 — Sith publication gate

The current `.github/workflows/package.yml` contains a `publish_nuget` job with the condition:

```yaml
if: ${{ github.event_name == 'workflow_dispatch' && inputs.publish == true && false }}
```

That explicit `&& false` keeps publication disabled. **Do not remove it as part of this audit.** A new `Enforce Sith branch inventory before publication` step has been added to `development` (commit [`e12d793`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/e12d793e035c72eb4b84e701c23f10857ff603ca)). It queries the remote branch inventory and requires exactly `master` and `development`; an API failure or any extra/missing branch fails the step closed. The normal Package GUI and UML workflows passed on that commit, but the inventory step itself was **not executed** because `publish_nuget` remains hard-disabled.

Because the job remains hard-disabled, the branch check does not run as part of ordinary build/test/package runs. Keep the hard-disabled condition in place. Before anyone ever re-enables publication, verify this inventory step in CI and also ensure the reviewed candidate is the intended source, policy evidence is current, and explicit owner authorization has been supplied. Build, test, pack, archive inspection, and consumer verification may continue independently. No NuGet publication is authorized here.

**Generated-documentation push race (2026-10-29):** the first promotion attempt was rejected because `development` advanced during the run. This was a non-fast-forward push race, not a content-validation failure. The safe rebase/retry path was added to `promote-source-uml-docs.yml` in [`f48dd15`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/f48dd15256521c88fad88371742c89d5fd1d326f); the retry passed in [run 40964081993](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/40964081993) and committed the refreshed generated pages at [`0ad4bde`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/0ad4bde2bd7eab8ab5ba726d4811f3c8c26d07e6). The promotion workflow now checks source drift before rebasing and pushing.

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
