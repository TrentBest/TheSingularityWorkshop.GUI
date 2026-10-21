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
