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


## Documentation recovery and PR-scope correction — 2026-10-21 17:53 UTC

### Recovered missing keeper-branch documents

The two guide paths previously reported missing on `development` have now been created as fresh, implementation-aware documents (not copied verbatim from stale branch snapshots):

- `docs/SVG_GUIDE.md` commit `79f0875ef11c6ec3e2a074532a238078181cfe35`. It points to the current `docs/SVG_ADAPTER.md` contract, records the source's depth/accessibility behavior, and explicitly distinguishes generated output, source-derived diagrams, concepts, and runtime captures. It makes no unsupported claim that a workflow or package release passed.
- `docs/DATUM_AND_INTERACTION_BOUNDARY.md` commit `a448f1bdc8cd248dce01458f3f0bd1cc20f58e2b`. It records datum ownership and the presentation/adapter/renderer boundary while labeling the input/output package and protocol integrations as proposals, not implemented features.

The architecture document on `development` already links to the datum-boundary path; that link now resolves. The SVG guide should be checked against the repository's documentation index and README during the final docs-link pass.

### Correction: PR #11 is not documentation-only in its current patch

PR #11's body describes a documentation-only scope, but the current live PR patch contains 59 changed files, including the package workflow, solution/project inventory, source code, tests, SVG assets, and source-derived UML generator. This mismatch is material. Do not accept its summary as a reliable scope statement and do not merge it wholesale. The useful guide content can be reviewed separately against the new keeper-branch `docs/SVG_GUIDE.md` and the current renderer source.

### Current branch disposition updates

- `work/webpage-event-builder-compatibility`: likely discard after a full changed-file inventory confirms there are no other unique files. Its event test is identical on development and its old source surface is superseded by the current documented implementation.
- `work/svg-tree-depth-guard`: likely discard after any useful unique regression assertions are transferred to development and verified. Do not restore the old renderer; development has the newer accessibility linkage.
- `work/gui-wpf-tests`: redundant-tip candidate (zero ahead, no changed files reported); final live comparison is still required.
- `fix/guinode-nullability-contract`: likely superseded by the current iterative traversal; final source and full branch-diff checks remain required.
- `work/svg-app-fsm-cos-boundary`: likely redundant for the concept document but not cleared until the complete PR and remaining branch file list are checked.
- `work/gui-uml-per-file-documentation`: its only reported changed file adds 19 lines to `docs/DATA_CARDS_AND_UML.md`. Compare those lines to the more extensive source-derived UML documentation on PR #18; do not assume identical purpose or freshness.
- `work/gui-release-mit-wpf-scope` and `work/gui-verify-xml-docs-in-packages`: retain as evidence sources until their unique release-checklist/workflow intent is reconciled with current `docs/RELEASE_READINESS.md` and `docs/NEXT_RELEASE_WORK_QUEUE.md`. Neither branch authorizes enabling publish.
- `feature/integrated-fsm-cos-hub` and `feature/nested-experiences`: both are highly divergent, with unique implementation, tests, docs, and workflow changes. Their old hub/nested-experience APIs should not be restored wholesale. Decide whether to explicitly abandon those designs or selectively rebuild any required capability against the current architecture.
- `architecture/datum-interaction-boundary`: the architectural principle has now been captured in a new keeper-branch document. PR #8 remains broad (50 files) and draft; compare any remaining unique principle or test before closing/discarding the branch.
- `work/svg-guide-current-contract`: PR #11's patch is much broader than its title/body claim. The keeper branch now has a fresh current-source guide, but unique non-guide changes in the branch/PR still need inventory before it can be discarded.
- `work/nuget-publish-gate` and `work/gui-publish-gate-reconciled`: both propose removing the deliberate publish hard stop. Do not merge either and do not enable publishing absent explicit authorization.
- `work/gui-svg-source-uml-generator`: PR #18 contains substantial unique infrastructure: Roslyn source analysis, renderer-neutral UML models, SVG diagram renderers, per-type docs/pages, verification scripts, and CI changes. It is a staging branch (draft, non-mergeable) and cannot be disposed of as merely decorative color changes. Audit it as a separate substantial workstream, validate generated documentation against current source revisions, and decide whether to rebuild selected slices on development or explicitly defer the feature.

**Decision remains NOT CLEARED.** Creating the two missing documents repairs a real documentation gap; it does not resolve every branch, PR, CI, or generated-artifact discrepancy.


## PR #18 current-head CI evidence — 2026-10-21 18:03 UTC

The live workflow runs attached to PR #18 head `b4c68259a2df7c6f565708a529b01e62c433dae0` were inspected by job and log, not inferred from the summary status:

- **Solution-derived UML inventory** run [39642809163](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39642809163): succeeded.
- **Source-derived diagrams** run [39642809045](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39642809045): failed. Generation, syntax-only model, project-semantic model, reflection mismatch rejection, UML model validation, and the partial-generic semantic fixture all completed successfully. The failure occurred in the workflow's SVG contract grep: it requires property stroke `#f2e83d`, but the generator currently emits `#d6ca00`. Later validation and upload steps were skipped. This is a concrete contract drift between workflow expectation and renderer/test palette, not evidence that the full diagram pipeline passed.
- **Package GUI** run [39642809017](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39642809017): `test-wpf` succeeded, but `test-and-package` failed in SVG tests. The failing test `Uml_card_palette_uses_namespace_blue_class_orange_and_property_yellow_borders` expected `#f2e83d` and received `#d6ca00`; 45 SVG tests passed and one failed. Subsequent conformance, build, gallery, package, and artifact steps were skipped. `publish_nuget` was skipped.
- These failures point to the same semantic palette disagreement: the expected yellow must be decided once, then the renderer, tests, generated asset, and workflow assertions must agree. Do not weaken/remove the assertion just to get a green build; choose and document the intended palette and regenerate artifacts.
- The branch is therefore **not merge-ready**. It is useful work with reproducible current-head failures, and should remain under review until the palette contract and full CI pass.

This is stronger evidence than the previous qualitative “staging vertical slice” status. It does not change the global Sith decision: no branches were deleted and the open PR remains open.


