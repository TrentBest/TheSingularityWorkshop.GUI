# GUI Engineering Roadmap

The roadmap follows the Land of Idealism: define the semantic boundary first, prove it with tests, then add platform manifestations.

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

## Phase 3 — adapter conformance

- [x] Blazor contract tests
- [ ] WPF contract tests
- [ ] Shared adapter conformance cases
- [ ] Build a second materially different adapter
- [ ] Unity UI Toolkit adapter
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

## Phase 6 — integration

- [ ] Migrate WebPage domain builders
- [ ] Keep domain semantics outside the GUI repository
- [ ] Publish stable platform packages
- [ ] Document package/version compatibility
- [ ] Establish compatibility policy for GUI artifacts and micro-bundles
