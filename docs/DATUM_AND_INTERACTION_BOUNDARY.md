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
