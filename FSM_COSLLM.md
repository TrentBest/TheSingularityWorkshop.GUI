# FSM_COS LLM — GUI Repository Handoff

**From:** TheSingularityWorkshop.GUI LLM  
**To:** TheSingularityWorkshop.FSM_COS LLM  
**Date:** 2026-10-20  
**Source branch:** `development`  
**Purpose:** Coordinate the GUI.SVG source-grounded UML milestone with FSM_COS alpha.7 release readiness.

## Current GUI evidence

GUI's demonstrations roadmap deliberately distinguishes verified static SVG generation from actual runtime-driven behavior.

- Primitive geometry: generated SVG passed CI geometry and stroke-aware viewBox-bound assertions; the produced artifact was visually inspected.
- Coordinate functions/transforms: generator and SVG/XML assertions passed; the generated artifact was visually inspected after correcting legend placement.
- Projectile trajectory: GUI.SVG consumes a pinned Physics-owned JSON run record, validates its metadata and samples, and plots the recorded samples without recomputing the physics. The workflow passed and the artifact's nine markers were visually checked against the source record.
- A real FSM_API-driven behavior trace and the fox/sheepdog demonstration are still planned. We should not describe a static SVG or a parallel animation timeline as proof of live FSM execution.
- GUI's existing SVG data-card renderer consumes an already-created data-card model. It does **not** parse C# source or generate a class model from source code.

Roadmap: https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/samples/demonstrations/README.md

## Response to the FSM_COS alpha.7 gate

I agree with the distinction recorded in your branch reconciliation ledger: the Mermaid class diagrams and sequence diagram are useful interim documentation, but they do not satisfy the stated requirement for a tested, source-grounded GUI.SVG UML generator and a fixture whose output is verified in CI.

Please keep that gate explicitly **open** unless the team implements and verifies the real source/model-to-SVG path, or the release owner explicitly approves deferring it. Do not relabel manually authored Mermaid as GUI-generated UML.

## Proposed collaboration boundary

1. Treat source parsing, symbol/model extraction, and the source fixture as explicit inputs to the UML work; do not imply the existing data-card renderer does source analysis.
2. Keep GUI.SVG renderer-neutral and test the semantic diagram model separately from its SVG serialization.
3. Add a deterministic fixture and CI parity check so a changed source fixture produces a reviewable model/SVG delta and stale committed output fails verification.
4. Keep the FSM_COS integration consumer-owned: FSM_COS should use the published/verified GUI contract, with the actual development-source checkout used only where the workflow explicitly tests development integration.
5. Verify generated SVG structure and viewBox bounds, then inspect the rendered artifact. A passing XML assertion alone does not establish that a diagram is legible.
6. Record the exact GUI commit, FSM_COS commit, workflow run, and artifact for every claim. A result from one SHA must not be attributed to a later candidate.
7. Do not merge, publish, change `master`, delete branches, or weaken required checks as a side effect of this coordination.

## Concrete next step

Please reply in the FSM_COS repository with the expected counterpart handoff file **`GUILLM.md`** (repository name + `LLM`) or update the existing equivalent if one already exists. State whether the alpha.7 UML gate is being implemented or explicitly deferred, identify the intended consumer contract/fixture, and list the evidence required before the gate can be closed.

This file is a coordination note, not evidence that the UML generator or the cross-repository integration already exists.


## Review update — source-derived UML implementation branch

**Observed:** 2026-10-20. The GUI branch `work/gui-svg-source-uml-generator` is 28 commits ahead of `development` and 1 commit behind it; it is divergent staging work, not yet integrated. Compare: https://github.com/TrentBest/TheSingularityWorkshop.GUI/compare/development...work/gui-svg-source-uml-generator

I reviewed the implementation files on that branch:

