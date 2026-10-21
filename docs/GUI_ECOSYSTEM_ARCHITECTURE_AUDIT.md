# GUI Ecosystem Architecture Audit

**Review date:** 2026-10-21  
**Review baseline:** `development` at `2c6e9b6c1517d9a5fda9609f50ab987d3f7ed816`  
**Scope:** Core semantic model/builders, WPF builder hierarchy, Blazor builders, SVG adapter, conformance tests, and the read-only legacy references in SingularityWarehouse and RevitFamilyManagerBuilders.

This is a source-level architecture review, not a release approval or a claim that every behavior has been exercised locally. No production code changes are implied by this report.

## Executive assessment

The repository is directionally sound: it has a renderer-neutral `GuiNode`, immutable-looking snapshots, recursive composition, a WPF adapter, a Blazor adapter, and an SVG adapter that depends on Core rather than pulling SVG concerns into Core. WPF's `GuiPanelBuilderWPF<TPanel,TBuilder>` preserves the concrete builder type using the same CRTP pattern present in the advanced Revit-family builder repository.

The primary risk is not absence of fluent APIs. It is **multiple overlapping construction contracts and incomplete agreement about which layer owns which responsibility**. The recommended response is a controlled consolidation around the existing semantic model, not a rewrite and not a bulk migration of every legacy builder.

## Reference implementations inspected

