# GuiBuilder UML

**Author:** Trent Best  
**Focal type:** `TheSingularityWorkshop.Workshop.Gui.GuiBuilder`  
**Diagram status:** manually authored and source-reviewed static SVG; regeneration is not automated.

## Target-centered neighborhood

![Workshop target-centered UML neighborhood for GuiBuilder](../assets/uml/classes/Core/GuiBuilder.svg)

The focal `GuiBuilder` is centered. Verified direct consumers appear above it; the immutable `GuiNode` result and private nested `NodeBuilder` construction state appear below; .NET base-library references appear to the right. The diagram uses the Workshop's recursive card language and labeled type-conduit stubs. Matching semantic type identities share a palette token, and receiving stubs are rotated 180 degrees. Text labels and type IDs reinforce color.

This image is static. It does not claim click-to-expand behavior, an observed execution trace, or a live FSM/GUI runtime. The fuller member contract and source/test links are in the [GuiBuilder type reference](../api/Core/GuiBuilder.md).

## Use cases

### 1. Compose a reusable semantic interface tree

- **Actor:** application or GUI builder author.
- **Goal:** describe a recursive interface tree without binding the domain model to WPF, Blazor, or SVG.
- **Trigger / preconditions:** the author has stable node IDs and node kinds for the intended interface.
- **Main flow:** call `GuiBuilder.Create`; set text, image references, and string properties; add configured child builders; call `Build()`.
- **Success outcome:** an immutable `GuiNode` snapshot preserves node identity, properties, and child order.
- **Alternative / failure:** invalid blank identity or property keys are rejected; a cyclic builder graph causes `Build()` to throw `InvalidOperationException`.
- **Evidence:** [source](../../src/Core/GuiBuilder.cs) · [tests](../../tests/Core.Tests/GuiBuilderTests.cs).
- **Status:** source- and test-backed for the behavior listed above.

### 2. Declare an action without granting execution authority

- **Actor:** application author and host command dispatcher.
- **Goal:** carry a stable action identity across the renderer boundary while keeping execution authority in the host.
- **Trigger / preconditions:** the host owns an explicit allow-list or command map.
- **Main flow:** declare `WithOnClick(commandId)`; build the node; let a capable adapter/host recognize the command identity; validate it against the host's allowed operations before dispatch.
- **Success outcome:** the semantic node contains `onClickCommand`; the host remains the authority for dispatch.
- **Alternative / failure:** blank IDs are rejected. An unrecognized or unauthorized command must not execute; Core itself does not perform the dispatch.
- **Evidence:** [builder source](../../src/Core/GuiBuilder.cs) · [command-identity test](../../tests/Core.Tests/GuiBuilderTests.cs) · [WPF semantic command boundary](../../src/WPF/WpfGuiRenderer.cs).
- **Status:** declaration and host-boundary intent are documented; this page does not claim every adapter implements identical dispatch behavior.

### 3. Adapt a coordinate-space description into the semantic tree

- **Actor:** author of a coordinate-space visualization.
- **Goal:** represent axes, grid, origin, bounds, and points as semantic nodes while reusing the general builder.
- **Trigger / preconditions:** a valid `CoordinateSystem` and root identifier are available.
- **Main flow:** create `CoordinateSystemGuiBuilder`, configure visibility and points, then call `Build()`.
- **Success outcome:** a `GuiNode` tree records coordinate-space properties and point children.
- **Alternative / failure:** coordinate validation can reject values outside the configured range; this is a data-contract failure, not a rendering failure.
- **Evidence:** [CoordinateSystemGuiBuilder source](../../src/Core/CoordinateSystemGuiBuilder.cs) · [CoordinateSystem tests](../../tests/Core.Tests/CoordinateSystemTests.cs).
- **Status:** source-backed usage path; platform-specific manifestation remains the responsibility of the selected adapter.

## Relationship semantics

- `GuiBuilders` is a convenience factory that calls `GuiBuilder.Create`.
- `CoordinateSystemGuiBuilder` delegates construction to `GuiBuilder`.
- `GuiBuilder.Build()` returns a `GuiNode` snapshot and uses private nested `NodeBuilder` state during construction.
- .NET base-library types support callbacks and iterative collections; they are framework references, not separate NuGet package dependencies.
- A relationship in this diagram does not imply composition unless the source/contract establishes ownership. The private `NodeBuilder` is implementation detail; `GuiNode` is the immutable output contract.
