# GUI Engineering Roadmap

The roadmap follows the Land of Idealism: define the semantic boundary first, prove it with tests and a real application, then add platform manifestations and runtime composition.

## Phase 0 — define the ideal

- [x] Define GUI as an extrinsic interface layer
- [x] Define manifest boundary
- [x] Distinguish GUI purpose from GUI platform
- [x] Define diagnostic GUI
- [x] Define operational GUI
- [x] Define informational GUI
- [x] Define spatial GUI
- [x] Define immersive GUI
- [x] Define expressive visualization
- [x] Define GUI/FSM_API relationship
- [x] Define the ephemeral GUI lifecycle
- [ ] Survey existing GUI systems and extract recurring semantic primitives
- [ ] Define the digital-shadow coverage principle
- [ ] Convert these principles into executable contracts
- [x] Define the 2D GUI spatial domain
- [x] Define the 3D extension and camera vocabulary
- [x] Identify the spatial contract surface
- [x] Define GUI as the outer execution boundary
- [x] Define the execution-host / manifestation distinction

## Phase 1 — establish the neutral model

- [x] Recursive GuiNode
- [x] Recursive GuiBuilder
- [x] Coordinate primitives
- [x] Core unit-test project
- [x] Architecture documentation
- [x] Adapter boundary documentation
- [ ] Remove or isolate remaining platform-shaped concepts from Core
- [ ] Define typed semantic property contracts
- [ ] Define stable identity rules
- [ ] Define capability terminology
- [ ] Define representation/media contracts
- [ ] Define semantic input/interaction contracts
- [ ] Define localization/internationalization contracts
- [ ] Define accessibility/alternate-presentation contracts
- [ ] Define lifecycle contracts

## Phase 2 — expressiveness before platform breadth

- [ ] Define a platform-neutral data-card/diagram contract (namespace/type/member hierarchy, header/body, stable identity, expansion state, and semantic edit actions); see [UML-Inspired Data Cards](docs/DATA_CARDS_AND_UML.md)
- [ ] Project a C# AST into ordered using-directive cards and namespace/type/member-group cards; test visibility-group ordering, expansion defaults, property details, and stable identity after visibility edits
- [ ] Keep code-definition/AST semantics in the code/creation domain rather than GUI Core
- [ ] Establish UML data-card visibility defaults and view/inspect/edit-mode affordances
- [ ] Generate an SVG living data-card diagram from a fixture shared with automated tests
- [ ] Verify that diagram edits cross an explicit domain command/validation boundary

The model must first prove that it can describe more than conventional widgets.

- [ ] Text and typography
- [ ] Images and media
- [ ] Geometry and 2D drawing
- [ ] Spatial layout
- [ ] Animation and transition intent
- [ ] Data/diagnostic presentation
- [ ] Operational interaction
- [ ] Viewport / scene intent
- [ ] 2D view/viewport contract
- [ ] Coordinate-space and transform contract
- [ ] Camera/observation contract
- [ ] Projection contract
- [ ] Visibility/depth/occlusion contract
- [ ] Hit-testing and interaction-routing contract
- [ ] Selection/focus semantics across views
- [ ] Multi-view synchronization
- [ ] 3D representation intent
- [ ] Deliberately unstructured visual representation
- [ ] Input abstraction independent of physical devices
- [ ] Accessibility intent
- [ ] Localization, Unicode, bidirectional text, and locale-aware presentation
- [ ] State/data observation and binding
- [ ] Spatial accessibility and alternate presentation
- [ ] Spatial performance/LOD hints
- [ ] Semantic geometry primitives and representation contracts
- [ ] Scene/subject hierarchy semantics without renderer ownership
- [ ] View/camera lifecycle and observation state transitions
- [ ] Cross-view identity, selection, focus, and synchronization invariants

## Phase 3 — adapter conformance and measurement

- [x] Blazor contract tests
- [x] Initial WPF renderer contract tests (Windows-native coverage for nested tree order, text, supported dimensions/spacing/colors/typography/alignment, null root, unknown-kind fallback, duplicate-ID rejection, the 256-edge depth guard, and exact semantic-ID preservation through a WPF attached property when `FrameworkElement.Name` cannot represent the ID; broader semantic coverage remains open)
- [x] Initial shared adapter conformance cases (identity, nested hierarchy, child order, and text; broader semantic coverage remains open)
- [x] Build a second materially different adapter (SVG)
- [ ] Unity UI Toolkit adapter
- [ ] Establish GUI benchmark methodology
- [ ] Establish executable GUI benchmark project and baseline — tracked by [GUI issue #2](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/2)
- [ ] Capability negotiation
- [ ] Adapter-specific manifestation tests

Candidate platforms are implementation targets, not semantic definitions:

- Web / Blazor
- WPF
- WinUI
- Unity UI Toolkit
- Avalonia
- .NET MAUI

## Phase 4 — deterministic GUI artifacts

- [ ] Serialization format
- [ ] Stable GUI manifests/artifacts
- [ ] Versioned GUI contracts
- [ ] Diagnostics for unsupported capabilities
- [ ] Golden-tree tests
- [ ] Deterministic reconstruction tests

## Phase 5 — runtime composition

- [ ] GUI micro-bundle contract
- [ ] Runnable experience contract
- [ ] Execution context contract
- [ ] GUI.Execute(...) execution contract
- [ ] GUI configuration contract
- [ ] Runtime observer model
- [ ] Execution lifecycle ownership
- [ ] External boundary adapter contract
- [ ] Multiple observers of one experience
- [ ] Hot replacement / reconfiguration
- [ ] Headless operation
- [ ] FSM_API integration boundary
- [ ] Input routing boundary
- [ ] Capability negotiation and degradation policy
- [ ] Semantic input contract
- [ ] Telemetry boundary and diagnostic stream

The intended order is deliberately conservative:

1. exercise the semantic model in the WebPage;
2. audit Core and adapters against the theory;
3. establish benchmark baselines;
4. make stable spatial contracts executable;
5. define the minimal execution contract;
6. add capability negotiation and semantic input.

See [Next Contract Surface](docs/NEXT_CONTRACT_SURFACE.md).

## Phase 6 — integration

The WebPage is the intended first practical proving ground for the architecture.

The original migration issue [WebPage #52](https://github.com/TrentBest/WebPage/issues/52) was closed on 2026-09-29. The current WebPage development project references the published GUI.Blazor package by default and can opt into the GUI repository project in repository-source mode. Issue closure and dependency references are not substitutes for end-to-end build/test evidence; keep the remaining integration checklist below unchecked until each criterion is demonstrated.

- [ ] Refactor the WebPage's reusable GUI construction onto TheSingularityWorkshop.GUI
- [ ] Keep WebPage domain semantics outside the GUI repository
- [ ] Demonstrate the Blazor adapter through the WebPage rather than through isolated GUI-only examples
- [ ] Use the WebPage as the ten-minute evaluation path from the GUI documentation
- [ ] Capture representative GUI reference artifacts from the working WebPage
- [ ] Keep the live WebPage manifestation explicitly separate from the semantic GUI contract
- [ ] Publish stable platform packages
- [ ] Document package/version compatibility
- [ ] Establish compatibility policy for GUI artifacts and micro-bundles