- [RevitFamilyManagerBuilders](https://github.com/TrentBest/RevitFamilyManagerBuilders/tree/master/src/RevitFamilyManager.Builders): advanced WPF reference. Its reusable patterns include CRTP panel builders, deferred child-definition rosters, nested component builders, grid/multi-panel/tab composition, focused numeric and enum controls, reflective UI generation, theme/resource fallbacks, and diagnostics. The reference's own README describes recursive, code-first composition. It also marks its older monolithic `GuiBuilderWPF` obsolete in favor of layout-specific builders; do not resurrect that class as the new center.
- [SingularityWarehouse](https://github.com/TrentBest/SingularityWarehouse/tree/master/Core.WPF/Builders): earlier lineage, including window chrome, system/theme defaults, reflective builders, multi-panel layouts, controls, and status/diagnostic hooks.
- [Current WPF pattern audit](WPF_BUILDER_PATTERN_AUDIT.md): already distinguishes transferable patterns from application-specific features.

These repositories are reference material only. This audit does not modify them or claim that every historical feature is appropriate for the public GUI package.

## Findings, ordered by priority

### P0 — Establish one canonical semantic tree-builder API

**Observed:** Core exposes two overlapping public construction paths:
- `GuiBuilder.Create(kind, id)` with callback-based `Child(kind, id, configure)`.
- `GuiBuilders.Panel/Column/Text/Button/Warning(...)` returning `GuiNodeBuilder`, with builder-to-builder and existing-node child overloads.

Both produce `GuiNode` trees and independently maintain mutable construction state. `CoordinateSystemGuiBuilder` also wraps `GuiBuilder`, which makes the duplication a real maintenance surface rather than merely two aliases in documentation.

**Risk:** fixes to validation, depth limits, child semantics, snapshots, or future metadata can diverge between the two paths. Users must learn which builder family to choose and may get different edge-case behavior.

**Recommendation:** designate `GuiNodeBuilder` (or an intentionally selected equivalent) as the canonical fluent builder. Preserve the other entry point as a compatibility facade if it is already consumed; do not break the public API just to make the source look tidy. Add parity tests for both while the migration is in progress, then document the canonical path prominently.

### P0 — Align construction and rendering depth safety

**Observed:** `GuiNode.TryFind` uses iterative depth-first traversal and has a 4,096-node-chain regression test. The WPF renderer validates the tree iteratively and rejects depth beyond 256. The SVG renderer also has a depth/unique-ID boundary. In contrast, both Core builder implementations recursively build descendants, and `GuiNodeBuilder.Child(GuiNode)` recursively clones a supplied tree.

**Risk:** a pathological or externally generated deep tree may be constructible in the model or enter a builder, then fail through process stack exhaustion before an adapter can return a controlled validation error. The maximum-depth policy is not currently a single shared contract.

**Recommendation:** define a shared documented depth/identity policy and enforce it before recursive materialization, or convert builder materialization/cloning to iterative algorithms. Test boundary depths and over-limit behavior for build, clone, find, and every adapter. Keep the WPF renderer's existing defensive validation; do not assume it protects Core builder construction.

### P1 — Keep recursive composition, but make WPF builder contracts intentional

**Observed:** the current WPF layer contains three related but distinct contracts:
- `ICoreBuilder<TProduct>` / `IComponentBuilder` for build identity and a generic product.
- `IComponentWPF` for WPF element manifestation, parent relationships, and message forwarding.
- `GuiPanelBuilderWPF<TPanel,TBuilder>` for CRTP fluent composition and deferred child declarations.

The CRTP base is a strong pattern and closely matches the advanced reference. But the interface split means there is not one obvious contract for a reusable WPF component builder. `NativeGuiBuilder<T>` also returns the base generic type from fluent methods, which is fine for direct use but can truncate fluent calls in subclasses.

**Recommendation:** keep semantic-tree construction separate from native WPF composition. Define and document a small common WPF builder contract, then retain specialized contracts only where they express real capabilities (parenting, message bus, native visual output). Add compile-time fluent-chain tests for derived builders. Avoid forcing WPF's `FrameworkElement` or messaging concepts into Core.

### P1 — Treat SVG as a renderer/manifestation boundary, not a second UI framework

**Observed:** `src/SVG` contains two public renderers that both accept `GuiNode`:
- `TheSingularityWorkshop.Workshop.Gui.Svg.SvgGuiRenderer`: a richer scene-oriented renderer with an explicit `svg-*` vocabulary, coordinate-system handling, presentation attributes, and XML-based output.
- `TheSingularityWorkshop.GUI.SVG.SemanticGuiSvgRenderer`: a simplified semantic-tree diagram renderer with its own options and rendering rules for common UI kinds.

The SVG project references Core only, which is the right dependency direction. The package and docs also explicitly state that declarative SVG animation is presentation data, not an FSM runtime or GPU renderer.

**Risk:** users can reasonably infer that these are interchangeable renderers when they produce different representations and support different vocabularies. The richer renderer's breadth can also tempt scene-specific properties and semantics to leak into Core.

**Recommendation:** explicitly classify the two use cases. If both are needed, name/document them as distinct contracts (for example, semantic UI preview versus SVG scene manifestation) and test the supported vocabulary of each. If one is only transitional, deprecate it only after checking consumers. Keep SVG element names, attributes, coordinate serialization, filters, links, and animation in the SVG package; keep Core's model portable.

### P1 — Port proven builder capabilities selectively, not wholesale

The advanced reference has capabilities that are not represented as equivalent reusable public APIs in the current GUI WPF surface: multi-slot/asymmetric grid composition, tabbed panels, CRUD scaffolding, a type-renderer registry for reflective editors, richer tree/pivot controls, theme orchestration, and diagnostic/status hooks.

This is a **capability gap**, not proof that every one of these features belongs in the core package. Some are host/application-specific; reflection-driven editors in particular need explicit opt-in, safety limits, and a model-binding contract before they belong in a general-purpose NuGet library.

**Recommended transfer order:**
1. Verify and stabilize recursive composition, repeatable build behavior, fluent subtype retention, identity, and ownership.
2. Add reusable layout builders where there is a concrete consumer: grid definitions/placements, multi-panel and tab composition.
3. Add optional reusable controls (numeric/enum first) with validation and interaction tests.
4. Consider theme tokens/resource fallbacks as an opt-in layer, with native system defaults and no mandatory Warehouse-specific resource keys.
5. Consider reflection/CRUD/tree/pivot features only after defining a deliberate extension boundary and testing real consumers.

### P1 — Expand cross-adapter contract tests beyond Blazor + SVG

**Observed:** the shared adapter conformance fixture currently compares Blazor and SVG output for selected shared invariants. WPF has its own Windows-targeted renderer tests, including nesting, order, properties, fallback behavior, duplicate IDs, and depth limits.

**Risk:** separately passing adapter suites do not guarantee the same semantic fixture retains the same meaning across all supported outputs.

**Recommendation:** define the common invariant subset (identity, child order, text, source, selected semantic properties, and defined fallback behavior) and run a common fixture through Blazor, SVG, and WPF in the Windows CI lane. Keep adapter-specific expectations separate rather than pretending every native behavior is identical.

### P2 — Clarify native builder lifecycle and repeatability

**Observed:** `NativeGuiBuilder<T>` allocates one WPF element at builder construction and returns that same element from `Build()`. The panel builder instead clears/repopulates its existing root from a deferred definition roster on each build.

**Risk:** callers may assume `Build()` always creates a fresh visual tree. Reusing a native element already attached to another parent can violate WPF visual-parent rules; repeated builds may also retain mutable state by design.

**Recommendation:** document the lifecycle explicitly and test it. Choose deliberately between single-use/same-instance builders and factories that produce fresh elements. Avoid silently changing identity semantics, since consumers may rely on the current behavior.

### P2 — Recalculate release readiness against the current commit

The readiness and next-release queue currently cite older October 19 CI evidence and a 70% estimate. This review baseline is October 21, commit `2c6e9b6...`; its Package GUI, source-derived diagram, and solution-derived UML workflows passed (runs [39687432158](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39687432158), [39687432107](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39687432107), and [39687432151](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39687432151)).

Those passing workflows are useful evidence, but they do not automatically close the architecture gaps above. The 70% figure should remain labelled as the last recorded estimate until the release checklist is reviewed and recalculated against a selected candidate. Package-version alignment, candidate-specific archive inspection, and downstream consumer proof remain release-gate items in the existing docs. No publication is authorized by this audit.

## Recommended execution sequence

1. **Contract map:** list every public builder, renderer, and interface; identify the canonical entry point and compatibility obligations.
2. **Core hardening:** unify validation and depth policy; remove unbounded recursive materialization/cloning risks; preserve compatibility tests.
3. **WPF builder conformance:** test CRTP fluent return types, child order, repeated builds, parent ownership, identity, native-builder lifecycle, and errors.
4. **Adapter conformance:** expand shared semantic fixtures to all adapters where comparable, while retaining native-specific tests.
5. **Selective capability transfer:** implement the highest-value missing reusable WPF compositions from the Revit reference behind clear contracts.
6. **SVG boundary:** explicitly document the two renderer roles and ensure scene-specific semantics stay out of Core.
7. **Living proof:** add/update examples that build one real semantic tree and show the resulting adapter outputs; do not substitute static mockups for implementation evidence.
8. **Release gate:** refresh readiness percentage and CI/archive/consumer evidence on the exact candidate commit. Keep NuGet publishing blocked until the owner explicitly authorizes it.

## Bottom line

The recursive-builder direction is right. Preserve it. The immediate task is to make the semantic builder, WPF composition builders, and SVG manifestation boundaries coherent and defensively tested before expanding the feature catalog. The legacy Autodesk-facing builders are a proven design reference—not a mandate to import every old feature or its application-specific assumptions.
