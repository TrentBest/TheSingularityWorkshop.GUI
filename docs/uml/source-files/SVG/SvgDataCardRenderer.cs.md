# SvgDataCardRenderer.cs.cs

- **Project:** SVG
- **Source:** [`src/SVG/SvgDataCardRenderer.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/SVG/SvgDataCardRenderer.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Renders a generic `DataCard` hierarchy as a static, inspectable SVG diagram.

## Source-grounded facts

- The renderer consumes an already-built card model; it does not parse source code or authorize edits.
- The edit-mode flag controls whether a permitted insertion affordance is shown, not domain authority.

## Declared types and generated UML evidence

- `SvgDataCardRenderer`
  - Generated type page: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgDataCardRenderer-574471487a.svg`
  - Generated SVG: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgDataCardRenderer-574471487a.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
