# SvgGuiRenderer.cs.cs

- **Project:** SVG
- **Source:** [`src/SVG/SvgGuiRenderer.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/SVG/SvgGuiRenderer.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Manifests a platform-neutral GUI tree as a self-contained SVG document.

## Source-grounded facts

- The source comments state that it does not execute FSM_API, run an animation clock, or claim to be a GPU renderer.
- Animation elements are explicit scene data; `CoordinateRange` is a nested helper type.

## Declared types and generated UML evidence

- `SvgGuiRenderer`
  - Generated type page: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgGuiRenderer-188b9d58e1.svg`
  - Generated SVG: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgGuiRenderer-188b9d58e1.svg`
- `SvgGuiRenderer.CoordinateRange`
  - Generated type page: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgGuiRenderer-CoordinateRange-9ab33e24ab.svg`
  - Generated SVG: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgGuiRenderer-CoordinateRange-9ab33e24ab.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
