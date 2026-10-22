# GUI Ecosystem Architecture Audit

**Review date:** 2026-10-21  
**Review baseline:** `development` at `2c6e9b6c1517d9a5fda9609f50ab987d3f7ed816`  
**Follow-up:** canonical Core builder consolidation and the shared WPF build/identity contract are committed on `development`; candidate-specific CI evidence is recorded in the repository Actions history.  
**Scope:** Core semantic model/builders, WPF builder hierarchy, Blazor builders, SVG adapter, conformance tests, and the read-only legacy references in SingularityWarehouse and RevitFamilyManagerBuilders.

This is a source-level architecture review, not a release approval or a claim that every behavior has been exercised locally.

## Executive assessment

The repository is directionally sound: it has a renderer-neutral `GuiNode`, immutable snapshots, recursive composition, a WPF adapter, a Blazor adapter, and an SVG adapter that depends on Core rather than pulling SVG concerns into Core. WPF's `GuiPanelBuilderWPF<TPanel,TBuilder>` preserves the concrete builder type using the same CRTP pattern present in the advanced Revit-family builder repository.

The primary risk is not absence of fluent APIs. It is **multiple overlapping construction contracts and incomplete agreement about which layer owns which responsibility**. The Core duplicate builder implementation has now been consolidated in source; CI must confirm the change before the finding is considered closed.

## Reference implementations inspected

