# WpfGuiRenderer — API contract

**User moniker:** Trent Best

`WpfGuiRenderer` is the static WPF adapter for `TheSingularityWorkshop.Workshop.Gui.GuiNode`. It produces a native `FrameworkElement`; it does not own the application, window lifetime, persistence, or authorization.

## Public entry points

| Member | Contract |
|---|---|
| `Render(GuiNode node)` | Validates and renders a tree without wiring host commands. |
| `Render(GuiNode node, Action<string> commandDispatcher)` | Validates and renders a tree; a Button with a nonblank `onClickCommand` property forwards that ID to the supplied callback on click. |

## Validation and failure behavior

- Null arguments are rejected with `ArgumentNullException`.
- Duplicate node IDs in a tree are rejected with `ArgumentException`.
- Depth beyond 256 is rejected with `InvalidOperationException` before control construction.
- Unknown node kinds use a simple container fallback rather than failing.
- Image-loading failures in the explicitly handled exception set become visible placeholder text; this is not a guarantee that every possible WPF imaging failure is recoverable.

## Security and host responsibilities

The renderer validates structural identity and depth; it does not sanitize all semantic properties or authorize commands. Treat manifests and node properties as input data. The host owns command allow-listing, authorization, data access, and application-level validation. A command ID is a request to the host, never proof of permission.

## Class diagram and use cases

See [WpfGuiRendererUML.md](../../uml/WpfGuiRendererUML.md) for the focused class card, relationships, and use cases. The [compiler-derived UML asset](../../uml/source-files/WPF/TheSingularityWorkshop-GUI-WPF-WpfGuiRenderer-09d386773a.svg) records the source relationship neighborhood; it complements rather than replaces this human-authored contract.
