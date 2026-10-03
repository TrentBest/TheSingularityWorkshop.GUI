# Datum, Presentation, and Interaction Boundary

## Purpose

GUI is the common human-facing bridge between **datum** and **non-datum presentation**.

Datum is application-owned information and state. GUI is the semantic layer that describes how a human observes, navigates, and requests changes to that datum.

A useful test is a rendered horse:

- the horse remains datum;
- lines, curves, polygons, meshes, and textures are manifestations;
- labels, displayed weight, dimensions, placement, sizing, selection, and interaction semantics belong to GUI;
- a user edit becomes a semantic request against the datum; GUI does not become the datum owner.

```text
                    DATUM
                      |
              application reality
                      |
                      v
              +---------------+
              |      GUI      |
              |               |
              | presentation  |
              | observation    |
              | interaction    |
              +-------+-------+
                      |
          semantic manifestation
                      |
        +-------------+-------------+
        |             |             |
      Blazor         WPF       3D / future
        |             |             |
     browser       desktop      native
```

## Presentation is not rendering

The useful boundary is:

`semantic presentation -> platform manifestation -> rendering`

For example:

- **GUI:** show the horse at this placement and display its weight.
- **Adapter:** translate that semantic request into the target platform.
- **Renderer:** draw the required pixels, vectors, meshes, or native controls.

A platform adapter may be sophisticated. The renderer may be sophisticated. Neither should redefine the meaning of the datum.

## GUI is about the user

GUI is a **human-facing service boundary** — a useful metaphor is a butler.

The butler:

- knows how to present what is available;
- understands the user's request;
- asks the appropriate system to change something;
- does not own the household's underlying assets.

GUI therefore mediates:

`datum <-> human`

while domain systems and hosts retain responsibility for state, policy, and execution.

## Input is used by GUI, but not owned by GUI

Physical input may come from:

- keyboard;
- mouse;
- touch;
- controller;
- hand tracking;
- gaze;
- voice;
- accessibility device;
- automation;
- another software agent.

GUI should not need to know which physical mechanism produced an interaction.

```text
physical/device observation
          |
          v
   input/output boundary
          |
          v
   semantic interaction
          |
          v
       GUI target
          |
          v
   domain request / datum change
```

This exposes a missing ecosystem boundary.

### Proposed package

Tentatively:

**TheSingularityWorkshop.FSM_InputOutput**

The name is provisional. The ownership boundary is the important part.

That package would define device-independent interaction concepts and common behaviors. Physical/device packages would translate platform observations into those concepts.

GUI consumes semantic interaction; it does not own the physical device abstraction.

## ProtocolAi connection

ProtocolAi provides a clean deterministic identity boundary for semantic interaction.

```text
physical signal
      |
      v
semantic intent
      |
      v
ProtocolAi identity
      |
      v
host policy / execution
```

An application could define identities for concepts such as activate, select, focus, navigate, cancel, submit, drag, transform, or inspect.

The exact vocabulary belongs to the application/domain, not to a universal GUI enum.

ProtocolAi can make those identities deterministic and addressable. It does **not** grant them authority to execute.

That preserves:

```text
ProtocolAi = WHAT
GrammarAi  = HOW
Host       = POLICY + EXECUTION
GUI        = HUMAN-FACING PRESENTATION + INTERACTION BRIDGE
```

This also gives AI systems a way to reference the same application-owned semantic identities without inventing physical-device vocabulary. It is an architectural opportunity, not a claim that ProtocolAi makes a model deterministic.

## The common GUI is an intersection

Let each platform's semantic capability set be:

`C₁, C₂, ..., Cₙ`

The common Core should be derived from the meaningful intersection:

`Core ≈ ⋂ Cᵢ`

This does not mean platforms have identical implementations. It is a rule for deciding what belongs in the common semantic layer.

### Shared but non-universal concepts

Suppose Blazor and WPF both have a meaningful concept called **Sparkles**, while another platform does not.

Do not force Sparkles into Core merely because two platforms implement it.

Instead:

```text
             GUI.Core
                 |
          +------+------+
          |             |
      Sparkles       other bridge
          |
    +-----+-----+
    |           |
 Blazor        WPF
    |           |
 implementation implementation
```

The bridge owns the shared semantic meaning. Each platform implementation may then extend it with native capabilities.

Thus:

`Platform = Core + applicable bridges + platform-specific extension`

## The minimum-behavior principle

The objective is not the smallest GUI API.

It is:

> **the smallest semantic behavior sufficient to preserve meaning across the domains that actually share that behavior.**

A concept belongs in Core when its meaning survives across all target domains in the common set.

A concept belongs in a bridge when its meaning survives across a meaningful subset.

A concept belongs in a platform package when its meaning is inherently tied to that platform.

This is the architectural equivalent of solving for the minimum sufficient common behavior.

## Ownership summary

| Concern | Owner |
|---|---|
| Datum / domain state | domain/application |
| Semantic presentation | GUI.Core / GUI bridges |
| Physical input | input/output adapters |
| Semantic interaction identity | input/output + application protocol |
| Deterministic symbol identity | ProtocolAi |
| Structural composition of commands | GrammarAi |
| Policy and execution | host |
| Native manifestation | GUI platform package |
| Runtime composition | FSM_COS |
| Rendering implementation | platform/rendering layer |

## What this changes in GUI.Core

GUI.Core should become richer semantically, not larger indiscriminately.

The next GUI contracts should focus on:

1. representing datum without owning datum;
2. semantic views and observations;
3. presentation properties such as placement, dimensions, sizing, and labeling;
4. modification intent without physical input ownership;
5. stable identity across changing representations;
6. bridge contracts for concepts shared by subsets of platforms;
7. conformance tests proving that platform adapters preserve meaning.

Input device types, WPF events, Blazor event callbacks, Unity input APIs, and similar mechanisms remain outside Core.

## Status

**Established direction:** GUI is the datum ↔ human bridge.

**Established boundary:** GUI consumes interaction; it does not own physical input.

**Proposed package:** FSM_InputOutput or a semantically equivalent interaction boundary.

**Proposed mathematical rule:** Core is the intersection of common semantic capabilities; bridges represent meaningful shared subsets; platform packages provide native extensions.

**Not yet frozen:** concrete APIs and package naming for the input/output boundary.
