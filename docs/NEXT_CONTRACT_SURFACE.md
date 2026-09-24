# Next Contract Surface

This document records the next architectural contracts to make precise before turning the richer execution model into Core interfaces.

The purpose is to resist a common failure mode: implementing an attractive interface before the semantic boundary it is supposed to protect is understood.

The current architecture points toward four related areas:

1. execution;
2. capability negotiation;
3. boundary input and telemetry;
4. conformance and audit.

They should be developed in that order of dependency, but they should not become one giant runtime abstraction.

## 1. Execution: define the semantics before the interfaces

The desired usage remains:

    Gui.Execute(app)

The important idea is not the static method. The important idea is that the GUI boundary can host a runnable experience.

The minimal semantic model should answer:

- What qualifies as a runnable experience?
- Who owns its lifetime?
- What information is available during execution?
- How is suspension different from termination?
- Can an experience be replaced without replacing its identity?
- What is observable by an execution host?
- How are failures reported?
- Can execution exist without a visual manifestation?

A useful conceptual separation is:

    Runnable Experience
            |
            v
    Execution Context
            |
            v
    Execution Host
            |
            +---- observation
            +---- input boundary
            +---- capability resolution
            +---- manifestation
            +---- lifecycle

### Important design constraint

Do not make Core responsible for a platform's task scheduler, UI synchronization context, browser event loop, WPF dispatcher, or renderer lifecycle.

Core should describe semantic lifecycle state.

A host may implement that lifecycle asynchronously or synchronously.

That means the first contract should probably describe states and transitions rather than immediately exposing task primitives, cancellation-token ownership, dispatcher types, or framework callbacks.

For example, the semantic lifecycle vocabulary may eventually include:

    Created -> Running -> Suspended -> Running -> Terminated
                        \-> Failed

The exact state machine remains proposed until its invariants are written and tested.

## 2. Capability negotiation: capabilities are facts, not wishes

Micro-bundles should not assume that a host can manifest every capability they describe.

The semantic problem is:

    Experience requirements
            |
            v
    Capability resolution
            |
            +---- available
            +---- adaptable
            +---- degradable
            +---- unavailable
            |
            v
    Execution plan

A capability should have enough identity to answer:

- what capability is being requested;
- what version or contract is required;
- whether it is required or optional;
- whether an adapter can provide it;
- whether degradation is permitted;
- what fallback preserves meaning;
- why a capability could not be satisfied.

### The key invariant

**Degradation must not silently change meaning.**

If a 3D experience is manifested in a 2D environment, the system may legitimately provide a 2D observation if the experience declares that as an acceptable representation.

It should not quietly pretend that an unsupported 3D capability was provided.

This makes capability negotiation part of representational compatibility rather than merely package dependency resolution.

## 3. Input and telemetry are related, but not the same

The physical boundary produces observations.

Examples include:

- mouse movement;
- touch;
- keyboard input;
- controller state;
- gaze;
- voice;
- device telemetry.

The semantic GUI should not need to know whether a selection originated from a mouse click or a VR controller trigger.

The boundary should therefore move toward:

    physical/device observation
            |
            v
    platform adapter
            |
            v
    semantic interaction
            |
            v
    interaction target
            |
            v
    experience action

### Semantic interaction

Candidate concepts include:

- activate;
- select;
- focus;
- point;
- drag;
- transform;
- navigate;
- cancel;
- submit.

These are examples, not yet a final enum.

The contract must preserve enough context for spatial interactions without importing platform-specific event types.

### Telemetry is different

Telemetry is observation of the system or environment, not necessarily a request to act.

It may include:

- pointer position;
- device state;
- sensor values;
- frame timing;
- diagnostics;
- execution metrics.

Input asks the experience to do something.

Telemetry tells an observer something about what is happening.

Keeping these streams conceptually distinct prevents an interaction event model from becoming a dumping ground for every external signal.

## 4. Spatial input is where the contracts meet

The spatial model already establishes:

    physical input
        -> platform coordinates
        -> semantic coordinate space
        -> hit test
        -> interaction target
        -> semantic action

This creates an important dependency order:

1. stable identity;
2. coordinate-space semantics;
3. view/viewport semantics;
4. hit-testing semantics;
5. interaction semantics;
6. input adapter.

