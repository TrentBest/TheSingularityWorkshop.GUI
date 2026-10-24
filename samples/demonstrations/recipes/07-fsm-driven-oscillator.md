# Recipe 07 — FSM-driven bounded oscillator

> A value reaches the end of its range, turns around, and comes back. No teleportation. No mysterious reset. Just a state machine doing exactly what we told it to do.

**Status: implementation planned; not yet a verified end-to-end proof.** This recipe defines the smallest useful bridge from FSM_API execution to a generated GUI.SVG artifact. It must not be presented as implemented until a runnable consumer, generated SVG, and deterministic tests exist.

## What the reader should understand

A function is defined over a bounded input range:

\[ x_{min} \leq x \leq x_{max}, \qquad y = f(x) \]

The oscillator moves through that range in discrete steps. At the maximum it reverses direction; at the minimum it reverses again. It does not snap back to the beginning. A plotted curve shows `f(x)`, while a marker shows the current `(x, f(x))` sample and an arrow or label shows direction.

The lesson is the behavior, not the animation trick: **FSM_API advances the state; the SVG only portrays the state it is given.**

## Intended architecture

```mermaid
flowchart TD
    T[Explicit tick / chosen step] --> F[FSM_API oscillator state machine]
    F --> S[Current x and direction]
    S --> M[Evaluate f(x)]
    M --> D[Neutral scene data: curve, marker, labels]
    D --> G[GUI Core semantic scene]
    G --> V[GUI.SVG renderer]
    V --> A[Generated SVG snapshot]
    A --> H[Host displays successive snapshots]
```

The state machine is the source of truth. A host may advance it on a timer, a simulation tick, or a test-controlled step. The host then reads the resulting state and updates or regenerates the SVG. Declarative SVG animation alone is not evidence that FSM_API drove the movement.

## State and transition contract

The minimal state is:

- `Minimum` and `Maximum`, with `Minimum < Maximum`;
- current `Value` in the inclusive interval;
- `StepMagnitude > 0`;
- direction, either forward or reverse;
- the chosen function `f(x)` and its domain constraints.

Each tick computes the next value. If the proposed step reaches or crosses an endpoint, set the value to that endpoint and reverse direction. Do not overshoot, wrap, reset, or lose the remaining state. The next tick moves inward. This handles ranges whose width is not evenly divisible by the step.

The exact endpoint policy must be explicit and tested. The initial version should use endpoint clamping and reverse on the same tick; it should not attempt to preserve overshoot distance unless a later recipe specifically teaches that behavior.

## Evidence required before marking implemented

- [ ] A runnable sample references the real FSM_API and GUI Core / GUI.SVG projects or packages it actually consumes.
- [ ] The FSM's transitions advance and reverse the value; no separate animation timeline duplicates the logic.
- [ ] The SVG curve and marker are generated from the same current state and function samples.
- [ ] A deterministic test verifies forward motion, maximum reversal, reverse motion, minimum reversal, and repeated cycles.
- [ ] Tests cover a step that does not divide the range evenly, a step larger than the range, invalid bounds, and invalid step magnitude.
- [ ] A recorded execution trace or assertions prove the state transitions; generated SVG/XML assertions prove the manifestation reflects that state.
- [ ] The generated artifact is visually reviewed and regenerated in CI.
- [ ] Documentation identifies what is live, what is a snapshot, and what the host is responsible for advancing.

## Implementation boundary

The existing GUI.SVG adapter serializes a semantic scene into SVG. It does not own a timer, invoke FSM_API transitions, or maintain live application state. The sample must supply the state-to-scene bridge and the host's stepping policy. Keep Unity-specific types out of the sample's core logic; the older `FSM_UnityIntegrationAdvanced` project is inspiration for the oscillator behavior, not a dependency to import into this cross-platform example.

Related reading:

- [GUI.SVG adapter contract](../../../docs/SVG_ADAPTER.md)
- [Recipe 02 — function transformations](02-coordinate-functions.md)
- [Recipe 04 — declarative SVG animation](04-animated-scene.md)
- [Demonstration roadmap and evidence policy](../README.md)

## A small rule for the larger documentation effort

Show the concept first, explain the implementation second, and state the evidence boundary plainly. If a picture is worth a thousand words, a picture whose state changes for a reason is worth even more.