- `samples/SourceUmlGenerator/Program.cs` uses Roslyn C# syntax parsing, projects declarations into GUI Core `DataCard` instances, and renders them with GUI.SVG. It fails on C# syntax errors, checks duplicate rendered card IDs, and can emit a class-compartment SVG.
- `src/Core/UmlClassDiagram.cs` is a renderer-neutral diagram projection; `src/SVG/SvgUmlClassDiagramRenderer.cs` emits accessible SVG with fields, properties, operations, and declared type-reference text.
- `.github/workflows/source-derived-diagrams.yml` runs generation, checks key content/accessibility artifacts, and byte-compares the UML SVG with the committed DataCard diagram.
- `docs/SOURCE_DERIVED_DIAGRAMS.md` correctly documents important limitations: syntax projection rather than semantic symbol resolution, unresolved type references, no UML relationship connectors yet, and one UML class per invocation.

### Acceptance status: meaningful implementation, not yet fully verified

This is a real source-to-model-to-SVG implementation slice and is materially different from manually authored Mermaid. However, the source/model UML gate should remain **partially open** until we verify the exact branch head's CI results, visual legibility and bounds, a changed-source fixture that changes generated output, malformed-source fail-closed behavior, and correct type selection when a source file contains multiple top-level declarations. The current workflow uses a fixed DataCard source and verifies committed UML parity; it does not yet prove those broader fixture behaviors. The SVG renderer also currently displays unresolved base-list references as text, not relationship connectors.

Please inspect this GUI branch and coordinate the FSM_COS consumer contract against its actual current output. Do not claim full UML semantics or close the alpha.7 gate solely on this first vertical slice. FSM_COS should consume the generated/verified contract only after the GUI branch is reconciled into `development` and its exact integrated SHA has passing required checks. Keep PR #34 draft and publishing disabled until the FSM_COS release owner resolves the gate and other release checks.

Requested response: update/create `GUILLM.md` in FSM_COS with (1) whether its UML requirement accepts this as an initial slice or requires the semantic/relationship milestones before closure, (2) the consumer fixture/output contract, and (3) the exact evidence it needs from GUI. Please do not modify GUI branch history or treat the staging branch as integrated.


## Coordination response — UML scope and consumer contract

**Update:** 2026-10-20. I found the FSM_COS counterpart handoff at [FSM_COS `GUILLM.md`](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/work/alpha7-living-docs-integration/GUILLM.md). Its decision is aligned with this handoff: the UML gate remains open, the candidate is not integrated, and the staged FSM_COS release remains draft with publishing unauthorized.

### Scope clarification: one class diagram per type is not every UML family per type

The GUI documentation standard requires **one source-grounded UML class diagram for each declared type** and a dedicated page for each maintained type. That means every class, interface, struct, record, and enum should have its own focused class view and documented relationships, with repository-level inventory/coverage enforcement.

It does **not** mean generating every UML diagram family for every class. Sequence/communication diagrams require a scenario or interaction; activity diagrams require a workflow; state machines require defined states/transitions; use-case diagrams describe actors and system goals; component/deployment diagrams describe larger architecture. Generate these when their subject and evidence exist, and link them from relevant type pages. Do not manufacture diagrams merely to fill a checklist.

The current generator is only the first class-view slice. It currently uses syntax declarations and unresolved base-list references, does not resolve compiler symbols or draw semantic relationship connectors, and selects the first top-level type for the UML output. It must not be advertised as satisfying per-type inventory coverage yet.

### Consumer boundary and API contract

- **Runtime renderer contract:** GUI Core's `UmlClassDiagram` / `UmlMember` model and GUI.SVG's `SvgUmlClassDiagramRenderer` are the renderer-neutral model and SVG renderer currently present on the staging branch. Their availability on an unmerged branch is not a supported package contract.
- **Source-analysis boundary:** Roslyn parsing/projection belongs in the separate `samples/SourceUmlGenerator` tool, not in GUI Core or the GUI runtime renderer. The current source parser is a sample/tooling implementation, not a promise that consumers must add Roslyn to their runtime.
- **FSM_COS consumption:** FSM_COS should not bind to staging internals or assume the candidate exists in a published package. Until GUI's integration and package/API version are verified, keep existing Mermaid diagrams accurately labeled as manually authored and keep the UML release gate open.
- **Semantic UML still missing:** compiler-symbol resolution for inheritance/interface realization and member types; relationship model/connectors with honest cardinality/ownership semantics; explicit per-type target selection; complete source-to-type inventory and generated pages; mutation tests proving fixture changes alter output and malformed input fails closed; visual inspection and exact-SHA CI evidence.

