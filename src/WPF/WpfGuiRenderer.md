<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# WpfGuiRenderer

**User moniker:** Trent Best  
**Purpose:** Adapt the renderer-neutral `GuiNode` tree into native Windows Presentation Foundation controls.

## Contract

`WpfGuiRenderer.Render(GuiNode)` validates the complete tree before creating controls. The overload accepting `Action<string>` also wires declared button activation commands to the supplied host dispatcher.

- A null root (or null dispatcher for the overload) throws `ArgumentNullException`.
- Node IDs must be unique within the rendered tree; duplicates throw `ArgumentException`.
- Tree depth is limited to 256 edges from the root; excessive depth throws `InvalidOperationException`.
- Known kinds map to native WPF controls: Button, Text, Column, Panel, Warning, and Image. Unknown kinds fall back to a simple bordered vertical content panel.
- Children are rendered in source order. Buttons with children receive a vertical content panel containing their label and rendered children.
- Node identity is preserved through `WpfGuiIdentity`; a legal WPF `Name` is also assigned where possible.
- Invalid or unsupported image sources degrade to visible placeholder text for the handled URI, decoding, and WPF image exceptions.
- The renderer is an adapter, not an application host: it does not create a window, start a dispatcher, persist state, or define domain authorization.

## Command and trust boundary

The command-dispatch overload forwards a nonblank `onClickCommand` property from a Button to the host callback. The callback is an integration boundary, **not** an authorization system. Hosts must map command IDs through an explicit allow-list and perform authorization at the point of action. Do not treat arbitrary manifest properties as trusted commands.

## UML class card

See [WpfGuiRenderer UML and use cases](../../docs/uml/WpfGuiRendererUML.md), alongside the [compiler-derived relationship diagram](../../docs/uml/source-files/WPF/TheSingularityWorkshop-GUI-WPF-WpfGuiRenderer-09d386773a.svg).

## Practical use case

A Windows host obtains a `GuiNode` tree from its application layer, calls `Render`, and places the returned `FrameworkElement` inside a window or panel. If buttons expose command IDs, use the dispatcher overload and resolve each ID against host-owned command policy. The same semantic tree remains independent of WPF until this adapter boundary.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
