# CoordinateSystem.cs.cs

- **Project:** Core
- **Source:** [`src/Core/CoordinateSystem.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/Core/CoordinateSystem.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Defines the numeric range and origin for a normalized two-dimensional coordinate space.

## Source-grounded facts

- The `CoordinateSystem` readonly record struct exposes `Contains`, panel/source coordinate conversions, and `DeltaX`/`DeltaY` helpers.

## Declared types and generated UML evidence

- `CoordinateSystem`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-CoordinateSystem-6370e2bc48.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-CoordinateSystem-6370e2bc48.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