### Work plan / acceptance

I will keep the candidate isolated from `development` until the staged work has been reconciled safely, then verify the exact integrated commit and workflows. Before calling the per-type documentation gate complete, require an inventory generated from actual production declarations, one page plus one class-diagram SVG per maintained type, symbol-grounded relationships (or clearly marked syntax-only interim output), drift/missing-artifact tests, accessibility and visual-bound checks, and exact commit/run/artifact provenance. This coordination note does not authorize merge, publication, master changes, branch deletion, or required-check changes.


## CI evidence update — exact staged candidate

I queried GitHub Actions for GUI candidate commit `1317c31f44cb0614ed845ea18fd7cb44cf0c7950`:

- [Source-derived diagrams run 39461444762](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39461444762): **success**. The generator ran, the SVG contract checks and committed UML parity check passed, and the generated artifact uploaded.
- [Package GUI run 39461444806](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39461444806): **failure** in `test-wpf`. The log says 22 WPF tests reported passed, then the test host aborted/crashed with `System.NullReferenceException` at `System.Threading.Thread.get_CurrentThread()` / `MS.Win32.HwndSubclass.SubclassWndProc`. The `test-and-package` job passed, including Core/Blazor/SVG tests and package verification; NuGet publication was skipped. This is not a clean green package workflow, and the WPF failure must be investigated rather than dismissed as harmless.
- Current compare after the coordination update: `work/gui-svg-source-uml-generator` is **28 commits ahead and 3 behind** GUI `development` (base now includes coordination commit `3c836977cc7a05579610e6babb6be9c45fc506e7`). It remains divergent staging work.

These results verify the source-derived diagram workflow on the candidate SHA, but do not prove fixture mutation behavior, semantic relationship correctness, per-type inventory coverage, or full repository release readiness.


## Follow-up verification — branch divergence and WPF host crash

**Observed:** 2026-10-20, after coordination commit `83478d443c589b6ff705f5c6f7f48c51ae8d691c`.

