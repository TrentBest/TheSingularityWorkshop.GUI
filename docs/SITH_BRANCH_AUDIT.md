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


## Exact-head WPF shutdown failure and inventory refresh — 2026-10-21

### WPF CI evidence

The live [Package GUI run #39656628688](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39656628688) targets development SHA `3e03d572101d2be627e97aa9407e64c3e7ec9627`. Its jobs are:

- `test-and-package`: **success**
- `test-wpf`: **failure**
- `publish_nuget`: **skipped**

The Windows WPF job successfully restored and built Core, WPF, and the WPF test project. The test runner reported 23 passed, 0 failed, 0 skipped, then aborted because the test-host process crashed with an unhandled `NullReferenceException` in `System.Threading.Thread.get_CurrentThread()` called by `MS.Win32.HwndSubclass.SubclassWndProc`. The step exited with code 1. Therefore this is a failed CI run; the 23 reported test successes do not make the job green.

The available log establishes that the crash occurs after the test runner's passing-test summary and during/around WPF native-window message handling or teardown. It does **not** identify which test/window owns the subclass or prove a particular fix. The next code investigation should inspect dispatcher/STA ownership, window lifetime, event/subclass cleanup, and test fixture disposal; reproduce locally or on a diagnostic CI commit, then require a clean Windows run on the exact resulting development SHA. Do not paper over this with unconditional success, a swallowed process exit, or by removing WPF coverage. The current log alone is insufficient to prescribe a safe source change.

The WPF build also emits CS1591 warnings for public WPF types and members. These warnings are not the cause identified by the log, but remain documentation quality debt and should be tracked separately from the crash.

### Live cleanup inventory refresh

The latest branch search still returns exactly 17 branches: `master`, `development`, and 15 non-keepers. The open-PR search still returns #8, #11, #15, #18, and #19. The Package GUI run's NuGet job was skipped. No branch was deleted, no PR was merged or closed, and the publish hard stop was not changed.

### Decision

**Sith status remains NOT CLEARED.** The WPF shutdown crash is a concrete blocker to calling current CI green. Continue semantic branch-by-branch reconciliation and resolve the five open PRs before issuing any deletion list. Do not enable or publish NuGet packages as part of this work.


## Fresh branch-diff pass — 2026-10-21 19:56 UTC

### Inventory and current comparison anchors

- Re-ran the live branch search: the same 17 branches remain, with only `master` and `development` designated as keepers.
- Re-ran the open-PR search: #8, #11, #15, #18, and #19 remain the known open review set. The source search for open PR #9 returned no result; because that search is not an authoritative per-number state endpoint, do not use it alone to infer PR #9's lifecycle state.
- Fresh compare responses identify current `development` at `293f42442584d5ff88b8cb14f00cc846917e7f79` for these comparisons.

### Newly checked branch dispositions

| Branch | Fresh comparison | Finding | Disposition |
|---|---|---|---|
| `work/gui-wpf-tests` | 0 ahead / 263 behind / no changed files | Exact compare reports no branch-only commits and no file changes relative to current development. This is the strongest technically redundant branch candidate in the current inventory. | Mark as **safe-to-discard candidate**, not deleted. Verify no open PR or other unresolved reference before the owner removes it. |
| `work/svg-app-fsm-cos-boundary` | 5 ahead / 447 behind; README +2/-2 and `docs/SVG_APP_CONCEPT.md` added relative to the merge base | The concept document is present on both refs at the same blob SHA `f98b408b2fba433017892c59182118ab6aaf47f3`. It carefully states that SVG_App is not implemented, describes FSM_COS `RuntimeAssembly` and GUI.SVG `GuiNode`/renderer as separate contracts, and proposes an assembly-inspection SVG as the first proof. Current README already links the concept. Thus this branch's apparent doc addition is already present on development; its remaining difference is a one-line README presentation change, not a unique architecture artifact. | Strong superseded candidate after confirming no open PR points to it and checking the README hunk for any desired wording. Do not merge the branch snapshot. |
| `work/gui-release-mit-wpf-scope` | 16 ahead / 264 behind; eight changed paths | The branch still has changes to the package workflow, root license, release work queue/readiness, and Core/Blazor/WPF project metadata. This is broader than a documentation-only duplicate. Current development already includes MIT package metadata and WPF scope, but the comparison confirms real historical differences remain. | Do not discard based on branch age or file names alone. Reconcile the exact workflow/project hunks against current development, particularly package license/readme/framework/XML metadata and the deliberate `&& false` publish hard stop. |
| `work/gui-verify-xml-docs-in-packages` | 4 ahead / 263 behind; workflow, release work queue, and readiness docs changed | The branch contains an all-four-package archive check for MIT license expression, README metadata/file, target framework, DLL, and XML docs. Current development's package workflow already has a corresponding all-four-package metadata verification step; the branch's workflow also carries an older overall snapshot and must not be merged wholesale. The readiness/queue text is older than current per-type UML gates. | Likely superseded, but retain as a discard candidate until a line-level comparison confirms the current workflow covers every assertion from the branch. Do not infer complete equivalence merely from the step name. |

### Guardrails retained

- No branch deletion, PR merge/closure, or package publication occurred in this pass.
- The current development package workflow's publish job remains hard-disabled by `&& false`; this is not to be changed without explicit authorization.
- Current source-derived UML implementation readiness remains **72%**. That percentage describes the staging UML work, not the overall package release, not Sith cleanup completion, and not authorization to merge or publish.
- Sith remains **NOT CLEARED**. The WPF test-host shutdown crash previously recorded in this document remains an independent CI blocker until a clean Windows WPF run passes on the exact resulting development SHA.


## WPF HWND teardown hypothesis and targeted test cleanup — 2026-10-21 19:57 UTC

The WPF test source review found a concrete lifecycle gap in `Window_builder_composes_content_and_host_options_without_showing_the_window`: it showed and hid an owner `Window`, then allowed the STA test thread to exit without closing that native window. The run log's crash in `HwndSubclass.SubclassWndProc` after all 23 tests reported passed is consistent with a stale WPF HWND/subclass receiving work as its owning thread exits, although the log does not prove this is the root cause.

A narrow test-only change was committed as `6fcc53c6646b889f593438b411204717f01bab2b` (`test: close WPF owner window before STA teardown`). The test now closes the built child window and closes the shown owner in `finally` blocks, including assertion-failure paths. This preserves all 23 tests and their assertions; it does not suppress the crash or change production behavior.

Exact-head Package GUI run [#39666044705](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39666044705) completed successfully on this test-only commit. Both `test-wpf` and `test-and-package` passed; `publish_nuget` was skipped. This is direct evidence that the cleanup change removed the observed CI failure on this exact head. It supports the HWND/STA teardown hypothesis but does not prove it is the only possible cause. Keep the fix and retain the WPF tests; do not suppress process exit or remove coverage.

No branch was deleted, no PR was merged or closed, and the NuGet publish hard stop remains in place.


## Event-builder compatibility branch: exact source reconciliation — 2026-10-21

A fresh three-way branch-tip comparison for `work/webpage-event-builder-compatibility` against current `development` reports 3 commits ahead, 513 behind, with only two changed paths:

- `src/Blazor/WorkshopGuiBuilder.cs`: two added lines relative to the comparison base.
- `tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs`: a 33-line test file added relative to the comparison base.

The current `development` `ElementBuilder` already exposes both `OnWheel(Action<WheelEventArgs>)` and `StopPropagation(string eventName = "onclick")`, and creates their attributes through `EventCallback.Factory.Create`. Therefore, the branch's apparent API contribution is already represented on development. Its added test checks that the `onwheel` frame value is an `Action<WheelEventArgs>`, whereas the production method explicitly creates an `EventCallback<WheelEventArgs>`; that assertion does not match the implementation contract and should not be copied verbatim. A future regression test should assert the actual EventCallback type or invoke/inspect its delegate through supported APIs.

PR [#9](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/9) is confirmed **closed and merged** (merged 2026-10-13). This establishes the PR lifecycle, not that the branch tip is worth retaining. Disposition: **intentionally superseded candidate**; do not merge the stale branch snapshot. The owner should still recheck the live branch/PR references before deleting the branch.

## Exact-head WPF CI correction — 2026-10-21

The preceding section's queued status is superseded by the completed result for run #39666044705: both Windows `test-wpf` and Linux `test-and-package` succeeded on `6fcc53c6646b889f593438b411204717f01bab2b`; the NuGet publish job was skipped. This clears the recorded WPF CI blocker for that exact code/test head. A subsequent documentation-only audit commit will require its own workflow run; no publication or branch deletion is implied.


## Additional superseded-branch decisions — 2026-10-21

### `work/gui-verify-xml-docs-in-packages` / PR #17

A fresh comparison against current `development` reports 4 commits ahead and 266 behind, with changes only to `.github/workflows/package.yml`, `docs/NEXT_RELEASE_WORK_QUEUE.md`, and `docs/RELEASE_READINESS.md`. The all-four-package verification step for MIT license expression, README metadata/file, target framework, assembly, and XML documentation is byte-for-byte identical between the two workflow files. Both workflow versions also retain the `&& false` publish hard stop. Current development's release queue and readiness document are substantially more extensive and include newer candidate gates, including per-type UML coverage, so the branch's older prose does not replace current guidance.

PR [#17](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/17) is confirmed closed and merged (2026-10-19). Disposition: **intentionally superseded candidate**; do not merge the stale branch snapshot. Recheck live references before the owner deletes it.

### `work/svg-tree-depth-guard` / PR #10

The current `docs/SVG_ADAPTER.md` is 16,580 characters versus 8,420 on the branch and contains the newer manifestation vocabulary and broader current adapter contract. The current renderer already uses iterative tree prevalidation, a 256-edge depth guard, and duplicate-ID validation. The earlier method-name reconciliation found every test method on the branch represented on development, while development has six additional renderer tests for stylesheet rendering, XML/non-ASCII escaping, deterministic output, malformed/non-finite values, blank required references, and duplicate IDs with coordinate overlays.

PR [#10](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/10) is confirmed closed and merged (2026-10-16). Taken together, current implementation, broader tests, updated adapter guide, and PR lifecycle support disposition as an **intentionally superseded candidate**. This does not imply that its branch tip is byte-identical to development; it means the reviewed behavior and documentation contribution are represented by the newer development state. Recheck branch references before owner deletion.

### Current gate

The latest verified code/test head `6fcc53c6646b889f593438b411204717f01bab2b` passed both Windows WPF and Linux package/test jobs in run [#39666044705](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39666044705). The follow-up audit documentation is a separate commit and must be judged by its own exact-head workflow. The live inventory still contains 15 non-keeper branches and five open PRs (#8, #11, #15, #18, #19); no branch deletion or publication is authorized by these dispositions. Sith remains **NOT CLEARED** until every remaining branch and open PR has a documented final disposition and the owner rechecks the inventory immediately before cleanup.

## Correction: PR #11 is a broad historical patch, not a guide-only change — 2026-10-21

The live PR endpoint for [#11](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/11) reports **59 changed files and 112 commits**. Its actual patch includes the package workflow, root README and NuGet README, solution/project changes, Core and Blazor source, SVG and WPF implementations, tests, architecture/implementation/adapter docs, and the new `docs/SVG_GUIDE.md`. Therefore, PR #11 must not be treated as merely a guide-file overlap with PR #18 or closed based only on the current guide's greater length. The branch is deeply history-diverged (112 ahead / 521 behind in the branch-tip comparison), and the patch must be triaged by changed file and semantic contract. **Keep PR #11 and its head branch on hold; do not merge wholesale and do not delete it yet.** The title is not a reliable description of the patch's full scope.

## Per-file UML requirement reconciled from `work/gui-uml-per-file-documentation` — 2026-10-21

A branch-tip comparison found one unique file change on `work/gui-uml-per-file-documentation`: 19 added lines in `docs/DATA_CARDS_AND_UML.md`, defining per-file/per-type UML coverage, required source-accurate relationships and signatures, Mermaid `classDiagram` as the default portable source, explicit treatment of non-type files, and future CI enforcement. This requirement was missing from current development, so it has now been reconciled into `docs/DATA_CARDS_AND_UML.md` in commit `f5d5c836c47b2e10f2dfe4da8ecbf0ec9de282ae`. The branch's unique content is therefore represented on development; classify it as a **superseded candidate**, pending a final branch/reference check before owner deletion. This change records the documentation gate; it does not claim automatic per-file UML generation or enforcement already exists.

The current `development` head is now subject to fresh CI after the documentation changes. The last verified code/test head `6fcc53c6646b889f593438b411204717f01bab2b` passed both Windows WPF and Linux package/test jobs, but that success is not a substitute for checking the newest documentation commit's workflow.



## Open-PR patch-scope review — 2026-10-21

### PR #8 — datum/interaction boundary

The live PR files endpoint shows this is not an architecture-note-only patch. It changes the package workflow, root README, roadmap, architecture docs, adds datum/interaction and nested-experience docs, adds implementation-status/adapter docs, and includes substantial Core, Blazor, and WPF source/test changes (including GUI Hub and native builder surfaces). Those paths overlap older historical implementation families and current development has since evolved, but the patch still contains a large semantic surface. Keep PR [#8](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/8) in **hold/reconcile**; extract and compare individual contracts rather than merging the branch wholesale or closing it based on the PR title.

### PR #18 — source-derived GUI.SVG diagrams

PR [#18](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/18) remains an open draft with a substantial but focused set of changes: source/solution-derived UML workflows, generator/model code, per-file UML pages, diagram assets, and inventory/contract verification scripts. It is relevant to the new per-file UML gate, but its proposed generator and verification workflow must be audited against actual source truth, path coverage, determinism, and current CI before promotion. Keep it as **active retain/reconcile**; do not mark release-ready or merge it solely because it addresses a desired capability.

### PRs #15 and #19 — duplicate publish-gate proposals

The live files endpoints confirm that both PR #15 and PR #19 change only the same logical condition: removing `&& false` from the `publish_nuget` job's `if` expression. The branch-tip comparisons are history-diverged, but each actual PR patch is one line. Neither change is authorized by branch cleanup or readiness work. Current development deliberately retains the hard stop; publication remains disabled. Keep the hard stop unchanged and preserve the PRs until the owner explicitly chooses a canonical proposal and authorizes any release-gate change. Do not merge either automatically.

### Current decision

The branch inventory is still 17 total branches (15 non-keepers) and the open-PR set remains #8, #11, #15, #18, #19. The newly added per-file UML requirement is now present in development, but no automated coverage enforcement is claimed. The WPF owner-window cleanup passed both Windows WPF and Linux package/test jobs on its exact code/test head; later documentation commits have their own CI in progress. Sith remains **NOT CLEARED**. No branch was deleted, no PR was merged or closed, and NuGet publication remains disabled.


## Release/MIT/WPF scope branch reconciliation — 2026-10-21

Fresh comparison of `work/gui-release-mit-wpf-scope` against current `development` reports 16 commits ahead / 267 behind and eight changed paths. Exact file inspection now narrows the substantive contribution:

- `LICENSE`, Core/Blazor/WPF project files, and `src/WPF/README.md` are byte-identical between the branch and development.
- The branch's package workflow has an older, separately named “Verify all package license and README metadata” step. Current development's all-four-package verification step already performs those checks and additionally verifies target framework, DLL, and XML documentation entries.
- The branch's release queue/readiness prose is older and shorter. Its historical 74% readiness statement and historical CI links are not current candidate evidence; current development has the more extensive release gates and updated blockers.

No unique MIT license or WPF package metadata change remains to be ported from this branch based on the inspected files. Disposition: **superseded candidate**, subject to one final live branch/reference check before the owner deletes it. Do not merge the historical branch snapshot. This is a branch disposition only; current release readiness remains separate and the NuGet publish hard stop remains unchanged.


## Live recheck after WPF shutdown fix — 2026-10-21 20:11 UTC

This section is based on a fresh branch comparison against `development` at `24c9da30a4a0967dd22bf714fdaf40626f4613c9`, the current live branch inventory, open-PR search, and completed GitHub Actions jobs. It supersedes earlier counts where the base moved.

### Exact-head CI

- Code/test commit `6fcc53c6646b889f593438b411204717f01bab2b`: [Package GUI run #39666044705](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39666044705) completed successfully. Both `test-and-package` and `test-wpf` succeeded; `publish_nuget` was skipped.
- Follow-up audit commit `9c7461a8ec8946b153b6a75ba24c8a9e96057b9f`: [Package GUI run #39666083948](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39666083948) completed successfully. Both test jobs succeeded; `publish_nuget` was skipped.
- Later development documentation commit `24c9da30a4a0967dd22bf714fdaf40626f4613c9`: [Package GUI run #39666894240](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39666894240) completed successfully.
- The Windows test job success supports the targeted WPF test cleanup; it does not prove the crash's root cause beyond doubt or clear the branch audit.
- The live package workflow still contains `&& false` in the NuGet publish condition. Publishing remains disabled.

### Fresh branch inventory and dispositions

The live inventory remains 17 branches: `master`, `development`, and 15 non-keepers. Open PRs remain #8, #11, #15, #18, and #19.

| Non-keeper branch | Fresh comparison / evidence | Current disposition |
|---|---|---|
| `work/gui-wpf-tests` | Exact comparison: 0 commits ahead, 272 behind, no changed files relative to current development. | **Verified-redundant candidate.** Recheck its ref and PR references immediately before owner deletion. |
| `fix/guinode-nullability-contract` | 1 commit ahead; only `src/Core/GuiNode.cs` differs. Development uses the newer iterative traversal and has current XML docs. | **Superseded candidate.** Final ref/PR-reference check before deletion. |
| `work/gui-uml-per-file-documentation` | 1 commit ahead; its only unique change is the 19-line per-file UML requirement already reconciled into `docs/DATA_CARDS_AND_UML.md` in `f5d5c836c47b2e10f2dfe4da8ecbf0ec9de282ae`. | **Superseded candidate.** Final ref check before deletion; no claim of automated enforcement. |
| `work/gui-release-mit-wpf-scope` | 16 commits ahead; inspected LICENSE, package project metadata, WPF README, workflow and release prose are either byte-identical to current development or superseded by stronger current checks/docs. | **Superseded candidate.** Final ref check before deletion. |
| `work/gui-verify-xml-docs-in-packages` | 4 commits ahead; all-four-package metadata verification step is identical to current development's step, which checks additional metadata. Branch release prose is older; publish hard stop remains. PR #17 is closed/merged. | **Superseded candidate.** Final ref check before deletion. |
| `work/svg-tree-depth-guard` | 5 commits ahead; current renderer has iterative prevalidation, depth guard and duplicate-ID checks; current tests/guide are newer and broader. PR #10 is closed/merged. | **Intentionally superseded candidate** based on reviewed behavior/docs/tests; final ref check before deletion. |
| `work/svg-app-fsm-cos-boundary` | 5 commits ahead; branch adds `docs/SVG_APP_CONCEPT.md` (132 lines) and a README wording change. The concept document was not found on current development in the earlier live contents check. | **HOLD.** Preserve/reconcile the unique concept note or record an explicit discard rationale before deletion; a merged PR label is not enough. |
| `work/webpage-event-builder-compatibility` | 3 commits ahead; unique changes are two lines in `WorkshopGuiBuilder.cs` and a 33-line event test. | **HOLD.** Inspect semantics and establish whether current development covers the compatibility behavior. |
| `work/gui-uml-per-file-documentation` | See reconciled requirement above; its source diff is one docs file only. | No additional implementation contribution identified. |
| `work/gui-publish-gate-reconciled` and `work/nuget-publish-gate` | Each actual branch patch is one workflow-line change proposing to remove the same `&& false` hard stop; histories diverge, but the logical patch duplicates. | **HOLD.** Both PRs #19 and #15 remain open. Do not merge or enable publication as part of Sith. Owner must explicitly choose a canonical proposal and authorize gate changes. |
| `work/gui-svg-source-uml-generator` | PR #18 remains open/draft. Its broad unique work includes source/solution-derived UML model and SVG renderers, generated per-file diagrams, tests and verification scripts. | **ACTIVE RETAIN/RECONCILE.** Do not merge wholesale; verify source truth, coverage, determinism, generated assets and exact-head CI. |
| `work/svg-guide-current-contract` | PR #11 is a broad historical patch (59 files, 112 commits), not merely a guide. It includes workflow, docs, source, tests, and assets. | **HOLD.** Review each unique semantic contract; do not merge wholesale or discard based on title. |
| `architecture/datum-interaction-boundary` | PR #8's branch is a broad, deeply diverged snapshot including workflow, docs, Core/Blazor/WPF implementation and tests, not just the datum note. | **HOLD.** Extract reusable architecture principles and compare implementation/test changes individually; do not merge wholesale. |
| `feature/integrated-fsm-cos-hub` | 76 commits ahead, broadly overlaps legacy hub, Core/Blazor/WPF and tests. | **HOLD for explicit disposition.** Current host-owned lifecycle direction conflicts with parts of this older hub approach; inspect whether any unique contracts/tests need preservation, then record discard/rebuild decisions. |
| `feature/nested-experiences` | 38 commits ahead, with unique nested-experience docs and broad Core/Blazor changes. | **HOLD for explicit disposition.** Compare useful contract/documentation against the current host-owned lifecycle direction; no wholesale merge. |

### Current clearance decision

This pass strengthens the list of superseded candidates but does **not** clear Sith. The concept note on `work/svg-app-fsm-cos-boundary`, the Blazor event-builder compatibility change, the broad open PRs #8/#11/#18, and the legacy feature branches still require explicit reconciliation or documented discard. PRs #15 and #19 remain open and publication remains deliberately disabled.

No branch was deleted, no PR was merged or closed, and no package was published. The owner remains responsible for branch deletion after the audit reaches a complete, current, evidence-backed all-clear.


## Semantic cross-check correction — 2026-10-21 20:12 UTC

A direct file-by-file recheck resolved two holds from the preceding section:

- **`work/svg-app-fsm-cos-boundary`: superseded candidate.** `docs/SVG_APP_CONCEPT.md` exists on current development and is byte-for-byte identical to the branch copy (8,501 characters). The only other reported branch-tip difference is a one-line README wording change. The unique concept note is therefore preserved; no separate port is needed. PR #12 was previously reported closed/merged. Recheck live refs before owner deletion.
- **`work/webpage-event-builder-compatibility`: superseded candidate.** The 33-line `tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs` is byte-for-byte identical between branch and development (1,163 characters). The branch's `WorkshopGuiBuilder.cs` has the same executable statements as development; the current development version adds XML documentation comments around the existing API. No unique event behavior or test remains to port. Recheck branch and PR references before owner deletion.

These are explicit updates to the preceding fresh branch table: those two rows no longer require content reconciliation. They are not evidence that all other branches or open PRs are cleared. The latest audit-document commit has a new workflow run; do not use the earlier passing run as exact-head evidence for this correction.

## Live inventory and semantic refresh — 2026-10-21 20:15 UTC

### Exact current anchors

- Fresh branch search and pagination return **17 total branches**: the two keeper branches (master, development) and 15 non-keepers.
- The current development comparison anchor is 3901b2ffd43d93b20aa054090ea30d4479fb4fe5 (the base SHA returned by fresh branch comparisons).
- The package workflow still contains the explicit && false hard stop on publish_nuget; do not remove it without explicit user authorization.
- The commit-workflow lookup for the queried development SHA returned no PR-triggered workflow records. That endpoint is PR-triggered-only and therefore is **not** evidence that all workflow runs are absent or that the newest audit commit passed CI. Require exact-head checks for the actual current development SHA before calling it green.

### Newly refreshed dispositions

- **work/gui-wpf-tests** — fresh compare: **0 ahead / 273 behind / no changed files** against current development. Branch-specific PR search returned no matching PR. This is a strong **safe-to-discard candidate**; still do not delete it on the assistant's authority, and recheck the complete branch/PR inventory immediately before owner deletion.
- **fix/guinode-nullability-contract** — fresh source comparison disproves a simple nullability-only description. The branch's GuiNode.TryFind recursively calls child TryFind; current development uses an explicit stack to preserve depth-first, left-to-right order without recursive call-stack growth, and has XML documentation for the public contract. Current development therefore contains a material robustness improvement plus documentation absent from the branch. The branch has only one commit ahead and 456 behind in the prior live compare, with no branch-specific PR returned by search. Disposition: **superseded candidate**, not a source to merge or port.
- **work/svg-app-fsm-cos-boundary** — fresh compare: **5 ahead / 456 behind**. docs/SVG_APP_CONCEPT.md has the same blob SHA on both refs (f98b408b2fba433017892c59182118ab6aaf47f3); it is already preserved on development and clearly marks SVG_App as a concept, not implemented software. The remaining README variation is from a stale branch snapshot. Disposition: **superseded candidate**, pending the owner's final live-reference check.
- **work/svg-tree-depth-guard** — fresh compare: **5 ahead / 457 behind**. Previously documented source/test review found its depth guard and test behavior represented in current development, with broader current adapter documentation and additional edge/error tests. PR #10 is closed and merged. Disposition remains **intentionally superseded candidate**; no wholesale merge.
- **work/nuget-publish-gate** — fresh compare: **1 ahead / 274 behind**, with only the workflow's one-line publish-gate change. It duplicates the logical change in PR #15/#19: removing the hard stop. Disposition remains **HOLD** until the user explicitly chooses and authorizes a canonical gate change. Do not enable publication.
- **work/gui-release-mit-wpf-scope** — refreshed compare now reads **16 ahead / 273 behind**. The earlier exact-file reconciliation stands: current package project metadata and release gates cover its contribution; do not merge the stale branch snapshot. Mark **superseded candidate**, subject to final owner-side reference check.
- **work/gui-verify-xml-docs-in-packages** — refreshed compare now reads **4 ahead / 272 behind**. The earlier exact workflow inspection established the all-four-package archive assertions are already byte-identical in development; the older queue/readiness text is superseded. PR #17 is closed and merged. Mark **superseded candidate**, subject to final owner-side reference check.

### Clearance and readiness

The current candidate list is more precise, but Sith is still **NOT CLEARED**. The broad open PRs #8 and #11 remain on hold; PR #18 remains active draft/reconcile; PRs #15 and #19 remain held because they propose removing the deliberate publication hard stop. The legacy hub/nested-experience branches still require explicit semantic discard/rebuild decisions. No branch was deleted, no PR was merged or closed, and no package was published.

Source-derived UML staging readiness remains **72%**. This is not overall package release readiness or Sith completion. Exact-head CI on the latest development SHA must be confirmed independently before claiming the latest audit is green.

## Legacy hub and WPF builder family — semantic boundary check — 2026-10-21

Three current development documents clarify how to handle the legacy branch family without confusing design intent with shipped API:

- docs/GUI_NESTED_EXPERIENCES.md explicitly labels nested experiences as a proposal and says current development does not contain the legacy Gui.Hub / GuiHub API.
- docs/GUI_EXECUTION_MODEL.md explicitly labels the execution-host architecture as proposed, not a completed runtime contract.
- docs/WPF_BUILDER_PATTERN_AUDIT.md is a transfer plan, not a mandate to port the historical builders. It records current implementations for native defaults, repeatable panel construction, a separate WpfWindowBuilder, and typed numeric/enum controls. It specifically rejects blindly importing arbitrary-object reflection, app-specific resource dictionaries, and external application/service dependencies.

The legacy branch source confirms that Gui.Hub is a lazily created process-wide singleton, while the old WPF branch includes specialized stable-key TreeView selection/expansion callbacks and separate PivotGrid, CRUD, log-viewer, multi-panel, tabbed-panel, color-picker, and reflective builder surfaces. Those are not all equivalent to current GuiNode renderers/builders, and their absence must not be misreported as successful feature parity.

**Disposition:** do not merge the old hub/WPF branch family wholesale. Current docs preserve the architecture proposal and the WPF audit records which reusable patterns have been adopted. However, the specialized WPF surfaces still need an explicit product decision: intentionally retire them, or rebuild selected capabilities against current APIs with focused tests. Keep the affected branches in **HOLD** until that decision is recorded; this is not a safe-to-discard classification.

No source API was added, no PR lifecycle changed, and no branch was deleted by this review.

## Additional exact-content branch checks — 2026-10-21 20:15 UTC

### `work/svg-app-fsm-cos-boundary`

The unique concept file `docs/SVG_APP_CONCEPT.md` is already present on development with the exact same blob SHA as the branch (`f98b408b2fba433017892c59182118ab6aaf47f3`). It correctly labels SVG_App as a proposed host and separates FSM_COS composition, host interpretation, GUI Core scene construction, and SVG manifestation without claiming the bridge exists. The branch's README is an older snapshot missing current README structure, living demonstration gallery links, current platform claims, and current navigation; its conceptual link is already preserved on development. The branch comparison reports five ahead / 461 behind, with no remaining unique concept-file contribution. Disposition: **superseded candidate**, subject to final live reference check before owner deletion.

### `fix/guinode-nullability-contract`

The only changed path is `src/Core/GuiNode.cs`. Development already carries the nullable `TryFind(string, out GuiNode?)` contract, XML API documentation, and iterative depth-first traversal (avoiding recursion-depth failure on deeply nested trees). The branch version is an older recursive traversal and lacks the current XML docs. Its nullability contract is represented on development; no unique behavior from this one-file branch is required. Disposition: **superseded candidate**, pending final reference check.

### `work/gui-wpf-tests`

The branch-tip comparison is exact: zero commits ahead, 277 behind, and zero changed files. Its content is already an ancestor/snapshot of development with no unique patch. Disposition: **superseded candidate**, pending final reference check.

### Legacy nested-experience / integrated-hub implementations remain on hold

`feature/nested-experiences`, `feature/integrated-fsm-cos-hub`, and `architecture/datum-interaction-boundary` are not safe to classify as superseded solely because their history is old. Their branch-tip diffs contain substantial historical source and test changes, including the legacy `Gui.Hub` singleton, hub contracts/renderers, and specialized WPF builders. Current documentation deliberately marks the nested-experience/execution-host API as a proposal and says those legacy APIs are not present on development. This is an explicit architectural boundary, not proof that every specialized capability is unnecessary. Keep these branches in **HOLD / product-decision required** until the owner decides which legacy capabilities are retired versus rebuilt against the current model. Do not merge their historical snapshots wholesale and do not delete them yet.

### Source-derived UML generator branch stays active

`work/gui-svg-source-uml-generator` is 227 commits ahead / 63 behind with a substantial patch across Roslyn/MSBuild-based project analysis, renderer-neutral UML models, type pages, SVG relationship diagrams, generated inventories, and CI verification. This is directly aligned with the repository's desired source-derived living UML direction. PR [#18](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/18) remains the active draft. The branch needs focused code/CI review and controlled reconciliation, not a superseded label. Retain it; do not merge wholesale merely because the capability is wanted.

### Current inventory and verification boundary

The latest live inventory still reports 17 branches total, 15 non-keepers, and open PRs #8, #11, #15, #18, and #19. The exact-head WPF code/test fix passed run [#39666044705](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39666044705). Documentation audit commits through `6e05337e324da2f467b3fb5f98c9c039ec664d10` are separate commits; the exact-head workflow [#39668518350](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39668518350) was still in progress at the time of this update. Do not claim the newest head is green until it completes. No branch deletion, PR closure/merge, or package publication is authorized by this audit.



## Open PR snapshot comparison — 2026-10-21 20:15 UTC

Re-read the current PR diffs rather than relying on their titles:

- **PR #8** (`architecture/datum-interaction-boundary`) proposes a broad historical CI redesign alongside its datum/human-bridge architecture. Its workflow has separate Core/Blazor/WPF packaging jobs and hard-disabled NuGet jobs, but its WPF lane only builds and packs; it explicitly says WPF tests will be added once the test project exists. Current development already has a dedicated `test-wpf` lane, so the PR workflow is not a drop-in replacement. Keep the PR open/held while extracting the small architecture contract; do not merge the historical snapshot wholesale.
- **PR #11** (`work/svg-guide-current-contract`) is also a historical repository snapshot, not a narrow guide fix. Its workflow introduces an SVG build/test/package lane and hard-disabled publishing jobs, but its WPF lane likewise stops at build/pack rather than running the current WPF test suite. Current development has newer renderer behavior, tests and package checks. Review the guide's claims against current renderer/tests and port only verified corrections; do not replace current CI with the old workflow.
- **PR #18** (`work/gui-svg-source-uml-generator`) is the substantive source-derived UML effort. Its additions include source and solution-derived inventory workflows, compiler/reflection identity checks, syntax/project-semantic model generation, generated SVG diagrams and test scripts. This is valuable but is not cleared for merge solely because it has breadth or generated output. Require exact-head CI, determinism/reproducibility checks, source-span/provenance validation, and review of generated artifacts and schema compatibility against the repository's documentation standard.

### CI checkpoint

- [Run #39668384128](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39668384128), SHA `67f91b255d04bf5a499e3e4a6e67db9fcbec2add`, completed successfully: `test-and-package` and `test-wpf` passed; `publish_nuget` was skipped.
- The newer audit-only commit `6e05337e324da2f467b3fb5f98c9c039ec664d10` is running as [run #39668518350](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39668518350) at the time of this entry. Its own result is not yet known.

No PR state changed. No merge, branch deletion or publication was performed.

## Source-derived UML generator branch — do not discard — 2026-10-21

Fresh comparison against development at 6e05337e324da2f467b3fb5f98c9c039ec664d10 shows work/gui-svg-source-uml-generator at **227 commits ahead / 63 behind**. Its unique contribution is substantial and must not be treated like a stale one-file branch:

- Adds samples/SourceUmlGenerator, including syntax-only and MSBuild/Roslyn project-semantic modes.
- Adds a renderer-neutral UmlDocumentationModel with source/revision provenance, identities, source spans, typed relationships/resolution states, and compiler-symbol provenance.
- Adds .github/workflows/source-derived-diagrams.yml with syntax-mode and project-semantic model generation plus a wrong-assembly identity rejection check.
- Adds SOURCE_DERIVED_UML.md, SOURCE_DERIVED_DIAGRAMS.md, per-source-type pages, inventory and visual audit docs, model/rendering code, and generator tests.

Verification against current development confirms docs/SOURCE_DERIVED_DIAGRAMS.md and .github/workflows/source-derived-diagrams.yml are not present at those paths on development. Therefore this branch is **HOLD — unique implementation/reconciliation required**, not safe to discard. The branch also includes broad historical changes and is 63 commits behind, so do not merge the whole branch blindly. Reconcile the generator/model/workflow and validated documentation as a focused PR or selective port, then run its exact-head workflow and inspect the generated artifacts. Preserve the branch until that reconciliation is complete.

### Readiness metric distinction

Current docs/RELEASE_READINESS.md still states a **70% qualitative package release readiness** estimate, with per-type docs/UML and CI drift enforcement incomplete. The Sith audit's **72%** is only the separate source-derived UML staging estimate and should not be presented as the package release readiness percentage. The latest source-derived generator workflow exists only on the held branch at this comparison anchor; do not imply that it already runs on development.

No branch was deleted and no PR lifecycle or package publication state was changed.

## Owner directive: preserve all UML-related work — 2026-10-21

**Policy clarification:** UML-related work is explicitly in scope to KEEP. Do not classify or delete a branch merely because its changes are documentation-only, generated diagrams, source-model code, SVG rendering, per-file/type pages, inventory, or UML CI. First preserve its unique requirement/implementation and reconcile it into the keeper branch or into a deliberately retained active workstream.

### Protected UML workstreams

- **`work/gui-svg-source-uml-generator` / PR #18 — ACTIVE RETAIN/RECONCILE.** Fresh compare: 227 commits ahead / 66 behind development. Preserve its unique Roslyn/MSBuild generator, renderer-neutral UML model, source-derived and solution-derived workflows, SVG renderers, per-file/type outputs, validation scripts, tests, and provenance/identity contract. Do not merge the broad branch wholesale; reconcile focused slices and require exact-head CI plus generated-output/source-truth review.
- **`work/gui-uml-per-file-documentation` — preserve the requirement; branch disposition only after verification.** Its unique 19-line requirement for per-file/per-type UML, accurate relationships/signatures, non-type-file treatment, and future enforcement was recorded in development by commit `f5d5c836c47b2e10f2dfe4da8ecbf0ec9de282ae`. That means the content is preserved; it does not mean automated enforcement is complete. Before the branch itself is considered redundant, verify the entire branch-tip diff and confirm there is no additional UML material.
- **UML-related work in `work/svg-guide-current-contract`, `work/gui-release-mit-wpf-scope`, and other branches:** inspect all UML/SVG diagrams, generated artifacts, model/schema changes, tests, and CI assertions as separate contributions. A branch may be stale overall while still containing material that must be harvested. Never use branch-level age or ahead/behind counts as a substitute for this content-level check.

### Required disposition vocabulary

- **PRESERVE / ACTIVE:** unique UML implementation remains under active development.
- **RECONCILED:** exact requirement or implementation has been verified in `development`; branch deletion still waits for final branch/PR reference checks and the owner's manual action.
- **HOLD:** unique content has not yet been fully inspected or integrated.
- **VERIFIED REDUNDANT:** only after exact content/behavior comparison proves every useful UML contribution is already preserved.

Until the content-level UML inventory is complete, no UML-related branch may be placed on a safe-to-discard list solely because it is old, divergent, documentation-only, or has a low ahead count. This policy does not authorize branch deletion, PR closure/merge, or publication.


## UML/SVG content-level sweep — 2026-10-21 20:21 UTC

Fresh comparisons were run against the current `development` head `93db853cf5b3e1c35f30ba975b3845145bcfaaa5`; these counts are snapshots and will change as work lands.

- **`work/gui-uml-per-file-documentation`: 1 ahead / 214 behind; one changed file, `docs/DATA_CARDS_AND_UML.md` (+19 lines).** This branch's visible unique change is the per-file/per-type UML requirement already recorded in development. Disposition: **RECONCILED requirement; HOLD branch deletion** until the complete tip diff and live PR/reference check confirm there is no additional content.
- **`work/svg-guide-current-contract`: 112 ahead / 534 behind; 59 changed files.** The diff includes actual SVG renderer implementation and tests (`src/SVG/SvgGuiRenderer.cs`, `SvgRenderOptions.cs`, `tests/SVG.Tests/SvgGuiRendererTests.cs`), SVG guide/docs, concept artwork, and workflow changes. This is not just stale prose. Disposition: **HOLD / content harvest required**; compare each unique renderer behavior and test against development before calling this branch redundant.
- **`work/svg-tree-depth-guard`: 5 ahead / 465 behind; 3 changed files** across renderer, tests and `docs/SVG_ADAPTER.md`. The previously recorded iterative prevalidation/depth guard and duplicate-ID checks are present in current development; retain this as a superseded candidate only after the entire current branch-tip diff and tests are checked against the keeper implementation.
- **`work/gui-release-mit-wpf-scope`: 16 ahead / 282 behind; 8 changed files.** The filtered diff contains package workflow and WPF README changes but no UML/SVG/diagram path. Keep its release/license/WPF-scope disposition separate from the UML inventory.
- **`work/gui-verify-xml-docs-in-packages`: 4 ahead / 281 behind; 3 changed files, with its workflow change already compared against the stronger current package assertions.** No UML/SVG path appears in this comparison.
- **`work/gui-svg-source-uml-generator`: 227 ahead / 67 behind; 86 changed files.** The content-level diff confirms unique implementation, not only generated docs: a 403-line source-derived-diagrams workflow, solution-derived workflow, source generator and project model builders, renderer-neutral UML model, SVG diagram renderers, model/renderer tests, validation scripts, generated per-source-file documentation, and visual assets. Disposition remains **PRESERVE / ACTIVE**. The latest observed PR head is `97554095656f2ef0ca94cbc4ada73848f3a953f8`; no check runs were returned for that exact head, so prior successful branch workflow runs do not establish current-head green CI. Require a new exact-head run plus deterministic regeneration and provenance/source-truth review.

This sweep does not mark the audit clear. No branch deletion, PR merge/closure, or publication was performed.

## Follow-up inventory refresh — 2026-10-21 20:22Z

Fresh branch inventory still contains 17 branches (the two keepers plus 15 non-keepers); open PR inventory still includes #19, #18, #15, #11, and #8. No destructive actions were taken.

### UML preservation cross-check

- `work/gui-svg-source-uml-generator` now compares as **227 ahead / 68 behind** `development` at base `44c700610182794f8e7f3f9f68a8bb46ace31c63`. Its 86-file diff includes both source implementation (`samples/SourceUmlGenerator`, `src/Core/UmlDocumentationModel.cs`, SVG UML renderers), both source/solution UML workflows, tests, scripts, and generated per-file UML pages. Preserve the entire workstream pending selective reconciliation and exact-head validation.
- `work/gui-uml-per-file-documentation` remains **1 ahead / 214 behind** in the latest stored comparison; the visible requirement is present in development, but its full current tip diff and live PR/reference status still need confirmation before the branch is considered redundant.
- `work/svg-guide-current-contract` compares as **112 ahead / 535 behind**, with 59 changed files in the latest result. The unique `src/SVG/SvgGuiRenderer.cs`, `src/SVG/SvgRenderOptions.cs`, `tests/SVG.Tests/SvgGuiRendererTests.cs`, SVG guide, and `docs/examples/living-energy-vortex.svg` are real implementation/documentation candidates, not a docs-only snapshot. Hold and reconcile renderer behavior/tests before disposition. The diff is broad (Core/Blazor/WPF, workflows, docs, and tests); no wholesale merge.

### CI evidence caveat

Commit-status lookup for policy commit `93db853cf5b3e1c35f30ba975b3845145bcfaaa5` returned no status entries. The workflow-run lookup is explicitly limited to pull-request-triggered runs and also returned no runs for that commit; this is not proof that all GitHub Actions workflows are absent or successful. Do not label the latest audit commit CI-green without exact-SHA evidence.



## Legacy hub / nested-experience branch content review — 2026-10-21 20:23 UTC

The full branch comparisons show these are not safe-to-delete branches merely because they are far behind:

- **`feature/integrated-fsm-cos-hub`: 76 ahead / 535 behind, 47 changed files.** It contains a complete alternative/historical WPF builder surface not present under the same paths in current development: `GuiBuilderWPF` with window chrome/options and interaction bus, plus `Controls` (enum, numeric, pivot-grid, tree-view), `Diagnostics/SovereignLogViewerGuiBuilderWPF`, `Dialogs/ColorForgePickerWPF` and `CrudGuiBuilderWPF`, `Factory/SovereignUIFactory`, `Layouts` (multi-panel/tabbed), and `Reflection/ReflectiveGuiBuilderWPF`. Current development does have a WPF implementation, but under a different, smaller file set; path differences alone do not prove semantic equivalence. Disposition: **PRESERVE / deliberate scope decision required**. Compare capabilities and tests individually before deciding which are out of scope or should be rebuilt against current contracts. Its package workflow also has coverage/Codecov and a WPF build/pack lane, but it explicitly lacks WPF tests and must not replace the current tested lane.
- **`feature/nested-experiences`: 38 ahead / 535 behind, 16 changed files.** Its `docs/GUI_NESTED_EXPERIENCES.md` is byte-identical to the same document on `feature/integrated-fsm-cos-hub` (blob `30b6355822e6a4262efe92422a24e6929b8db958`). It records recursive experience execution, context inheritance, re-entry/suspension, nested manifestation, outcome/progression boundaries, and a licensed-IP boundary. These are architectural proposals, not evidence that the lifecycle contract is implemented. Current development's `docs/DATUM_AND_INTERACTION_BOUNDARY.md` documents the datum/presentation/interaction ownership boundary, but does not itself implement `Gui.Hub` or recursive experience execution. Disposition: **PRESERVE THE UNIQUE CONCEPTUAL CONTRACT FOR REVIEW; HOLD branch disposition** until each required capability is mapped to current architecture or explicitly deferred.
- **`architecture/datum-interaction-boundary`: 62 ahead / 535 behind, 50 changed files.** Its `docs/DATUM_AND_INTERACTION_BOUNDARY.md` is materially different from current development's version. The branch emphasizes datum ↔ human, presentation vs rendering, and platform manifestation; current development adds careful qualification that the note is an ownership principle rather than an implementation claim, separates physical input from semantic interaction, and explicitly avoids creating an input package or ProtocolAi dependency. Disposition: **RECONCILE IDEAS, DO NOT OVERWRITE CURRENT DOC**; preserve the stronger implementation/non-implementation and dependency-boundary caveats while harvesting any missing useful examples.
- These three branches share broad historical source changes across Core/Blazor/WPF and package workflows. They are not three independent feature branches whose commit counts can be added together; the content overlap requires semantic review. No merge or deletion recommendation is made from ahead/behind counts alone.

### CI confirmation

- [Run #39669555915](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39669555915) completed successfully on `44c700610182794f8e7f3f9f68a8bb46ace31c63`: `test-and-package` passed, `test-wpf` passed, and `publish_nuget` was skipped.

No branch deletion, PR merge/closure, or package publication was performed.

## Follow-up implementation review — 2026-10-21 20:23Z

### Source-derived UML branch: actionable workflow review

- The generator branch's workflow `source-derived-diagrams.yml` uses mutable action tags (`actions/checkout@v4`, `actions/setup-dotnet@v4`). This conflicts with the repository-wide hardening target to pin GitHub Actions to immutable commit SHAs. Pinning and reviewing the action versions is a required reconciliation task, not a reason to discard the UML generator.
- The workflow has meaningful verification beyond diagram rendering: it builds syntax-only and project-semantic models, checks exact-build reflection identity rejection, validates schema/provenance and deterministic type inventories, and requires one generated Markdown page and SVG per discovered type. Review this gate and its generated artifacts on the exact PR head before accepting it.
- Exact source review confirms the generator's docs describe schema v1 as a **staging interchange contract**, not a supported package API; syntax-only mode does not claim semantic relationships. Preserve those boundaries when porting or revising docs.

### SVG renderer branch: concrete unique implementation

- `work/svg-guide-current-contract` adds `SvgRenderOptions` (accessible title/description, dimensions, optional background, positive-dimension validation, constrained color validation) and a deterministic `SvgGuiRenderer` that serializes the current `GuiNode` semantic tree, encodes text/attributes, emits accessibility metadata, and rejects tree depth beyond 64. Tests exercise escaping, nested ordering, container layout, and additional renderer behavior.
- These files are absent from `development` at the latest comparison and are not safe to label redundant. However, the branch is 112 ahead / 537 behind and changes 56 files across core, Blazor, WPF, SVG, package workflow, tests, and docs. Preserve as a selective port/rebuild workstream; do not merge wholesale or copy isolated files without their dependencies and tests.
- The source-derived UML branch is a separate workstream. Keep UML model identity/provenance and SVG runtime-renderer behavior distinct in both documentation and reconciliation.

### Current audit guardrail

Neither workstream is cleared for branch deletion. No branches or PRs were modified. The latest audit commit's status remains unverified; absence of reported status entries is not a passing result.

## Architecture branch content harvested without importing legacy implementation — 2026-10-21

The unique explanatory material in `architecture/datum-interaction-boundary` has now been reconciled into current `docs/DATUM_AND_INTERACTION_BOUNDARY.md` (commit `3aa9e4398463aebd9e6a79585ad3c3106cca13d0`). Preserved examples include the rendered-horse datum/manifestation distinction; semantic presentation → platform manifestation → rendering; physical device observations → semantic interaction → domain request; the intersection/bridge rule for common Core capabilities; and the distinction between protocol identity and execution authority. The added section explicitly labels these as explanatory models, freezes no proposed package name/API, and states that no ProtocolAi dependency is introduced into GUI.Core.

This is a **content harvest, not a branch merge**. The branch's existing safety qualifications remain authoritative: no new physical-input package is created by this note, stable identities do not grant execution authority, and implementation claims must be verified against current source. The remaining historical source/test changes on `architecture/datum-interaction-boundary` are still subject to separate reconciliation; the branch is not cleared for deletion solely because the examples were preserved.

The latest successful audit-documentation run before this harvest was [#39669778202](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39669778202), which passed on commit `1b108174d1fc1dce2ec3739ef14104b58a0f9534`. That success does not cover this new documentation commit. A fresh exact-head workflow must complete before claiming the current head is green.



## SVG depth guard and package-scope branch cross-check — 2026-10-21 20:25 UTC

- **`work/svg-tree-depth-guard`: implementation/test behavior is present in current development.** Both versions set a maximum tree depth of 256, use iterative prevalidation before recursive serialization, reject duplicate node IDs, and have tests for the exact depth limit and over-limit trees. Current development additionally has `aria-labelledby` references to generated title/description IDs, with tests covering both title-only and title-plus-description cases and duplicate IDs in coordinate overlays. This is strong evidence the branch's depth-guard change is superseded, but its full three-file diff still includes `docs/SVG_ADAPTER.md`; do a final doc diff/reference check before calling it owner-delete-ready.
- **`work/gui-verify-xml-docs-in-packages`: candidate superseded by current package workflow.** Its metadata gate checks MIT license, README, target framework and XML documentation. Current `development` has the same category of gate and labels it “Verify all package license, README, target framework, and XML documentation metadata,” alongside additional current gallery/sample and package-consumer verification. The branch changes only workflow and release-queue/readiness prose; the full docs diffs and branch references still need a final owner-facing disposition before deletion.
- **`work/gui-release-mit-wpf-scope`: LICENSE is already reconciled.** Its `LICENSE` blob SHA is identical on the branch and `development` (`65f61d17ccdee9f87d75ea2f2f26ad7b1fd34ee4`). The branch workflow's MIT metadata check is present in current development, with the current workflow carrying more checks than this older branch. The remaining scope is a full review of its eight-file diff, particularly its WPF README/project metadata and stale release queue/readiness wording; do not treat the ahead count as unique value.
- **`work/gui-uml-per-file-documentation`: requirement retained, branch not yet cleared.** The branch's only changed file remains `docs/DATA_CARDS_AND_UML.md` (+19 lines); the requirement is present in current development's broader document. Since this is UML-related, preserve the branch until the exact added requirement text is cross-checked and branch references are resolved, regardless of low ahead count.
- **Publishing guard check:** `development` still has `if: ${{ github.event_name == 'workflow_dispatch' && inputs.publish == true && false }}` on `publish_nuget`. This is an intentional hard stop. Neither publish-gate branch is authorized for adoption.

The above are content comparisons, not branch deletions or PR decisions. The current development head's latest workflow runs were still in progress when checked: #39670075560, #39670037267, and #39669984320. Do not infer a green exact-head result from earlier successful runs.

## Branch inventory and UML example refresh — 2026-10-21 20:26Z

- The live branch inventory now returns **18 branches**: `master`, `development`, and 16 non-keepers. This supersedes earlier 17-branch counts. No branch was deleted.
- The current development history includes commit `d50cdc2e0fcf98781d7b3b9792ece04eef1aa7f9` (`docs(uml): add canonical DataCard type page with UML`). This is a useful manually authored example: it identifies the authoritative source file, assembly, member visibility, responsibilities, boundaries, and verification status, while explicitly stating that one page does not prove complete per-type coverage. Preserve this as the model for the manual per-file documentation stage.
- The source-derived UML generator branch comparison has moved to **227 ahead / 74 behind** current `development` (base `ee9b2534d16e6e06aa472961e9a35fbd44c684a2`). The changed-file list includes both UML workflows, the generator and model/renderers, tests, validation scripts, generated per-source pages, and a large set of unrelated/stale documentation and package/workflow edits. Reconcile by focused file/behavior groups; no wholesale merge.
- A newly visible branch, `feat/physics-seeded-pointset-svg`, is **4 ahead / 0 behind** development and adds a seeded 2D point-set demonstration, its project, README, and recipe. It is directly relevant to living SVG demonstrations. Keep it as a preserve/reconcile candidate; it is not an obsolete divergent branch.
- The latest open-PR search still surfaces #8, #11, #15, #18, and #19. Both publish-gate PRs remain unauthorized; the `&& false` hard stop stays intact. The workflow-run connector does not provide evidence of general push-triggered checks for the generator base commit, so exact-head CI remains unverified rather than green.

### Next concrete reconciliation order

1. Treat `docs/api/Core/DataCard.md` plus its UML SVG as the manual quality bar; keep its claims scoped to source-verifiable facts.
2. Extract the UML model/generator implementation, renderers, tests, and workflows from `work/gui-svg-source-uml-generator` as a coherent candidate set. Check dependencies and pin actions to immutable SHAs; verify determinism and generated-artifact drift before adoption.
3. Audit generated pages against actual source and label syntax-only relationships honestly; do not import unrelated package/release/readiness snapshots from the divergent branch.
4. Reconcile the seeded point-set demo and `SvgGuiRenderer`/`SvgRenderOptions` work independently, preserving runnable sample recipes and tests.
5. Only after each branch's unique payload and PR/reference disposition is recorded may the user manually delete any surplus branch.



## Seeded Physics point-set demonstration discovered — 2026-10-21 20:27 UTC

A fresh live branch inventory surfaced `feat/physics-seeded-pointset-svg`, which was absent from the earlier 17-branch snapshot. Current inventory is **18 branches total**: `master`, `development`, and 16 non-keepers.

- Branch comparison now reports **6 ahead / 1 behind, 6 changed files** (the earlier 4-ahead/0-behind snapshot is stale because `development` advanced). Unique payload:
  - `samples/demonstrations/SeededPointSet2d/Program.cs`
  - `samples/demonstrations/SeededPointSet2d/README.md`
  - `samples/demonstrations/SeededPointSet2d/SeededPointSet2d.csproj`
  - `samples/demonstrations/recipes/05-seeded-point-set.md`
  - updates to `samples/demonstrations/GALLERY.md` and `samples/demonstrations/index.html`.
- The consumer parses the versioned Physics run record, validates model/schema identity, finite half-open bounds, point count, stable point indices, and each coordinate; it then verifies each emitted SVG marker ID and mapped coordinate against the input. This is a strong living-documentation example of Physics owning data generation and GUI.SVG owning manifestation.
- The sample project references the existing GUI Core and SVG projects. However, I have not run this project locally or found exact-head CI evidence that compiles and executes it. The recipe's claim that CI also runs the Physics exporter is not supported by this branch's six-file diff (no workflow change); that sentence must be removed or backed by a separately verified workflow before the recipe is promoted.
- The sample has not yet been copied into `development`. **Disposition: PRESERVE / harvest after correcting the unsupported CI claim and adding a reproducible validation path. Do not delete this branch.** Its gallery/index additions should be applied to current development, not wholesale-replaced from this one-commit-behind branch.

## Exact-head CI confirmation — 2026-10-21 20:27 UTC

- [Run #39670188386](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39670188386) completed successfully on exact development SHA `ee9b2534d16e6e06aa472961e9a35fbd44c684a2`. This run is green for the existing `test-and-package` and `test-wpf` jobs; NuGet publishing remains skipped by the hard stop.
- This result covers the audit commit at that SHA, not any later commit created by this new audit update.

No branch deletion, PR merge/closure, or package publication was performed.

## Seeded Physics point-set branch — live PR reconciliation correction — 2026-10-21 20:29 UTC

The previous subsection above was written from an earlier snapshot and is now superseded where noted. Live PR #20 is open (not merged): [feat: render Physics seeded point-set records in GUI.SVG](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/20), head `f2f9de5b323ebc9adafd19bf9e7315c341f4f789`, base development `725990b8a009ca3a505c59a9f0c75db7d200a4ba`. Current comparison is **7 ahead / 2 behind, 7 changed files**. The prior 6-ahead/1-behind, six-file count and “no workflow change” statement are stale.

The current PR adds the package workflow steps to check out Physics at immutable source commit `879a25e19879eaee782d9588f06ba76420b23215`, run its exporter, feed the generated record to the GUI.SVG consumer, and upload the record plus SVG as an artifact. This directly addresses the earlier concern that CI did not exercise the cross-repository data path. The recipe claim that CI runs the exporter is now supported by the PR diff, but remains **unverified until the exact PR head workflow succeeds**.

Review items before adoption:
- The new workflow steps currently use `actions/checkout@v4` and `actions/upload-artifact@v7` tags rather than immutable action commit SHAs. Reconcile with the repository's action-pinning hardening policy before merging.
- The source checkout itself is pinned to the immutable Physics commit above; do not describe the source revision as floating.
- The consumer validates record identity/schema, bounds, finite coordinates, count, stable indices, and each SVG marker's ID and mapped coordinates. Confirm malformed/missing properties, empty point sets, and maximum supported point count are handled by executable tests, not documentation alone.
- The PR is seven commits ahead and two behind development; review the two behind commits for workflow conflicts before any merge decision. No merge is authorized here.

Exact-head checks at the time of this note: the latest development workflow #39670539036 and PR-head workflow #39670565537 were still in progress; their `test-wpf` jobs had passed while `test-and-package` remained in progress. Neither run is yet evidence of a completed green pipeline.

Current live inventory remains **18 branches total**: `master`, `development`, and 16 non-keepers. The new branch is **PRESERVE / ACTIVE PR #20**, not a deletion candidate. Keep the prior note's caution about harvesting only after exact-head validation, but do not repeat its now-corrected claim that the branch contains no workflow change.



## Source-derived UML generator: content and enforcement review — 2026-10-21 20:29 UTC

Branch `work/gui-svg-source-uml-generator` remains a **preserve/reconcile** branch, not a deletion candidate. The current comparison is 227 ahead / 76 behind with 86 changed files. The semantic work is substantial and should not be discarded; the implementation still needs a safe integration plan rather than wholesale merge.

### Positive, directly inspected evidence

- `src/Core/UmlDocumentationModel.cs` defines a versioned neutral model with provenance, stable identities, source spans/declarations, members, typed relationships, and resolution states. Tests explicitly cover JSON round-trip, unsupported schema versions, unknown properties, invalid source spans, and resolved edges pointing at missing types.
- The generator workflow runs both syntax-only and project-semantic generation. It checks provenance/revision pinning, exact reflection assembly identity (including a negative test using the wrong assembly), deterministic inventory ordering, and one Markdown/SVG page per modeled type.
- `docs/uml/README.md` claims a baseline of 67 types across 37 production files and correctly says the generated per-type Markdown/SVG are CI artifacts rather than committed repository pages. That is transparent, but it means the user-requested per-`.cs` documentation is not yet a durable, browsable set of committed pages in `development`; the manual authored source-file index and CI artifact output must be reconciled into the intended long-term contract.
- Workflow uses `actions/checkout@v4` and `actions/setup-dotnet@v4`, not immutable action commit SHAs. This fails the repository's stated CI-hardening goal; pin reviewed actions to full SHAs and retain readable version comments before adopting this workflow.
- The workflow produces artifacts under `artifacts/` and currently does not show a repository-diff/regeneration gate for committed UML pages. Decide explicitly whether generated pages are committed or CI artifacts; do not silently claim both.
- Exact-head PR #18 remains open/draft. Its head SHA must receive a fresh successful workflow run after any reconciliation; prior historical run IDs are not proof for the current head.

### Integration gate for PR #18

1. Preserve the renderer-neutral model and tests, source-span/provenance contract, typed relationship semantics, and source-derived SVG renderers.
2. Pin all third-party GitHub Actions by immutable SHA.
3. Make source identity and source revision explicit; keep generated output deterministic and fail closed on invalid provenance/reflection identity.
4. Decide whether per-file companion Markdown and focused class SVGs are committed to `docs/UML` or generated as a clearly linked CI artifact; user expectation is browsable per-`.cs` documentation, not artifact-only output.
5. Ensure generator coverage matches current production files and tests, and avoid treating generated source structure as human-authored use cases.
6. Run exact-head CI plus regeneration-twice/diff-stability verification; inspect SVG output for meaningful readable diagrams, not just existence.

No source-generator code was merged, and PR #18 remains open.

## Branch inventory / workflow status refresh — 2026-10-21 20:29 UTC

- Live branch inventory remains **18 branches** (2 keepers + 16 non-keepers).
- Open PRs remain six: the Physics seeded-point-set demonstration; PR #19 and #15 publish-gate proposals; PR #18 source-derived UML; PR #11 SVG guide/current contract; PR #8 datum/interaction boundary.
- Latest audit-commit workflow [run #39670539036](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39670539036) is **still in progress** at `Pack WPF`; `test-wpf` has passed. Do not call this commit green yet.
- Earlier [run #39670436736](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39670436736) succeeded on SHA `725990b8a009ca3a505c59a9f0c75db7d200a4ba`, but it predates the current audit commit.
- The NuGet publish hard stop remains in `development`; no package was published.

No branch deletion, PR merge/closure, or package publication was performed.

## Correction and follow-through: Physics seeded point-set PR — 2026-10-21 20:29Z

- Current branch inventory: **18 branches** (2 keepers + 16 non-keepers). PR #20 is open for `feat/physics-seeded-pointset-svg`; the branch now compares **8 ahead / 2 behind** `development`, with seven changed files. This supersedes the earlier 4-ahead/0-behind comparison.
- Important correction to the prior note: this branch **does** change `.github/workflows/package.yml`. It checks out Physics at immutable commit `eea463a5034682b8eb149bd73013ac7dd590eb83`, runs `SeededPointSet2dExport`, runs the GUI consumer against that exact run record, and uploads both the JSON record and generated SVG. Therefore the end-to-end CI claim is backed by the branch workflow source, though it is not yet proven by a successful exact-head workflow run.
- The sample's input checks are concrete: model/schema identity, half-open bounds, finite coordinates, count cap and consistency, stable indices, and exact comparison of emitted marker IDs and mapped coordinates to the input record. The recipe carefully avoids claiming physical simulation or a probability distribution.
- `development`'s `samples/demonstrations/GALLERY.md` does not yet contain Recipe 05. Harvest the sample, recipe, gallery entry, HTML index link, and package workflow changes together; review the pinned Physics SHA and action references, retain the publishing hard stop, then validate the PR's actual workflow run before merge consideration.
- The PR's workflow contains mutable major-version action tags (`actions/checkout@v4`, `actions/setup-dotnet@v4`, `actions/upload-artifact@v7`, `actions/download-artifact@v8`, `NuGet/login@v1`). Pin all actions to reviewed immutable SHAs as part of hardening; do not let this unrelated action-pinning work obscure the sample's data-provenance proof.
- This section supersedes the earlier stale audit sentence claiming the branch diff had no workflow change. The prior 4-ahead comparison was a stale baseline; use the current comparison above.

Disposition: **PRESERVE / RECONCILE; do not delete branch or merge PR until exact-head CI and workflow hardening are reviewed.**

## PR #20 exact-head CI failure observed — 2026-10-21 20:31 UTC

The initial end-to-end run failed twice on consecutive PR heads:
- [Run #39670565537](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39670565537), head `f2f9de5b323ebc9adafd19bf9e7315c341f4f789`: Physics exporter succeeded and emitted 128 points, but GUI consumer failed at `Program.cs:116` with `SVG marker 0 does not match the corresponding Physics record point.`
- [Run #39670716083](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39670716083), head `f3e23a1687938b4cc81a550a59dd2c314dea523c`: same failure at the same assertion after exporter success. WPF tests passed on both runs. The current PR head has advanced again to `39bee41a690f63d0e2f656fdd99445976ea07594`; its run [#39670789022](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39670789022) was still in progress at this checkpoint, with WPF passed and `test-and-package` in progress.

This is a real validation failure, not a flaky infrastructure issue proven by available evidence. The generated artifact upload and package jobs are skipped after the consumer exits nonzero. Do not describe the data-to-SVG proof as working until a later exact-head run passes. Investigate the emitted SVG's actual marker attributes and numeric normalization rather than weakening/removing the per-marker verification. Keep PR #20 open and preserve its branch; do not merge or delete it.

## PR #20 final CI and merge correction — 2026-10-21 20:33 UTC

Correction to the 20:31 failure note above: the two failures were real on heads `f2f9de5` and `f3e23a1`, but the later exact PR head `39bee41a690f63d0e2f656fdd99445976ea07594` **passed both required jobs** in [run #39670789022](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39670789022): `test-and-package` and `test-wpf` succeeded; `publish_nuget` was skipped. The final fix changed the SVG numeric formatter to use `0.###`, matching the renderer's emitted coordinate precision; the per-marker identity/coordinate assertions remain enabled. Thus the exact-head data-to-SVG proof is validated, rather than merely documented.

PR #20 was subsequently merged by repository owner TrentBest at 2026-10-21 20:32:53Z, merge commit `b584b8a0e0c9bbf068c0155dc8dfadb7bd4f56c3`. The merge commit's post-merge development workflow [#39671256858](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39671256858) was still in progress at this checkpoint (both `test-and-package` and `test-wpf` running). Therefore PR-head CI is green, but **post-merge development CI is not yet confirmed green**. Keep the branch in the live inventory until the final Sith inventory/manual deletion checkpoint; do not delete it automatically.

Action pinning remains a separate hardening item: the new workflow steps use major-version tags for checkout/upload actions, while the Physics source itself is pinned to a specific commit. This does not invalidate the successful functional proof, but it remains an outstanding workflow-hardening concern.



## PR #20 post-merge verification — 2026-10-21 20:35 UTC

- Confirmed post-merge development workflow [#39671256858](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39671256858): `test-and-package` **success**, `test-wpf` **success**, and `publish_nuget` **skipped**.
- The end-to-end seeded-point-set chain completed in that run: pinned Physics checkout, real Physics exporter, GUI.SVG consumer validation, and artifact upload all succeeded. The renderer's per-marker identity/coordinate checks remain enabled.
- The merged PR is [#20](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/20), merge commit `b584b8a0e0c9bbf068c0155dc8dfadb7bd4f56c3). A combined commit status query returned no individual status records; the workflow job results above are the actual evidence for this run.
- The old feature branch still appears in the branch inventory and currently compares as 9 commits ahead / 8 behind `development`, with seven changed paths in the compare result. Because PR #20 is merged, this stale/diverged branch is now a cleanup candidate, **not an automatic deletion**: before manual deletion, verify that no post-merge branch-only commits contain work not represented by the merged PR.
- Remaining workflow-hardening item: pin mutable GitHub Action tags to immutable SHAs in the keeper workflow. The successful functional run does not resolve that hardening task.
- Sith remains **NOT CLEARED**. UML staging readiness remains 72%; no branch was deleted, no other PR was merged/closed, and package publication remains unauthorized.

## Sith checkpoint — post-merge Physics sample and current keeper status — 2026-10-21 20:37 UTC

- Latest development head is `cb373cbdbebeff68a294a867ddf168fdf738983c` (commit: `docs: record seeded point-set post-merge verification`). The exact-head Package GUI workflow [#39671660534](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39671660534) completed **success**. The immediately preceding post-merge run [#39671436021](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39671436021) also completed **success**; the functional seeded-point-set workflow [#39671256858](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39671256858) completed **success**. These runs validate the merged sample and subsequent documentation heads; NuGet publishing remains skipped.
- PR #20 is merged, but `feat/physics-seeded-pointset-svg` remains present. Current compare is **9 ahead / 9 behind**, with the same seven sample/workflow paths as the PR. Its branch tip is divergent, so do not infer it is safe to delete from merged status alone. The merged sample is now represented on `development`; a final branch-only commit review is still required before it can be listed as manually deletable.
- Live branch count remains **18**: `master`, `development`, and 16 non-keepers. The open PR inventory is now **five**, not six: #19, #18, #15, #11, and #8. PR #20 is closed by merge. No branch was deleted and no other PR was merged or closed.
- Source-derived UML readiness remains **72%**. PR #18 remains open/draft and is not merge-ready: its workflows still use mutable action tags (`actions/checkout@v4`, `actions/setup-dotnet@v4`, `actions/upload-artifact@v7`); generated per-type pages remain CI artifacts rather than committed browsable `docs/UML` pages; and the current PR head still needs fresh exact-head CI. The exact-head check must follow the latest PR update, not reuse historical green runs.
- The current keeper workflow retains the explicit NuGet publication hard stop. Release readiness does not authorize publication. Sith cleanup remains **NOT CLEARED**; continue evidence-based reconciliation and leave branch deletion to the maintainer.


## Fresh branch reconciliation checkpoint — 2026-10-21 20:37 UTC

- Re-enumerated the branch list: **18 branches** remain, comprising the required `master` and `development` keepers plus 16 non-keepers. No branch was deleted.
- Refreshed comparisons against the current `development` tip `cb373cbdbebeff68a294a867ddf168fdf738983c`.
- **UML work remains protected.** `work/gui-svg-source-uml-generator` is 227 commits ahead / 84 behind and contains the source analyzer/model/renderers, semantic relationship tests, generated per-type UML pages, inventory validation, and source-derived diagram workflows. This is a substantial divergent workstream; do not wholesale merge or discard it. PR [#18](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/18) remains in the open-PR search results. Qualitative UML staging readiness remains 72%.
- `work/gui-uml-per-file-documentation` is 1 ahead / 230 behind and its only changed path is `docs/DATA_CARDS_AND_UML.md` (+19 lines). That requirements text must be preserved in `development` or explicitly confirmed identical before the branch is considered redundant; the branch itself is not a substitute for the larger UML generator work.
- `work/svg-guide-current-contract` is 112 ahead / 550 behind. Its diff includes `src/SVG/SvgRenderOptions.cs`, `src/SVG/SvgGuiRenderer.cs`, renderer tests, guide/docs, and unrelated package/legacy API changes. Keep selectively reconciling the renderer and tests; do not wholesale merge or classify the branch as disposable by divergence alone. PR [#11](https://github.com/TrentBest/TheSingularityWorkshop.GUI/pull/11) remains in the open-PR search results.
- Stronger cleanup candidates, still **not deletion authorizations**: `work/gui-wpf-tests` is 0 ahead / 298 behind with no changed paths; `work/svg-app-fsm-cos-boundary` is 5 ahead / 482 behind and changes only README plus `docs/SVG_APP_CONCEPT.md`; `work/svg-tree-depth-guard` is 5 ahead / 482 behind and changes the SVG renderer/tests and an older adapter guide; `work/webpage-event-builder-compatibility` is 3 ahead / 545 behind and changes the Blazor builder plus event tests. Verify current PR/reference state and content equivalence before final disposition.
- Legacy hub/nested-experience branches remain an explicit product decision, not routine cleanup: `feature/integrated-fsm-cos-hub` is 76 ahead / 551 behind and `feature/nested-experiences` is 38 ahead / 551 behind; their diffs include old GUI hub/reflection APIs and specialized WPF builders. Do not claim these APIs were merged into current development.
- Open-PR search still returns #8 (datum/interaction boundary), #11 (SVG guide), #15 and #19 (publish-gate history), and #18 (source-derived UML). Preserve these workstreams pending explicit PR disposition. The deliberate NuGet publish guard remains unchanged; no dispatch with publication enabled.
- Readiness/status: branch cleanup **NOT CLEARED**; UML staging readiness **72%**. The latest post-merge functional run for PR #20 passed package/tests and WPF, with NuGet publish skipped. Audit commit CI remains unverified where no exact-head run/status evidence is available. No branches were deleted, no PRs were merged/closed by this audit, and no package was published.

## SVG guide branch: implementation drift confirmed — 2026-10-21 20:38 UTC

A fresh comparison of `work/svg-guide-current-contract` against `development` shows **112 ahead / 551 behind**, diverged. The diff is broad (57 changed paths in the compare response), not a documentation-only branch: it contains an older GUI Core/Blazor/WPF surface, an older SVG renderer, tests, package workflow changes, and documentation/assets. Do not merge or delete it wholesale.

- Its `docs/SVG_GUIDE.md` describes the older `SvgGuiRenderer` contract and is materially stale against current `development`'s guide and implementation. The branch renderer uses a recursive depth limit of 64 and string concatenation; current development uses `XElement`, iterative prevalidation with a depth limit of 256, duplicate-ID validation, and explicit `svg-` node kinds. Current tests cover SVG primitives, transforms, gradients/clips/masks, coordinate mapping, and the new scene contract. These are different API/architecture generations, not interchangeable documentation snapshots.
- The older guide's useful evidence discipline remains relevant (distinguish serialized SVG from a running app/GPU path; label hand-authored concept art honestly; state that visual controls are not interactive). The current `docs/SVG_GUIDE.md` already retains this discipline and explicitly documents the current renderer's constraints and untrusted-URL boundary.
- Branch-only implementation files (including older Blazor/WPF builders, hub/reflection surfaces, and SVG tests) still require a deliberate capability-by-capability disposition; their existence is not evidence those APIs were ported. Conversely, their age/divergence does not justify preserving the entire branch as a keeper.

Disposition: **HOLD / HARVEST AND VERIFY**. Current guide appears to supersede the branch's guide, but do not call the whole branch manually deletable until the remaining unique implementation/docs/assets and workflow changes are reconciled. Current keeper package workflow run [#39671906125](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39671906125) was still running at this checkpoint; it is not yet exact-head green evidence for the latest audit update.


## Content-equivalence spot checks — 2026-10-21 20:38 UTC

Additional file-level checks refine the candidate list:

- `work/gui-uml-per-file-documentation`: `docs/DATA_CARDS_AND_UML.md` is **not byte-identical** to development (branch blob `9974b59d70679ac1d9174539df539ba609c4a41d`, development blob `e8a5164eba4e2d121f505fd8fc9f79272392579e`; 16,202 versus 30,500 characters). Do not discard until the older file's unique requirements are compared and any still-relevant material is preserved. This confirms that the one-commit branch cannot be called redundant from commit count alone.
- `work/svg-tree-depth-guard`: `src/SVG/SvgGuiRenderer.cs` and `tests/SVG.Tests/SvgGuiRendererTests.cs` are **not byte-identical** to development (renderer blobs `b9bca133c0541f838e7d47a61163df4d17a41155` vs `88262776417a8d4138556e256c85386cbafba682`; test blobs `ea8b9621b538513182bae5951ad0836023c9604c` vs `5d8a323c61e8eaef4835861d7f2ae305bb462047`). Keep it on the reconciliation list until the depth guard and any branch-only test intent are reviewed, despite the small ahead count.
- `work/svg-app-fsm-cos-boundary`: `docs/SVG_APP_CONCEPT.md` is byte-identical on both refs (blob `f98b408b2fba433017892c59182118ab6aaf47f3`). This file itself needs no further harvesting; still verify the branch's README hunk and PR/reference state before final branch disposition.
- `work/webpage-event-builder-compatibility`: `tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs` is byte-identical on both refs (blob `be6c179c751d8f817293f4f445584df23675afd5`). The builder source still differs and must be compared before disposition.
- Audit commit `fe4ec10a7d739d9432d4aba5894c521be25754e7` currently has no individual status records and no PR-triggered workflow runs returned by the connector; CI is **unverified**, not green.
- Sith remains **NOT CLEARED**. UML staging readiness remains **72%**. No branch deletion, PR closure/merge, or package publication was performed.

## Follow-up checkpoint — 2026-10-21 20:38 UTC

### Post-merge Physics proof is green

The merge commit `b584b8a0e0c9bbf068c0155dc8dfadb7bd4f56c3` has completed its post-merge workflow successfully: [run #39671256858](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39671256858) reports `test-and-package` and `test-wpf` succeeded; `publish_nuget` was skipped. The exact PR-head run #39670789022 also passed both required jobs. The seeded point-set feature is therefore validated on both PR head and merge commit. Its branch `feat/physics-seeded-pointset-svg` remains in the live branch inventory; this ledger does not delete it.

### WebPage event-builder compatibility branch — content comparison completed

Live comparison of `work/webpage-event-builder-compatibility` against development: **3 commits ahead / 548 behind, 2 changed files**. The test file `tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs` is byte-identical on both refs (blob `be6c179c751d8f817293f4f445584df23675afd5`). The only source-file difference in `src/Blazor/WorkshopGuiBuilder.cs` is XML documentation comments on `WorkshopGui` and `ElementBuilder`; the public methods and implementation body are otherwise identical. This is a **superseded candidate** for source/test content, subject to a final branch/PR/reference check at the manual cleanup checkpoint. No unique runtime behavior or unique test is visible in the current diff.

### Current exact-head CI caution

Development has advanced again to `98de63fc6738e157d0d7e697d809e23363e09d0a` while this audit was running. The latest workflow run visible at the checkpoint, [#39671995083](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39671995083), targets earlier head `f4e58be9181c91077bfb507ae79a937092da752d` and is still in progress; its WPF and test/package jobs were both running. Therefore this is **not yet exact-head CI evidence for `98de63f`**. The most recent completed green development workflow before these newer documentation commits is #39671660534 on `cb373cbdbebeff68a294a867ddf168fdf738983c`. Treat newer-head CI as pending until a workflow for the actual current head completes.

### Source-derived UML PR #18

PR #18 remains open and draft, with 86 changed files and head `97554095656f2ef0ca94cbc4ada73848f3a953f8`. The GitHub check-runs endpoint currently returns no individual check records for that exact head, so its CI status is **unverified**, not green. Preserve/reconcile the branch; do not infer readiness from the size of the implementation or from unrelated development CI.

No branch was deleted, no PR was closed or merged by this audit pass, `master` remains untouched, and NuGet publishing remains gated.

