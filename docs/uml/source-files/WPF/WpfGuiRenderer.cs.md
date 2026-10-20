# WpfGuiRenderer.cs.cs

- **Project:** WPF
- **Source:** [`src/WPF/WpfGuiRenderer.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/WPF/WpfGuiRenderer.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Recursively manifests a platform-neutral GUI node tree as native WPF controls.

## Source-grounded facts

- Core remains unaware of WPF; unknown node kinds degrade to a simple content panel.

## Declared types and generated UML evidence

- `WpfGuiRenderer`
  - Generated type page: `wpf/types/TheSingularityWorkshop-GUI-WPF-WpfGuiRenderer-09d386773a.svg`
  - Generated SVG: `wpf/types/TheSingularityWorkshop-GUI-WPF-WpfGuiRenderer-09d386773a.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
