# GridGuiBuilderWPF.cs.cs

- **Project:** WPF
- **Source:** [`src/WPF/GridGuiBuilderWPF.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/WPF/GridGuiBuilderWPF.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Builds a resizable WPF `Grid` layout with configurable tracks, component placements, and splitter behavior.

## Source-grounded facts

- Public operations configure splitter thickness/style/resize behavior, add row/column tracks, and place components.
- Nested helper types capture grid dimension definitions, component placements, and compatibility proxy behavior.

## Declared types and generated UML evidence

- `GridGuiBuilderWPF`
  - Generated type page: `wpf/types/TheSingularityWorkshop-GUI-WPF-Layouts-GridGuiBuilderWPF-321cd88678.svg`
  - Generated SVG: `wpf/types/TheSingularityWorkshop-GUI-WPF-Layouts-GridGuiBuilderWPF-321cd88678.svg`
- `GridGuiBuilderWPF.CompatibilityElementProxy`
  - Generated type page: `wpf/types/TheSingularityWorkshop-GUI-WPF-Layouts-GridGuiBuilderWPF-CompatibilityElementProxy-f9eb9eb7a2.svg`
  - Generated SVG: `wpf/types/TheSingularityWorkshop-GUI-WPF-Layouts-GridGuiBuilderWPF-CompatibilityElementProxy-f9eb9eb7a2.svg`
- `GridGuiBuilderWPF.ComponentPlacement`
  - Generated type page: `wpf/types/TheSingularityWorkshop-GUI-WPF-Layouts-GridGuiBuilderWPF-ComponentPlacement-f821509b69.svg`
  - Generated SVG: `wpf/types/TheSingularityWorkshop-GUI-WPF-Layouts-GridGuiBuilderWPF-ComponentPlacement-f821509b69.svg`
- `GridGuiBuilderWPF.GridDimensionDefinition`
  - Generated type page: `wpf/types/TheSingularityWorkshop-GUI-WPF-Layouts-GridGuiBuilderWPF-GridDimensionDefinition-b59be17753.svg`
  - Generated SVG: `wpf/types/TheSingularityWorkshop-GUI-WPF-Layouts-GridGuiBuilderWPF-GridDimensionDefinition-b59be17753.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