## Focused branch-diff findings — 2026-10-21 18:10 UTC

### `work/svg-tree-depth-guard`: no unique renderer tests remain

The current branch and `development` versions of `tests/SVG.Tests/SvgGuiRendererTests.cs` were inspected by test method name. Every test in the branch also exists on `development`. Development has additional tests for embedded stylesheet serialization, XML attribute escaping/non-ASCII text, deterministic output, malformed/non-finite geometry, blank required references, and duplicate IDs for enabled coordinate overlays. Thus the branch's previously noted +25 test lines are not unique coverage missing from development; its test file is an older subset. The branch renderer also lacks the current title/description ID linkage. This branch is now a **strong discard candidate**, pending a final tree/PR check and ensuring no other unique file exists beyond the already identified adapter-doc/renderer changes.

### `work/webpage-event-builder-compatibility`: source/test contribution is already represented

The branch's added `WorkshopGuiBuilderEventTests.cs` blob is exactly the same as development's, and the event builder methods `OnWheel`, `StopPropagation`, and `PreventDefault` are present in both. The branch's reported diff consists of the Blazor builder file and the 33-line event test; no distinct test behavior has been found. The development version additionally documents the event surface. This is a **strong discard candidate**, pending checking the merged PR #9 record and exact tip tree one last time.

### `work/gui-wpf-tests`: redundant tip confirmed by current comparison

The fresh live compare reports zero commits ahead, 238 behind, and zero changed files against development. Its current tree is therefore identical at the compared tip. This is a **strong discard candidate**; still retain the keeper branches `master` and `development` only, and do not delete anything on the user's behalf.

### `work/gui-uml-per-file-documentation`: its apparent extra lines are not unique

The current `docs/DATA_CARDS_AND_UML.md` content fetched from this branch and development has the same substantive text in the displayed opening section. The branch's 19-line delta must not be counted as valuable unique documentation without a complete blob comparison; the content is now present on development and PR #18 contains a larger implementation proposal. Keep it out of the merge path unless an exact remaining unique paragraph is identified.

### `work/svg-app-fsm-cos-boundary`: concept doc is already merged

PR #12 is closed and describes a documentation-only proposal for `SVG_App`; the concept document is present on development. Its earlier changed-file comparison found the same `docs/SVG_APP_CONCEPT.md` blob on development. Treat this branch as a **strong discard candidate** after a final current-tip comparison confirms no other unique tree content.

### PR #11 description versus actual patch

PR #11 remains open and says its scope is documentation-only (`docs/SVG_GUIDE.md`), yet GitHub reports 59 changed files. The new keeper-branch guide is an independently authored, current-source replacement, but that alone does not disposition the other 58 files in the PR. Continue to treat the PR as broad, unmergeable as-is, and require file-level inventory before closure or branch deletion.

### Publish gate remains an explicit owner decision

