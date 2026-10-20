# WpfWindowBuilder.cs.cs

- **Project:** WPF
- **Source:** [`src/WPF/WpfWindowBuilder.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/WPF/WpfWindowBuilder.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Builds a native WPF window around adapter-created or host-supplied content.

## Source-grounded facts

- Fluent operations configure title, size, content, owner, startup location, resize mode, style, background, icon, and taskbar visibility.
- `Build` does not show the window or start an application; window lifetime remains host-controlled.

## Declared types and generated UML evidence

- `WpfWindowBuilder`
  - Generated type page: `wpf/types/TheSingularityWorkshop-GUI-WPF-WpfWindowBuilder-8f86ce64e7.svg`
  - Generated SVG: `wpf/types/TheSingularityWorkshop-GUI-WPF-WpfWindowBuilder-8f86ce64e7.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