- Fresh compare reports `work/gui-svg-source-uml-generator` is **28 commits ahead / 4 behind** `development`, status `diverged`. The branch remains isolated staging work. [Compare](https://github.com/TrentBest/TheSingularityWorkshop.GUI/compare/development...work/gui-svg-source-uml-generator)
- [Package GUI run 39461444806](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39461444806) has two distinct outcomes: `test-and-package` succeeded, including Core/Blazor/SVG tests, package verification, WPF build, and package artifact upload; `test-wpf` failed, so the overall workflow is **failure** and `publish_nuget` was skipped.
- The WPF job log confirms the assembly compiled and the test runner reported 22/22 passed, but then the test host crashed and aborted with `System.NullReferenceException` in `System.Threading.Thread.get_CurrentThread()` called by `MS.Win32.HwndSubclass.SubclassWndProc`. A test summary saying 22 passed does not negate the runner crash or turn the job green.
- The log alone does **not** establish whether the crash is an existing runner/environment issue or a regression from this candidate. Do not suppress or dismiss it without a baseline comparison or a reproducible diagnosis. No workflow was rerun or altered as part of this investigation.

The source-derived diagram workflow remains green for candidate SHA `1317c31f44cb0614ed845ea18fd7cb44cf0c7950` ([run 39461444762](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39461444762)); that proves its current generation/parity checks only, not semantic UML completeness or release readiness. Next safe step is to compare the same WPF job against a known-good `development` commit/run, then separately continue explicit type targeting, source inventory, semantic relationships, and fixture mutation coverage. Do not merge, publish, alter master, delete branches, or weaken required checks without authorization.

## Follow-up implementation — explicit UML type targeting

**Observed:** 2026-10-20. The source-derived UML staging branch now supports `--type <name-or-namespace.name>` for choosing the class diagram declaration rather than silently always selecting the first top-level type.

- Generator commit: https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/580629e3175f00105803cda2eb41f0550dfef1fe
- CI invocation commit: https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/86ccbadfab918af2b953c413c55fe72f3bb43911
- Documentation commit: https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/2a0ca6cedfc8fe603e2a27eced52aaa8e21a3b63
- The workflow now invokes the generator with `--type DataCard`. The available workflow-run lookup returned no run for the latest candidate SHA, so this is implementation evidence, not a passing CI claim.

This is only the first missing capability closed. The generator still needs semantic symbol resolution and drawn relationships, deterministic per-type inventory/page generation, source-mutation fixtures, malformed-input tests, and rendered-artifact inspection. The candidate branch remains staging and divergent; do not treat these commits as integrated into `development` or as grounds to close the FSM_COS alpha.7 gate.


## Product architecture update — living UML documentation

**Update:** 2026-10-20. The GUI staging branch now contains a dedicated [Living UML Architecture](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/work/gui-svg-source-uml-generator/docs/LIVING_UML_ARCHITECTURE.md) document, and the source-derived diagram overview links to it. This is a design/acceptance contract, not evidence that the full pipeline has shipped.

The user clarified the product goal: developers should be able to generate UML for articles from their actual code, embed those diagrams in documentation, and have them updated when code changes. The proposed pipeline is source analysis (Roslyn) plus post-build reflection against the exact compiled domain, a versioned normalized documentation model (potentially under `docs/AST`), renderer-neutral UML projection, GUI.SVG rendering, per-type documentation pages, and deterministic regeneration/drift checks.

Important correctness boundary: reflection metadata is not an AST and cannot replace Roslyn's source locations, comments, and syntax-level intent. If `docs/AST` stores normalized JSON rather than actual syntax trees, name and document it as a source-analysis/documentation-model snapshot. SVG remains generated output, never the source of truth. Build-time analysis/reflection must stay outside GUI Core; runtime rendering should consume renderer-neutral models.

Acceptance is broader than a class-compartment SVG: project-wide deterministic type inventory, a per-type page and class view for every maintained type, symbol-resolved relationship connectors, evidence-driven sequence/state/activity/architecture views, source-embedded generated includes, mutation/stale-artifact tests, malformed-input fail-closed tests, accessibility/bounds/visual inspection, and exact-SHA CI evidence.

The staging branch remains unintegrated; do not treat the architecture document as proof of implementation or close the FSM_COS alpha.7 UML gate. Continue coordinating the intended consumer contract against verified GUI APIs and exact integrated commit evidence.


## Follow-up architecture — observable diagram models and console drivers

**Update:** 2026-10-20. The GUI UML staging branch now documents the complementary live-view idea in [Observable Diagram Models and Console-Driven Views](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/work/gui-svg-source-uml-generator/docs/OBSERVABLE_DIAGRAM_MODELS.md), linked from [Source-Derived Diagrams](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/work/gui-svg-source-uml-generator/docs/SOURCE_DERIVED_DIAGRAMS.md).

The user proposed the direction as **View => Observes => Data <= Modifies <= ConsoleApp**. Interpret this as an observable, authoritative model with a console driver/test harness and a renderer that observes consistent snapshots. A console app should be able to construct and mutate model data directly; Roslyn/reflection are optional producers for source-derived documentation, not prerequisites for every live visualization.

Implementation boundaries:
- Model/data owns semantic state, stable IDs, revisions, constraints, provenance, and validated mutations.
- View/renderer observes complete snapshots and projects them into diagram/SVG output.
- A host refreshes/replaces the displayed SVG when a new model revision is rendered. An SVG file/string alone is a static snapshot; it does not make itself reactive.
- Prefer immutable snapshots or equivalent synchronization; support coalescing obsolete revisions while guaranteeing the latest committed state is eventually rendered.
- The console driver should stress valid extremes and boundary cases through the public command path, asserting invariants and reporting diagnostics. It must not bypass validation or treat arbitrary SVG/XML mutation as model state.
- Prove notifications, rejected-command atomicity, observer disposal, deterministic rendering, large-model bounds, and one host-level end-to-end refresh before claiming live behavior.

This complements source-derived UML generation rather than replacing it. The current GUI staging code does not yet implement subscriptions, a live host refresh path, semantic relationship connectors, or the complete project-wide source/reflection pipeline. The staging branch remains unintegrated; the FSM_COS alpha.7 UML gate stays open until implementation and exact-commit evidence support closure.


## FSM_COS consumer contract response — proposed schema v1

**Update:** 2026-10-20. This is a concrete proposal for coordination, not a claim that the staging implementation or a public package already supports it. The FSM_COS alpha.7 UML gate remains open.

### Contract ownership and versioning

- The canonical interchange is a versioned, serializable **documentation model** (not a Roslyn AST, not reflection output, and not SVG).
- `schemaVersion` versions the serialized contract independently of NuGet/package versions. Consumers must reject unsupported major versions and must not silently interpret unknown relationship kinds as a known kind.
- The canonical model retains source facts and provenance. A renderer-neutral diagram projection selects nodes/edges and layout hints; GUI.SVG renders that projection. Layout coordinates are presentation data, never semantic identity.
- Proposed ownership: the stable public DTO/model contract belongs in GUI Core only after its invariants and compatibility surface are tested; SVG serialization/layout belongs in GUI.SVG; Roslyn compilation, source inventory, and post-build reflection belong in a separate build-time tool. The current `UmlClassDiagram` / `UmlMember` are staging types, not yet a supported consumer API. No package/version is promised until the model and compatibility policy are accepted and integrated.

### Example canonical source/documentation model

```json
{
  "schemaVersion": "1.0",
  "provenance": {
    "repository": "TrentBest/TheSingularityWorkshop.GUI",
    "sourceRevision": "<full git commit SHA>",
    "projectPath": "src/Core/TheSingularityWorkshop.GUI.Core.csproj",
    "assemblyIdentity": "TheSingularityWorkshop.GUI.Core, Version=<resolved>",
    "targetFramework": "net8.0",
    "configuration": "Release",
    "analysisMode": "roslyn-compilation",
    "builtAssemblySha256": "<sha256-or-null-if-not-built>"
  },
  "types": [{
    "id": "type:<assembly-id>:TheSingularityWorkshop.Workshop.Gui.DataCard",
    "identity": {
      "assembly": "<assembly identity>",
      "namespace": "TheSingularityWorkshop.Workshop.Gui",
      "containingTypes": [],
      "metadataName": "DataCard",
      "genericArity": 0,
      "kind": "class"
    },
    "sourceDeclarations": [{
      "path": "src/Core/DataCard.cs",
      "span": { "start": 0, "length": 1200, "startLine": 1, "startColumn": 1, "endLine": 40, "endColumn": 2 }
    }],
    "members": [{
      "id": "member:<type-id>:M:GetCollapsedSummary()",
      "kind": "method",
      "metadataName": "GetCollapsedSummary",
      "signature": "string GetCollapsedSummary()",
      "sourceDeclarations": [{
        "path": "src/Core/DataCard.cs",
        "span": { "start": 400, "length": 80, "startLine": 14, "startColumn": 5, "endLine": 16, "endColumn": 6 }
      }]
    }]
  }],
  "relationships": [{
    "id": "edge:<source-type-id>:implements:<target-type-id>",
    "kind": "interface-realization",
    "sourceTypeId": "<source type ID>",
    "target": {
      "typeId": "<resolved target type ID>",
      "displayName": "IExample",
      "resolution": "resolved",
      "scope": "project"
    },
    "direction": "source-to-target",
    "label": "implements",
    "multiplicity": null,
    "ownershipEvidence": null,
    "provenance": {
      "kind": "compiler-symbol",
      "sourcePath": "src/Core/Example.cs",
      "span": { "start": 20, "length": 10, "startLine": 2, "startColumn": 20, "endLine": 2, "endColumn": 30 }
    }
  }]
}
```

The values above illustrate field shapes only; placeholder identities/spans are not evidence about a real declaration. Production output must use actual compiler identities and exact source spans. Stable IDs are based on assembly/project identity plus metadata type identity (namespace, containing-type chain, metadata name and generic arity); member IDs add the canonical metadata signature including parameter/ref kinds and generic arity. Line/column/span are navigational provenance, not the sole identity basis. Paths are normalized repository-relative paths. Avoid timestamps and other volatile values in deterministic artifacts.

### Relationship semantics

The v1 relationship enum should be closed and typed: `inheritance`, `interface-realization`, `association`, and `dependency`. Edges are directed from the declaring/using type to the referenced target; presentation layout may place dependencies above the focus type, but must not reverse semantic direction. The target must carry an explicit resolution status such as `resolved`, `external`, or `unresolved`, and a scope/assembly identity when known.

- Inheritance and interface realization require resolved compiler symbols.
- Association/dependency must record the evidence kind and source member/operation that establishes the reference. A type appearing in a field or property signature alone does not prove ownership.
- Multiplicity is optional and only emitted when it can be justified from the member/collection shape and documented interpretation; do not pretend generic collection syntax proves domain cardinality.
- Aggregation/composition/ownership are **not default v1 edge kinds**. They require explicit human-authored or otherwise reviewable ownership evidence, including evidence source and review status.
- If compilation cannot resolve a symbol, preserve its spelling as an unresolved/external reference and render it as such. Never draw a resolved semantic connector from a matching name string.

### Human-authored use cases are a separate model

Use-case intent is not source-derived. Keep it in a separate authored document/model, linked to type IDs but never mixed into compiler-derived facts. A v1 record should contain:

```json
{
  "schemaVersion": "1.0",
  "id": "use-case:<stable-slug>",
  "title": "<actor goal>",
  "actor": { "name": "<actor>", "kind": "human|system|external-service" },
  "goal": "<observable outcome>",
  "trigger": "<initiating event>",
  "preconditions": [],
  "mainFlow": [{ "step": 1, "text": "<action/outcome>", "evidence": [] }],
  "alternateFlows": [{ "id": "A1", "condition": "<condition>", "steps": [], "evidence": [] }],
  "postconditions": [],
  "relatedTypeIds": [],
  "evidence": [{ "kind": "test|source|trace|decision|external", "uri": "<stable link>", "claim": "<what this supports>" }],
  "review": { "state": "draft|needs-review|accepted|rejected", "reviewer": null, "reviewedRevision": null },
  "ownership": { "owner": "<team/person/role>", "origin": "human-authored|llm-assisted", "assumptions": [] }
}
```

An LLM may help elicit and edit these records, but must mark assumptions and leave review state explicit. Do not generate fake use cases to meet diagram counts. Some low-level types should link to system-level use cases rather than invent actor-facing goals.

### Per-type artifacts and multi-declaration files

Use stable type IDs—not source filenames alone—as inventory identity. For each maintained type, generate a main `<TypeSlug>.md` page with purpose, local class card, member/contract notes, and source links, plus `<TypeSlug>UML.md` with the wider relationship diagram and authored use cases. A deterministic slug should include a namespace/containing-type disambiguator and generic arity where needed; collisions must fail generation rather than overwrite a page. Several declarations in one `.cs` file produce separate type records/pages/artifacts. Nested and partial declarations are merged by compiler symbol into one type identity with multiple source spans. A documented grouped-page exception must be explicit in the inventory, never silently assumed.

### Adoption gates for FSM_COS

FSM_COS should continue to use its clearly labeled interim Mermaid until all of the following are true:

1. GUI exposes a documented, tested model/API and schema version on the integrated `development` line and in a verified package; no dependency on staging internals.
2. Source-analysis tooling compiles the actual project/solution, emits stable inventory/provenance, and resolves semantic edges; unresolved symbols remain explicit.
3. Fixture tests cover source mutation, overloads/generics/nested/partial/multiple types, ambiguous names, malformed source and compilation failure; stale/missing/orphaned artifacts fail CI.
4. SVG tests cover XML, unique IDs, accessible title/description, bounds/text overflow, semantic edge direction/style and deterministic regeneration; rendered artifacts receive visual inspection.
5. Package/API compatibility, build/test/security/package workflows pass on the exact integrated GUI SHA, with run links and generated artifacts recorded. The known WPF test-host crash must be compared against baseline and resolved or diagnosed transparently, not hidden.
6. FSM_COS consumes only the verified package/API, regenerates its own artifacts deterministically, and records both GUI and FSM_COS source revisions. Its own exact candidate CI must pass; publication remains a separate release-owner decision.

This proposal does not close the gate, authorize a merge/publication, or claim current implementation completeness. The next implementation step is to add the versioned canonical DTOs and semantic compiler-backed extraction in the isolated GUI candidate, then prove them with fixtures before declaring a stable package boundary.


## Follow-up — schema foundation and source-fixture acceptance

**Update:** 2026-10-20. The staging branch `work/gui-svg-source-uml-generator` has progressed beyond the prior documentation-only proposal, while remaining isolated from `development`.

- Added `src/Core/UmlDocumentationModel.cs` on the staging branch. It defines schema-v1 DTOs for project/build/source-revision provenance, type/member metadata identity, source spans, typed relationship kinds and resolution status, relationship evidence, and a separate human-authored use-case model.
- Added canonical JSON serialization with camel-case fields, kebab-case string enums, unsupported-version rejection, unknown-property rejection, and validation for identity uniqueness, source spans, provenance, relationship endpoints, direction, and external/unresolved states.
- Added Core tests for serialization round-tripping, enum wire values, unsupported schema versions, unknown fields, invalid spans, and resolved edges pointing to missing target types.
- Added CI fixtures for source mutation, explicit fully qualified selection across multiple declarations, ambiguous simple-name failure, and malformed-source fail-closed behavior.

### Exact candidate evidence

- Source-derived diagrams run [39469739639](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39469739639) passed on candidate commit `2da63840d418fac811145ba8175edfd3b517f999`, including the new mutation/selection/malformed-source fixture step.
- Package GUI run [39469739655](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39469739655) was still in progress at this update; Core, Blazor, SVG and shared adapter test steps had passed, and the workflow was progressing through WPF/gallery/package verification. Do not call this full package workflow green until the final conclusion is success.

### FSM_COS consumer decision remains unchanged

The new DTOs are a tested **contract foundation**, not yet the supported package/API contract FSM_COS needs. The source generator still emits the existing DataCard/single-class UML projections; it does not yet serialize the canonical model, load a project/solution compilation for semantic symbol resolution, render multi-node semantic connectors, or generate per-type inventory/pages. The current UML class output still labels base-list references as unresolved syntax facts.

FSM_COS should keep the interim Mermaid diagrams and alpha.7 UML gate open. Next required implementation is compiler-backed source/project analysis feeding the canonical model, then typed multi-node relationship rendering and deterministic per-type outputs. Only after those stages are integrated into `development` with exact-SHA green build/test/security/package workflows and visual/bounds evidence should FSM_COS consider consuming a published GUI contract. No merge, publication, branch deletion, or check weakening is authorized by this update.
