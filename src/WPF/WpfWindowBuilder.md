<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# WpfWindowBuilder

**User moniker:** Trent Best  
**Purpose:** Fluent configuration of one native WPF `Window`, without taking over the application's lifetime.

## Contract

A new builder creates one window with default dimensions 700 × 500, centered startup placement, resizable single-border chrome, taskbar presence, and the system window brush.

Fluent methods configure title, size, content, owner, startup location, resize mode, window style, background, icon, and taskbar visibility. `Build()` returns the same configured window instance.

- `WithTitle`, `WithContent`, `OwnedBy`, `WithBackground`, and `WithIcon` reject null values.
- `WithSize` requires both dimensions to be finite and greater than zero; otherwise it throws `ArgumentOutOfRangeException`.
- The content element must not already belong to another visual parent.
- `Build()` does not call `Show()`, create an `Application`, start a dispatcher, or dispose the window. The host controls all of those lifecycle decisions.

## UML class card

See [WpfWindowBuilder UML and use cases](../../docs/uml/WpfWindowBuilderUML.md) and the [compiler-derived relationship diagram](../../docs/uml/source-files/WPF/TheSingularityWorkshop-GUI-WPF-WpfWindowBuilder-8f86ce64e7.svg).

## Practical use case

Render a semantic tree with `WpfGuiRenderer`, supply the resulting element to `WithContent`, configure the window title and size, then call `Build()`. The application decides when and how to show the returned window, including whether it is owned or modal.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