The two one-line publish PRs (#15 and #19) both remove the deliberate `&& false` guard. They are duplicates in intent, not candidates for automatic merge. Publishing remains disabled on development; no release authorization is implied by CI or readiness.

**Sith remains NOT CLEARED.** The three strong discard candidates above are not yet a user-facing deletion list until their full tree and associated PR states are reconciled. The user remains the actor who deletes non-keeper branches.


## Final candidate recheck — 2026-10-21 18:10 UTC

Live comparisons were rerun after the previous audit commit, using the then-current `development` tip `e0769dc9b44984263b42ad0b4857f7953b669b37` as the compare base:

- `work/gui-wpf-tests`: 0 ahead, 239 behind, zero changed files. This is the clearest redundant-tip candidate.
- `work/webpage-event-builder-compatibility`: 3 ahead, 486 behind, only `src/Blazor/WorkshopGuiBuilder.cs` and `tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs` reported. PR #9 is confirmed merged (merge commit `40f759123aa906f63f94351a025f3ada9d313f65`). The test blob is identical to development and the event methods exist on both; the remaining branch source differences are not a reason to preserve the branch without identifying a unique behavior.
- `work/svg-app-fsm-cos-boundary`: 5 ahead, 423 behind; only README and `docs/SVG_APP_CONCEPT.md` appear in the compare. PR #12 is confirmed merged (merge commit `73f24c106bd32f2f944f5a9425211b7065396145`). Concept content is already on development.
- `work/svg-tree-depth-guard`: 5 ahead, 423 behind; its only reported files are `docs/SVG_ADAPTER.md`, `src/SVG/SvgGuiRenderer.cs`, and `tests/SVG.Tests/SvgGuiRendererTests.cs`. Test-method inventory confirms every branch test exists on development; development has additional tests and newer title/description accessibility linkage. No unique test needs porting from this branch.
- `fix/guinode-nullability-contract`: 1 ahead, 423 behind, only `src/Core/GuiNode.cs` reported. The iterative traversal on development supersedes the branch's older implementation; keep this as a final source-level confirmation item, not a merge candidate.
- PR #17 is confirmed merged (merge commit `4bd42c78b95ba9f3834eb3b137aa710cd8f1dc3f`), but that does not by itself prove its branch's remaining release docs/workflow are identical to development.

These comparisons strengthen the discard cases, but this document is still not an all-clear: the remaining divergent branches and all open PRs require disposition, including PR #18's failed checks and the unresolved deliberate publish hard stop. No branch deletion is performed by the assistant.


## Focused branch reconciliation pass — 2026-10-21 18:25 UTC

Live branch-tip comparisons were repeated against `development`. These findings supersede older statements in this audit where the exact changed-file comparison differs.

| Branch | Compare result | Finding | Disposition |
|---|---:|---|---|
| `work/gui-release-mit-wpf-scope` | 16 ahead / 247 behind; 8 changed files | The MIT `LICENSE` is byte-identical to development, as is the WPF README. The branch still carries distinct package workflow and project-file edits plus release-readiness/work-queue wording. Do not discard until those remaining edits are reviewed against the current workflow and release docs. | Hold for targeted diff review; no wholesale merge. |
| `work/gui-verify-xml-docs-in-packages` | 4 ahead / 246 behind; workflow and two release docs | Its workflow does not enable publishing: the development workflow has the same publish hard stop. The branch's remaining value is limited to workflow/doc deltas; PR #17 being merged does not prove this branch tip is wholly redundant. | Review the exact workflow/doc hunks, then discard if superseded. Never use it to authorize publication. |
| `feature/integrated-fsm-cos-hub` | 76 ahead / 499 behind; 40+ changed paths | Broad legacy hub/API and renderer implementation, tests, WPF controls, workflow, and docs remain different. This is not a small patch to transplant. Current architecture puts runtime lifecycle in the consuming host and keeps hub/runtime proposals distinct from the GUI semantic tree. | Owner-level design decision: intentionally discard legacy implementation or define a small capability to rebuild against current contracts. No wholesale merge. |
| `feature/nested-experiences` | 38 ahead / 499 behind; 16 changed paths | Carries the legacy recursive experience/hub contract, related Core/Blazor API changes, tests, workflow and docs. The concept is not equivalent to an implemented current GUI runtime contract; current documentation treats such lifecycle as host-owned/proposal-level. | Explicitly decide discard vs rebuild a host-owned experience contract. No wholesale merge. |
| `work/gui-uml-per-file-documentation` | 1 ahead / 179 behind; only `docs/DATA_CARDS_AND_UML.md` reported changed | The opening substantive content fetched from branch and development is identical. No unique implementation or asset was reported by the compare. Verify full blob equality or the remaining 19-line delta before calling it redundant. | Near-redundant candidate; one final full-content check required. |
| `work/gui-wpf-tests` | Previously 0 ahead / 0 changed files | Strong redundant-tip candidate, but branch inventory and compare must be rechecked immediately before deletion. | Candidate only; not a global all-clear. |
| `fix/guinode-nullability-contract` | Prior exact source review found only an obsolete `GuiNode.cs` variant | Development's iterative traversal and documentation supersede the branch implementation. | Intentionally discardable after checking historical PR disposition and current inventory. |
| `work/webpage-event-builder-compatibility` | Prior compare found the event behavior/test represented on development; PR #9 merged | The branch is behind development and does not currently justify preserving its old tip. | Intentionally discardable after current-tip recheck. |
| `work/svg-app-fsm-cos-boundary` | Prior compare found only README and `docs/SVG_APP_CONCEPT.md`; PR #12 merged | Contribution is reported represented in development. | Intentionally discardable after current-tip recheck. |
| `work/svg-tree-depth-guard` | Prior exact test inventory found every branch test method on development; development renderer includes stronger accessibility and validation behavior | No missing test method identified; the old renderer is superseded. Remaining doc diff should be checked for any unique factual content. | Likely intentionally discardable after documentation diff check. |

### Current package-workflow comparison

The fetched `.github/workflows/package.yml` content on `work/gui-release-mit-wpf-scope` and `development` is identical for the reviewed opening section; both retain the deliberate `&& false` publish hard stop. Therefore this branch's workflow delta reported by GitHub is elsewhere in the file or in workflow structure not shown by the opening excerpt, and must be inspected as a full diff before a disposition. Do not infer that the workflow is identical from this partial comparison.

### UML guide overlap check

The first substantial section of `docs/DATA_CARDS_AND_UML.md` on `work/gui-uml-per-file-documentation` and `development` is identical. The compare reports a 19-line delta, so the exact remainder still requires comparison; the branch is not yet certified redundant.

### Decision after this pass

The keeper target remains exactly `master` and `development`. This pass materially narrows several branch dispositions, but it does **not** clear Sith cleanup: the two legacy experience/hub branches need an explicit keep/rebuild/discard decision; PRs #8, #11, #15, #18 and #19 remain to be dispositioned; and several remaining unique docs/workflow hunks still need exact review. No branches or PRs were deleted, closed, merged, or published in this pass.


## Exact content comparison correction — 2026-10-21 18:26 UTC

A full-content comparison resolves two previously open questions:

- **`work/gui-uml-per-file-documentation` is superseded, not a unique keeper.** The branch's `docs/DATA_CARDS_AND_UML.md` is 130 lines; development's is 240 lines. The common opening section is identical, but the branch's later per-file UML section is replaced on development by a substantially expanded SVG/living-documentation and incremental implementation contract. The branch does not contain unique code/assets according to the compare. Classify the branch as intentionally discardable after current inventory recheck.
- **`work/gui-release-mit-wpf-scope` is an older partial workflow/doc snapshot, not a candidate to merge wholesale.** Its workflow is 247 lines versus development's 396 lines and diverges around package/artifact verification; the current development workflow has additional package checks and gallery verification. Its queue/readiness documents are also older snapshots: the branch queue says 74% while development's current readiness gate says 70%, reflecting later work and the hard per-type UML gate. The root MIT license is byte-identical to development, and the WPF README is also unchanged in substance. Do not bring back the older readiness percentage or replace the newer workflow/docs. Any remaining useful project-file delta must be checked independently before branch discard.

These findings narrow the disposition but do not change the global decision: **NOT CLEARED** until open PRs are explicitly resolved and all remaining branch-specific material is either reconciled or deliberately discarded.


### Release-scope branch project-file verification — 2026-10-21 18:27 UTC

Fetched the Core, Blazor, and WPF project files from `work/gui-release-mit-wpf-scope` and `development`; all three are byte-identical, including package IDs, target frameworks, MIT expressions, versions, README metadata, and dependencies. The branch solution file is also older: it omits `samples/DroneConstructionVision/DroneConstructionVision.csproj`, which is present on development. The root license and WPF README are identical as already noted. Together with the older workflow and stale 74% queue estimate, this leaves no identified unique project/license content that needs porting from `work/gui-release-mit-wpf-scope`. **Disposition: intentionally discardable after a final branch/PR inventory check**; do not merge the branch.


## Publish-gate PR verification — 2026-10-21 18:31 UTC

Fresh PR inventory still shows five open PRs: #8, #11, #15, #18, and #19. Their disposition remains open; no PR was merged or closed during this pass.

- **PR #19** (`work/gui-publish-gate-reconciled`, exact head `76c4393eed0320249314612d8c20fda2a5992f33`) has a successful Package GUI run [#39512573959](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39512573959). Job-level results: `test-and-package` succeeded, `test-wpf` succeeded, and `publish_nuget` was skipped. This satisfies the PR's stated CI verification without dispatching a publish workflow.
- **PR #15** (`work/nuget-publish-gate`, exact head `b3a9695ed4e18b9727ef016eea75233efedd0a91`) has only a cancelled historical run [#39196092517](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39196092517); that is not equivalent to passing its exact head. It is the older duplicate of the same one-line intent and should not be merged in addition to PR #19.
- Both branches remove the hard-coded `&& false` from the publication condition, but only PR #19 is based on the current development workflow and has a successful exact-head package/WPF run. The resulting condition still requires `workflow_dispatch` and `inputs.publish == true`; ordinary push/PR events cannot publish. This corrects the dead switch but **does not authorize a release**. Actual publication still requires the owner's explicit release authorization and a deliberate manual dispatch.
- PR #19 is now technically verified for its stated CI gate. It has not been merged by this audit. Keep its branch until the maintainer chooses whether to merge this narrow correction; then discard the duplicate PR #15 branch only after its PR is explicitly closed and the final inventory is checked.


## Open PR patch-scope review — 2026-10-21 18:33 UTC

Reviewed the actual patch content for PRs #8, #11, and #18 (not just their titles/descriptions).

### PR #8 — Datum/interaction boundary (50-file draft)

The draft is not a docs-only architecture note. Its package workflow redesign adds separate Core/Blazor/WPF build and package lanes, coverage upload that fails the job if Codecov fails, and multiple NuGet.org publish jobs. Those publish jobs retain `&& false`, so they remain disabled in that snapshot. It also adds a large `NuGetReadme.md`, broad README/package metadata changes, and multiple code/test/diagram changes. The architecture principle has now been recorded in the development-branch boundary document, but that does not imply the rest of this patch is represented or safe. **Disposition: do not merge wholesale.** Continue a file-by-file inventory of any genuinely unique tests/behavior, and treat workflow/package modernization as a separate reviewed change. Verify all action references and the current package matrix before any workflow hunk is adopted.

### PR #11 — SVG guide current contract (59-file draft)

The patch is also not documentation-only. In addition to its guide and README content, it reworks the package workflow, introduces separate SVG packaging/coverage and WPF lanes, modifies project/repository inventory and verification scripts, and changes source/tests/assets. The SVG guide has since been independently recreated on development against the current adapter contract. Therefore PR #11's description understates its scope, and neither its workflow nor its implementation should be merged as a unit. **Disposition: do not merge wholesale.** Compare each non-guide file to current development, identify any still-missing tests or behavior, and port only validated deltas.

### PR #18 — source-derived UML

The draft contains distinct source analysis, project-semantic/reflection verification, per-type Markdown/SVG generation, solution-level inventory, and CI workflows. These are substantive and are not represented merely by the manually created `docs/UML` convention. However, the exact-head CI failure documented above is a real palette contract mismatch, so the PR remains not merge-ready. **Disposition: retain as the active workstream; fix the yellow-border contract consistently across renderer, tests, workflow assertions, and generated examples, then rerun the exact-head workflows.** Do not close/discard this branch until the implementation is either validated and deliberately integrated or the same capabilities are rebuilt on development.

### PRs #15 and #19 — publish gate

PR #19's exact-head package and WPF tests passed; the publish job was skipped. The patch's purpose is to remove the hard-coded false condition, restoring the intended manual-dispatch plus explicit `publish: true` gate. This is a CI-mechanism correction, not release approval. Since the repository owner has not asked for a package release, no publish workflow was dispatched. The older duplicate PR #15 should be closed as superseded once the owner accepts PR #19; it must not be merged alongside #19.

### Required work before Sith clearance

1. Finish non-guide changed-file comparisons for PRs #8 and #11, then explicitly choose selective port or deliberate discard for every unique behavior/test.
2. Resolve PR #18's palette mismatch and re-run its exact-head workflows, or deliberately rebuild its unique source-UML capabilities on development before discarding its branch.
3. Obtain a maintainer decision on PR #19's narrow publish-gate correction; close duplicate #15 only after that decision.
4. Make an explicit discard-or-rebuild decision for the legacy `feature/integrated-fsm-cos-hub` and `feature/nested-experiences` designs.
5. Recheck the live branch and open-PR inventories and the final development diff immediately before the all-clear.

No branch deletion or PR state change was performed during this review.


## PR #18 palette contract repair attempt — 2026-10-21 18:38 UTC

On `work/gui-svg-source-uml-generator`, I aligned the two SVG renderers with the existing property-yellow contract already asserted by the SVG unit test and workflow: `#f2e83d`. The prior implementation emitted `#d6ca00` in both the DataCard renderer and UML class-diagram property divider, while the test and workflow expected `#f2e83d`. This was a genuine contract inconsistency, not a test to weaken.

- `src/SVG/SvgDataCardRenderer.cs`: property-card border changed from `#d6ca00` to `#f2e83d`.
- `src/SVG/SvgUmlClassDiagramRenderer.cs`: property-compartment divider changed from `#d6ca00` to `#f2e83d`.
- The existing SVG unit assertion and source-derived workflow assertion already specify `#f2e83d`; no assertion was weakened.
- Resulting PR #18 head: `97554095656f2ef0ca94cbc4ada73848f3a953f8`.

The commit is now on the PR branch, but the GitHub workflow-run lookup for this exact head returned no runs at the time of checking. Therefore this is **a source-level repair attempt, not a verified fix**. Recheck CI after GitHub registers the new commit; do not claim PR #18 passes until exact-head package, source-derived diagram, and solution-derived inventory workflows all finish successfully. PR #18 remains draft and unmerged.


## Final focused recheck — 2026-10-21 18:41 UTC

### SVG depth-guard branch documentation resolved

Compared `docs/SVG_ADAPTER.md` from `work/svg-tree-depth-guard` with current `development`. The branch copy is an older contract document; development adds the external-reference/URL policy and a more explicit trust/security boundary, plus capability evidence for the fox/sheepdog scene (what is generated, what is declarative animation, and what is *not* a live FSM/physics/input/sound integration), reproducible verification commands, and a gallery-to-test map. These are not missing branch-only contributions; the current development copy is the more complete version. The branch's source and test changes also appear superseded by development's iterative depth validation and expanded tests. **Disposition recommendation: discard the branch after the final inventory check.**

### Other current compare snapshots

- `work/gui-wpf-tests`: exact compare reports 0 commits ahead and 0 changed files; current development already contains its content. Strong discard candidate.
- `work/gui-uml-per-file-documentation`: 1 commit ahead / 185 behind; its sole changed file is `docs/DATA_CARDS_AND_UML.md`, but the development copy is the expanded version. Strong discard candidate.
- `work/gui-release-mit-wpf-scope`: 16 commits ahead / 253 behind and 8 changed paths in the compare summary. Previous byte-level project checks established Core/Blazor/WPF project files, LICENSE, and WPF README as identical; remaining differences are workflow and release tracking files that are older than current development. Strong discard candidate, no wholesale merge.
- `fix/guinode-nullability-contract`: one ahead / 436 behind, with only the obsolete `src/Core/GuiNode.cs` change in the compare file list. Current development's traversal/docs implementation supersedes it. Strong discard candidate.
- `work/svg-tree-depth-guard`: 5 ahead / 436 behind with `docs/SVG_ADAPTER.md`, `src/SVG/SvgGuiRenderer.cs`, and `tests/SVG.Tests/SvgGuiRendererTests.cs` listed as changed relative to its merge base. Documentation and test behavior were compared against current development; current development has the stronger contract and broader tests. Final code-diff review remains part of the deletion checklist.

### PR #18 status clarification

The latest source repair commit is `97554095656f2ef0ca94cbc4ada73848f3a953f8`. The workflow-run listing still contains runs whose actual `head_sha` is the earlier `b4c68259a2df7c6f565708a529b01e62c433dae0`, even though those runs' PR metadata now references the newer head. The newer SHA's check-runs endpoint reports zero check runs. Therefore the prior failures/successes are not evidence for the repaired commit. Do not mark the palette repair verified; trigger/await workflows for the exact SHA and inspect each run's own `head_sha`.

No branches or PRs were changed by this recheck. Sith remains uncleared.


## Exact-head workflow recheck — 2026-10-21 18:52 UTC

The GitHub Actions collection for `work/gui-svg-source-uml-generator` still shows the three relevant most recent runs on `b4c68259a2df7c6f565708a529b01e62c433dae0`, not on the current PR head `97554095656f2ef0ca94cbc4ada73848f3a953f8`:

- [Package GUI #39642809017](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39642809017) — failed on the old SHA.
- [Source-derived diagrams #39642809045](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39642809045) — failed on the old SHA.
- [Solution-derived UML inventory #39642809163](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39642809163) — passed on the old SHA.

Although the run records list PR #18 metadata whose head is the newer SHA, each run's own `head_sha` is the old commit. The current SHA's check-runs endpoint returns no checks. Therefore **there is still no exact-head evidence** that the palette correction passes. The GitHub connection available for this audit supports GET-only workflow inspection; it cannot dispatch or rerun workflows. The next valid proof must come from GitHub Actions triggered for the new head (or a maintainer rerun/dispatch), then checked by each run's own `head_sha`.

Fresh compare snapshots also reaffirm:
- `work/svg-tree-depth-guard` is 5 ahead / 437 behind, with only `docs/SVG_ADAPTER.md`, `src/SVG/SvgGuiRenderer.cs`, and `tests/SVG.Tests/SvgGuiRendererTests.cs` in the compare file list. Documentation/tests are superseded; keep it on the discard-candidate list pending final source hunk review.
- `work/gui-verify-xml-docs-in-packages` is 4 ahead / 253 behind and has three changed paths: package workflow plus release work queue/readiness docs. Its workflow is a stale snapshot; the docs are historical snapshots. Final discard is contingent on checking that its XML-doc verification behavior exists on development, not merely that PR #17 merged.
- `work/svg-guide-current-contract` is still a very broad 112-ahead / 506-behind compare, with 59 paths previously inventoried. PR #11's current description claims docs-only, but the current branch comparison includes workflow, source, tests, package projects, WPF implementation, and architecture docs. Do not merge it as described. Its guide content is preserved separately on development; any remaining unique non-guide content must be classified before the branch can be discarded.
- PR #18 remains open/draft; PR #8 and #11 remain open; PRs #15 and #19 remain open duplicates for the publish gate. No PR or branch state was changed.

No deletion authorization is implied by these compare results.

## Live CI and final-recheck update — 2026-10-21 19:06 UTC

Current `development` tip at audit time: `3e03d572101d2be627e97aa9407e64c3e7ec9627`.

### Exact-head package workflow result

- [Package GUI run 39656628688](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39656628688) ran on the exact current development SHA above and **failed**.
- `test-and-package` passed.
- `publish_nuget` was skipped as intended; `.github/workflows/package.yml` still contains the deliberate `&& false` hard stop.
- `test-wpf` failed because the test host crashed during WPF shutdown. The log says all 23 WPF tests passed, then the test run was aborted by an unhandled `System.NullReferenceException` in `System.Threading.Thread.get_CurrentThread()` called from `MS.Win32.HwndSubclass.SubclassWndProc`. The job exited 1. This is **not** a green WPF verification: investigate STA thread/dispatcher/window teardown and obtain a clean exact-head run before claiming CI is green.
- The current run does not authorize publication. Do not rerun a publish-enabled dispatch.

### Branch-tip recheck

- Live branch inventory remains 17 branches: `master`, `development`, and 15 non-keepers.
- `work/gui-wpf-tests` is exactly the current development commit `3e03d572101d2be627e97aa9407e64c3e7ec9627` as its branch head, with zero commits ahead and no changed files in the comparison. It is the strongest verified-redundant candidate, but this audit remains not-cleared until all remaining branch/PR dispositions are complete.
- `fix/guinode-nullability-contract` is 1 commit ahead / 438 behind and differs only in `src/Core/GuiNode.cs`; current development supersedes that one-line change with iterative traversal and XML documentation. It is a strong intentionally-discard candidate, subject to the final open-PR/state check before owner deletion.
- Open PRs #8, #11, #15, #18, and #19 remain unresolved at this check. PR #15 and #19 still represent competing publish-gate proposals; #18's current source-derived UML head still lacks exact-head workflow evidence as documented above.
- No branch was deleted and no PR was merged or closed in this pass.

**Current decision: NOT CLEARED.** The newly observed WPF test-host crash is an additional blocker for describing the current development candidate as fully verified. Branch cleanup and package publication remain separate decisions.

## Focused branch-source verification — 2026-10-21 19:08 UTC

### `work/svg-tree-depth-guard`

Re-fetched `src/SVG/SvgGuiRenderer.cs`, `tests/SVG.Tests/SvgGuiRendererTests.cs`, and `docs/SVG_ADAPTER.md` from both this branch and `development`.

- Current development renderer has `MaximumTreeDepth = 256`, an iterative stack-based depth-validation pass before recursive SVG serialization, and a clear exception when a tree exceeds the limit.
- Current development tests explicitly cover accepting a tree at the configured maximum and rejecting excessive depth before recursive serialization. The branch test file is an older, shorter snapshot and does not contain those two depth-specific tests.
- Current development adapter documentation states the depth-limit contract and explains why validation happens before recursive serialization; the branch documentation is an older version.
- Compare snapshot: 5 commits ahead / 438 behind, with only the adapter guide, SVG renderer, and SVG renderer tests listed as changed files.

**Disposition recommendation: discard candidate after final inventory check.** Its implementation, focused regression tests, and documented contract are already present in the more current development version. No branch deletion performed.

### `work/gui-verify-xml-docs-in-packages`

Compared `.github/workflows/package.yml` and the release work queue on the branch against current development.

- Current development already contains the step `Verify all package license, README, target framework, and XML documentation metadata`. It checks Core, Blazor, WPF, and SVG for the expected framework directory, a DLL, and a matching XML documentation file, alongside README and MIT license metadata.
- The branch workflow is an older, substantially shorter snapshot. Its XML documentation check is not missing from development.
- The branch's release queue/readiness files are historical snapshots. Current development has newer baseline evidence and explicit per-type documentation/UML requirements; do not restore those branch versions wholesale.
- Compare snapshot: 4 commits ahead / 254 behind; three changed paths (workflow, next-release work queue, release readiness).

**Disposition recommendation: discard candidate after final inventory check.** No unique XML-documentation gate needs to be cherry-picked based on this comparison. No branch deletion performed.

These are branch-disposition recommendations, not deletion authorization. Recheck the live branch/PR inventory and any newer commits before the owner deletes candidates.

## Broad-PR tree inventory correction — 2026-10-21 19:10 UTC

A recursive Git tree comparison was run against the live `development` tree (153 files at this snapshot), `architecture/datum-interaction-boundary` (PR #8 head `43449f49607798d23c951ffd50650da268456540`), and `work/svg-guide-current-contract` (PR #11 head `01ed8175bf2971f98e7fb324885c9e79103d0fac`). This checks actual path/blob presence, not just merge-base change lists.

**Correction to any earlier implication that PR #8/#11 are safe discard candidates:** they are not cleared for deletion yet. The recursive trees show paths present on the PR branches but absent from current development, including source and test files. These files may be obsolete, replaced by different APIs, or still contain behavior/design material worth preserving; path absence alone does not decide which.

Examples on both branches but absent by path from development include:
- Core: `src/Core/Gui.cs`, `src/Core/GuiHub.cs`, `src/Core/GuiKinds.cs`, `src/Core/IGuiHub.cs`, and `src/Core/ReflectionGuiBuilder.cs`.
- WPF: `src/WPF/Abstractions/GuiBuilderWPF.cs`, `GuiContextWPF.cs`, the `Controls/*` builders for enum/numeric/pivot-grid/tree-view, WPF dialogs/factory/layout/diagnostics builders, and `src/WPF/WpfGuiHubRenderer.cs`.
- Tests: `tests/Core.Tests/GuiHubTests.cs`, `ReflectionGuiBuilderTests.cs`, and `tests/Blazor.Tests/BlazorGuiHubRendererTests.cs`.
- PR #11 additionally has branch-only `src/SVG/SvgRenderOptions.cs`, `docs/examples/living-energy-vortex.svg`, `docs/images/gui-svg-manifestation.svg`, and `docs/LIVING_DOCUMENTATION.md`.

Many files that exist on both sides have different blob hashes, including Core/Blazor/WPF renderers/builders, SVG renderer/tests, workflow, project files, and architecture documentation. The branch versions are older/smaller in several sampled cases, but that is not proof every API or behavior has a current equivalent.

**Required next step before disposal:** perform a per-path semantic disposition for the branch-only files and material behavior in changed files: (1) equivalent current type/API and tests, (2) deliberately retired behavior with rationale, (3) unique useful behavior to port, or (4) obsolete/unwanted work safe to discard. In particular, review the branch-only reflection builder, hub API, WPF control builders, and SVG render options/visual assets. Do not merge either broad PR wholesale; do not delete either branch until this inventory is closed.

The existing branch/source findings for `work/svg-tree-depth-guard` and `work/gui-verify-xml-docs-in-packages` remain as recorded above. The overall Sith decision remains **NOT CLEARED**.

## Semantic disposition detail for broad branches — 2026-10-21 19:12 UTC

Spot-checked branch-only public APIs against the current source tree. This is not yet a full per-member audit, but it identifies concrete gaps that cannot be declared superseded solely because the files are old.

| Branch-only item | What the branch source exposes | Current-development comparison | Required disposition |
|---|---|---|---|
| `src/Core/ReflectionGuiBuilder.cs` | `ReflectionGuiBuilder.Create(object model)` projects an arbitrary public object model into a `GuiNode`. | No same-path file or `ReflectionGuiBuilder` type in current Core. Current `GuiBuilder` / `GuiBuilders` construct semantic trees explicitly; they are not demonstrated equivalents for reflection projection. | Decide whether reflection-based projection remains a desired feature; if yes, port/rebuild with tests, otherwise record deliberate retirement. |
| `src/Core/GuiHub.cs`, `Gui.cs`, `GuiKinds.cs`, `IGuiHub.cs` | A stable-ID hub/root surface and centralized kind constants. | These types are absent by path/type from current Core; current renderer entry points operate directly on `GuiNode`. | Establish whether the hub abstraction was intentionally removed in the newer architecture or whether any host/root semantics were lost. |
| Branch WPF `GuiBuilderWPF` / `GuiContextWPF` | Older broad window/chrome, interaction-mode, status reporting, and fluent element composition APIs. | Current development has `WpfWindowBuilder`, `NativeGuiBuilder<T>`, and `GuiPanelBuilderWPF`-based composition, but API/blob hashes differ. | Compare capabilities and lifecycle/interaction behavior, not names alone; document replaced contracts and test any retained behavior. |
| `TreeViewGuiBuilderWPF` | Stable-key branch/leaf model, payloads, node selection, expansion callbacks. | No same-path TreeView builder in current WPF tree. | Check whether tree selection/expansion is required; port or explicitly retire. |
| `PivotGridGuiBuilderWPF` | Builds a cross-tab/pivot view from an enumerable and public property names. | No same-path pivot-grid builder in current WPF tree. | Evaluate as a specialized adapter feature; do not silently discard if still wanted. |
| `CrudGuiBuilderWPF<T>`, `SovereignLogViewerGuiBuilderWPF`, `SovereignUIFactory` | Generic CRUD panel, bounded/filterable log viewer, and factory methods for WPF layout/control builders. | No same-path equivalents found in the current WPF tree. | Determine whether these were experimental/application-specific utilities or desired package APIs; record explicit decision. |
| `src/SVG/SvgRenderOptions.cs` on PR #11 | `SvgRenderOptions` with `Title`, `Description`, `Width`, `Height`, and `Background`; renderer signature accepts options. | Current development exposes `Render(GuiNode root, int width = 960, int height = 540)`; no options record, title/description/background options overload, or matching type found. | This is a concrete API difference. Decide whether to port the options contract with tests/accessibility checks or deliberately keep the narrower current API. |
| PR #11 visual assets and `docs/LIVING_DOCUMENTATION.md` | Conceptual animated energy-vortex SVG and semantic-boundary SVG; a living-documentation guide. | These paths are absent by path from current development. The vortex file itself labels its visual as a proof of concept, not renderer output or physical simulation. | Preserve only if they fit current gallery/documentation standards; label conceptual vs generated artifacts accurately. |

The current tree does contain newer `EnumControlBuilderWPF` and `NumericControlBuilderWPF`, so the branch's enum/numeric builder files may be functionally replaced despite path/name changes. That equivalence must be supported by comparing contracts/tests before closing the item.

**Updated disposition:** PR #8 and PR #11 remain **preserve-and-classify**, not discard candidates yet. Do not merge wholesale. First decide the listed capabilities one by one, port only the approved useful behavior to development with tests, and then rerun the tree/PR inventory. No branch or PR state was changed.

## Publish-gate duplicate review — 2026-10-21 19:14 UTC

Fetched the current PR diffs and package workflow from `development`, `work/nuget-publish-gate` (PR #15), and `work/gui-publish-gate-reconciled` (PR #19).

- Both PR diffs make the same one-line logical change: remove `&& false` from the `publish_nuget` job condition, leaving `github.event_name == 'workflow_dispatch' && inputs.publish == true`.
- The current development workflow still contains `&& false`, so the publish job remains deliberately disabled even when a manual dispatch sets `publish=true`.
- PR #19 is the more recent/reconciled proposal and GitHub reports it mergeable at the prior live check; PR #15's historical run was cancelled and does not provide passing evidence. However, #19's workflow file is still a different snapshot from current development, so recheck the actual merge diff/required checks before any future merge.
- Removing `&& false` would change the publication guard from permanently disabled to eligible on a manual dispatch with `publish=true`. That is a release-control change, not a harmless CI cleanup.

**Disposition:** treat #15 as a duplicate of #19 for review purposes, but do not close #15 or merge #19 automatically. Publishing is not authorized. Keep the current development guard unchanged until the owner explicitly approves enabling publication, and preserve the rule that readiness or green CI alone is not release authorization.

## Remaining branch-tree triage — 2026-10-21 19:16 UTC

Recursive tree snapshots compared the following live branch refs against `development` (153 blob paths at the time of the query). These are path/blob inventories, not semantic proof of equivalence.

- `feature/integrated-fsm-cos-hub` (99 paths): 25 paths absent from development. It shares the same older hub/reflection/WPF-specialty source family identified under PR #8, including `GuiHub`, `ReflectionGuiBuilder`, WPF TreeView/PivotGrid/CRUD/log-viewer/factory APIs, plus older docs and tests. Treat as a related preservation audit, not a separate automatic deletion candidate.
- `feature/nested-experiences` (77 paths): 5 paths absent from development: `NuGetReadme.md`, `src/Core/Gui.cs`, `src/Core/GuiHub.cs`, `src/Core/IGuiHub.cs`, and `tests/Core.Tests/GuiHubTests.cs`. These overlap the hub feature family and need an explicit retirement/equivalence decision.
- `work/svg-app-fsm-cos-boundary` (82 paths) and `work/webpage-event-builder-compatibility` (72 paths): no path-only additions versus development in this tree snapshot, but many existing paths have different blob contents. Their source/workflow/test diffs still require semantic review before deletion; “no branch-only path” does not prove every behavior is preserved.
- `work/gui-publish-gate-reconciled` (147 paths): no branch-only paths, six changed paths in the current tree comparison, including the package workflow, solution, nested-experience docs, and a DataCard API/UML page/diagram. Its one-line publish-gate change is documented above, but the changed DataCard documentation/diagram and solution diff should be reconciled before any closure/discard decision.
- `work/nuget-publish-gate` (104 paths): no branch-only paths; many files differ because it is an older snapshot. Its one-line publish-guard change is documented above. Do not merge its broad historical snapshot.
- `work/gui-wpf-tests` (105 paths), `work/gui-release-mit-wpf-scope` (105 paths), `work/gui-uml-per-file-documentation` (113 paths), and `fix/guinode-nullability-contract` (81 paths): no path-only additions versus development in these snapshots. This supports the prior redundant/superseded assessments, but final disposition still requires checking each branch's exact current diff/PR relationship and retaining the known unique behavior already identified in the audit.

**Disposition rule remains unchanged:** only `master` and `development` are keepers, but a non-keeper is not safe to delete until its branch-specific code, tests, workflow, docs, and assets have an explicit preserved/retired/port decision. No branch or PR state was changed by this triage.


## Live focused audit pass — 2026-10-21 19:52 UTC

### Fresh keeper/branch/PR inventory

- Live branch search returned **17 branches**: the two keepers `master` and `development`, plus 15 non-keepers. The names still match the inventory above.
- Live open-PR search returned **#8, #11, #15, #18, and #19**. All five remain open; #8 and #18 are drafts. No branch, PR, or release state was changed in this pass.
- The current `development` package workflow blob is `526aea54700c3617822825a90bbd9f9ec5ae7eea`. Its publish condition remains `github.event_name == 'workflow_dispatch' && inputs.publish == true && false`; NuGet publication is still deliberately disabled.
- Current qualitative release readiness remains **72%** per the source-derived UML staging status. This is not release-ready and is not a merge or publication authorization.

### Exact branch comparisons refreshed against current development

| Branch | Ahead / behind | Changed files in live compare | Current interpretation |
|---|---:|---|---|
| `work/webpage-event-builder-compatibility` | 3 / 507 | `src/Blazor/WorkshopGuiBuilder.cs` (+2), `tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs` (+33) | Both event-handler implementations and the event test were fetched for direct comparison. The test file has the same blob SHA on both refs (`be6c179c751d8f817293f4f445584df23675afd5`); the branch's builder lacks current XML documentation and the current surrounding `PreventDefault` documentation/API is already on development. The branch's two added source lines are the existing `OnWheel` and `StopPropagation` members, not a unique feature. Strong intentionally-discardable candidate after PR #9's state is checked and the current inventory is repeated. |
| `work/svg-tree-depth-guard` | 5 / 444 | `docs/SVG_ADAPTER.md` (+11/-2), `src/SVG/SvgGuiRenderer.cs` (+22/-12), `tests/SVG.Tests/SvgGuiRendererTests.cs` (+25) | Current development has the 256-depth guard and iterative unique-ID validation, plus title/description IDs and `aria-labelledby` accessibility linkage missing from the older branch renderer. Do not copy the renderer wholesale. The unique test additions still need a hunk-level check before deciding whether any regression assertion should be ported. |
| `feature/integrated-fsm-cos-hub` | 76 / 513 | Broad workflow, README/docs, Core, Blazor, WPF, and tests; includes new `GuiHub`, `IGuiHub`, `GuiKinds`, `Gui`, reflection and specialized WPF builder files | This is a legacy feature snapshot, not a safe merge unit. No current type-level equivalence has been demonstrated for the old hub/root contract, reflection projection, or all specialized WPF builders. Explicitly decide whether to rebuild selected contracts with current APIs/tests or intentionally discard the legacy architecture. |
| `work/gui-publish-gate-reconciled` | 1 / 34 | `.github/workflows/package.yml` only (+1/-1) | The proposed one-line change removes `&& false`. This is a release-control change and remains unauthorized. Keep the current development hard stop. |

### Architecture preservation note

The legacy hub/reflection family remains an unresolved product decision, not a branch-cleanup technicality:

- `ReflectionGuiBuilder.Create(object model)` projects arbitrary public object models into `GuiNode`; the current explicit builder API has not been shown to provide the same reflection projection contract.
- `GuiHub`, `IGuiHub`, `Gui`, and `GuiKinds` add stable-ID/root/hub abstractions that are not the same API as renderers consuming `GuiNode` directly.
- The legacy WPF branch contains TreeView, PivotGrid, CRUD, log-viewer, factory, multi-panel/tabbed and reflective builder surfaces. Some enum/numeric controls have newer equivalents on development, but the remaining specialized behavior has not been proven equivalent.
- PR #11's current description says it is documentation-only at `docs/SVG_GUIDE.md`; the branch comparison history is highly divergent, so review the actual PR patch, not the whole branch snapshot. Do not merge the branch wholesale.
- PR #18's UML/source-derived generator remains draft. Previously reported passing runs only prove the SHAs named by those runs; exact-head CI and remaining implementation gates are still required.

### Decision

**Sith status remains NOT CLEARED.** This pass performed read-only source/compare/inventory checks and updated only this audit document. No branches were deleted, no PRs were merged or closed, and NuGet publication remains disabled. The next meaningful gate is to finish the hunk-level test review for the SVG depth-guard branch and record an explicit rebuild/discard decision for the legacy hub/reflection/specialized-WPF contract family.


### SVG depth-guard test-name reconciliation — 2026-10-21 19:53 UTC

The test source was fetched from both current `development` and `work/svg-tree-depth-guard` and test method names were compared. Every test method present on the older branch also exists on development, including `Rejects_negative_geometry_dimensions`, `Accepts_tree_at_the_configured_maximum_depth`, and `Rejects_excessive_tree_depth_before_recursive_svg_serialization`. Development additionally has six methods absent from the branch: embedded stylesheet rendering, XML/non-ASCII escaping, deterministic repeated rendering, malformed/non-finite numeric rejection, blank required-reference rejection, and duplicate IDs with enabled coordinate overlays. Therefore the branch adds no uniquely named regression test method at this snapshot; it remains older in both renderer accessibility linkage and test breadth. This supports classifying `work/svg-tree-depth-guard` as intentionally superseded after confirming the live PR/branch inventory again, rather than porting tests based solely on the branch's raw added-line count.
