<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>
# GUI Release Closeout — Current Engineering Snapshot

**Snapshot date:** 2026-11-26  
**Review branch:** `development`  
**Release decision:** **BLOCKED — no candidate selected, no publication approval.**

This document is the current closeout index for implementation, documentation, evidence, and unresolved release gates. It supersedes the stale 2026-10-22 readiness estimate and historical .NET 8 package evidence. No readiness percentage is stated: the owner-accepted GUI domain-definition gate is the controlling pre-release decision.

## Package and framework boundary

- The maintained development tree has migrated to **.NET 10**.
- The previously approved next package-family scope remains **Core, Blazor, SVG, and WPF**.
- **GUI.Markdown** is a fifth development package candidate only. It is not included in the approved release scope unless the owner explicitly adds it.
- WPF is an initial Windows renderer/builder surface; do not imply full adapter parity.
- Package version/channel selection is intentionally deferred until the GUI domain definition is accepted and the exact candidate is verified.

## Latest verified implementation baseline

The latest recorded product baseline is development commit [`d0e5322`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/d0e5322aaf8a92daea6e244eac0119314e0cb9f0). Its exact workflow evidence:

| Workflow | Run | Result |
|---|---:|---|
| Package GUI | [45752767528](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/45752767528) | Passed; Linux/Core/Blazor/Markdown/SVG/shared-adapter validation, Windows WPF validation, package creation/metadata, clean-consumer verification; NuGet publish skipped |
| CiAi Commit Evidence | [45752767524](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/45752767524) | Passed |
| Source-derived diagrams | [45752767518](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/45752767518) | Passed |
| Solution-derived UML inventory | [45752767512](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/45752767512) | Passed |
| Pages build | [45752767985](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/45752767985) | Build/artifact validation passed; deployment skipped by the manual-dispatch-only safeguard |
| Sith Policy | [45752767510](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/45752767510) | Correctly failed closed on the then-current 27-branch inventory |

The branch inventory has since changed. This historical Sith failure is not the current result. The latest documentation reconciliation head recorded at the previous check was [`cf881a1`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/cf881a101997b369d139e612ac90e1793a79cef3); GitHub returned no combined status or associated workflow runs for that exact head when checked. Therefore its CI is **unverified**, not green. A fresh paginated branch enumeration now returns exactly `development` and `master`; this satisfies the inventory prerequisite but is not evidence that the Sith workflow has passed. Rerun the exact-head gates and obtain explicit workflow results before treating the checkpoint as verified.

## Domain and documentation closeout

The full GUI domain definition remains **OPEN**. It must identify what GUI owns, what it deliberately does not own, current implementation versus specified/proposed/unverified capabilities, the authoritative state/command boundary, lifecycle ownership, adapter-specific support, host/accessibility behavior, and the boundaries with FSM_COS, WebPage/WebApp, Visualize, and Renderer. Owner acceptance is required before selecting a release candidate or package versions.

The UML view taxonomy and member presentation proposal are now consolidated in [UML Views and Data-Card Standard](UML_VIEWS_AND_DATA_CARD_STANDARD.md). The Static bucket is a presentation category, not a language accessibility level; exact source modifiers are retained and declarations appear once. This remains a proposal pending owner acceptance, semantic-model/renderer alignment, fixtures, and tests. The [Data Cards and UML](DATA_CARDS_AND_UML.md), [Per-Type Documentation Standard](CLASS_DOCUMENTATION_STANDARD.md), [UML index](uml/README.md), and [Full GUI Domain Coverage Map](FULL_GUI_DOMAIN_COVERAGE.md) link to the same proposal.

The consumer-facing [semantic-tree-to-SVG use case](USE_CASES/01-SEMANTIC-TREE-TO-SVG.md) and [Evidence and Boundaries theory chapter](THEORY/07-EVIDENCE-AND-BOUNDARIES.md) are already present and indexed. They distinguish source structure, executable tests, consumer artifacts, and architectural claims; none substitutes for another.

## Remaining release gates

1. Owner review and acceptance of the full GUI domain definition.
2. Complete the authored documentation and behavior-boundary work identified in [Next-Release Work Queue](NEXT_RELEASE_WORK_QUEUE.md) and [Release Readiness](RELEASE_READINESS.md).
3. Align the proposed UML presentation semantics with the renderer/model and retain fixture-based tests; do not claim interactive AST editing from static SVG.
4. Re-run all required workflows on the exact post-cleanup `development` head, including Windows-native WPF and archive-only consumer checks.
5. Select the exact candidate and approved package scope/channel only after the domain gate is accepted; inspect exact .NET 10 candidate archives and verify the relevant downstream Blazor/WebPage host path.
6. Keep NuGet publication disabled until the owner explicitly approves the exact inspected artifacts.

## Repository governance and operational boundaries

- The Sith target remains exactly `development` and `master`. A fresh paginated branch enumeration on 2026-11-26 returned exactly those two branches; the continuation page was empty. The three reconciled proposal branches are no longer present. This verifies the branch inventory, not a successful Sith workflow run; exact-head CI/Sith evidence remains outstanding. Never weaken the policy to obtain a green result.
- `master` remains untouched. The latest compare confirms substantial divergence between `development` and `master`; this is not a promotion instruction. Do not merge, rebase, reset, or force-update `master` as part of cleanup. Do not merge, rebase, reset, or force-update `master` as part of cleanup.
- Required status-check/branch-protection configuration remains an administrative gap; the connected GitHub integration cannot write protection settings. Do not claim the checks are enforced.
- Pages deployment remains manual-dispatch-only. Build validation may run on push; deployment must not be dispatched without explicit authorization.
- No NuGet package was published in this closeout.

**Bottom line:** implementation and documentation have advanced, but the GUI domain gate, exact-head CI, branch inventory, release-candidate evidence, and explicit owner approval remain separate gates. A green build is not release authorization.


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
