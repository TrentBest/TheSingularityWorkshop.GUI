# GUI Engineering Roadmap

The repository currently contains the first recursive model and two platform implementations. The next phase is contract engineering.

## Phase 1 — establish the neutral model

- [x] Recursive GuiNode
- [x] Recursive GuiBuilder
- [x] Coordinate primitives
- [x] Core unit-test project
- [x] Architecture documentation
- [x] Adapter boundary documentation
- [ ] Remove or isolate platform-shaped concepts from Core
- [ ] Define typed semantic property contracts
- [ ] Define stable identity rules
- [ ] Define capability terminology

## Phase 2 — make builders composable

- [ ] Builder interfaces/factories
- [ ] Explicit composition contract
- [ ] Reusable semantic controls
- [ ] Layout intent model
- [ ] Interaction intent model
- [ ] Accessibility intent model

## Phase 3 — adapters

- [x] Blazor contract tests
- [ ] WPF contract tests
- [ ] Unity UI Toolkit adapter
- [ ] Shared adapter conformance cases
- [ ] Capability negotiation

## Phase 4 — deterministic GUI artifacts

- [ ] Serialization format
- [ ] Stable manifests
- [ ] Versioned GUI contracts
- [ ] Diagnostics for unsupported capabilities
- [ ] Golden-tree tests

## Phase 5 — integration

- [ ] Migrate WebPage domain builders
- [ ] Keep domain semantics outside the GUI repository
- [ ] Publish stable platform packages
- [ ] Document package/version compatibility
