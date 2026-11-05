<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# Operation Sith — GUI Branch Reconciliation

**Audit timestamp:** 2026-11-04 21:20 UTC  
**Repository:** `TrentBest/TheSingularityWorkshop.GUI`  
**Canonical branches:** `master`, `development` only  
**Development head:** [`c48d32c652cf00493da8c6a66625f8406961a7aa`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/c48d32c652cf00493da8c6a66625f8406961a7aa)  
**Master head:** [`36342df0654d6960f0ea36d3410d135366c09d1c`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/36342df0654d6960f0ea36d3410d135366c09d1c)

This is a point-in-time reconciliation ledger for [Operation Sith issue #62](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/62). It is intended to make branch work findable and support owner-directed cleanup. It does **not** authorize branch deletion, PR closure/merge, force-push, release, deployment, or package publication.

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
| 2026-11-04 06:57 | `work/ciai-evidence-lane` | 1 / 5 | [#74](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/74) | `.github/workflows/ciai-evidence.yml` — read-only CiAi exact-run evidence workflow |
| 2026-10-30 20:22 | `work/adapter-conformance-three-way-2026-10-30` | 7 / 59 | [#65](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/65) | `docs/TESTING.md`; shared adapter conformance tests; WPF shared conformance tests |
| 2026-10-29 22:21 | `work/sith-branch-reconciliation-2026-10-29` | 22 / 99 | [#64](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/64) | `docs/BRANCH_RECONCILIATION.md` — prior rescue ledger; **this current ledger supersedes its stale snapshot** |
| 2026-10-28 20:28 | `work/uml-neighborhood-self-reference-clearance-2026-10-28` | 4 / 103 | [#60](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/60) | UML graph renderer and regression tests — self-reference routing |
| 2026-10-28 20:01 | `work/uml-visibility-tier-data-cards-2026-10-28` | 3 / 103 | [#57](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/57) | `docs/DATA_CARDS_AND_UML.md` — visibility-tier/data-card proposal |
| 2026-10-28 05:12 | `work/fix-uml-self-reference-clearance-2026-10-27` | 7 / 105 | [#55](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/55) | Package workflow, UML graph renderer, regression tests — self-reference clearance |
| 2026-10-28 20:00 | `work/uml-visual-data-cards-2026-10-27` | 19 / 105 | [#50](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/50) | `src/SVG/SvgUmlClassDiagramRenderer.cs`; `tests/SVG.Tests/SvgUmlClassDiagramRendererTests.cs` — nested UML cards |
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

**Timestamp note:** these are commit timestamps on branch heads, not PR update timestamps or a claim that every file in a branch was last edited at that time. Rows are sorted by the timestamp; the two October 28 entries are ordered by exact timestamp where relevant.

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
- **CI status is not yet audited for all 20 exact branch-head SHAs.** Do not infer green from historical PR descriptions or from development CI. The next pass must query exact-head checks and inspect any unique code diffs before marking a PR/branch as resolved.

## 4. PR disposition queue

Every row below is an open draft PR, not an approved merge. Review in this order to reduce overlap and avoid losing newer work:

1. **Newest CI evidence lane:** [#74](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/74). Review its workflow and exact-head checks independently; it is recent and not part of the older rescue audit.
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

1. Audit exact-head CI for all 20 PR branches and record the result beside each PR.
2. Review the actual unique diffs in priority order; classify each as **already integrated**, **needs integration**, **preserve as open decision**, or **superseded/rejected**.
3. Integrate only reviewed, useful changes into `development`, with tests and consumer evidence where behavior changes.
4. Update this ledger after each reconciliation pass. Keep its table sorted by branch-head commit time, newest first.
5. Only after PR disposition and fresh comparison should any branch be labeled **READY FOR OWNER DELETION**.
6. Recheck the remote inventory after the owner performs cleanup; the target is exactly `master` and `development`.

**Current decision:** branch cleanup is not yet ready for owner deletion. The first actionable cleanup pass is PR disposition plus exact-head CI/diff review—not further feature implementation.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
