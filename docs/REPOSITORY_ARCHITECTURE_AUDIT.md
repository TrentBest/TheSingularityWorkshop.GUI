# Repository Architecture and Completeness Audit

**Review baseline:** PR #18 candidate `b5eafdc555cd2cc66f981947c43794cee50ace71` (2026-10-20).  
**Exact-head evidence:** [solution-derived UML inventory run #39508725522](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39508725522), [source-derived diagrams run #39508725497](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39508725497), and [package GUI run #39508725482](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39508725482) all completed successfully on this candidate; NuGet publication was skipped. The solution workflow now builds and reconciles exact assembly metadata/type inventories for all four maintained product projects. This verifies the configured inventory, source-diagram, and package/test scope—not member-level metadata parity, completion of the UML roadmap, or publication authorization.  
**Purpose:** map the repository's intended whole, identify correct ownership boundaries, separate implemented behavior from scaffolding, and sequence the missing bridges.  
**Audit type:** architecture and repository-structure assessment based on the solution manifest, project files, current roadmap, release gates, and the source-derived UML staging work. This is not yet a declaration-by-declaration code audit of every production file.

## Executive assessment

The repository has a coherent and worthwhile central idea: a platform-neutral semantic GUI model whose meaning is manifested by adapters. It also contains a much larger long-term vision—spatial and immersive representation, arbitrary visualization, semantic input, live diagrams, and an execution-host model. Those ambitions should remain visible, but they must not all become Core responsibilities or release promises at once.

The main problem is not that the vision is too broad. It is that **vision, contracts, working implementations, generated evidence, and downstream orchestration are not yet consistently separated and tracked as different states**. Several architecture documents already acknowledge this; the repository's next step is to turn that distinction into an executable inventory and a dependency-aware implementation plan.

### Current confidence by area

These are qualitative engineering estimates, not test-coverage percentages or release authorization.

| Area | Assessment | What is true today | Main missing bridge |
|---|---|---|---|
| Core semantic tree and builders | **Working foundation; incomplete contract** | Recursive model/builders and Core tests exist. | Typed semantic properties, stable identity rules, capabilities, lifecycle, input, accessibility, localization, state binding and media contracts. |
| Blazor adapter | **Prototype with test evidence** | Core-to-Blazor rendering path and render-tree tests exist. | Complete property/event/accessibility/lifecycle contract and candidate-version real-host proof. |
| SVG adapter | **Strong inspectable output path; partial semantic coverage** | SVG serialization, generated gallery, geometry guards and deterministic-output checks exist. | Explicit hostile-input/reference policy, broader semantic coverage, visual text-bound checks, and parity evidence. |
| WPF adapter/builders | **Initial tested native path** | Windows tests cover a meaningful initial surface. | Complete mapping, lifecycle/events/accessibility, capability differences and cross-adapter parity. |
| Diagram/data-card semantics | **Design vocabulary; not yet a complete stable contract** | Ownership is documented; existing recursive GUI nodes can manifest diagrams. | Typed diagram contract, commands/provenance, expansion/edit modes and validated mutations. |
| Source-derived UML | **Verified staging vertical slice; all-project type inventory verified** | Solution-level CI builds Core, Blazor, WPF and SVG in Release, checks generated type-page coverage/provenance, and reconciles compiled assembly metadata/type inventories against compiler models (32/6/22/7 types). | Member-level metadata parity, multi-target-framework support, full graph/navigation, visual bounds, authored scenarios, embedded per-type UML SVGs and generated-artifact drift checks. |
| Documentation and theory | **Broadly articulated; organization is uneven** | Architecture, theory, adapter guides, galleries, roadmap and release gates exist. | Single source of truth for status, remove stale/duplicated content, ensure every intended document is discoverable, and add evidence-linked living diagrams. |
| Packaging and release | **Repeatable workflow; not release-ready** | Package/test workflows and Windows lane exist; publication is gated. | Align project and published versions, select a candidate, inspect exact candidate archives, and verify downstream consumer against that candidate. |
| Runtime composition / execution host | **Vision/specification, not GUI implementation** | A conceptual execution model is documented. | Put experience composition, micro-bundle resolution, runtime ownership and orchestration in the appropriate runtime layer; expose only presentation/interaction contracts here. |

## 1. What belongs in this repository

### Keep in GUI Core

- Platform-neutral GUI identity, hierarchy, recursive composition and snapshots.
- Semantically defined content, properties, layout/spatial intent, selection/focus and interaction intent when those contracts can be stated without a particular UI platform.
- Diagram/data-card representation contracts that are reusable beyond C# and UML.
- Stable identity and capability/unsupported-feature reporting contracts shared by adapters.
- Contract tests that define the neutral model's promised behavior.

**Do not put in Core:** Roslyn syntax trees, C# AST/source-generation rules, SVG elements, Blazor render fragments, WPF controls, DOM/CSS/JavaScript, runtime host orchestration, or domain-specific Workshop concepts. Core must remain a representation contract, not a code parser or application framework.

### Keep as adapter packages

- **GUI.Blazor:** manifestation in Blazor, render-tree lifecycle, web-specific accessibility and event translation.
- **GUI.SVG:** deterministic SVG manifestation, geometry/coordinate mapping, SVG accessibility and serialization policy.
- **GUI.WPF:** native WPF manifestation and WPF-specific builders/identity bridge.
- Shared conformance tests should specify only genuinely shared semantics; adapter-specific differences must remain explicit instead of being hidden by a lowest-common-denominator claim.

The four package family (Core, Blazor, SVG, WPF) is already explicitly in scope for the next package-family release. That is a scope decision, not evidence of parity.

## 2. What belongs elsewhere (or in a separate tool)

| Concern | Correct home | Boundary |
|---|---|---|
| C# parsing, Roslyn compilation, source provenance and source-to-model projection | Build-time source-analysis tool/package | It consumes source/project context and produces neutral diagram/documentation data. It must not become a Core runtime dependency. |
| Reflection against the exact compiled output assembly | Build-time documentation integration | Reconcile against the exact target-framework build; do not silently compile a different configuration or infer from an unrelated assembly. |
| SVG rendering of semantic diagram models | GUI.SVG | The model is renderer-neutral; SVG owns the manifestation. |
| Authoritative source-code AST, code editing and code-generation commands | Code/creation domain or dedicated editor/tool | A diagram is a view; editing must issue validated domain commands rather than mutate rendered geometry as source of truth. |
| WebPage-specific experiences, Workshop/Laboratory/Forge concepts and application navigation | WebPage / consuming application | The GUI package should not acquire application-domain knowledge. |
| Experience manifests, micro-bundle resolution/composition, runtime context, arbitration, execution lifecycle and hot replacement | Runtime composition layer such as FSM_COS / AnyApp architecture | GUI can observe runtime state and issue semantic interaction requests; it should not duplicate the runtime's authority. |
| Device-specific input translation | Input/device adapters at the boundary | Mouse, keyboard, touch, voice, gaze and controllers map to semantic intent; physical device details do not become Core meaning. |
| Deployment, hosting, Azure storage and immutable experience addresses | Hosting/repository/runtime integration | GUI consumes the appropriate contract and should not own hosting policy. |
| Unity-specific presentation | Separate adapter/integration only if explicitly revived | Unity is not a current required target; no Unity types should leak into Core. |

**Important architectural reconciliation:** `docs/GUI_EXECUTION_MODEL.md` describes the execution-host inversion as proposed architecture, while the core architecture describes GUI as an extrinsic observer/manifester. Preserve both as an explicit design question, but do not implement execution ownership in the GUI package by implication. The likely stable boundary is GUI interaction/observation plus a separately owned runtime execution contract. If the broader architecture assigns the outer host to GUI, it needs a crisp contract showing that the runtime remains authoritative and the GUI layer does not become the FSM_COS/AnyApp composition engine.

## 3. Concrete repository-structure findings

### Verified from the current solution and project declarations

1. **The test-project solution omission is resolved on this candidate.** `TheSingularityWorkshop.GUI.slnx` now includes `tests/WPF.Tests/TheSingularityWorkshop.GUI.WPF.Tests.csproj` and `tests/Adapter.Conformance.Tests`; the solution-level inventory workflow validates the maintained product-project scope separately from tests and tooling.
2. **The documentation project is not a complete documentation inventory.** `TheSingularityWorkshop.GUI.Documentation.csproj` explicitly lists many architecture/theory files, but the current UML documents (`SOURCE_DERIVED_UML.md`, `LIVING_UML_ARCHITECTURE.md`, `UML_VISUAL_AUDIT.md`, and related data-card/gallery material) are not in its explicit item list. Decide whether the Visual Studio documentation project should include all maintained documentation and enforce that decision automatically.
3. **The README is both an onboarding page and a long-form architecture document.** Its initial What/Why/How/See-it-in-a-minute sequence is useful, but it then repeats architecture, package boundaries, theory, integration caveats and reference assets at length. This conflicts with the stated goal of a short five-section README. Keep the entry point brief and move the authoritative details to linked documents; avoid deleting useful theory—remove duplication, not content.
4. **The release-readiness history contains several superseded estimates and historical artifact observations.** The audit trail is useful, but the current decision surface must be easy to find. Keep one clearly labeled current estimate/candidate status and move old evidence into a dated history or archive section so readers do not mistake older package contents for the candidate.
5. **Project and published package versions have previously diverged.** The release checklist explicitly records this as a blocker. Candidate version alignment must be verified from current package metadata, not inferred from a prior successful workflow.
6. **Source-derived UML is intentionally not complete, but the all-project type-inventory gate is now verified.** The solution-level workflow builds all four maintained product projects in Release, validates non-empty type models and one page per model type, checks revision/project provenance, and compares exact-build PE assembly/type metadata with the Roslyn models. Member signatures/attributes/constraints and target-framework-specific surfaces remain open, as do whole-project graph navigation, rendered text bounds, stronger model normalization, authored use-case persistence/review, per-type embedded UML SVGs, and committed generated-artifact drift enforcement. Passing inventory checks is not complete per-file UML coverage.
7. **Generated-output cleanup has a safe boundary in the current staging implementation.** The manifest-backed type-page pruning only removes listed stale generated pages and rejects unsafe paths; the regression test preserves unrelated authored Markdown. Keep this fail-closed policy as the generator grows.

## 4. The broad-strokes architecture map

```text
Consuming application (WebPage / desktop / future host)
  owns domain meaning, workflows, and experience-specific builders
          |
          v
Runtime composition / experience state (FSM_COS / AnyApp boundary)
  owns bundles, configuration, execution context, lifecycle and arbitration
          |
          | observable state + semantic interaction requests
          v
GUI semantic contracts (Core)
  identity, hierarchy, presentation intent, diagram semantics,
  input intent, capability and lifecycle contracts
          |
          +----------------+----------------+----------------+
          v                v                v                v
       Blazor             SVG              WPF        future adapters
          |
          +--> platform-specific manifestation and interaction mapping

Build-time documentation tool (separate from runtime packages)
  source/project analysis -> neutral model -> inventory/per-type pages
  -> semantic graph model -> GUI.SVG -> visual + drift verification
  -> optional reflection reconciliation against exact build outputs
```

This map is the scaffold. Individual contracts can remain incomplete as long as their ownership, dependencies, evidence and next implementation step are explicit.

## 5. Completeness rules: do not confuse these states

Every capability, contract, type page and diagram should have one of these statuses:

- **Implemented:** source exists.
- **Tested:** named tests exercise the behavior.
- **Verified:** the relevant test/build ran on a named exact commit.
- **Specified:** desired behavior is documented but not implemented.
- **Scaffolded:** a stable seam or fixture exists, but the end-to-end path is incomplete.
- **Blocked:** a named dependency or decision prevents completion.
- **Deferred:** intentionally outside current scope, with an owner/boundary.
- **Retired:** removed from the intended architecture and no longer tracked as a future promise.

A green CI run only proves the configured checks that actually ran. A document describing a contract does not make it implemented. A generated SVG that is structurally valid is not yet visually verified for text overflow, connector overlap, or meaningful layout.

## 6. Recommended execution order

### Gate A — make the repository inventory trustworthy

- [x] Discover all `.csproj` files and require each to be a solution member or an explicitly classified standalone sample.
- [x] Validate solution project paths and explicit documentation-project file references; reject missing or duplicate entries.
- [x] Enumerate repository Markdown, GitHub Actions workflow files, and visual/reference assets while excluding build/cache output.
- [x] Classify Markdown as authored-maintained, require explicit workflow classifications, and require explicit authored/generated/reference classifications for every visual asset; CI rejects newly unclassified or stale entries.
- [x] Compare generated DataCard UML and the SVG feature gallery against their checked-in generated assets in CI.
- [ ] Reconcile the fox/sheepdog scene generator with the richer checked-in authored SVG. The current generator lacks several authored behavior states, so its output must stay separate rather than overwrite or be falsely declared byte-identical to the richer reference.
- [ ] Extend lifecycle labels to project/sample/source inventories and distinguish fixture, historical, and retired items. Current explicit classifications cover project inclusion/exclusion, workflows, Markdown, and visual assets.
- [ ] Identify duplicate/stale claims and choose one authoritative status location for each capability.
- [ ] Expand drift and lifecycle validation to every generated asset, including stale/orphaned output detection and safe separation of authored files.
- [ ] Keep the inventory machine-checkable so the audit does not become another manually drifting document.

### Gate B — stabilize the smallest useful semantic contracts

- Audit Core public types and dependencies for neutrality.
- Define stable identity, typed properties, capability reporting, lifecycle, input intent, accessibility/alternate presentation, localization and state binding in deliberate increments.
- Keep diagram semantics renderer-neutral and code AST/source parsing outside Core.
- Turn the shared contract into adapter conformance tests; record gaps by adapter.

### Gate C — make source-derived documentation a real repository capability

- Extend the current Core-focused project-semantic model to deterministic **solution/multi-project inventory**: project paths, project references, target frameworks, source roots, load diagnostics and inclusion/exclusion decisions.
- Generate source inventory and per-type documentation/diagrams for each in-scope production project, not only a focused Core slice.
- [x] Load the explicitly selected Core DLL by exact path, verify its assembly identity against the compiler model, and compare type identities; fail on a mismatched artifact or missing/unexpected non-generated types.
- [ ] Extend reflection reconciliation to member signatures, attributes, generic constraints, and each target-framework-specific surface.
- Add whole-project graph navigation/layout after the model is normalized; do not force a giant graph before identities and relationships are reliable.
- Verify painted text bounds and connector geometry on rendered output, in addition to XML/accessibility/local-reference checks.
- Add authored scenario/use-case storage with provenance and clear generated/authored separation.
- Regenerate in CI and fail on missing, stale, orphaned or unexpectedly edited generated artifacts. Never silently overwrite authored files.

### Gate D — release and downstream proof

- Align package versions and release channel.
- Select a candidate SHA and inspect the actual four-package archives.
- Verify clean consumption from package archives rather than relying only on project references or a build from the source checkout.
- Run the consuming WebPage against the selected candidate package set.
- Keep publish disabled until all gates pass and publication is explicitly authorized.

### Gate E — expand the vision without corrupting boundaries

Only after the core contracts are dependable, progressively add richer media, 2D/3D view/viewport/camera semantics, spatial identity/transforms, semantic input, animation intent, multi-view synchronization, accessibility and alternate presentation, capability negotiation, and runtime-observer integration. Treat GPU/immersive/future adapters as explicit extensions, not as obligations to add platform dependencies to Core now.

## 7. Audit conclusion

**Do not reduce the vision. Reduce ambiguity.** The correct next milestone is not “implement every idea” and not “finish UML in isolation.” It is a trustworthy repository inventory that ties each intended capability to its owner, current state, evidence, dependencies and next gate. Then the broad architecture can be filled in incrementally without losing the destination or misplacing responsibilities.

The current PR is a useful first vertical slice for the documentation pipeline. It is not the whole solution, a stable published diagram API, or release authorization. Keep it draft until its stated gates are complete and the integrated candidate is verified.


## 8. Refined vision: GUI is a substantial presentation layer, not a small widget library

The operating-system analogy is useful: presenting, organizing, navigating, inspecting, and interacting with information can itself be a large subsystem. **The GUI layer is allowed to be massive as a family of capabilities.** The architectural constraint is not “keep GUI small”; it is “keep each package's responsibility narrow, composable, and optional.”

The package family should let a consumer install the smallest useful presentation stack for its environment:

- **Core** provides the minimum platform-neutral semantic vocabulary and composition model.
- **Adapters** add concrete manifestations such as Blazor, SVG, or WPF only when the host needs them.
- **Specialized packages** can add diagrams, immersive editing, spatial presentation, richer input, or other presentation capabilities without making every consumer inherit those dependencies.
- **Build-time tools** analyze source and produce models/assets without introducing compiler tooling into runtime packages.
- **Runtime/application layers** supply the definitions, state, domain commands, and lifecycle that presentation surfaces inspect or request changes through.

This means a broad GUI vision and a small dependency footprint are complementary rather than contradictory. A minimal headless consumer should not need SVG or WPF; a web consumer should not need WPF; an editor that wants rich diagrams can opt into the relevant package set. Package boundaries should be validated through dependency-graph checks and clean-consumer tests, not just described in prose.

### SVG as an optional experience surface

SVG is not merely a documentation export format. It can also be a presentation surface inside a consuming experience, when that experience chooses to include the SVG capability. A developer editing a definition could enter an immersive inspection mode and see the living SVG/UML view attached to that definition: the visual becomes a navigable explanation of the thing being edited, with links back to semantic identities, source locations, relationships, and applicable evidence.

That possibility should remain in the architecture now, but it must be staged honestly:

1. **Foundation (current direction):** stable definition/type identity, provenance links, source-derived model, deterministic SVG manifestation, accessible SVG output.
2. **Inspectable experience:** a consuming app can associate an SVG asset/model with a definition and navigate between the definition and its visual without treating the SVG as authoritative source.
3. **Interactive editor mode:** selection in the visual resolves to a semantic identity; commands go through the authoritative editor/domain model; validation and state changes are reflected back into the view.
4. **Immersive presentation:** alternate layouts, viewport/navigation, spatial or multi-panel modes, and optional richer input adapters can be explored without imposing them on minimal consumers.

The SVG must remain a manifestation of a canonical semantic/documentation model, not the only source of truth. If an editor allows changes, it must issue validated semantic commands against the authoritative definition; editing path geometry must not silently rewrite the meaning of a type or workflow.

### Refined boundary: presentation can be broad while runtime authority stays explicit

The GUI layer may coordinate presentation surfaces, navigation, selection, focus, view state, and semantic interaction requests. It may be the outer presentation host in some architectures. That does **not** automatically make it the owner of application workflow execution, bundle resolution, arbitration, or domain state. Those responsibilities must be explicitly assigned to the consuming app/runtime contract (for example, FSM_COS / AnyApp), and the GUI host should integrate with that contract rather than clone it.

Accordingly, the earlier observer/manifester framing is a safe default boundary, not a restriction to passive rendering. GUI may be interactive and may offer an immersive editor; the critical distinction is between **presentation/editor interaction state** and **authoritative application/runtime state**. Document that contract before implementing host inversion.

### Acceptance checks for this expanded direction

- A Core-only consumer restores and runs without adapter dependencies.
- Each adapter/package can be selected independently, and its transitive dependency graph is intentional.
- Shared semantic behavior is tested by conformance tests; adapter-specific capability gaps are reported, not hidden.
- A definition can reference its generated visual through stable identity/provenance rather than a hard-coded copied SVG blob.
- The visual can be omitted entirely, loaded on demand, or presented in an immersive mode without making SVG a mandatory runtime dependency.
- Any future editing interaction resolves to semantic identity and validated commands; the SVG is never treated as authoritative domain state.
- Host/runtime integration states which layer owns lifecycle, execution, domain state, navigation, and view state.
