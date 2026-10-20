# DataCard.cs.cs

- **Project:** Core
- **Source:** [`src/Core/DataCard.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/Core/DataCard.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Represents an immutable, renderer-neutral card in a structured diagram.

## Source-grounded facts

- The card stores identity, kind, title/subtitle, expansion defaults, and child-insertion affordance state.
- The SVG card renderer consumes this model; the model itself does not parse source or authorize domain edits.

## Declared types and generated UML evidence

- `DataCard`
  - Generated type page: `core/types/TheSingularityWorkshop-Workshop-Gui-DataCard-aa6db0751e.svg`
  - Generated SVG: `core/types/TheSingularityWorkshop-Workshop-Gui-DataCard-aa6db0751e.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