Without those foundations, a generic input interface risks becoming a thin wrapper around mouse events rather than a platform-neutral interaction model.

## 5. Audit the existing code before expanding Core

Before introducing the execution interfaces, audit the current implementation against the theory.

The audit should look for:

### Core leakage

- platform namespaces;
- renderer-specific properties;
- DOM/XAML concepts;
- platform event delegates;
- assumptions that text is English;
- assumptions that a panel is the only presentation surface.

### Identity leakage

- IDs derived from display text;
- coordinates used as identity;
- replacing a representation accidentally replacing a subject;
- view identity confused with subject identity.

### Lifecycle leakage

- renderer lifecycle assumed to be experience lifecycle;
- adapter construction assumed to mean execution;
- disposal of a manifestation assumed to mean termination of the experience.

### Adapter leakage

- semantic decisions made only because a target platform happens to support them;
- unsupported features silently ignored;
- renderer-specific properties promoted into Core.

### Spatial leakage

- panel treated as view;
- view treated as camera;
- camera treated as graphics-engine object;
- 3D concepts introduced without a 2D semantic counterpart.

The audit should produce concrete findings, not a generalized cleanliness score.

### Current audit snapshot

A targeted review of the current Core source on development found no direct references to WPF, Blazor, Unity, DOM, HTML, or XAML in the Core search surface.

That is a useful boundary result.

The more immediate architectural gap is not platform leakage. It is semantic richness: the current GuiNode/GuiBuilder model still represents text, source, and properties primarily as strings, while the roadmap calls for typed semantic properties, stable identity rules, localization, accessibility, and richer spatial contracts.

That is important because it tells us **what not to fix prematurely**. We do not need to introduce execution interfaces merely to make the current Core look more abstract. We need to increase semantic precision where the existing model is genuinely stringly typed.

## 6. The contract should be layered

The emerging architecture is better represented as several small contracts than one universal GUI runtime interface:

    Semantic GUI
        |
        +-- identity
        +-- representation
        +-- spatial observation
        +-- interaction
        +-- lifecycle
        |
        v
    Execution boundary
        |
        +-- capabilities
        +-- input adapters
        +-- telemetry
        +-- manifestation
        |
        v
    Platform host

This preserves the repository's existing principle:

> Meaning belongs in Core; mechanisms belong at the boundary.

It also leaves room for a host that is visual, headless, automated, accessibility-oriented, or otherwise nontraditional.

## 7. Recommended implementation sequence

### First — WebPage proving slice

Make the real Blazor application consume the semantic GUI model.

This is the most valuable next implementation because it exercises the boundary against an actual application rather than an invented sample.

### Second — audit

Audit Core and the existing Blazor/WPF adapters against the now-visible contracts.

The audit should identify actual leaks and missing invariants before new interfaces are introduced.

### Third — benchmark baseline

Build the benchmark project from the WebPage workload.

This gives the project a factual performance baseline before richer runtime machinery is added.

### Fourth — executable spatial contracts

Turn stable parts of the 2D/multi-view model into tests:

- identity survives movement;
- multiple views can observe one subject;
- camera changes do not change subject identity;
- view replacement preserves subject identity;
- semantic input can traverse a viewport to an interaction target.

### Fifth — execution contract

Only after the above should the minimal Gui.Execute contract be promoted from architecture to Core API.

### Sixth — capability negotiation and boundary input

Add capability resolution and semantic input once execution and spatial semantics give them a stable home.

This order reduces the risk of designing runtime interfaces around unresolved concepts.

## 8. What remains intentionally unresolved

- Should execution return a session/handle, a result, or both?
- Is lifecycle state itself an FSM_API concern, a GUI concern, or a host concern?
- How are capability versions represented?
- What is the smallest useful degradation policy?
- How should failures distinguish unavailable capability, invalid experience, adapter failure, and experience failure?
- How much temporal information belongs in semantic input?
- Which telemetry is semantic and which belongs only to diagnostics?
- How should multiple observers coordinate without becoming coupled?
- What is the minimum spatial contract needed before 3D can be executable?
- Which performance hints belong in Core and which belong entirely to adapters?

These are design questions, not missing implementation details.

## Status

**Specified:** architectural direction and dependency order.

**Proposed:** executable contracts and concrete interfaces.

**Unresolved:** exact API shapes where introducing an interface now would freeze semantics prematurely.
