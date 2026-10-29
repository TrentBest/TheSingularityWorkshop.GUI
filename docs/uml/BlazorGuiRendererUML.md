<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# BlazorGuiRenderer — UML and use cases

**Purpose:** document the boundary that turns a renderer-neutral `GuiNode` tree into Blazor render instructions.

## UML class diagram

![Compiler-derived class relationship diagram for BlazorGuiRenderer](source-files/Blazor/TheSingularityWorkshop-Workshop-Gui-BlazorGuiRenderer-43313af0a7.svg)

The focal class consumes Core's `GuiNode`; its output type is Blazor's `RenderFragment`, and its implementation uses `RenderTreeBuilder`. The linked diagram is compiler-derived structural evidence, not a runtime sequence diagram.

## Use cases

### Render a semantic tree in a Blazor component

1. Build a Core `GuiNode` tree.
2. Call `BlazorGuiRenderer.Render(node)` to obtain a `RenderFragment`.
3. Render the fragment from a Blazor component.
4. Let Blazor execute the fragment into its render tree.

**Expected result:** supported nodes are expressed as corresponding HTML elements, with identity, source, properties, text, and children applied according to the renderer contract.

### Reject malformed identity or excessive nesting

1. Supply a tree with repeated IDs or depth beyond 256.
2. Call `Render`.
3. Observe the validation exception before the fragment is returned.

**Expected result:** malformed trees are rejected before Blazor render-tree construction.

## Explicit limits

- This class does not own application state, routing, persistence, or host lifecycle.
- Unknown kinds currently fall back to `div`; this is not a guarantee of semantic equivalence.
- The renderer does not define an allow-list for arbitrary properties or URL schemes. Treat model input as untrusted until the consuming application applies its policy.
- See the [authored API contract](../api/Blazor/BlazorGuiRenderer.md), [implementation](../../src/Blazor/BlazorGuiRenderer.cs), and [tests](../../tests/Blazor.Tests/BlazorGuiRendererTests.cs).

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