- [RevitFamilyManagerBuilders](https://github.com/TrentBest/RevitFamilyManagerBuilders/tree/master/src/RevitFamilyManager.Builders): advanced WPF reference. Its reusable patterns include CRTP panel builders, deferred child-definition rosters, nested component builders, grid/multi-panel/tab composition, focused numeric and enum controls, reflective UI generation, theme/resource fallbacks, and diagnostics. The reference's own README describes recursive, code-first composition. It also marks its older monolithic `GuiBuilderWPF` obsolete in favor of layout-specific builders; do not resurrect that class as the new center.
- [SingularityWarehouse](https://github.com/TrentBest/SingularityWarehouse/tree/master/Core.WPF/Builders): earlier lineage, including window chrome, system/theme defaults, reflective builders, multi-panel layouts, controls, and status/diagnostic hooks.
- [Current WPF pattern audit](WPF_BUILDER_PATTERN_AUDIT.md): already distinguishes transferable patterns from application-specific features.

These repositories are reference material only. This audit does not modify them or claim that every historical feature is appropriate for the public GUI package.

## Findings, ordered by priority

### P0 — Canonical semantic tree-builder API

**Initial observation:** Core exposed two overlapping public construction paths: `GuiBuilder.Create(kind, id)` with callback-based children, and `GuiBuilders.Panel/Column/Text/Button/Warning(...)` returning a separate `GuiNodeBuilder`. Both maintained their own mutable construction state and materialization logic.

**Correction made on development:** `GuiBuilder` is now the sole fluent builder type. `GuiBuilders` is only a semantic factory over that same type. The separate `GuiNodeBuilder` implementation has been removed. Canonical composition supports callback children, builder-to-builder children, and immutable `GuiNode` children. Builder materialization and immutable-node cloning use iterative traversal; cyclic builder composition throws a controlled `InvalidOperationException`. Tests exercise factory/direct type identity, nested composition, deep trees, clone behavior, and cycle rejection.

**Verification state:** source change is committed at `c1afcdb282c99cdb59fc9c90d6c0833ae0d09d41`; the Package GUI, source-derived diagram, and solution-derived UML workflows are queued/running and have not yet verified the commit. Do not mark this finding closed until all required workflows pass and generated public API documentation no longer lists the removed type.

### P0 — Align construction and rendering depth safety

**Observed:** `GuiNode.TryFind` uses iterative depth-first traversal and has a 4,096-node-chain regression test. The WPF renderer validates the tree iteratively and rejects depth beyond 256. The SVG renderer also has a depth/unique-ID boundary.

**Correction made on development:** the canonical Core builder materializes child trees iteratively and clones supplied immutable `GuiNode` trees iteratively, removing the previous recursive build/clone paths. The 4,096-level builder and clone regression tests are part of the current candidate. The cross-layer maximum-depth policy is still not a single shared contract; adapter-specific boundaries require separate review.

**Risk remaining:** a tree can be built by Core and later be rejected by an adapter with a stricter limit. This should be documented and tested as an intentional boundary rather than mistaken for one global depth guarantee.

### P1 — Keep recursive composition, but make WPF builder contracts intentional

**Observed:** the current WPF layer contains three related but distinct contracts:
- `ICoreBuilder<TProduct>` / `IComponentBuilder` for build identity and a generic product.
- `IComponentWPF` for WPF element manifestation, parent relationships, and message forwarding.
- `GuiPanelBuilderWPF<TPanel,TBuilder>` for CRTP fluent composition and deferred child declarations.

The CRTP panel base is a strong pattern and closely matches the advanced reference. The `NativeGuiBuilder<T>` fluent-subtype defect has now been corrected on `development`: the shared implementation is `NativeGuiBuilder<T,TBuilder>`, while `NativeGuiBuilder<T>` remains the convenient concrete self-typed form. A Windows-targeted test uses a specialized button builder to prove that native fluent operations retain the derived type.

**Correction made on development:** `IComponentWPF` now extends the common `ICoreBuilder<FrameworkElement>` contract, so composable WPF components and native element builders share the same build/identity contract. `GuiPanelBuilderWPF` maps its stable component `Id` to `GetBuilderId()`. The additional `SetParent`/`GetParent` and `Publish`/`Receive` methods remain capabilities of composable components rather than being imposed on every native element builder. A regression assertion verifies that a panel builder can be consumed through the shared contract and returns its stable identity.

**Boundary retained intentionally:** `IComponentBuilder` adds a generic host-parent reference for native builders; `IComponentWPF` adds typed component-tree parenting and message forwarding. These remain distinct because their semantics differ. The CRTP correction and shared identity contract are awaiting candidate-specific CI.

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

### P2 — Native builder lifecycle and component ownership

**Contract confirmed in source and tests:** `NativeGuiBuilder<T,TBuilder>.Build()` returns the same native element instance on every call. `GuiPanelBuilderWPF<TPanel,TBuilder>.Build()` returns the same root panel and clears/repopulates its direct children from the deferred roster on each call. This is repeatable reconstruction of a builder-owned tree, not a fresh-tree factory.

**Correction made on development:** panel composition now registers each `IComponentWPF` child before queuing its mount. Registration rejects the same component being added twice to one builder and rejects a component whose logical parent is another builder. Grid placements use the same ownership guard as ordinary panel composition, closing the separate path that previously bypassed the rule. The controlled failure tells callers to create a distinct component instance for each visual parent rather than letting WPF fail later while attaching an already-parented element.

**Intentional boundary:** `CanvasGuiBuilderWPF.PlaceAt(UIElement,...)` accepts a raw element, not a component, so its element ownership is not covered by the component-parent contract yet. A future adapter-wide ownership policy should cover raw-element APIs consistently. The current correction prevents duplicate/reparented component registrations; it does not claim that arbitrary externally-mutated WPF visual trees are fully policed.

**Verification:** new Windows-targeted regression tests cover duplicate registration and cross-parent rejection while retaining the existing repeated-build behavior. Candidate CI must pass before this finding is considered verified.

### P2 — Recalculate release readiness against the current commit

The readiness and next-release queue cite older October 19 CI evidence and a 70% estimate. The original review baseline was October 21, commit `2c6e9b6...`; its Package GUI, source-derived diagram, and solution-derived UML workflows passed (runs [39687432158](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39687432158), [39687432107](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39687432107), and [39687432151](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39687432151)).

Those passing workflows are useful evidence, but they do not automatically close the architecture gaps above. The 70% figure should remain labelled as the last recorded estimate until the release checklist is reviewed and recalculated against a selected candidate. Package-version alignment, candidate-specific archive inspection, and downstream consumer proof remain release-gate items in the existing docs. No publication is authorized by this audit.

## Recommended execution sequence

1. **Core consolidation:** complete verification of the canonical builder commit, update generated API documentation, and close stale references to the removed builder.
2. **WPF builder conformance:** test CRTP fluent return types, child order, repeated builds, parent ownership, identity, native-builder lifecycle, and errors.
3. **Adapter conformance:** expand shared semantic fixtures to all adapters where comparable, while retaining native-specific tests.
4. **Selective capability transfer:** implement the highest-value missing reusable WPF compositions from the Revit reference behind clear contracts.
5. **SVG boundary:** explicitly document the two renderer roles and ensure scene-specific semantics stay out of Core.
6. **Living proof:** add/update examples that build one real semantic tree and show the resulting adapter outputs; do not substitute static mockups for implementation evidence.
7. **Release gate:** refresh readiness percentage and CI/archive/consumer evidence on the exact candidate commit. Keep NuGet publishing blocked until the owner explicitly authorizes it.

## Bottom line

The recursive-builder direction is right. Preserve it. The Core semantic builder is now consolidated in source around one fluent type; CI and generated API documentation must verify the consolidation before it is considered complete. Keep WPF's recursive composition and SVG manifestation boundaries coherent and defensively tested before expanding the feature catalog. The legacy Autodesk-facing builders are a proven design reference—not a mandate to import every old feature or its application-specific assumptions.
