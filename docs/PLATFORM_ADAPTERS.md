# Platform Adapter Guide

![The engineering boundary](assets/gui-engineering-boundary.jpg)

*Platform adapters are the controlled passage from semantic GUI intent to native manifestation.*

A platform adapter consumes the neutral GUI model and materializes it for a target UI technology. That definition has an important consequence: **a platform-specific builder library is not automatically an adapter for `GuiNode`.** The model-to-platform bridge must exist in code and have tests.

## Current implementation status

| Path | Current evidence | Boundary |
| --- | --- | --- |
| Core → Blazor | `BlazorGuiRenderer` consumes `GuiNode`; Blazor render-tree tests exercise output. | Prototype renderer; property forwarding and semantic coverage are not yet a finalized cross-platform contract. |
| Core → SVG | `SvgGuiRenderer` consumes `GuiNode`; SVG tests and the generated feature gallery cover scene serialization. | SVG scene vocabulary is explicit; generic unknown node kinds fall back to SVG groups and place node text in a title, not a general-purpose native widget layout. |
| WPF builders → WPF | `src/WPF` contains reusable native WPF builders and configuration. CI builds the WPF project. | A tested `GuiNode`-to-WPF bridge and WPF contract-test suite have not yet been established. |
| Unity UI Toolkit | No adapter implementation in this repository. | Candidate future target only. |

Treat these as distinct evidence levels. A project build establishes that the project compiles under that workflow; it does not establish semantic parity with another adapter or prove an end-to-end application integration.

## Adapter responsibilities

An adapter should:

1. consume Core types;
2. map known semantic node kinds;
3. translate supported properties intentionally;
4. preserve child order;
5. report or handle unsupported capabilities explicitly;
6. own platform resources and event lifetimes;
7. avoid changing domain meaning.

## Do not use blind property passthrough

A renderer should not assume that every Core property is a native attribute.

For example, a future Core property named `layout.horizontalAlignment` should be translated by each adapter according to its own platform rules. It should not become an HTML attribute merely because HTML permits arbitrary attributes.

The current Blazor renderer contains a deliberately simple compatibility implementation. It forwards node properties as render attributes and maps a small set of known kinds to HTML tags. This is useful prototype behavior, not the final cross-platform contract.

Its recursive render-tree builder uses source-stable sequence numbers: each invocation of the renderer method starts at the same local sequence positions instead of carrying a global counter through the tree. The Blazor conformance test pins this behavior because sequence numbers guide framework diffing; they are not node IDs or a runtime count.

The SVG renderer has a separate explicit SVG scene vocabulary. It can serialize SVG-specific nodes and coordinate-system scenes, but a generic Core tree is not automatically equivalent to an interactive widget tree merely because it can be serialized as SVG.

## Capability differences

Platforms are not identical.

An adapter may support more capabilities than another adapter. The correct response is not to contaminate Core with platform-specific APIs.

Instead, capabilities should eventually be:

- declared;
- negotiated;
- degraded predictably; or
- rejected with a useful diagnostic.

## Testing adapters

Adapter tests should have two levels:

### Contract tests

Verify that the same Core tree produces the required semantic result. Shared conformance cases should check only explicit invariants—such as stable identity, child order, text preservation, and hierarchy—not equality of native element types.

### Platform tests

Verify native behavior that cannot be represented by Core alone, including framework lifecycle, event wiring, resource ownership, accessibility integration, and native layout.

Blazor and SVG have adapter-specific suites, and the initial shared conformance project runs the same small semantic tree through both adapters to compare identity, child ordering, and text preservation. The shared suite is intentionally narrow: it does not establish general widget parity or cover every node kind. It verifies that both renderers preserve `GuiNode.Id` when a properties dictionary also contains an `id` entry, reject duplicate node IDs within a rendered tree, and reject trees deeper than 256 edges from the root. Core construction itself remains permissive; identity uniqueness and this rendering depth guard are adapter-boundary rules. Blazor's generic property forwarding skips the reserved `id` key case-insensitively so casing variants cannot add a conflicting identity attribute. WPF contract tests and broader shared conformance cases remain open.

This keeps platform behavior testable without making the Core model platform-aware.
