# WorkshopGuiBuilder.cs.cs

- **Project:** Blazor
- **Source:** [`src/Blazor/WorkshopGuiBuilder.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/Blazor/WorkshopGuiBuilder.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Provides factory methods and the base element builder used to compose Blazor UI fragments.

## Source-grounded facts

- Factories create HTML elements, panels, multi-panels, buttons, and images.
- `ElementBuilder` supports attributes, ARIA labels, title, tabindex, CSS class/style, normalized position, text, and children.

## Declared types and generated UML evidence

- `ElementBuilder`
  - Generated type page: `blazor/types/TheSingularityWorkshop-Gui-ElementBuilder-f81e2f3925.svg`
  - Generated SVG: `blazor/types/TheSingularityWorkshop-Gui-ElementBuilder-f81e2f3925.svg`
- `WorkshopGui`
  - Generated type page: `blazor/types/TheSingularityWorkshop-Gui-WorkshopGui-85b7f742aa.svg`
  - Generated SVG: `blazor/types/TheSingularityWorkshop-Gui-WorkshopGui-85b7f742aa.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
