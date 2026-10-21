# Datum, Presentation, and Interaction Boundary

## Purpose

The GUI layer is intended to be the human-facing semantic bridge between application-owned datum and a platform-specific presentation. This is an ownership principle, not a claim that every interaction or presentation contract has already been implemented.

A useful example is a horse in an application:

- The horse's identity, state, and domain meaning remain application-owned datum.
- A mesh, SVG silhouette, or native control is a manifestation.
- Labels, selection affordances, layout intent, and user-facing interaction meaning are presentation concerns when they can be expressed independently of a particular platform.
- A user action should be expressed as a semantic request; the host/domain system retains policy and authority to accept, reject, or execute it.

## Ownership boundary

```text
application / domain owns datum and policy
                 |
                 v
      semantic GUI presentation
                 |
                 v
       platform-specific adapter
                 |
                 v
          rendered manifestation
```

The GUI model should not become the owner of domain data merely because it displays that data. Similarly, a renderer should not redefine domain meaning merely because it draws the result.

## Physical input is a separate concern

Keyboard, mouse, touch, controller, gaze, voice, accessibility devices, and automation are different sources of physical or external observations. A portable GUI contract should consume semantic interaction rather than require every GUI consumer to understand every physical device.

A possible future boundary is:

```text
physical observation -> input/output adapter -> semantic interaction
                                                |
                                                v
                                          GUI target
                                                |
                                                v
                                     host/domain policy
```

A separate input/output package has been proposed as a possible home for device-independent interaction concepts. Its name, API, and implementation are **not established by this document**. No physical-input package is created here, and GUI.Core should not acquire a dependency on ProtocolAi or a device-specific library as a consequence of this architectural note.

## Protocol identity is not execution authority

Stable protocol identities may be useful for addressing semantic actions across components, including AI-driven clients. Identity alone does not determine policy or grant execution authority. The consuming host remains responsible for authorization, validation, and execution. Any ProtocolAi integration should be described as a proposal until its actual code and dependency direction are verified.

## Current implementation versus direction

The current Core architecture is a recursive `GuiNode` tree with identity, kind, optional text/source, semantic properties, and children. Platform adapters own native manifestation. Do not infer from this note that a complete semantic interaction vocabulary, device abstraction, universal data-binding model, or AI execution pathway already exists.

See [GUI Architecture](ARCHITECTURE.md), [GUI Model](GUI_MODEL.md), and [Platform Adapters](PLATFORM_ADAPTERS.md) for the implementation-facing contracts.

## Concrete examples preserved from the architecture review

The following examples make the ownership boundary more concrete; they are explanatory models, not claims that the corresponding APIs or packages already exist.

### Example: a rendered horse

- The horse and its authoritative state remain application-owned datum.
- Lines, curves, polygons, meshes, and textures are possible manifestations.
- Labels, displayed weight, dimensions, placement, sizing, selection, and interaction semantics describe how the GUI presents and exposes that datum.
- A user edit becomes a semantic request to the owning application or host; the GUI does not become the owner of the horse's state.

This example is deliberately platform-neutral: the same semantic presentation may be manifested as browser content, desktop controls, vectors, or 3D geometry.

### Presentation, manifestation, and rendering

Keep three responsibilities distinct:

`semantic presentation -> platform manifestation -> rendering`

For example, “show this object at this placement and display its weight” is a presentation request. An adapter maps that request into a platform-specific representation. The renderer produces the target platform's pixels, vectors, meshes, or native controls. Neither the adapter nor renderer should redefine the meaning or ownership of the datum.

### Physical input is not semantic intent

Keyboard, mouse, touch, controller, hand tracking, gaze, voice, accessibility devices, automation, and another software agent may all originate an equivalent semantic interaction. The conceptual flow is:

`physical/device observation -> input boundary -> semantic interaction -> GUI target -> domain request`

GUI consumes semantic interaction; it does not own physical device abstractions. An input/output package has been discussed as a possible future boundary, but no package name or API is frozen by this document.

### Common Core and capability bridges

Treat the common Core as the meaningful intersection of capabilities across supported platforms, not the union of everything any one platform can do. A semantic capability shared by only a subset of platforms may belong in an explicit bridge with implementations for those platforms, rather than being forced into Core or duplicated with inconsistent meaning. The exact capability sets and bridge contracts must be established from real implementations and conformance tests.

### Protocol identities do not grant execution authority

Stable semantic identities may help software clients address actions such as activate, select, focus, navigate, cancel, submit, drag, transform, or inspect. Those examples are application/domain vocabulary, not a universal GUI enum. A protocol identity can identify intent; it does not authorize or execute it. The consuming host remains responsible for validation, policy, authorization, and execution. No ProtocolAi dependency is introduced into GUI.Core by this architectural note.

