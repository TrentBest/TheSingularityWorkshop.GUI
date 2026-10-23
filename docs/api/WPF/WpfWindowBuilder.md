# WpfWindowBuilder — API contract

**User moniker:** Trent Best

`WpfWindowBuilder` is a fluent wrapper around a single WPF `Window`. Each configuration method returns the same builder; `Build()` returns its configured window instance.

## Configuration surface

| Member | Behavior |
|---|---|
| `WithTitle(string)` | Sets the title; rejects null. |
| `WithSize(double, double)` | Requires positive, finite width and height; selects manual sizing. |
| `WithContent(FrameworkElement)` | Sets root content; rejects null. The element must not already have a visual parent. |
| `OwnedBy(Window)` | Sets the owner; rejects null. |
| `WithStartupLocation(WindowStartupLocation)` | Sets startup placement policy. |
| `WithResizeMode(ResizeMode)` | Sets WPF resize policy. |
| `WithWindowStyle(WindowStyle)` | Sets native window chrome style. |
| `WithBackground(Brush)` | Sets background; rejects null. |
| `WithIcon(ImageSource)` | Sets icon; rejects null. |
| `ShowInTaskbar(bool)` | Controls taskbar representation. |
| `Build()` | Returns the configured window without showing it. |

## Lifecycle boundary

The builder does not start an application, display the window, run a dispatcher, or manage window lifetime. The caller must follow WPF threading and visual-parent rules. Invalid size arguments throw `ArgumentOutOfRangeException`; null arguments to guarded methods throw `ArgumentNullException`.

## Class diagram and use cases

See [WpfWindowBuilderUML.md](../../uml/WpfWindowBuilderUML.md) for the focused class card and use cases, plus the [compiler-derived relationship SVG](../../uml/source-files/WPF/TheSingularityWorkshop-GUI-WPF-WpfWindowBuilder-8f86ce64e7.svg).
