# Autonomous Design (AD) badge — GUI presentation contract

**Status:** presentation proposal and deterministic visual fixtures. This document does not define the runtime status schema.

## Purpose

The AD badge lets a repository present its current participation state in the Autonomous Design pipeline. GUI owns the visual assets and presentation. The badge is a projection of authoritative ecosystem data; it is not itself an execution controller or CI verdict.

## Two independent dimensions

### 1. AD lifecycle — Foundry authoritative

| Semantic state | Display label | Meaning |
|---|---|---|
| `OFF` | OFF | Explicitly disabled. |
| `WORKING` | WORKING | Autonomous work is actively progressing, including CI, diagnosis, repair, and revalidation. |
| `AWAITING_HUMAN_APPROVAL` | AWAITING HUMAN REVIEW | A healthy handoff has reached a human decision/approval boundary. |
| Unknown / stale / unavailable / contradictory | STATUS UNAVAILABLE | Current lifecycle cannot be established. Never map this to OFF. |

### 2. Required dependency readiness — CiAi graph/evidence authoritative

| Semantic state | Display label | Meaning |
|---|---|---|
| READY | READY | Required dependency evidence satisfies its gate. |
| BUILDING / WAITING | BUILDING / WAITING | A required upstream dependency is building or awaiting an update. |
| FAILED | FAILED | A required dependency has failed its gate. |
| STALE / UNKNOWN / UNAVAILABLE | UNKNOWN / STALE | Evidence cannot establish readiness. |

Optional dependencies must be explicitly marked **OPTIONAL / NON-GATING** in a detailed panel or graph. They must not block the required-dependency rollup. Each direct dependency entry should show its repository name, required/optional role, current status, and a link to its exact evidence. The full dependency graph and sequence belong to CiAi Pages.

## Asset inventory

Lifecycle assets:
- `assets/autonomous-design/off.svg`
- `assets/autonomous-design/working.svg`
- `assets/autonomous-design/review.svg`
- `assets/autonomous-design/unavailable.svg`

Dependency rollup assets:
- `assets/autonomous-design/dependency-ready.svg`
- `assets/autonomous-design/dependency-waiting.svg`
- `assets/autonomous-design/dependency-failed.svg`
- `assets/autonomous-design/dependency-unknown.svg`

Each SVG includes a title and description. The working animation is decorative only and disables under `prefers-reduced-motion: reduce`. Every state remains understandable from text and icon/shape without color or motion.

## Deterministic presentation fixtures

These are visual acceptance cases, not a wire-format or canonical schema:

| Fixture | Lifecycle | Dependency rollup | Expected presentation |
|---|---|---|---|
| A | OFF | READY | OFF remains explicit; dependency dimension remains separately visible. |
| B | WORKING | BUILDING / WAITING | Working gear plus waiting/break-light dependency indicator. |
| C | WORKING | FAILED | Lifecycle remains WORKING while the dependency failure is separately visible. |
| D | AWAITING HUMAN REVIEW | READY | Human-attention/mail motif; healthy handoff, not an error. |
| E | STATUS UNAVAILABLE | UNKNOWN / STALE | Explicit unavailable state; never render as OFF or READY. |
| F | WORKING | FAILED with one optional dependency READY | Required failure is visible; optional edge is labelled non-gating and does not alter the required rollup. |
| G | WORKING | READY with multiple direct dependencies | Show each repository and its individual status in the detailed panel; compact badge shows the rollup. |

## Integration boundary

- Foundry supplies the authoritative AD lifecycle projection and durable human-attention state.
- CiAi supplies dependency graph, required/optional edge semantics, readiness rollup, and exact evidence links.
- GUI maps validated semantic states to these assets and to the embeddable GUI presentation.
- Do not infer lifecycle from GitHub Actions, build results, branch names, badge delivery, or dependency health.
- Do not infer dependency health from the AD lifecycle state.
- Missing, stale, malformed, or contradictory inputs render the unavailable state for the affected dimension. Do not silently coerce them to OFF, READY, or success.
- The final field names, freshness rules, and payload shape must be agreed with Foundry/CiAi before runtime integration. This proposal intentionally does not create a competing schema.

## Distribution and security proposal

1. Keep GUI as the source-controlled owner of versioned SVG assets.
2. For repository READMEs, use a single image link to the CiAi AD detail/graph page. Do not depend on multiple links embedded inside an SVG; GitHub and other renderers may sanitize them.
3. Prefer immutable, version-pinned asset URLs (release/tag or commit SHA) once the cross-repository delivery owner and versioning policy are agreed. Avoid mutable `master` URLs for long-lived consumer references.
4. The badge and public graph should expose only deliberately public repository names, coarse status, timestamps/freshness, and approved evidence links. Never embed tokens, private logs, secrets, internal prompts, or sensitive work descriptions in SVG metadata or public pages.
5. Treat cache freshness as part of the status contract. A cached badge must not appear current indefinitely; the detail page should expose evidence time and stale/unavailable state.

## Explicitly out of scope

- Sith Policy / branch-governance status.
- CI pass/fail as an AD lifecycle signal.
- Defining Foundry's lifecycle schema or CiAi's dependency/evidence schema.
- Badge deployment, package publication, release, or merge.
