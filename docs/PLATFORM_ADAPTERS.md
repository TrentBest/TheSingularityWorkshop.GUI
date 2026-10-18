# Platform Adapter Guide

![The engineering boundary](assets/gui-engineering-boundary.jpg)

*Platform adapters are the controlled passage from semantic GUI intent to native manifestation.*

A platform adapter consumes the neutral GUI model and materializes it for a target UI technology. That definition has an important consequence: **a platform-specific builder library is not automatically an adapter for `GuiNode`.** The model-to-platform bridge must exist in code and have tests.

## Current implementation status

| Path | Current evidence | Boundary |
| --- | --- | --- |
| Core → Blazor | `BlazorGuiRenderer` consumes `GuiNode`; Blazor render-tree tests exercise output. | Prototype renderer; property forwarding and semantic coverage are not yet a finalized cross-platform contract. |
| Core → SVG | `SvgGuiRenderer` consumes `GuiNode`; SVG tests and the generated feature gallery cover scene serialization. | SVG scene vocabulary is explicit; generic unknown node kinds fall back to SVG groups and place node text in a title, not a general-purpose native widget layout. |
| Core → WPF | `WpfGuiRenderer` consumes `GuiNode`; Windows CI runs native renderer tests for hierarchy/order, identity and text, supported layout/color properties, null-root rejection, and unknown-kind fallback. | Initial renderer contract coverage now exists. It does not establish parity with Blazor/SVG or cover the full node/property/lifecycle surface. |
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

That difference is observable today: a `data-note` property is forwarded by Blazor, while the SVG renderer ignores it unless it is part of the SVG node-specific property mapping or the explicit presentation/accessibility attribute allowlist. Common names such as `class` and `fill` are supported in both paths, but that does **not** mean the property dictionaries have shared semantics. The shared conformance test records this current adapter-specific behavior as evidence, not as a finalized parity requirement.

| Model field | Blazor prototype | SVG renderer |
| --- | --- | --- |
| `Id` | Emits semantic HTML `id`; property keys equal to `id` case-insensitively are ignored. | Emits semantic SVG `id`; generic property passthrough is not used. |
| `Source` | Emits `src` on the mapped HTML element, regardless of node kind. | Used as an `svg-image` `href` fallback; other kinds do not automatically receive it. |
| `Text` | Emits text content inside the HTML element. | Used according to SVG node kind; an unknown kind places it in a `title`, not visible widget text. |
| `Properties` | Forwards entries as render attributes, except the reserved identity key. | Maps only recognized node-specific properties plus the presentation/accessibility allowlist. Unknown properties may be omitted. |
| `Children` | Emits children in model order. | Emits children in model order, subject to the SVG vocabulary and fallback behavior. |

This table describes implementation, not a recommendation to preserve every behavior forever. The next contract step is to define which properties are semantic Core concepts, which are target-specific extensions, and how unsupported properties should be reported. Do not infer that arbitrary HTML attributes, SVG scene attributes, or interactive event wiring are portable Core capabilities.

Its recursive render-tree builder uses source-stable sequence numbers: each invocation of the renderer method starts at the same local sequence positions instead of carrying a global counter through the tree. Optional source/text fields and property/child loops must not shift later sequence positions as runtime data changes; the property loop therefore uses one stable sequence position for each emitted property. Blazor conformance tests pin the recursive element positions and verify that source, multiple properties, and text retain their expected positions. Sequence numbers guide framework diffing; they are not node IDs or a runtime count. This still does not prove complete interactive diff correctness for arbitrary tree mutations.

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

Blazor and SVG have adapter-specific suites, and the shared conformance project runs the same small semantic tree through both adapters to compare identity, child ordering, and text preservation. That shared suite is intentionally narrow: it does not establish general widget parity or cover every node kind. It verifies that both renderers preserve `GuiNode.Id` when a properties dictionary also contains an `id` entry, reject duplicate node IDs within a rendered tree, and reject trees deeper than 256 edges from the root. Core construction itself remains permissive; identity uniqueness and this rendering depth guard are adapter-boundary rules. Blazor's generic property forwarding skips the reserved `id` key case-insensitively so casing variants cannot add a conflicting identity attribute.

The WPF test suite runs on `windows-latest`, because its controls and renderer require WPF. It covers a representative nested tree and child order, supported dimensions/spacing/colors/typography/alignment, null-root rejection, unknown-kind fallback, duplicate-ID rejection, and the 256-edge depth guard. WPF assigns `GuiNode.Id` to native `FrameworkElement.Name` only when the value is a legal WPF name. To preserve the model identity without imposing WPF constraints on Core or consuming `FrameworkElement.Tag`, the adapter also stores the exact ID in the WPF-specific attached property `WpfGuiIdentity.NodeIdProperty`, readable through `WpfGuiIdentity.GetNodeId(element)`. Thus IDs such as `widget:save/primary` remain available even though the native `Name` is empty. This is an adapter-specific bridge, not a new Core dependency or a promise that WPF name-based lookup can use arbitrary IDs. Broader property mappings, interaction/event behavior, accessibility, and resource lifecycle remain open. Do not treat the current suite as full cross-adapter conformance.

This keeps platform behavior testable without making the Core model platform-aware.
